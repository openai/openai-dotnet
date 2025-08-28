using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAICUA001")]
[CodeGenType("ComputerActionClickButton")]
public enum ComputerCallActionMouseButton
{
    Left,
    Right,
    Wheel,
    Back,
    Forward
}
