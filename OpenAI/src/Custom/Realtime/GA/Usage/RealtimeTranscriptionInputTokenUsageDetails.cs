using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("TranscriptTextUsageTokensInputTokenDetailsGA")]
public partial class RealtimeTranscriptionInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("TextTokens")]
    public int? TextTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("AudioTokens")]
    public int? AudioTokenCount { get; set; }
}
