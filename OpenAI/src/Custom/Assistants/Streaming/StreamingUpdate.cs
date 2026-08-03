using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.ServerSentEvents;
using System.Text;
using System.Text.Json;

namespace OpenAI.Assistants;

/// <summary>
/// Represents a single item of streamed Assistants API data.
/// </summary>
/// <remarks>
/// Please note that this is the abstract base type. To access data, downcast an instance of this type to an
/// appropriate, derived update type:
/// <para>
/// For messages: <see cref="MessageStatusUpdate"/>, <see cref="MessageContentUpdate"/>
/// </para>
/// <para>
/// For runs and run steps: <see cref="RunUpdate"/>, <see cref="RunStepUpdate"/>, <see cref="RunStepDetailsUpdate"/>,
/// <see cref="RequiredActionUpdate"/>
/// </para>
/// <para>
/// For threads: <see cref="ThreadUpdate"/>
/// </para>
/// </remarks>
[Experimental("OPENAI001")]
public abstract partial class StreamingUpdate
{
    /// <summary>
    /// A value indicating what type of event this update represents.
    /// </summary>
    /// <remarks>
    /// Many events share the same response type. For example, <see cref="StreamingUpdateReason.RunCreated"/> and
    /// <see cref="StreamingUpdateReason.RunCompleted"/> are both associated with a <see cref="ThreadRun"/> instance.
    /// You can use the value of <see cref="UpdateKind"/> to differentiate between these events when the type is not
    /// sufficient to do so.
    /// </remarks>
    public StreamingUpdateReason UpdateKind { get; }

    internal StreamingUpdate(StreamingUpdateReason updateKind)
    {
        UpdateKind = updateKind;
    }

    internal static IEnumerable<StreamingUpdate> FromSseItem(SseItem<byte[]> sseItem)
    {
        StreamingUpdateReason updateKind = StreamingUpdateReasonExtensions.FromSseEventLabel(sseItem.EventType);
        using JsonDocument dataDocument = JsonDocument.Parse(sseItem.Data);
        JsonElement e = dataDocument.RootElement;
        ModelReaderWriterOptions serializationOptions = ModelReaderWriterOptions.Json;

        return updateKind switch
        {
            StreamingUpdateReason.ThreadCreated => ThreadUpdate.DeserializeThreadCreationUpdates(e, updateKind, serializationOptions),
            StreamingUpdateReason.RunCreated
            or StreamingUpdateReason.RunQueued
            or StreamingUpdateReason.RunInProgress
            or StreamingUpdateReason.RunCompleted
            or StreamingUpdateReason.RunIncomplete
            or StreamingUpdateReason.RunFailed
            or StreamingUpdateReason.RunCancelling
            or StreamingUpdateReason.RunCancelled
            or StreamingUpdateReason.RunExpired => RunUpdate.DeserializeRunUpdates(e, updateKind, serializationOptions),
            StreamingUpdateReason.RunRequiresAction => RequiredActionUpdate.DeserializeRequiredActionUpdates(e),
            StreamingUpdateReason.RunStepCreated
            or StreamingUpdateReason.RunStepInProgress
            or StreamingUpdateReason.RunStepCompleted
            or StreamingUpdateReason.RunStepFailed
            or StreamingUpdateReason.RunStepCancelled
            or StreamingUpdateReason.RunStepExpired => RunStepUpdate.DeserializeRunStepUpdates(e, updateKind, serializationOptions),
            StreamingUpdateReason.MessageCreated
            or StreamingUpdateReason.MessageInProgress
            or StreamingUpdateReason.MessageCompleted
            or StreamingUpdateReason.MessageFailed => MessageStatusUpdate.DeserializeMessageStatusUpdates(e, updateKind, serializationOptions),
            StreamingUpdateReason.RunStepUpdated => RunStepDetailsUpdate.DeserializeRunStepDetailsUpdates(e, updateKind, serializationOptions),
            StreamingUpdateReason.MessageUpdated => MessageContentUpdate.DeserializeMessageContentUpdates(e, updateKind, serializationOptions),
            StreamingUpdateReason.Error => throw CreateExceptionFromErrorEvent(e),
            _ => [],
        };
    }

    // The service can emit an "error" event mid-stream (for example, when an account is out of quota or the server
    // fails while generating). Unlike other unmodeled events, which are benign and yield no updates, the error event
    // carries a service failure that callers must observe. Surface it through the SDK's standard error exception so
    // that a streaming failure is reported the same way as a non-streaming one rather than looking like a clean,
    // truncated stream.
    private static ClientResultException CreateExceptionFromErrorEvent(JsonElement data)
    {
        if (data.TryGetProperty("error", out JsonElement errorElement))
        {
            string code = errorElement.TryGetProperty("code", out JsonElement c) && c.ValueKind == JsonValueKind.String ? c.GetString() : null;
            string message = errorElement.TryGetProperty("message", out JsonElement m) && m.ValueKind == JsonValueKind.String ? m.GetString() : null;
            string param = errorElement.TryGetProperty("param", out JsonElement p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null;
            string kind = errorElement.TryGetProperty("type", out JsonElement t) && t.ValueKind == JsonValueKind.String ? t.GetString() : null;

            return new ClientResultException(FormatErrorMessage(code, message, param, kind));
        }

        return new ClientResultException(data.GetRawText());
    }

    private static string FormatErrorMessage(string code, string message, string param, string kind)
    {
        StringBuilder messageBuilder = new();
        messageBuilder.Append("Streaming error (").Append(kind).Append(": ").Append(code).AppendLine(")");

        if (!string.IsNullOrEmpty(param))
        {
            messageBuilder.Append("Parameter: ").AppendLine(param);
        }

        messageBuilder.AppendLine();
        messageBuilder.Append(message);
        return messageBuilder.ToString();
    }
}

/// <summary>
/// Represents a single item of streamed data that encapsulates an underlying response value type.
/// </summary>
/// <typeparam name="T"> The response value type of the "delta" payload. </typeparam>
[Experimental("OPENAI001")]
public partial class StreamingUpdate<T> : StreamingUpdate
    where T : class
{
    /// <summary>
    /// The underlying response value received with the streaming event.
    /// </summary>
    public T Value { get; }

    internal StreamingUpdate(T value, StreamingUpdateReason updateKind)
        : base(updateKind)
    {
        Value = value;
    }

    /// <summary>
    /// Implicit operator that allows the underlying value type of the <see cref="StreamingUpdate{T}"/> to be used
    /// directly.
    /// </summary>
    /// <param name="update"></param>
    public static implicit operator T(StreamingUpdate<T> update) => update.Value;
}
