using System;

namespace OpenAI.Images;

/// <summary>
/// Represents the result data for an image generation request.
/// </summary>
[CodeGenModel("Image")]
public partial class GeneratedImage
{
    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// The binary image data received from the response, provided when
    /// <see cref="ImageGenerationOptions.ResponseFormat"/> is set to <see cref="GeneratedImageFormat.Bytes"/>.
    /// </summary>
    /// <remarks>
    /// This property is mutually exclusive with <see cref="ImageUri"/> and will be <c>null</c> when the other
    /// is present.
    /// </remarks>
    [CodeGenMember("B64Json")]
    public BinaryData ImageBytes { get; }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// A temporary internet location for an image, provided by default or when
    /// <see cref="ImageGenerationOptions.ResponseFormat"/> is set to <see cref="GeneratedImageFormat.Uri"/>.
    /// </summary>
    /// <remarks>
    /// This property is mutually exclusive with <see cref="ImageBytes"/> and will be <c>null</c> when the other
    /// is present.
    /// </remarks>
    [CodeGenMember("Url")]
    public Uri ImageUri { get; }
}
