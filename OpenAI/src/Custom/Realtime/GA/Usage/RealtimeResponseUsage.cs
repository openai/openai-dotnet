using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseUsageGA")]
public partial class RealtimeResponseUsage
{
    // CUSTOM: Renamed.
    [CodeGenMember("TotalTokens")]
    public int? TotalTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("InputTokens")]
    public int? InputTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputTokens")]
    public int? OutputTokenCount { get; set; }
}