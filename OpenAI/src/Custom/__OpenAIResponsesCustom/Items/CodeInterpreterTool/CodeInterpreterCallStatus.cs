using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Recreated as CLR enum.
[Experimental("OPENAI001")]
[CodeGenType("CodeInterpreterToolCallItemResourceStatus")]
public enum CodeInterpreterCallStatus
{
    InProgress,
    Interpreting,
    Completed,
    Incomplete,
    Failed
}