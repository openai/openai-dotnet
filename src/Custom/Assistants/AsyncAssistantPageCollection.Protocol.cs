using OpenAI.Assistants;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

#nullable enable

namespace OpenAI;

#pragma warning disable OPENAI001
internal class AsyncProtocolAssistantPageCollection : IAsyncEnumerable<ClientResult>
{
    private readonly AssistantClient _client;

    private readonly int? _limit;
    private readonly string? _order;
    private readonly string? _after;
    private readonly string? _before;

    private readonly RequestOptions? _options;

    public AsyncProtocolAssistantPageCollection(AssistantClient client, int? limit, string order, string after, string before, RequestOptions options)
    {
        _client = client;

        _limit = limit;
        _order = order;
        _after = after;
        _before = before;

        _options = options;
    }

    public async IAsyncEnumerator<ClientResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        string? after = _after;
        bool hasMore = false;

        do
        {
            ClientResult result = await _client.GetAssistantsPageAsync(
                limit: _limit,
                order: _order,
                after: after,
                before: _before,
                options: _options).ConfigureAwait(false);

            yield return result;

            PipelineResponse response = result.GetRawResponse();
            using JsonDocument doc = JsonDocument.Parse(response.Content);
            hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();
            after = doc.RootElement.GetProperty("last_id"u8).GetString();

        } while (hasMore);
    }
}
#pragma warning restore OPENAI001