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

#pragma warning disable SCME0001
        if (Optional.IsDefined(DefaultToolChoice) && !Patch.Contains("$.default_tool_choice"u8))
        {
            writer.WriteStringValue(DefaultToolChoice.Value.ToString());
        }
        if (Optional.IsDefined(CustomToolChoice) && !Patch.Contains("$.custom_tool_choice"u8))
        {
            writer.WriteObjectValue(CustomToolChoice, options);
        }
#pragma warning restore SCME0001
    }

    internal static ResponseToolChoice DeserializeResponseToolChoice(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        ResponseDefaultToolChoice? defaultToolChoice = default;
        ResponseCustomToolChoice customToolChoice = default;
#pragma warning disable SCME0001
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001

        if (element.ValueKind == JsonValueKind.String)
        {
            defaultToolChoice = new ResponseDefaultToolChoice(element.GetString());
        }
        else
        {
            customToolChoice = ResponseCustomToolChoice.DeserializeResponseCustomToolChoice(element, element.GetUtf8Bytes(), options);
        }

        return new ResponseToolChoice(defaultToolChoice, customToolChoice, patch);
    }
}
