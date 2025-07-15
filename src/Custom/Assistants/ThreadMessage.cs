using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenType("MessageObject")]
public partial class ThreadMessage
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.message`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "thread.message";


    /// <inheritdoc cref="MessageRole"/>
    [CodeGenMember("Role")]
    public MessageRole Role { get; }

    /// <summary> A list of files attached to the message, and the tools they were added to. </summary>
    public IReadOnlyList<MessageCreationAttachment> Attachments { get; }

    internal static ThreadMessage FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeThreadMessage(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
