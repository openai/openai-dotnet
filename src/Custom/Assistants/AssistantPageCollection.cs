using OpenAI.Assistants;
using System.ClientModel;
using System.ClientModel.Primitives;
using System;

#nullable enable

namespace OpenAI;

#pragma warning disable OPENAI001
internal class AssistantPageCollection : PageCollection<Assistant>
{
    private readonly AssistantClient _client;

    private readonly int? _limit;
    private readonly string? _order;
    private readonly string? _after;
    private readonly string? _before;

    // service method constructor
    public AssistantPageCollection(AssistantClient client, int? limit, string? order, string? after, string? before)
    {
        _client = client;

        _limit = limit;
        _order = order;
        _after = after;
        _before = before;

        FirstPageToken = BinaryData.FromString(after ?? string.Empty);
    }

    public override BinaryData FirstPageToken { get; }

    public override ClientPage<Assistant> GetPage(BinaryData pageToken, RequestOptions? options)
    {
        OpenAIPageToken token = OpenAIPageToken.FromBytes(pageToken);

        ClientResult result = _client.GetAssistantsPage(
            limit: _limit,
            order: _order,
            after: token.After,
            before: _before,
            options: options);

        PipelineResponse response = result.GetRawResponse();
        InternalListAssistantsResponse list = ModelReaderWriter.Read<InternalListAssistantsResponse>(response.Content)!;

        BinaryData? nextPageToken = OpenAIPageToken.GetNextPageToken(list.HasMore, list.LastId);
        return ClientPage<Assistant>.Create(list.Data, pageToken, nextPageToken, response);
    }

}
#pragma warning restore OPENAI001