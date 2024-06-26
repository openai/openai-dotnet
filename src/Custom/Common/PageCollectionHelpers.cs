using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class PageCollectionHelpers
{
    public static AsyncPageCollection<T> CreateAsync<T>(ClientToken firstPageToken,
        Func<ClientToken, RequestOptions?, Task<ClientPage<T>>> getPageAsync) where T : notnull
        => new AsyncFuncPageCollection<T>(firstPageToken, getPageAsync);

    public static PageCollection<T> Create<T>(ClientToken firstPageToken,
        Func<ClientToken, RequestOptions?, ClientPage<T>> getPage) where T : notnull
        => new FuncPageCollection<T>(firstPageToken, getPage);

    public static IAsyncEnumerable<ClientResult> CreatePrototolAsync(ClientToken firstPageToken,
        Func<ClientToken, Task<ClientResult>> getPageAsync,
        Func<ClientResult, ClientToken?> getNextPageToken)
        => new AsyncFuncResultEnumerable(firstPageToken, getPageAsync, getNextPageToken);

    public static IEnumerable<ClientResult> CreatePrototol(ClientToken firstPageToken,
            Func<ClientToken, ClientResult> getPage,
            Func<ClientResult, ClientToken?> getNextPageToken)
        => new FuncResultEnumerable(firstPageToken, getPage, getNextPageToken);

    private class AsyncFuncPageCollection<T> : AsyncPageCollection<T> where T : notnull
    {
        private readonly ClientToken _firstPageToken;
        private readonly Func<ClientToken, RequestOptions?, Task<ClientPage<T>>> _getPageAsync;

        public AsyncFuncPageCollection(ClientToken firstPageToken, Func<ClientToken, RequestOptions?, Task<ClientPage<T>>> getPageAsync)
        {
            _firstPageToken = firstPageToken;
            _getPageAsync = getPageAsync;
        }

        public override ClientToken FirstPageToken => _firstPageToken;

        public override async Task<ClientPage<T>> GetPageAsync(ClientToken pageToken, RequestOptions? options = null)
            => await _getPageAsync(pageToken, options).ConfigureAwait(false);
    }

    private class FuncPageCollection<T> : PageCollection<T> where T : notnull
    {
        private readonly ClientToken _firstPageToken;
        private readonly Func<ClientToken, RequestOptions?, ClientPage<T>> _getPage;

        public FuncPageCollection(ClientToken firstPageToken, Func<ClientToken, RequestOptions?, ClientPage<T>> getPage)
        {
            _firstPageToken = firstPageToken;
            _getPage = getPage;
        }

        public override ClientToken FirstPageToken => _firstPageToken;

        public override ClientPage<T> GetPage(ClientToken pageToken, RequestOptions? options = null)
            => _getPage(pageToken, options);
    }

    private class AsyncFuncResultEnumerable : IAsyncEnumerable<ClientResult>
    {
        private readonly ClientToken _firstPageToken;
        private readonly Func<ClientToken, Task<ClientResult>> _getPageAsync;
        private readonly Func<ClientResult, ClientToken?> _getNextPageToken;

        public AsyncFuncResultEnumerable(ClientToken firstPageToken,
            Func<ClientToken, Task<ClientResult>> getPageAsync,
            Func<ClientResult, ClientToken?> getNextPageToken)
        {
            _firstPageToken = firstPageToken;
            _getPageAsync = getPageAsync;
            _getNextPageToken = getNextPageToken;
        }

        public async IAsyncEnumerator<ClientResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            ClientToken? pageToken = _firstPageToken;

            do
            {
                ClientResult result = await _getPageAsync(pageToken).ConfigureAwait(false);
                yield return result;
                pageToken = _getNextPageToken(result);
            }
            while (pageToken != null);
        }
    }

    private class FuncResultEnumerable : IEnumerable<ClientResult>
    {
        private readonly ClientToken _firstPageToken;
        private readonly Func<ClientToken, ClientResult> _getPage;
        private readonly Func<ClientResult, ClientToken?> _getNextPageToken;

        public FuncResultEnumerable(ClientToken firstPageToken,
            Func<ClientToken, ClientResult> getPage,
            Func<ClientResult, ClientToken?> getNextPageToken)
        {
            _firstPageToken = firstPageToken;
            _getPage = getPage;
            _getNextPageToken = getNextPageToken;
        }

        public IEnumerator<ClientResult> GetEnumerator()
        {
            ClientToken? pageToken = _firstPageToken;

            do
            {
                ClientResult result = _getPage(pageToken);
                yield return result;
                pageToken = _getNextPageToken(result);
            }
            while (pageToken != null);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
