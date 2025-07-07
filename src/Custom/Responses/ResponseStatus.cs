using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("ResponseStatus")]
public enum ResponseStatus
{
    InProgress,
    Completed,
    Cancelled,
    Queued,
    Incomplete,
    Failed
}
