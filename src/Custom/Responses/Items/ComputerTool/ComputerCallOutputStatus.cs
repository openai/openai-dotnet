using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAICUA001")]
[CodeGenType("ComputerToolCallOutputItemResourceStatus")]
public enum ComputerCallOutputStatus
{
    InProgress,
    Completed,
    Incomplete
}