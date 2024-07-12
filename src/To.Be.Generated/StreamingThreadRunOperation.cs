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

public partial class StreamingThreadRunOperation : ThreadRunOperation
{
    // TODO: These will move to convenience base type.
    public string? ThreadId { get; private set; }
    public string? RunId { get; private set; }

    public ThreadRun? Value { get; private set; }
    public RunStatus? Status { get; private set; }

    private readonly Func<Task<ClientResult>> _createRunAsync;
    private readonly Func<ClientResult> _createRun;

    // TODO: don't have this field in two places.
    private bool _isCompleted;

    internal StreamingThreadRunOperation(
        ClientPipeline pipeline,
        Uri endpoint,

        // Note if we pass funcs we don't need to pass in the pipeline.
        Func<Task<ClientResult>> createRunAsync,
        Func<ClientResult> createRun)
        : base(pipeline, endpoint)
    {
        _createRunAsync = createRunAsync;
        _createRun = createRun;
    }

    public override bool IsCompleted
    {
        get => _isCompleted;
        protected set => _isCompleted = value;
    }

    public override async Task WaitAsync(CancellationToken cancellationToken = default)
    {
        // TODO: add validation that stream is only requested and enumerated once!

        // Create an instance of an IAsyncEnumerable<StreamingUpdate>
        AsyncStreamingUpdateCollection updates = new AsyncStreamingUpdateCollection(_createRunAsync);

        // Enumerate those updates and update the state for each one
        await foreach (StreamingUpdate update in updates.WithCancellation(cancellationToken))
        {
            if (update is RunUpdate runUpdate)
            {
                ApplyUpdate(runUpdate);
            }
        }
    }

    public override void Wait(CancellationToken cancellationToken = default)
    {
        // Create an instance of an IAsyncEnumerable<StreamingUpdate>
        StreamingUpdateCollection updates = new StreamingUpdateCollection(_createRun);

        // Enumerate those updates and update the state for each one
        foreach (StreamingUpdate update in updates)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (update is RunUpdate runUpdate)
            {
                ApplyUpdate(runUpdate);
            }
        }
    }

    private void ApplyUpdate(RunUpdate update)
    {
        // Set ThreadId
        ThreadId ??= update.Value.ThreadId;

        // Set RunId
        RunId ??= update.Value.Id;

        // Set Status
        Status = update.Value.Status;

        // Set Value
        Value = update.Value;

        // DON'T SetRawResponse

        // Set IsCompleted
        IsCompleted = update.Value.Status == RunStatus.Completed;
    }

    // Public APIs specific to streaming LRO
    public async IAsyncEnumerable<StreamingUpdate> GetUpdatesStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        AsyncStreamingUpdateCollection updates = new AsyncStreamingUpdateCollection(_createRunAsync);

        // Enumerate those updates and update the state for each one
        await foreach (StreamingUpdate update in updates.WithCancellation(cancellationToken))
        {
            if (update is RunUpdate runUpdate)
            {
                ApplyUpdate(runUpdate);
            }

            yield return update;
        }
    }

    public IEnumerable<StreamingUpdate> GetUpdatesStreaming(CancellationToken cancellationToken = default)
    {
        StreamingUpdateCollection updates = new StreamingUpdateCollection(_createRun);

        // Enumerate those updates and update the state for each one
        foreach (StreamingUpdate update in updates)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (update is RunUpdate runUpdate)
            {
                ApplyUpdate(runUpdate);
            }

            yield return update;
        }
    }

    // TODO: the below is the equivalent of "GetUpdatesStreaming"; what is the 
    // equivalent of Wait?

    public virtual async IAsyncEnumerable<StreamingUpdate> SubmitToolOutputsToRunStreamingAsync(
        IEnumerable<ToolOutput> toolOutputs,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (ThreadId is null || RunId is null)
        {
            throw new InvalidOperationException("Cannot submit tools until first update stream has been applied.");
        }

        BinaryContent content = new InternalSubmitToolOutputsRunRequest(
            toolOutputs.ToList(), stream: true, null).ToBinaryContent();

        // TODO: can we do this the same way as this in the other method instead
        // of having to take all those funcs?
        async Task<ClientResult> getResultAsync() =>
            await SubmitToolOutputsToRunAsync(ThreadId, RunId, content, cancellationToken.ToRequestOptions(streaming: true))
            .ConfigureAwait(false);

        // TODO: Ensure we call SetRawResponse for the current operation.
        // Note: we'll want to do this for the protocol implementation of this method
        // as well.

        // Return the updates as a stream but also update the state as each is returned.

        AsyncStreamingUpdateCollection updates = new AsyncStreamingUpdateCollection(getResultAsync);

        await foreach (StreamingUpdate update in updates.WithCancellation(cancellationToken))
        {
            // TODO: we should only need to set this once, can optimize.s
            SetRawResponse(updates.GetRawResponse());

            if (update is RunUpdate runUpdate)
            {
                ApplyUpdate(runUpdate);
            }

            yield return update;
        }
    }

    #region hide

    //// used to defer first request.
    //internal virtual async Task<ClientResult> CreateRunAsync(string threadId, BinaryContent content, RequestOptions? options = null)
    //{
    //    Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
    //    Argument.AssertNotNull(content, nameof(content));

    //    PipelineMessage? message = null;
    //    try
    //    {
    //        message = CreateCreateRunRequest(threadId, content, options);
    //        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    //    }
    //    finally
    //    {
    //        if (options?.BufferResponse != false)
    //        {
    //            message?.Dispose();
    //        }
    //    }
    //}

    //internal PipelineMessage CreateCreateRunRequest(string threadId, BinaryContent content, RequestOptions? options)
    //{
    //    var message = Pipeline.CreateMessage();
    //    message.ResponseClassifier = PipelineMessageClassifier200;
    //    var request = message.Request;
    //    request.Method = "POST";
    //    var uri = new ClientUriBuilder();
    //    uri.Reset(_endpoint);
    //    uri.AppendPath("/threads/", false);
    //    uri.AppendPath(threadId, true);
    //    uri.AppendPath("/runs", false);
    //    request.Uri = uri.ToUri();
    //    request.Headers.Set("Accept", "application/json");
    //    request.Headers.Set("Content-Type", "application/json");
    //    request.Content = content;
    //    message.Apply(options);
    //    return message;
    //}

    //private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    //private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
    #endregion
}
