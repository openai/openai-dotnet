using System;

namespace OpenAI.Moderations;

/// <summary>
///     A part of the moderation input.
///     <list>
///         <item>
///             Call <see cref="CreateTextPart(string)"/> to create a <see cref="ModerationInputPart"/> for text.
///         </item>
///         <item>
///             Call <see cref="CreateImagePart(Uri)"/> or
///             <see cref="CreateImagePart(BinaryData, string)"/> to create a
///             <see cref="ModerationInputPart"/> for an image.
///         </item>
///     </list>
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("CreateModerationRequestInput")]
public abstract partial class ModerationInputPart
{
    /// <summary> Creates a new <see cref="ModerationInputPart"/> that encapsulates text. </summary>
    /// <param name="text"> The text. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="text"/> is null. </exception>
    public static ModerationInputPart CreateTextPart(string text)
    {
        Argument.AssertNotNull(text, nameof(text));

        return new ModerationTextPart(text: text);
    }

    /// <summary> Creates a new <see cref="ModerationInputPart"/> that encapsulates an image. </summary>
    /// <param name="imageUri"> The public internet URI where the image is located. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageUri"/> is null. </exception>
    public static ModerationInputPart CreateImagePart(Uri imageUri)
    {
        Argument.AssertNotNull(imageUri, nameof(imageUri));

        return new ModerationImagePart(new InternalModerationImagePartImageUrl(imageUri));
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

        return new ModerationImagePart(new InternalModerationImagePartImageUrl(imageBytes, imageBytesMediaType));
    }
}
