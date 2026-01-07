namespace OpenAI.Images;

// CUSTOM: Renamed.
[CodeGenType("ImageGenInputUsageDetails")]
public partial class ImageInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("TextTokens")]
    public long TextTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("ImageTokens")]
    public long ImageTokenCount { get; }
}