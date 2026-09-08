using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponseTextFormatConfiguration")]
public partial class ResponseTextFormat
{
    // CUSTOM: Renamed to "Kind".
    [CodeGenMember("Type")]
    public ResponseTextFormatKind Kind { get; set; }

    public static ResponseTextFormat CreateTextFormat() => new InternalResponsesTextFormatText();

    public static ResponseTextFormat CreateJsonObjectFormat() => new InternalResponsesTextFormatJsonObject();

    public static ResponseTextFormat CreateJsonSchemaFormat(string jsonSchemaFormatName, BinaryData jsonSchema, string jsonSchemaFormatDescription = null, bool? jsonSchemaIsStrict = null)
    {
        Argument.AssertNotNullOrEmpty(jsonSchemaFormatName, nameof(jsonSchemaFormatName));
        Argument.AssertNotNull(jsonSchema, nameof(jsonSchema));

         return new InternalResponsesTextFormatJsonSchema(
            ResponseTextFormatKind.JsonSchema,
            patch: default,
            description: jsonSchemaFormatDescription,
            name: jsonSchemaFormatName,
            strict: jsonSchemaIsStrict,
            schema: jsonSchema);
    }
}