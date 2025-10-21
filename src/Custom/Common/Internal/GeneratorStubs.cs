using System;

namespace OpenAI.Internal;

[CodeGenType("ResponseFormat")] internal partial class InternalResponseFormat { }
[CodeGenType("ResponseFormatType")] internal readonly partial struct InternalResponseFormatType
{
    public static implicit operator InternalResponseFormatType(ResponseFormatType2 kind)
    {
        return kind switch
        {
            ResponseFormatType2.Text => Text,
            ResponseFormatType2.JsonObject => JsonObject,
            ResponseFormatType2.JsonSchema => JsonSchema,
            _ => throw new InvalidOperationException("Unknown response format type")
        };
    }
}
[CodeGenType("ResponseFormatJsonObject")] internal partial class InternalResponseFormatJsonObject { }
[CodeGenType("ResponseFormatJsonSchema")] internal partial class InternalResponseFormatJsonSchema { }
[CodeGenType("ResponseFormatJsonSchemaSchema")] internal partial class InternalResponseFormatJsonSchemaSchema { }
[CodeGenType("ResponseFormatText")] internal partial class InternalResponseFormatText { }
[CodeGenType("UnknownResponseFormat")] internal partial class InternalUnknownResponseFormat { }
[CodeGenType("WebSearchLocation")] internal partial class InternalWebSearchLocation { }
[CodeGenType("WebSearchContextSize")] internal readonly partial struct InternalWebSearchContextSize { }
[CodeGenType("LogProbProperties")] internal partial class InternalLogProbProperties { }
[CodeGenType("ModelIdsShared")] internal readonly partial struct InternalModelIdsShared { }
[CodeGenType("VoiceIdsShared")] internal readonly partial struct InternalVoiceIdsShared { }