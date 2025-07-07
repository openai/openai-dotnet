using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("WebSearchToolCallItemResourceStatus")]
public enum WebSearchCallStatus
{
    InProgress,
    Searching,
    Completed,
    Failed
}