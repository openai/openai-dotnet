using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponseUsageInputTokensDetails")]
public partial class ResponseInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("CachedTokens")]
    public int CachedTokenCount { get; }
}