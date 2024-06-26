using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class OpenAIPagingHelpers
{
    public delegate Task<ClientResult> GetPageValuesAsync(int? limit, string? order, string? after, string? before, RequestOptions? options);
    public delegate ClientResult GetPageValues(int? limit, string? order, string? after, string? before, RequestOptions? options);

    public static AsyncPageCollection<TValue> CreateAsync<TValue, TList>(
        ClientToken firstPageToken,
        GetPageValuesAsync getListValuesAsync)
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
            OpenAIPageToken? nextPageToken = token.GetNextPageToken(list.HasMore, list.LastId);

            return ClientPage<TValue>.Create(list.Data, pageToken, nextPageToken, response);
        }

        return PageCollectionHelpers.CreateAsync(firstPageToken, getPageAsync);
    }

    public static PageCollection<TValue> Create<TValue, TList>(
        ClientToken firstPageToken,
        GetPageValues getListValues)
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
            OpenAIPageToken? nextPageToken = token.GetNextPageToken(list.HasMore, list.LastId);

            return ClientPage<TValue>.Create(list.Data, pageToken, nextPageToken, response);
        }

        return PageCollectionHelpers.Create(firstPageToken, getPage);
    }

    public static IAsyncEnumerable<ClientResult> CreateProtocolAsync(
        int? limit, string? order, string? after, string? before, RequestOptions? options,
        GetPageValuesAsync getPageValuesAsync)
    {
        OpenAIPageToken firstPageToken = OpenAIPageToken.FromListOptions(limit, order, after, before);

        async Task<ClientResult> getPageAsync(ClientToken pageToken)
        {
            OpenAIPageToken token = (OpenAIPageToken)pageToken;
            return await getPageValuesAsync(token.Limit, token.Order, token.After, token.Before, options).ConfigureAwait(false);
        }

        ClientToken? getNextPageToken(ClientResult result)
        {
            PipelineResponse response = result.GetRawResponse();

            using JsonDocument doc = JsonDocument.Parse(response.Content);
            bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();
            after = doc.RootElement.GetProperty("last_id"u8).GetString();

            return OpenAIPageToken.GetNextPageToken(limit, order, after, before, hasMore);
        }

        return PageCollectionHelpers.CreatePrototolAsync(firstPageToken, getPageAsync, getNextPageToken);
    }

    public static IEnumerable<ClientResult> CreateProtocol(
        int? limit, string? order, string? after, string? before, RequestOptions? options,
        GetPageValues getPageValues)
    {
        OpenAIPageToken firstPageToken = OpenAIPageToken.FromListOptions(limit, order, after, before);

        ClientResult getPage(ClientToken pageToken)
        {
            OpenAIPageToken token = (OpenAIPageToken)pageToken;
            return getPageValues(token.Limit, token.Order, token.After, token.Before, options);
        }

        ClientToken? getNextPageToken(ClientResult result)
        {
            PipelineResponse response = result.GetRawResponse();

            using JsonDocument doc = JsonDocument.Parse(response.Content);
            bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();
            after = doc.RootElement.GetProperty("last_id"u8).GetString();

            return OpenAIPageToken.GetNextPageToken(limit, order, after, before, hasMore);
        }

        return PageCollectionHelpers.CreatePrototol(firstPageToken, getPage, getNextPageToken);
    }
}