using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;

namespace OpenAI.Models;

/// <summary>
/// Represents a collection of entries for available models.
/// </summary>
[CodeGenType("ListModelsResponse")]
[CodeGenSuppress("Data")]
[CodeGenSuppress(nameof(OpenAIModelCollection))]
[CodeGenSuppress(nameof(OpenAIModelCollection), typeof(string), typeof(IDictionary<string, BinaryData>))]
public partial class OpenAIModelCollection : ReadOnlyCollection<OpenAIModel>
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> Gets the object. </summary>
    private string Object { get; } = "list";

    /// <summary> Initializes a new instance of <see cref="OpenAIModelCollection"/>. </summary>
    /// <param name="data"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="data"/> is null. </exception>
    internal OpenAIModelCollection(IEnumerable<OpenAIModel> data)
        : base([.. data])
    {
        Argument.AssertNotNull(data, nameof(data));
    }

    /// <summary> Initializes a new instance of <see cref="OpenAIModelCollection"/>. </summary>
    /// <param name="object"></param>
    /// <param name="data"></param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    internal OpenAIModelCollection(string @object, IReadOnlyList<OpenAIModel> data, IDictionary<string, BinaryData> serializedAdditionalRawData)
        : base([.. data])
    {
        Object = @object;
        SerializedAdditionalRawData = serializedAdditionalRawData;
    }

    /// <summary> Initializes a new instance of <see cref="OpenAIModelCollection"/> for deserialization. </summary>
    internal OpenAIModelCollection()
        : base([])
    {
    }

    internal static OpenAIModelCollection FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeOpenAIModelCollection(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
