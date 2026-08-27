using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
[CodeGenType("TranscriptTextUsageTokensInputTokenDetails")]
public partial class AudioTranscriptionInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("TextTokens")]
    public int? TextTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("AudioTokens")]
    public int? AudioTokenCount { get; }
}
