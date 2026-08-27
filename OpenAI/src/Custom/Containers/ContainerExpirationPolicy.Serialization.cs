using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Containers;

[CodeGenSerialization(nameof(Duration), SerializationValueHook = nameof(SerializeDurationValue), DeserializationValueHook = nameof(DeserializeDurationValue))]
public partial class ContainerExpirationPolicy
{
    // CUSTOM: Serialized from a TimeSpan to an integer using a serialization hook, because TypeSpec does not
    // currently support encoding durations as integer minutes: https://github.com/microsoft/typespec/issues/8690
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeDurationValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteNumberValue(Convert.ToInt64(Math.Round(Duration.Value.TotalMinutes)));
    }

    // CUSTOM: Deserialized from an integer to a TimeSpan using a deserialization hook, because TypeSpec does not
    // currently support encoding durations as integer minutes: https://github.com/microsoft/typespec/issues/8690
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeDurationValue(JsonProperty property, ref TimeSpan? duration, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.Null)
        {
            return;
        }
        duration = TimeSpan.FromMinutes(property.Value.GetInt64());
    }
}
