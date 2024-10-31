using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventRateLimitsUpdatedRateLimitsItem")]
public partial class ConversationRateLimitDetailsItem
{
    [CodeGenMember("Limit")]
    public int MaximumCount { get; }

    [CodeGenMember("Remaining")]
    public int RemainingCount { get; }

    [CodeGenMember("ResetSeconds")]
    public TimeSpan TimeUntilReset { get; }
}
