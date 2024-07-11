//using System;
//using System.ClientModel;
//using System.ClientModel.Primitives;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Net.ServerSentEvents;
//using System.Threading;
//using System.Threading.Tasks;

//#nullable enable

//namespace OpenAI.Assistants;

//// Protocol version
//public partial class StreamingThreadRunOperation : ThreadRunOperation
//{
//    private readonly Uri _endpoint;

//    // TODO: allocate differently based on delayed request or not per IDisposable
//    private IAsyncEnumerable<SseItem<byte[]>>? _asyncEventStream;
//    //private IEnumerable<SseItem<byte[]>>? _eventStream;

//    // Note: what does it mean to have a protocol type for status?
//    private string? _status;

//    //   internal StreamingThreadRunOperation(
//    //       ClientPipeline pipeline,
//    //       Uri endpoint,
//    //       RequestOptions? requestOptions,
//    //       string threadId,
//    //       string runId) : base(pipeline)
//    //   {
//    //       Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
//    //       Argument.AssertNotNullOrEmpty(runId, nameof(runId));

//    //       _threadId = threadId;
//    //       _runId = runId;

//    //       _endpoint = endpoint;
//    //       _requestOptions = requestOptions;
//    //}

//    // Constructor used in protocol method - takes existing response
//    internal StreamingThreadRunOperation(
//        ClientPipeline pipeline,
//        Uri endpoint,
//        RequestOptions? requestOptions,
//        string threadId,
//        PipelineResponse response) : base(pipeline, response)
//    {

//    }

//    //public override async Task WaitAsync()
//    //{
//    //    PipelineResponse response = GetRawResponse();

//    //    // TODO: switch based on whether response has been received yet or not
//    //    Debug.Assert(response.ContentStream is not null);

//    //    IAsyncEnumerable<SseItem<byte[]>> events = SseParser.Create(
//    //            response.ContentStream!,
//    //            (_, bytes) => bytes.ToArray()).EnumerateAsync();

//    //    // TODO: plumb through cancellation token
//    //    await foreach (SseItem<byte[]> item in events)
//    //    {
//    //        if (IsUpdateEvent(item))
//    //        {
//    //            ApplyUpdate(item);
//    //        }
//    //    }

//    //    // Note: we should throw if user called this and we're in a state
//    //    // where it can't be completed and can't be resumed.
//    //    if (!HasCompleted)
//    //    {
//    //        throw new InvalidOperationException("Reached the end of the event stream without completing.  Consider calling WaitForStatusChange method instead.");
//    //    }
//    //}

//    //public override void WaitForCompletion()
//    //{
//    //    throw new NotImplementedException();
//    //}

//    //// Note: these have to work for protocol-only, so can't return the status.
//    //public async Task<string> WaitForStatusChangeAsync(RequestOptions? options)
//    //{
//    //    //if (_eventStream is not null)
//    //    //{
//    //    //    throw new InvalidOperationException("Cannot stream events asynchronously after synchronous streaming has begun.");
//    //    //}

//    //    if (_asyncEventStream is null)
//    //    {
//    //        // TODO: request SSE stream if haven't yet

//    //        _asyncEventStream = SseParser.Create(
//    //            GetRawResponse().ContentStream!,
//    //            (_, bytes) => bytes.ToArray()).EnumerateAsync();
//    //    }

//    //    CancellationToken cancellationToken = options?.CancellationToken ?? default;
//    //    await foreach (SseItem<byte[]> item in _asyncEventStream.WithCancellation(cancellationToken))
//    //    {
//    //        if (IsUpdateEvent(item))
//    //        {
//    //            string? priorStatus = _status;
//    //            ApplyUpdate(item);
//    //            if (priorStatus != _status)
//    //            {
//    //                return _status!;
//    //            }
//    //        }
//    //    }

//    //    return _status!;
//    //}

//    //// Returns state differently based on protocol vs convenience -- options vs.
//    //// cancellation token used to differentiate as in other clients.
//    //public string WaitForStatusChange(RequestOptions? options)
//    //{
//    //    throw new NotImplementedException();
//    //}

//    //private bool IsUpdateEvent(SseItem<byte[]> item)
//    //{
//    //    string name = item.EventType;

//    //    bool isUpdateEvent =
//    //        name == "thread.run.created" ||
//    //        name == "thread.run.queued" ||
//    //        name == "thread.run.in_progress" ||
//    //        name == "thread.run.requires_action" ||
//    //        name == "thread.run.cancelling" ||
//    //        name == "thread.run.cancelled" ||
//    //        name == "thread.run.failed" ||
//    //        name == "thread.run.completed" ||
//    //        name == "thread.run.incomplete" ||
//    //        name == "thread.run.expired";

//    //    return isUpdateEvent;
//    //}

//    //private void ApplyUpdate(SseItem<byte[]> update)
//    //{
//    //    _status = GetStatus(update);
//    //    HasCompleted = GetHasCompleted(_status);
//    //}

//    //private static string GetStatus(SseItem<byte[]> update)
//    //{
//    //    // Take "thread.run." off the front of the name
//    //    // TODO: perf
//    //    return update.EventType.AsSpan().Slice("thread.run.".Length).ToString();
//    //}

//    //private static bool GetHasCompleted(string status)
//    //{
//    //    bool hasCompleted =
//    //        status == "expired" ||
//    //        status == "completed" ||
//    //        status == "failed" ||
//    //        status == "incomplete" ||
//    //        status == "cancelled";

//    //    return hasCompleted;
//    //}
//}