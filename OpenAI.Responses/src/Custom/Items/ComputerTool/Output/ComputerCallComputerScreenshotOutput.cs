using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ComputerToolCallOutputItemOutputComputerScreenshot")]
public partial class ComputerCallComputerScreenshotOutput
{
    // CUSTOM: Renamed to use the ImageUri naming convention.
    [CodeGenMember("ImageUrl")]
    public string ImageUri { get; set; }
}
