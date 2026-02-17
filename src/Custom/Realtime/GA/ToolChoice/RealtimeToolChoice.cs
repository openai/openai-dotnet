using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetRealtimeToolChoiceGA")]
[CodeGenVisibility(nameof(GARealtimeToolChoice), CodeGenVisibility.Internal)]
public partial class GARealtimeToolChoice
{
    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeToolChoice(GARealtimeDefaultToolChoice defaultToolChoice)
    {
        DefaultToolChoice = defaultToolChoice;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeToolChoice(GARealtimeCustomToolChoice customToolChoice)
    {
        Argument.AssertNotNull(customToolChoice, nameof(customToolChoice));

        CustomToolChoice = customToolChoice;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultToolChoice")]
    public GARealtimeDefaultToolChoice? DefaultToolChoice { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomToolChoice")]
    public GARealtimeCustomToolChoice CustomToolChoice { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeToolChoice(GARealtimeDefaultToolChoice defaultToolChoice) => new(defaultToolChoice);

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeToolChoice(GARealtimeCustomToolChoice customToolChoice) => customToolChoice is null ? null : new(customToolChoice);
}
