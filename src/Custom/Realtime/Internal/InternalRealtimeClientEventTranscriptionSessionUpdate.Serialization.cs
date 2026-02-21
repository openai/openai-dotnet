using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

// The GA API uses "session.update" for both conversation and transcription sessions.
// The TypeSpec defines "transcription_session.update" but that's no longer accepted by the API.
// This custom serialization overrides the event type to use "session.update".

[Experimental("OPENAI002")]
[CodeGenType("RealtimeClientEventTranscriptionSessionUpdate")]
[CodeGenSuppress("JsonModelWriteCore", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
internal partial class InternalRealtimeClientEventTranscriptionSessionUpdate
{
    protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<InternalRealtimeClientEventTranscriptionSessionUpdate>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(InternalRealtimeClientEventTranscriptionSessionUpdate)} does not support writing '{format}' format.");
        }

        // Write "type" as "session.update" instead of the generated "transcription_session.update"
        if (_additionalBinaryDataProperties?.ContainsKey("type") != true)
        {
            writer.WritePropertyName("type"u8);
            writer.WriteStringValue("session.update");
        }

        if (Optional.IsDefined(EventId) && _additionalBinaryDataProperties?.ContainsKey("event_id") != true)
        {
            writer.WritePropertyName("event_id"u8);
            writer.WriteStringValue(EventId);
        }

        if (_additionalBinaryDataProperties?.ContainsKey("session") != true)
        {
            writer.WritePropertyName("session"u8);
            writer.WriteObjectValue(Session, options);
        }

        if (_additionalBinaryDataProperties != null)
        {
            foreach (var item in _additionalBinaryDataProperties)
            {
                if (ModelSerializationExtensions.IsSentinelValue(item.Value))
                {
                    continue;
                }
                writer.WritePropertyName(item.Key);
#if NET6_0_OR_GREATER
                writer.WriteRawValue(item.Value);
#else
                using (JsonDocument document = JsonDocument.Parse(item.Value))
                {
                    JsonSerializer.Serialize(writer, document.RootElement);
                }
#endif
            }
        }
    }
}
