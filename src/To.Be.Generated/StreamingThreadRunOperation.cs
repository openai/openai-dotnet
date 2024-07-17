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
public partial class StreamingThreadRunOperation : ThreadRunOperation
{
    private readonly Func<Task<ClientResult>> _createRunAsync;
    private readonly Func<ClientResult> _createRun;

    // TODO: don't have this field in two places.
    private bool _isCompleted;

    private AsyncStreamingUpdateCollection? _currUpdateCollectionAsync;

    private ContinuableAsyncEnumerator<StreamingUpdate> _updateEnumeratorAsync;

    internal StreamingThreadRunOperation(
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

        _updateEnumeratorAsync = new(GetAsyncUpdateEnumerator);
        _currUpdateCollectionAsync ??= new AsyncStreamingUpdateCollection(_createRunAsync);
    }

    public override bool IsCompleted
    {
        get => _isCompleted;
        protected set => _isCompleted = value;
    }

    public override async Task WaitAsync(CancellationToken cancellationToken = default)
    {
        // TODO: add validation that stream is only requested and enumerated once.
        // TODO: Make sure you can't create the same run twice and/or submit tools twice
        // somehow, even accidentally.

        await foreach (var _ in GetUpdatesStreamingAsync(cancellationToken).ConfigureAwait(false))
        {
            // Wait.
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
        IAsyncEnumerator<StreamingUpdate> enumerator = GetStreamingUpdateEnumeratorAsync(cancellationToken);

        try
        {
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                if (enumerator.Current is RunUpdate update)
                {
                    ApplyUpdate(update);
                }

                yield return enumerator.Current;
            }
        }
        finally
        {
            if (enumerator != null) await enumerator.DisposeAsync();
        }
    }

    public IEnumerable<StreamingUpdate> GetUpdatesStreaming(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private IAsyncEnumerator<StreamingUpdate> GetStreamingUpdateEnumeratorAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    // TODO: Figure out visibility here -- protected makes sense but type is
    // internal only
    protected IEnumerator<StreamingUpdate> GetStreamingUpdateEnumerator(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    //public override async Task<bool> UpdateAsync(CancellationToken cancellationToken = default)
    //{
    //    // This does:
    //    //   1. Get update
    //    //   2. Apply update
    //    //   3. Returns whether to continue polling/has more updates

    //    // TODO: use cancellationToken?  How is it plumbed into MoveNext?

    //    if (!await _updateEnumeratorAsync.MoveNextAsync().ConfigureAwait(false))
    //    {
    //        return false;
    //    }

    //    StreamingUpdate update = _updateEnumeratorAsync.Current;
    //    if (update is RunUpdate runUpdate)
    //    {
    //        ApplyUpdate(runUpdate);
    //    }

    //    return !IsCompleted;
    //}

    //public override bool Update(CancellationToken cancellationToken = default)
    //{
    //    throw new NotImplementedException();
    //}

    private void ApplyUpdate(RunUpdate update)
    {
        // Set ThreadId
        ThreadId ??= update.Value.ThreadId;

        // Set RunId
        Id ??= update.Value.Id;

        // Set Status
        Status = update.Value.Status;

        // Set Value
        Value = update.Value;

        // SetRawResponse
        // TODO: This currently won't work as intended because we are setting 
        // _currUpdateCollectionAsync to assist the ContinuableAsyncEnumerator.
        // Revisit this when we close on how enumerator should be exposed.
        if (_currUpdateCollectionAsync is not null)
        {
            SetRawResponse(_currUpdateCollectionAsync.GetRawResponse());
        }

        // Set IsCompleted
        IsCompleted = update.Value.Status.IsTerminal;
    }

    private IAsyncEnumerator<StreamingUpdate>? GetAsyncUpdateEnumerator()
    {
        IAsyncEnumerator<StreamingUpdate>? enumerator = _currUpdateCollectionAsync?.GetAsyncEnumerator();

        // Only get it once until we reset it.

        // TODO: Make sure this doesn't leak a reference to a network stream
        // that then can't be disposed.
        _currUpdateCollectionAsync = null;

        return enumerator;
    }

    public virtual void SubmitToolOutputsToRunStreaming(
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        if (ThreadId is null || Id is null)
        {
            throw new InvalidOperationException("Cannot submit tools until first update stream has been applied.");
        }

        BinaryContent content = new InternalSubmitToolOutputsRunRequest(
            toolOutputs.ToList(), stream: true, null).ToBinaryContent();

        // TODO: can we do this the same way as this in the other method instead
        // of having to take all those funcs?
        async Task<ClientResult> getResultAsync() =>
            await SubmitToolOutputsToRunAsync(ThreadId, Id, content, cancellationToken.ToRequestOptions(streaming: true))
            .ConfigureAwait(false);

        // TODO: Ensure we call SetRawResponse for the current operation.
        // Note: we'll want to do this for the protocol implementation of this method
        // as well.

        // Return the updates as a stream but also update the state as each is returned.

        _currUpdateCollectionAsync = new AsyncStreamingUpdateCollection(getResultAsync);
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
