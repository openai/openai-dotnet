using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetRealtimeMaxOutputTokenCountGA")]
[CodeGenVisibility(nameof(RealtimeMaxOutputTokenCount), CodeGenVisibility.Internal)]
public partial class RealtimeMaxOutputTokenCount
{
    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeMaxOutputTokenCount(RealtimeDefaultMaxOutputTokenCount defaultMaxOutputTokenCount)
    {
        DefaultMaxOutputTokenCount = defaultMaxOutputTokenCount;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeMaxOutputTokenCount(int customMaxOutputTokenCount)
    {
        CustomMaxOutputTokenCount = customMaxOutputTokenCount;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultMaxOutputTokenCount")]
    public RealtimeDefaultMaxOutputTokenCount? DefaultMaxOutputTokenCount { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomMaxOutputTokenCount")]
    public int? CustomMaxOutputTokenCount { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator RealtimeMaxOutputTokenCount(RealtimeDefaultMaxOutputTokenCount defaultMaxOutputTokenCount) => new(defaultMaxOutputTokenCount);

    // CUSTOM: Added for convenience.
    public static implicit operator RealtimeMaxOutputTokenCount(int customMaxOutputTokenCount) => new(customMaxOutputTokenCount);
}
