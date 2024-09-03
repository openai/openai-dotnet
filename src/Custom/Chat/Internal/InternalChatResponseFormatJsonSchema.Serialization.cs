using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.InternalChatResponseFormatJsonSchema>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
internal partial class InternalChatResponseFormatJsonSchema : IJsonModel<InternalChatResponseFormatJsonSchema>
{
    void IJsonModel<InternalChatResponseFormatJsonSchema>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalChatResponseFormatJsonSchema, writer, options);

    internal static void SerializeInternalChatResponseFormatJsonSchema(InternalChatResponseFormatJsonSchema instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    protected internal override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        if (SerializedAdditionalRawData?.ContainsKey("json_schema") != true)
        {
            writer.WritePropertyName("json_schema"u8);
            writer.WriteObjectValue(JsonSchema, options);
        }
        if (SerializedAdditionalRawData?.ContainsKey("type") != true)
        {
            writer.WritePropertyName("type"u8);
            writer.WriteStringValue(Type);
        }
        if (SerializedAdditionalRawData != null)
        {
            foreach (var item in SerializedAdditionalRawData)
            {
                if (ModelSerializationExtensions.IsSentinelValue(item.Value))
                {
                    continue;
                }
                writer.WritePropertyName(item.Key);
#if NET6_0_OR_GREATER
				writer.WriteRawValue(item.Value);
#else
                using (JsonDocument document = JsonDocument.Parse(item.Value))
                {
                    JsonSerializer.Serialize(writer, document.RootElement);
                }
#endif
            }
        }
        writer.WriteEndObject();
    }
}
