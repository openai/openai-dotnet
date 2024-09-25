using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenAI.Moderations;

[CodeGenModel("CreateModerationResponse")]
[CodeGenSuppress("Results")]
[CodeGenSuppress(nameof(ModerationResultCollection))]
[CodeGenSuppress(nameof(ModerationResultCollection), typeof(string), typeof(string), typeof(IReadOnlyList<ModerationResult>))]
public partial class ModerationResultCollection : ReadOnlyCollection<ModerationResult>
{
    // CUSTOM: Recovered this field. See https://github.com/Azure/autorest.csharp/issues/4636.
    /// <summary>
    /// Keeps track of any properties unknown to the library.
    /// <para>
    /// To assign an object to the value of this property use <see cref="BinaryData.FromObjectAsJson{T}(T, System.Text.Json.JsonSerializerOptions?)"/>.
    /// </para>
    /// <para>
    /// To assign an already formatted json string to this property use <see cref="BinaryData.FromString(string)"/>.
    /// </para>
    /// <para>
    /// Examples:
    /// <list type="bullet">
    /// <item>
    /// <term>BinaryData.FromObjectAsJson("foo")</term>
    /// <description>Creates a payload of "foo".</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromString("\"foo\"")</term>
    /// <description>Creates a payload of "foo".</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromObjectAsJson(new { key = "value" })</term>
    /// <description>Creates a payload of { "key": "value" }.</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromString("{\"key\": \"value\"}")</term>
    /// <description>Creates a payload of { "key": "value" }.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    private IDictionary<string, BinaryData> SerializedAdditionalRawData;

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
}
