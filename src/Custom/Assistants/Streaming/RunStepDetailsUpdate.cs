using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

/// <summary>
/// The update type presented when run step details, including tool call progress, have changed.
/// </summary>
[Experimental("OPENAI001")]
public class RunStepDetailsUpdate : StreamingUpdate
{
    internal readonly InternalRunStepDelta _delta;
    internal readonly InternalRunStepDeltaStepDetailsToolCallsObjectToolCallsObject _toolCall;
    private readonly InternalRunStepDeltaStepDetailsMessageCreationObject _asMessageCreation;
    private readonly InternalRunStepDeltaStepDetailsToolCallsCodeObject _asCodeCall;
    private readonly InternalRunStepDeltaStepDetailsToolCallsFileSearchObject _asFileSearchCall;
    private readonly InternalRunStepDeltaStepDetailsToolCallsFunctionObject _asFunctionCall;

    /// <inheritdoc cref="InternalRunStepDelta.Id"/>
    public string StepId => _delta?.Id;

    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsMessageCreationObjectMessageCreation"/>
    public string CreatedMessageId => _asMessageCreation?.MessageCreation?.MessageId;

    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsToolCallsCodeObject.Id"/>
    public string ToolCallId
        => _asCodeCall?.Id
        ?? _asFileSearchCall?.Id
        ?? _asFunctionCall?.Id
        ?? (_toolCall?.SerializedAdditionalRawData?.TryGetValue("id", out BinaryData idData) == true
            ? idData.ToString()
            : null);

    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsToolCallsCodeObject.Index"/>
    public int? ToolCallIndex => _asCodeCall?.Index ?? _asFileSearchCall?.Index ?? _asFunctionCall?.Index;

    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreter.Input"/>
    public string CodeInterpreterInput => _asCodeCall?.CodeInterpreter?.Input;

    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreter.Outputs"/>
    public IReadOnlyList<RunStepUpdateCodeInterpreterOutput> CodeInterpreterOutputs
        => _asCodeCall?.CodeInterpreter?.Outputs;

    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsToolCallsFunctionObjectFunction.Name"/>
    public string FunctionName => _asFunctionCall.Function?.Name;

    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsToolCallsFunctionObjectFunction.Arguments"/>
    public string FunctionArguments => _asFunctionCall?.Function?.Arguments;

    /// <inheritdoc cref="InternalRunStepDeltaStepDetailsToolCallsFunctionObjectFunction.Output"/>
    public string FunctionOutput => _asFunctionCall?.Function?.Output;

    internal RunStepDetailsUpdate(
        InternalRunStepDelta stepDelta,
        InternalRunStepDeltaStepDetailsToolCallsObjectToolCallsObject toolCall = null)
            : base(StreamingUpdateReason.RunStepUpdated)
    {
        _asMessageCreation = stepDelta?.Delta?.StepDetails as InternalRunStepDeltaStepDetailsMessageCreationObject;
        _asCodeCall = toolCall as InternalRunStepDeltaStepDetailsToolCallsCodeObject;
        _asFileSearchCall = toolCall as InternalRunStepDeltaStepDetailsToolCallsFileSearchObject;
        _asFunctionCall = toolCall as InternalRunStepDeltaStepDetailsToolCallsFunctionObject;
        _delta = stepDelta;
        _toolCall = toolCall;
    }

    internal static IEnumerable<RunStepDetailsUpdate> DeserializeRunStepDetailsUpdates(
        JsonElement element,
        StreamingUpdateReason updateKind,
        ModelReaderWriterOptions options = null)
    {
        InternalRunStepDelta stepDelta = InternalRunStepDelta.DeserializeInternalRunStepDelta(element, options);
        List<RunStepDetailsUpdate> updates = [];
        if (stepDelta?.Delta?.StepDetails is InternalRunStepDeltaStepDetailsMessageCreationObject)
        {
            updates.Add(new RunStepDetailsUpdate(stepDelta));
        }
        else if (stepDelta?.Delta?.StepDetails is InternalRunStepDeltaStepDetailsToolCallsObject toolCalls)
        {
            foreach (InternalRunStepDeltaStepDetailsToolCallsObjectToolCallsObject toolCall in toolCalls.ToolCalls)
            {
                updates.Add(new RunStepDetailsUpdate(stepDelta, toolCall));
            }
        }
        return updates;
    }
}
