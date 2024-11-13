using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class ChatMessage : IJsonModel<ChatMessage>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SerializeContentValue(Utf8JsonWriter writer, ModelReaderWriterOptions options = null)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DeserializeContentValue(JsonProperty property, ref ChatMessageContent content, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.Null)
        {
            return;
        }
        else if (property.Value.ValueKind == JsonValueKind.String)
        {
            content = new ChatMessageContent(property.Value.GetString());
        }
        else if (property.Value.ValueKind == JsonValueKind.Array)
        {
            IList<ChatMessageContentPart> parts = [];

            foreach (var item in property.Value.EnumerateArray())
            {
                parts.Add(ChatMessageContentPart.DeserializeChatMessageContentPart(item, options));
            }

            content = new ChatMessageContent(parts);
        }
    }

    void IJsonModel<ChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, WriteCore, writer, options);

    internal static void WriteCore(ChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    internal virtual void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => throw new InvalidOperationException($"The {nameof(WriteCore)} method should be invoked on an overriding type derived from {nameof(ChatMessage)}.");
}
