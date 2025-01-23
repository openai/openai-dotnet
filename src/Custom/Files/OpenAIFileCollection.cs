using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenAI.Files;

[CodeGenModel("ListFilesResponse")]
[CodeGenSuppress("Data")]
[CodeGenSuppress(nameof(OpenAIFileCollection))]
[CodeGenSuppress(nameof(OpenAIFileCollection),typeof(InternalListFilesResponseObject), typeof(IDictionary<string, BinaryData>))]
public partial class OpenAIFileCollection : ReadOnlyCollection<OpenAIFile>
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> Gets the object. </summary>
    private InternalListFilesResponseObject Object { get; } = InternalListFilesResponseObject.List;
    
    /// <summary> Initializes a new instance of <see cref="OpenAIFileCollection"/>. </summary>
    /// <param name="data"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="data"/> is null. </exception>
    internal OpenAIFileCollection(IEnumerable<OpenAIFile> data)
        : base([.. data])
    {
        Argument.AssertNotNull(data, nameof(data));
    }

    /// <summary> Initializes a new instance of <see cref="OpenAIFileCollection"/>. </summary>
    /// <param name="data"></param>
    /// <param name="object"></param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    internal OpenAIFileCollection(IReadOnlyList<OpenAIFile> data, InternalListFilesResponseObject @object, IDictionary<string, BinaryData> serializedAdditionalRawData)
        : base([.. data])
    {
        Object = @object;
        SerializedAdditionalRawData = serializedAdditionalRawData;
    }

    /// <summary> Initializes a new instance of <see cref="OpenAIFileCollection"/> for deserialization. </summary>
    internal OpenAIFileCollection()
        : base([])
    {
    }
}