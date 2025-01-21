using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

// CUSTOM: Renamed.
[Experimental("OPENAI001")]
[CodeGenModel("RunStepDetailsToolCallsFileSearchResultObjectContent")]
public partial class RunStepFileSearchResultContent
{
    // CUSTOM: Renamed.
    [CodeGenMember("Type")]
    public RunStepFileSearchResultContentKind Kind { get; } = RunStepFileSearchResultContentKind.Text;
}