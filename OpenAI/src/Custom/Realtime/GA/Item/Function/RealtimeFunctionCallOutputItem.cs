using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeConversationItemFunctionCallOutputGA")]
public partial class RealtimeFunctionCallOutputItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public string FunctionOutput { get; set; }
}