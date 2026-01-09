using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Moderations;

// CUSTOM: Renamed.
[CodeGenType("CreateModerationRequestInput")]
public partial class ModerationInputPart
{
    // CUSTOM: Converted to public enum from internal extensible type.
    [CodeGenMember("Type")]
    internal InternalModerationInputPartType InternalType { get; set; }
    public ModerationInputPartKind Kind
    {
        get => InternalType.ToString().ToModerationInputPartKind();
        private set => InternalType = Kind.ToSerialString();
    }

    // CUSTOM: Exposed input text properties.
    public string Text => (this as InternalModerationTextPart)?.InternalText;

    // CUSTOM: Exposed input image properties.
    public Uri ImageUri => (this as InternalModerationImagePart)?.ImageUrl?.ImageUri;
    public BinaryData ImageBytes => (this as InternalModerationImagePart)?.ImageUrl?.ImageBytes;
    public string ImageBytesMediaType => (this as InternalModerationImagePart)?.ImageUrl?.ImageBytesMediaType;

    /// <summary> Creates a new <see cref="ModerationInputPart"/> that encapsulates text. </summary>
    /// <param name="text"> The text. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="text"/> is null. </exception>
    public static ModerationInputPart CreateTextPart(string text)
    {
        Argument.AssertNotNull(text, nameof(text));

        return new InternalModerationTextPart(text);
    }

    /// <summary> Creates a new <see cref="ModerationInputPart"/> that encapsulates an image. </summary>
    /// <param name="imageUri"> The public internet URI where the image is located. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageUri"/> is null. </exception>
    public static ModerationInputPart CreateImagePart(Uri imageUri)
    {
        Argument.AssertNotNull(imageUri, nameof(imageUri));

        return new InternalModerationImagePart(new InternalModerationImagePartImageUrl(imageUri));
    }

    /// <summary> Creates a new <see cref="ModerationInputPart"/> that encapsulates an image. </summary>
    /// <param name="imageBytes"> The image bytes. </param>
    /// <param name="imageBytesMediaType"> The MIME type of the image, e.g., <c>image/png</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageBytes"/> or <paramref name="imageBytesMediaType"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageBytesMediaType"/> is an empty string, and was expected to be non-empty. </exception>
    public static ModerationInputPart CreateImagePart(BinaryData imageBytes, string imageBytesMediaType)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNullOrEmpty(imageBytesMediaType, nameof(imageBytesMediaType));

        return new InternalModerationImagePart(new InternalModerationImagePartImageUrl(imageBytes, imageBytesMediaType));
    }
}
