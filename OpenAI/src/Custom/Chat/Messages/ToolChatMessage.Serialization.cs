using Microsoft.TypeSpec.Generator.Customizations;
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
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (Patch.Contains("$"u8))
        {
            writer.WriteRawValue(Patch.GetJson("$"u8));
            return;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        writer.WriteStartObject();
        WriteRoleProperty(writer, options);
        writer.WritePropertyName("tool_call_id"u8);
        writer.WriteStringValue(ToolCallId);
        WriteContentProperty(writer, options);
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        writer.WriteEndObject();
    }
}
