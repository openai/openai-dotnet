using Microsoft.TypeSpec.Generator.Customizations;
using System.ComponentModel;

namespace OpenAI.Conversations;

[CodeGenType("ConversationDeletionResult")]
public partial class ConversationDeletionResult
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Object { get; } = "conversation.deleted";
}
