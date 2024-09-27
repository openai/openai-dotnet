using System;

namespace OpenAI.Files;

[CodeGenModel("OpenAIFile")]
public partial class OpenAIFile
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> The object type, which is always "file". </summary>
    private InternalOpenAIFileObject Object { get; } = InternalOpenAIFileObject.File;

    // CUSTOM: Renamed.
    /// <summary> The size of the file, in bytes. </summary>
    [CodeGenMember("Bytes")]
    public int? SizeInBytes { get; }

    // CUSTOM: Added the Obsolete attribute.
    [Obsolete($"This property is obsolete. If this is a fine-tuning training file, it may take some time to process"
        + $" after it has been uploaded. While the file is processing, you can still create a fine-tuning job but it"
        + $" will not start until the file processing has completed.")]
    public FileStatus Status { get; }

    // CUSTOM: Added the Obsolete attribute.
    [Obsolete($"This property is obsolete. For details on why a fine-tuning training file failed validation, see the"
        + $" `error` field on the fine-tuning job.")]
    public string StatusDetails { get; }
}
