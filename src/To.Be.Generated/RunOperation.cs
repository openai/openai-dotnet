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
public partial class RunOperation : ClientResult
{
    // For use with polling convenience methods where the response has been
    // obtained prior to creation of the LRO type.
    internal RunOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        ThreadRun value,
        RunStatus status,
        PipelineResponse response)
        : base(response)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
        _pollingInterval = new();

        if (response.Headers.TryGetValue("Content-Type", out string? contentType) &&
            contentType == "text/event-stream; charset=utf-8")
        {
            throw new ArgumentException("Cannot create polling operation from streaming response.", nameof(response));
        }

        Value = value;
        Status = status;

        ThreadId = value.ThreadId;
        RunId = value.Id;

        RehydrationToken = new RunOperationToken(value.ThreadId, value.Id);
    }

    // For use with streaming convenience methods - response hasn't been provided yet.
    internal RunOperation(
        ClientPipeline pipeline,
        Uri endpoint)
        : base()
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
        
        // This constructor is provided for streaming convenience method only.
        // Because of this, we don't set the polling interval type.
    }

    // Note: these all have to be nullable because the derived streaming type
    // cannot set them until it reads the first event from the SSE stream.
    public string? RunId { get => _runId; protected set => _runId = value; }
    public string? ThreadId { get => _threadId; protected set => _threadId = value; }

    public ThreadRun? Value { get; protected set; }
    public RunStatus? Status { get; protected set; }

    #region OperationResult methods

    public virtual async Task WaitUntilStoppedAsync(CancellationToken cancellationToken = default)
        => await WaitUntilStoppedAsync(default, cancellationToken).ConfigureAwait(false);

    public virtual void WaitUntilStopped(CancellationToken cancellationToken = default)
        => WaitUntilStopped(default, cancellationToken);

    public virtual async Task WaitUntilStoppedAsync(TimeSpan? pollingInterval, CancellationToken cancellationToken = default)
    {
        if (IsStreaming)
        {
            // We would have to read from the stream to get the run ID to poll for.
            throw new NotSupportedException("Cannot poll for status updates from streaming operation.");
        }

        await foreach (ThreadRun update in GetUpdatesAsync(pollingInterval, cancellationToken))
        {
            // Don't keep polling if would do so infinitely.
            if (update.Status == RunStatus.RequiresAction)
            {
                return;
            }
        }
    }

    public virtual void WaitUntilStopped(TimeSpan? pollingInterval, CancellationToken cancellationToken = default)
    {
        if (IsStreaming)
        {
            // We would have to read from the stream to get the run ID to poll for.
            throw new NotSupportedException("Cannot poll for status updates from streaming operation.");
        }

        foreach (ThreadRun update in GetUpdates(pollingInterval, cancellationToken))
        {
            // Don't keep polling if would do so infinitely.
            if (update.Status == RunStatus.RequiresAction)
            {
                return;
            }
        }
    }

    // Expose enumerable APIs similar to the streaming ones.
    public virtual async IAsyncEnumerable<ThreadRun> GetUpdatesAsync(
        TimeSpan? pollingInterval = default,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (pollingInterval is not null)
        {
            // TODO: don't allocate
            _pollingInterval = new PollingInterval(pollingInterval);
        }

        IAsyncEnumerator<ClientResult<ThreadRun>> enumerator =
            new RunOperationUpdateEnumerator(_pipeline, _endpoint, _threadId!, _runId!, cancellationToken);

        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            ApplyUpdate(enumerator.Current);

            yield return enumerator.Current;

            // TODO: do we need null check?
            await _pollingInterval!.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public virtual IEnumerable<ThreadRun> GetUpdates(
        TimeSpan? pollingInterval = default,
        CancellationToken cancellationToken = default)
    {
        if (pollingInterval is not null)
        {
            // TODO: don't allocate
            _pollingInterval = new PollingInterval(pollingInterval);
        }

        IEnumerator<ClientResult<ThreadRun>> enumerator = new RunOperationUpdateEnumerator(
            _pipeline, _endpoint, _threadId!, _runId!, cancellationToken);

        while (enumerator.MoveNext())
        {
            ApplyUpdate(enumerator.Current);

            yield return enumerator.Current;

            // TODO: do we need null check?
            _pollingInterval!.Wait();
        }
    }

    private void ApplyUpdate(ClientResult<ThreadRun> update)
    {
        Value = update;
        Status = update.Value.Status;
        IsCompleted = Status.Value.IsTerminal;

        SetRawResponse(update.GetRawResponse());
    }

    #endregion

    #region Convenience overloads of generated protocol methods

    /// <summary>
    /// Gets an existing <see cref="ThreadRun"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadRun"/> instance. </returns>
    public virtual async Task<ClientResult<ThreadRun>> GetRunAsync(CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await GetRunAsync(_threadId!, _runId!, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Gets an existing <see cref="ThreadRun"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadRun"/> instance. </returns>
    public virtual ClientResult<ThreadRun> GetRun(CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = GetRun(_threadId!, _runId!, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual async Task<ClientResult<ThreadRun>> CancelRunAsync(CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await CancelRunAsync(_threadId!, _runId!, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual ClientResult<ThreadRun> CancelRun(CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = CancelRun(_threadId!, _runId!, cancellationToken.ToRequestOptions());
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
    public virtual async Task SubmitToolOutputsToRunAsync(
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        BinaryContent content = new InternalSubmitToolOutputsRunRequest(toolOutputs).ToBinaryContent();
        ClientResult protocolResult = await SubmitToolOutputsToRunAsync(_threadId!, _runId!, content, cancellationToken.ToRequestOptions())
            .ConfigureAwait(false);
        ClientResult<ThreadRun> update = CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
        ApplyUpdate(update);
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual void SubmitToolOutputsToRun(
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        BinaryContent content = new InternalSubmitToolOutputsRunRequest(toolOutputs).ToBinaryContent();
        ClientResult protocolResult = SubmitToolOutputsToRun(_threadId!, _runId!, content, cancellationToken.ToRequestOptions());
        ClientResult<ThreadRun> update = CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
        ApplyUpdate(update);
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
        RunStepsPageEnumerator enumerator = new(_pipeline, _endpoint,
            _threadId!,
            _runId!,
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
    /// Gets a page collection holding <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="RunStep"/>. </returns>
    public virtual PageCollection<RunStep> GetRunSteps(
        RunStepCollectionOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        RunStepsPageEnumerator enumerator = new(_pipeline, _endpoint,
            ThreadId!,
            RunId!,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="RunStep"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="RunStep"/>. </returns>
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

    /// <summary>
    /// Gets a single run step from a run.
    /// </summary>
    /// <param name="stepId"> The ID of the run step. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="RunStep"/> instance corresponding to the specified step. </returns>
    public virtual async Task<ClientResult<RunStep>> GetRunStepAsync(string stepId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await GetRunStepAsync(_threadId!, _runId!, stepId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
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
        ClientResult protocolResult = GetRunStep(_threadId!, _runId!, stepId, cancellationToken.ToRequestOptions());
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