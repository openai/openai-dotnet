using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Images;

/// <summary> A collection of generated images. </summary>
[CodeGenType("ImagesResponse")]
[CodeGenSuppress(nameof(GeneratedImageCollection), typeof(DateTimeOffset))]
public partial class GeneratedImageCollection : ReadOnlyCollection<GeneratedImage>
{
    // CUSTOM: Set the inherited Items property via the base constructor.
    internal GeneratedImageCollection(DateTimeOffset createdAt, IList<GeneratedImage> items, GeneratedImageBackground? background, GeneratedImageFileFormat? outputFileFormat, GeneratedImageSize? size, GeneratedImageQuality? quality, ImageTokenUsage usage, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        : base(items ?? new ChangeTrackingList<GeneratedImage>())
    {
        CreatedAt = createdAt;
        Background = background;
        OutputFileFormat = outputFileFormat;
        Size = size;
        Quality = quality;
        Usage = usage;
        _additionalBinaryDataProperties = additionalBinaryDataProperties;
    }

    // CUSTOM: Call the base constructor.
    internal GeneratedImageCollection() : this(default, null, default, default, default, default, null, null)
    {
    }

    // CUSTOM: Renamed.
    /// <summary> The timestamp at which the result images were generated. </summary>
    [CodeGenMember("Created")]
    public DateTimeOffset CreatedAt { get; }

    // CUSTOM: Changed property type.
    /// <summary> Allows to set transparency for the background of the generated image(s). </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("Background")]
    public GeneratedImageBackground? Background { get; }

    // CUSTOM:
    // - Renamed.
    // - Changed property type.
    /// <summary> The format in which the generated images are returned. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("OutputFormat")]
    public GeneratedImageFileFormat? OutputFileFormat { get; set; }

    // CUSTOM: Changed property type.
    /// <summary> The quality of the image that will be generated. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("Quality")]
    public GeneratedImageQuality? Quality { get; }

    // CUSTOM: Changed property type.
    /// <summary> The size of the generated images. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("Size")]
    public GeneratedImageSize? Size { get; }
}