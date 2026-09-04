using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.TypeSpec.Generator.Snippets.Snippet;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Adds access to serialized additional data and makes its backing field mutable on mutable base models.
/// </summary>
public class AdditionalPropertiesVisitor : ScmLibraryVisitor
{
    private const string RawDataPropertyName = "SerializedAdditionalRawData";
    private const string AdditionalPropertiesFieldName = "_additionalBinaryDataProperties";

    protected override TypeProvider VisitType(TypeProvider type)
    {
        var additionalPropertiesField = type.Fields.FirstOrDefault(f => f.Name == AdditionalPropertiesFieldName);
        if (type is ModelProvider { BaseModelProvider: null } && additionalPropertiesField != null)
        {
            var properties = new List<PropertyProvider>(type.Properties)
            {
                new PropertyProvider($"", MethodSignatureModifiers.Internal,
                    typeof(IDictionary<string, BinaryData>), RawDataPropertyName,
                    new ExpressionPropertyBody(
                        additionalPropertiesField,
                        type.DeclarationModifiers.HasFlag(TypeSignatureModifiers.ReadOnly) ? null : additionalPropertiesField.Assign(Value)),
                    type)
            };

            type.Update(properties: properties);
        }

        return type;
    }

    protected override FieldProvider VisitField(FieldProvider field)
    {
        if (field.Name == AdditionalPropertiesFieldName && !field.EnclosingType.DeclarationModifiers.HasFlag(TypeSignatureModifiers.ReadOnly))
        {
            field.Modifiers &= ~FieldModifiers.ReadOnly;
        }

        return field;
    }
}
