using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading;

#nullable enable

namespace OpenAI;

internal class PageCollectionHelpers
{
    public static PageCollection<T> Create<T>(PageEnumerator<T> enumerator)
        => new EnumeratorPageCollection<T>(enumerator);

    public static AsyncPageCollection<T> CreateAsync<T>(PageEnumerator<T> enumerator)
        => new EnumeratorAsyncPageCollection<T>(enumerator);

    private class EnumeratorPageCollection<T> : PageCollection<T>
    {
        private readonly PageEnumerator<T> _enumerator;

        public EnumeratorPageCollection(PageEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        protected override IEnumerator<PageResult<T>> GetEnumeratorCore()
            => _enumerator;
    }

    private class EnumeratorAsyncPageCollection<T> : AsyncPageCollection<T>
    {
        private readonly PageEnumerator<T> _enumerator;

        public EnumeratorAsyncPageCollection(PageEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        protected override IAsyncEnumerator<PageResult<T>> GetAsyncEnumeratorCore(CancellationToken cancellationToken = default)
            => _enumerator;
    }

    public static IEnumerable<ClientResult> Create(PageResultEnumerator enumerator)
    {
        do
        {
            if (enumerator.Current is not null)
            {
                yield return enumerator.Current;
            }
        }
        while (enumerator.MoveNext());
    }

    public static async IAsyncEnumerable<ClientResult> CreateAsync(PageResultEnumerator enumerator)
    {
        do
        {
            if (enumerator.Current is not null)
            {
                yield return enumerator.Current;
            }
        }
        while (await enumerator.MoveNextAsync().ConfigureAwait(false));
    }
}
