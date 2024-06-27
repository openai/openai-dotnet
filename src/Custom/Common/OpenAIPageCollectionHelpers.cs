using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Utility;

#nullable enable

namespace OpenAI;

internal class OpenAIPageCollectionHelpers
{
    public delegate Task<ClientResult> GetPageValuesAsync(int? limit, string? order, string? after, string? before, RequestOptions? options);
    public delegate ClientResult GetPageValues(int? limit, string? order, string? after, string? before, RequestOptions? options);

    public static AsyncPageCollection<TValue> CreateAsync<TValue, TList>(
        ContinuationToken firstPageToken,
        GetPageValuesAsync getPageValuesAsync,
        Func<ContinuationToken, OpenAIPageToken> getToken,
        RequestOptions? options)
            where TValue : notnull
            where TList : IJsonModel<TList>, IInternalListResponse<TValue>
    {
        async Task<PageResult<TValue>> getPageAsync(ContinuationToken pageToken)
        {
            OpenAIPageToken token = getToken(pageToken);

            ClientResult result = await getPageValuesAsync(
                limit: token.Limit,
                order: token.Order,
                after: token.After,
                before: token.Before,
                options).ConfigureAwait(false);

            PipelineResponse response = result.GetRawResponse();
            IInternalListResponse<TValue> list = ModelReaderWriter.Read<TList>(response.Content)!;
            OpenAIPageToken? nextPageToken = token.GetNextPageToken(list.HasMore, list.LastId);

            return PageResult<TValue>.Create(list.Data, pageToken, nextPageToken, response);
        }

        return PageCollectionHelpers.CreateAsync(firstPageToken, getPageAsync);
    }

    public static PageCollection<TValue> Create<TValue, TList>(
        ContinuationToken firstPageToken,
        GetPageValues getPageValues,
        Func<ContinuationToken, OpenAIPageToken> getToken,
        RequestOptions? options)
            where TValue : notnull
            where TList : IJsonModel<TList>, IInternalListResponse<TValue>
    {
        PageResult<TValue> getPage(ContinuationToken pageToken)
        {
            OpenAIPageToken token = getToken(pageToken);

            ClientResult result = getPageValues(
                limit: token.Limit,
                order: token.Order,
                after: token.After,
                before: token.Before,
                options);

            PipelineResponse response = result.GetRawResponse();
            IInternalListResponse<TValue> list = ModelReaderWriter.Read<TList>(response.Content)!;
            OpenAIPageToken? nextPageToken = token.GetNextPageToken(list.HasMore, list.LastId);

            return PageResult<TValue>.Create(list.Data, pageToken, nextPageToken, response);
        }

        return PageCollectionHelpers.Create(firstPageToken, getPage);
    }

    public static IAsyncEnumerable<ClientResult> CreateProtocolAsync(
        ContinuationToken firstPageToken,
        GetPageValuesAsync getPageValuesAsync,
        Func<ContinuationToken, OpenAIPageToken> getToken,
        RequestOptions? options)
    {
        async Task<ClientResult> getPageAsync(ContinuationToken pageToken)
        {
            OpenAIPageToken token = getToken(pageToken);
            return await getPageValuesAsync(token.Limit, token.Order, token.After, token.Before, options).ConfigureAwait(false);
        }

        ContinuationToken? getNextPageToken(ContinuationToken pageToken, ClientResult result)
        {
            PipelineResponse response = result.GetRawResponse();

            using JsonDocument doc = JsonDocument.Parse(response.Content);
            bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();
            string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

            OpenAIPageToken token = getToken(pageToken);
            return token.GetNextPageToken(hasMore, lastId);
        }

        return PageCollectionHelpers.CreatePrototolAsync(firstPageToken, getPageAsync, getNextPageToken);
    }

    public static IEnumerable<ClientResult> CreateProtocol(
        ContinuationToken firstPageToken,
        GetPageValues getPageValues,
        Func<ContinuationToken, OpenAIPageToken> getToken,
        RequestOptions? options)
    {
        ClientResult getPage(ContinuationToken pageToken)
        {
            OpenAIPageToken token = (OpenAIPageToken)pageToken;
            return getPageValues(token.Limit, token.Order, token.After, token.Before, options);
        }

        ContinuationToken? getNextPageToken(ContinuationToken pageToken, ClientResult result)
        {
            PipelineResponse response = result.GetRawResponse();

            using JsonDocument doc = JsonDocument.Parse(response.Content);
            bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();
            string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

            OpenAIPageToken token = getToken(pageToken);
            return token.GetNextPageToken(hasMore, lastId);
        }

        return PageCollectionHelpers.CreatePrototol(firstPageToken, getPage, getNextPageToken);
    }
}