namespace OpenAI.Assistants;

public enum RunStepToolCallKind
{
    Unknown,
    CodeInterpreter,
    FileSearch,
    Function,
}