using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ComputerUsePreviewTool")]
[CodeGenSuppress("ComputerTool")]
public partial class ComputerTool
{
    public ComputerTool() : this(ResponseToolKind.ComputerUsePreview, default, default, default, default)
    {
    }
}