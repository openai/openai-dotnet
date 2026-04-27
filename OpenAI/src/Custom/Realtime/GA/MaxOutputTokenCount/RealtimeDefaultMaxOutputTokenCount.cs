using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("DotNetRealtimeDefaultMaxOutputTokenCountGA")]
public readonly partial struct RealtimeDefaultMaxOutputTokenCount
{
    // CUSTOM: Renamed.
    [CodeGenMember("Inf")]
    public static RealtimeDefaultMaxOutputTokenCount Infinity { get; } = new RealtimeDefaultMaxOutputTokenCount(InfValue);
}