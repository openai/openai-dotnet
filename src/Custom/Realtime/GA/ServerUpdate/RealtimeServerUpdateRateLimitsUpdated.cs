using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventRateLimitsUpdatedGA")]
public partial class GARealtimeServerUpdateRateLimitsUpdated
{
    // CUSTOM: Renamed.
    [CodeGenMember("RateLimits")]
    public IList<GARealtimeRateLimitDetails> RateLimitDetails { get; }
}
