using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.SystemChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class SystemChatMessage : IJsonModel<SystemChatMessage>
{
    void IJsonModel<SystemChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeSystemChatMessage, writer, options);

    internal static void SerializeSystemChatMessage(SystemChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    internal override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(Role.ToSerialString());

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

        writer.WriteOptionalProperty("name"u8, ParticipantName, options);
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}