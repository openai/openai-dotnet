using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Assistants;

[CodeGenType("MessageDeltaContentImageFileObjectImageFile")]
internal partial class InternalMessageDeltaContentImageFileObjectImageFile
{
    [CodeGenMember("Detail")]
    internal string Detail { get; set; }
}
