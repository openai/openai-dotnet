using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.FunctionChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class FunctionChatMessage : IJsonModel<FunctionChatMessage>
{
    void IJsonModel<FunctionChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeFunctionChatMessage, writer, options);

    internal static void SerializeFunctionChatMessage(FunctionChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        instance.WriteCore(writer, options);
    }

    internal override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(Role.ToSerialString());
        writer.WritePropertyName("name"u8);
        writer.WriteStringValue(FunctionName);

        // Content is required, can be a single string or null.
        if (Optional.IsDefined(Content) && Content.IsInnerCollectionDefined())
        {
            writer.WritePropertyName("content"u8);
            if (Content.Count > 0 && Content[0].Text != null)
            {
                writer.WriteStringValue(Content[0].Text);
            }
            else
            {
                writer.WriteNullValue();
            }
        }

        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}