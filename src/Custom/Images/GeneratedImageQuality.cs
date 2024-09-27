namespace OpenAI.Images;

// CUSTOM: Renamed.
/// <summary> 
///     The quality of the image that will be generated. <see cref="High"/> creates images with finer details and
///     greater consistency across the image.
/// </summary>
[CodeGenModel("CreateImageRequestQuality")]
public readonly partial struct GeneratedImageQuality
{
    [CodeGenMember("Hd")]
    public static GeneratedImageQuality High { get; } = new GeneratedImageQuality(HighValue);
}