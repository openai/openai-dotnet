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
            revisedPrompt: revisedPrompt,
            imageBytes: imageBytes,
            imageUri: imageUri,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Images.GeneratedImageCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Images.GeneratedImageCollection"/> instance for mocking. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static GeneratedImageCollection GeneratedImageCollection(DateTimeOffset createdAt, IEnumerable<GeneratedImage> items)
        => GeneratedImageCollection(
            createdAt: createdAt,
            items: items,
            background: default,
            outputFileFormat: default,
            size: default,
            quality: default,
            usage: null);

    /// <summary> Initializes a new instance of <see cref="OpenAI.Images.GeneratedImageCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Images.GeneratedImageCollection"/> instance for mocking. </returns>.
    [Experimental("OPENAI001")]
    public static GeneratedImageCollection GeneratedImageCollection(DateTimeOffset createdAt = default, IEnumerable<GeneratedImage> items = null, GeneratedImageBackground? background = default, GeneratedImageFileFormat? outputFileFormat = default, GeneratedImageSize? size = default, GeneratedImageQuality? quality = default, ImageTokenUsage usage = null)
    {
        items ??= new List<GeneratedImage>();

        return new GeneratedImageCollection(
            createdAt: createdAt,
            items: items.ToList(),
            background: background,
            outputFileFormat: outputFileFormat,
            size: size,
            quality: quality,
            usage: usage,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Images.ImageInputTokenUsageDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Images.ImageInputTokenUsageDetails"/> instance for mocking. </returns>.
    [Experimental("OPENAI001")]
    public static ImageInputTokenUsageDetails ImageInputTokenUsageDetails(long textTokenCount = default, long imageTokenCount = default)
    {
        return new ImageInputTokenUsageDetails(
            textTokenCount: textTokenCount,
            imageTokenCount: imageTokenCount,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Images.ImageTokenUsage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Images.ImageTokenUsage"/> instance for mocking. </returns>.
    [Experimental("OPENAI001")]
    public static ImageTokenUsage ImageTokenUsage(long inputTokenCount = default, long outputTokenCount = default, long totalTokenCount = default, ImageInputTokenUsageDetails inputTokenDetails = default, ImageOutputTokenUsageDetails outputTokenDetails = default)
    {
        return new ImageTokenUsage(
            inputTokenCount: inputTokenCount,
            outputTokenCount: outputTokenCount,
            totalTokenCount: totalTokenCount,
            inputTokenDetails: inputTokenDetails,
            outputTokenDetails: outputTokenDetails,
            additionalBinaryDataProperties: null);
    }
}
