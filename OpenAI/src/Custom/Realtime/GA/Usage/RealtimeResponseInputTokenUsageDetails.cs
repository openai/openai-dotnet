using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseUsageInputTokenDetailsGA")]
public partial class RealtimeResponseInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("CachedTokens")]
    public int? CachedTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("TextTokens")]
    public int? TextTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("ImageTokens")]
    public int? ImageTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("AudioTokens")]
    public int? AudioTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("CachedTokensDetails")]
    public RealtimeResponseInputCachedTokenUsageDetails CachedTokenDetails { get; set; }
}