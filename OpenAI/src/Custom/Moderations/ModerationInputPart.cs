using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Moderations;

// CUSTOM: Renamed.
[CodeGenType("CreateModerationRequestInput")]
public partial class ModerationInputPart
{
    // CUSTOM: Renamed to Kind and removed setter.
    [CodeGenMember("Type")]
    public ModerationInputPartKind Kind { get; }

    // CUSTOM: Exposed input text properties.
    public string Text => (this as InternalModerationTextPart)?.InternalText;

    // CUSTOM: Exposed input image properties.
    public Uri ImageUri => (this as InternalModerationImagePart)?.ImageUrl?.Url;

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
}
