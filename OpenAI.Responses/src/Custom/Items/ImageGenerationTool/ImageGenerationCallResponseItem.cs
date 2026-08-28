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

    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public ImageGenerationCallStatus? Status { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("OutputFormat")]
    public ImageGenerationToolOutputFileFormat? OutputFileFormat { get; set; }
}
