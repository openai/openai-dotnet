using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace OpenAI.Models;

/// <summary>
/// Represents a collection of entries for available models.
/// </summary>
[CodeGenType("ListModelsResponse")]
[CodeGenSuppress("Data")]
[CodeGenSuppress(nameof(OpenAIModelCollection))]
[CodeGenSuppress(nameof(OpenAIModelCollection), typeof(InternalListModelsResponseObject), typeof(IDictionary<string, BinaryData>))]
public partial class OpenAIModelCollection : ReadOnlyCollection<OpenAIModel>
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> Gets the object. </summary>
    private InternalListModelsResponseObject Object { get; } = InternalListModelsResponseObject.List;

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
    internal OpenAIModelCollection(InternalListModelsResponseObject @object, IReadOnlyList<OpenAIModel> data, IDictionary<string, BinaryData> serializedAdditionalRawData)
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
}
