using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAICUA001")]
[CodeGenType("ComputerToolCallItemResourceStatus")]
public enum ComputerCallStatus
{
    InProgress,
    Completed,
    Incomplete
}