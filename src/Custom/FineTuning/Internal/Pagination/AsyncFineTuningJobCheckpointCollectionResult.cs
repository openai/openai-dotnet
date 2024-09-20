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
    private readonly FineTuningClient _fineTuningClient;
    private readonly ClientPipeline _pipeline;
    private readonly RequestOptions? _options;

    // Initial values
    private readonly string _jobId;
    private readonly int? _limit;
    private readonly string _after;

    public AsyncFineTuningJobCheckpointCollectionResult(FineTuningClient fineTuningClient,
        ClientPipeline pipeline, RequestOptions? options,
        string jobId, int? limit, string after)
    {
        _fineTuningClient = fineTuningClient;
        _pipeline = pipeline;
        _options = options;

        _jobId = jobId;
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

        return FineTuningJobEventCollectionPageToken.FromResponse(page, _jobId, _limit); 
    }

    public async Task<ClientResult> GetFirstPageAsync()
        => await GetJobCheckpointsAsync(_jobId, _after, _limit, _options).ConfigureAwait(false);

    public async Task<ClientResult> GetNextPageAsync(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response?.Content);

        JsonElement data = doc.RootElement.GetProperty("data");
        JsonElement lastItem = data.EnumerateArray().LastOrDefault();
        string? lastId = lastItem.TryGetProperty("id", out JsonElement idElement) ?
            idElement.GetString() : null;

        return await GetJobCheckpointsAsync(_jobId, lastId, _limit, _options).ConfigureAwait(false);
    }

    public static bool HasNextPage(ClientResult result)
        => FineTuningJobCheckpointCollectionResult.HasNextPage(result);

    internal virtual async Task<ClientResult> GetJobCheckpointsAsync(string jobId, string? after, int? limit, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        using PipelineMessage message = _fineTuningClient.CreateGetFineTuningJobCheckpointsRequest(jobId, after, limit, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }
}
