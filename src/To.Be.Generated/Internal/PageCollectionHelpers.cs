using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading;

#nullable enable

namespace OpenAI;

internal class PageCollectionHelpers
{
    public static AsyncPageCollection<T> CreateAsync<T>(
        IAsyncEnumerator<ClientResult> enumerator,
        Func<ClientResult, PageResult<T>> getPageFromResult)
        => new AsyncFuncPageCollection<T>(enumerator, getPageFromResult);

    public static PageCollection<T> Create<T>(
        IEnumerator<ClientResult> enumerator,
        Func<ClientResult, PageResult<T>> getPageFromResult)
        => new FuncPageCollection<T>(enumerator, getPageFromResult);

    public static IEnumerable<ClientResult> CreateProtocol(IEnumerator<ClientResult> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    public static async IAsyncEnumerable<ClientResult> CreateProtocolAsync(IAsyncEnumerator<ClientResult> enumerator)
    {
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield return enumerator.Current;
        }
    }

    private class AsyncFuncPageCollection<T> : AsyncPageCollection<T>
    {
        private readonly IAsyncEnumerator<ClientResult> _enumerator;
        private readonly Func<ClientResult, PageResult<T>> _getPageFromResult;

        public AsyncFuncPageCollection(
            IAsyncEnumerator<ClientResult> enumerator,
            Func<ClientResult, PageResult<T>> getPageFromResult)
        {
            _enumerator = enumerator;
            _getPageFromResult = getPageFromResult;
        }

        protected override async IAsyncEnumerator<PageResult<T>> GetAsyncEnumeratorCore(CancellationToken cancellationToken = default)
        {
            while (await _enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield return _getPageFromResult(_enumerator.Current);
            }
        }
    }

    private class FuncPageCollection<T> : PageCollection<T>
    {
        private readonly IEnumerator<ClientResult> _enumerator;
        private readonly Func<ClientResult, PageResult<T>> _getPageFromResult;

        public FuncPageCollection(
            IEnumerator<ClientResult> enumerator,
            Func<ClientResult, PageResult<T>> getPageFromResult)
        {
            _enumerator = enumerator;
            _getPageFromResult = getPageFromResult;
        }

        protected override IEnumerator<PageResult<T>> GetEnumeratorCore()
        {
            while (_enumerator.MoveNext())
            {
                yield return _getPageFromResult(_enumerator.Current);
            }
        }
    }
}
