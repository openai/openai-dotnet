using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("ResponseUsageInputTokensDetails")]
public partial class ResponseInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("CachedTokens")]
    public int CachedTokenCount { get; }
}