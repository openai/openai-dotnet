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
public partial class RunOperation : OperationResult
{
    // For use with polling convenience methods where the response has been
    // obtained prior to creation of the LRO type.
    internal RunOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        ThreadRun value,
        RunStatus status,
        RequestOptions options,
        PipelineResponse response)
        : base(response)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
        _options = options;
        _pollingInterval = new();

        if (response.Headers.TryGetValue("Content-Type", out string? contentType) &&
            contentType == "text/event-stream; charset=utf-8")
        {
            throw new ArgumentException("Cannot create polling operation from streaming response.", nameof(response));
        }

        Value = value;
        Status = status;

        ThreadId = value.ThreadId;
        Id = value.Id;

        RehydrationToken = new RunOperationToken(value.ThreadId, value.Id);
    }

    internal RunOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        string threadId,
        string runId,
        RequestOptions options)
        : this(pipeline, endpoint, new RunOperationToken(threadId, runId), options)
    {
    }

    // For use with rehydration client methods where the response has not been
    // obtained yet, but will once the client method makes a call to Update.
    internal RunOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        RunOperationToken token,
        RequestOptions options) : base()
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
        _options = options;
        _pollingInterval = new();

        ThreadId = token.ThreadId;
        Id = token.RunId;

        RehydrationToken = token;
    }

    // Note: these all have to be nullable because the derived streaming type
    // cannot set them until it reads the first event from the SSE stream.

    public string? Id { get => _runId; protected set { _runId = value; } }
    public string? ThreadId { get => _threadId; protected set { _threadId = value; } }

    public ThreadRun? Value { get; protected set; }
    public RunStatus? Status { get; protected set; }


    #region OperationResult methods

    public override Task WaitAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override void Wait(CancellationToken cancellationToken = default)
    {
        if (_isStreaming)
        {
            // We would have to read from the stream to get the run ID to poll for.
            throw new NotSupportedException("Cannot poll for status updates from streaming operation.");
        }

        foreach (ThreadRun update in GetUpdates(cancellationToken: cancellationToken))
        {
            // Don't keep polling if would do so infinitely.
            if (update.Status == RunStatus.RequiresAction)
            {
                return;
            }
        }
    }

    public Task WaitAsync(TimeSpan? pollingInterval, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Wait(TimeSpan? pollingInterval, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        //if (_isStreaming)
        //{
        //    // We would have to read from the stream to get the run ID to poll for.
        //    throw new NotSupportedException("Cannot poll for status updates from streaming operation.");
        //}

        //foreach (ThreadRun update in GetUpdates(pollingInterval, cancellationToken))
        //{
        //    // Don't keep polling if would do so infinitely.
        //    if (update.Status == RunStatus.RequiresAction)
        //    {
        //        return;
        //    }
        //}
    }

    // TODO: evaluate this experiment
    // Expose enumerable APIs similar to the streaming ones.
    public virtual IAsyncEnumerable<ThreadRun> GetUpdatesAsync(
        TimeSpan? pollingInterval = default,
        /*[EnumeratorCancellation]*/ CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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

        IEnumerator<ClientResult<ThreadRun>> enumerator = GetUpdateEnumerator();

        while (enumerator.MoveNext())
        {
            ApplyUpdate(enumerator.Current);

            yield return enumerator.Current;

            cancellationToken.ThrowIfCancellationRequested();

            _pollingInterval.Wait();
        }
    }

    private IAsyncEnumerator<ClientResult<ThreadRun>> GetUpdateEnumeratorAsync()
        // TODO: null check thread and run id
        => new RunOperationUpdateEnumerator(_pipeline, _endpoint, _threadId!, _runId!, _options);

    // TODO: Figure out visibility here -- protected makes sense but type is
    // internal only
    private IEnumerator<ClientResult<ThreadRun>> GetUpdateEnumerator()
        // TODO: null check thread and run id
        => new RunOperationUpdateEnumerator(_pipeline, _endpoint, _threadId!, _runId!, _options);

    private void ApplyUpdate(ClientResult<ThreadRun> update)
    {
        Value = update;
        Status = update.Value.Status;
        IsCompleted = Status.Value.IsTerminal;

        SetRawResponse(update.GetRawResponse());
    }

    #endregion

    #region Convenience overloads of generated protocol methods

    // TODO: decide if we want to keep GetRun methods here - they could enable
    // manual polling scenarios if we wanted to do it that way.

    /// <summary>
    /// Gets an existing <see cref="ThreadRun"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadRun"/> instance. </returns>
    internal virtual async Task<ClientResult<ThreadRun>> GetRunAsync(CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await GetRunAsync(_threadId!, _runId!, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadRun.FromResponse);
    }

    /// <summary>
    /// Gets an existing <see cref="ThreadRun"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadRun"/> instance. </returns>
    internal virtual ClientResult<ThreadRun> GetRun(CancellationToken cancellationToken = default)
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
            Id!,
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