using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

internal class MessageCollectionClient : PageResultEnumerator
{
    private readonly InternalAssistantMessageClient _messageSubClient;

    private readonly string _threadId;
    private readonly int? _limit;
    private readonly string _order;

    // Note: this one is special
    private string _after;

    private readonly string _before;
    private readonly RequestOptions _options;

    public MessageCollectionClient(InternalAssistantMessageClient subclient, string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        _threadId = threadId;
        _limit = limit;
        _order = order;
        _after = after;
        _before = before;
        _options = options;

        _messageSubClient = subclient;
    }

    // TODO: do we need these in so many places?
    public string ThreadId => _threadId;

    public int? Limit => _limit;

    public string? Order => _order;

    public string? After => _after;

    public string? Before => _before;

    public override async Task<ClientResult> GetFirstAsync()
        => await GetMessagesPageAsync(_threadId, _limit, _order, _after, _before, _options).ConfigureAwait(false);

    public override ClientResult GetFirst()
        => GetMessagesPage(_threadId, _limit, _order, _after, _before, _options);

    public override async Task<ClientResult> GetNextAsync(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        _after = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return await GetMessagesPageAsync(_threadId, _limit, _order, _after, _before, _options).ConfigureAwait(false);
    }

    public override ClientResult GetNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        _after = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return GetMessagesPage(_threadId, _limit, _order, _after, _before, _options);
    }

    public override bool HasNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }

    // Note: these are the protocol methods - they are generated here
    internal virtual async Task<ClientResult> GetMessagesPageAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    => await _messageSubClient.GetMessagesAsync(threadId, limit, order, after, before, options).ConfigureAwait(false);

    internal virtual ClientResult GetMessagesPage(string threadId, int? limit, string order, string after, string before, RequestOptions options)
        => _messageSubClient.GetMessages(threadId, limit, order, after, before, options);

    // Note: this is the static page deserialization method
    public static PageResult<ThreadMessage> GetPageFromResult(
        MessageCollectionClient resultEnumerator,
        ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();
        InternalListMessagesResponse list = ModelReaderWriter.Read<InternalListMessagesResponse>(response.Content)!;

        MessageCollectionPageToken pageToken = MessageCollectionPageToken.FromOptions(
            resultEnumerator.ThreadId,
            resultEnumerator.Limit,
            resultEnumerator.Order,
            resultEnumerator.After,
            resultEnumerator.Before);

        MessageCollectionPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<ThreadMessage>.Create(list.Data, pageToken, nextPageToken, response);
    }
}
