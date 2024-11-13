using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

internal class AsyncVectorStoreCollectionResult : AsyncCollectionResult<VectorStore>
{
    private readonly VectorStoreClient _vectorStoreClient;
    private readonly ClientPipeline _pipeline;
    private readonly RequestOptions? _options;
    private readonly CancellationToken _cancellationToken;

    // Initial values
    private readonly int? _limit;
    private readonly string? _order;
    private readonly string? _after;
    private readonly string? _before;

    public AsyncVectorStoreCollectionResult(VectorStoreClient vectorStoreClient,
        ClientPipeline pipeline, RequestOptions? options,
        int? limit, string? order, string? after, string? before)
    {
        _vectorStoreClient = vectorStoreClient;
        _pipeline = pipeline;
        _options = options;
        _cancellationToken = _options?.CancellationToken ?? default;

        _limit = limit;
        _order = order;
        _after = after;
        _before = before;
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

    protected override IAsyncEnumerable<VectorStore> GetValuesFromPageAsync(ClientResult page)
    {
        PipelineResponse response = page.GetRawResponse();
        InternalListVectorStoresResponse list = ModelReaderWriter.Read<InternalListVectorStoresResponse>(response.Content)!;
        return list.Data.ToAsyncEnumerable(_cancellationToken);
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return VectorStoreCollectionPageToken.FromResponse(page, _limit, _order, _before);
    }

    public async Task<ClientResult> GetFirstPageAsync()
        => await GetVectorStoresAsync(_limit, _order, _after, _before, _options).ConfigureAwait(false);

    public async Task<ClientResult> GetNextPageAsync(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return await GetVectorStoresAsync(_limit, _order, lastId, _before, _options).ConfigureAwait(false);
    }

    public static bool HasNextPage(ClientResult result)
        => VectorStoreCollectionResult.HasNextPage(result);

    internal virtual async Task<ClientResult> GetVectorStoresAsync(int? limit, string? order, string? after, string? before, RequestOptions? options)
    {
        using PipelineMessage message = _vectorStoreClient.CreateGetVectorStoresRequest(limit, order, after, before, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }
}
