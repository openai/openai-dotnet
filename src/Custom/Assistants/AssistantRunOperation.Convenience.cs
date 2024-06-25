using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

public partial class AssistantRunOperation
{
    // Note: Hypothesis that we can remove the APIs that take ThreadRun as a parameter
    // Question: does ThreadRun make sense as a type anymore?

    ///// <summary>
    ///// Gets a refreshed instance of an existing <see cref="ThreadRun"/>.
    ///// </summary>
    ///// <param name="run"> The run to get a refreshed instance of. </param>
    ///// <returns> A new <see cref="ThreadRun"/> instance with updated information. </returns>
    //public virtual Task<ClientResult<ThreadRun>> GetRunAsync(ThreadRun run)
    //    => GetRunAsync(run?.ThreadId, run?.Id);

    ///// <summary>
    ///// Gets a refreshed instance of an existing <see cref="ThreadRun"/>.
    ///// </summary>
    ///// <param name="run"> The run to get a refreshed instance of. </param>
    ///// <returns> A new <see cref="ThreadRun"/> instance with updated information. </returns>
    //public virtual ClientResult<ThreadRun> GetRun(ThreadRun run)
    //    => GetRun(run?.ThreadId, run?.Id);


    ///// <summary>
    ///// Cancels an in-progress <see cref="ThreadRun"/>.
    ///// </summary>
    ///// <param name="run"> The run to cancel. </param>
    ///// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    //public virtual Task<ClientResult<ThreadRun>> CancelRunAsync(ThreadRun run)
    //    => CancelRunAsync(run?.ThreadId, run?.Id);

    ///// <summary>
    ///// Cancels an in-progress <see cref="ThreadRun"/>.
    ///// </summary>
    ///// <param name="run"> The run to cancel. </param>
    ///// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    //public virtual ClientResult<ThreadRun> CancelRun(ThreadRun run)
    //    => CancelRun(run?.ThreadId, run?.Id);


    ///// <summary>
    ///// Submits a collection of required tool call outputs to a run and resumes the run.
    ///// </summary>
    ///// <param name="toolOutputs">
    ///// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    ///// </param>
    ///// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    //public virtual Task<ClientResult<ThreadRun>> SubmitToolOutputsToRunAsync(
    //    IEnumerable<ToolOutput> toolOutputs)
    //        => SubmitToolOutputsToRunAsync(toolOutputs);

    ///// <summary>
    ///// Submits a collection of required tool call outputs to a run and resumes the run.
    ///// </summary>
    ///// <param name="toolOutputs">
    ///// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    ///// </param>
    ///// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    //public virtual ClientResult<ThreadRun> SubmitToolOutputsToRun(
    //    IEnumerable<ToolOutput> toolOutputs)
    //        => SubmitToolOutputsToRun(toolOutputs);

    ///// <summary>
    ///// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    ///// </summary>
    ///// <param name="toolOutputs">
    ///// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    ///// </param>
    //public virtual AsyncResultCollection<StreamingUpdate> SubmitToolOutputsToRunStreamingAsync(
    //    IEnumerable<ToolOutput> toolOutputs)
    //        => SubmitToolOutputsToRunStreamingAsync(toolOutputs);

    ///// <summary>
    ///// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    ///// </summary>
    ///// <param name="toolOutputs">
    ///// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    ///// </param>
    //public virtual ResultCollection<StreamingUpdate> SubmitToolOutputsToRunStreaming(
    //    IEnumerable<ToolOutput> toolOutputs)
    //        => SubmitToolOutputsToRunStreaming(toolOutputs);

    ///// <summary>
    ///// Gets a collection of <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    ///// </summary>
    ///// <param name="resultOrder">
    ///// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    ///// timestamp.
    ///// </param>
    ///// <returns> A collection of run steps that can be enumerated using <c>await foreach</c>. </returns>
    //public virtual PageableCollection<RunStep> GetRunSteps(
    //    ListOrder? resultOrder = default)
    //{
    //    return GetRunSteps(resultOrder);
    //}

    ///// <summary>
    ///// Gets a collection of <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    ///// </summary>
    ///// <param name="resultOrder">
    ///// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    ///// timestamp.
    ///// </param>
    ///// <returns> A collection of run steps that can be enumerated using <c>foreach</c>. </returns>
    //public virtual AsyncPageableCollection<RunStep> GetRunStepsAsync(
    //    ListOrder? resultOrder = default)
    //{
    //    return GetRunStepsAsync(resultOrder);
    //}
}
