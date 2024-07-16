using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

// Convenience version
public partial class ThreadRunOperation : OperationResult
{
    public string? ThreadId { get; protected set; }
    public string? RunId { get; protected set; }

    #region Convenience overloads of generated protocol methods

    /// <summary>
    /// Gets an existing <see cref="ThreadRun"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadRun"/> instance. </returns>
    public virtual async Task<ClientResult<ThreadRun>> GetRunAsync(CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = await GetRunAsync(threadId, runId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Gets an existing <see cref="ThreadRun"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadRun"/> instance. </returns>
    public virtual ClientResult<ThreadRun> GetRun(CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = GetRun(threadId, runId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual async Task<ClientResult<ThreadRun>> CancelRunAsync(CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = await CancelRunAsync(threadId, runId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual ClientResult<ThreadRun> CancelRun(CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = CancelRun(threadId, runId, cancellationToken.ToRequestOptions());
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
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual ClientResult<ThreadRun> SubmitToolOutputsToRun(
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
    /// Gets a page collection holding <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="RunStep"/>. </returns>
    public virtual AsyncPageCollection<RunStep> GetRunStepsAsync(
        RunStepCollectionOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        RunStepsPageEnumerator enumerator = new(_pipeline, _endpoint,
            threadId,
            runId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="RunStep"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="RunStep"/>. </returns>
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
    /// Gets a single run step from a run.
    /// </summary>
    /// <param name="stepId"> The ID of the run step. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="RunStep"/> instance corresponding to the specified step. </returns>
    public virtual async Task<ClientResult<RunStep>> GetRunStepAsync(string stepId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await GetRunStepAsync(threadId, runId, stepId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
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
        ClientResult protocolResult = GetRunStep(threadId, runId, stepId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, RunStep.FromResponse);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ClientResult<T> CreateResultFromProtocol<T>(ClientResult protocolResult, Func<PipelineResponse, T> responseDeserializer)
    {
        PipelineResponse pipelineResponse = protocolResult.GetRawResponse();
        T deserializedResultValue = responseDeserializer.Invoke(pipelineResponse);
        return ClientResult.FromValue(deserializedResultValue, pipelineResponse);
    }

    #endregion
}