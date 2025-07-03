using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Files;

[CodeGenType("CreateFileRequestPurpose")]
public readonly partial struct FileUploadPurpose
{
    // CUSTOM: Added Experimental attribute.
    [Experimental("OPENAI001")]
    public static FileUploadPurpose UserData { get; } = new FileUploadPurpose(UserDataValue);

    // CUSTOM:
    // - Added Experimental attribute.
    // - Renamed.
    [Experimental("OPENAI001")]
    [CodeGenMember("Evals")]
    public static FileUploadPurpose Evaluations { get; } = new(EvalsValue);
}