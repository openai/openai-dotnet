using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("RunStepDetailsToolCallsFileSearchResultObject")]
public partial class RunStepFileSearchResult
{
    // CUSTOM: made internal pending design support for include[] query string parameter
    [CodeGenMember("Content")]
    internal IReadOnlyList<InternalRunStepDetailsToolCallsFileSearchResultObjectContent> Content { get; }
}
