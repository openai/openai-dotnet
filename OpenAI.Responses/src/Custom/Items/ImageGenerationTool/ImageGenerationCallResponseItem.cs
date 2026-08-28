using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ImageGenToolCallItemResource")]
[CodeGenSuppress("ImageGenerationCallResponseItem")]
public partial class ImageGenerationCallResponseItem
{
    public ImageGenerationCallResponseItem() : this(ResponseItemKind.ImageGenerationCall, null, default, default, default, default, default, default, default, null, null)
    {
    }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputFormat")]
    public ImageGenerationToolOutputFileFormat? OutputFileFormat { get; set; }
}
