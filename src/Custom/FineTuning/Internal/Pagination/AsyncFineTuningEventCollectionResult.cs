using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
internal class AsyncFineTuningEventCollectionResult : AsyncCollectionResult<FineTuningEvent>
{
    private readonly FineTuningJob _job;
    private readonly RequestOptions? _options;
    private readonly CancellationToken _cancellationToken;

    // Initial values
    private readonly int? _limit;
    private readonly string? _after;

    public AsyncFineTuningEventCollectionResult(
        FineTuningJob job,
        RequestOptions? options,
        int? limit, string? after)
    {
        _job = job;
        _options = options;

        _limit = limit;
        _after = after;
        _cancellationToken = _options?.CancellationToken ?? default;
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

        return FineTuningEventCollectionPageToken.FromResponse(page, _job.JobId, _limit);
    }

    public async Task<ClientResult> GetFirstPageAsync()
        => await _job.GetEventsPageAsync(_after, _limit, _options).ConfigureAwait(false);

    public async Task<ClientResult> GetNextPageAsync(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response?.Content);

        JsonElement data = doc.RootElement.GetProperty("data");
        JsonElement lastItem = data.EnumerateArray().LastOrDefault();
        string? lastId = lastItem.TryGetProperty("id", out JsonElement idElement) ?
            idElement.GetString() : null;

        return await _job.GetEventsPageAsync(lastId, _limit, _options).ConfigureAwait(false);
    }

    public static bool HasNextPage(ClientResult result)
        => FineTuningEventCollectionResult.HasNextPage(result);

    protected override IAsyncEnumerable<FineTuningEvent> GetValuesFromPageAsync(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        PipelineResponse response = page.GetRawResponse();
        InternalListFineTuningJobEventsResponse list = ModelReaderWriter.Read<InternalListFineTuningJobEventsResponse>(response.Content)!;
        return list.Data.ToAsyncEnumerable(_cancellationToken);
    }
}
