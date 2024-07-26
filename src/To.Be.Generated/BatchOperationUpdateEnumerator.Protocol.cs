using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Batch;

internal partial class BatchOperationUpdateEnumerator :
    IAsyncEnumerator<ClientResult>,
    IEnumerator<ClientResult>
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;
    private readonly RequestOptions _options;

    private readonly string _batchId;

    // TODO: does this one need to be nullable?
    private ClientResult? _current;
    private bool _hasNext = true;

    public BatchOperationUpdateEnumerator(
        ClientPipeline pipeline,
        Uri endpoint,

        string batchId,

        RequestOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;

        _batchId = batchId;

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

        ClientResult result = GetBatch(_batchId, _options);

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

        ClientResult result = await GetBatchAsync(_batchId, _options).ConfigureAwait(false);

        _current = result;
        _hasNext = HasNext(result);

        return true;
    }

    // TODO: handle Dispose and DisposeAsync using proper patterns?
    ValueTask IAsyncDisposable.DisposeAsync() => default;

    #endregion

    private bool HasNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        // TODO: don't parse JsonDocument twice if possible
        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string? status = doc.RootElement.GetProperty("status"u8).GetString();

        bool isComplete = status == "completed" ||
            status == "cancelled" ||
            status == "expired" ||
            status == "failed";

        return !isComplete;
    }

    // Generated methods

    /// <summary>
    /// [Protocol Method] Retrieves a batch.
    /// </summary>
    /// <param name="batchId"> The ID of the batch to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetBatchAsync(string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateRetrieveBatchRequest(batchId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a batch.
    /// </summary>
    /// <param name="batchId"> The ID of the batch to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetBatch(string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateRetrieveBatchRequest(batchId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    internal PipelineMessage CreateRetrieveBatchRequest(string batchId, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/batches/", false);
        uri.AppendPath(batchId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}
