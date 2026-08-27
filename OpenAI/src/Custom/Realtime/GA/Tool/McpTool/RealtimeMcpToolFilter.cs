using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("MCPToolFilterGA")]
public partial class RealtimeMcpToolFilter
{
    [CodeGenMember("ReadOnly")]
    public bool? IsReadOnly { get; set; }
}
