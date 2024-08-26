using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Files;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIFilesModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Files.OpenAIFileInfo"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Files.OpenAIFileInfo"/> instance for mocking. </returns>
    public static OpenAIFileInfo OpenAIFileInfo(string id = null, int? sizeInBytes = null, DateTimeOffset createdAt = default, string filename = null, OpenAIFilePurpose purpose = default, OpenAIFileStatus status = default, string statusDetails = null)
    {
        return new OpenAIFileInfo(
            id,
            sizeInBytes,
            createdAt,
            filename,
            @object: InternalOpenAIFileObject.File,
            purpose,
            status,
            statusDetails,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Files.OpenAIFileInfoCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Files.OpenAIFileInfoCollection"/> instance for mocking. </returns>
    public static OpenAIFileInfoCollection OpenAIFileInfoCollection(IEnumerable<OpenAIFileInfo> items = null)
    {
        items ??= new List<OpenAIFileInfo>();

        return new OpenAIFileInfoCollection(
            items.ToList(),
            InternalListFilesResponseObject.List,
            serializedAdditionalRawData: null);
    }
}
