namespace OpenAI.Files;

[CodeGenModel("DeleteFileResponse")]
public partial class FileDeletionResult
{
    [CodeGenMember("Id")]
    public string FileId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `file`. </summary>
    [CodeGenMember("Object")]
    internal InternalDeleteFileResponseObject Object { get; } = InternalDeleteFileResponseObject.File;
}
