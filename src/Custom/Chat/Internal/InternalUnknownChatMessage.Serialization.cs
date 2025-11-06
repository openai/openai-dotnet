using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class InternalUnknownChatMessage : IJsonModel<ChatMessage>
{
    void IJsonModel<ChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance<ChatMessage, InternalUnknownChatMessage>(this, WriteCore, writer, options);

    internal static void WriteCore(InternalUnknownChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        instance.WriteCore(writer, options);
    }

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

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        writer.WriteEndObject();
    }
}
