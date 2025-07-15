using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

// CUSTOM: Renamed.
[CodeGenType("RunStepDetailsToolCallsFileSearchResultObjectContent")]
public partial class RunStepFileSearchResultContent
{
    // CUSTOM: Apply typed discriminator value.
    [CodeGenMember("Type")]
    public RunStepFileSearchResultContentKind Kind { get; } = RunStepFileSearchResultContentKind.Text;
}