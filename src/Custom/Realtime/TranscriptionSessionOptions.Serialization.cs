using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

// The GA API requires a different structure for transcription sessions.
// - Properties must be nested under audio.input instead of being flat
// - The session type must be included

[CodeGenSuppress("JsonModelWriteCore", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class TranscriptionSessionOptions
{
    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<TranscriptionSessionOptions>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(TranscriptionSessionOptions)} does not support writing '{format}' format.");
        }

        // GA API requires the session type to be specified
        if (_additionalBinaryDataProperties?.ContainsKey("type") != true)
        {
            writer.WritePropertyName("type"u8);
            writer.WriteStringValue("transcription");
        }

        // GA API uses nested audio.input structure for transcription sessions
        bool hasAudioProperties = Optional.IsDefined(InputAudioFormat) ||
                                   Optional.IsDefined(InputTranscriptionOptions) ||
                                   Optional.IsDefined(TurnDetectionOptions) ||
                                   Optional.IsDefined(InputNoiseReductionOptions);

        if (hasAudioProperties && _additionalBinaryDataProperties?.ContainsKey("audio") != true)
        {
            writer.WritePropertyName("audio"u8);
            writer.WriteStartObject();
            writer.WritePropertyName("input"u8);
            writer.WriteStartObject();

            if (Optional.IsDefined(InputAudioFormat))
            {
                writer.WritePropertyName("format"u8);
                // GA API uses object format like {"type":"audio/pcm","rate":24000}
                writer.WriteStartObject();
                var audioFormat = InputAudioFormat.Value.ToString();
                string formatType = audioFormat switch
                {
                    "pcm16" => "audio/pcm",
                    "g711_ulaw" => "audio/pcm",
                    "g711_alaw" => "audio/pcm",
                    _ => audioFormat
                };
                writer.WritePropertyName("type"u8);
                writer.WriteStringValue(formatType);
                if (audioFormat == "pcm16")
                {
                    writer.WritePropertyName("rate"u8);
                    writer.WriteNumberValue(24000);
                }
                writer.WriteEndObject();
            }
            if (Optional.IsDefined(InputTranscriptionOptions))
            {
                writer.WritePropertyName("transcription"u8);
                writer.WriteObjectValue(InputTranscriptionOptions, options);
            }
            if (Optional.IsDefined(InputNoiseReductionOptions))
            {
                writer.WritePropertyName("noise_reduction"u8);
                writer.WriteObjectValue(InputNoiseReductionOptions, options);
            }
            if (Optional.IsDefined(TurnDetectionOptions))
            {
                writer.WritePropertyName("turn_detection"u8);
                writer.WriteObjectValue(TurnDetectionOptions, options);
            }

            writer.WriteEndObject(); // end input
            writer.WriteEndObject(); // end audio
        }

        if (Optional.IsCollectionDefined(Include) && _additionalBinaryDataProperties?.ContainsKey("include") != true)
        {
            writer.WritePropertyName("include"u8);
            writer.WriteStartArray();
            foreach (string item in Include)
            {
                if (item == null)
                {
                    writer.WriteNullValue();
                    continue;
                }
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
        }
        if (Optional.IsDefined(ClientSecret) && _additionalBinaryDataProperties?.ContainsKey("client_secret") != true)
        {
            writer.WritePropertyName("client_secret"u8);
            writer.WriteObjectValue(ClientSecret, options);
        }
        if (Optional.IsCollectionDefined(_internalModalities) && _additionalBinaryDataProperties?.ContainsKey("modalities") != true)
        {
            writer.WritePropertyName("modalities"u8);
            writer.WriteStartArray();
            foreach (InternalRealtimeRequestSessionModality item in _internalModalities)
            {
                writer.WriteStringValue(item.ToString());
            }
            writer.WriteEndArray();
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
