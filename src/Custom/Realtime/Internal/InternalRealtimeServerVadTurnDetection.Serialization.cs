using OpenAI.VectorStores;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeServerVadTurnDetection")]
[CodeGenSerialization(nameof(PrefixPaddingMs), DeserializationValueHook = nameof(DeserializeMillisecondDuration), SerializationValueHook = nameof(SerializePrefixPaddingMs))]
[CodeGenSerialization(nameof(SilenceDurationMs), DeserializationValueHook = nameof(DeserializeMillisecondDuration), SerializationValueHook = nameof(SerializeSilenceDurationMs))]
internal partial class InternalRealtimeServerVadTurnDetection
{
    private static void DeserializeMillisecondDuration(JsonProperty property, ref TimeSpan? duration)
    {
        if (property.Value.ValueKind == JsonValueKind.Number)
        {
            duration = TimeSpan.FromMilliseconds(property.Value.GetInt32());
        }
    }

    private void SerializePrefixPaddingMs(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        SerializeMillisecondDuration(writer, PrefixPaddingMs);
    }

    private void SerializeSilenceDurationMs(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        SerializeMillisecondDuration(writer, SilenceDurationMs);
    }

    private static void SerializeMillisecondDuration(Utf8JsonWriter writer, TimeSpan? duration)
    {
        if (duration.HasValue)
        {
            writer.WriteNumberValue((int)duration.Value.TotalMilliseconds);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
