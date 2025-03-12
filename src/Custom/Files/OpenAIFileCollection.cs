using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenAI.Files;

[CodeGenType("ListFilesResponse")]
[CodeGenSuppress("Data")]
[CodeGenSuppress(nameof(OpenAIFileCollection))]
[CodeGenSuppress(nameof(OpenAIFileCollection), typeof(string), typeof(string), typeof(bool))]
[CodeGenSuppress(nameof(OpenAIFileCollection), typeof(InternalListFilesResponseObject), typeof(string), typeof(string), typeof(bool), typeof(IDictionary<string, BinaryData>))]
public partial class OpenAIFileCollection : ReadOnlyCollection<OpenAIFile>
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    [CodeGenMember("Object")]
    private InternalListFilesResponseObject Object { get; } = InternalListFilesResponseObject.List;

    // CUSTOM: Internalizing pending stanardized pagination representation for the list operation.
    [CodeGenMember("FirstId")]
    internal string FirstId { get; }

    [CodeGenMember("LastId")]
    internal string LastId { get; }

    [CodeGenMember("HasMore")]
    internal bool HasMore { get; }

    /// <summary> Initializes a new instance of <see cref="OpenAIFileCollection"/>. </summary>
    /// <param name="data"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="data"/> is null. </exception>
    internal OpenAIFileCollection(IEnumerable<OpenAIFile> data, string firstId, string lastId, bool hasMore)
        : base([.. data])
    {
        Argument.AssertNotNull(data, nameof(data));
        FirstId = firstId;
        LastId = lastId;
        HasMore = hasMore;
    }

    /// <summary> Initializes a new instance of <see cref="OpenAIFileCollection"/>. </summary>
    /// <param name="data"></param>
    /// <param name="object"></param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    internal OpenAIFileCollection(IReadOnlyList<OpenAIFile> data, string @object, string firstId, string lastId, bool hasMore, IDictionary<string, BinaryData> serializedAdditionalRawData)
        : base([.. data])
    {
        Object = @object;
        SerializedAdditionalRawData = serializedAdditionalRawData;
        FirstId = firstId;
        LastId = lastId;
        HasMore = hasMore;
    }

    /// <summary> Initializes a new instance of <see cref="OpenAIFileCollection"/> for deserialization. </summary>
    internal OpenAIFileCollection()
        : base([])
    {
    }
}