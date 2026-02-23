using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("MCPToolFilterGA")]
public partial class GARealtimeMcpToolFilter
{
    [CodeGenMember("ReadOnly")]
    public bool? IsReadOnly { get; set; }
}
