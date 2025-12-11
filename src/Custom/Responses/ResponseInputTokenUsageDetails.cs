namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseUsageInputTokensDetails")]
public partial class ResponseInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("CachedTokens")]
    public int CachedTokenCount { get; set; }
}