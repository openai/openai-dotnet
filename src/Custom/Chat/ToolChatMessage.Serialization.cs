using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ToolChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class ToolChatMessage : IJsonModel<ToolChatMessage>
{
    void IJsonModel<ToolChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeToolChatMessage, writer, options);

    internal static void SerializeToolChatMessage(ToolChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    internal override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(Role.ToSerialString());
        writer.WritePropertyName("tool_call_id"u8);
        writer.WriteStringValue(ToolCallId);

        // Content is required, can be a single string or a collection of ChatMessageContentPart.
        if (Optional.IsDefined(Content) && Content.IsInnerCollectionDefined())
        {
            writer.WritePropertyName("content"u8);
            if (Content.Count == 1 && Content[0].Text != null)
            {
                writer.WriteStringValue(Content[0].Text);
            }
            else
            {
                writer.WriteStartArray();
                foreach (ChatMessageContentPart part in Content)
                {
                    writer.WriteObjectValue(part, options);
                }
                writer.WriteEndArray();
            }
        }

        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}
