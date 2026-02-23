using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseUsageInputTokenDetailsGA")]
public partial class GARealtimeResponseInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("CachedTokens")]
    public int? CachedTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("TextTokens")]
    public int? TextTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("ImageTokens")]
    public int? ImageTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("AudioTokens")]
    public int? AudioTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("CachedTokensDetails")]
    public GARealtimeResponseInputCachedTokenUsageDetails CachedTokenDetails { get; }
}