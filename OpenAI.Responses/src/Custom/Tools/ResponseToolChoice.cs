using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("DotNetResponseToolChoice")]
[CodeGenVisibility(nameof(ResponseToolChoice), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class ResponseToolChoice
{
    public ResponseToolChoice(ResponseDefaultToolChoice defaultToolChoice)
    {
        DefaultToolChoice = defaultToolChoice;
    }

    public ResponseToolChoice(ResponseCustomToolChoice customToolChoice)
    {
        Argument.AssertNotNull(customToolChoice, nameof(customToolChoice));

        CustomToolChoice = customToolChoice;
    }

    [CodeGenMember("DefaultToolChoice")]
    public ResponseDefaultToolChoice? DefaultToolChoice { get; }

    [CodeGenMember("CustomToolChoice")]
    public ResponseCustomToolChoice CustomToolChoice { get; }

    public static implicit operator ResponseToolChoice(ResponseDefaultToolChoice defaultToolChoice) => new(defaultToolChoice);

    public static implicit operator ResponseToolChoice(ResponseCustomToolChoice customToolChoice) => customToolChoice is null ? null : new(customToolChoice);
}
