using Microsoft.TypeSpec.Generator.Customizations;
using System.ComponentModel;

namespace OpenAI.Conversations;

[CodeGenType("ConversationResource")]
public partial class ConversationResource
{
    [CodeGenMember("Object")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Object { get; set; } = "conversation";
}
