namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ImageGenToolCallItemResource")]
public partial class ImageGenerationCallResponseItem
{
    // CUSTOM: Made nullable since this is a read-only property.
    [CodeGenMember("Status")]
    public ImageGenerationCallStatus? Status { get; }
}
