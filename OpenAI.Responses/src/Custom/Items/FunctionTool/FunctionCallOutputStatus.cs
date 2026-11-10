using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM:
// - Experimental attribute added by generator.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("FunctionToolCallOutputStatus")]
public enum FunctionCallOutputStatus
{
    InProgress,
    Completed,
    Incomplete
}
