using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

internal class AsyncRunCollectionResult : AsyncCollectionResult<ThreadRun>
{
    private readonly InternalAssistantRunClient _runClient;
    private readonly RequestOptions? _options;
    private readonly CancellationToken _cancellationToken;

    // Initial values
    private readonly string _threadId;
    private readonly int? _limit;
    private readonly string? _order;
    private readonly string? _after;
    private readonly string? _before;

    public AsyncRunCollectionResult(InternalAssistantRunClient runClient,
        RequestOptions? options,
        string threadId,
        int? limit, string? order, string? after, string? before)
    {
        _runClient = runClient;
        _options = options;
        _cancellationToken = _options?.CancellationToken ?? default;

        _threadId = threadId;
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

    protected override IAsyncEnumerable<ThreadRun> GetValuesFromPageAsync(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        PipelineResponse response = page.GetRawResponse();
        InternalListRunsResponse list = ModelReaderWriter.Read<InternalListRunsResponse>(response.Content)!;
        return list.Data.ToAsyncEnumerable(_cancellationToken);
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return RunCollectionPageToken.FromResponse(page, _threadId, _limit, _order, _before);
    }

    public async Task<ClientResult> GetFirstPageAsync()
        => await _runClient.GetRunsAsync(_threadId, _limit, _order, _after, _before, _options).ConfigureAwait(false);

    public async Task<ClientResult> GetNextPageAsync(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return await _runClient.GetRunsAsync(_threadId, _limit, _order, lastId, _before, _options).ConfigureAwait(false);
    }

    public static bool HasNextPage(ClientResult result)
        => RunCollectionResult.HasNextPage(result);
}
