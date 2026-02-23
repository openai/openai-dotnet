using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseMCPCallArgumentsDoneGA")]
public partial class GARealtimeServerUpdateResponseMcpCallArgumentsDone
{
    // CUSTOM:
    // - Renamed.
    // - Changed type.
    [CodeGenMember("Arguments")]
    public BinaryData ToolArguments { get; }
}
