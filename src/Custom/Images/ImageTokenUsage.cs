namespace OpenAI.Images;

// CUSTOM:
// - Renamed.
[CodeGenType("ImageGenUsage")]
public partial class ImageTokenUsage
{
    // CUSTOM: Renamed.
    [CodeGenMember("InputTokens")]
    public long InputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputTokens")]
    public long OutputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("TotalTokens")]
    public long TotalTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("InputTokensDetails")]
    public ImageInputTokenUsageDetails InputTokenDetails { get; }
}