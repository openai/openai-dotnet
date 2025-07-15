using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>rate_limits_updated</c>, which is received during a response and provides
/// the most recent information about configured rate limits.
/// </summary>
[CodeGenType("RealtimeServerEventRateLimitsUpdated")]
public partial class RateLimitsUpdate
{
    public ConversationRateLimitDetailsItem TokenDetails
        => _tokenDetails ??= AllDetails.FirstOrDefault(item => item.Name == "tokens");
    private ConversationRateLimitDetailsItem _tokenDetails;
    public ConversationRateLimitDetailsItem RequestDetails
        => _tokenDetails ??= AllDetails.FirstOrDefault(item => item.Name == "requests");

    [CodeGenMember("RateLimits")]
    public IReadOnlyList<ConversationRateLimitDetailsItem> AllDetails { get; }
}