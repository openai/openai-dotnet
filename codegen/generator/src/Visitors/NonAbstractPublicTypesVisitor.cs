using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System.Reflection;

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
            // Keep types defined in custom code without 'abstract' non-abstract

            // To do: replace with this line when dependencies updated to include modifier support
            // type.Update(modifiers: type.DeclarationModifiers & ~TypeSignatureModifiers.Abstract);
            // To do: remove this reflection-based workaround for the above:
            FieldInfo privateModifiersInfo = typeof(TypeProvider)
                .GetField("_declarationModifiers", BindingFlags.Instance | BindingFlags.NonPublic)!;
            TypeSignatureModifiers privateValue = (TypeSignatureModifiers)privateModifiersInfo.GetValue(type)!;
            privateValue &= ~TypeSignatureModifiers.Abstract;
            privateModifiersInfo.SetValue(type, privateValue);
        }
        return type;
    }
}