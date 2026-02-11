using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Realtime;

// Custom deserialization to handle GA API sending "session.updated" for transcription sessions
// The type field in the JSON will be "session.updated" but we need to return TranscriptionSessionConfigured kind

[CodeGenSuppress("DeserializeTranscriptionSessionConfiguredUpdate", typeof(JsonElement), typeof(ModelReaderWriterOptions))]
public partial class TranscriptionSessionConfiguredUpdate
{
    internal static TranscriptionSessionConfiguredUpdate DeserializeTranscriptionSessionConfiguredUpdate(JsonElement element, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        // Always use TranscriptionSessionConfigured kind for this update type
        // regardless of whether the type field is "session.updated" or "transcription_session.updated"
        RealtimeUpdateKind kind = RealtimeUpdateKind.TranscriptionSessionConfigured;
        string eventId = default;
        IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
        InternalRealtimeTranscriptionSessionCreateResponse internalSession = default;

        foreach (var prop in element.EnumerateObject())
        {
            if (prop.NameEquals("type"u8))
            {
                // Skip - we always use TranscriptionSessionConfigured
                continue;
            }
            if (prop.NameEquals("event_id"u8))
            {
                eventId = prop.Value.GetString();
                continue;
            }
            if (prop.NameEquals("session"u8))
            {
                internalSession = InternalRealtimeTranscriptionSessionCreateResponse.DeserializeInternalRealtimeTranscriptionSessionCreateResponse(prop.Value, options);
                continue;
            }
            additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
        }
        return new TranscriptionSessionConfiguredUpdate(kind, eventId, additionalBinaryDataProperties, internalSession);
    }
}
