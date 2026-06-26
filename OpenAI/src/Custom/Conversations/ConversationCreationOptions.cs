using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Conversations;

[CodeGenType("ConversationCreationOptions")]
public partial class ConversationCreationOptions
{
    public static implicit operator BinaryContent(ConversationCreationOptions conversationCreationOptions)
    {
        if (conversationCreationOptions == null)
        {
            return null;
        }

        return BinaryContent.Create(conversationCreationOptions, ModelSerializationExtensions.WireOptions);
    }
}
