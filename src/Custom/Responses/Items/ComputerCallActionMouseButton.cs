using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

[CodeGenType("ResponsesComputerCallClickButtonType")]
[Experimental("OPENAICUA001")]
public enum ComputerCallActionMouseButton
{
    Left,
    Right,
    Wheel,
    Back,
    Forward
}
