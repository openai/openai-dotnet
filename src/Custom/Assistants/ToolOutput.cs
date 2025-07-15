using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[CodeGenType("SubmitToolOutputsRunRequestToolOutput")]
public partial class ToolOutput
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolOutput"/>.
    /// </summary>
    /// <param name="toolCallId">
    /// The ID of <see cref="InternalRequiredToolCall"/> that the provided output resolves.
    /// </param>
    /// <param name="output"> The output from the specified tool. </param>
    public ToolOutput(string toolCallId, string output)
        : this(toolCallId, output, null)
    { }
}
