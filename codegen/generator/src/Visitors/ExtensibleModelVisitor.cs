using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Exposes extensible discriminated bases in explicitly opted-in namespaces.
/// Eligible models are public classes with an open discriminator, including models that fix an
/// inherited discriminator while declaring a new one for a nested hierarchy.
/// The discriminator property is identified by generator metadata and its type must be
/// a string-backed extensible enum. Ordinary models, fixed leaves, and closed enums are unchanged.
/// Explicit custom discriminator members with a different visibility, type, or setter remain authoritative.
/// Keep this allowlist separate from protocol models: protocol setters do not imply extensibility.
/// This visitor never changes wire names or fixed discriminator values.
/// </summary>
public class ExtensibleModelVisitor : ScmLibraryVisitor
{
    private static readonly HashSet<string> _extensibleModelNamespaces = new(StringComparer.OrdinalIgnoreCase)
    {
        "OpenAI.Responses",
    };

    protected override TypeProvider? PostVisitType(TypeProvider type)
    {
        // Only opted-in public reference models can be extended.
        // Concrete leaves and generated unknown-value fallbacks retain their normal codegen behavior.
        if (type is not ModelProvider model
            || !model.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Public)
            || model.Type.IsValueType
            || model.IsUnknownDiscriminatorModel
            || !_extensibleModelNamespaces.Contains(model.Type.Namespace))
        {
            return type;
        }

        // Find the declaring model through metadata rather than assuming a CLR property name.
        // An open intermediate base may inherit its discriminator from another eligible base.
        // The owner is the model declaring the property, not necessarily the model currently being visited.
        PropertyProvider? discriminatorProperty = FindDiscriminatorProperty(model, out ModelProvider? discriminatorPropertyOwner);

        // A fixed value belongs to the inherited layer only when this model declares a new discriminator.
        // Fixed leaves (including standalone fixed models) must not gain initialization constructors.
        if (model.DiscriminatorValue is not null
            && (!ReferenceEquals(discriminatorPropertyOwner, model)
                || FindDiscriminatorProperty(model.BaseModelProvider, out _) is null))
        {
            return type;
        }

        // Both the model and discriminator owner must opt in. Only string-backed extensible enums
        // can accept consumer-defined values; raw strings, numeric enums, and closed enums are unchanged.
        if (discriminatorProperty is null || discriminatorPropertyOwner is null
            || !_extensibleModelNamespaces.Contains(discriminatorPropertyOwner.Type.Namespace)
            || !ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap.TryGetValue(discriminatorProperty.Type, out var discriminatorTypeProvider)
            || discriminatorTypeProvider is not EnumProvider { IsExtensible: true } discriminatorType
            || !discriminatorType.EnumUnderlyingType.Equals(typeof(string)))
        {
            return type;
        }

        // Do not replace an intentional handwritten contract (including a renamed original member).
        // A compatible public get-only customization can still use the generated initialization constructor.
        PropertyProvider? customDiscriminatorProperty = discriminatorPropertyOwner.CustomCodeView?.Properties
            .FirstOrDefault(property => property.Name == discriminatorProperty.Name || property.OriginalName == discriminatorProperty.Name);
        if (customDiscriminatorProperty is not null
            && (!customDiscriminatorProperty.Modifiers.HasFlag(MethodSignatureModifiers.Public)
                || !customDiscriminatorProperty.Type.Equals(discriminatorProperty.Type)
                || customDiscriminatorProperty.Body is AutoPropertyBody { HasSetter: true }))
        {
            return type;
        }

        const TypeSignatureModifiers typeVisibility = TypeSignatureModifiers.Public | TypeSignatureModifiers.Internal;
        if (!discriminatorType.Type.IsPublic)
        {
            CSharpType previousDiscriminatorType = discriminatorType.Type;
            // Make the discriminator enum public. reset rebuilds its cached CSharpType so IsPublic changes.
            // It also clears cached provider state: pass that state back explicitly to preserve customized
            // members (including internal ones), attributes, interfaces, documentation, and output location.
            discriminatorType.Update(
                modifiers: discriminatorType.DeclarationModifiers & ~typeVisibility | TypeSignatureModifiers.Public,
                properties: discriminatorType.Properties,
                fields: discriminatorType.Fields,
                methods: discriminatorType.Methods,
                constructors: discriminatorType.Constructors,
                serializations: discriminatorType.SerializationProviders,
                nestedTypes: discriminatorType.NestedTypes,
                attributes: discriminatorType.Attributes,
                implements: discriminatorType.Implements,
                xmlDocs: discriminatorType.XmlDocs,
                relativeFilePath: discriminatorType.RelativeFilePath,
                reset: true);
            // Update references to the rebuilt type, including static enum members returning their own type.
            ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap[discriminatorType.Type] = discriminatorType;
            foreach (PropertyProvider member in discriminatorType.Properties.Where(property => property.Type.Equals(previousDiscriminatorType)))
            {
                // Point self-typed enum properties at the public CSharpType rather than its stale internal copy.
                member.Update(type: discriminatorType.Type);
            }
        }

        // Only the declaring model owns this property. Do not mutate an inherited property here:
        // its own visitor pass may already have applied an explicit visibility override.
        // AutoPropertyBody represents generated { get; set; } or { get; } accessors, rather than a custom body.
        // Only rewrite that known shape: make the property public/get-only, use the public enum type,
        // and preserve any initializer. Custom accessor implementations are not replaced.
        if (ReferenceEquals(discriminatorPropertyOwner, model) && discriminatorProperty.Body is AutoPropertyBody body)
        {
            discriminatorProperty.Update(
                modifiers: discriminatorProperty.Modifiers
                    & ~(MethodSignatureModifiers.Public | MethodSignatureModifiers.Internal
                        | MethodSignatureModifiers.Protected | MethodSignatureModifiers.Private)
                    | MethodSignatureModifiers.Public,
                type: discriminatorType.Type,
                body: new AutoPropertyBody(HasSetter: false, InitializationExpression: body.InitializationExpression));
        }

