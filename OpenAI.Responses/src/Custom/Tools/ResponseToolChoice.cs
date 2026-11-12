using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetResponseToolChoice")]
[CodeGenVisibility(nameof(ResponseToolChoice), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class ResponseToolChoice
{
    public ResponseToolChoice(ResponseToolChoiceEnum  defaultToolChoice)
    {
        DefaultToolChoice = defaultToolChoice;
    }

    public ResponseToolChoice(ResponseToolChoiceObject customToolChoice)
    {
        Argument.AssertNotNull(customToolChoice, nameof(customToolChoice));

        CustomToolChoice = customToolChoice;
    }

    [CodeGenMember("DefaultToolChoice")]
    public ResponseToolChoiceEnum? DefaultToolChoice { get; }

    [CodeGenMember("CustomToolChoice")]
    public ResponseToolChoiceObject CustomToolChoice { get; }

    public static implicit operator ResponseToolChoice(ResponseToolChoiceEnum  defaultToolChoice) => new(defaultToolChoice);

    public static implicit operator ResponseToolChoice(ResponseToolChoiceObject customToolChoice) => customToolChoice is null ? null : new(customToolChoice);
}
