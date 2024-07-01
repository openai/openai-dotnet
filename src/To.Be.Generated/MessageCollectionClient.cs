using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

internal class MessageCollectionClient : PageResultEnumerator
{
    private readonly InternalAssistantMessageClient _messageSubClient;

    private readonly string _threadId;
    private readonly int? _limit;
    private readonly string _order;
    private readonly string _after;
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

    public override ClientResult GetFirst()
        => GetMessagesPage(_threadId, _limit, _order, _after, _before, _options);

    public override ClientResult GetNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return GetMessagesPage(_threadId, _limit, _order, lastId, _before, _options);
    }

    public override bool HasNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }

    /// <inheritdoc cref="InternalAssistantMessageClient.GetMessages"/>
    internal virtual ClientResult GetMessagesPage(string threadId, int? limit, string order, string after, string before, RequestOptions options)
        => _messageSubClient.GetMessages(threadId, limit, order, after, before, options);
}
