using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

#nullable enable

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
internal class FineTuningCheckpointCollectionResult : CollectionResult<FineTuningCheckpoint>
{
    private readonly FineTuningJob _job;
    private readonly RequestOptions? _options;

    // Initial values
    private readonly int? _limit;
    private readonly string? _after;

    public FineTuningCheckpointCollectionResult(
        FineTuningJob job,
        RequestOptions? options,
        int? limit, string? after)
    {
        _job = job;
        _options = options;

        _limit = limit;
        _after = after;
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

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return FineTuningCheckpointCollectionPageToken.FromResponse(page, _job.JobId, _limit);
    }

    public ClientResult GetFirstPage()
        => _job.GetCheckpointsPage(_after, _limit, _options);

    public ClientResult GetNextPage(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response?.Content);

        JsonElement data = doc.RootElement.GetProperty("data");
        JsonElement lastItem = data.EnumerateArray().LastOrDefault();
        string? lastId = lastItem.TryGetProperty("id", out JsonElement idElement) ?
            idElement.GetString() : null;

        return _job.GetCheckpointsPage(lastId, _limit, _options);
    }

    public static bool HasNextPage(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }

    protected override IEnumerable<FineTuningCheckpoint> GetValuesFromPage(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        PipelineResponse response = page.GetRawResponse();
        InternalListFineTuningJobCheckpointsResponse points = ModelReaderWriter.Read<InternalListFineTuningJobCheckpointsResponse>(response.Content)!;
        return points.Data;
    }
}
