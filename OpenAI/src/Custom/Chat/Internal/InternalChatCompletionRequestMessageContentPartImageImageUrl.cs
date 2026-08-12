using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenType("ChatCompletionRequestMessageContentPartImageImageUrl")]
[CodeGenSuppress("InternalChatCompletionRequestMessageContentPartImageImageUrl", typeof(string))]
internal partial class InternalChatCompletionRequestMessageContentPartImageImageUrl
{
    private Uri _imageUri;
    private DataUriValue _imageValue;

    // CUSTOM: Changed type from Uri to string to be able to support data URIs properly.
    /// <summary> Either a URL of the image or the base64 encoded image data. </summary>
    [CodeGenMember("Url")]
    internal string InternalUrl
    {
        get => _imageValue?.GetValue(cache: true);
        set
        {
            _imageValue = value is null ? null : new DataUriValue(value, parseDataUri: true);
            _imageUri = null;
            if (value is not null && !_imageValue.IsDataUri)
            {
                _imageUri = new Uri(value);
            }
        }
    }

    /// <summary> Initializes a new instance of <see cref="InternalChatCompletionRequestMessageContentPartImageImageUrl"/>. </summary>
    /// <param name="uri"> Either a URL of the image or the base64 encoded image data. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="uri"/> is null. </exception>
    public InternalChatCompletionRequestMessageContentPartImageImageUrl(Uri uri, ChatImageDetailLevel? detailLevel = default)
        : this(null, detailLevel, default)
    {
        Argument.AssertNotNull(uri, nameof(uri));
        _imageUri = uri;
        _imageValue = new DataUriValue(uri.AbsoluteUri, parseDataUri: false);
    }

    public InternalChatCompletionRequestMessageContentPartImageImageUrl(BinaryData imageBytes, string imageBytesMediaType, ChatImageDetailLevel? detailLevel = default)
        : this(null, detailLevel, default)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNull(imageBytesMediaType, nameof(imageBytesMediaType));

        _imageValue = new DataUriValue(imageBytes, imageBytesMediaType);
    }

    public Uri ImageUri => _imageUri;

    public BinaryData ImageBytes => _imageValue?.Bytes;

    public string ImageBytesMediaType => _imageValue?.MediaType;

    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<InternalChatCompletionRequestMessageContentPartImageImageUrl>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(InternalChatCompletionRequestMessageContentPartImageImageUrl)} does not support writing '{format}' format.");
        }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (!Patch.Contains("$.url"u8))
        {
            writer.WritePropertyName("url"u8);
            writer.WriteStringValue(_imageValue?.GetValue(cache: false));
        }
        if (Optional.IsDefined(Detail) && !Patch.Contains("$.detail"u8))
        {
            writer.WritePropertyName("detail"u8);
            writer.WriteStringValue(Detail.Value.ToString());
        }

        Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}
