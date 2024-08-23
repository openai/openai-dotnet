using System;
using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents the common base type for a piece of message content used for chat completions.
/// </summary>
[CodeGenModel("ChatMessageContentPart")]
[CodeGenSuppress("ChatMessageContentPart", typeof(IDictionary<string, BinaryData>))]
public partial class ChatMessageContentPart
{
    private readonly ChatMessageContentPartKind _kind;
    private readonly string _text;
    private readonly string _refusal;
    private readonly InternalChatCompletionRequestMessageContentPartImageImageUrl _imageUrl;
    private readonly string _dataUri;

    internal ChatMessageContentPart(string text)
    {
        Argument.AssertNotNull(text, nameof(text));

        _text = text;
        _kind = ChatMessageContentPartKind.Text;
    }

    // CUSTOM: Made internal.
    internal ChatMessageContentPart()
    {
    }


    internal ChatMessageContentPart(Uri imageUri, ImageChatMessageContentPartDetail? imageDetail = null)
    {
        Argument.AssertNotNull(imageUri, nameof(imageUri));

        _imageUrl = new(imageUri) { Detail = imageDetail };
        _kind = ChatMessageContentPartKind.Image;
    }

    internal ChatMessageContentPart(BinaryData imageBytes, string imageBytesMediaType, ImageChatMessageContentPartDetail? imageDetail = null)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNull(imageBytesMediaType, nameof(imageBytesMediaType));

        _imageUrl = new(imageBytes, imageBytesMediaType) { Detail = imageDetail };
        _kind = ChatMessageContentPartKind.Image;
    }

    /// <summary> Initializes a new instance of <see cref="ChatMessageContentPart"/>. </summary>
    /// <param name="kind"> The kind. </param>
    /// <param name="text"> The text. </param>
    /// <param name="imageUrl"> The image URI. </param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    internal ChatMessageContentPart(string kind, string text, string refusal, InternalChatCompletionRequestMessageContentPartImageImageUrl imageUrl, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        _kind = new ChatMessageContentPartKind(kind);
        _text = text;
        _refusal = refusal;
        _imageUrl = imageUrl;
        SerializedAdditionalRawData = serializedAdditionalRawData;
    }

    /// <summary>
    /// The content part kind.
    /// </summary>
    public ChatMessageContentPartKind Kind => _kind;

    /// <summary>
    /// The text content.
    /// </summary>
    public string Text => _text;

    /// <summary>
    /// The refusal message from the assistant.
    /// </summary>
    public string Refusal => _refusal;

    /// <summary>
    /// The image URI content.
    /// </summary>
    public Uri ImageUri => _imageUrl?.ImageUri;

    /// <summary>
    /// The image URI content.
    /// </summary>
    public BinaryData ImageBytes => _imageUrl?.ImageBytes;

    /// <summary>
    /// The image URI content.
    /// </summary>
    public string ImageBytesMediaType => _imageUrl?.ImageBytesMediaType;

    /// <summary>
    /// The image URI detail.
    /// </summary>
    public ImageChatMessageContentPartDetail? ImageDetail => _imageUrl?.Detail;

    /// <summary>
    /// Creates a new instance of <see cref="ChatMessageContentPart"/> that encapsulates text content.
    /// </summary>
    /// <param name="text"> The content for the new instance. </param>
    /// <returns> A new instance of <see cref="ChatMessageContentPart"/>. </returns>
    public static ChatMessageContentPart CreateTextMessageContentPart(string text)
    {
        Argument.AssertNotNull(text, nameof(text));

        return new(text);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ChatMessageContentPart"/> that encapsulates an assistant refusal message.
    /// </summary>
    /// <param name="refusal"> The refusal message from the assistant. </param>
    /// <returns> A new instance of <see cref="ChatMessageContentPart"/>. </returns>
    public static ChatMessageContentPart CreateRefusalMessageContentPart(string refusal)
    {
        Argument.AssertNotNull(refusal, nameof(refusal));

        return new ChatMessageContentPart(
            ChatMessageContentPartKind.Refusal.ToString(),
            text: null,
            refusal: refusal,
            imageUrl: null,
            serializedAdditionalRawData: null);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ChatMessageContentPart"/> that encapsulates image content obtained from
    /// an internet location that will be accessible to the model when evaluating a message with this content.
    /// </summary>
    /// <param name="imageUri"> An internet location pointing to an image. This must be accessible to the model. </param>
    /// <param name="imageDetail"> The detail level of the image. </param>
    /// <returns> A new instance of <see cref="ChatMessageContentPart"/>. </returns>
    public static ChatMessageContentPart CreateImageMessageContentPart(Uri imageUri, ImageChatMessageContentPartDetail? imageDetail = null)
    {
        Argument.AssertNotNull(imageUri, nameof(imageUri));

        return new(imageUri, imageDetail);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ChatMessageContentPart"/> that encapsulates image content obtained from
    /// an internet location that will be accessible to the model when evaluating a message with this content.
    /// </summary>
    /// <param name="imageBytes"> The readable stream containing the image data to use as content. </param>
    /// <param name="imageBytesMediaType">The MIME descriptor, like <c>image/png</c>, corresponding to the image data format of the provided data.</param>
    /// <param name="imageDetail"> The detail level of the image. </param>
    /// <returns> A new instance of <see cref="ChatMessageContentPart"/>. </returns>
    public static ChatMessageContentPart CreateImageMessageContentPart(BinaryData imageBytes, string imageBytesMediaType, ImageChatMessageContentPartDetail? imageDetail = null)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNull(imageBytesMediaType, nameof(imageBytesMediaType));

        return new(imageBytes, imageBytesMediaType, imageDetail);
    }

    /// <summary>
    /// Returns text representation of this part.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Text;

    /// <summary>
    /// Implicitly creates a new <see cref="ChatMessageContentPart"/> instance from an item of plain text.
    /// </summary>
    /// <remarks>
    /// Using a <see cref="string"/> in the position of a <see cref="ChatMessageContentPart"/> is equivalent to
    /// calling the <see cref="CreateTextMessageContentPart(string)"/> method.
    /// </remarks>
    /// <param name="content"> The text content to use as this content part. </param>
    public static implicit operator ChatMessageContentPart(string content) => new(content);
}
