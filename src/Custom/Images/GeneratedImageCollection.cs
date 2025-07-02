using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace OpenAI.Images;

/// <summary>
/// Represents an image generation response payload that contains information for multiple generated images.
/// </summary>
[CodeGenType("ImagesResponse")]
[CodeGenSuppress(nameof(GeneratedImageCollection), typeof(DateTimeOffset))]
[CodeGenVisibility(nameof(Data), CodeGenVisibility.Internal)]
public partial class GeneratedImageCollection : ReadOnlyCollection<GeneratedImage>
{
    // CUSTOM: Set the inherited Items property via the base constructor in favor of the intercepted Data property.
    /// <summary> Initializes a new instance of <see cref="GeneratedImageCollection"/>. </summary>
    /// <param name="data"></param>
    /// <param name="usage"></param>
    /// <param name="createdAt"></param>
    /// <param name="additionalBinaryDataProperties"> Keeps track of any properties unknown to the library. </param>
    internal GeneratedImageCollection(IList<GeneratedImage> data, ImageTokenUsage usage, DateTimeOffset createdAt, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        : base([.. data])
    {
        Usage = usage;
        CreatedAt = createdAt;
        SerializedAdditionalRawData = additionalBinaryDataProperties;
    }

    // CUSTOM: Set the inherited Items property via the base constructor in favor of the intercepted Data property.
    /// <summary> Initializes a new instance of <see cref="GeneratedImageCollection"/> for deserialization. </summary>
    internal GeneratedImageCollection() : base([])
    {
    }

    // CUSTOM: Renamed.
    /// <summary>
    /// The timestamp at which the result images were generated.
    /// </summary>
    [CodeGenMember("Created")]
    public DateTimeOffset CreatedAt { get; }

    internal static GeneratedImageCollection FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeGeneratedImageCollection(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}