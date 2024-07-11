using System;
using System.ClientModel;
using System.ClientModel.Primitives;
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

    private readonly Func<Task<ClientResult>> _getResultAsync;
    private readonly Func<ClientResult> _getResult;

    // TODO: don't have this in two places.
    private bool _isCompleted;

    internal StreamingThreadRunOperation(
        ClientPipeline pipeline,
        Uri endpoint,

        // Note if we pass funcs we don't need to pass in the pipeline.
        Func<Task<ClientResult>> getResultAsync,
        Func<ClientResult> getResult)
        : base(pipeline, endpoint)
    {
        _getResultAsync = getResultAsync;
        _getResult = getResult;
    }

    public override bool IsCompleted
    {
        get => _isCompleted;
        protected set => _isCompleted = value;
    }

    public override async Task WaitAsync(CancellationToken cancellationToken = default)
    {
        // Create an instance of an IAsyncEnumerable<StreamingUpdate>
        AsyncStreamingUpdateCollection updates = new AsyncStreamingUpdateCollection(_getResultAsync);

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
        throw new NotImplementedException();
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
}
