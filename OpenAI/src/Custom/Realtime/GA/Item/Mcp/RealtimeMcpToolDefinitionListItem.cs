using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeMCPListToolsGA")]
public partial class RealtimeMcpToolDefinitionListItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("Tools")]
    public IList<RealtimeMcpToolDefinition> ToolDefinitions { get; }
}