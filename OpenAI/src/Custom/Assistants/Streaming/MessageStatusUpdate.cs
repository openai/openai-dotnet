using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

/// <summary>
/// The update type presented when the status of a message changes.
/// </summary>
[Experimental("OPENAI001")]
public class MessageStatusUpdate : StreamingUpdate<ThreadMessage>
{
    internal MessageStatusUpdate(ThreadMessage message, StreamingUpdateReason updateKind)
        : base(message, updateKind)
    { }

    internal static IEnumerable<MessageStatusUpdate> DeserializeMessageStatusUpdates(
        JsonElement element,
        StreamingUpdateReason updateKind,
        ModelReaderWriterOptions options = null)
    {
        ThreadMessage message = ThreadMessage.DeserializeThreadMessage(element, options);
        return updateKind switch
        {
            _ => [new MessageStatusUpdate(message, updateKind)],
        };
    }
}