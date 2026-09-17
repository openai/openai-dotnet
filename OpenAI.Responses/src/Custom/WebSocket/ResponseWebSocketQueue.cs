using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

// Waiting does not remove an item: cancellation cannot consume the next event.
internal sealed class ResponseWebSocketQueue
{
    private readonly Queue<ResponseWebSocketServerEvent> _items = new();
    private TaskCompletionSource<bool> _changed = NewSignal();
    private Exception _error;
    private int _receiving;
    private readonly object _gate = new();
    private readonly Action<ResponseWebSocketServerEvent> _removed;

    internal ResponseWebSocketQueue(Action<ResponseWebSocketServerEvent> removed) => _removed = removed;
    private static TaskCompletionSource<bool> NewSignal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    internal bool Add(ResponseWebSocketServerEvent item)
    {
        lock (_gate)
        {
            if (_error != null) return false;
            _items.Enqueue(item);
            var signal = _changed;
            _changed = NewSignal();
            signal.TrySetResult(true);
            return true;
        }
    }

    internal void Complete(Exception error, bool discard = false)
    {
        lock (_gate)
        {
            _error ??= error;
            if (discard) while (_items.Count > 0) _removed(_items.Dequeue());
            _changed.TrySetResult(true);
        }
    }

    internal void EnterRead()
    {
        if (Interlocked.Exchange(ref _receiving, 1) != 0)
            throw new InvalidOperationException("Only one receive operation may be active for an event stream.");
    }

    internal void ExitRead() => Volatile.Write(ref _receiving, 0);

    internal async Task<ResponseWebSocketServerEvent> ReceiveAsync(CancellationToken cancellationToken)
    {
        EnterRead();
        try { return await ReceiveCoreAsync(cancellationToken).ConfigureAwait(false); }
        finally { ExitRead(); }
    }

    // The caller holds stream ownership for its entire receive operation.
    internal async Task<ResponseWebSocketServerEvent> ReceiveCoreAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            Task changed;
            lock (_gate)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_items.Count > 0) { var item = _items.Dequeue(); _removed(item); return item; }
                if (_error != null) throw _error;
                changed = _changed.Task;
            }
            var canceled = NewSignal();
            using (cancellationToken.Register(() => canceled.TrySetCanceled(cancellationToken)))
            {
                await await Task.WhenAny(changed, canceled.Task).ConfigureAwait(false);
            }
        }
    }
}
