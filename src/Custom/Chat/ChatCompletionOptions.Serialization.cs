using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Chat;

public partial class ChatCompletionOptions
{
    // CUSTOM: Added custom serialization to circumvent serialization failure of required 'messages', which is moved
    //         to a method parameter and should not block object serialization validity.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeMessagesValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (Messages is not null)
        {
            writer.WriteStartArray();
            foreach (var item in Messages)
            {
                writer.WriteObjectValue<ChatMessage>(item, options);
            }
            writer.WriteEndArray();
        }
        else
        {
            writer.WriteNullValue();
        }
    }

    // CUSTOM: Added custom serialization to treat a single string as a collection of strings with one item.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeStopSequencesValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartArray();
        foreach (var item in StopSequences)
        {
            writer.WriteStringValue(item);
        }
        writer.WriteEndArray();
    }

    // CUSTOM: Added custom serialization to treat a single string as a collection of strings with one item.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeStopSequencesValue(JsonProperty property, ref IList<string> stop)
    {
        if (property.Value.ValueKind == JsonValueKind.Null)
        {
            stop = null;
        }
        else if (property.Value.ValueKind == JsonValueKind.String)
        {
            List<string> array = [property.Value.GetString()];
            stop = array;
        }
        else
        {
            List<string> array = [];
            foreach (var item in property.Value.EnumerateArray())
            {
                array.Add(item.GetString());
            }
            stop = array;
        }
    }

    // CUSTOM: Added custom serialization to represent tokens as integers instead of strings.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeLogitBiasesValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        foreach (var item in LogitBiases)
        {
            writer.WritePropertyName(item.Key.ToString(CultureInfo.InvariantCulture));
            writer.WriteNumberValue(item.Value);
        }
        writer.WriteEndObject();
    }

    // CUSTOM: Added custom serialization to represent tokens as integers instead of strings.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeLogitBiasesValue(JsonProperty property, ref IDictionary<int, int> logitBias)
    {
        if (property.Value.ValueKind == JsonValueKind.Null)
        {
            logitBias = null;
        }
        else
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            foreach (var property0 in property.Value.EnumerateObject())
            {
                dictionary.Add(int.Parse(property0.Name, CultureInfo.InvariantCulture), property0.Value.GetInt32());
            }
            logitBias = dictionary;
        }
    }
}
