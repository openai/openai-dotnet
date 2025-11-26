namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ImageGenToolCallItemResource")]
public partial class ImageGenerationCallResponseItem
{
    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public ImageGenerationCallStatus? Status { get; set; }
}
