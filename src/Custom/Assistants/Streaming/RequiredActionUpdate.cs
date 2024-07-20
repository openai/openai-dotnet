using System.Collections.Generic;
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
public class RequiredActionUpdate : RunUpdate
{
    public IReadOnlyList<RequiredAction> RequiredActions { get; }

    internal RequiredActionUpdate(ThreadRun run, IReadOnlyList<RequiredAction> actions)
        : base(run, StreamingUpdateReason.RunRequiresAction)
    {
        RequiredActions = actions;
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
        return [new(run, run.RequiredActions)];
    }
}