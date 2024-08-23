using System;

namespace OpenAI.Internal;

[CodeGenModel("OmniTypedResponseFormat")] internal partial class InternalOmniTypedResponseFormat { }
[CodeGenModel("ResponseFormatJsonObject")] internal partial class InternalResponseFormatJsonObject { }
[CodeGenModel("ResponseFormatJsonSchema")] internal partial class InternalResponseFormatJsonSchema { }
[CodeGenModel("ResponseFormatJsonSchemaJsonSchema")] internal partial class InternalResponseFormatJsonSchemaJsonSchema { [CodeGenMember("Schema")] public BinaryData Schema { get; set; } }
[CodeGenModel("ResponseFormatJsonSchemaSchema")] internal partial class InternalResponseFormatJsonSchemaSchema { }
[CodeGenModel("ResponseFormatText")] internal partial class InternalResponseFormatText { }
[CodeGenModel("UnknownOmniTypedResponseFormat")] internal partial class InternalUnknownOmniTypedResponseFormat { }
