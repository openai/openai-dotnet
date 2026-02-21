using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

[CodeGenType("DotNetRealtimeTurnDetectionGA")]
public partial class GARealtimeTurnDetection
{
    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeTurnDetection(GARealtimeDefaultTurnDetection defaultTurnDetection)
    {
        DefaultTurnDetection = defaultTurnDetection;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeTurnDetection(GARealtimeCustomTurnDetection customTurnDetection)
    {
        Argument.AssertNotNull(customTurnDetection, nameof(customTurnDetection));

        CustomTurnDetection = customTurnDetection;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultTurnDetection")]
    public GARealtimeDefaultTurnDetection? DefaultTurnDetection { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomTurnDetection")]
    public GARealtimeCustomTurnDetection CustomTurnDetection { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeTurnDetection(GARealtimeDefaultTurnDetection defaultTurnDetection) => new(defaultTurnDetection);

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeTurnDetection(GARealtimeCustomTurnDetection customTurnDetection) => customTurnDetection is null ? null : new(customTurnDetection);
}
