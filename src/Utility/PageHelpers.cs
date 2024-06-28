using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Utility;

internal class PageHelpers
{
    public static AsyncCollectionResult<T> CreateCollectionAsync<T>(Func<Task<PageResult<T>>> getfirstPageAsync)
        => new AsyncFuncPagedCollection<T>(getfirstPageAsync);

    public static CollectionResult<T> CreateCollection<T>(Func<PageResult<T>> getfirstPage)
        => new FuncPagedCollection<T>(getfirstPage);

    public static PageResult<T> CreatePage<T>(
        IReadOnlyList<T> values,
        PageResult result,
        Func<PageResult, PageResult<T>> toPage)
        => new FuncPage<T>(values, result, toPage);

    public static PageResult CreatePageResult(
            ContinuationToken pageToken,
            ContinuationToken? nextPageToken,
            PipelineResponse response,
            Func<Task<PageResult>> getNextResultAsync,
            Func<PageResult> getNextResult)

        => new FuncPageResult(pageToken, nextPageToken, response, getNextResultAsync, getNextResult);

    private class AsyncFuncPagedCollection<T> : AsyncCollectionResult<T>
    {
        private readonly Func<Task<PageResult<T>>> _getfirstPageAsync;

        public AsyncFuncPagedCollection(Func<Task<PageResult<T>>> getfirstPageAsync)
        {
            _getfirstPageAsync = getfirstPageAsync;
        }

        public override async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            PageResult<T> page = await _getfirstPageAsync().ConfigureAwait(false);

            while (page.NextPageToken is not null)
            {
                foreach (T value in page.Values)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return value;
                }

                page = (PageResult<T>)await page.GetNextResultAsync().ConfigureAwait(false);
            }
        }
    }

    private class FuncPagedCollection<T> : CollectionResult<T>
    {
        private readonly Func<PageResult<T>> _getfirstPage;

        public FuncPagedCollection(Func<PageResult<T>> getfirstPage)
        {
            _getfirstPage = getfirstPage;
        }

        public override IEnumerator<T> GetEnumerator()
        {
            PageResult<T> page = _getfirstPage();

            while (page.NextPageToken is not null)
            {
                foreach (T value in page.Values)
                {
                    yield return value;
                }

                page = (PageResult<T>)page.GetNextResult();
            }
        }
    }

    // Convenience page result
    private class FuncPage<T> : PageResult<T>
    {
        private readonly PageResult _result;
        private readonly Func<PageResult, PageResult<T>> _toPage;

        public FuncPage(
            
            // page params
            IReadOnlyList<T> values,
            PageResult result,

            // convenience layer conversion params
            Func<PageResult, PageResult<T>> toPage)
            : base(values, result.PageToken, result.NextPageToken, result.GetRawResponse())
        {
            _result = result;
            _toPage = toPage;
        }

        protected override async Task<PageResult> GetNextResultAsyncCore()
            => _toPage(await _result.GetNextResultAsync().ConfigureAwait(false));

        protected override PageResult GetNextResultCore()
            => _toPage(_result.GetNextResult());
    }

    // Protocol page result
    private class FuncPageResult : PageResult
    {
        private readonly Func<Task<PageResult>> _getNextResultAsync;
        private readonly Func<PageResult> _getNextResult;

        public FuncPageResult(
            
            // page params
            ContinuationToken pageToken,
            ContinuationToken? nextPageToken,
            PipelineResponse response,

            // subclient params
            Func<Task<PageResult>> getNextResultAsync,
            Func<PageResult> getNextResult)

            : base(pageToken, nextPageToken, response)
        {
            _getNextResultAsync = getNextResultAsync;
            _getNextResult = getNextResult;
        }

        protected override async Task<PageResult> GetNextResultAsyncCore()
            => await _getNextResultAsync().ConfigureAwait(false);

        protected override PageResult GetNextResultCore()
            => _getNextResult();
    }

    // TODO - can we generalize page token too?
    //private class FuncPageToken<TToken> : ContinuationToken
    //{
    //    private readonly Func<BinaryData> _toBytes;
    //    private readonly Func<TToken?> _getNextPageToken;

    //    public FuncPageToken(
    //        Func<BinaryData> toBytes,
    //        Func<TToken?> getNextPageToken)
    //    {
    //        _toBytes = toBytes;
    //        _getNextPageToken = getNextPageToken;
    //    }

    //    public override BinaryData ToBytes() => _toBytes();

    //    public TToken? GetNextPageToken() => _getNextPageToken();
    //}
}
