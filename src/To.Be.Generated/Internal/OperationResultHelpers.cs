using System.ClientModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

#nullable enable

namespace OpenAI;

internal class OperationResultHelpers
{
    public static IAsyncEnumerable<ClientResult<T>> CreateAsync<T>(IAsyncEnumerator<ClientResult<T>> enumerator)
        => new AsyncPollingCollection<T>(enumerator);

    public static IEnumerable<ClientResult<T>> Create<T>(IEnumerator<ClientResult<T>> enumerator)
        => new PollingCollection<T>(enumerator);

    // We have these to support manually polling LROs -- i.e. this is layered
    // so that the IEnumerable adds the "wait polling interval" around the
    // outside of advancing the enumerator, so if someone wants to poll
    // manually, they just call updateCollection.GetEnumerator() and MoveNext
    // on it themselves in whatever polling pattern they'd like.
    private class AsyncPollingCollection<T> : IAsyncEnumerable<ClientResult<T>>
    {
        private readonly IAsyncEnumerator<ClientResult<T>> _enumerator;

        public AsyncPollingCollection(IAsyncEnumerator<ClientResult<T>> enumerator)
        {
            _enumerator = enumerator;
        }

        public IAsyncEnumerator<ClientResult<T>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => _enumerator;
    }

    private class PollingCollection<T> : IEnumerable<ClientResult<T>>
    {
        private readonly IEnumerator<ClientResult<T>> _enumerator;

        public PollingCollection(IEnumerator<ClientResult<T>> enumerator)
        {
            _enumerator = enumerator;
        }

        public IEnumerator<ClientResult<T>> GetEnumerator()
            => _enumerator;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
