using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[CodeGenType("MessageContent")]
public partial class MessageContent
{
    /// <summary>
    /// Creates a new <see cref="MessageContent"/> instance that refers to an uploaded image with a known file ID.
    /// </summary>
    /// <param name="imageFileId"></param>
    /// <param name="detail"></param>
    /// <returns></returns>
    public static MessageContent FromImageFileId(
        string imageFileId,
        MessageImageDetail? detail = null)
            => new InternalMessageContentImageFileObject(
                imageFile: new(
                    fileId: imageFileId,
                    detail: detail?.ToSerialString(),
                    additionalBinaryDataProperties: null));

    /// <summary>
    /// Creates a new instance of <see cref="MessageContent"/> that refers to an image at a model-accessible
    /// internet location.
    /// </summary>
    /// <param name="imageUri"></param>
    /// <param name="detail"></param>
    /// <returns></returns>
    public static MessageContent FromImageUri(Uri imageUri, MessageImageDetail? detail = null)
        => new InternalMessageContentImageUrlObject(
            imageUrl: new(
                url: imageUri,
                detail: detail?.ToSerialString(),
                additionalBinaryDataProperties: null));

    /// <summary>
    /// Creates a new <see cref="MessageContent"/> instance that encapsulates a simple string input.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static MessageContent FromText(string text) => new InternalMessageContentTextObject(text);

    /// <inheritdoc cref="InternalMessageContentImageUrlObject.InternalUrl"/>
    public Uri ImageUri => AsInternalImageUrl?.ImageUrl?.Url;
    /// <inheritdoc cref="InternalMessageContentImageFileObject.InternalFileId"/>
    public string ImageFileId => AsInternalImageFile?.ImageFile?.FileId;
    /// <inheritdoc cref="InternalMessageContentImageFileObject.InternalDetail"/>
    public MessageImageDetail? ImageDetail => AsInternalImageFile?.ImageFile?.Detail?.ToMessageImageDetail() ?? AsInternalImageUrl?.ImageUrl?.Detail?.ToMessageImageDetail();
    /// <inheritdoc cref="InternalMessageContentTextObject.InternalText"/>
    public string Text => AsInternalText?.InternalTextLiteralValue ?? AsInternalText?.InternalTextObjectValue?.Value;
    /// <inheritdoc cref="InternalMessageContentTextObject.InternalAnnotations"/>
    public IReadOnlyList<TextAnnotation> TextAnnotations => AsInternalText?.WrappedAnnotations ?? [];
    public string Refusal => AsRefusal?.InternalRefusal;

    private InternalMessageContentImageFileObject AsInternalImageFile => this as InternalMessageContentImageFileObject;
    private InternalMessageContentImageUrlObject AsInternalImageUrl => this as InternalMessageContentImageUrlObject;
    private InternalMessageContentTextObject AsInternalText => this as InternalMessageContentTextObject;
    private InternalMessageContentRefusalObject AsRefusal => this as InternalMessageContentRefusalObject;

    /// <summary>
    /// The implicit conversion operator that infers an equivalent <see cref="MessageContent"/> 
    /// instance from a plain <see cref="string"/>.
    /// </summary>
    /// <param name="value"> The text for the message content. </param>
    public static implicit operator MessageContent(string value) => FromText(value);
}
