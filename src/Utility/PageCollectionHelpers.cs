//using System;
//using System.ClientModel;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//#nullable enable

//namespace OpenAI.Utility;

//internal class PageCollectionHelpers
//{
//    public static AsyncCollectionResult<T> CreateAsync<T>(ContinuationToken firstPageToken,
//        Func<ContinuationToken, Task<PageResult<T>>> getPageAsync) where T : notnull
//        => new AsyncFuncCollectionResult<T>(firstPageToken, getPageAsync);

//    public static CollectionResult<T> Create<T>(ContinuationToken firstPageToken,
//        Func<ContinuationToken, PageResult<T>> getPage) where T : notnull
//        => new FuncCollectionResult<T>(firstPageToken, getPage);

//    public static IAsyncEnumerable<ClientResult> CreatePrototolAsync(ContinuationToken firstPageToken,
//        Func<ContinuationToken, Task<ClientResult>> getPageAsync,
//        Func<ContinuationToken, ClientResult, ContinuationToken?> getNextPageToken)
//        => new AsyncFuncResultEnumerable(firstPageToken, getPageAsync, getNextPageToken);

//    public static IEnumerable<ClientResult> CreatePrototol(ContinuationToken firstPageToken,
//            Func<ContinuationToken, ClientResult> getPage,
//            Func<ContinuationToken, ClientResult, ContinuationToken?> getNextPageToken)
//        => new FuncResultEnumerable(firstPageToken, getPage, getNextPageToken);

//    private class AsyncFuncCollectionResult<T> : AsyncCollectionResult<T> where T : notnull
//    {
//        private readonly ContinuationToken _firstPageToken;
//        private readonly Func<ContinuationToken,  Task<PageResult<T>>> _getPageAsync;

//        public AsyncFuncPageCollection(ContinuationToken firstPageToken,
//            Func<ContinuationToken, Task<PageResult<T>>> getPageAsync)
//        {
//            _firstPageToken = firstPageToken;
//            _getPageAsync = getPageAsync;
//        }

//        public override ContinuationToken FirstPageToken => _firstPageToken;

//        public override async Task<PageResult<T>> GetPageAsyncCore(ContinuationToken pageToken)
//            => await _getPageAsync(pageToken).ConfigureAwait(false);
//    }

//    private class FuncCollectionResult<T> : CollectionResult<T> where T : notnull
//    {
//        private readonly ContinuationToken _firstPageToken;
//        private readonly Func<ContinuationToken,  PageResult<T>> _getPage;

//        public FuncPageCollection(ContinuationToken firstPageToken,
//            Func<ContinuationToken, PageResult<T>> getPage)
//        {
//            _firstPageToken = firstPageToken;
//            _getPage = getPage;
//        }

//        public override ContinuationToken FirstPageToken => _firstPageToken;

//        public override PageResult<T> GetPageCore(ContinuationToken pageToken)
//            => _getPage(pageToken);
//    }

//    private class AsyncFuncResultEnumerable : IAsyncEnumerable<ClientResult>
//    {
//        private readonly ContinuationToken _firstPageToken;
//        private readonly Func<ContinuationToken, Task<ClientResult>> _getPageAsync;
//        private readonly Func<ContinuationToken, ClientResult, ContinuationToken?> _getNextPageToken;

//        public AsyncFuncResultEnumerable(ContinuationToken firstPageToken,
//            Func<ContinuationToken, Task<ClientResult>> getPageAsync,
//            Func<ContinuationToken, ClientResult, ContinuationToken?> getNextPageToken)
//        {
//            _firstPageToken = firstPageToken;
//            _getPageAsync = getPageAsync;
//            _getNextPageToken = getNextPageToken;
//        }

//        public async IAsyncEnumerator<ClientResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
//        {
//            ContinuationToken? pageToken = _firstPageToken;

//            do
//            {
//                ClientResult result = await _getPageAsync(pageToken).ConfigureAwait(false);
//                yield return result;
//                pageToken = _getNextPageToken(pageToken, result);
//            }
//            while (pageToken != null);
//        }
//    }

//    private class FuncResultEnumerable : IEnumerable<ClientResult>
//    {
//        private readonly ContinuationToken _firstPageToken;
//        private readonly Func<ContinuationToken, ClientResult> _getPage;
//        private readonly Func<ContinuationToken, ClientResult, ContinuationToken?> _getNextPageToken;

//        public FuncResultEnumerable(ContinuationToken firstPageToken,
//            Func<ContinuationToken, ClientResult> getPage,
//            Func<ContinuationToken, ClientResult, ContinuationToken?> getNextPageToken)
//        {
//            _firstPageToken = firstPageToken;
//            _getPage = getPage;
//            _getNextPageToken = getNextPageToken;
//        }

//        public IEnumerator<ClientResult> GetEnumerator()
//        {
//            ContinuationToken? pageToken = _firstPageToken;

//            do
//            {
//                ClientResult result = _getPage(pageToken);
//                yield return result;
//                pageToken = _getNextPageToken(pageToken, result);
//            }
//            while (pageToken != null);
//        }

//        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//    }
//}
