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

internal class PaginationHelpers
{
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

    private class FuncPage<T> : PageResult<T>
    {
        private readonly PageResult _result;
        private readonly Func<PageResult, PageResult<T>> _toPage;

        // TODO: can it be simplified?
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
}
