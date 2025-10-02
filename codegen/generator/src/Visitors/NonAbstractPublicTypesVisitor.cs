using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// A visitor that removes a generated 'abstract' modifier if a custom code definition exists for the type and it
/// doesn't provide 'abstract'.
/// </summary>
public class NonAbstractPublicTypesVisitor : ScmLibraryVisitor
{
    protected override TypeProvider VisitType(TypeProvider type)
    {
        if (type.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Public)
            && type.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Abstract)
            && type.CustomCodeView?.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Abstract) == false)
        {
            var serializationProviders = type.SerializationProviders;

            // ensure we build the serialization attributes before we change the modifiers
            foreach (var serialization in serializationProviders)
            {
                _ = serialization.Attributes;
            }

            // Keep types defined in custom code without 'abstract' non-abstract
            type.Update(modifiers: type.DeclarationModifiers & ~TypeSignatureModifiers.Abstract);

            // reset the serialization to pick up the new modifiers while keeping any serialization attributes
            foreach (var serialization in serializationProviders)
            {
                serialization.Update(attributes: serialization.Attributes, reset: true);
            }
        }
        return type;
    }
}