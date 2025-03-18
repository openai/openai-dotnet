using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponsesComputerCallItemStatus")]
[Experimental("OPENAICUA001")]
public enum ComputerCallStatus
{
    InProgress,
    Completed,
    Incomplete
}