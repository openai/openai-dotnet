using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenType("RunStepObject")]
public partial class RunStep
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.run.step`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "thread.run.step";

    /// <summary>
    /// The <c>step_details</c> associated with this run step.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Please note <see cref="RunStepDetails"/> is the base class.
    /// </para>
    /// </remarks>
    [CodeGenMember("StepDetails")]
    public RunStepDetails Details { get; }

    internal static RunStep FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeRunStep(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
