using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

#nullable enable

namespace OpenAI.Responses;

internal class ResponseItemCollectionResult : CollectionResult<ResponseItem>
{
    private readonly OpenAIResponseClient _parentClient;
    private readonly RequestOptions? _options;

    // Initial values
    private readonly string _responseId;
    private readonly int? _limit;
    private readonly string? _order;
    private readonly string? _after;
    private readonly string? _before;

    public ResponseItemCollectionResult(
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

    protected override IEnumerable<ResponseItem> GetValuesFromPage(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        PipelineResponse response = page.GetRawResponse();
        InternalResponseItemList list = ModelReaderWriter.Read<InternalResponseItemList>(response.Content)!;
        return list.Data;
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
    {
        Argument.AssertNotNull(page, nameof(page));

        return ResponseItemCollectionPageToken.FromResponse(page, _limit, _order, _before);
    }

    public ClientResult GetFirstPage()
        => GetResponseInputItems(_responseId, _limit, _order, _after, _before, _options);

    public ClientResult GetNextPage(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return GetResponseInputItems(_responseId, _limit, _order, lastId, _before, _options);
    }

    public static bool HasNextPage(ClientResult result)
    {
        Argument.AssertNotNull(result, nameof(result));

        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }

    internal virtual ClientResult GetResponseInputItems(string responseId, int? limit, string? order, string? after, string? before, RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateGetInputItemsRequest(responseId, limit, order, after, before, options);
        return ClientResult.FromResponse(_parentClient.Pipeline.ProcessMessage(message, options));
    }
}
