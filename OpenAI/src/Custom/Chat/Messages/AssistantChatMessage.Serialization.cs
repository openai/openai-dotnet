using Microsoft.TypeSpec.Generator.Customizations;
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
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (Patch.Contains("$"u8))
        {
            writer.WriteRawValue(Patch.GetJson("$"u8));
            return;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        writer.WriteStartObject();
        WriteRoleProperty(writer, options);
        WriteContentProperty(writer, options);
        writer.WriteOptionalProperty("refusal"u8, Refusal, options);
        writer.WriteOptionalProperty("name"u8, ParticipantName, options);
        writer.WriteOptionalCollection("tool_calls"u8, ToolCalls, options);
        writer.WriteOptionalProperty("function_call"u8, FunctionCall, options);
        writer.WriteOptionalProperty("audio"u8, OutputAudioReference, options);
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        Patch.WriteTo(writer);
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        writer.WriteEndObject();
    }
}
