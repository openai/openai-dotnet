using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ImageGenToolCallItemResource")]
public partial class ImageGenerationCallResponseItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("OutputFormat")]
    public ImageGenerationToolOutputFileFormat? OutputFileFormat { get; set; }
}
