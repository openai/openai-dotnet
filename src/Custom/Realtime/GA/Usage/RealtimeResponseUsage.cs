using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseUsageGA")]
public partial class RealtimeResponseUsage
{
    // CUSTOM: Renamed.
    [CodeGenMember("TotalTokens")]
    public int? TotalTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("InputTokens")]
    public int? InputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputTokens")]
    public int? OutputTokenCount { get; }
}