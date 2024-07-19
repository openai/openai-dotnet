using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.FineTuning;

internal partial class FineTuningOperationUpdateEnumerator :
    IAsyncEnumerator<ClientResult>,
    IEnumerator<ClientResult>
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;
    private readonly string _jobId;
    private readonly RequestOptions _options;

    private ClientResult? _current;
    private bool _hasNext = true;

    public FineTuningOperationUpdateEnumerator(
        ClientPipeline pipeline,
        Uri endpoint,
        string jobId,
        RequestOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;

        _jobId = jobId;

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

        ClientResult result = GetJob(_jobId, _options);

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

        ClientResult result = await GetJobAsync(_jobId, _options).ConfigureAwait(false);

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

        bool isComplete = status == "succeeded" ||
            status == "failed" ||
            status == "cancelled";

        return !isComplete;
    }

    // Generated methods

    /// <summary>
    /// [Protocol Method] Get info about a fine-tuning job.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetJobAsync(string jobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        using PipelineMessage message = CreateRetrieveFineTuningJobRequest(jobId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Get info about a fine-tuning job.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetJob(string jobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        using PipelineMessage message = CreateRetrieveFineTuningJobRequest(jobId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    internal PipelineMessage CreateRetrieveFineTuningJobRequest(string fineTuningJobId, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/fine_tuning/jobs/", false);
        uri.AppendPath(fineTuningJobId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}
