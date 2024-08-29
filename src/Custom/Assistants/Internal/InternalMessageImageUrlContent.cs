using System;

namespace OpenAI.Assistants;

/// <summary>
/// Represents an item of image URL content within an Assistants API message.
/// </summary>
/// <remarks>
/// Use the <see cref="MessageContent.FromImageUrl(Uri,MessageImageDetail?)"/> method to
/// create an instance of this type.
/// </remarks>
[CodeGenModel("MessageContentImageUrlObject")]
[CodeGenSuppress("MessageImageUrlContent", typeof(InternalMessageContentImageUrlObjectImageUrl))]
internal partial class InternalMessageImageUrlContent
{
    [CodeGenMember("Type")]
    private string _type = "image_url";

    [CodeGenMember("ImageUrl")]
    internal InternalMessageContentImageUrlObjectImageUrl _imageUrl;

    /// <inheritdoc cref="InternalMessageContentImageUrlObjectImageUrl.Url"/>
    public Uri InternalUrl => _imageUrl.Url;

    /// <inheritdoc cref="InternalMessageContentImageUrlObjectImageUrl.Detail"/>
    public MessageImageDetail? InternalDetail => _imageUrl.Detail?.ToMessageImageDetail();

    /// <summary> Initializes a new instance of <see cref="InternalMessageImageUrlContent"/>. </summary>
    internal InternalMessageImageUrlContent(Uri url, MessageImageDetail? detail = null)
        : this(new InternalMessageContentImageUrlObjectImageUrl(url, detail?.ToSerialString(), null))
    { }

    /// <summary> Initializes a new instance of <see cref="InternalMessageImageUrlContent"/>. </summary>
    /// <param name="imageUrl"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageUrl"/> is null. </exception>
    internal InternalMessageImageUrlContent(InternalMessageContentImageUrlObjectImageUrl imageUrl)
    {
        Argument.AssertNotNull(imageUrl, nameof(imageUrl));
        _imageUrl = imageUrl;
    }
}
