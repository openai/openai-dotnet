using System;
using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Moderations;

[CodeGenType("ModerationImageURLInputImageUrl")]
[CodeGenSuppress("InternalModerationImagePartImageUrl", typeof(string))]
internal partial class InternalModerationImagePartImageUrl
{
    private Uri _imageUri;
    private BinaryData _imageBytes;
    private string _imageBytesMediaType;
    private string _internalUrl;

    // CUSTOM: Changed type from Uri to string to be able to support data URIs properly.
    /// <summary> Either a URL of the image or the base64 encoded image data. </summary>
    [CodeGenMember("Url")]
    internal string InternalUrl
    {
        get => _internalUrl;
        set
        {
            _internalUrl = value;
            if (value is not null && !DataEncodingHelpers.TryParseDataUri(value, out _imageBytes, out _imageBytesMediaType))
            {
                _imageUri = new Uri(value);
            }
        }
    }

    /// <summary> Initializes a new instance of <see cref="ImagePartImageUrl"/>. </summary>
    /// <param name="uri"> Either a URL of the image or the base64 encoded image data. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="uri"/> is null. </exception>
    public InternalModerationImagePartImageUrl(Uri uri)
    {
        Argument.AssertNotNull(uri, nameof(uri));
        _imageUri = uri;
        _internalUrl = uri.ToString();
    }

    public InternalModerationImagePartImageUrl(BinaryData imageBytes, string imageBytesMediaType)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNull(imageBytesMediaType, nameof(imageBytesMediaType));

        _imageBytes = imageBytes;
        _imageBytesMediaType = imageBytesMediaType;
        _internalUrl = DataEncodingHelpers.CreateDataUri(imageBytes, imageBytesMediaType);
    }

    public Uri ImageUri => _imageUri;

    public BinaryData ImageBytes => _imageBytes;

    public string ImageBytesMediaType => _imageBytesMediaType;
}
