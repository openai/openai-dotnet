using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
[CodeGenType("ChatCompletionDeleted")]
public partial class ChatCompletionDeletionResult
{
    // CUSTOM: Made internal.
    [CodeGenMember("Object")]
    internal string Object { get; } = "chat.completion.deleted";

    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string ChatCompletionId { get; }

    internal static ChatCompletionDeletionResult FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeChatCompletionDeletionResult(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
