using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class OpenAIPageCollectionHelpers
{
    public static AsyncPageCollection<TValue> Create<TValue, TList>(int? limit, string? order, string? after, string? before)
        where TList : IJsonModel<TValue>, IInternalListResponse<TValue>
    {
        BinaryData firstPageToken = OpenAIPageToken.FromListOptions(limit, order, after, before);
        PageCollectionHelpers.Create()
    }

    public static PageCollection<T> Create<T>(BinaryData firstPageToken, Func<BinaryData?, RequestOptions?, ClientPage<T>> getPage) where T : notnull
        => new FuncPageable<T>(firstPageToken, getPage);
}