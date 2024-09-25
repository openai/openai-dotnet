using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace OpenAI.Models;

/// <summary>
/// Represents a collection of entries for available models.
/// </summary>
[CodeGenModel("ListModelsResponse")]
[CodeGenSuppress("Data")]
[CodeGenSuppress(nameof(OpenAIModelCollection))]
[CodeGenSuppress(nameof(OpenAIModelCollection), typeof(InternalListModelsResponseObject), typeof(IReadOnlyList<OpenAIModel>))]
public partial class OpenAIModelCollection : ReadOnlyCollection<OpenAIModel>
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> Gets the object. </summary>
    private InternalListModelsResponseObject Object { get; } = InternalListModelsResponseObject.List;

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
