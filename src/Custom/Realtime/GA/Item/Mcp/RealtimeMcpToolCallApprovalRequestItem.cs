using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeMCPApprovalRequestGA")]
public partial class RealtimeMcpToolCallApprovalRequestItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("Name")]
    public string ToolName { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type.
    [CodeGenMember("Arguments")]
    public BinaryData ToolArguments { get; set; }
}