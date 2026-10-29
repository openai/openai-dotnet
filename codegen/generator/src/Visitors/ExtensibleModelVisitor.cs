using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Exposes extensible discriminated bases in explicitly opted-in namespaces.
/// Eligible models are public classes whose discriminator property is not constrained to one value
/// (generator metadata has no DiscriminatorValue), unlike a concrete leaf such as type = "message".
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
        // Only opted-in public reference models without a fixed discriminator can be extended.
        // Concrete leaves and generated unknown-value fallbacks retain their normal codegen behavior.
        if (type is not ModelProvider model
            || !model.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Public)
            || model.Type.IsValueType
            || model.DiscriminatorValue is not null
            || model.IsUnknownDiscriminatorModel
            || !_extensibleModelNamespaces.Contains(model.Type.Namespace))
        {
            return type;
        }

        // Find the declaring model through metadata rather than assuming a CLR property name.
        // An open intermediate base may inherit its discriminator from another eligible base.
        // The owner is the model declaring the property, not necessarily the model currently being visited.
        PropertyProvider? discriminatorProperty = null;
        ModelProvider? discriminatorPropertyOwner = model;
        for (; discriminatorPropertyOwner is not null; discriminatorPropertyOwner = discriminatorPropertyOwner.BaseModelProvider)
        {
            discriminatorProperty = discriminatorPropertyOwner.Properties.FirstOrDefault(property => property.IsDiscriminator);
            if (discriminatorProperty is not null)
            {
                break;
            }
        }

        if (discriminatorProperty is null)
        {
            // A handwritten property can suppress the generated property list while constructor parameters
            // still retain the original discriminator metadata. Use that metadata as the fallback.
            for (discriminatorPropertyOwner = model; discriminatorPropertyOwner is not null; discriminatorPropertyOwner = discriminatorPropertyOwner.BaseModelProvider)
            {
                discriminatorProperty = discriminatorPropertyOwner.Constructors.SelectMany(constructor => constructor.Signature.Parameters)
                    .Select(parameter => parameter.Property)
                    .FirstOrDefault(property => property?.IsDiscriminator == true);
                if (discriminatorProperty is not null)
                {
                    break;
                }
            }
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

        // Expose initialization constructors on eligible open bases, not fixed leaves (excluded above).
        // ResponseItem has only (kind); other bases can require more arguments, such as
        // StreamingResponseUpdate(kind, sequenceNumber). Preserve those arguments and all constructor bodies.
        // The full constructor carries serialization state such as JsonPatch and must remain unchanged.
        foreach (ConstructorProvider constructor in model.Constructors)
        {
            if (ReferenceEquals(constructor, model.FullConstructor))
            {
                continue;
            }

            if (!constructor.Signature.Parameters.Any(parameter => parameter.Property?.IsDiscriminator == true))
            {
                if (model.BaseModelProvider is null)
                {
                    continue;
                }

                // An open intermediate base needs its inherited discriminator parameter in the initialization signature
                // so generated base-constructor forwarding can pass the consumer's value to the root.
                constructor.Signature.Update(parameters: [discriminatorProperty.AsParameter, .. constructor.Signature.Parameters]);
                ConstructorInitializer? initializer = constructor.Signature.Initializer;
                constructor.Signature.Update(initializer: initializer is null
                    ? new ConstructorInitializer(true, [new PositionalParameterReferenceExpression(discriminatorProperty.AsParameter)])
                    : new ConstructorInitializer(initializer.IsBase,
                        [new PositionalParameterReferenceExpression(discriminatorProperty.AsParameter), .. initializer.Arguments]));
            }

            // Allow subclasses in other assemblies to initialize the discriminator property without exposing serialization state.
            constructor.Signature.Update(modifiers: MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal);
            foreach (ParameterProvider parameter in constructor.Signature.Parameters.Where(parameter => parameter.Property?.IsDiscriminator == true))
            {
                // Refresh the parameter's enum type after promotion; preserve its name, default, and metadata.
                parameter.Update(type: discriminatorType.Type);
            }
        }

        return type;
    }
}
