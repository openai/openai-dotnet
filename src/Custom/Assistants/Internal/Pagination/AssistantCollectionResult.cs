using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

#nullable enable

namespace OpenAI.Assistants;

internal class AssistantCollectionResult : CollectionResult<Assistant>
{
    private readonly AssistantClient _assistantClient;
    private readonly ClientPipeline _pipeline;
    private readonly RequestOptions? _options;

    // Initial values
    private readonly int? _limit;
    private readonly string? _order;
    private readonly string? _after;
    private readonly string? _before;

    public AssistantCollectionResult(AssistantClient assistantClient,
        ClientPipeline pipeline, RequestOptions? options,
        int? limit, string? order, string? after, string? before)
    {
        _assistantClient = assistantClient;
        _pipeline = pipeline;
        _options = options;

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

    protected override IEnumerable<Assistant> GetValuesFromPage(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        PipelineResponse response = page.GetRawResponse();
        InternalListAssistantsResponse list = ModelReaderWriter.Read<InternalListAssistantsResponse>(response.Content)!;
        return list.Data;
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return AssistantCollectionPageToken.FromResponse(page, _limit, _order, _before);
    }

    public ClientResult GetFirstPage()
        => GetAssistants(_limit, _order, _after, _before, _options);

    public ClientResult GetNextPage(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return GetAssistants(_limit, _order, lastId, _before, _options);
    }

    public static bool HasNextPage(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }

    internal virtual ClientResult GetAssistants(int? limit, string? order, string? after, string? before, RequestOptions? options)
    {
        using PipelineMessage message = _assistantClient.CreateGetAssistantsRequest(limit, order, after, before, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }
}
