using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Files;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIFilesModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Files.FileDeletionResult"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Files.FileDeletionResult"/> instance for mocking. </returns>
    public static FileDeletionResult FileDeletionResult(string fileId = null, bool deleted = default)
    {
        return new FileDeletionResult(
            fileId,
            InternalDeleteFileResponseObject.File,
            deleted,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Files.OpenAIFileInfo"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Files.OpenAIFileInfo"/> instance for mocking. </returns>
    public static OpenAIFile OpenAIFileInfo(string id = null, int? sizeInBytes = null, DateTimeOffset createdAt = default, string filename = null, FilePurpose purpose = default, FileStatus status = default, string statusDetails = null)
    {
        return new OpenAIFile(
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

    /// <summary> Initializes a new instance of <see cref="OpenAI.Files.OpenAIFileCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Files.OpenAIFileCollection"/> instance for mocking. </returns>
    public static OpenAIFileCollection OpenAIFileCollection(IEnumerable<OpenAIFile> items = null)
    {
        items ??= new List<OpenAIFile>();

        return new OpenAIFileCollection(
            items.ToList(),
            InternalListFilesResponseObject.List,
            serializedAdditionalRawData: null);
    }
}
