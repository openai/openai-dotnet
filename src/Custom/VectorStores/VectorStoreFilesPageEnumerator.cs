using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

internal partial class VectorStoreFilesPageEnumerator : PageEnumerator<VectorStoreFileAssociation>
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    private readonly string _vectorStoreId;
    private readonly int? _limit;
    private readonly string? _order;

    private readonly string? _before;
    private readonly string? _filter;
    private readonly RequestOptions _options;

    private string? _after;

    public VectorStoreFilesPageEnumerator(
        ClientPipeline pipeline,
        Uri endpoint,
        string vectorStoreId, 
        int? limit, string? order, string? after, string? before, string? filter,
        RequestOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;

        _vectorStoreId = vectorStoreId;
        _limit = limit;
        _order = order;
        _after = after;
        _before = before;
        _filter = filter;
        _options = options;
    }

    public override async Task<ClientResult> GetFirstAsync()
        => await GetFileAssociationsAsync(_vectorStoreId, _limit, _order, _after, _before, _filter, _options).ConfigureAwait(false);

    public override ClientResult GetFirst()
        => GetFileAssociations(_vectorStoreId, _limit, _order, _after, _before, _filter, _options);

    public override async Task<ClientResult> GetNextAsync(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        _after = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return await GetFileAssociationsAsync(_vectorStoreId, _limit, _order, _after, _before, _filter, _options).ConfigureAwait(false);
    }

    public override ClientResult GetNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        _after = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return GetFileAssociations(_vectorStoreId, _limit, _order, _after, _before, _filter, _options);
    }

    public override bool HasNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }

    public override PageResult<VectorStoreFileAssociation> GetPageFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        InternalListVectorStoreFilesResponse list = ModelReaderWriter.Read<InternalListVectorStoreFilesResponse>(response.Content)!;

        VectorStoreFilesPageToken pageToken = VectorStoreFilesPageToken.FromOptions(_vectorStoreId, _limit, _order, _after, _before, _filter);
        VectorStoreFilesPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<VectorStoreFileAssociation>.Create(list.Data, pageToken, nextPageToken, response);
    }

    internal virtual async Task<ClientResult> GetFileAssociationsAsync(string vectorStoreId, int? limit, string? order, string? after, string? before, string? filter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        using PipelineMessage message = CreateGetVectorStoreFilesRequest(vectorStoreId, limit, order, after, before, filter, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    internal virtual ClientResult GetFileAssociations(string vectorStoreId, int? limit, string? order, string? after, string? before, string? filter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        using PipelineMessage message = CreateGetVectorStoreFilesRequest(vectorStoreId, limit, order, after, before, filter, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    internal PipelineMessage CreateGetVectorStoreFilesRequest(string vectorStoreId, int? limit, string? order, string? after, string? before, string? filter, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/vector_stores/", false);
        uri.AppendPath(vectorStoreId, true);
        uri.AppendPath("/files", false);
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
        if (filter != null)
        {
            uri.AppendQuery("filter", filter, true);
        }
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}
