using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.FineTuning;

internal class AsyncFineTuningJobCheckpointCollectionResult : AsyncCollectionResult
{
    private readonly FineTuningJobOperation _operation;
    private readonly RequestOptions? _options;

    // Initial values
    private readonly int? _limit;
    private readonly string? _after;

    public AsyncFineTuningJobCheckpointCollectionResult(
        FineTuningJobOperation fineTuningJobOperation,
        RequestOptions? options,
        int? limit, string? after)
    {
        _operation = fineTuningJobOperation;
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

        return FineTuningJobEventCollectionPageToken.FromResponse(page, _operation.JobId, _limit);
    }

    public async Task<ClientResult> GetFirstPageAsync()
        => await _operation.GetJobCheckpointsPageAsync(_after, _limit, _options).ConfigureAwait(false);

    public async Task<ClientResult> GetNextPageAsync(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response?.Content);

        JsonElement data = doc.RootElement.GetProperty("data");
        JsonElement lastItem = data.EnumerateArray().LastOrDefault();
        string? lastId = lastItem.TryGetProperty("id", out JsonElement idElement) ?
            idElement.GetString() : null;

        return await _operation.GetJobCheckpointsPageAsync(lastId, _limit, _options).ConfigureAwait(false);
    }

    public static bool HasNextPage(ClientResult result)
        => FineTuningJobCheckpointCollectionResult.HasNextPage(result);
}
