using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeTruncationRetentionRatioGA")]
public partial class RealtimeCustomRetentionRatioTruncation
{
    // CUSTOM: Renamed.
    [CodeGenMember("TokenLimits")]
    public RealtimeRetentionRatioTokenLimitDetails TokenLimitDetails { get; set; }
}
