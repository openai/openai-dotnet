using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("ToolChoiceFunctionGA")]
public partial class GARealtimeCustomFunctionToolChoice
{
    // CUSTOM: Renamed.
    [CodeGenMember("Name")]
    public string FunctionName { get; set; }
}