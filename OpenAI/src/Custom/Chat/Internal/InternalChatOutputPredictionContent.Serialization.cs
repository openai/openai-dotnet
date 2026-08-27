using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSerialization(nameof(Content), SerializationValueHook = nameof(SerializeContentValue), DeserializationValueHook = nameof(DeserializeContentValue))]
internal partial class InternalChatOutputPredictionContent
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeContentValue(Utf8JsonWriter writer, ModelReaderWriterOptions options = null)
        => Content.WriteTo(writer, options);

    private static void DeserializeContentValue(JsonProperty property, ref ChatMessageContent content)
    {
        content = ChatMessageContent.DeserializeChatMessageContent(property.Value);
    }
}