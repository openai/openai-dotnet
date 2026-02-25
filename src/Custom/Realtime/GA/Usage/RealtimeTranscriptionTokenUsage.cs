using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("TranscriptTextUsageTokensGA")]
public partial class GARealtimeTranscriptionTokenUsage
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
