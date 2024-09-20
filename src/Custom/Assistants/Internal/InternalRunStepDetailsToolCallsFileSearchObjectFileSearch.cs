namespace OpenAI.Assistants;

[CodeGenModel("RunStepDetailsToolCallsFileSearchObjectFileSearch")]
internal partial class InternalRunStepDetailsToolCallsFileSearchObjectFileSearch
{
    // CUSTOM: reuse input model for ranking options, which differs only in the request presence of 'auto' as a selection
    /// <inheritdoc cref="FileSearchToolDefinition.RankingOptions"/>
    [CodeGenMember("RankingOptions")]
    public FileSearchRankingOptions RankingOptions { get; }
}