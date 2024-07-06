using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

// This gets generated client-specific
// There is an evolution story around this from protocol to convenience
// When protocol-only, this inherits from OperationResultPoller
// When conveniences are added, this inherits from OperationPoller<ThreadRun>.
internal class ThreadRunPoller : OperationPoller<ThreadRun>
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    public readonly string _threadId;
    public readonly string _runId;

    private readonly RequestOptions _options;

    internal ThreadRunPoller(
        ClientPipeline pipeline,
        Uri endpoint,
        ClientResult result,
        string threadId,
        string runId,
        RequestOptions options) : base(result)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;

        _threadId = threadId;
        _runId = runId;

        _options = options;
    }

    public override ThreadRun GetValueFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();
        return ThreadRun.FromResponse(response);
    }

    // Poller subclient method implementations
    public override async Task<ClientResult> UpdateStatusAsync()
        => await GetRunAsync(_options).ConfigureAwait(false);

    public override ClientResult UpdateStatus()
        => GetRun(_options);

    public override bool HasStopped(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string status = doc.RootElement.GetProperty("status"u8).GetString()!;

        bool hasStopped =
            status == "expired" ||
            status == "completed" ||
            status == "failed" ||
            status == "incomplete" ||
            status == "cancelled";

        return hasStopped;
    }

    // Protocol methods

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

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}
