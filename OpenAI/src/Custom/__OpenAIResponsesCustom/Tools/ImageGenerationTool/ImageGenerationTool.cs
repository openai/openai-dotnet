using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ImageGenTool")]
public partial class ImageGenerationTool
{
    // CUSTOM:Renamed.
    [CodeGenMember("OutputCompression")]
    public int? OutputCompressionFactor { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputFormat")]
    public ImageGenerationToolOutputFileFormat? OutputFileFormat { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Moderation")]
    public ImageGenerationToolModerationLevel? ModerationLevel { get; set; }

    // CUSTOM:Renamed.
    [CodeGenMember("PartialImages")]
    public int? PartialImageCount { get; set; }

    // CUSTOM: Convert to string to ensure backwards compatibility.
    [CodeGenMember("Model")]
    public string Model { get; set; }
}
