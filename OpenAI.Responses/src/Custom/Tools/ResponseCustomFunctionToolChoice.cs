using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("ToolChoiceObjectFunction")]
public partial class ResponseCustomFunctionToolChoice
{
    [CodeGenMember("Name")]
    public string FunctionName { get; set; }
}