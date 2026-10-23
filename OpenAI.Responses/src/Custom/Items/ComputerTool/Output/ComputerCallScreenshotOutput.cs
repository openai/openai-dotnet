using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ComputerToolCallOutputItemOutputComputerScreenshot")]
public partial class ComputerCallScreenshotOutput
{
    [CodeGenMember("ImageUrl")]
    public string ImageUri { get; set; }
}
