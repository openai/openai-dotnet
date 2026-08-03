using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Experimental attribute added by generator.
// - Renamed.
// - Converted to extensible enum.
[CodeGenType("ComputerActionType")]
public readonly partial struct ComputerCallActionKind
{
    [CodeGenMember("Keypress")]
    public static ComputerCallActionKind KeyPress { get; } = new ComputerCallActionKind(KeypressValue);
}