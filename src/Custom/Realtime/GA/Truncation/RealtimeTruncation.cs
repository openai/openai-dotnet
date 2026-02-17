using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetRealtimeTruncationGA")]
[CodeGenVisibility(nameof(GARealtimeTruncation), CodeGenVisibility.Internal)]
public partial class GARealtimeTruncation
{
    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeTruncation(GARealtimeDefaultTruncation defaultTruncation)
    {
        DefaultTruncation = defaultTruncation;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeTruncation(GARealtimeCustomTruncation customTruncation)
    {
        Argument.AssertNotNull(customTruncation, nameof(customTruncation));

        CustomTruncation = customTruncation;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultTruncation")]
    public GARealtimeDefaultTruncation? DefaultTruncation { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomTruncation")]
    public GARealtimeCustomTruncation CustomTruncation { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeTruncation(GARealtimeDefaultTruncation defaultTruncation) => new(defaultTruncation);

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeTruncation(GARealtimeCustomTruncation customTruncation) => customTruncation is null ? null : new(customTruncation);
}
