using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Images;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ImagesResponseUsage")]
public partial class ImageTokenUsage
{
    // CUSTOM: Renamed.
    [CodeGenMember("InputTokens")]
    public int InputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputTokens")]
    public int OutputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("TotalTokens")]
    public int TotalTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("InputTokensDetails")]
    public ImageInputTokenUsageDetails InputTokenDetails { get; }
}