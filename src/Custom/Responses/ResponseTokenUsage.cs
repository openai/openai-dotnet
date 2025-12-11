namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseUsage")]
public partial class ResponseTokenUsage
{
    // CUSTOM: Renamed.
    [CodeGenMember("InputTokens")]
    public int InputTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputTokens")]
    public int OutputTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("TotalTokens")]
    public int TotalTokenCount { get; set;  }

    // CUSTOM: Renamed.
    [CodeGenMember("InputTokensDetails")]
    public ResponseInputTokenUsageDetails InputTokenDetails { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputTokensDetails")]
    public ResponseOutputTokenUsageDetails OutputTokenDetails { get; set; }
}