        // Expose initialization constructors on eligible open bases, retaining any fixed inherited kind.
        // ResponseItem has only (kind); other bases can require more arguments, such as
        // StreamingResponseUpdate(kind, sequenceNumber). Preserve required arguments and existing statements.
        // The full serialization constructor itself remains unchanged. A different constructor may delegate
        // to it, so update only the delegated discriminator argument and preserve its serialization state.
        foreach (ConstructorProvider constructor in model.Constructors)
        {
            if (ReferenceEquals(constructor, model.FullConstructor))
            {
                continue;
            }

            if (!constructor.Signature.Parameters.Any(parameter => ReferenceEquals(parameter.Property, discriminatorProperty)))
            {
                if (model.BaseModelProvider is null)
                {
                    continue;
                }

                // Add the selected discriminator, initializing a locally declared property in this body
                // or forwarding an inherited property through the existing constructor chain.
                constructor.Signature.Update(parameters: [discriminatorProperty.AsParameter, .. constructor.Signature.Parameters]);
                ConstructorInitializer? initializer = constructor.Signature.Initializer;
                ValueExpression discriminatorArgument = discriminatorProperty.AsParameter;
                if (ReferenceEquals(discriminatorPropertyOwner, model) && initializer?.IsBase != false)
                {
                    constructor.Update(bodyStatements: new MethodBodyStatements(
                    [
                        .. constructor.BodyStatements?.ToList() ?? [],
                        Snippet.This.Property(discriminatorProperty.Name).Assign(discriminatorArgument).Terminate(),
                    ]));
                }
                else if (initializer is null)
                {
                    initializer = new ConstructorInitializer(true, [discriminatorArgument]);
                }
                else if (initializer.IsBase)
                {
                    // The generator can already emit an argument for an inherited discriminator without
                    // declaring its parameter. Replace that slot rather than adding a second discriminator.
                    ConstructorProvider? baseConstructor = model.BaseModelProvider.Constructors.FirstOrDefault(candidate =>
                        !ReferenceEquals(candidate, model.BaseModelProvider.FullConstructor)
                        && candidate.Signature.Parameters.Any(parameter => ReferenceEquals(parameter.Property, discriminatorProperty))
                        && candidate.Signature.Parameters.Count == initializer.Arguments.Count);
                    if (baseConstructor is not null)
                    {
                        int index = baseConstructor.Signature.Parameters.ToList()
                            .FindIndex(parameter => ReferenceEquals(parameter.Property, discriminatorProperty));
                        ValueExpression[] arguments = [.. initializer.Arguments];
                        arguments[index] = discriminatorArgument;
                        initializer = new ConstructorInitializer(true, arguments);
                    }
                    else
                    {
                        initializer = new ConstructorInitializer(true, [discriminatorArgument, .. initializer.Arguments]);
                    }
                }
                else
                {
                    ConstructorProvider? fullConstructor = model.FullConstructor;
                    int discriminatorParameterIndex = -1;
                    if (fullConstructor is not null)
                    {
                        for (int i = 0; i < fullConstructor.Signature.Parameters.Count; i++)
                        {
                            if (ReferenceEquals(fullConstructor.Signature.Parameters[i].Property, discriminatorProperty))
                            {
                                discriminatorParameterIndex = i;
                                break;
                            }
                        }
                    }

                    if (discriminatorParameterIndex >= 0 && discriminatorParameterIndex < initializer.Arguments.Count)
                    {
                        ValueExpression[] arguments = [.. initializer.Arguments];
                        arguments[discriminatorParameterIndex] = discriminatorArgument;
                        initializer = new ConstructorInitializer(false, arguments);
                    }
                }

                // Apply the initializer chosen above to forward the inherited discriminator while preserving
                // every other argument in the existing base or delegated constructor call.
                constructor.Signature.Update(initializer: initializer);
            }

            // Allow subclasses in other assemblies to initialize the discriminator property without exposing serialization state.
            constructor.Signature.Update(modifiers: MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal);
            foreach (ParameterProvider parameter in constructor.Signature.Parameters.Where(parameter => ReferenceEquals(parameter.Property, discriminatorProperty)))
            {
                // Refresh the parameter's enum type after promotion; preserve its name, default, and metadata.
                parameter.Update(type: discriminatorType.Type);
            }
        }

        return type;
    }

    private static PropertyProvider? FindDiscriminatorProperty(ModelProvider? model, out ModelProvider? owner)
    {
        for (owner = model; owner is not null; owner = owner.BaseModelProvider)
        {
            ModelProvider currentOwner = owner;
            // Custom properties can suppress generated members. Check their constructor metadata before
            // searching an ancestor, and ignore inherited parameters in the full serialization constructor.
            PropertyProvider? property = owner.Properties.FirstOrDefault(property => property.IsDiscriminator)
                ?? owner.Constructors.SelectMany(constructor => constructor.Signature.Parameters)
                    .Select(parameter => parameter.Property)
                    .FirstOrDefault(property => property?.IsDiscriminator == true
                        && ReferenceEquals(property.EnclosingType, currentOwner));
            if (property is not null)
            {
                return property;
            }
        }

        return null;
    }
}
