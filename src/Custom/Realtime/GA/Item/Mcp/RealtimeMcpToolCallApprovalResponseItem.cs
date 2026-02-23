using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeMCPApprovalResponseGA")]
public partial class GARealtimeMcpToolCallApprovalResponseItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("Approve")]
    public bool Approved { get; set; }
}