using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ImageGenToolInputImageMask")]
[CodeGenVisibility(nameof(ImageGenerationToolInputImageMask), CodeGenVisibility.Internal)]
public partial class ImageGenerationToolInputImageMask
{
    private DataUriValue _imageValue;

    [CodeGenMember("ImageUri")]
    public string ImageUri
    {
        get => _imageValue?.GetValue(cache: true);
        set => _imageValue = value is null ? null : new DataUriValue(value, parseDataUri: false);
    }

    public ImageGenerationToolInputImageMask(Uri imageUri)
    {
        Argument.AssertNotNull(imageUri, nameof(imageUri));

        ImageUri = imageUri.AbsoluteUri;
    }

    /// <summary> Initializes a mask from binary image data. </summary>
    /// <param name="imageBytes">
    /// The image bytes.
    ///
    /// No copy of the memory is made. If a mutable buffer is used,
    /// it must remain unaltered until the operation is complete. The
    /// caller retains ownership of the memory backing this value.
    /// </param>
    public ImageGenerationToolInputImageMask(BinaryData imageBytes)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNullOrEmpty(imageBytes.MediaType, nameof(imageBytes.MediaType));

        _imageValue = new DataUriValue(imageBytes, imageBytes.MediaType);
    }

    public ImageGenerationToolInputImageMask(string fileId)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        FileId = fileId;
    }

    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<ImageGenerationToolInputImageMask>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(ImageGenerationToolInputImageMask)} does not support writing '{format}' format.");
        }
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
