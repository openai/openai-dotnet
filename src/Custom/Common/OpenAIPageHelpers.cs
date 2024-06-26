using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class OpenAIPageHelpers
{
    internal delegate Task<ClientResult> GetListValuesAsync(int? limit, string? order, string? after, string? before, RequestOptions? options);
    internal delegate ClientResult GetListValues(int? limit, string? order, string? after, string? before, RequestOptions? options);

    internal static AsyncPageCollection<TValue> CreateAsync<TValue, TList>(
        ClientToken firstPageToken,
        GetListValuesAsync getListValuesAsync) 
            where TValue : notnull
            where TList : IJsonModel<TList>, IInternalListResponse<TValue>
    {
        async Task<ClientPage<TValue>> getPageAsync(ClientToken pageToken, RequestOptions? options)
        {
            OpenAIPageToken token = (OpenAIPageToken)pageToken;

            ClientResult result = await getListValuesAsync(
                limit: token.Limit,
                order: token.Order,
                after: token.After,
                before: token.Before,
                options).ConfigureAwait(false);

            PipelineResponse response = result.GetRawResponse();
            IInternalListResponse<TValue> list = ModelReaderWriter.Read<TList>(response.Content)!;

            OpenAIPageToken? nextPageToken = OpenAIPageToken.GetNextPageToken(
                token,
                list.HasMore,
                list.LastId);

            return ClientPage<TValue>.Create(list.Data, pageToken, nextPageToken, response);
        }

        return PageCollectionHelpers.Create(firstPageToken, getPageAsync);
    }

    internal static PageCollection<TValue> Create<TValue, TList>(
        ClientToken firstPageToken, 
        GetListValues getListValues)
            where TValue : notnull
            where TList : IJsonModel<TList>, IInternalListResponse<TValue>
    {
        ClientPage<TValue> getPage(ClientToken pageToken, RequestOptions? options)
        {
            OpenAIPageToken token = (OpenAIPageToken)pageToken;

            ClientResult result = getListValues(
                limit: token.Limit,
                order: token.Order,
                after: token.After,
                before: token.Before,
                options);

            PipelineResponse response = result.GetRawResponse();
            IInternalListResponse<TValue> list = ModelReaderWriter.Read<TList>(response.Content)!;

            OpenAIPageToken? nextPageToken = OpenAIPageToken.GetNextPageToken(
                token,
                list.HasMore,
                list.LastId);

            return ClientPage<TValue>.Create(list.Data, pageToken, nextPageToken, response);
        }

        return PageCollectionHelpers.Create(firstPageToken, getPage);
    }
}