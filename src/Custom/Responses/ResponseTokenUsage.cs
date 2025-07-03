using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("ResponseUsage")]
public partial class ResponseTokenUsage
{
    // CUSTOM: Renamed.
    [CodeGenMember("InputTokens")]
    public int InputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputTokens")]
    public int OutputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("TotalTokens")]
    public int TotalTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("InputTokensDetails")]
    public ResponseInputTokenUsageDetails InputTokenDetails { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputTokensDetails")]
    public ResponseOutputTokenUsageDetails OutputTokenDetails { get; }

	[CodeGenMember("InputTokensDetails")]
	public ResponseInputTokenDetails InputTokenDetails { get; }
}
