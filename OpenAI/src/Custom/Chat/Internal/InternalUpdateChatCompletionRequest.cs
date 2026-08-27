using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Chat;

// CUSTOM: Renamed.
[CodeGenType("UpdateChatCompletionRequest")]
internal partial class InternalUpdateChatCompletionRequest
{
    public static implicit operator BinaryContent(InternalUpdateChatCompletionRequest internalUpdateChatCompletionRequest)
    {
        if (internalUpdateChatCompletionRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalUpdateChatCompletionRequest, ModelSerializationExtensions.WireOptions);
    }
}
