using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

internal partial class RunOperationUpdateEnumerator :
    IAsyncEnumerator<ClientResult>,
    IEnumerator<ClientResult>
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;
    private readonly string _threadId;
    private readonly string _runId;
    private readonly RequestOptions _options;

    private ClientResult? _current;
    private bool _hasNext = true;

    public RunOperationUpdateEnumerator(
        ClientPipeline pipeline,
        Uri endpoint,
        string threadId,
        string runId,
        RequestOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;

        _threadId = threadId;
        _runId = runId;

        _options = options;
    }

    public ClientResult Current => _current!;

    #region IEnumerator<ClientResult> methods

    object IEnumerator.Current => _current!;

    bool IEnumerator.MoveNext()
    {
        if (!_hasNext)
        {
            _current = null;
            return false;
        }

        ClientResult result = GetRun(_threadId, _runId, _options);

        _current = result;
        _hasNext = HasNext(result);

        return true;
    }

    void IEnumerator.Reset() => _current = null;

    void IDisposable.Dispose() { }

    #endregion

    #region IAsyncEnumerator<ClientResult> methods

    ClientResult IAsyncEnumerator<ClientResult>.Current => _current!;

    public async ValueTask<bool> MoveNextAsync()
    {
        if (!_hasNext)
        {
            _current = null;
            return false;
        }

        ClientResult result = await GetRunAsync(_threadId, _runId, _options).ConfigureAwait(false);

        _current = result;
        _hasNext = HasNext(result);

        return true;
    }

    // TODO: handle Dispose and DisposeAsync using proper patterns?
    ValueTask IAsyncDisposable.DisposeAsync() => default;

    #endregion

    // Methods used by both implementations

    private bool HasNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        // TODO: don't parse JsonDocument twice if possible
        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string? status = doc.RootElement.GetProperty("status"u8).GetString();

        bool isComplete = status == "expired" ||
            status == "completed" ||
            status == "failed" ||
            status == "incomplete" ||
            status == "cancelled";

        return !isComplete;
    }

    // Generated methods

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
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
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
