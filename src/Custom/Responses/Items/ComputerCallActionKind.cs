using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAICUA001")]
[CodeGenType("ComputerActionType")]
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