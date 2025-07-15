using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenType("ThreadObject")]
public partial class AssistantThread
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "thread";


    /// <summary>
    /// The set of resources that are made available to the assistant's tools on this thread.
    /// The resources are specific to the type of tool.
    /// For example, the `code_interpreter` tool requires a list of file IDs, while the `file_search` tool requires a list of vector store IDs.
    /// </summary>
    public ToolResources ToolResources { get; }

    internal static AssistantThread FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeAssistantThread(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
