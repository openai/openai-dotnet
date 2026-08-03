using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Moderations;

// CUSTOM:
// - Experimental attribute added by generator.
// - Renamed.
// - Converted to extensible enum.
[CodeGenType("CreateModerationRequestInputType")]
public readonly partial struct ModerationInputPartKind
{
    [CodeGenMember("ImageUrl")]
    public static ModerationInputPartKind Image { get; } = new ModerationInputPartKind(ImageUrlValue);
}
