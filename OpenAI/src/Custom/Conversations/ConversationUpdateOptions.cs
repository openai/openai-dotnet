using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Conversations;

[CodeGenType("ConversationUpdateOptions")]
public partial class ConversationUpdateOptions
{
    public static implicit operator BinaryContent(ConversationUpdateOptions conversationUpdateOptions)
    {
        if (conversationUpdateOptions == null)
        {
            return null;
        }

        return BinaryContent.Create(conversationUpdateOptions, ModelSerializationExtensions.WireOptions);
    }
}
