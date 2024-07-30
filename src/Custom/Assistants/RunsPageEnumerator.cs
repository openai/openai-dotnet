using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

internal partial class RunsPageEnumerator : PageEnumerator<ThreadRun>
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    private readonly string _threadId;
    private readonly int? _limit;
    private readonly string _order;

    private string _after;

    private readonly string _before;
    private readonly RequestOptions _options;

    public RunsPageEnumerator(
        ClientPipeline pipeline,
        Uri endpoint,
        string threadId, int? limit, string order, string after, string before,
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
        => await GetRunsAsync(_threadId, _limit, _order, _after, _before, _options).ConfigureAwait(false);

    public override ClientResult GetFirst()
        => GetRuns(_threadId, _limit, _order, _after, _before, _options);

    public override async Task<ClientResult> GetNextAsync(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        _after = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return await GetRunsAsync(_threadId, _limit, _order, _after, _before, _options).ConfigureAwait(false);
    }

    public override ClientResult GetNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        _after = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        return GetRuns(_threadId, _limit, _order, _after, _before, _options);
    }

    public override bool HasNext(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();

        return hasMore;
    }

    public override PageResult<ThreadRun> GetPageFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        InternalListRunsResponse list = ModelReaderWriter.Read<InternalListRunsResponse>(response.Content)!;

        RunsPageToken pageToken = RunsPageToken.FromOptions(_threadId, _limit, _order, _after, _before);
        RunsPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<ThreadRun>.Create(list.Data, pageToken, nextPageToken, response);
    }

    internal async virtual Task<ClientResult> GetRunsAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        using PipelineMessage message = CreateGetRunsRequest(threadId, limit, order, after, before, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    internal virtual ClientResult GetRuns(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        using PipelineMessage message = CreateGetRunsRequest(threadId, limit, order, after, before, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    private PipelineMessage CreateGetRunsRequest(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs", false);
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
