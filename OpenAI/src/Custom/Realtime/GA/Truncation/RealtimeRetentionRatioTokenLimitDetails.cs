using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("TokenLimitsGA")]
public partial class RealtimeRetentionRatioTokenLimitDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("PostInstructions")]
    public int? MaxPostInstructionsTokenCount { get; set; }
}
