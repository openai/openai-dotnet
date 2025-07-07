using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("FileSearchToolCallItemResourceStatus")]
public enum FileSearchCallStatus
{
    InProgress,
    Searching,
    Completed,
    Incomplete,
    Failed
}
