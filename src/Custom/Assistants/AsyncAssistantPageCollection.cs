using OpenAI.Assistants;
using System.ClientModel;
using System.ClientModel.Primitives;
using System;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

#pragma warning disable OPENAI001
internal class AsyncAssistantPageCollection : AsyncPageCollection<Assistant>
{
    private readonly AssistantClient _client;

    // service method constructor
    public AsyncAssistantPageCollection(AssistantClient client, int? limit, string? order, string? after, string? before)
    {
        _client = client;
        FirstPageToken = OpenAIPageToken.FromListOptions(limit, order, after, before);
    }

    // rehydration constructor
    public AsyncAssistantPageCollection(AssistantClient client, BinaryData firstPageToken)
    {
        _client = client;
        FirstPageToken = firstPageToken;
    }

    public override BinaryData FirstPageToken { get; }

    public override async Task<ClientPage<Assistant>> GetPageAsync(BinaryData pageToken, RequestOptions? options)
    {
        OpenAIPageToken token = OpenAIPageToken.FromBytes(pageToken);

        ClientResult result = await _client.GetAssistantsPageAsync(
            limit: token.Limit,
            order: token.Order,
            after: token.After,
            before: token.Before,
            options: options).ConfigureAwait(false);

        PipelineResponse response = result.GetRawResponse();
        InternalListAssistantsResponse list = ModelReaderWriter.Read<InternalListAssistantsResponse>(response.Content)!;

        BinaryData? nextPageToken = OpenAIPageToken.GetNextPageToken(
            token,
            list.HasMore,
            list.LastId);
        return ClientPage<Assistant>.Create(list.Data, pageToken, nextPageToken, response);
    }
}
#pragma warning restore OPENAI001