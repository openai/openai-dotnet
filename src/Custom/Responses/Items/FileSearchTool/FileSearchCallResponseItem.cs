namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FileSearchToolCallItemResource")]
public partial class FileSearchCallResponseItem
{
    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public FileSearchCallStatus? Status { get; set; }
}