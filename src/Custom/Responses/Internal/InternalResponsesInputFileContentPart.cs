using System;

namespace OpenAI.Responses;

[CodeGenType("ResponsesInputContentFile")]
internal partial class InternalResponsesInputFileContentPart
{
    // CUSTOM:
    // - Renamed.
    // - Changed type from string to BinaryData.
    [CodeGenMember("FileData")]
    public BinaryData FileBytes { get; set; }
}