using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

// Convenience version
public partial class ThreadRunOperation : OperationResult
{
    private readonly string _threadId;
    private readonly string _runId;

    // TODO: Do the IDs need to be public?
    public string ThreadId => _threadId;

    public string RunId => _runId;

    // Question: what value is being computed here?
    // Hypothesis: it's just the thread run value itself - which is progressively updated
    // over the course of the thread run.
    // Question: is this true for streaming too?  What's the usage pattern here?
    // For now, let's put a ThreadRun object on this that we'll update while polling, 
    // and loop back to see if that abstraction works across both polling and streaming
    // LROs.

    // Note that since ThreadRun is a convenience model, we may need to illustrate
    // protocol and convenience versions of this, and show that evolution.

    // TODO: Add state-machine specific WaitForStatusChange overloads.

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual async Task<ClientResult<ThreadRun>> CancelRunAsync(CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await CancelRunAsync(cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual ClientResult<ThreadRun> CancelRun(CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = CancelRun(cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual async Task<ClientResult<ThreadRun>> SubmitToolOutputsToRunAsync(
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        BinaryContent content = new InternalSubmitToolOutputsRunRequest(toolOutputs).ToBinaryContent();
        ClientResult protocolResult = await SubmitToolOutputsToRunAsync(content, cancellationToken.ToRequestOptions())
            .ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual ClientResult<ThreadRun> SubmitToolOutputsToRun(
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        BinaryContent content = new InternalSubmitToolOutputsRunRequest(toolOutputs).ToBinaryContent();
        ClientResult protocolResult = SubmitToolOutputsToRun(content, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Gets a single run step from a run.
    /// </summary>
    /// <param name="stepId"> The ID of the run step. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="RunStep"/> instance corresponding to the specified step. </returns>
    public virtual async Task<ClientResult<RunStep>> GetRunStepAsync(string stepId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await GetRunStepAsync(stepId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, RunStep.FromResponse);
    }

    /// <summary>
    /// Gets a single run step from a run.
    /// </summary>
    /// <param name="stepId"> The ID of the run step. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="RunStep"/> instance corresponding to the specified step. </returns>
    public virtual ClientResult<RunStep> GetRunStep(string stepId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = GetRunStep(stepId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, RunStep.FromResponse);
    }

    /// <summary>
    /// Gets a collection of <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns></returns>
    public virtual AsyncPageCollection<RunStep> GetRunStepsAsync(
        RunStepCollectionOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        RunStepsPageEnumerator enumerator = new(_pipeline, _endpoint,
            _threadId,
            _runId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    public virtual AsyncPageCollection<RunStep> GetRunStepsAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        RunStepsPageToken pageToken = RunStepsPageToken.FromToken(firstPageToken);
        RunStepsPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.ThreadId,
            pageToken.RunId,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// Gets a collection of <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns></returns>
    public virtual PageCollection<RunStep> GetRunSteps(
        RunStepCollectionOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        RunStepsPageEnumerator enumerator = new(_pipeline, _endpoint,
            _threadId,
            _runId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }

    public virtual PageCollection<RunStep> GetRunSteps(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        RunStepsPageToken pageToken = RunStepsPageToken.FromToken(firstPageToken);
        RunStepsPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.ThreadId,
            pageToken.RunId,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ClientResult<T> CreateResultFromProtocol<T>(
        ClientResult protocolResult,
        Func<PipelineResponse, T> responseDeserializer)
    {
        PipelineResponse pipelineResponse = protocolResult.GetRawResponse();
        T deserializedResultValue = responseDeserializer.Invoke(pipelineResponse);
        return ClientResult.FromValue(deserializedResultValue, pipelineResponse);
    }
}
