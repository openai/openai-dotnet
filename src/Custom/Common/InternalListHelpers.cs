using System.ClientModel;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OpenAI;

internal static class InternalListHelpers
{
    internal delegate Task<ClientResult> AsyncListResponseFunc(string continuationToken, int? pageSize);
    internal delegate ClientResult ListResponseFunc(string continuationToken, int? pageSize);

    internal static AsyncPageableCollection<T> CreateAsyncPageable<T, U>(AsyncListResponseFunc listResponseFunc)
        where U : IJsonModel<U>, IInternalListResponse<T>
    {
        async Task<ResultPage<T>> pageFunc(string continuationToken, int? pageSize)
            => GetPageFromProtocol<T,U>(await listResponseFunc(continuationToken, pageSize).ConfigureAwait(false));
        return PageableResultHelpers.Create((pageSize) => pageFunc(null, pageSize), pageFunc);
    }

    internal static PageableCollection<T> CreatePageable<T, U>(ListResponseFunc listResponseFunc)
        where U : IJsonModel<U>, IInternalListResponse<T>
    {
        ResultPage<T> pageFunc(string continuationToken, int? pageSize)
            => GetPageFromProtocol<T, U>(listResponseFunc(continuationToken, pageSize));
        return PageableResultHelpers.Create((pageSize) => pageFunc(null, pageSize), pageFunc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ResultPage<TInstance> GetPageFromProtocol<TInstance, UInternalList>(ClientResult protocolResult)
            where UInternalList : IJsonModel<UInternalList>, IInternalListResponse<TInstance>
    {
        PipelineResponse response = protocolResult.GetRawResponse();
        IInternalListResponse<TInstance> values = ModelReaderWriter.Read<UInternalList>(response.Content);
        return ResultPage<TInstance>.Create(values.Data, values.HasMore ? values.LastId : null, response);
    }
}
