using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
internal class AsyncFineTuningJobCollectionResult : AsyncCollectionResult<FineTuningJob>
{
    private readonly FineTuningClient _fineTuningClient;
    private readonly ClientPipeline _pipeline;
    private readonly RequestOptions? _options;
    private readonly CancellationToken _cancellationToken;

    // Initial values
    private readonly int? _limit;
    private readonly int? _pageSize;
    private readonly string _after;

    public AsyncFineTuningJobCollectionResult(FineTuningClient fineTuningClient,
        ClientPipeline pipeline, RequestOptions? options,
        int? pageSize, string after)
    {
        _fineTuningClient = fineTuningClient;
        _pipeline = pipeline;
        _options = options;

        _pageSize = pageSize;
        _after = after;
        _cancellationToken = _options?.CancellationToken ?? default;
    }


    public async override IAsyncEnumerable<ClientResult> GetRawPagesAsync()
    {
        ClientResult page = await GetFirstPageAsync().ConfigureAwait(false);

        while (true)
        {
            yield return page;
            if (!HasNextPage(page))
            {
                break;
            }
            page = await GetNextPageAsync(page);
        }
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return FineTuningCollectionPageToken.FromResponse(page, _pageSize);
    }

    public async Task<ClientResult> GetFirstPageAsync()
        => await GetJobsAsync(_after, _pageSize, _options).ConfigureAwait(false);

    public async Task<ClientResult> GetNextPageAsync(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response?.Content);

        JsonElement data = doc.RootElement.GetProperty("data");
        JsonElement lastItem = data.EnumerateArray().LastOrDefault();
        string? lastId = lastItem.TryGetProperty("id", out JsonElement idElement) ?
            idElement.GetString() : null;

        return await GetJobsAsync(lastId, _pageSize, _options).ConfigureAwait(false);
    }

    public static bool HasNextPage(ClientResult result)
        => FineTuningJobCollectionResult.HasNextPage(result);

    internal virtual async Task<ClientResult> GetJobsAsync(string? after, int? limit, RequestOptions? options)
    {
        using PipelineMessage message = _fineTuningClient.GetJobsPipelineMessage(after, limit, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    protected override IAsyncEnumerable<FineTuningJob> GetValuesFromPageAsync(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        PipelineResponse response = page.GetRawResponse();
        return _fineTuningClient.CreateJobsFromPageResponse(response).ToAsyncEnumerable(_cancellationToken);
    }
}
