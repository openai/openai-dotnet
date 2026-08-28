using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ComputerToolCallSafetyCheck")]
[CodeGenSuppress("ComputerCallSafetyCheck")]
public partial class ComputerCallSafetyCheck
{

}
