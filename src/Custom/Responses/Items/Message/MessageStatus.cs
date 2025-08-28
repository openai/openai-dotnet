using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("ResponsesMessageItemResourceStatus")]
public enum MessageStatus
{
    InProgress,
    Completed,
    Incomplete
}
