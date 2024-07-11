using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

// Protocol version
public partial class ThreadRunOperation : OperationResult
{
    private readonly Uri _endpoint;

    private string? _threadId;
    private string? _runId;
    private string? _status;

    private readonly PollingInterval _pollingInterval;

    internal ThreadRunOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        PipelineResponse response)
        : base(pipeline, response)
    {
        _endpoint = endpoint;   
        _pollingInterval = new();
    }

    // Note: these have to work for protocol-only.
    public override Task WaitAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override void Wait(CancellationToken cancellationToken = default)
    {
        // TODO: if don't have a response yet, get that first and ApplyUpdate.

        if (_threadId == null || _runId == null)
        {
            ApplyUpdate(GetRawResponse());
        }

        if (_threadId == null || _runId == null)
        {
            throw new InvalidOperationException("ThreadId or RunId is not set.");
        }

        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            _pollingInterval.Wait();

            // TODO: RequestOptions/CancellationToken logic around this ... ?
            ClientResult result = GetRun(_threadId, _runId, cancellationToken.ToRequestOptions());
            PipelineResponse response = result.GetRawResponse();

            ApplyUpdate(response);

            if (_status == "requires_action")
            {
                throw new InvalidOperationException("Reached a suspended state where operation cannot be completed.  Consider calling WaitForStatusChange method instead.");
            }
        }
        while (!IsCompleted);
    }

    private void ApplyUpdate(PipelineResponse response)
    {
        using JsonDocument doc = JsonDocument.Parse(response.Content);

        _status = doc.RootElement.GetProperty("status"u8).GetString();
        _threadId ??= doc.RootElement.GetProperty("thread_id"u8).GetString();
        _runId ??= doc.RootElement.GetProperty("id"u8).GetString();

        IsCompleted = GetIsCompleted(_status!);

        SetRawResponse(response);
    }

    private static bool GetIsCompleted(string status)
    {
        bool hasCompleted =
            status == "expired" ||
            status == "completed" ||
            status == "failed" ||
            status == "incomplete" ||
            status == "cancelled";

        return hasCompleted;
    }

    #region protocol methods

    /// <summary>
    /// [Protocol Method] Retrieves a run.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) that was run. </param>
    /// <param name="runId"> The ID of the run to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunAsync(string threadId, string runId, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        using PipelineMessage message = CreateGetRunRequest(threadId, runId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) that was run. </param>
    /// <param name="runId"> The ID of the run to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRun(string threadId, string runId, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        using PipelineMessage message = CreateGetRunRequest(threadId, runId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Modifies a run.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) that was run. </param>
    /// <param name="runId"> The ID of the run to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> ModifyRunAsync(string threadId, string runId, BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyRunRequest(threadId, runId, content, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Modifies a run.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) that was run. </param>
    /// <param name="runId"> The ID of the run to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult ModifyRun(string threadId, string runId, BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyRunRequest(threadId, runId, content, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Cancels a run that is `in_progress`.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which this run belongs. </param>
    /// <param name="runId"> The ID of the run to cancel. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CancelRunAsync(string threadId, string runId, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        using PipelineMessage message = CreateCancelRunRequest(threadId, runId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Cancels a run that is `in_progress`.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which this run belongs. </param>
    /// <param name="runId"> The ID of the run to cancel. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CancelRun(string threadId, string runId, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        using PipelineMessage message = CreateCancelRunRequest(threadId, runId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] When a run has the `status: "requires_action"` and `required_action.type` is
    /// `submit_tool_outputs`, this endpoint can be used to submit the outputs from the tool calls once
    /// they're all completed. All outputs must be submitted in a single request.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) to which this run belongs. </param>
    /// <param name="runId"> The ID of the run that requires the tool output submission. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> SubmitToolOutputsToRunAsync(string threadId, string runId, BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage? message = null;
        try
        {
            message = CreateSubmitToolOutputsToRunRequest(threadId, runId, content, options);
            return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message?.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] When a run has the `status: "requires_action"` and `required_action.type` is
    /// `submit_tool_outputs`, this endpoint can be used to submit the outputs from the tool calls once
    /// they're all completed. All outputs must be submitted in a single request.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) to which this run belongs. </param>
    /// <param name="runId"> The ID of the run that requires the tool output submission. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult SubmitToolOutputsToRun(string threadId, string runId, BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage? message = null;
        try
        {
            message = CreateSubmitToolOutputsToRunRequest(threadId, runId, content, options);
            return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message?.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] Returns a list of run steps belonging to a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread the run and run steps belong to. </param>
    /// <param name="runId"> The ID of the run the run steps belong to. </param>
    /// <param name="limit">
    /// A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
    /// default is 20.
    /// </param>
    /// <param name="order">
    /// Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    /// for descending order. Allowed values: "asc" | "desc"
    /// </param>
    /// <param name="after">
    /// A cursor for use in pagination. `after` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include after=obj_foo in order to fetch the next page of the list.
    /// </param>
    /// <param name="before">
    /// A cursor for use in pagination. `before` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include before=obj_foo in order to fetch the previous page of the list.
    /// </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunStepsAsync(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        using PipelineMessage message = CreateGetRunStepsRequest(threadId, runId, limit, order, after, before, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Returns a list of run steps belonging to a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread the run and run steps belong to. </param>
    /// <param name="runId"> The ID of the run the run steps belong to. </param>
    /// <param name="limit">
    /// A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
    /// default is 20.
    /// </param>
    /// <param name="order">
    /// Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    /// for descending order. Allowed values: "asc" | "desc"
    /// </param>
    /// <param name="after">
    /// A cursor for use in pagination. `after` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include after=obj_foo in order to fetch the next page of the list.
    /// </param>
    /// <param name="before">
    /// A cursor for use in pagination. `before` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include before=obj_foo in order to fetch the previous page of the list.
    /// </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRunSteps(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        using PipelineMessage message = CreateGetRunStepsRequest(threadId, runId, limit, order, after, before, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run step.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which the run and run step belongs. </param>
    /// <param name="runId"> The ID of the run to which the run step belongs. </param>
    /// <param name="stepId"> The ID of the run step to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="stepId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="stepId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunStepAsync(string threadId, string runId, string stepId, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNullOrEmpty(stepId, nameof(stepId));

        using PipelineMessage message = CreateGetRunStepRequest(threadId, runId, stepId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run step.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which the run and run step belongs. </param>
    /// <param name="runId"> The ID of the run to which the run step belongs. </param>
    /// <param name="stepId"> The ID of the run step to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="stepId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="stepId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRunStep(string threadId, string runId, string stepId, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNullOrEmpty(stepId, nameof(stepId));

        using PipelineMessage message = CreateGetRunStepRequest(threadId, runId, stepId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    internal PipelineMessage CreateGetRunRequest(string threadId, string runId, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateModifyRunRequest(string threadId, string runId, BinaryContent content, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "POST";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        request.Headers.Set("Content-Type", "application/json");
        request.Content = content;
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateCancelRunRequest(string threadId, string runId, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "POST";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        uri.AppendPath("/cancel", false);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateSubmitToolOutputsToRunRequest(string threadId, string runId, BinaryContent content, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "POST";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        uri.AppendPath("/submit_tool_outputs", false);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        request.Headers.Set("Content-Type", "application/json");
        request.Content = content;
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateGetRunStepsRequest(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        uri.AppendPath("/steps", false);
        if (limit != null)
        {
            uri.AppendQuery("limit", limit.Value, true);
        }
        if (order != null)
        {
            uri.AppendQuery("order", order, true);
        }
        if (after != null)
        {
            uri.AppendQuery("after", after, true);
        }
        if (before != null)
        {
            uri.AppendQuery("before", before, true);
        }
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateGetRunStepRequest(string threadId, string runId, string stepId, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        uri.AppendPath("/steps/", false);
        uri.AppendPath(stepId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
    #endregion
}