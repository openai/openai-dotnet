using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

/// <summary>
///     A part of the chat message content.
///     <list>
///         <item>
///             Call <see cref="CreateTextPart(string)"/> to create a <see cref="ChatMessageContentPart"/> that
///             encapsulates text.
///         </item>
///         <item>
///             Call <see cref="CreateImagePart(Uri, ChatImageDetailLevel?)"/> or
///             <see cref="CreateImagePart(BinaryData, string, OpenAI.Chat.ChatImageDetailLevel?)"/> to create a
///             <see cref="ChatMessageContentPart"/> that encapsulates an image.
///         </item>
///         <item>
///             Call <see cref="CreateRefusalPart(string)"/> to create a <see cref="ChatMessageContentPart"/> that
///             encapsulates a refusal coming from the model.
///         </item>
///         <item>
///             Call <see cref="CreateInputAudioPart(BinaryData, ChatInputAudioFormat)"/> to create a content part
///             encapsulating input audio for user role messages.
///         </item>
///     </list>
/// </summary>
[CodeGenType("ChatCompletionRequestMessageContentPart")]
public partial class ChatMessageContentPart
{
    private readonly ChatMessageContentPartKind _kind;
    private readonly string _text;
    private readonly InternalChatCompletionRequestMessageContentPartImageImageUrl _imageUri;
    private readonly InternalChatCompletionRequestMessageContentPartAudioInputAudio _inputAudio;
    private readonly InternalChatCompletionRequestMessageContentPartFileFile _fileFile;
    private readonly string _refusal;

    // CUSTOM: Made internal.
    internal ChatMessageContentPart()
    {
    }

    // CUSTOM: Added to support deserialization.
    internal ChatMessageContentPart(
        ChatMessageContentPartKind kind,
        string text = default,
        InternalChatCompletionRequestMessageContentPartImageImageUrl imageUri = default,
        string refusal = default,
        InternalChatCompletionRequestMessageContentPartAudioInputAudio inputAudio = default,
        InternalChatCompletionRequestMessageContentPartFileFile fileFile = default,
        IDictionary<string, BinaryData> serializedAdditionalRawData = default)
    {
        _kind = kind;
        _text = text;
        _imageUri = imageUri;
        _refusal = refusal;
        _inputAudio = inputAudio;
        _fileFile = fileFile;
        _additionalBinaryDataProperties = serializedAdditionalRawData;
    }

    /// <summary> The kind of content part. </summary>
    public ChatMessageContentPartKind Kind => _kind;

    // CUSTOM: Spread.
    /// <summary> The text. </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.Text"/>. </remarks>
    public string Text => _text;

    // CUSTOM: Spread.
    /// <summary> The public internet URI where the image is located. </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.Image"/>. </remarks>
    public Uri ImageUri => _imageUri?.ImageUri;

    // CUSTOM: Spread.
    /// <summary> The image bytes. </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.Image"/>. </remarks>
    public BinaryData ImageBytes => _imageUri?.ImageBytes;

    // CUSTOM: Spread.
    /// <summary> The MIME type of the image, e.g., <c>image/png</c>. </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.Image"/>. </remarks>
    public string ImageBytesMediaType => _imageUri?.ImageBytesMediaType;

    // CUSTOM:
    // - Added Experimental attribute.
    // - Spread.
    /// <summary>
    /// The encoded binary audio payload associated with the content part.
    /// </summary>
    /// <remarks>
    /// Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.InputAudio"/>. The content part
    /// represents user role audio input.
    /// </remarks>
    [Experimental("OPENAI001")]
    public BinaryData InputAudioBytes => _inputAudio?.Data;

    // CUSTOM:
    // - Added Experimental attribute.
    // - Spread.
    /// <summary>
    /// The encoding format that the audio data provided in <see cref="InputAudioBytes"/> should be interpreted with.
    /// </summary>
    /// <remarks>
    /// Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.InputAudio"/>. The content part
    /// represents user role audio input.
    /// </remarks>
    [Experimental("OPENAI001")]
    public ChatInputAudioFormat? InputAudioFormat => _inputAudio?.Format;

