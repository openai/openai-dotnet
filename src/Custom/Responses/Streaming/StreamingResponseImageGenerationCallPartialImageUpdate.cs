using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseImageGenCallPartialImageEvent")]
public partial class StreamingResponseImageGenerationCallPartialImageUpdate
{
    // CUSTOM: Renamed.
    [CodeGenMember("PartialImageB64")]
    public BinaryData PartialImageBytes { get; }
}
