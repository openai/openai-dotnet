using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.InternalChatCompletionResponseMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
internal partial class InternalChatCompletionResponseMessage : IJsonModel<InternalChatCompletionResponseMessage>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeContentValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeContentValue(JsonProperty property, ref IReadOnlyList<ChatMessageContentPart> content, ModelReaderWriterOptions options = null)
    {
        throw new NotImplementedException();
    }

    void IJsonModel<InternalChatCompletionResponseMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalChatCompletionResponseMessage, writer, options);

    internal static void SerializeInternalChatCompletionResponseMessage(InternalChatCompletionResponseMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        if (Optional.IsCollectionDefined(instance.Content))
        {
            if (instance.Content[0] != null)
            {
                writer.WritePropertyName("content"u8);
                writer.WriteStringValue(instance.Content[0].Text);
            }
            else
            {
                writer.WriteNull("content");
            }
        }
        if (Optional.IsCollectionDefined(instance.ToolCalls))
        {
            writer.WritePropertyName("tool_calls"u8);
            writer.WriteStartArray();
            foreach (var item in instance.ToolCalls)
            {
                writer.WriteObjectValue(item, options);
            }
            writer.WriteEndArray();
        }
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(instance.Role.ToSerialString());
        if (Optional.IsDefined(instance.FunctionCall))
        {
            writer.WritePropertyName("function_call"u8);
            writer.WriteObjectValue<ChatFunctionCall>(instance.FunctionCall, options);
        }
        writer.WriteSerializedAdditionalRawData(instance.SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    internal static InternalChatCompletionResponseMessage DeserializeInternalChatCompletionResponseMessage(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        IReadOnlyList<ChatMessageContentPart> content = default;
        string refusal = default;
        IReadOnlyList<ChatToolCall> toolCalls = default;
        ChatMessageRole role = default;
        ChatFunctionCall functionCall = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("content"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                List<ChatMessageContentPart> array = new List<ChatMessageContentPart>();
                array.Add(ChatMessageContentPart.CreateTextMessageContentPart(property.Value.GetString()));
                content = array;
                continue;
            }
            if (property.NameEquals("refusal"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                refusal = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("tool_calls"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                List<ChatToolCall> array = new List<ChatToolCall>();
                foreach (var item in property.Value.EnumerateArray())
                {
                    array.Add(ChatToolCall.DeserializeChatToolCall(item, options));
                }
                toolCalls = array;
                continue;
            }
            if (property.NameEquals("role"u8))
            {
                role = property.Value.GetString().ToChatMessageRole();
                continue;
            }
            if (property.NameEquals("function_call"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                functionCall = ChatFunctionCall.DeserializeChatFunctionCall(property.Value, options);
                continue;
            }
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new InternalChatCompletionResponseMessage(content ?? new ChangeTrackingList<ChatMessageContentPart>(), refusal, toolCalls ?? new ChangeTrackingList<ChatToolCall>(), role, functionCall, serializedAdditionalRawData);
    }
}
