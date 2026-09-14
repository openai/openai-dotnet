using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetRealtimeToolChoiceGA")]
[CodeGenVisibility(nameof(RealtimeToolChoice), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class RealtimeToolChoice
{
    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeToolChoice(RealtimeDefaultToolChoice defaultToolChoice)
    {
        DefaultToolChoice = defaultToolChoice;
    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        _patch.SetPropagators(PropagateSet, PropagateGet);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeToolChoice(RealtimeCustomToolChoice customToolChoice)
    {
        Argument.AssertNotNull(customToolChoice, nameof(customToolChoice));

        CustomToolChoice = customToolChoice;
    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        _patch.SetPropagators(PropagateSet, PropagateGet);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultToolChoice")]
    public RealtimeDefaultToolChoice? DefaultToolChoice { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomToolChoice")]
    public RealtimeCustomToolChoice CustomToolChoice { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator RealtimeToolChoice(RealtimeDefaultToolChoice defaultToolChoice) => new(defaultToolChoice);

    // CUSTOM: Added for convenience.
    public static implicit operator RealtimeToolChoice(RealtimeCustomToolChoice customToolChoice) => customToolChoice is null ? null : new(customToolChoice);
}
