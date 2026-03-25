using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Audio;

// CUSTOM: This type is not its own object. Instead, it represents a union, and as such, it must directly forward
// its serialization and deserialization logic to the components of said union.
public partial class AudioTranscriptionChunkingStrategy
{
    // CUSTOM: Edited to remove calls to WriteStartObject() and WriteEndObject().
    void IJsonModel<AudioTranscriptionChunkingStrategy>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        JsonModelWriteCore(writer, options);
    }

    // CUSTOM: Edited to serialize the different components of the union.
    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<AudioTranscriptionChunkingStrategy>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(AudioTranscriptionChunkingStrategy)} does not support writing '{format}' format.");
        }
        if (Optional.IsDefined(DefaultChunkingStrategy))
        {
            writer.WriteStringValue(DefaultChunkingStrategy.Value.ToString());
        }
        if (Optional.IsDefined(CustomChunkingStrategy))
        {
            writer.WriteObjectValue(CustomChunkingStrategy, options);
        }
    }

    // CUSTOM: Edited to deserialize the different components of the union.
    internal static AudioTranscriptionChunkingStrategy DeserializeAudioTranscriptionChunkingStrategy(JsonElement element, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        AudioTranscriptionDefaultChunkingStrategy? defaultChunkingStrategy = default;
        AudioTranscriptionCustomChunkingStrategy customChunkingStrategy = default;
        IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();

        if (element.ValueKind == JsonValueKind.String)
        {
            defaultChunkingStrategy = new AudioTranscriptionDefaultChunkingStrategy(element.GetString());
        }
        else
        {
            customChunkingStrategy = AudioTranscriptionCustomChunkingStrategy.DeserializeAudioTranscriptionCustomChunkingStrategy(element, options);
        }

        return new AudioTranscriptionChunkingStrategy(defaultChunkingStrategy, customChunkingStrategy, additionalBinaryDataProperties);
    }
}
