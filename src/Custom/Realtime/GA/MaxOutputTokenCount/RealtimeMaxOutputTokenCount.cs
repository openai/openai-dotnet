using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetRealtimeMaxOutputTokenCountGA")]
[CodeGenVisibility(nameof(GARealtimeMaxOutputTokenCount), CodeGenVisibility.Internal)]
public partial class GARealtimeMaxOutputTokenCount
{
    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeMaxOutputTokenCount(GARealtimeDefaultMaxOutputTokenCount defaultMaxOutputTokenCount)
    {
        DefaultMaxOutputTokenCount = defaultMaxOutputTokenCount;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeMaxOutputTokenCount(int customMaxOutputTokenCount)
    {
        CustomMaxOutputTokenCount = customMaxOutputTokenCount;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultMaxOutputTokenCount")]
    public GARealtimeDefaultMaxOutputTokenCount? DefaultMaxOutputTokenCount { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomMaxOutputTokenCount")]
    public int? CustomMaxOutputTokenCount { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeMaxOutputTokenCount(GARealtimeDefaultMaxOutputTokenCount defaultMaxOutputTokenCount) => new(defaultMaxOutputTokenCount);

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeMaxOutputTokenCount(int customMaxOutputTokenCount) => new(customMaxOutputTokenCount);
}
