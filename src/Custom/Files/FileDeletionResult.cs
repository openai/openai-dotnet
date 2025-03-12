namespace OpenAI.Files;

[CodeGenType("DeleteFileResponse")]
public partial class FileDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string FileId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `file`. </summary>
    [CodeGenMember("Object")]
    internal InternalDeleteFileResponseObject Object { get; } = InternalDeleteFileResponseObject.File;
}
