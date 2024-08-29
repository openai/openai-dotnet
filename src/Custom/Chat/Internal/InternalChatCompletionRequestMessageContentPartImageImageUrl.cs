using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenAI.Chat;

[CodeGenModel("ChatCompletionRequestMessageContentPartImageImageUrl")]
[CodeGenSuppress("InternalChatCompletionRequestMessageContentPartImageImageUrl", typeof(string))]
internal partial class InternalChatCompletionRequestMessageContentPartImageImageUrl
{
#if NET8_0_OR_GREATER
    [GeneratedRegex(@"^data:(?<type>.+?);base64,(?<data>.+)$")]
    private static partial Regex ParseDataUriRegex();
#else
    private static Regex ParseDataUriRegex() => s_parseDataUriRegex;
    private static readonly Regex s_parseDataUriRegex = new(@"^data:(?<type>.+?);base64,(?<data>.+)$", RegexOptions.Compiled);
#endif

    private readonly Uri _imageUri;
    private readonly BinaryData _imageBytes;
    private readonly string _imageBytesMediaType;

    // CUSTOM: Changed type from Uri to string to be able to support data URIs properly.
    /// <summary> Either a URL of the image or the base64 encoded image data. </summary>
    [CodeGenMember("Url")]
    internal string Url { get; }

    /// <summary> Initializes a new instance of <see cref="InternalChatCompletionRequestMessageContentPartImageImageUrl"/>. </summary>
    /// <param name="uri"> Either a URL of the image or the base64 encoded image data. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="uri"/> is null. </exception>
    public InternalChatCompletionRequestMessageContentPartImageImageUrl(Uri uri)
    {
        Argument.AssertNotNull(uri, nameof(uri));

        _imageUri = uri;

        Url = uri.ToString();
    }

    public InternalChatCompletionRequestMessageContentPartImageImageUrl(BinaryData imageBytes, string imageBytesMediaType)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNull(imageBytesMediaType, nameof(imageBytesMediaType));

        _imageBytes = imageBytes;
        _imageBytesMediaType = imageBytesMediaType;

        string base64EncodedData = Convert.ToBase64String(_imageBytes.ToArray());
        Url = $"data:{_imageBytesMediaType};base64,{base64EncodedData}";
    }

    /// <summary> Initializes a new instance of <see cref="InternalChatCompletionRequestMessageContentPartImageImageUrl"/>. </summary>
    /// <param name="url"> Either a URL of the image or the base64 encoded image data. </param>
    /// <param name="detail"> Specifies the detail level of the image. Learn more in the [Vision guide](/docs/guides/vision/low-or-high-fidelity-image-understanding). </param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    internal InternalChatCompletionRequestMessageContentPartImageImageUrl(string url, ImageChatMessageContentPartDetail? detail, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        Match parsedDataUri = ParseDataUriRegex().Match(url);

        if (parsedDataUri.Success)
        {
            _imageBytes = BinaryData.FromBytes(Convert.FromBase64String(parsedDataUri.Groups["data"].Value));
            _imageBytesMediaType = parsedDataUri.Groups["type"].Value;
        }
        else
        {
            _imageUri = new Uri(url);
        }

        Url = url;
        Detail = detail;
        SerializedAdditionalRawData = serializedAdditionalRawData;
    }

    public Uri ImageUri => _imageUri;

    public BinaryData ImageBytes => _imageBytes;

    public string ImageBytesMediaType => _imageBytesMediaType;
}
