using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Images;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIImageModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Images.GeneratedImage"/>. </summary>
    /// <param name="imageBytes">
    /// The binary image data received from the response, provided when
    /// <see cref="ImageGenerationOptions.ResponseFormat"/> is set to <see cref="GeneratedImageFormat.Bytes"/>.
    /// </param>
    /// <param name="imageUri">
    /// A temporary internet location for an image, provided by default or when
    /// <see cref="ImageGenerationOptions.ResponseFormat"/> is set to <see cref="GeneratedImageFormat.Uri"/>.
    /// </param>
    /// <returns> A new <see cref="OpenAI.Images.GeneratedImage"/> instance for mocking. </returns>
    public static GeneratedImage GeneratedImage(BinaryData imageBytes = null, Uri imageUri = null, string revisedPrompt = null)
    {
        return new GeneratedImage(
            imageBytes,
            imageUri,
            revisedPrompt,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Images.GeneratedImageCollection"/>. </summary>
    /// <param name="createdAt"> The timestamp at which the result image was generated. </param>
    /// <returns> A new <see cref="OpenAI.Images.GeneratedImageCollection"/> instance for mocking. </returns>
    public static GeneratedImageCollection GeneratedImageCollection(DateTimeOffset createdAt = default, IEnumerable<GeneratedImage> items = null)
    {
        items ??= new List<GeneratedImage>();

        return new GeneratedImageCollection(
            createdAt,
            items.ToList(),
            serializedAdditionalRawData: null);
    }
}
