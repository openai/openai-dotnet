using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;
public partial class MessageContent : IJsonModel<MessageContent>
{
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
