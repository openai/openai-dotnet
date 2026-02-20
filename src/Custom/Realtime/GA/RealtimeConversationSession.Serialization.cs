using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Realtime;

[CodeGenSerialization(nameof(MaxOutputTokenCount), SerializationValueHook = nameof(SerializeMaxOutputTokenCountValue), DeserializationValueHook = nameof(DeserializeMaxOutputTokenCountValue))]
public partial class GARealtimeConversationSession
{
    // CUSTOM: The REST API serializes this as a string.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeMaxOutputTokenCountValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (MaxOutputTokenCount == int.MaxValue)
        {
            writer.WriteStringValue("inf");
            return;
        }

        writer.WriteNumberValue(MaxOutputTokenCount.Value);
    }

    // CUSTOM: Replaced the call to GetRawText() for a call to GetString() because otherwise the starting and ending
    // quotes of the string are included in the BinaryData. While this is actually a string in the REST API, we want to
    // handle it as JSON binary data instead.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeMaxOutputTokenCountValue(JsonProperty property, ref int? maxOutputTokenCount, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.String)
        {
            maxOutputTokenCount = int.MaxValue;
            return;
        }

        maxOutputTokenCount = property.Value.GetInt32();
    }
}