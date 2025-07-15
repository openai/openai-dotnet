using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenType("AssistantObject")]
public partial class Assistant
{
    private const string AssistantValue = "assistant";

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `assistant`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = AssistantValue;

    /// <inheritdoc cref="AssistantResponseFormat"/>
    public AssistantResponseFormat ResponseFormat { get; }

    /// <summary>
    /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
    ///
    /// We generally recommend altering this or temperature but not both.
    /// </summary>
    [CodeGenMember("TopP")]
    public float? NucleusSamplingFactor { get; }

    internal static Assistant FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeAssistant(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
