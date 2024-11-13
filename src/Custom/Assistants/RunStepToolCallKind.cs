using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
public enum RunStepToolCallKind
{
    Unknown,
    CodeInterpreter,
    FileSearch,
    Function,
}