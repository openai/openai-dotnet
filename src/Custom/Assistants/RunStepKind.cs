using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenType("RunStepObjectType")]
public enum RunStepKind
{
    // CUSTOM: Renamed.
    [CodeGenMember("MessageCreation")]
    CreatedMessage,

    // CUSTOM: Renamed.
    [CodeGenMember("ToolCalls")]
    ToolCall,
}
