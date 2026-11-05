using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

/// <summary>A bounded FIFO queue whose producers asynchronously wait for available capacity.</summary>
/// <typeparam name="T">The type of items stored in the queue.</typeparam>
/// <remarks>Concurrent producers wait asynchronously when the queue is full. Stop and await queue operations before disposal.</remarks>
internal class BoundedConcurrentQueue<T> : IDisposable
{
    private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
    private readonly SemaphoreSlim _availableSlots;
    private int _disposed;

    /// <summary>Gets the maximum number of items the queue can hold.</summary>
    public int Limit { get; }

    /// <summary>Initializes an empty queue with the specified capacity.</summary>
    /// <param name="limit">The maximum number of queued items. Must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="limit"/> is zero or negative.</exception>
    public BoundedConcurrentQueue(int limit)
    {
        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), limit, "The queue limit must be positive.");
        }

        Limit = limit;
        _availableSlots = new SemaphoreSlim(limit, limit);
    }

    /// <summary>Enqueues an item, asynchronously waiting for capacity when the queue is full.</summary>
    /// <param name="obj">The item to enqueue. May be null when <typeparamref name="T"/> permits null.</param>
    /// <param name="cancellationToken">Cancels the wait for capacity.</param>
    /// <returns>A task that completes once the item has been enqueued, not when it has been consumed.</returns>
    /// <remarks>
    /// Cancellation observed before acquiring capacity prevents insertion. Once capacity has been
    /// acquired, cancellation does not retract the item and the operation may complete successfully.
    /// </remarks>
    /// <exception cref="OperationCanceledException">The wait for capacity was canceled.</exception>
    /// <exception cref="ObjectDisposedException">The queue has been disposed.</exception>
    public async Task EnqueueAsync(T obj, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        await _availableSlots.WaitAsync(cancellationToken).ConfigureAwait(false);
        _queue.Enqueue(obj);
    }

    /// <summary>Attempts to remove the oldest item without waiting for an item to arrive.</summary>
    /// <param name="obj">The removed item, or the default value of <typeparamref name="T"/> if empty.</param>
    /// <returns>True if an item was removed; otherwise, false.</returns>
    /// <remarks>A successful removal releases capacity for one producer.</remarks>
    /// <exception cref="ObjectDisposedException">The queue has been disposed.</exception>
    public bool TryDequeue(out T obj)
    {
        ThrowIfDisposed();
        if (_queue.TryDequeue(out obj))
        {
            _availableSlots.Release();
            return true;
        }
        return false;
    }

    /// <summary>Releases the capacity semaphore and discards remaining items.</summary>
    /// <remarks>
    /// Cleanup is synchronous, so this queue implements <see cref="IDisposable"/> rather than
    /// <see cref="IAsyncDisposable"/>. This method does not cancel or await pending enqueues;
    /// the owner must stop and await all queue operations first. Repeated calls have no effect.
    /// Discarded items are not themselves disposed.
    /// </remarks>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
        {
            _availableSlots.Dispose();
            while (_queue.TryDequeue(out _))
            {
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(BoundedConcurrentQueue<T>));
        }
    }
}