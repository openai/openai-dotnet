using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Batch;

internal partial class BatchesPageEnumerator : PageResultEnumerator
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    private readonly int? _limit;
    private readonly RequestOptions _options;

    private string _after;

    public BatchesPageEnumerator(
        ClientPipeline pipeline,
        Uri endpoint,
        string after, int? limit,
        RequestOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;

        _after = after;
        _limit = limit;
        _options = options;
    }

    public override async Task<ClientResult> GetFirstAsync()
        => await GetBatchesAsync(_after, _limit, _options).ConfigureAwait(false);

    public override ClientResult GetFirst()
        => GetBatches(_after, _limit, _options);

    public override async Task<ClientResult> GetNextAsync(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        _after = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return await GetBatchesAsync(_after, _limit, _options).ConfigureAwait(false);
    }

    public override ClientResult GetNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        _after = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return GetBatches(_after, _limit, _options);
    }

    public override bool HasNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }

    internal virtual async Task<ClientResult> GetBatchesAsync(string after, int? limit, RequestOptions options)
    {
        using PipelineMessage message = CreateGetBatchesRequest(after, limit, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    internal virtual ClientResult GetBatches(string after, int? limit, RequestOptions options)
    {
        using PipelineMessage message = CreateGetBatchesRequest(after, limit, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    internal PipelineMessage CreateGetBatchesRequest(string after, int? limit, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/batches", false);
        if (after != null)
        {
            uri.AppendQuery("after", after, true);
        }
        if (limit != null)
        {
            uri.AppendQuery("limit", limit.Value, true);
        }
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}
