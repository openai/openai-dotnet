using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Moderations;

// CUSTOM: Renamed.
[CodeGenType("ModerationTextInput")]
public partial class ModerationTextPart : ModerationInputPart
{
    internal ModerationTextPart(string text) : base(ModerationInputPartKind.Text)
    {
        Argument.AssertNotNull(text, nameof(text));

        Text = text;
    }
}
