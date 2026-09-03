using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenAI.Files;

[CodeGenType("ListFilesResponse")]
[CodeGenSuppress(nameof(OpenAIFileCollection))]
[CodeGenSuppress(nameof(OpenAIFileCollection), typeof(string), typeof(IEnumerable<OpenAIFile>), typeof(string), typeof(string), typeof(bool))]
[CodeGenSuppress(nameof(OpenAIFileCollection), typeof(string), typeof(IList<OpenAIFile>), typeof(string), typeof(string), typeof(bool), typeof(IDictionary<string, BinaryData>))]
[CodeGenVisibility(nameof(FirstId), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(LastId), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(HasMore), CodeGenVisibility.Internal)]
[CodeGenVisibility(nameof(Data), CodeGenVisibility.Internal)]
public partial class OpenAIFileCollection : ReadOnlyCollection<OpenAIFile>
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    [CodeGenMember("Object")]
    private string Object { get; } = "list";

    /// <summary> Initializes a new instance of <see cref="OpenAIFileCollection"/>. </summary>
    /// <param name="data"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="data"/> is null. </exception>
    internal OpenAIFileCollection(IEnumerable<OpenAIFile> data, string firstId, string lastId, bool hasMore)
        : base([.. data])
    {
        Argument.AssertNotNull(data, nameof(data));
        Data = [.. data];
        FirstId = firstId;
        LastId = lastId;
        HasMore = hasMore;
    }

    /// <summary> Initializes a new instance of <see cref="OpenAIFileCollection"/>. </summary>
    /// <param name="data"></param>
    /// <param name="object"></param>
    /// <param name="additionalBinaryDataProperties"> Keeps track of any properties unknown to the library. </param>
    internal OpenAIFileCollection(string @object, IReadOnlyList<OpenAIFile> data, string firstId, string lastId, bool hasMore, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        : base([.. data])
    {
        Object = @object;
        Data = [.. data];
        SerializedAdditionalRawData = additionalBinaryDataProperties;
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
