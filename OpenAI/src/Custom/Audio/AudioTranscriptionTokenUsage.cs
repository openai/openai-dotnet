using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
[CodeGenType("TranscriptTextUsageTokens")]
public partial class AudioTranscriptionTokenUsage
{
    // CUSTOM: Renamed.
    [CodeGenMember("InputTokens")]
    public int InputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputTokens")]
    public int OutputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("TotalTokens")]
    public int TotalTokenCount { get; }
}
