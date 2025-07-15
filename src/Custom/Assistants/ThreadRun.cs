using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Assistants;

// CUSTOM:
//  - Required actions are abstracted into a forward-compatible, strongly-typed conceptual
//    hierarchy and formatted into a more intuitive collection for the consumer.

[CodeGenType("RunObject")]
public partial class ThreadRun
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.run`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "thread.run";

    [CodeGenMember("RequiredAction")]
    internal readonly InternalRunRequiredAction _internalRequiredAction;

    /// <summary>
    /// The list of required actions that must have their results submitted for the run to continue.
    /// </summary>
    /// <remarks>
    /// <see cref="Assistants.RequiredAction"/> is the abstract base type for all required actions. Its
    /// concrete type can be one of:
    /// <list type="bullet">
    /// <item> <see cref="InternalRequiredFunctionToolCall"/> </item>
    /// </list>
    /// </remarks>
    public IReadOnlyList<RequiredAction> RequiredActions => _internalRequiredAction?.SubmitToolOutputs?.ToolCalls ?? [];

    /// <inheritdoc cref="AssistantResponseFormat"/>
    [CodeGenMember("ResponseFormat")]
    public AssistantResponseFormat ResponseFormat { get; }

    [CodeGenMember("ToolChoice")]
    public ToolConstraint ToolConstraint { get; }

    /// <summary>
    /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
    ///
    /// We generally recommend altering this or temperature but not both.
    /// </summary>
    [CodeGenMember("TopP")]
    public float? NucleusSamplingFactor { get; }

    /// <summary>
    /// Whether parallel function calling is enabled during tool use for the thread.
    /// </summary>
    /// <remarks>
    /// Assumed <c>true</c> if not otherwise specified.
    /// </remarks>
    [CodeGenMember("ParallelToolCalls")]
    public bool? AllowParallelToolCalls { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("MaxPromptTokens")]
    public int? MaxInputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("MaxCompletionTokens")]
    public int? MaxOutputTokenCount { get; }

    internal static ThreadRun FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeThreadRun(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
