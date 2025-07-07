using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Responses;

internal class AsyncResponseItemCollectionResult : AsyncCollectionResult<ResponseItem>
{
    private readonly OpenAIResponseClient _parentClient;
    private readonly RequestOptions? _options;

    // Initial values
    private readonly string _responseId;
    private readonly int? _limit;
    private readonly string? _order;
    private readonly string? _after;
    private readonly string? _before;

    public AsyncResponseItemCollectionResult(
        OpenAIResponseClient parentClient,
        string responseId,
        int? limit, string? order, string? after, string? before,
        RequestOptions? options)
    {
        _parentClient = parentClient;
        _responseId = responseId;
        _limit = limit;
        _order = order;
        _after = after;
        _before = before;
        _options = options;
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

    protected override IAsyncEnumerable<ResponseItem> GetValuesFromPageAsync(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        PipelineResponse response = page.GetRawResponse();
        InternalResponseItemList list = ModelReaderWriter.Read<InternalResponseItemList>(response.Content)!;
        return list.Data.ToAsyncEnumerable(_options?.CancellationToken ?? default);
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return ResponseItemCollectionPageToken.FromResponse(page, _limit, _order, _before);
    }

    public async Task<ClientResult> GetFirstPageAsync()
        => await GetResponsesAsync(_responseId, _limit, _order, _after, _before, _options).ConfigureAwait(false);

    public async Task<ClientResult> GetNextPageAsync(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return await GetResponsesAsync(_responseId, _limit, _order, lastId, _before, _options).ConfigureAwait(false);
    }

    public static bool HasNextPage(ClientResult result) => ResponseItemCollectionResult.HasNextPage(result);

    internal virtual async Task<ClientResult> GetResponsesAsync(string responseId, int? limit, string? order, string? after, string? before, RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateGetInputItemsRequest(responseId, limit, order, after, before, options);
        return ClientResult.FromResponse(await _parentClient.Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }
}
