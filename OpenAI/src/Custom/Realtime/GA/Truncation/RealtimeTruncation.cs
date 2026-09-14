using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetRealtimeTruncationGA")]
[CodeGenVisibility(nameof(RealtimeTruncation), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class RealtimeTruncation
{
    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeTruncation(RealtimeDefaultTruncation defaultTruncation)
    {
        DefaultTruncation = defaultTruncation;
    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        _patch.SetPropagators(PropagateSet, PropagateGet);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeTruncation(RealtimeCustomTruncation customTruncation)
    {
        Argument.AssertNotNull(customTruncation, nameof(customTruncation));

        CustomTruncation = customTruncation;
    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        _patch.SetPropagators(PropagateSet, PropagateGet);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultTruncation")]
    public RealtimeDefaultTruncation? DefaultTruncation { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomTruncation")]
    public RealtimeCustomTruncation CustomTruncation { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator RealtimeTruncation(RealtimeDefaultTruncation defaultTruncation) => new(defaultTruncation);

    // CUSTOM: Added for convenience.
    public static implicit operator RealtimeTruncation(RealtimeCustomTruncation customTruncation) => customTruncation is null ? null : new(customTruncation);
}
