using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeServerEventRateLimitsUpdatedRateLimitsItem")]
public partial class ConversationRateLimitDetailsItem
{
    [CodeGenMember("Limit")]
    public int MaximumCount { get; }

    [CodeGenMember("Remaining")]
    public int RemainingCount { get; }

    [CodeGenMember("ResetSeconds")]
    public TimeSpan TimeUntilReset { get; }
}
