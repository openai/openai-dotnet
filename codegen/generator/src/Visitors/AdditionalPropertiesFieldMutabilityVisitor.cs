using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Removes readonly from the generated additional-properties backing field on mutable models.
/// </summary>
public class AdditionalPropertiesFieldMutabilityVisitor : ScmLibraryVisitor
{
    protected override FieldProvider VisitField(FieldProvider field)
    {
        if (field.Name == SerializationVisitorHelpers.AdditionalPropertiesFieldName
            && !field.EnclosingType.DeclarationModifiers.HasFlag(TypeSignatureModifiers.ReadOnly))
        {
            field.Modifiers &= ~FieldModifiers.ReadOnly;
        }

        return field;
    }
}
