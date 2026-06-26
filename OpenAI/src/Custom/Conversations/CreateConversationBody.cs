using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Conversations;

[CodeGenType("CreateConversationBody")]
public partial class CreateConversationBody
{
    public static implicit operator BinaryContent(CreateConversationBody createConversationBody)
    {
        if (createConversationBody == null)
        {
            return null;
        }

        return BinaryContent.Create(createConversationBody, ModelSerializationExtensions.WireOptions);
    }
}
