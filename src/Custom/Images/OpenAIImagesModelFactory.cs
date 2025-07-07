using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
            revisedPrompt,
            imageBytes,
            imageUri,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Images.GeneratedImageCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Images.GeneratedImageCollection"/> instance for mocking. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static GeneratedImageCollection GeneratedImageCollection(DateTimeOffset createdAt, IEnumerable<GeneratedImage> items)
        => GeneratedImageCollection(createdAt, items, usage: default);

    /// <summary> Initializes a new instance of <see cref="OpenAI.Images.GeneratedImageCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Images.GeneratedImageCollection"/> instance for mocking. </returns>.
    [Experimental("OPENAI001")]
    public static GeneratedImageCollection GeneratedImageCollection(DateTimeOffset createdAt = default, IEnumerable<GeneratedImage> items = null, ImageTokenUsage usage = default)
    {
        items ??= new List<GeneratedImage>();

        return new GeneratedImageCollection(
            items.ToList(),
            usage,
            createdAt,
            additionalBinaryDataProperties: null);
    }
}
