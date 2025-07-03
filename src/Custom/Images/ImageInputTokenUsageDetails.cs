using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Images;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("ImagesResponseUsageInputTokensDetails")]
public partial class ImageInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("TextTokens")]
    public int TextTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("ImageTokens")]
    public int ImageTokenCount { get; }
}