using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

/// <summary>
/// The update type presented when the status of a <see cref="ThreadRun"/> has changed to <c>requires_action</c>,
/// indicating that tool output submission or another intervention is needed for the run to continue.
/// </summary>
/// <remarks>
/// Distinct <see cref="RequiredActionUpdate"/> instances will generated for each required action, meaning that
/// parallel function calling will present multiple updates even if the tool calls arrive at the same time.
/// </remarks>
[Experimental("OPENAI001")]
public class RequiredActionUpdate : RunUpdate
{
    /// <inheritdoc cref="InternalRequiredFunctionToolCall.InternalName"/>
    public string FunctionName => AsFunctionCall?.FunctionName;

    /// <inheritdoc cref="InternalRequiredFunctionToolCall.InternalArguments"/>
    public string FunctionArguments => AsFunctionCall?.FunctionArguments;

    /// <inheritdoc cref="InternalRequiredFunctionToolCall.Id"/>
    public string ToolCallId => AsFunctionCall?.Id;

    private InternalRequiredFunctionToolCall AsFunctionCall => _requiredAction as InternalRequiredFunctionToolCall;

    private readonly RequiredAction _requiredAction;

    internal RequiredActionUpdate(ThreadRun run, RequiredAction action)
        : base(run, StreamingUpdateReason.RunRequiresAction)
    {
        _requiredAction = action;
    }

    /// <summary>
    /// Gets the full, deserialized <see cref="ThreadRun"/> instance associated with this streaming required action
    /// update.
    /// </summary>
    /// <returns></returns>
    public ThreadRun GetThreadRun() => Value;

    internal static IEnumerable<RequiredActionUpdate> DeserializeRequiredActionUpdates(JsonElement element)
    {
        ThreadRun run = ThreadRun.DeserializeThreadRun(element);
        List<RequiredActionUpdate> updates = [];
        foreach (RequiredAction action in run.RequiredActions ?? [])
        {
            updates.Add(new(run, action));
        }
        return updates;
    }
}