    // CUSTOM:
    // - Added Experimental attribute.
    // - Spread.
    /// <summary> The ID of the previously uploaded file that the content part represents. </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.File"/> and the content part refers to a previously uploaded file. </remarks>
    [Experimental("OPENAI001")]
    public string FileId => _fileFile?.FileId;

    // CUSTOM:
    // - Added Experimental attribute.
    // - Spread.
    /// <summary> The binary file content of the file content part. </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.File"/> and the content refers to data for a new file. </remarks>
    [Experimental("OPENAI001")]
    public BinaryData FileBytes => _fileFile?.FileBytes;

    // CUSTOM:
    // - Added Experimental attribute.
    // - Spread.
    /// <summary> The MIME type of the file, e.g., <c>application/pdf</c>. </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.File"/> and the content refers to data for a new file. </remarks>
    [Experimental("OPENAI001")]
    public string FileBytesMediaType => _fileFile?.FileBytesMediaType;

    // CUSTOM:
    // - Added Experimental attribute.
    // - Spread.
    /// <summary> The filename for the new file content creation that the content part encapsulates. </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.File"/> and the content refers to data for a new file. </remarks>
    [Experimental("OPENAI001")]
    public string Filename => _fileFile?.Filename;

    // CUSTOM: Spread.
    /// <summary>
    ///     The level of detail with which the model should process the image and generate its textual understanding of
    ///     it. Learn more in the <see href="https://platform.openai.com/docs/guides/vision/low-or-high-fidelity-image-understanding">vision guide</see>.
    /// </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.Image"/>. </remarks>
    public ChatImageDetailLevel? ImageDetailLevel => _imageUri?.Detail;

    // CUSTOM: Spread.
    /// <summary> The refusal message generated by the model. </summary>
    /// <remarks> Present when <see cref="Kind"/> is <see cref="ChatMessageContentPartKind.Refusal"/>. </remarks>
    public string Refusal => _refusal;

    /// <summary> Creates a new <see cref="ChatMessageContentPart"/> that encapsulates text. </summary>
    /// <param name="text"> The text. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="text"/> is null. </exception>
    public static ChatMessageContentPart CreateTextPart(string text)
    {
        Argument.AssertNotNull(text, nameof(text));

        return new ChatMessageContentPart(ChatMessageContentPartKind.Text, text: text);
    }

    /// <summary> Creates a new <see cref="ChatMessageContentPart"/> that encapsulates an image. </summary>
    /// <param name="imageUri"> The public internet URI where the image is located. </param>
    /// <param name="imageDetailLevel">
    ///     The level of detail with which the model should process the image and generate its textual understanding of
    ///     it. Learn more in the <see href="https://platform.openai.com/docs/guides/vision/low-or-high-fidelity-image-understanding">vision guide</see>.
    /// </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageUri"/> is null. </exception>
    public static ChatMessageContentPart CreateImagePart(Uri imageUri, ChatImageDetailLevel? imageDetailLevel = null)
    {
        Argument.AssertNotNull(imageUri, nameof(imageUri));

        return new ChatMessageContentPart(
            kind: ChatMessageContentPartKind.Image,
            imageUri: new(imageUri, imageDetailLevel));
    }

    /// <summary> Creates a new <see cref="ChatMessageContentPart"/> that encapsulates an image. </summary>
    /// <param name="imageBytes"> The image bytes. </param>
    /// <param name="imageBytesMediaType"> The MIME type of the image, e.g., <c>image/png</c>. </param>
    /// <param name="imageDetailLevel">
    ///     The level of detail with which the model should process the image and generate its textual understanding of
    ///     it. Learn more in the <see href="https://platform.openai.com/docs/guides/vision/low-or-high-fidelity-image-understanding">vision guide</see>.
    /// </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageBytes"/> or <paramref name="imageBytesMediaType"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageBytesMediaType"/> is an empty string, and was expected to be non-empty. </exception>
    public static ChatMessageContentPart CreateImagePart(BinaryData imageBytes, string imageBytesMediaType, ChatImageDetailLevel? imageDetailLevel = null)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNullOrEmpty(imageBytesMediaType, nameof(imageBytesMediaType));

