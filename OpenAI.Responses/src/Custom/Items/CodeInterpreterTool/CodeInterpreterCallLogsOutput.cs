using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("CodeInterpreterToolLogsOutput")]
[CodeGenSuppress("CodeInterpreterCallLogsOutput")]
public partial class CodeInterpreterCallLogsOutput
{
    public CodeInterpreterCallLogsOutput() : this(InternalCodeInterpreterToolOutputType.Logs, default, null)
    {
    }
}