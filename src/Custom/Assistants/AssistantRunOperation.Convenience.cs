using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

public partial class AssistantRunOperation
{
    /// <summary>
    /// Gets a refreshed instance of an existing <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to get a refreshed instance of. </param>
    /// <returns> A new <see cref="ThreadRun"/> instance with updated information. </returns>
    public virtual Task<ClientResult<ThreadRun>> GetRunAsync(ThreadRun run)
        => GetRunAsync(run?.ThreadId, run?.Id);

    /// <summary>
    /// Gets a refreshed instance of an existing <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to get a refreshed instance of. </param>
    /// <returns> A new <see cref="ThreadRun"/> instance with updated information. </returns>
    public virtual ClientResult<ThreadRun> GetRun(ThreadRun run)
        => GetRun(run?.ThreadId, run?.Id);


    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to cancel. </param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual Task<ClientResult<ThreadRun>> CancelRunAsync(ThreadRun run)
        => CancelRunAsync(run?.ThreadId, run?.Id);

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to cancel. </param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual ClientResult<ThreadRun> CancelRun(ThreadRun run)
        => CancelRun(run?.ThreadId, run?.Id);


    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="run"> The run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual Task<ClientResult<ThreadRun>> SubmitToolOutputsToRunAsync(
        ThreadRun run,
        IEnumerable<ToolOutput> toolOutputs)
            => SubmitToolOutputsToRunAsync(run?.ThreadId, run?.Id, toolOutputs);

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="run"> The run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual ClientResult<ThreadRun> SubmitToolOutputsToRun(
        ThreadRun run,
        IEnumerable<ToolOutput> toolOutputs)
            => SubmitToolOutputsToRun(run?.ThreadId, run?.Id, toolOutputs);

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    /// </summary>
    /// <param name="run"> The run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    public virtual AsyncResultCollection<StreamingUpdate> SubmitToolOutputsToRunStreamingAsync(
        ThreadRun run,
        IEnumerable<ToolOutput> toolOutputs)
            => SubmitToolOutputsToRunStreamingAsync(run?.ThreadId, run?.Id, toolOutputs);

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    /// </summary>
    /// <param name="run"> The run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    public virtual ResultCollection<StreamingUpdate> SubmitToolOutputsToRunStreaming(
        ThreadRun run,
        IEnumerable<ToolOutput> toolOutputs)
            => SubmitToolOutputsToRunStreaming(run?.ThreadId, run?.Id, toolOutputs);

    /// <summary>
    /// Gets a collection of <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to list run steps from. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <returns> A collection of run steps that can be enumerated using <c>await foreach</c>. </returns>
    public virtual PageableCollection<RunStep> GetRunSteps(
        ThreadRun run,
        ListOrder? resultOrder = default)
    {
        Argument.AssertNotNull(run, nameof(run));

        return GetRunSteps(run.ThreadId, run.Id, resultOrder);
    }

    /// <summary>
    /// Gets a collection of <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to list run steps from. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <returns> A collection of run steps that can be enumerated using <c>foreach</c>. </returns>
    public virtual AsyncPageableCollection<RunStep> GetRunStepsAsync(
        ThreadRun run,
        ListOrder? resultOrder = default)
    {
        Argument.AssertNotNull(run, nameof(run));

        return GetRunStepsAsync(run.ThreadId, run.Id, resultOrder);
    }
}
