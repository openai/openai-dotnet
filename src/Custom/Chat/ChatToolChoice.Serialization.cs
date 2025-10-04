using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatToolChoice>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class ChatToolChoice : IJsonModel<ChatToolChoice>
{
    void IJsonModel<ChatToolChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeChatToolChoice, writer, options);

    internal static void SerializeChatToolChoice(ChatToolChoice instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (instance.Patch.Contains("$"u8))
        {
            writer.WriteRawValue(instance.Patch.GetJson("$"u8));
            return;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        if (instance._predefined)
        {
            writer.WriteStringValue(instance._predefinedValue);
        }
        else
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type"u8);
            writer.WriteStringValue(instance._type.ToString());
            writer.WritePropertyName("function"u8);
            writer.WriteObjectValue(instance._function, options);
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            instance.Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            writer.WriteEndObject();
        }
    }

    internal static ChatToolChoice DeserializeChatToolChoice(JsonElement element, BinaryData data, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        else if (element.ValueKind == JsonValueKind.String)
        {
            return new ChatToolChoice(
                predefined: true,
                predefinedValue: element.ToString(),
                type: null,
                function: null,
                patch: default);
        }
        else
        {
            string type = default;
            InternalChatCompletionNamedToolChoiceFunction function = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("type"u8))
                {
                    type = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("function"u8))
                {
                    function = InternalChatCompletionNamedToolChoiceFunction.DeserializeInternalChatCompletionNamedToolChoiceFunction(property.Value, property.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (true)
                {
                    patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(property.Name)], property.Value.GetUtf8Bytes());
                }
            }
            return new ChatToolChoice(
                predefined: false,
                predefinedValue: null,
                type: FunctionType,
                function: new InternalChatCompletionNamedToolChoiceFunction(function.Name),
                patch: patch);
        }
    }
}