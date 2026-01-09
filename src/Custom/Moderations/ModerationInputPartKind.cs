using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Moderations;

// CUSTOM: Renamed
[Experimental("OPENAI001")]
[CodeGenType("CreateModerationRequestInputType")]
public enum ModerationInputPartKind
{
    [CodeGenMember("Text")]
    Text,

    [CodeGenMember("ImageUrl")]
    Image,
}
