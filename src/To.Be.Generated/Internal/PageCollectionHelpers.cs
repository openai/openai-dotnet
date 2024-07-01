using System;
using System.ClientModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Utility;

internal class PageCollectionHelpers
{
    public static PageCollection<T> Create<T>(IEnumerator<PageResult<T>> enumerator)
        => new PageCollectionClient<T>(enumerator);

    public static IEnumerable<ClientResult> CreateProtocol(IEnumerator<ClientResult> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    private class PageCollectionClient<T> : PageCollection<T>
    {
        private readonly IEnumerator<PageResult<T>> _enumerator;

        public PageCollectionClient(IEnumerator<PageResult<T>> enumerator)
        {
            _enumerator = enumerator;
        }

        public override IEnumerator<PageResult<T>> GetEnumerator()
        {
            while (_enumerator.MoveNext())
            {
                yield return _enumerator.Current;
            }
        }
    }

    //public static AsyncPageCollection<T> CreateAsync<T>(
    //    ContinuationToken firstPageToken,
    //    Func<ContinuationToken, Task<PageResult<T>>> getPageAsync) where T : notnull
    //    => new AsyncFuncPageCollection<T>(firstPageToken, getPageAsync);

    //public static PageCollection<T> Create<T>(
    //    ContinuationToken firstPageToken,
    //    Func<ContinuationToken, PageResult<T>> getPage) where T : notnull
    //    => new FuncPageCollection<T>(firstPageToken, getPage);

    //public static IAsyncEnumerable<ClientResult> CreatePrototolAsync(
    //    ContinuationToken firstPageToken,
    //    Func<ContinuationToken, Task<ClientResult>> getPageAsync,
    //    Func<ContinuationToken, ClientResult, ContinuationToken?> getNextPageToken)
    //    => new AsyncFuncResultEnumerable(firstPageToken, getPageAsync, getNextPageToken);

    //public static IEnumerable<ClientResult> CreatePrototol(ContinuationToken firstPageToken,
    //        Func<ContinuationToken, ClientResult> getPage,
    //        Func<ContinuationToken, ClientResult, ContinuationToken?> getNextPageToken)
    //    => new FuncResultEnumerable(firstPageToken, getPage, getNextPageToken);

    //private class AsyncFuncPageCollection<T> : AsyncPageCollection<T> where T : notnull
    //{
    //    private readonly Func<ContinuationToken, Task<PageResult<T>>> _getPageAsync;

    //    private ContinuationToken _currentPageToken;

    //    public AsyncFuncPageCollection(
    //        ContinuationToken firstPageToken,
    //        Func<ContinuationToken, Task<PageResult<T>>> getPageAsync)
    //    {
    //        _currentPageToken = firstPageToken;
    //        _getPageAsync = getPageAsync;
    //    }

    //    protected override ContinuationToken CurrentPageToken
    //    {
    //        get => _currentPageToken;
    //        set => _currentPageToken = value;
    //    }

    //    public override async Task<PageResult<T>> GetPageAsyncCore(ContinuationToken pageToken)
    //        => await _getPageAsync(pageToken).ConfigureAwait(false);
    //}

    //private class FuncPageCollection<T> : PageCollection<T> where T : notnull
    //{
    //    private readonly Func<ContinuationToken, PageResult<T>> _getPage;

    //    private ContinuationToken _currentPageToken;

    //    public FuncPageCollection(
    //        ContinuationToken firstPageToken,
    //        Func<ContinuationToken, PageResult<T>> getPage)
    //    {
    //        _currentPageToken = firstPageToken;
    //        _getPage = getPage;
    //    }

    //    protected override ContinuationToken CurrentPageToken
    //    {
    //        get => _currentPageToken;
    //        set => _currentPageToken = value;
    //    }

    //    protected override PageResult<T> GetPageCore(ContinuationToken pageToken)
    //        => _getPage(pageToken);
    //}

    //private class AsyncFuncResultEnumerable : IAsyncEnumerable<ClientResult>
    //{
    //    private readonly ContinuationToken _firstPageToken;
    //    private readonly Func<ContinuationToken, Task<ClientResult>> _getPageAsync;
    //    private readonly Func<ContinuationToken, ClientResult, ContinuationToken?> _getNextPageToken;

    //    public AsyncFuncResultEnumerable(ContinuationToken firstPageToken,
    //        Func<ContinuationToken, Task<ClientResult>> getPageAsync,
    //        Func<ContinuationToken, ClientResult, ContinuationToken?> getNextPageToken)
    //    {
    //        _firstPageToken = firstPageToken;
    //        _getPageAsync = getPageAsync;
    //        _getNextPageToken = getNextPageToken;
    //    }

    //    public async IAsyncEnumerator<ClientResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    //    {
    //        ContinuationToken? pageToken = _firstPageToken;

    //        do
    //        {
    //            ClientResult result = await _getPageAsync(pageToken).ConfigureAwait(false);
    //            yield return result;
    //            pageToken = _getNextPageToken(pageToken, result);
    //        }
    //        while (pageToken != null);
    //    }
    //}

    //private class FuncResultEnumerable : IEnumerable<ClientResult>
    //{
    //    private readonly ContinuationToken _firstPageToken;
    //    private readonly Func<ContinuationToken, ClientResult> _getPage;
    //    private readonly Func<ContinuationToken, ClientResult, ContinuationToken?> _getNextPageToken;

    //    public FuncResultEnumerable(ContinuationToken firstPageToken,
    //        Func<ContinuationToken, ClientResult> getPage,
    //        Func<ContinuationToken, ClientResult, ContinuationToken?> getNextPageToken)
    //    {
    //        _firstPageToken = firstPageToken;
    //        _getPage = getPage;
    //        _getNextPageToken = getNextPageToken;
    //    }

    //    public IEnumerator<ClientResult> GetEnumerator()
    //    {
    //        ContinuationToken? pageToken = _firstPageToken;

    //        do
    //        {
    //            ClientResult result = _getPage(pageToken);
    //            yield return result;
    //            pageToken = _getNextPageToken(pageToken, result);
    //        }
    //        while (pageToken != null);
    //    }

    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //}
}
