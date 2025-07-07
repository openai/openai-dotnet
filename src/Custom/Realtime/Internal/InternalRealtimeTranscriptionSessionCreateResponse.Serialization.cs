using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Realtime;

[CodeGenSerialization(nameof(InputAudioFormat), DeserializationValueHook = nameof(DeserializeInputAudioFormatValue), SerializationValueHook = nameof(SerializeInputAudioFormatValue))]
internal partial class InternalRealtimeTranscriptionSessionCreateResponse
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeInputAudioFormatValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => writer.WriteStringValue(InputAudioFormat.ToString());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeInputAudioFormatValue(JsonProperty property, ref RealtimeAudioFormat inputAudioFormat, ModelReaderWriterOptions options = null)
    {
        inputAudioFormat = new(property.Value.GetString());
    }
}
