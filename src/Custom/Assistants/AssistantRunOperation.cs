using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.InternalListHelpers;
using static OpenAI.CancellationTokenExtensions;

namespace OpenAI.Assistants;

public partial class AssistantRunOperation // TODO: inherit from SCM ResultValueOperation type
{
    private readonly string _threadId;
    private readonly string _runId;
    private readonly InternalAssistantRunClient _runSubClient;

    internal AssistantRunOperation(string threadId, string runId, InternalAssistantRunClient runSubClient)
    {
        _threadId = threadId;
        _runId = runId;
        _runSubClient = runSubClient;
    }

    /// <summary>
    /// Gets an existing <see cref="ThreadRun"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve the run from. </param>
    /// <param name="runId"> The ID of the run to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadRun"/> instance. </returns>
    public virtual async Task<ClientResult<ThreadRun>> GetRunAsync(string threadId, string runId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = await GetRunAsync(threadId, runId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Gets an existing <see cref="ThreadRun"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve the run from. </param>
    /// <param name="runId"> The ID of the run to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadRun"/> instance. </returns>
    public virtual ClientResult<ThreadRun> GetRun(string threadId, string runId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = GetRun(threadId, runId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }


    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run to cancel. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual async Task<ClientResult<ThreadRun>> CancelRunAsync(string threadId, string runId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = await CancelRunAsync(threadId, runId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run to cancel. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual ClientResult<ThreadRun> CancelRun(string threadId, string runId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = CancelRun(threadId, runId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="threadId"> The thread ID of the thread being run. </param>
    /// <param name="runId"> The ID of the run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual async Task<ClientResult<ThreadRun>> SubmitToolOutputsToRunAsync(
        string threadId,
        string runId,
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        BinaryContent content = new InternalSubmitToolOutputsRunRequest(toolOutputs).ToBinaryContent();
        ClientResult protocolResult = await SubmitToolOutputsToRunAsync(threadId, runId, content, cancellationToken.ToRequestOptions())
            .ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="threadId"> The thread ID of the thread being run. </param>
    /// <param name="runId"> The ID of the run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual ClientResult<ThreadRun> SubmitToolOutputsToRun(
        string threadId,
        string runId,
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        BinaryContent content = new InternalSubmitToolOutputsRunRequest(toolOutputs).ToBinaryContent();
        ClientResult protocolResult = SubmitToolOutputsToRun(threadId, runId, content, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    /// </summary>
    /// <param name="threadId"> The thread ID of the thread being run. </param>
    /// <param name="runId"> The ID of the run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    public virtual AsyncResultCollection<StreamingUpdate> SubmitToolOutputsToRunStreamingAsync(
        string threadId,
        string runId,
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        BinaryContent content = new InternalSubmitToolOutputsRunRequest(toolOutputs.ToList(), stream: true, null)
            .ToBinaryContent();

        async Task<ClientResult> getResultAsync() =>
            await SubmitToolOutputsToRunAsync(threadId, runId, content, cancellationToken.ToRequestOptions(streaming: true))
            .ConfigureAwait(false);

        return new AsyncStreamingUpdateCollection(getResultAsync);
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    /// </summary>
    /// <param name="threadId"> The thread ID of the thread being run. </param>
    /// <param name="runId"> The ID of the run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    public virtual ResultCollection<StreamingUpdate> SubmitToolOutputsToRunStreaming(
        string threadId,
        string runId,
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        BinaryContent content = new InternalSubmitToolOutputsRunRequest(toolOutputs.ToList(), stream: true, null)
            .ToBinaryContent();

        ClientResult getResult() => SubmitToolOutputsToRun(threadId, runId, content, cancellationToken.ToRequestOptions(streaming: true));

        return new StreamingUpdateCollection(getResult);
    }

    /// <summary>
    /// Gets a collection of <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run to list run steps from. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of run steps that can be enumerated using <c>await foreach</c>. </returns>
    public virtual AsyncPageableCollection<RunStep> GetRunStepsAsync(
        string threadId,
        string runId,
        ListOrder? resultOrder = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        return CreateAsyncPageable<RunStep, InternalListRunStepsResponse>((continuationToken, pageSize)
            => GetRunStepsAsync(threadId, runId, pageSize, resultOrder?.ToString(), continuationToken, null, cancellationToken.ToRequestOptions()));
    }

    /// <summary>
    /// Gets a collection of <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run to list run steps from. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of run steps that can be enumerated using <c>foreach</c>. </returns>
    public virtual PageableCollection<RunStep> GetRunSteps(
        string threadId,
        string runId,
        ListOrder? resultOrder = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        return CreatePageable<RunStep, InternalListRunStepsResponse>((continuationToken, pageSize)
            => GetRunSteps(threadId, runId, pageSize, resultOrder?.ToString(), continuationToken, null, cancellationToken.ToRequestOptions()));
    }

    /// <summary>
    /// Gets a single run step from a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run. </param>
    /// <param name="stepId"> The ID of the run step. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="RunStep"/> instance corresponding to the specified step. </returns>
    public virtual async Task<ClientResult<RunStep>> GetRunStepAsync(string threadId, string runId, string stepId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await GetRunStepAsync(threadId, runId, stepId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, RunStep.FromResponse);
    }


    /// <summary>
    /// Gets a single run step from a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run. </param>
    /// <param name="stepId"> The ID of the run step. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="RunStep"/> instance corresponding to the specified step. </returns>
    public virtual ClientResult<RunStep> GetRunStep(string threadId, string runId, string stepId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = GetRunStep(threadId, runId, stepId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, RunStep.FromResponse);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ClientResult<T> CreateResultFromProtocol<T>(ClientResult protocolResult, Func<PipelineResponse, T> responseDeserializer)
    {
        PipelineResponse pipelineResponse = protocolResult?.GetRawResponse();
        T deserializedResultValue = responseDeserializer.Invoke(pipelineResponse);
        return ClientResult.FromValue(deserializedResultValue, pipelineResponse);
    }
}
