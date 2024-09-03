using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public abstract partial class ChatMessage : IJsonModel<ChatMessage>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SerializeContentValue(Utf8JsonWriter writer, ModelReaderWriterOptions options = null)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DeserializeContentValue(JsonProperty property, ref IList<ChatMessageContentPart> content, ModelReaderWriterOptions options = null)
    {
        content ??= new ChangeTrackingList<ChatMessageContentPart>();

        if (property.Value.ValueKind == JsonValueKind.Null)
        {
            return;
        }
        else if (property.Value.ValueKind == JsonValueKind.String)
        {
            content.Add(ChatMessageContentPart.CreateTextMessageContentPart(property.Value.GetString()));
        }
        else if (property.Value.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in property.Value.EnumerateArray())
            {
                content.Add(ChatMessageContentPart.DeserializeChatMessageContentPart(item, options));
            }
        }
    }

    void IJsonModel<ChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, WriteCore, writer, options);

    internal static void WriteCore(ChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    protected internal abstract void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
}
