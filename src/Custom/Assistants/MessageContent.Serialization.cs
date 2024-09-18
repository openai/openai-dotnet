using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.MessageContent>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public abstract partial class MessageContent : IJsonModel<MessageContent>
{
    void IJsonModel<MessageContent>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, WriteCore, writer, options);

    internal static void WriteCore(MessageContent instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    internal abstract void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);

    internal static MessageContent DeserializeMessageContent(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        foreach (var property in element.EnumerateObject())
        {
            if (element.TryGetProperty("type", out JsonElement discriminator))
            {
                switch (discriminator.GetString())
                {
                    case "image_file": return InternalMessageImageFileContent.DeserializeInternalMessageImageFileContent(element, options);
                    case "image_url": return InternalMessageImageUrlContent.DeserializeInternalMessageImageUrlContent(element, options);
                    case "text": return InternalResponseMessageTextContent.DeserializeInternalResponseMessageTextContent(element, options);
                    default: return null;
                }
            }
        }

        return null;
    }
}