using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenType("DeleteMessageResponse")]
public partial class MessageDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string MessageId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.message.deleted`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "thread.message.deleted";

    internal static MessageDeletionResult FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeMessageDeletionResult(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
