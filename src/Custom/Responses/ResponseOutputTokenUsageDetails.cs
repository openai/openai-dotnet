using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("ResponseUsageOutputTokensDetails")]
public partial class ResponseOutputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("ReasoningTokens")]
    public int ReasoningTokenCount { get; }

}