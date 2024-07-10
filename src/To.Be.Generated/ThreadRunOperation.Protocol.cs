using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

// Protocol version
public partial class ThreadRunOperation : OperationResult
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;
    private readonly RequestOptions? _requestOptions;


    private readonly string _threadId;
    private readonly string _runId;

    private string _status;

    private readonly PollingInterval _pollingInterval;
    private bool _statusChangedFromLastUpdate;

    internal ThreadRunOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        RequestOptions? requestOptions,
        string threadId,
        string runId,
        PipelineResponse response)
        : base(response)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        _threadId = threadId;
        _runId = runId;

        _pipeline = pipeline;
        _endpoint = endpoint;
        _requestOptions = requestOptions;

        _status = GetStatus(response);

        _pollingInterval = new();
    }

    // Factory method - supports returning different OperationResult subtypes
    // from protocol method.
    public static ThreadRunOperation FromResult(OperationResult result)
    {
        if (result is ThreadRunOperation runOperation)
        {
            return runOperation;
        }

        throw new InvalidOperationException("Cannot create 'ThreadRunOperation' from protocol 'OperationResult' when streaming response was specified in request.");
    }

    // Note: these have to work for protocol-only.
    public override Task WaitForCompletionAsync()
    {
        throw new NotImplementedException();
    }

    public override void WaitForCompletion()
    {
        do
        {
            _pollingInterval.Wait();

            ClientResult result = GetRun(_requestOptions);
            PipelineResponse response = result.GetRawResponse();

            ApplyUpdate(response);
        }
        while (!HasCompleted);
    }

    // Note: these have to work for protocol-only, so can't return the status.
    public Task WaitForStatusChangeAsync(/* TODO: Take polling interval param. */)
    {
        throw new NotImplementedException();
    }

    public void WaitForStatusChange(/* TODO: Take polling interval param. */)
    {
        do
        {
            _pollingInterval.Wait();

            ClientResult result = GetRun(_requestOptions);
            PipelineResponse response = result.GetRawResponse();

            ApplyUpdate(response);
        }
        while (!_statusChangedFromLastUpdate);
    }

    private static string GetStatus(PipelineResponse response)
    {
        using JsonDocument doc = JsonDocument.Parse(response.Content);
        return doc.RootElement.GetProperty("status"u8).GetString()!;
    }

    private bool GetHasCompleted(string status)
    {
        bool hasCompleted =
            status == "expired" ||
            status == "completed" ||
            status == "failed" ||
            status == "incomplete" ||
            status == "cancelled";

        return hasCompleted;
    }

    private void ApplyUpdate(PipelineResponse response)
    {
        string status = GetStatus(response);

        HasCompleted = GetHasCompleted(status);
        _statusChangedFromLastUpdate = _status != status;
        _status = status;
        SetRawResponse(response);
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunAsync(RequestOptions? options)
    {
        using PipelineMessage message = CreateGetRunRequest(_threadId, _runId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRun(RequestOptions? options)
    {
        using PipelineMessage message = CreateGetRunRequest(_threadId, _runId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Modifies a run.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> ModifyRunAsync(BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyRunRequest(_threadId, _runId, content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Modifies a run.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult ModifyRun(BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyRunRequest(_threadId, _runId, content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Cancels a run that is `in_progress`.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CancelRunAsync(RequestOptions? options)
    {
        using PipelineMessage message = CreateCancelRunRequest(_threadId, _runId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Cancels a run that is `in_progress`.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CancelRun(RequestOptions? options)
    {
        using PipelineMessage message = CreateCancelRunRequest(_threadId, _runId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] When a run has the `status: "requires_action"` and `required_action.type` is
    /// `submit_tool_outputs`, this endpoint can be used to submit the outputs from the tool calls once
    /// they're all completed. All outputs must be submitted in a single request.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> SubmitToolOutputsToRunAsync(BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage? message = null;
        try
        {
            message = CreateSubmitToolOutputsToRunRequest(_threadId, _runId, content, options);
            return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
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
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult SubmitToolOutputsToRun(BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage? message = null;
        try
        {
            message = CreateSubmitToolOutputsToRunRequest(_threadId, _runId, content, options);
            return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message?.Dispose();
            }
        }
    }

    public virtual IAsyncEnumerable<ClientResult> GetRunStepsAsync(int? limit, string order, string after, string before, RequestOptions options)
    {
        PageResultEnumerator enumerator = new RunStepsPageEnumerator(_pipeline, _endpoint, _threadId, _runId, limit, order, after, before, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    public virtual IEnumerable<ClientResult> GetRunSteps(int? limit, string order, string after, string before, RequestOptions options)
    {
        PageResultEnumerator enumerator = new RunStepsPageEnumerator(_pipeline, _endpoint, _threadId, _runId, limit, order, after, before, options);
        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run step.
    /// </summary>
    /// <param name="stepId"> The ID of the run step to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunStepAsync( string stepId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(stepId, nameof(stepId));

        using PipelineMessage message = CreateGetRunStepRequest(_threadId, _runId, stepId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run step.
    /// </summary>
    /// <param name="stepId"> The ID of the run step to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRunStep(string stepId, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(stepId, nameof(stepId));

        using PipelineMessage message = CreateGetRunStepRequest(_threadId, _runId, stepId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    internal PipelineMessage CreateGetRunRequest(string threadId, string runId, RequestOptions? options)
    {
        var message = _pipeline.CreateMessage();
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
        var message = _pipeline.CreateMessage();
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
        var message = _pipeline.CreateMessage();
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
        var message = _pipeline.CreateMessage();
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
        var message = _pipeline.CreateMessage();
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
        var message = _pipeline.CreateMessage();
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
}