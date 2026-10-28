using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Exposes extensible discriminated bases in explicitly opted-in namespaces.
/// Eligible models are public classes with an unfixed discriminator identified by generator metadata,
/// backed by a string extensible enum. Ordinary models, fixed leaves, and closed enums are unchanged.
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
        PropertyProvider? discriminator = null;
        ModelProvider? discriminatorModel = model;
        for (; discriminatorModel is not null; discriminatorModel = discriminatorModel.BaseModelProvider)
        {
            discriminator = discriminatorModel.Properties.FirstOrDefault(property => property.IsDiscriminator);
            if (discriminator is not null)
            {
                break;
            }
        }

        if (discriminator is null)
        {
            // A handwritten property can suppress the generated property list while constructor parameters
            // still retain the original discriminator metadata. Use that metadata as the fallback.
            for (discriminatorModel = model; discriminatorModel is not null; discriminatorModel = discriminatorModel.BaseModelProvider)
            {
                discriminator = discriminatorModel.Constructors.SelectMany(constructor => constructor.Signature.Parameters)
                    .Select(parameter => parameter.Property)
                    .FirstOrDefault(property => property?.IsDiscriminator == true);
                if (discriminator is not null)
                {
                    break;
                }
            }
        }

        // Both the model and discriminator owner must opt in. Only string-backed extensible enums
        // can accept consumer-defined values; raw strings, numeric enums, and closed enums are unchanged.
        if (discriminator is null || discriminatorModel is null
            || !_extensibleModelNamespaces.Contains(discriminatorModel.Type.Namespace)
            || !ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap.TryGetValue(discriminator.Type, out var discriminatorType)
            || discriminatorType is not EnumProvider { IsExtensible: true } kind
            || !kind.EnumUnderlyingType.Equals(typeof(string)))
        {
            return type;
        }

        // Do not replace an intentional handwritten contract (including a renamed original member).
        // A compatible public get-only customization can still use the generated initialization constructor.
        PropertyProvider? customDiscriminator = discriminatorModel.CustomCodeView?.Properties
            .FirstOrDefault(property => property.Name == discriminator.Name || property.OriginalName == discriminator.Name);
        if (customDiscriminator is not null
            && (!customDiscriminator.Modifiers.HasFlag(MethodSignatureModifiers.Public)
                || !customDiscriminator.Type.Equals(discriminator.Type)
                || customDiscriminator.Body is AutoPropertyBody { HasSetter: true }))
        {
            return type;
        }

        const TypeSignatureModifiers typeVisibility = TypeSignatureModifiers.Public | TypeSignatureModifiers.Internal;
        if (!kind.Type.IsPublic)
        {
            CSharpType previousKindType = kind.Type;
            // Promotion rebuilds the cached CSharpType. Retain customized members, attributes, and output
            // location; in particular, making the enum public must not expose intentionally internal members.
            kind.Update(
                modifiers: kind.DeclarationModifiers & ~typeVisibility | TypeSignatureModifiers.Public,
                properties: kind.Properties,
                fields: kind.Fields,
                methods: kind.Methods,
                constructors: kind.Constructors,
                serializations: kind.SerializationProviders,
                nestedTypes: kind.NestedTypes,
                attributes: kind.Attributes,
                implements: kind.Implements,
                xmlDocs: kind.XmlDocs,
                relativeFilePath: kind.RelativeFilePath,
                reset: true);
            // Update references to the rebuilt type, including static enum members returning their own type.
            ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap[kind.Type] = kind;
            foreach (PropertyProvider member in kind.Properties.Where(property => property.Type.Equals(previousKindType)))
            {
                member.Update(type: kind.Type);
            }
        }

        // Only the declaring model owns this property. Do not mutate an inherited property here:
        // its own visitor pass may already have applied an explicit visibility override.
        // Preserve initialization while removing a protocol-model setter and exposing the public kind type.
        if (ReferenceEquals(discriminatorModel, model) && discriminator.Body is AutoPropertyBody body)
        {
            discriminator.Update(
                modifiers: discriminator.Modifiers
                    & ~(MethodSignatureModifiers.Public | MethodSignatureModifiers.Internal
                        | MethodSignatureModifiers.Protected | MethodSignatureModifiers.Private)
                    | MethodSignatureModifiers.Public,
                type: kind.Type,
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

                // An open intermediate base needs its inherited kind in the initialization signature
                // so generated base-constructor forwarding can pass the consumer's value to the root.
                constructor.Signature.Update(parameters: [discriminator.AsParameter, .. constructor.Signature.Parameters]);
            }

            // Allow subclasses in other assemblies to initialize a kind without exposing serialization state.
            // Use the promoted enum type on discriminator parameters; no defaults or extra overloads are added.
            constructor.Signature.Update(modifiers: MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal);
            foreach (ParameterProvider parameter in constructor.Signature.Parameters.Where(parameter => parameter.Property?.IsDiscriminator == true))
            {
                parameter.Update(type: kind.Type);
            }
        }

        return type;
    }
}
