using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

/// <summary>Dispatches queued messages to a single asynchronous handler in FIFO order.</summary>
/// <typeparam name="T">The type of messages accepted by the handler.</typeparam>
/// <remarks>
/// Dispatch starts at construction and awaits each handler before processing the next message.
/// Disposal discards pending messages but lets the active handler finish. A handler must not await this
/// subscriber's <see cref="Completion"/> or <see cref="DisposeAsync"/>.
/// </remarks>
internal sealed class MessageReceivedSubscriber<T> : IDisposable, IAsyncDisposable
{
    private readonly Func<T, Task> _handler;
    private readonly BoundedConcurrentQueue<T> _queue;
    private readonly SemaphoreSlim _availableItems = new SemaphoreSlim(0);
    private readonly CancellationTokenSource _stopSource = new CancellationTokenSource();
    private readonly TaskCompletionSource<bool> _resourcesDisposed = new TaskCompletionSource<bool>(
        TaskCreationOptions.RunContinuationsAsynchronously);
    // The drain loop owns the initial reference. Enqueue and cancellation operations add their own.
    private int _activeOperations = 1;
    private int _cleanupStarted;
    private int _disposed;

    /// <summary>Gets the task that completes after dispatch stops and owned resources are released.</summary>
    /// <remarks>
    /// Normal disposal completes this task successfully. Handler exceptions propagate through this task;
    /// a handler cancellation can leave it canceled. Awaiting this property does not request shutdown.
    /// </remarks>
    internal Task Completion { get; }

    /// <summary>Creates a subscriber and starts its asynchronous dispatch loop.</summary>
    /// <param name="handler">The callback to await for each message. Must return a non-null task.</param>
    /// <param name="capacity">The maximum number of pending messages. Defaults to 100.</param>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is null.</exception>
    internal MessageReceivedSubscriber(Func<T, Task> handler, int capacity = 100)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _queue = new BoundedConcurrentQueue<T>(capacity);
        Completion = DrainQueueAsync();
    }

    /// <summary>Queues a message, asynchronously waiting when the pending-message queue is full.</summary>
    /// <param name="message">The message to pass to the handler.</param>
    /// <param name="cancellationToken">Cancels waiting for queue capacity, not handler execution.</param>
    /// <returns>A task representing acceptance into the queue, not completion of the handler.</returns>
    /// <remarks>
    /// Shutdown cancels blocked enqueues. Acceptance does not guarantee delivery if shutdown races
    /// with enqueueing or occurs before dispatch. Canceling an accepted message's token does not remove it.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Shutdown has already been requested.</exception>
    /// <exception cref="OperationCanceledException">The capacity wait or subscriber lifetime was canceled.</exception>
    internal async Task EnqueueAsync(T message, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _activeOperations);
        try
        {
            if (Volatile.Read(ref _disposed) != 0)
            {
                throw new ObjectDisposedException(nameof(MessageReceivedSubscriber<T>));
            }

            using CancellationTokenSource linkedSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, _stopSource.Token);
            await _queue.EnqueueAsync(message, linkedSource.Token).ConfigureAwait(false);
            _stopSource.Token.ThrowIfCancellationRequested();
            _availableItems.Release();
        }
        finally
        {
            ReleaseOperation();
        }
    }

    private async Task DrainQueueAsync()
    {
        CancellationToken stopToken = _stopSource.Token;
        try
        {
            while (!stopToken.IsCancellationRequested)
            {
                try
                {
                    await _availableItems.WaitAsync(stopToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (stopToken.IsCancellationRequested)
                {
                    return;
                }

                if (!stopToken.IsCancellationRequested && _queue.TryDequeue(out T message))
                {
                    await _handler(message).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            Dispose();
            ReleaseOperation();
            await _resourcesDisposed.Task.ConfigureAwait(false);
        }
    }

    /// <summary>Requests shutdown and cancels blocked producers without waiting for active work.</summary>
    /// <remarks>
    /// Prevents new enqueues. Owned resources are released after active enqueues and dispatch have stopped.
    /// Repeated calls are safe. Use <see cref="DisposeAsync"/> or await <see cref="Completion"/> to observe
    /// cleanup completion and handler failures without blocking the calling thread.
    /// </remarks>
    public void Dispose()
    {
        Interlocked.Increment(ref _activeOperations);
        try
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _stopSource.Cancel();
            }
        }
        finally
        {
            ReleaseOperation();
        }
    }

    /// <summary>Requests shutdown and asynchronously waits for active work and resource cleanup.</summary>
    /// <returns>An operation that completes when <see cref="Completion"/> completes.</returns>
    /// <remarks>
    /// Propagates handler failures or cancellation through <see cref="Completion"/>. Repeated calls await
    /// the same completion task. Prefer this method, or await using, when the caller must await cleanup.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        Dispose();
        await Completion.ConfigureAwait(false);
    }

    private void ReleaseOperation()
    {
        if (Interlocked.Decrement(ref _activeOperations) == 0
            && Volatile.Read(ref _disposed) != 0
            && Interlocked.Exchange(ref _cleanupStarted, 1) == 0)
        {
            _queue.Dispose();
            _availableItems.Dispose();
            _stopSource.Dispose();
            _resourcesDisposed.SetResult(true);
        }
    }
}
