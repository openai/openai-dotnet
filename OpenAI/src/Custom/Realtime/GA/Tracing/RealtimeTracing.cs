using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetRealtimeTracingGA")]
[CodeGenVisibility(nameof(RealtimeTracing), CodeGenVisibility.Internal)]
public partial class RealtimeTracing
{
    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeTracing(RealtimeDefaultTracing defaultTracing)
    {
        DefaultTracing = defaultTracing;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeTracing(RealtimeCustomTracing customTracing)
    {
        Argument.AssertNotNull(customTracing, nameof(customTracing));

        CustomTracing = customTracing;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultTracing")]
    public RealtimeDefaultTracing? DefaultTracing { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomTracing")]
    public RealtimeCustomTracing CustomTracing { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator RealtimeTracing(RealtimeDefaultTracing defaultTracing) => new(defaultTracing);

    // CUSTOM: Added for convenience.
    public static implicit operator RealtimeTracing(RealtimeCustomTracing customTracing) => customTracing is null ? null : new(customTracing);
}
