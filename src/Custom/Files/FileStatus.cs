using System;

namespace OpenAI.Files;

// CUSTOM: Added the Obsolete attribute.
[Obsolete($"This struct is obsolete. If this is a fine-tuning training file, it may take some time to process"
    + $" after it has been uploaded. While the file is processing, you can still create a fine-tuning job but it"
    + $" will not start until the file processing has completed.")]
[CodeGenModel("OpenAIFileStatus")]
public enum FileStatus
{
    Uploaded,

    Processed,

    Error,
}