using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenAI.Images;

/// <summary> Represents an image generation response with one or more images. </summary>
[CodeGenType("ImagesResponse")]
[CodeGenSuppress(nameof(GeneratedImageCollection), typeof(DateTimeOffset))]
[CodeGenVisibility(nameof(Data), CodeGenVisibility.Internal)]
public partial class GeneratedImageCollection : ReadOnlyCollection<GeneratedImage>
{
    // CUSTOM: Set the inherited Items property via the base constructor in favor of the intercepted Data property.
    internal GeneratedImageCollection(DateTimeOffset createdAt, IList<GeneratedImage> data, InternalImagesResponseBackground? background, InternalImagesResponseOutputFormat? outputFormat, InternalImagesResponseSize? size, InternalImagesResponseQuality? quality, ImageTokenUsage usage, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        : base([.. data])
    {
        CreatedAt = createdAt;
        Data = data ?? new ChangeTrackingList<GeneratedImage>();
        Background = background;
        OutputFormat = outputFormat;
        Size = size;
        Quality = quality;
        Usage = usage;
        _additionalBinaryDataProperties = additionalBinaryDataProperties;
    }

    // CUSTOM: Set the inherited Items property via the base constructor in favor of the intercepted Data property.
    internal GeneratedImageCollection()
        : base([])
    {
    }

    // CUSTOM: Renamed.
    /// <summary> The timestamp at which the result images were generated. </summary>
    [CodeGenMember("Created")]
    public DateTimeOffset CreatedAt { get; }
}