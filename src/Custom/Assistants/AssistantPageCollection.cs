using OpenAI.Assistants;
using System.ClientModel;
using System.ClientModel.Primitives;

#nullable enable

namespace OpenAI;

#pragma warning disable OPENAI001
internal class AssistantPageCollection : PageCollection<Assistant>
{
    private readonly AssistantClient _client;

    // service method constructor
    public AssistantPageCollection(AssistantClient client, int? limit, string? order, string? after, string? before)
    {
        _client = client;
        FirstPageToken = OpenAIPageToken.FromListOptions(limit, order, after, before);
    }

    // rehydration constructor
    public AssistantPageCollection(AssistantClient client, ClientToken firstPageToken)
    {
        _client = client;
        FirstPageToken = firstPageToken;
    }

    public override ClientToken FirstPageToken { get; }

    public override ClientPage<Assistant> GetPage(ClientToken pageToken, RequestOptions? options)
    {
        OpenAIPageToken token = (OpenAIPageToken)pageToken;

        ClientResult result = _client.GetAssistantsPage(
            limit: token.Limit,
            order: token.Order,
            after: token.After,
            before: token.Before,
            options: options);

        PipelineResponse response = result.GetRawResponse();
        InternalListAssistantsResponse list = ModelReaderWriter.Read<InternalListAssistantsResponse>(response.Content)!;

        OpenAIPageToken? nextPageToken = OpenAIPageToken.GetNextPageToken(
            token,
            list.HasMore,
            list.LastId);

        return ClientPage<Assistant>.Create(list.Data, pageToken, nextPageToken, response);
    }
}
#pragma warning restore OPENAI001