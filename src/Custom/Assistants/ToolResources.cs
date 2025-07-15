using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenType("AssistantObjectToolResources1")]
[CodeGenVisibility(nameof(ToolResources), CodeGenVisibility.Public)]
public partial class ToolResources
{
    /// <summary> Gets the code interpreter. </summary>
    public CodeInterpreterToolResources CodeInterpreter { get; set; }
    /// <summary> Gets the file search. </summary>
    public FileSearchToolResources FileSearch { get; set; }
}
