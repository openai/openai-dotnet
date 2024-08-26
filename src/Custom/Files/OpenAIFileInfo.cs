namespace OpenAI.Files;

[CodeGenModel("OpenAIFile")]
public partial class OpenAIFileInfo
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> The object type, which is always "file". </summary>
    private InternalOpenAIFileObject Object { get; } = InternalOpenAIFileObject.File;

    // CUSTOM: Renamed.
    /// <summary> The size of the file, in bytes. </summary>
    [CodeGenMember("Bytes")]
    public int? SizeInBytes { get; }
}
