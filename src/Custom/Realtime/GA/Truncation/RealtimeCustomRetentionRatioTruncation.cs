using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeTruncationRetentionRatioGA")]
public partial class GARealtimeCustomRetentionRatioTruncation
{
    // CUSTOM: Renamed.
    [CodeGenMember("TokenLimits")]
    public GARealtimeRetentionRatioTokenLimitDetails TokenLimitDetails { get; set; }
}
