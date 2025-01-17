﻿using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

#nullable enable

namespace OpenAI.Assistants;

internal class RunCollectionResult : CollectionResult<ThreadRun>
{
    private readonly InternalAssistantRunClient _runClient;
    private readonly RequestOptions? _options;

    // Initial values
    private readonly string _threadId;
    private readonly int? _limit;
    private readonly string? _order;
    private readonly string? _after;
    private readonly string? _before;

    public RunCollectionResult(InternalAssistantRunClient runClient,
        RequestOptions? options,
        string threadId, int? limit, string? order, string? after, string? before)
    {
        _runClient = runClient;
        _options = options;

        _threadId = threadId;
        _limit = limit;
        _order = order;
        _after = after;
        _before = before;
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

    protected override IEnumerable<ThreadRun> GetValuesFromPage(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        PipelineResponse response = page.GetRawResponse();
        InternalListRunsResponse list = ModelReaderWriter.Read<InternalListRunsResponse>(response.Content)!;
        return list.Data;
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return RunCollectionPageToken.FromResponse(page, _threadId, _limit, _order, _before);
    }

    public ClientResult GetFirstPage()
        => _runClient.ListRuns(_threadId, _limit, _order, _after, _before, _options);

    public ClientResult GetNextPage(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return _runClient.ListRuns(_threadId, _limit, _order, lastId, _before, _options);
    }

    public static bool HasNextPage(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }
}