        return new ChatMessageContentPart(
            kind: ChatMessageContentPartKind.Image,
            imageUri: new(imageBytes, imageBytesMediaType, imageDetailLevel));
    }

    /// <summary> Creates a new <see cref="ChatMessageContentPart"/> that encapsulates a refusal coming from the model. </summary>
    /// <param name="refusal"> The refusal message generated by the model. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="refusal"/> is null. </exception>
    public static ChatMessageContentPart CreateRefusalPart(string refusal)
    {
        Argument.AssertNotNull(refusal, nameof(refusal));

        return new ChatMessageContentPart(
            kind: ChatMessageContentPartKind.Refusal,
            refusal: refusal);
    }

    /// <summary> Creates a new <see cref="ChatMessageContentPart"/> that encapsulates user role input audio in a known format. </summary>
    /// <remarks>
    /// Binary audio content parts may only be used with <see cref="UserChatMessage"/> instances to represent user audio input. When referring to
    /// past audio output from the model, use <see cref="ChatOutputAudioReference(string)"/> instead.
    /// </remarks>
    /// <param name="inputAudioBytes"> The audio data. </param>
    /// <param name="inputAudioFormat"> The format of the audio data. </param>
    [Experimental("OPENAI001")]
    public static ChatMessageContentPart CreateInputAudioPart(BinaryData inputAudioBytes, ChatInputAudioFormat inputAudioFormat)
    {
        Argument.AssertNotNull(inputAudioBytes, nameof(inputAudioBytes));

        return new ChatMessageContentPart(
            kind: ChatMessageContentPartKind.InputAudio,
            inputAudio: new(inputAudioBytes, inputAudioFormat));
    }

    /// <summary> Creates a new <see cref="ChatMessageContentPart"/> that represents a previously uploaded file. </summary>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is null or empty. </exception>
    [Experimental("OPENAI001")]
    public static ChatMessageContentPart CreateFilePart(string fileId)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        return new ChatMessageContentPart(
            kind: ChatMessageContentPartKind.File,
            fileFile: new()
            {
                FileId = fileId,
            });
    }

    /// <summary> Creates a new <see cref="ChatMessageContentPart"/> that encapsulates new file data to upload. </summary>
    /// <param name="fileBytes"> The binary content of the file. </param>
    /// <param name="fileBytesMediaType"> The MIME type of the file, e.g., <c>application/pdf</c>. </param>
    /// <param name="filename"> The filename to use for the file that will be created. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileBytes"/> or <paramref name="fileBytesMediaType"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileBytesMediaType"/> or <paramref name="filename"/>> is an empty string, and was expected to be non-empty. </exception>
    [Experimental("OPENAI001")]
    public static ChatMessageContentPart CreateFilePart(BinaryData fileBytes, string fileBytesMediaType, string filename)
    {
        Argument.AssertNotNull(fileBytes, nameof(fileBytes));
        Argument.AssertNotNullOrEmpty(fileBytesMediaType, nameof(fileBytesMediaType));
        Argument.AssertNotNullOrEmpty(filename, nameof(filename));

        return new ChatMessageContentPart(
            kind: ChatMessageContentPartKind.File,
            fileFile: new(fileBytes, fileBytesMediaType, filename));
    }

    /// <summary>
    ///     Implicitly instantiates a new <see cref="ChatMessageContentPart"/> from a <see cref="string"/>. As such,
    ///     using a <see cref="string"/> in place of a <see cref="ChatMessageContentPart"/> is equivalent to calling the
    ///     <see cref="CreateTextPart(string)"/> method.
    /// </summary>
    /// <param name="text"> The text encapsulated by this <see cref="ChatMessageContentPart"/>. </param>
    public static implicit operator ChatMessageContentPart(string text) => CreateTextPart(text);
}
