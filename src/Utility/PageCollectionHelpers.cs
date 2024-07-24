using System.ClientModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class PageCollectionHelpers
{
    public static PageCollection<T> Create<T>(PageEnumerator<T> enumerator)
        => new EnumeratorPageCollection<T>(enumerator);

    public static AsyncPageCollection<T> CreateAsync<T>(PageEnumerator<T> enumerator)
        => new AsyncEnumeratorPageCollection<T>(enumerator);

    private class EnumeratorPageCollection<T> : PageCollection<T>
    {
        private readonly PageEnumerator<T> _enumerator;

        public EnumeratorPageCollection(PageEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        protected override PageResult<T> GetCurrentPageCore()
            => _enumerator.GetCurrentPage();

        protected override IEnumerator<PageResult<T>> GetEnumeratorCore()
            => _enumerator;
    }

    private class AsyncEnumeratorPageCollection<T> : AsyncPageCollection<T>
    {
        private readonly PageEnumerator<T> _enumerator;

        public AsyncEnumeratorPageCollection(PageEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        protected override async Task<PageResult<T>> GetCurrentPageAsyncCore()
            => await _enumerator.GetCurrentPageAsync().ConfigureAwait(false);

        protected override IAsyncEnumerator<PageResult<T>> GetAsyncEnumeratorCore(CancellationToken cancellationToken = default)
            => _enumerator;
    }
}
