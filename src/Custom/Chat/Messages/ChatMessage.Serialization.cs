using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
[CodeGenSerialization(nameof(Content), SerializationValueHook = nameof(SerializeContentValue), DeserializationValueHook = nameof(DeserializeContentValue))]
public partial class ChatMessage
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SerializeContentValue(Utf8JsonWriter writer, ModelReaderWriterOptions options = null)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DeserializeContentValue(JsonProperty property, ref ChatMessageContent content, ModelReaderWriterOptions options = null)
    {
        content = ChatMessageContent.DeserializeChatMessageContent(property.Value, options);
    }

    void IJsonModel<ChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, WriteCore, writer, options);

    internal static void WriteCore(ChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    internal virtual void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => throw new InvalidOperationException($"The {nameof(WriteCore)} method should be invoked on an overriding type derived from {nameof(ChatMessage)}.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void WriteRoleProperty(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(Role.ToSerialString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void WriteContentProperty(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (Optional.IsDefined(Content) && Content.IsInnerCollectionDefined())
        {
            writer.WritePropertyName("content"u8);
            Content.WriteTo(writer, options);
        }
    }
}
