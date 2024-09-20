using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Batch;

internal class AsyncBatchCollectionResult : AsyncCollectionResult
{
    private readonly BatchClient _batchClient;
    private readonly ClientPipeline _pipeline;
    private readonly RequestOptions? _options;

    // Initial values
    private readonly int? _limit;
    private readonly string _after;

    public AsyncBatchCollectionResult(BatchClient batchClient,
        ClientPipeline pipeline, RequestOptions? options,
        int? limit, string after)
    {
        _batchClient = batchClient;
        _pipeline = pipeline;
        _options = options;

        _limit = limit;
        _after = after;
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

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return BatchCollectionPageToken.FromResponse(page, _limit);
    }

    public async Task<ClientResult> GetFirstPageAsync()
        => await GetBatchesAsync(_after, _limit, _options).ConfigureAwait(false);

    public async Task<ClientResult> GetNextPageAsync(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return await GetBatchesAsync(lastId, _limit, _options).ConfigureAwait(false);
    }

    public static bool HasNextPage(ClientResult result)
        => BatchCollectionResult.HasNextPage(result);

    internal virtual async Task<ClientResult> GetBatchesAsync(string? after, int? limit, RequestOptions? options)
    {
        using PipelineMessage message = _batchClient.CreateGetBatchesRequest(after, limit, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }
}
