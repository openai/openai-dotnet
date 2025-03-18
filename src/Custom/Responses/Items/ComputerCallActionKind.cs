using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

[CodeGenType("ResponsesComputerCallActionType")]
[Experimental("OPENAICUA001")]
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