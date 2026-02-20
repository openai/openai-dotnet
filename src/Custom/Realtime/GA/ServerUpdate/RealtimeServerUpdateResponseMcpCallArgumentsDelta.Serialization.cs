using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Realtime;

[CodeGenSerialization(nameof(Delta), SerializationValueHook = nameof(SerializeDeltaValue), DeserializationValueHook = nameof(DeserializeDeltaValue))]
public partial class GARealtimeServerUpdateResponseMcpCallArgumentsDelta
{
    // CUSTOM: The REST API serializes this as a string.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeDeltaValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string value = Delta.ToMemory().IsEmpty
            ? string.Empty
            : Delta.ToString();
        writer.WriteStringValue(value);
    }

    // CUSTOM: Replaced the call to GetRawText() for a call to GetString() because otherwise the starting and ending
    // quotes of the string are included in the BinaryData. While this is actually a string in the REST API, we want to
    // handle it as JSON binary data instead.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeDeltaValue(JsonProperty property, ref BinaryData delta, ModelReaderWriterOptions options = null)
    {
        delta = BinaryData.FromString(property.Value.GetString());
    }
}