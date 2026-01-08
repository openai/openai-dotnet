using System;
using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Moderations;

// CUSTOM: Renamed.
[CodeGenType("ModerationImageURLInput")]
public partial class ModerationImagePart : ModerationInputPart
{
    internal ModerationImagePart(InternalModerationImagePartImageUrl imageUrl) : base(ModerationInputPartKind.Image)
    {
        Argument.AssertNotNull(imageUrl, nameof(imageUrl));

        ImageUrl = imageUrl;
    }

    internal InternalModerationImagePartImageUrl ImageUrl { get; }

    /// <summary> The public internet URI where the image is located. </summary>
    /// <remarks> Present when <see cref="ModerationInputPart.Kind"/> is <see cref="ModerationInputPartKind.Image"/>. </remarks>
    public Uri ImageUri => ImageUrl?.ImageUri;

    // CUSTOM: Spread.
    /// <summary> The image bytes. </summary>
    /// <remarks> Present when <see cref="ModerationInputPart.Kind"/> is <see cref="ModerationInputPartKind.Image"/>. </remarks>
    public BinaryData ImageBytes => ImageUrl?.ImageBytes;

    // CUSTOM: Spread.
    /// <summary> The MIME type of the image, e.g., <c>image/png</c>. </summary>
    /// <remarks> Present when <see cref="ModerationInputPart.Kind"/> is <see cref="ModerationInputPartKind.Image"/>. </remarks>
    public string ImageBytesMediaType => ImageUrl?.ImageBytesMediaType;
}
