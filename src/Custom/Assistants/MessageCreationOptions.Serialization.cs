using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants
{
    public partial class MessageCreationOptions : IJsonModel<MessageCreationOptions>
    {
        private void SerializeContent(Utf8JsonWriter writer, ModelReaderWriterOptions options = null)
        {
            if (Content.Count == 1 && Content[0] is InternalMessageContentTextObject textContent)
            {
                writer.WriteStringValue(textContent.Text);
            }
            else
            {
                writer.WriteStartArray();
                foreach (var item in Content)
                {
                    writer.WriteObjectValue<MessageContent>(item, options);
                }
                writer.WriteEndArray();
            }
        }
    }
}
