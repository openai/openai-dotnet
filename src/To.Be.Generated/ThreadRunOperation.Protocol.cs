using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

// Protocol version
public partial class ThreadRunOperation : OperationResult
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    internal ThreadRunOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        string threadId,
        string runId,
        PipelineResponse response)
        : base(ThreadRunOperationToken.FromOptions(threadId, runId), response)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        _threadId = threadId;
        _runId = runId;

        _pipeline = pipeline;
        _endpoint = endpoint;
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

    /// <summary>
    /// [Protocol Method] Returns a list of run steps belonging to a run.
    /// </summary>
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
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunStepsAsync(int? limit, string order, string after, string before, RequestOptions? options)
    {
        using PipelineMessage message = CreateGetRunStepsRequest(_threadId, _runId, limit, order, after, before, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Returns a list of run steps belonging to a run.
    /// </summary>
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
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRunSteps(int? limit, string order, string after, string before, RequestOptions? options)
    {
        using PipelineMessage message = CreateGetRunStepsRequest(_threadId, _runId, limit, order, after, before, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run step.
    /// </summary>
    /// <param name="stepId"> The ID of the run step to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunStepAsync(string stepId, RequestOptions? options)
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
