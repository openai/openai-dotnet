using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventRateLimitsUpdatedRateLimitsGA")]
public partial class RealtimeRateLimitDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("Remaining")]
    public int? RemainingCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("ResetSeconds")]
    public TimeSpan? TimeUntilReset { get; }
}