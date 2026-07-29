using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("CustomToolFormat")]
[CodeGenVisibility(nameof(CustomToolFormat), CodeGenVisibility.ProtectedInternal, typeof(CustomToolFormatKind))]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class CustomToolFormat
{
}
