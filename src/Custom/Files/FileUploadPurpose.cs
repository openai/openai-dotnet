namespace OpenAI.Files;

[CodeGenType("CreateFileRequestPurpose")]
public readonly partial struct FileUploadPurpose
{
    // CUSTOM: Renamed.
    [CodeGenMember("Evals")]
    public static FileUploadPurpose Evaluations { get; } = new("evals");
}