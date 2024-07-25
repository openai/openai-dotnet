using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.InternalCreateChatCompletionStreamResponseChoice>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
internal partial class InternalCreateChatCompletionStreamResponseChoice : IJsonModel<InternalCreateChatCompletionStreamResponseChoice>
{
    // CUSTOM:
    // - Made FinishReason nullable.
    void IJsonModel<InternalCreateChatCompletionStreamResponseChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalCreateChatCompletionStreamResponseChoice, writer, options);

    internal static void SerializeInternalCreateChatCompletionStreamResponseChoice(InternalCreateChatCompletionStreamResponseChoice instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("delta"u8);
        writer.WriteObjectValue(instance.Delta, options);
        if (Optional.IsDefined(instance.Logprobs))
        {
            if (instance.Logprobs != null)
            {
                writer.WritePropertyName("logprobs"u8);
                writer.WriteObjectValue(instance.Logprobs, options);
            }
            else
            {
                writer.WriteNull("logprobs");
            }
        }
        if (Optional.IsDefined(instance.FinishReason))
        {
            if (instance.FinishReason != null)
            {
                writer.WritePropertyName("finish_reason"u8);
                writer.WriteStringValue(instance.FinishReason.Value.ToSerialString());
            }
            else
            {
                writer.WriteNull("finish_reason");
            }
        }
        writer.WritePropertyName("index"u8);
        writer.WriteNumberValue(instance.Index);
        writer.WriteSerializedAdditionalRawData(instance.SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    internal static InternalCreateChatCompletionStreamResponseChoice DeserializeInternalCreateChatCompletionStreamResponseChoice(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        InternalChatCompletionStreamResponseDelta delta = default;
        InternalCreateChatCompletionStreamResponseChoiceLogprobs logprobs = default;
        ChatFinishReason? finishReason = default;
        int index = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("delta"u8))
            {
                delta = InternalChatCompletionStreamResponseDelta.DeserializeInternalChatCompletionStreamResponseDelta(property.Value, options);
                continue;
            }
            if (property.NameEquals("logprobs"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    logprobs = null;
                    continue;
                }
                logprobs = InternalCreateChatCompletionStreamResponseChoiceLogprobs.DeserializeInternalCreateChatCompletionStreamResponseChoiceLogprobs(property.Value, options);
                continue;
            }
            if (property.NameEquals("finish_reason"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    finishReason = null;
                    continue;
                }
                finishReason = property.Value.GetString().ToChatFinishReason();
                continue;
            }
            if (property.NameEquals("index"u8))
            {
                index = property.Value.GetInt32();
                continue;
            }
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new InternalCreateChatCompletionStreamResponseChoice(delta, logprobs, finishReason, index, serializedAdditionalRawData);
    }
}
