using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.InternalChatCompletionStreamResponseDelta>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
internal partial class InternalChatCompletionStreamResponseDelta : IJsonModel<InternalChatCompletionStreamResponseDelta>
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

    void IJsonModel<InternalChatCompletionStreamResponseDelta>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalChatCompletionStreamResponseDelta, writer, options);

    internal static void SerializeInternalChatCompletionStreamResponseDelta(InternalChatCompletionStreamResponseDelta instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
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
        if (Optional.IsDefined(instance.FunctionCall))
        {
            writer.WritePropertyName("function_call"u8);
            writer.WriteObjectValue(instance.FunctionCall, options);
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
        if (Optional.IsDefined(instance.Role))
        {
            writer.WritePropertyName("role"u8);
            writer.WriteStringValue(instance.Role.Value.ToSerialString());
        }
        writer.WriteSerializedAdditionalRawData(instance.SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    internal static InternalChatCompletionStreamResponseDelta DeserializeInternalChatCompletionStreamResponseDelta(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        IReadOnlyList<ChatMessageContentPart> content = default;
        StreamingChatFunctionCallUpdate functionCall = default;
        IReadOnlyList<StreamingChatToolCallUpdate> toolCalls = default;
        ChatMessageRole? role = default;
        string refusal = default;
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
            if (property.NameEquals("function_call"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                functionCall = StreamingChatFunctionCallUpdate.DeserializeStreamingChatFunctionCallUpdate(property.Value, options);
                continue;
            }
            if (property.NameEquals("tool_calls"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                List<StreamingChatToolCallUpdate> array = new List<StreamingChatToolCallUpdate>();
                foreach (var item in property.Value.EnumerateArray())
                {
                    array.Add(StreamingChatToolCallUpdate.DeserializeStreamingChatToolCallUpdate(item, options));
                }
                toolCalls = array;
                continue;
            }
            if (property.NameEquals("role"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                role = property.Value.GetString().ToChatMessageRole();
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
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new InternalChatCompletionStreamResponseDelta(content ?? new ChangeTrackingList<ChatMessageContentPart>(), functionCall, toolCalls ?? new ChangeTrackingList<StreamingChatToolCallUpdate>(), role, refusal, serializedAdditionalRawData);
    }
}
