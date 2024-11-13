using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

#nullable enable

namespace OpenAI.VectorStores;

internal class VectorStoreFileCollectionResult : CollectionResult<VectorStoreFileAssociation>
{
    private readonly VectorStoreClient _vectorStoreClient;
    private readonly ClientPipeline _pipeline;
    private readonly RequestOptions? _options;

    // Initial values
    private readonly string _vectorStoreId;
    private readonly int? _limit;
    private readonly string? _order;
    private readonly string? _after;
    private readonly string? _before;
    private readonly string? _filter;

    public VectorStoreFileCollectionResult(VectorStoreClient messageClient,
        ClientPipeline pipeline, RequestOptions? options,
        string vectorStoreId,
        int? limit, string? order, string? after, string? before, string? filter)
    {
        _vectorStoreClient = messageClient;
        _pipeline = pipeline;
        _options = options;

        _vectorStoreId = vectorStoreId;
        _limit = limit;
        _order = order;
        _after = after;
        _before = before;
        _filter = filter;
    }

    public override IEnumerable<ClientResult> GetRawPages()
    {
        ClientResult page = GetFirstPage();
        yield return page;

        while (HasNextPage(page))
        {
            page = GetNextPage(page);
            yield return page;
        }
    }

    protected override IEnumerable<VectorStoreFileAssociation> GetValuesFromPage(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        PipelineResponse response = page.GetRawResponse();
        InternalListVectorStoreFilesResponse list = ModelReaderWriter.Read<InternalListVectorStoreFilesResponse>(response.Content)!;
        return list.Data;
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return VectorStoreFileCollectionPageToken.FromResponse(page, _vectorStoreId, _limit, _order, _before, _filter);
    }

    public ClientResult GetFirstPage()
        => GetFileAssociations(_vectorStoreId, _limit, _order, _after, _before, _filter, _options);

    public ClientResult GetNextPage(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string? lastId = doc.RootElement.GetProperty("last_id"u8).GetString();

        return GetFileAssociations(_vectorStoreId, _limit, _order, lastId, _before, _filter, _options);
    }

    public static bool HasNextPage(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }

    internal virtual ClientResult GetFileAssociations(string vectorStoreId, int? limit, string? order, string? after, string? before, string? filter, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        using PipelineMessage message = _vectorStoreClient.CreateGetVectorStoreFilesRequest(vectorStoreId, limit, order, after, before, filter, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }
}
