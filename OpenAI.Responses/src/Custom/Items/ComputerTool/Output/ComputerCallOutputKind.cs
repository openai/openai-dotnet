using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ComputerToolCallOutputItemOutputType")]
public readonly partial struct ComputerCallOutputKind
{
    // CUSTOM: Renamed to match the full discriminator value.
    [CodeGenMember("Screenshot")]
    public static ComputerCallOutputKind ComputerScreenshot { get; } = new("computer_screenshot");
}
