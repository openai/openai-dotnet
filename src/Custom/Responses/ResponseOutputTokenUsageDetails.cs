namespace OpenAI.Responses;

[CodeGenType("ResponsesResponseUsageOutputTokensDetails")]
public partial class ResponseOutputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("ReasoningTokens")]
    public int ReasoningTokenCount { get; }

}