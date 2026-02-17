using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetRealtimeTracingGA")]
[CodeGenVisibility(nameof(GARealtimeTracing), CodeGenVisibility.Internal)]
public partial class GARealtimeTracing
{
    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeTracing(GARealtimeDefaultTracing defaultTracing)
    {
        DefaultTracing = defaultTracing;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeTracing(GARealtimeCustomTracing customTracing)
    {
        Argument.AssertNotNull(customTracing, nameof(customTracing));

        CustomTracing = customTracing;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultTracing")]
    public GARealtimeDefaultTracing? DefaultTracing { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomTracing")]
    public GARealtimeCustomTracing CustomTracing { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeTracing(GARealtimeDefaultTracing defaultTracing) => new(defaultTracing);

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeTracing(GARealtimeCustomTracing customTracing) => customTracing is null ? null : new(customTracing);
}
