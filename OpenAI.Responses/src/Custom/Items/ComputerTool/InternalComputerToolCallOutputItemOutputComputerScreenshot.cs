using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

internal partial class InternalComputerToolCallOutputItemOutputComputerScreenshot
{
    private DataUriValue _imageValue;

    [CodeGenMember("ImageUrl")]
    public string ImageUrl
    {
        get => _imageValue?.GetValue(cache: true);
        set => _imageValue = value is null ? null : new DataUriValue(value, parseDataUri: false);
    }

    internal void SetImageBytes(BinaryData imageBytes, string imageBytesMediaType)
    {
        _imageValue = new DataUriValue(imageBytes, imageBytesMediaType);
    }

    protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<InternalComputerToolCallOutputItemOutputComputerScreenshot>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(InternalComputerToolCallOutputItemOutputComputerScreenshot)} does not support writing '{format}' format.");
        }
        base.JsonModelWriteCore(writer, options);
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (_imageValue is not null && !Patch.Contains("$.image_url"u8))
        {
            writer.WritePropertyName("image_url"u8);
            writer.WriteStringValue(_imageValue.GetValue(cache: false));
        }
        if (Optional.IsDefined(FileId) && !Patch.Contains("$.file_id"u8))
        {
            writer.WritePropertyName("file_id"u8);
            writer.WriteStringValue(FileId);
        }

        Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}
