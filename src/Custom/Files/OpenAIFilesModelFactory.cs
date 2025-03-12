using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            deleted,
            fileId,
            InternalDeleteFileResponseObject.File,
            additionalBinaryDataProperties: null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static OpenAIFile OpenAIFileInfo(string id, int? sizeInBytes, DateTimeOffset createdAt, string filename, FilePurpose purpose, FileStatus status, string statusDetails) =>
        OpenAIFileInfo(
            id: id,
            sizeInBytes: sizeInBytes,
            createdAt: createdAt,
            filename: filename,
            purpose: purpose,
            status: status,
            statusDetails: statusDetails,
            expiresAt: default);

    public static OpenAIFile OpenAIFileInfo(string id = null, int? sizeInBytes = null, DateTimeOffset createdAt = default, string filename = null, FilePurpose purpose = default, FileStatus status = default, string statusDetails = null, DateTimeOffset? expiresAt = null)
    {
        return new OpenAIFile(
            id: id,
            createdAt: createdAt,
            expiresAt: expiresAt,
            filename: filename,
            purpose: purpose,
            @object: InternalOpenAIFileObject.File,
            sizeInBytes: sizeInBytes,
            status: status,
            statusDetails: statusDetails,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Files.OpenAIFileCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Files.OpenAIFileCollection"/> instance for mocking. </returns>
    public static OpenAIFileCollection OpenAIFileCollection(IEnumerable<OpenAIFile> items = null)
    {
        return new OpenAIFileCollection(
            items?.ToList() ?? [],
            firstId: null,
            lastId : null,
            hasMore: false);
    }
}
