namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseUsageOutputTokensDetails")]
public partial class ResponseOutputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("ReasoningTokens")]
    public int ReasoningTokenCount { get; set; }
}