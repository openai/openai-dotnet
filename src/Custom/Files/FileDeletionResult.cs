using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Files;

[CodeGenType("DeleteFileResponse")]
public partial class FileDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string FileId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `file`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "file";

    internal static FileDeletionResult FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeFileDeletionResult(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
