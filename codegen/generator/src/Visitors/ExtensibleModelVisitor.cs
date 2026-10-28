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
        if (type is not ModelProvider model
            || !model.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Public)
            || model.Type.IsValueType
            || model.DiscriminatorValue is not null
            || model.IsUnknownDiscriminatorModel
            || !_extensibleModelNamespaces.Contains(model.Type.Namespace))
        {
            return type;
        }

        // A discriminator may be inherited by another open base in the hierarchy.
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

        if (discriminator is null || discriminatorModel is null
            || !_extensibleModelNamespaces.Contains(discriminatorModel.Type.Namespace)
            || !ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap.TryGetValue(discriminator.Type, out var discriminatorType)
            || discriminatorType is not EnumProvider { IsExtensible: true } kind
            || !kind.EnumUnderlyingType.Equals(typeof(string)))
        {
            return type;
        }

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
            // Rebuild the cached CSharpType, but retain member customizations already applied to the enum.
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
            ScmCodeModelGenerator.Instance.TypeFactory.CSharpTypeMap[kind.Type] = kind;
            foreach (PropertyProvider member in kind.Properties.Where(property => property.Type.Equals(previousKindType)))
            {
                member.Update(type: kind.Type);
            }
        }

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

        // Only change initialization constructors, never the full/serialization constructor (JsonPatch).
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

                constructor.Signature.Update(parameters: [discriminator.AsParameter, .. constructor.Signature.Parameters]);
            }

            constructor.Signature.Update(modifiers: MethodSignatureModifiers.Protected | MethodSignatureModifiers.Internal);
            foreach (ParameterProvider parameter in constructor.Signature.Parameters.Where(parameter => parameter.Property?.IsDiscriminator == true))
            {
                parameter.Update(type: kind.Type);
            }
        }

        return type;
    }
}
