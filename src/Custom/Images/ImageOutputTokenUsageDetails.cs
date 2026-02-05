using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Images;

// CUSTOM: Renamed.
[CodeGenType("ImageGenOutputTokensDetails")]
public partial class ImageOutputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("TextTokens")]
    public long TextTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("ImageTokens")]
    public long ImageTokenCount { get; }
}
