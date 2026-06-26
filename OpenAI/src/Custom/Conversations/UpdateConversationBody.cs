using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Conversations;

[CodeGenType("UpdateConversationBody")]
public partial class UpdateConversationBody
{
    public static implicit operator BinaryContent(UpdateConversationBody updateConversationBody)
    {
        if (updateConversationBody == null)
        {
            return null;
        }

        return BinaryContent.Create(updateConversationBody, ModelSerializationExtensions.WireOptions);
    }
}
