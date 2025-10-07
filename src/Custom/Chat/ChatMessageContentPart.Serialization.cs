using OpenAI.Files;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatMessageContentPart>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class ChatMessageContentPart : IJsonModel<ChatMessageContentPart>
{
    void IJsonModel<ChatMessageContentPart>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, WriteCoreContentPart, writer, options);

    internal static void WriteCoreContentPart(ChatMessageContentPart instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (instance.Patch.Contains("$"u8))
        {
            writer.WriteRawValue(instance.Patch.GetJson("$"u8));
            return;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        writer.WriteStartObject();
        writer.WritePropertyName("type"u8);
        writer.WriteStringValue(instance._kind.ToSerialString());

        if (instance._kind == ChatMessageContentPartKind.Text)
        {
            writer.WritePropertyName("text"u8);
            writer.WriteStringValue(instance._text);
        }
        else if (instance._kind == ChatMessageContentPartKind.Refusal)
        {
            writer.WritePropertyName("refusal"u8);
            writer.WriteStringValue(instance._refusal);
        }
        else if (instance._kind == ChatMessageContentPartKind.Image)
        {
            writer.WritePropertyName("image_url"u8);
            writer.WriteObjectValue(instance._imageUri, options);
        }
        else if (instance._kind == ChatMessageContentPartKind.InputAudio)
        {
            writer.WritePropertyName("input_audio"u8);
            writer.WriteObjectValue(instance._inputAudio, options);
        }
        else if (instance._kind == ChatMessageContentPartKind.File)
        {
            writer.WritePropertyName("file"u8);
            writer.WriteObjectValue(instance._fileFile, options);
        }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        instance.Patch.WriteTo(writer);
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        writer.WriteEndObject();
    }

    internal static ChatMessageContentPart DeserializeChatMessageContentPart(JsonElement element, BinaryData data, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        ChatMessageContentPartKind kind = default;
        string text = default;
        string refusal = default;
        InternalChatCompletionRequestMessageContentPartImageImageUrl imageUri = default;
        InternalChatCompletionRequestMessageContentPartAudioInputAudio inputAudio = default;
        InternalChatCompletionRequestMessageContentPartFileFile fileFile = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("type"u8))
            {
                kind = property.Value.GetString().ToChatMessageContentPartKind();
                continue;
            }
            if (property.NameEquals("text"u8))
            {
                text = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("image_url"u8))
            {
                imageUri = InternalChatCompletionRequestMessageContentPartImageImageUrl.DeserializeInternalChatCompletionRequestMessageContentPartImageImageUrl(property.Value, property.Value.GetUtf8Bytes(), options);
                continue;
            }
            if (property.NameEquals("refusal"u8))
            {
                refusal = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("input_audio"u8))
            {
                inputAudio = InternalChatCompletionRequestMessageContentPartAudioInputAudio
                    .DeserializeInternalChatCompletionRequestMessageContentPartAudioInputAudio(property.Value, property.Value.GetUtf8Bytes(), options);
                continue;
            }
            if (property.NameEquals("file"u8))
            {
                fileFile = InternalChatCompletionRequestMessageContentPartFileFile
                    .DeserializeInternalChatCompletionRequestMessageContentPartFileFile(property.Value, property.Value.GetUtf8Bytes(), options);
                continue;
            }
            if (true)
            {
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(property.Name)], property.Value.GetUtf8Bytes());
            }
        }
        return new ChatMessageContentPart(kind, text, imageUri, refusal, inputAudio, fileFile, patch);
    }
}
