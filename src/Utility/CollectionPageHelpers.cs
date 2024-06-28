using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Utility;

internal class CollectionPageHelpers
{
    public static PageResult CreatePageProtocol(
            Func<Task<PageResult>> getNextAsync,
            Func<PageResult> getNext,
            bool hasNext,
            PipelineResponse response)
        => new FuncPageResult(getNextAsync, getNext, hasNext, response);

    private class FuncPageResult : PageResult
    {
        private readonly Func<Task<PageResult>> _getNextAsync;
        private readonly Func<PageResult> _getNext;

        public FuncPageResult(
            Func<Task<PageResult>> getNextAsync,
            Func<PageResult> getNext,
            bool hasNext,
            PipelineResponse response)
            : base(hasNext, response)
        {
            _getNextAsync = getNextAsync;
            _getNext = getNext;
        }

        protected override async Task<PageResult> GetNextAsyncCore()
            => await _getNextAsync().ConfigureAwait(false);

        protected override PageResult GetNextCore()
            => _getNext();
    }

    //public static AsyncCollectionResult<T> CreateAsync<T>(ContinuationToken firstPageToken,
    //    Func<ContinuationToken, Task<PageResult<T>>> getPageAsync) where T : notnull
    //    => new AsyncFuncCollectionResult<T>(firstPageToken, getPageAsync);

    //public static CollectionResult<T> Create<T>(ContinuationToken firstPageToken,
    //    Func<ContinuationToken, PageResult<T>> getPage) where T : notnull
    //    => new FuncCollectionResult<T>(firstPageToken, getPage);

    //private class AsyncFuncCollectionResult<T> : AsyncCollectionResult<T> where T : notnull
    //{
    //    //private readonly ContinuationToken _firstPageToken;
    //    //private readonly Func<ContinuationToken, Task<PageResult<T>>> _getPageAsync;

    //    //public AsyncFuncPageCollection(ContinuationToken firstPageToken,
    //    //    Func<ContinuationToken, Task<PageResult<T>>> getPageAsync)
    //    //{
    //    //    _firstPageToken = firstPageToken;
    //    //    _getPageAsync = getPageAsync;
    //    //}

    //    //public override ContinuationToken FirstPageToken => _firstPageToken;

    //    //public override async Task<PageResult<T>> GetPageAsyncCore(ContinuationToken pageToken)
    //    //    => await _getPageAsync(pageToken).ConfigureAwait(false);
    //}

    //private class FuncCollectionResult<T> : CollectionResult<T> where T : notnull
    //{
    //    //private readonly ContinuationToken _firstPageToken;
    //    //private readonly Func<ContinuationToken, PageResult<T>> _getPage;

    //    //public FuncPageCollection(ContinuationToken firstPageToken,
    //    //    Func<ContinuationToken, PageResult<T>> getPage)
    //    //{
    //    //    _firstPageToken = firstPageToken;
    //    //    _getPage = getPage;
    //    //}

    //    //public override ContinuationToken FirstPageToken => _firstPageToken;

    //    //public override PageResult<T> GetPageCore(ContinuationToken pageToken)
    //    //    => _getPage(pageToken);

    //    public override IEnumerator<T> GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
