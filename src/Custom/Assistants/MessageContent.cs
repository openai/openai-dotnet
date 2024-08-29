using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("MessageContent")]
public abstract partial class MessageContent
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
            => new InternalMessageImageFileContent(imageFileId, detail);

    /// <summary>
    /// Creates a new instance of <see cref="MessageContent"/> that refers to an image at a model-accessible
    /// internet location.
    /// </summary>
    /// <param name="imageUri"></param>
    /// <param name="detail"></param>
    /// <returns></returns>
    public static MessageContent FromImageUrl(Uri imageUri, MessageImageDetail? detail = null)
        => new InternalMessageImageUrlContent(imageUri, detail);

    /// <summary>
    /// Creates a new <see cref="MessageContent"/> instance that encapsulates a simple string input.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static MessageContent FromText(string text)
        => new InternalRequestMessageTextContent(text);

    /// <inheritdoc cref="InternalMessageImageUrlContent.InternalUrl"/>
    public Uri ImageUrl => AsInternalImageUrl?.InternalUrl;
    /// <inheritdoc cref="InternalMessageImageFileContent.InternalFileId"/>
    public string ImageFileId => AsInternalImageFile?.InternalFileId;
    /// <inheritdoc cref="InternalMessageImageFileContent.InternalDetail"/>
    public MessageImageDetail? ImageDetail => AsInternalImageFile?.InternalDetail ?? AsInternalImageUrl?.InternalDetail;
    /// <inheritdoc cref="InternalResponseMessageTextContent.InternalText"/>
    public string Text => AsInternalRequestText?.InternalText ?? AsInternalResponseText?.InternalText;
    /// <inheritdoc cref="InternalResponseMessageTextContent.InternalAnnotations"/>
    public IReadOnlyList<TextAnnotation> TextAnnotations => AsInternalResponseText?.InternalAnnotations ?? [];
    public string Refusal => AsRefusal?.InternalRefusal;

    private InternalMessageImageFileContent AsInternalImageFile => this as InternalMessageImageFileContent;
    private InternalMessageImageUrlContent AsInternalImageUrl => this as InternalMessageImageUrlContent;
    private InternalResponseMessageTextContent AsInternalResponseText => this as InternalResponseMessageTextContent;
    private InternalRequestMessageTextContent AsInternalRequestText => this as InternalRequestMessageTextContent;
    private InternalMessageRefusalContent AsRefusal => this as InternalMessageRefusalContent;

    /// <summary>
    /// The implicit conversion operator that infers an equivalent <see cref="MessageContent"/> 
    /// instance from a plain <see cref="string"/>.
    /// </summary>
    /// <param name="value"> The text for the message content. </param>
    public static implicit operator MessageContent(string value) => FromText(value);

    /// Creates a new instance of <see cref="MessageContent"/> for mocking.
    protected MessageContent()
    { }
}
