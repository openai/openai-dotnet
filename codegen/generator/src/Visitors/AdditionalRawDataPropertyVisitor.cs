using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.TypeSpec.Generator.Snippets.Snippet;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Adds an internal property exposing the generated additional-properties backing field on base models.
/// </summary>
public class AdditionalRawDataPropertyVisitor : ScmLibraryVisitor
{
    private const string RawDataPropertyName = "SerializedAdditionalRawData";

    protected override TypeProvider VisitType(TypeProvider type)
    {
        FieldProvider? additionalPropertiesField = type.Fields.FirstOrDefault(field => field.Name == SerializationVisitorHelpers.AdditionalPropertiesFieldName);

        if (type is ModelProvider { BaseModelProvider: null } && additionalPropertiesField != null)
        {
            List<PropertyProvider> properties =
            [
                .. type.Properties,
                new PropertyProvider(
                    "",
                    MethodSignatureModifiers.Internal,
                    typeof(IDictionary<string, BinaryData>),
                    RawDataPropertyName,
                    new ExpressionPropertyBody(
                        additionalPropertiesField,
                        type.DeclarationModifiers.HasFlag(TypeSignatureModifiers.ReadOnly) ? null : additionalPropertiesField.Assign(Value)),
                    type),
            ];

            type.Update(properties: properties);
        }

        return type;
    }
}
