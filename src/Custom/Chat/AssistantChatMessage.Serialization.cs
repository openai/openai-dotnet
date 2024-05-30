using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.AssistantChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class AssistantChatMessage : IJsonModel<AssistantChatMessage>
{
    void IJsonModel<AssistantChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeAssistantChatMessage, writer, options);

    internal static AssistantChatMessage DeserializeAssistantChatMessage(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        string name = default;
        IList<ChatToolCall> toolCalls = default;
        ChatFunctionCall functionCall = default;
        string role = default;
        IList<ChatMessageContentPart> content = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("name"u8))
            {
                name = property.Value.GetString();
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
            if (property.NameEquals("function_call"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    functionCall = null;
                    continue;
                }
                functionCall = ChatFunctionCall.DeserializeChatFunctionCall(property.Value, options);
                continue;
            }
            if (property.NameEquals("role"u8))
            {
                role = property.Value.GetString();
                continue;
            }
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
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new AssistantChatMessage(
            role,
            content ?? new ChangeTrackingList<ChatMessageContentPart>(),
            serializedAdditionalRawData,
            name,
            toolCalls ?? new ChangeTrackingList<ChatToolCall>(),
            functionCall);
    }

    internal static void SerializeAssistantChatMessage(AssistantChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        if (Optional.IsDefined(instance.ParticipantName))
        {
            writer.WritePropertyName("name"u8);
            writer.WriteStringValue(instance.ParticipantName);
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
        if (Optional.IsDefined(instance.FunctionCall))
        {
            if (instance.FunctionCall != null)
            {
                writer.WritePropertyName("function_call"u8);
                writer.WriteObjectValue(instance.FunctionCall, options);
            }
            else
            {
                writer.WriteNull("function_call");
            }
        }
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(instance.Role);
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
        writer.WriteSerializedAdditionalRawData(instance._serializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}
