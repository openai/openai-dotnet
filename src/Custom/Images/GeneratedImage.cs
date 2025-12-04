using System;

namespace OpenAI.Images;

/// <summary> Represents the result data for an image generation request. </summary>
[CodeGenType("Image")]
public partial class GeneratedImage
{
    // CUSTOM: Renamed.
    /// <summary>
    ///     The binary image data received from the response, provided when
    ///     <see cref="ImageGenerationOptions.ResponseFormat"/> is set to <see cref="GeneratedImageFormat.Bytes"/>.
    /// </summary>
    [CodeGenMember("B64Json")]
    public BinaryData ImageBytes { get; }

    // CUSTOM: Renamed.
    /// <summary>
    ///     A temporary internet location for an image, provided by default or when
    ///     <see cref="ImageGenerationOptions.ResponseFormat"/> is set to <see cref="GeneratedImageFormat.Uri"/>.
    /// </summary>
    [CodeGenMember("Url")]
    public Uri ImageUri { get; }
}
