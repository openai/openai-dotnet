using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseUsageInputTokenDetailsCachedTokensDetailsGA")]
public partial class RealtimeResponseInputCachedTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("TextTokens")]
    public int? TextTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("ImageTokens")]
    public int? ImageTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("AudioTokens")]
    public int? AudioTokenCount { get; }
}