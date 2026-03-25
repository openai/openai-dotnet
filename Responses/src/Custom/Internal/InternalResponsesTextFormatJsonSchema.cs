using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

[CodeGenType("ResponseTextFormatConfigurationJsonSchema")]
internal partial class InternalResponsesTextFormatJsonSchema
{
    // CUSTOM: Make schema type a plain BinaryData instance.
    [CodeGenMember("Schema")]
    internal BinaryData Schema { get; set; }
}