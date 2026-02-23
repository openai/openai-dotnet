using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeConversationItemFunctionCallGA")]
public partial class GARealtimeFunctionCallItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("Name")]
    public string FunctionName { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type.
    [CodeGenMember("Arguments")]
    public BinaryData FunctionArguments { get; set; }
}