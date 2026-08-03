using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Moderations;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Converted to extensible enum.
[CodeGenType("CreateModerationRequestInputType")]
public readonly partial struct ModerationInputPartKind
{
    [CodeGenMember("ImageUrl")]
    public static ModerationInputPartKind Image { get; } = new ModerationInputPartKind(ImageUrlValue);
}
