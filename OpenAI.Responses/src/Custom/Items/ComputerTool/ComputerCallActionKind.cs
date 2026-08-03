using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ComputerActionType")]
public readonly partial struct ComputerCallActionKind
{
    [CodeGenMember("Keypress")]
    public static ComputerCallActionKind KeyPress { get; } = new ComputerCallActionKind(KeypressValue);
}