using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeMCPToolCallGA")]
public partial class RealtimeMcpToolCallItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("Name")]
    public string ToolName { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type.
    [CodeGenMember("Arguments")]
    public BinaryData ToolArguments { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public string ToolOutput { get; set; }
}