using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("FunctionToolCallOutputItemResourceStatus")]
public enum FunctionCallOutputStatus
{
    InProgress,
    Completed,
    Incomplete
}