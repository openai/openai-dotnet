using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>rate_limits.updated</c> server event.
/// Emitted at the beginning of a Response to indicate the updated rate limits.
/// When a Response is created some tokens will be "reserved" for the output
/// tokens, the rate limits shown here reflect that reservation, which is then
/// adjusted accordingly once the Response is completed.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventRateLimitsUpdatedGA")]
public partial class GARealtimeServerUpdateRateLimitsUpdated
{
    // CUSTOM: Renamed.
    [CodeGenMember("RateLimits")]
    public IList<GARealtimeRateLimitDetails> RateLimitDetails { get; }
}
