using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("ThreadObject")]
public partial class AssistantThread
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread`. </summary>
    [CodeGenMember("Object")]
    internal InternalThreadObjectObject Object { get; } = InternalThreadObjectObject.Thread;


    /// <summary>
    /// The set of resources that are made available to the assistant's tools on this thread.
    /// The resources are specific to the type of tool.
    /// For example, the `code_interpreter` tool requires a list of file IDs, while the `file_search` tool requires a list of vector store IDs.
    /// </summary>
    public ToolResources ToolResources { get; }
}
