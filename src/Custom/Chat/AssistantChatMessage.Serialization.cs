using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.AssistantChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class AssistantChatMessage : IJsonModel<AssistantChatMessage>
{
    void IJsonModel<AssistantChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeAssistantChatMessage, writer, options);

    internal static void SerializeAssistantChatMessage(AssistantChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    internal override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(Role.ToSerialString());

        // Content is optional, can be a single string or a collection of ChatMessageContentPart.
        if (Optional.IsDefined(Content) && Content.IsInnerCollectionDefined())
        {
            if (Content.Count > 0)
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
        }

        writer.WriteOptionalProperty("refusal"u8, Refusal, options);
        writer.WriteOptionalProperty("name"u8, ParticipantName, options);
        writer.WriteOptionalCollection("tool_calls"u8, ToolCalls, options);
        writer.WriteOptionalProperty("function_call"u8, FunctionCall, options);
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}
