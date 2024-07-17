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

// Streaming version
public partial class StreamingRunOperation : RunOperation
{
    private readonly Func<Task<ClientResult>> _createRunAsync;
    private readonly Func<ClientResult> _createRun;

    private StreamingRunOperationUpdateEnumerator? _enumerator;

    internal StreamingRunOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        RequestOptions options,

        // Note if we pass funcs we don't need to pass in the pipeline.
        Func<Task<ClientResult>> createRunAsync,
        Func<ClientResult> createRun)
        : base(pipeline, endpoint, options)
    {
        _createRunAsync = createRunAsync;
        _createRun = createRun;
    }

    // TODO: this duplicates a field on the base type.  Address?
    public override bool IsCompleted { get; protected set; }

    public override async Task WaitAsync(CancellationToken cancellationToken = default)
    {
        // TODO: add validation that stream is only requested and enumerated once.
        // TODO: Make sure you can't create the same run twice and/or submit tools twice
        // somehow, even accidentally.

        await foreach (StreamingUpdate update in GetUpdatesStreamingAsync(cancellationToken).ConfigureAwait(false))
        {
            // Should terminate naturally when get to "requires action"

            //// Don't keep polling if would do so infinitely.
            //if (update is RunUpdate runUpdate &&
            //    runUpdate.Value.Status == RunStatus.RequiresAction)
            //{
            //    return;
            //}
        }
    }

    public override void Wait(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    // Public APIs specific to streaming LRO
    public async IAsyncEnumerable<StreamingUpdate> GetUpdatesStreamingAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // TODO: validate this against case where user doesn't submit tools...

        if (_enumerator is null)
        {
            AsyncStreamingUpdateCollection updates = new AsyncStreamingUpdateCollection(_createRunAsync);
            _enumerator = new StreamingRunOperationUpdateEnumerator(updates);
        }

        try
        {
            while (await _enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                if (_enumerator.Current is RunUpdate update)
                {
                    ApplyUpdate(update);
                }

                yield return _enumerator.Current;
            }
        }
        finally
        {
            if (_enumerator != null)
            {
                await _enumerator.DisposeAsync();
                _enumerator = null;
            }
        }
    }

    public IEnumerable<StreamingUpdate> GetUpdatesStreaming(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private void ApplyUpdate(ThreadRun update)
    {
        Id ??= update.Id;
        ThreadId ??= update.ThreadId;

        Value = update;
        Status = update.Status;
        IsCompleted = update.Status.IsTerminal;

        SetRawResponse(_enumerator!.GetRawResponse());
    }

    // TODO: should we have an async version of this one?

    public virtual async Task SubmitToolOutputsToRunStreamingAsync(
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        if (ThreadId is null || Id is null)
        {
            throw new InvalidOperationException("Cannot submit tools until first update stream has been applied.");
        }

        if (_enumerator is null)
        {
            throw new InvalidOperationException(
                "Cannot submit tools until first run update stream has been enumerated. " +
                "Call 'Wait' or 'GetUpdatesStreaming' to read update stream.");
        }

        BinaryContent content = new InternalSubmitToolOutputsRunRequest(
            toolOutputs.ToList(), stream: true, null).ToBinaryContent();

        // TODO: can we do this the same way as this in the other method instead
        // of having to take all those funcs?
        async Task<ClientResult> getResultAsync() =>
            await SubmitToolOutputsToRunAsync(ThreadId, Id, content, cancellationToken.ToRequestOptions(streaming: true))
            .ConfigureAwait(false);

        AsyncStreamingUpdateCollection updates = new AsyncStreamingUpdateCollection(getResultAsync);
        await _enumerator.ReplaceUpdateCollectionAsync(updates).ConfigureAwait(false);
    }

    public virtual void SubmitToolOutputsToRunStreaming(
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        if (ThreadId is null || Id is null)
        {
            throw new InvalidOperationException("Cannot submit tools until first update stream has been applied.");
        }

        if (_enumerator is null)
        {
            throw new InvalidOperationException(
                "Cannot submit tools until first run update stream has been enumerated. " +
                "Call 'Wait' or 'GetUpdatesStreaming' to read update stream.");
        }

        BinaryContent content = new InternalSubmitToolOutputsRunRequest(
            toolOutputs.ToList(), stream: true, null).ToBinaryContent();

        // TODO: can we do this the same way as this in the other method instead
        // of having to take all those funcs?
        ClientResult getResult() =>
            SubmitToolOutputsToRun(ThreadId, Id, content, cancellationToken.ToRequestOptions(streaming: true));

        StreamingUpdateCollection updates = new StreamingUpdateCollection(getResult);
        _enumerator.ReplaceUpdateCollection(updates);
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
