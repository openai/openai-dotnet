using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OpenAI;

internal static class InternalListHelpers
{
    internal delegate Task<ClientResult> AsyncListResponseFunc(string continuationToken, int? pageSize);
    internal delegate ClientResult ListResponseFunc(string continuationToken, int? pageSize);

    internal static AsyncPageableCollection<T> CreateAsyncPageable<T,
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
#endif
        TInternalList>(AsyncListResponseFunc listResponseFunc)
        where TInternalList : IJsonModel<TInternalList>, IInternalListResponse<T>
    {
        async Task<ResultPage<T>> pageFunc(string continuationToken, int? pageSize)
            => GetPageFromProtocol<T,TInternalList>(await listResponseFunc(continuationToken, pageSize).ConfigureAwait(false));
        return PageableResultHelpers.Create((pageSize) => pageFunc(null, pageSize), pageFunc);
    }

    internal static PageableCollection<T> CreatePageable<T,
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
#endif
        TInternalList>(ListResponseFunc listResponseFunc)
        where TInternalList : IJsonModel<TInternalList>, IInternalListResponse<T>
    {
        ResultPage<T> pageFunc(string continuationToken, int? pageSize)
            => GetPageFromProtocol<T, TInternalList>(listResponseFunc(continuationToken, pageSize));
        return PageableResultHelpers.Create((pageSize) => pageFunc(null, pageSize), pageFunc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ResultPage<TInstance> GetPageFromProtocol<TInstance,
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
#endif
        TInternalList>(ClientResult protocolResult)
            where TInternalList : IJsonModel<TInternalList>, IInternalListResponse<TInstance>
    {
        PipelineResponse response = protocolResult.GetRawResponse();
        IInternalListResponse<TInstance> values = ModelReaderWriter.Read<TInternalList>(response.Content);
        return ResultPage<TInstance>.Create(values.Data, values.HasMore ? values.LastId : null, response);
    }
}
