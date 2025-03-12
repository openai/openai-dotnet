using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenType("RunStepDetailsToolCallType")]
public enum RunStepToolCallKind
{
    CodeInterpreter,
    FileSearch,
    Function,
}