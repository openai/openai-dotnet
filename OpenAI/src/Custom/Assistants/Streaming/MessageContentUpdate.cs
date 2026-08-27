using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

/// <summary>
/// Represents a streaming update to <see cref="ThreadMessage"/> content as part of the Assistants API.
/// </summary>
/// <remarks>
/// Distinct <see cref="MessageContentUpdate"/> instances will be generated for each <see cref="MessageContent"/> part
/// and each content subcomponent, such as <see cref="TextAnnotationUpdate"/> instances, even if this information
/// arrived in the same response chunk.
/// </remarks>
[Experimental("OPENAI001")]
public partial class MessageContentUpdate : StreamingUpdate
{
    /// <inheritdoc cref="MessageDeltaObject.Id"/>
    public string MessageId => _delta.Id;

    /// <inheritdoc cref="InternalMessageDeltaContentImageFileObject.Index"/>
    public int MessageIndex => _textContent?.Index
        ?? _imageFileContent?.Index
        ?? _imageUrlContent?.Index
        ?? _refusalContent?.Index
        ?? TextAnnotation?.ContentIndex
        ?? 0;

    /// <inheritdoc cref="InternalMessageDeltaObjectDelta.Role"/>
    public MessageRole? Role => _delta.Delta?.Role;

    /// <inheritdoc cref="InternalMessageDeltaContentImageFileObjectImageFile.FileId"/>
    public string ImageFileId => _imageFileContent?.ImageFile?.FileId;

    /// <inheritdoc cref="MessageImageDetail"/>
    public MessageImageDetail? ImageDetail => _imageFileContent?.ImageFile?.Detail?.ToMessageImageDetail()
        ?? _imageUrlContent?.ImageUrl?.Detail?.ToMessageImageDetail();

    /// <inheritdoc cref="InternalMessageDeltaContentTextObjectText.Value"/>
    public string Text => _textContent?.Text?.Value;

    /// <summary>
    /// An update to an annotation associated with a specific content item in the message's content items collection.
    /// </summary>
    public TextAnnotationUpdate TextAnnotation { get; }

    public string RefusalUpdate => _refusalContent?.Refusal;

    private readonly InternalMessageDeltaContentImageFileObject _imageFileContent;
    private readonly InternalMessageDeltaContentTextObject _textContent;
    private readonly InternalMessageDeltaContentImageUrlObject _imageUrlContent;
    private readonly InternalMessageDeltaContentRefusalObject _refusalContent;
    private readonly InternalMessageDeltaObject _delta;

    internal MessageContentUpdate(InternalMessageDeltaObject delta, InternalMessageDeltaContent content)
        : base(StreamingUpdateReason.MessageUpdated)
    {
        _delta = delta;
        _textContent = content as InternalMessageDeltaContentTextObject;
        _imageFileContent = content as InternalMessageDeltaContentImageFileObject;
        _imageUrlContent = content as InternalMessageDeltaContentImageUrlObject;
        _refusalContent = content as InternalMessageDeltaContentRefusalObject;
    }

    internal MessageContentUpdate(InternalMessageDeltaObject delta, TextAnnotationUpdate annotation)
        : base(StreamingUpdateReason.MessageUpdated)
    {
        _delta = delta;
        TextAnnotation = annotation;
    }

    internal static IEnumerable<MessageContentUpdate> DeserializeMessageContentUpdates(
        JsonElement element,
        StreamingUpdateReason _,
        ModelReaderWriterOptions options = null)
    {
        InternalMessageDeltaObject deltaObject = InternalMessageDeltaObject.DeserializeInternalMessageDeltaObject(element, options);
        List<MessageContentUpdate> updates = [];
        foreach (InternalMessageDeltaContent deltaContent in deltaObject.Delta.Content ?? [])
        {
            updates.Add(new(deltaObject, deltaContent));
            if (deltaContent is InternalMessageDeltaContentTextObject textContent)
            {
                foreach (InternalMessageDeltaTextContentAnnotation internalAnnotation in textContent.Text.Annotations)
                {
                    TextAnnotationUpdate annotation = new(internalAnnotation);
                    updates.Add(new(deltaObject, annotation));
                }
            }
        }
        return updates;
    }
}

