using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Images;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIImagesModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Images.GeneratedImage"/>. </summary>
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
