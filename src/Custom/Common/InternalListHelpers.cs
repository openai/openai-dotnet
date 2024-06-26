using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal static class InternalListHelpers
{
    //internal delegate Task<ClientResult> AsyncListResponseFunc(string continuationToken, int? pageSize);

    //internal static AsyncPageCollection<T> CreateAsyncPageable<T, U>(AsyncListResponseFunc listResponseFunc)
    //    where U : IJsonModel<U>, IInternalListResponse<T>
    //{
    //    async Task<ClientPage<T>> pageFunc(string continuationToken, int? pageSize)
    //        => GetPageFromProtocol<T, U>(await listResponseFunc(continuationToken, pageSize).ConfigureAwait(false));
    //    return PageableResultHelpers.Create((pageSize) => pageFunc(null, pageSize), pageFunc);
    //}

    //internal delegate ClientResult ListResponseFunc(string continuationToken, int? pageSize);

    internal delegate ClientResult GetListValues(int? limit, string? order, string? after, string? before, RequestOptions? options);

    internal static PageCollection<TValue> CreatePageable<TValue, TList>(GetListValues getListValues)
        where TList : IJsonModel<TList>, IInternalListResponse<TValue>
    {
        ClientPage<TValue> getPage(BinaryData pageToken, RequestOptions? options)
        {
            OpenAIPageToken token = OpenAIPageToken.FromBytes(pageToken);

            ClientResult result = getListValues(
                limit: token.Limit,
                order: token.Order, 
                after: token.After,
                before: token.Before,
                options);

            PipelineResponse response = result.GetRawResponse();
            IInternalListResponse<TValue> list = ModelReaderWriter.Read<TList>(response.Content)!;

            BinaryData? nextPageToken = OpenAIPageToken.GetNextPageToken(
                token,
                list.HasMore,
                list.LastId);

            return ClientPage<TValue>.Create(list.Data, pageToken, nextPageToken, response);
        }

        return PageCollectionHelpers.Create();
    }
}
