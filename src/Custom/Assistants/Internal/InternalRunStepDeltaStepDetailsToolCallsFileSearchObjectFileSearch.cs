namespace OpenAI.Assistants;

[CodeGenType("RunStepDeltaStepDetailsToolCallsFileSearchObjectFileSearch")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsFileSearchObjectFileSearch
{
    // CUSTOM: reuse input model for ranking options, which differs only in the request presence of 'auto' as a selection
    [CodeGenMember("RankingOptions")]
    public FileSearchRankingOptions RankingOptions { get; }
}