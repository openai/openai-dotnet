using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeMCPListToolsGA")]
public partial class GARealtimeMcpToolDefinitionListItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("Tools")]
    public IList<GARealtimeMcpToolDefinition> ToolDefinitions { get; }
}