using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenAI.Chat;

[CodeGenType("ChatCompletionRequestMessageContentPartImageImageUrl")]
[CodeGenSuppress("InternalChatCompletionRequestMessageContentPartImageImageUrl", typeof(string))]
internal partial class InternalChatCompletionRequestMessageContentPartImageImageUrl
{
    private Uri _imageUri;
    private BinaryData _imageBytes;
    private string _imageBytesMediaType;

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
    private string _internalUrl;

    /// <summary> Initializes a new instance of <see cref="InternalChatCompletionRequestMessageContentPartImageImageUrl"/>. </summary>
    /// <param name="uri"> Either a URL of the image or the base64 encoded image data. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="uri"/> is null. </exception>
    public InternalChatCompletionRequestMessageContentPartImageImageUrl(Uri uri, ChatImageDetailLevel? detailLevel = default)
        : this(detailLevel, null, null)
    {
        Argument.AssertNotNull(uri, nameof(uri));
        _imageUri = uri;
        _internalUrl = uri.ToString();
    }

    public InternalChatCompletionRequestMessageContentPartImageImageUrl(BinaryData imageBytes, string imageBytesMediaType, ChatImageDetailLevel? detailLevel = default)
        : this(detailLevel, null, null)
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
