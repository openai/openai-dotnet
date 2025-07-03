using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

internal class AsyncVectorStoreFileBatchCollectionResult : AsyncCollectionResult<VectorStoreFileAssociation>
{
    private readonly VectorStoreClient _vectorStoreClient;
    private readonly ClientPipeline _pipeline;
    private readonly RequestOptions? _options;
    private readonly CancellationToken _cancellationToken;

    // Initial values
    private readonly string _vectorStoreId;
    private readonly string _batchId;
    private readonly int? _limit;
    private readonly string? _order;
    private readonly string? _after;
    private readonly string? _before;
    private readonly string? _filter;

    public AsyncVectorStoreFileBatchCollectionResult(VectorStoreClient messageClient,
        ClientPipeline pipeline, RequestOptions options,
        string vectorStoreId, string batchId,
        int? limit, string? order, string? after, string? before, string? filter)
    {
        _vectorStoreClient = messageClient;
        _pipeline = pipeline;
        _options = options;
        _cancellationToken = _options?.CancellationToken ?? default;

        _vectorStoreId = vectorStoreId;
        _batchId = batchId;
        _limit = limit;
        _order = order;
        _after = after;
        _before = before;
        _filter = filter;
    }

    public async override IAsyncEnumerable<ClientResult> GetRawPagesAsync()
    {
        ClientResult page = await GetFirstPageAsync().ConfigureAwait(false);
        yield return page;

        while (HasNextPage(page))
        {
            page = await GetNextPageAsync(page);
            yield return page;
        }
    }

    protected override IAsyncEnumerable<VectorStoreFileAssociation> GetValuesFromPageAsync(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        PipelineResponse response = page.GetRawResponse();
        InternalListVectorStoreFilesResponse list = ModelReaderWriter.Read<InternalListVectorStoreFilesResponse>(response.Content)!;
        return list.Data.ToAsyncEnumerable(_cancellationToken);
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return VectorStoreFileBatchCollectionPageToken.FromResponse(page, _vectorStoreId, _batchId, _limit, _order, _before, _filter);
    }

    public async Task<ClientResult> GetFirstPageAsync()
        => await GetFileAssociationsAsync(_vectorStoreId, _batchId, _limit, _order, _after, _before, _filter, _options).ConfigureAwait(false);

    public async Task<ClientResult> GetNextPageAsync(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string? lastId = doc.RootElement.GetProperty("last_id"u8).GetString();

        return await GetFileAssociationsAsync(_vectorStoreId, _batchId, _limit, _order, lastId, _before, _filter, _options).ConfigureAwait(false);
    }

    public static bool HasNextPage(ClientResult result)
        => VectorStoreFileBatchCollectionResult.HasNextPage(result);

    internal virtual async Task<ClientResult> GetFileAssociationsAsync(string vectorStoreId, string batchId, int? limit, string? order, string? after, string? before, string? filter, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = _vectorStoreClient.CreateGetFilesInVectorStoreBatchRequest(vectorStoreId, batchId, limit, order, after, before, filter, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }
}
