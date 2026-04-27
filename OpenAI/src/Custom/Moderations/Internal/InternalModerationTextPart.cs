using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Moderations;

[CodeGenType("ModerationTextInput")]
internal partial class InternalModerationTextPart
{
    // CUSTOM: Rename for parent recombination of common properties
    [CodeGenMember("Text")]
    public string InternalText { get; set; }
}
