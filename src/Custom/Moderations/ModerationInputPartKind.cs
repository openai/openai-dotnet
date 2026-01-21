using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Moderations;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Plain enum type to convert from an extensible enum
[Experimental("OPENAI001")]
[CodeGenType("CreateModerationRequestInputType")]
public enum ModerationInputPartKind
{
    [CodeGenMember("Text")]
    Text,

    [CodeGenMember("ImageUrl")]
    Image,
}
