using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace OpenAI.Moderations;

[CodeGenType("CreateModerationResponse")]
[CodeGenSuppress("Results")]
[CodeGenSuppress(nameof(ModerationResultCollection))]
[CodeGenSuppress(nameof(ModerationResultCollection), typeof(string), typeof(string))]
[CodeGenSuppress(nameof(ModerationResultCollection), typeof(string), typeof(string), typeof(IDictionary<string, BinaryData>))]
public partial class ModerationResultCollection : ReadOnlyCollection<ModerationResult>
{
    /// <summary> Initializes a new instance of <see cref="ModerationResultCollection"/>. </summary>
    /// <param name="id"> The unique identifier for the moderation request. </param>
    /// <param name="model"> The model used to generate the moderation results. </param>
    /// <param name="results"> A list of moderation objects. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="id"/>, <paramref name="model"/> or <paramref name="results"/> is null. </exception>
    internal ModerationResultCollection(string id, string model, IEnumerable<ModerationResult> results)
        : base([.. results])
    {
        Argument.AssertNotNull(id, nameof(id));
        Argument.AssertNotNull(model, nameof(model));
        Argument.AssertNotNull(results, nameof(results));

        Id = id;
        Model = model;
    }

    /// <summary> Initializes a new instance of <see cref="ModerationResultCollection"/>. </summary>
    /// <param name="id"> The unique identifier for the moderation request. </param>
    /// <param name="model"> The model used to generate the moderation results. </param>
    /// <param name="results"> A list of moderation objects. </param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    internal ModerationResultCollection(string id, string model, IReadOnlyList<ModerationResult> results, IDictionary<string, BinaryData> serializedAdditionalRawData)
        : base([.. results])
    {
        Id = id;
        Model = model;
        SerializedAdditionalRawData = serializedAdditionalRawData;
    }

    /// <summary> Initializes a new instance of <see cref="ModerationResultCollection"/> for deserialization. </summary>
    internal ModerationResultCollection()
        : base([])
    {
    }

    internal static ModerationResultCollection FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeModerationResultCollection(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
