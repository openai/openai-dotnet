using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("RunStepDetailsToolCallType")]
public enum RunStepToolCallKind
{
    CodeInterpreter,
    FileSearch,
    Function,
}