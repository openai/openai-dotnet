using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class PageCollectionHelpers
{
    public static AsyncPageCollection<T> Create<T>(BinaryData firstPageToken, Func<BinaryData?, RequestOptions?, Task<ClientPage<T>>> getPageAsync) where T : notnull
        => new FuncAsyncPageCollection<T>(firstPageToken, getPageAsync);

    public static PageCollection<T> Create<T>(BinaryData firstPageToken, Func<BinaryData?, RequestOptions?, ClientPage<T>> getPage) where T : notnull
        => new FuncPageCollection<T>(firstPageToken, getPage);

    private class FuncAsyncPageCollection<T> : AsyncPageCollection<T> where T : notnull
    {
        private readonly BinaryData _firstPageToken;
        private readonly Func<BinaryData?, RequestOptions?, Task<ClientPage<T>>> _getPageAsync;

        public FuncAsyncPageCollection(BinaryData firstPageToken, Func<BinaryData?, RequestOptions?, Task<ClientPage<T>>> getPageAsync)
        {
            _firstPageToken = firstPageToken;
            _getPageAsync = getPageAsync;
        }

        public override BinaryData FirstPageToken => _firstPageToken;

        public override async Task<ClientPage<T>> GetPageAsync(BinaryData pageToken, RequestOptions? options = null)
            => await _getPageAsync(pageToken, options).ConfigureAwait(false);
    }

    private class FuncPageCollection<T> : PageCollection<T> where T : notnull
    {
        private readonly BinaryData _firstPageToken;
        private readonly Func<BinaryData?, RequestOptions?, ClientPage<T>> _getPage;

        public FuncPageCollection(BinaryData firstPageToken, Func<BinaryData?, RequestOptions?, ClientPage<T>> getPage)
        {
            _firstPageToken = firstPageToken;
            _getPage = getPage;
        }

        public override BinaryData FirstPageToken => _firstPageToken;

        public override ClientPage<T> GetPage(BinaryData pageToken, RequestOptions? options = null)
            => _getPage(pageToken, options);
    }
}
