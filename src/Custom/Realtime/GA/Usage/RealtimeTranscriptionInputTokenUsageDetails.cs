using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("TranscriptTextUsageTokensInputTokenDetailsGA")]
public partial class GARealtimeTranscriptionInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("TextTokens")]
    public int? TextTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("AudioTokens")]
    public int? AudioTokenCount { get; }
}
