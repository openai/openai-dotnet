using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.InternalCreateChatCompletionStreamResponseChoice>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class InternalCreateChatCompletionStreamResponseChoice : IJsonModel<InternalCreateChatCompletionStreamResponseChoice>
{
    // CUSTOM:
    // - Made FinishReason nullable.
    void IJsonModel<InternalCreateChatCompletionStreamResponseChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalCreateChatCompletionStreamResponseChoice, writer, options);

    internal static void SerializeInternalCreateChatCompletionStreamResponseChoice(InternalCreateChatCompletionStreamResponseChoice instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (instance.Patch.Contains("$"u8))
        {
            writer.WriteRawValue(instance.Patch.GetJson("$"u8));
            return;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

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
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        instance.Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        writer.WriteEndObject();
    }

    internal static InternalCreateChatCompletionStreamResponseChoice DeserializeInternalCreateChatCompletionStreamResponseChoice(JsonElement element, BinaryData data, ModelReaderWriterOptions options = null)
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
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("delta"u8))
            {
                delta = InternalChatCompletionStreamResponseDelta.DeserializeInternalChatCompletionStreamResponseDelta(property.Value, property.Value.GetUtf8Bytes(), options);
                continue;
            }
            if (property.NameEquals("logprobs"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    logprobs = null;
                    continue;
                }
                logprobs = InternalCreateChatCompletionStreamResponseChoiceLogprobs.DeserializeInternalCreateChatCompletionStreamResponseChoiceLogprobs(property.Value, property.Value.GetUtf8Bytes(), options);
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
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(property.Name)], property.Value.GetUtf8Bytes());
            }
        }
        return new InternalCreateChatCompletionStreamResponseChoice(delta: delta, logprobs: logprobs, index: index, finishReason: finishReason, patch: patch);
    }
}
