using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Share role-aware string shorthand normalization and strict array validation across Decisions deserialization hooks.
internal static class DecisionsSerializationHelpers
{
    internal static IList<T> DeserializeTextOrParts<T>(
        JsonProperty property,
        Func<string, T> createTextPart,
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, T> deserializePart,
        ModelReaderWriterOptions options)
    {
        if (property.Value.ValueKind == JsonValueKind.String)
        {
            return [createTextPart(property.Value.GetString())];
        }

        if (property.Value.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException($"Expected decision {property.Name} to be a string or an array but found {property.Value.ValueKind}.");
        }

        List<T> parts = [];
        foreach (JsonElement item in property.Value.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException($"Expected decision {property.Name} array elements to be objects but found {item.ValueKind}.");
            }
            parts.Add(deserializePart(item, item.GetUtf8Bytes(), options));
        }
        return parts;
    }

    // CUSTOM: Normalize a single free-form JSON object to the one-item data array and preserve object JSON verbatim.
    internal static IList<BinaryData> DeserializeJsonObjects(JsonProperty property)
    {
        if (property.Value.ValueKind == JsonValueKind.Object)
        {
            return [BinaryData.FromBytes(property.Value.GetUtf8Bytes())];
        }

        if (property.Value.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException($"Expected decision {property.Name} to be an object or an array of objects but found {property.Value.ValueKind}.");
        }

        List<BinaryData> objects = [];
        foreach (JsonElement item in property.Value.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException($"Expected decision {property.Name} array elements to be objects but found {item.ValueKind}.");
            }
            objects.Add(BinaryData.FromBytes(item.GetUtf8Bytes()));
        }
        return objects;
    }
}
