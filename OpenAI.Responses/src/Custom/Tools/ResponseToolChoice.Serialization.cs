using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class ResponseToolChoice
{
    void IJsonModel<ResponseToolChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
#pragma warning disable SCME0001
        if (Patch.Contains("$"u8))
        {
            writer.WriteRawValue(Patch.GetJson("$"u8));
            return;
        }
#pragma warning restore SCME0001

        JsonModelWriteCore(writer, options);
    }

    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<ResponseToolChoice>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(ResponseToolChoice)} does not support writing '{format}' format.");
        }

        // CUSTOM: The active component is serialized at the union root, without synthetic property names.
        if (Optional.IsDefined(DefaultToolChoice))
        {
            writer.WriteStringValue(DefaultToolChoice.Value.ToString());
        }
        else if (Optional.IsDefined(CustomToolChoice))
        {
            writer.WriteObjectValue(CustomToolChoice, options);
        }
    }

    internal static ResponseToolChoice DeserializeResponseToolChoice(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        ResponseToolChoiceEnum ? defaultToolChoice = default;
        ResponseToolChoiceObject customToolChoice = default;
#pragma warning disable SCME0001
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001

        if (element.ValueKind == JsonValueKind.String)
        {
            defaultToolChoice = new ResponseToolChoiceEnum (element.GetString());
        }
        else
        {
            customToolChoice = ResponseToolChoiceObject.DeserializeResponseToolChoiceObject (element, element.GetUtf8Bytes(), options);
        }

        return new ResponseToolChoice(defaultToolChoice, customToolChoice, patch);
    }

    // CUSTOM: Forward non-root wire paths to the object component of the flattened union.
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    private bool PropagateGet(ReadOnlySpan<byte> jsonPath, out JsonPatch.EncodedValue value)
    {
        value = default;
        return CustomToolChoice is not null
            && !jsonPath.SequenceEqual("$"u8)
            && CustomToolChoice.Patch.TryGetEncodedValue(jsonPath, out value);
    }

    private bool PropagateSet(ReadOnlySpan<byte> jsonPath, JsonPatch.EncodedValue value)
    {
        if (CustomToolChoice is null || jsonPath.SequenceEqual("$"u8))
        {
            return false;
        }

        CustomToolChoice.Patch.Set(jsonPath, value);
        return true;
    }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
}
