using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

internal partial class MessagesPageEnumerator : PageEnumerator<ThreadMessage>
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    private readonly string _threadId;
    private readonly int? _limit;
    private readonly string _order;

    private string _after;

    private readonly string _before;
    private readonly RequestOptions _options;

    public MessagesPageEnumerator(
        ClientPipeline pipeline,
        Uri endpoint,
        string threadId, 
        int? limit, string order, string after, string before,
        RequestOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;

        _threadId = threadId;
        _limit = limit;
        _order = order;
        _after = after;
        _before = before;

        _options = options;
    }

    public override async Task<ClientResult> GetFirstAsync()
        => await GetMessagesAsync(_threadId, _limit, _order, _after, _before, _options).ConfigureAwait(false);

    public override ClientResult GetFirst()
        => GetMessages(_threadId, _limit, _order, _after, _before, _options);

    public override async Task<ClientResult> GetNextAsync(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        _after = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return await GetMessagesAsync(_threadId, _limit, _order, _after, _before, _options).ConfigureAwait(false);
    }

    public override ClientResult GetNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        _after = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return GetMessages(_threadId, _limit, _order, _after, _before, _options);
    }

    public override bool HasNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }

    public override PageResult<ThreadMessage> GetPageFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        InternalListMessagesResponse list = ModelReaderWriter.Read<InternalListMessagesResponse>(response.Content)!;

        MessagesPageToken pageToken = MessagesPageToken.FromOptions(_threadId, _limit, _order, _after, _before);
        MessagesPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<ThreadMessage>.Create(list.Data, pageToken, nextPageToken, response);
    }

    internal virtual async Task<ClientResult> GetMessagesAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        using PipelineMessage message = CreateGetMessagesRequest(threadId, limit, order, after, before, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    internal virtual ClientResult GetMessages(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        using PipelineMessage message = CreateGetMessagesRequest(threadId, limit, order, after, before, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    private PipelineMessage CreateGetMessagesRequest(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/messages", false);
        if (limit != null)
        {
            uri.AppendQuery("limit", limit.Value, true);
        }
        if (order != null)
        {
            uri.AppendQuery("order", order, true);
        }
        if (after != null)
        {
            uri.AppendQuery("after", after, true);
        }
        if (before != null)
        {
            uri.AppendQuery("before", before, true);
        }
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}
