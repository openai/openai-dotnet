using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Chat;

internal partial class InternalChatCompletionStreamResponseDelta : IJsonModel<InternalChatCompletionStreamResponseDelta>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeContentValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (Content.Count > 0)
        {
            writer.WriteStringValue(Content[0].Text);
        }
        else
        {
            writer.WriteNullValue();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeContentValue(JsonProperty property, ref ChatMessageContent content, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.Null)
        {
            // This is a collection property. We must return an empty collection instead of a null value.
            content = new ChatMessageContent();
            return;
        }
        content = new ChatMessageContent(property.Value.ToString());
    }
}
