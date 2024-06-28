using OpenAI.Utility;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class OpenAIPaginationHelpers
{
    public delegate Task<PageResult> GetPageValuesAsync(int? limit, string? order, string? after, string? before, RequestOptions? options);
    public delegate PageResult GetPageValues(int? limit, string? order, string? after, string? before, RequestOptions? options);

    public static PageResult CreatePageResult(ClientResult result,
        int? limit, string order, string after, string before, RequestOptions options,
        GetPageValuesAsync getPageValuesAsync,
        GetPageValues getPageValues)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        bool hasMore = doc.RootElement.GetProperty("has_more"u8).GetBoolean();
        string lastId = doc.RootElement.GetProperty("last_id"u8).GetString()!;

        async Task<PageResult> GetNextAsync()
        {
            after = lastId;
            ClientResult nextResult = await getPageValuesAsync(limit, order, after, before, options).ConfigureAwait(false);
            return CreatePageResult(nextResult, limit, order, after, before, options, getPageValuesAsync, getPageValues);
        }

        PageResult GetNext()
        {
            ClientResult nextResult = getPageValues(limit, order, lastId, before, options);
            return CreatePageResult(nextResult, limit, order, after, before, options, getPageValuesAsync, getPageValues);
        }

        return PaginationHelpers.CreatePageResult(GetNextAsync, GetNext, hasMore, result.GetRawResponse());
    }
}