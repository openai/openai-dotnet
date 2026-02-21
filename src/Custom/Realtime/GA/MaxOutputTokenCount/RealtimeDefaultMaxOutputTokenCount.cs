using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("DotNetRealtimeDefaultMaxOutputTokenCountGA")]
public readonly partial struct GARealtimeDefaultMaxOutputTokenCount
{
    // CUSTOM: Renamed.
    [CodeGenMember("Inf")]
    public static GARealtimeDefaultMaxOutputTokenCount Infinity { get; } = new GARealtimeDefaultMaxOutputTokenCount(InfValue);
}