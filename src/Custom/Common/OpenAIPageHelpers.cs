using OpenAI.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class OpenAIPageHelpers
{
    public delegate Task<PageResult> GetPageResultAsync(int? limit, string? order, string? after, string? before, RequestOptions? options);
    public delegate PageResult GetPageResult(int? limit, string? order, string? after, string? before, RequestOptions? options);

    // Convenience method version
    public static PageResult<TValue> CreatePage<TValue, TList>(PageResult pageResult)
            where TValue : notnull
            where TList : IJsonModel<TList>, IInternalListResponse<TValue>
    {
        PipelineResponse response = pageResult.GetRawResponse();
        IInternalListResponse<TValue> list = ModelReaderWriter.Read<TList>(response.Content)!;

        return PageHelpers.CreatePage(list.Data, pageResult, CreatePage<TValue, TList>);
    }

    // Protocol method version
    public static PageResult CreatePageResult(
        OpenAIPageToken pageToken,
        RequestOptions options,
        ClientResult result,
        GetPageResultAsync getPageValuesAsync,
        GetPageResult getPageValues)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        OpenAIPageToken? nextPageToken = pageToken.GetNextPageToken(hasMore, lastId);

        async Task<PageResult> GetNextAsync()
        {
            if (nextPageToken is null)
            {
                throw new InvalidOperationException("Cannot get next page result when NextPageToken is null.");
            }

            ClientResult nextResult = await getPageValuesAsync(nextPageToken.Limit, nextPageToken.Order, nextPageToken.After, nextPageToken.Before, options).ConfigureAwait(false);
            return CreatePageResult(nextPageToken, options, nextResult, getPageValuesAsync, getPageValues);
        }

        PageResult GetNext()
        {
            if (nextPageToken is null)
            {
                throw new InvalidOperationException("Cannot get next page result when NextPageToken is null.");
            }

            ClientResult nextResult = getPageValues(nextPageToken.Limit, nextPageToken.Order, nextPageToken.After, nextPageToken.Before, options);
            return CreatePageResult(nextPageToken, options, nextResult, getPageValuesAsync, getPageValues);
        }

        return PageHelpers.CreatePageResult(
            pageToken, nextPageToken, result.GetRawResponse(),
            GetNextAsync, GetNext);
    }
}