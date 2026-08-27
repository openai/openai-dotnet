using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Assistants;

[CodeGenType("MessageContentImageFileObjectImageFile")]
internal partial class InternalMessageContentItemFileObjectImageFile
{
    [CodeGenMember("Detail")]
    internal string Detail { get; set; }
}
