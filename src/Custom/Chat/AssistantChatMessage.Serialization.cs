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

    protected internal override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(Role.ToSerialString());
        ChatMessageContentPart.WriteCoreContentPartList(Content, writer, options);
        writer.WriteOptionalProperty("refusal"u8, Refusal, options);
        writer.WriteOptionalProperty("name"u8, ParticipantName, options);
        writer.WriteOptionalCollection("tool_calls"u8, ToolCalls, options);
        writer.WriteOptionalProperty("function_call"u8, FunctionCall, options);
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}
