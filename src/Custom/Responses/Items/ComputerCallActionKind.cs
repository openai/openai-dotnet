namespace OpenAI.Responses;

[CodeGenType("ResponsesComputerCallActionType")]
public enum ComputerCallActionKind
{
    Click,

    DoubleClick,

    Drag,

    [CodeGenMember("Keypress")]
    KeyPress,

    Move,

    Screenshot,

    Scroll,

    Type,

    Wait
}