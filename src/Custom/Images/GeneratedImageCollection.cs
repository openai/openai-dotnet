using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenAI.Images;

/// <summary>
/// Represents an image generation response payload that contains information for multiple generated images.
/// </summary>
[CodeGenType("ImagesResponse")]
[CodeGenSuppress("Data")]
[CodeGenSuppress("Created")]
[CodeGenSuppress(nameof(GeneratedImageCollection))]
[CodeGenSuppress(nameof(GeneratedImageCollection), typeof(DateTimeOffset))]
[CodeGenSuppress(nameof(GeneratedImageCollection), typeof(DateTimeOffset), typeof(IDictionary<string, BinaryData>))]
public partial class GeneratedImageCollection : ReadOnlyCollection<GeneratedImage>
{
    // CUSTOM: Set the inherited Items property via the base constructor in favor of the suppressed Data property.
    /// <summary> Initializes a new instance of <see cref="GeneratedImageCollection"/>. </summary>
    /// <param name="created"></param>
    /// <param name="data"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="data"/> is null. </exception>
    internal GeneratedImageCollection(DateTimeOffset created, IEnumerable<GeneratedImage> data)
        : base([.. data])
    {
        Argument.AssertNotNull(data, nameof(data));

        CreatedAt = created;
    }

    // CUSTOM: Set the inherited Items property via the base constructor in favor of the suppressed Data property.
    /// <summary> Initializes a new instance of <see cref="GeneratedImageCollection"/>. </summary>
    /// <param name="created"></param>
    /// <param name="data"></param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    internal GeneratedImageCollection(DateTimeOffset created, IReadOnlyList<GeneratedImage> data, IDictionary<string, BinaryData> serializedAdditionalRawData)
        : base([.. data])
    {
        CreatedAt = created;
        SerializedAdditionalRawData = serializedAdditionalRawData;
    }

    // CUSTOM: Set the inherited Items property via the base constructor in favor of the suppressed Data property.
    /// <summary> Initializes a new instance of <see cref="GeneratedImageCollection"/> for deserialization. </summary>
    internal GeneratedImageCollection()
        : base([])
    {
    }

    // CUSTOM: Renamed.
    /// <summary>
    /// The timestamp at which the result image was generated.
    /// </summary>
    [CodeGenMember("Created")]
    public DateTimeOffset CreatedAt { get; }
}