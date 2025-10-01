using System;
using System.Collections;

namespace OpenAI.Responses;

[CodeGenType("ResponseTextFormatConfigurationJsonSchema")]
public partial class InternalResponsesTextFormatJsonSchema
{
    // CUSTOM: Make schema type a plain BinaryData instance.
    [CodeGenMember("Schema")]
    internal BinaryData Schema { get; set; }
}