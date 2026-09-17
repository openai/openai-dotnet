using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

/// <summary>An owned, reusable Responses API WebSocket with one physical reader.</summary>
[Experimental("OPENAI001")]
public sealed class ResponseWebSocketConnection : IDisposable, IAsyncDisposable
{
    private readonly WebSocket _socket;
    private readonly ResponseWebSocketOptions _options;
    private readonly Func<CancellationToken, Task<ResponseWebSocketConnection>> _open;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly object _gate = new();
    private readonly Dictionary<string, ResponseWebSocketQueue> _lanes = new(StringComparer.Ordinal);
    private readonly ResponseWebSocketQueue _events;
    private readonly Task _reader;
    private int _pendingSends;
    private int _buffered;
    private long _bufferedBytes;
    private int _closed;
    private int _closing;
    private Exception _failure;

    internal ResponseWebSocketConnection(WebSocket socket, ResponseWebSocketOptions options,
        Func<CancellationToken, Task<ResponseWebSocketConnection>> open)
    {
        _socket = socket;
        _options = options;
        _open = open;
        _events = new ResponseWebSocketQueue(Removed);
        _reader = ReadAsync();
    }

    /// <summary>True when this connection replaced a disconnected connection. Server-side cache is not restored automatically.</summary>
    public bool HasConnectionStateLoss { get; private set; }

    /// <summary>Receives the next event not claimed by an explicitly opened lane. Canceling a wait leaves the connection open.</summary>
    public Task<ResponseWebSocketServerEvent> ReceiveAsync(CancellationToken cancellationToken = default)
        => _events.ReceiveAsync(cancellationToken);

    /// <summary>Reads the default event stream through a terminal response and returns its complete output.</summary>
    /// <remarks>Events with a named stream identifier are skipped. Use ReceiveAsync to observe unclaimed named-stream events.</remarks>
    /// <exception cref="InvalidDataException">The terminal event has no response snapshot.</exception>
    public Task<ResponseResult> ReceiveResponseAsync(CancellationToken cancellationToken = default)
        => ResponseWebSocketLane.ReadResponseAsync(_events, cancellationToken, defaultLane: true);

    /// <summary>Enumerates unclaimed events. Cancellation cancels the wait, not the socket.</summary>
    /// <remarks>Owns the default event stream until enumeration ends or the enumerator is disposed.</remarks>
    public async IAsyncEnumerable<ResponseWebSocketServerEvent> GetEventsAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _events.EnterRead();
        try
        {
            while (true) yield return await _events.ReceiveCoreAsync(cancellationToken).ConfigureAwait(false);
        }
        finally { _events.ExitRead(); }
    }

    /// <summary>Reserves a stream identifier before sending commands for it. Each event has exactly one receiver.</summary>
    public ResponseWebSocketLane OpenLane(string streamId)
    {
        if (string.IsNullOrEmpty(streamId)) throw new ArgumentException("A lane requires a nonempty stream identifier.", nameof(streamId));
        lock (_gate)
        {
            ThrowIfClosed();
            if (_lanes.Count >= _options.MaxLanes) throw new InvalidOperationException("The connection's lane limit was reached.");
            if (_lanes.ContainsKey(streamId)) throw new InvalidOperationException("This stream identifier already has a lane.");
            var queue = new ResponseWebSocketQueue(Removed);
            _lanes.Add(streamId, queue);
            return new ResponseWebSocketLane(this, streamId, queue);
        }
    }

    internal void Detach(string streamId, ResponseWebSocketQueue queue)
    {
        lock (_gate)
        {
            if (_lanes.TryGetValue(streamId, out var existing) && ReferenceEquals(existing, queue))
            {
                _lanes.Remove(streamId);
                queue.Complete(new ObjectDisposedException(nameof(ResponseWebSocketLane)), discard: true);
            }
        }
    }

    /// <summary>Sends a generated command. Successful completion indicates a local write, not server acceptance.</summary>
    public Task SendAsync(ResponseWebSocketCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        return SendCoreAsync(() => ModelReaderWriter.Write(command, ModelReaderWriterOptions.Json, OpenAIContext.Default), cancellationToken);
    }

    /// <summary>Sends one raw JSON object, allowing forward-compatible command fields.</summary>
    /// <remarks>A canceled or failed physical write aborts the connection because delivery may be uncertain. Commands are never replayed.</remarks>
    public Task SendAsync(BinaryData command, CancellationToken cancellationToken = default)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        return SendCoreAsync(() => command, cancellationToken);
    }

    internal async Task SendCoreAsync(Func<BinaryData> prepare, CancellationToken cancellationToken)
    {
        ThrowIfClosed();
        cancellationToken.ThrowIfCancellationRequested();
        if (Interlocked.Increment(ref _pendingSends) > _options.MaxPendingSends)
        {
            Interlocked.Decrement(ref _pendingSends);
            throw new InvalidOperationException("The connection's pending-send limit was reached. The command was not sent.");
        }
        bool acquired = false;
        try
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
            var command = prepare();
            if (_options.MaxMessageBytes > 0 && command.ToMemory().Length > _options.MaxMessageBytes) throw new ArgumentException("The command exceeds MaxMessageBytes.", nameof(command));
            using (var document = System.Text.Json.JsonDocument.Parse(command, ModelSerializationExtensions.JsonDocumentOptions))
            {
                if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
                    throw new ArgumentException("A WebSocket command must be a JSON object.", nameof(command));
            }
            await _sendLock.WaitAsync(linked.Token).ConfigureAwait(false);
            acquired = true;
            ThrowIfClosed();
            cancellationToken.ThrowIfCancellationRequested();
            var bytes = command.ToArray();
            try
            {
                await _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, linked.Token).ConfigureAwait(false);
            }
            catch (Exception error)
            {
                Fail(error);
                throw;
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            ThrowIfClosed();
            throw;
        }
        finally
        {
            if (acquired) _sendLock.Release();
            Interlocked.Decrement(ref _pendingSends);
        }
    }

    private async Task ReadAsync()
    {
        var buffer = new byte[_options.MaxMessageBytes > 0 ? Math.Min(8192, _options.MaxMessageBytes) : 8192];
        try
        {
            while (!_lifetime.IsCancellationRequested)
            {
                using var message = new MemoryStream();
                WebSocketReceiveResult frame;
                do
                {
                    frame = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), _lifetime.Token).ConfigureAwait(false);
                    if (frame.MessageType == WebSocketMessageType.Close)
                        throw new EndOfStreamException($"The Responses WebSocket closed ({frame.CloseStatus}). A response is complete only after a terminal event.");
                    if (frame.MessageType != WebSocketMessageType.Text) throw new InvalidDataException("Expected a text WebSocket message.");
                    if (_options.MaxMessageBytes > 0 && frame.Count > _options.MaxMessageBytes - message.Length) throw new InvalidDataException("The incoming message exceeds MaxMessageBytes.");
                    message.Write(buffer, 0, frame.Count);
                } while (!frame.EndOfMessage);
                var item = ResponseWebSocketServerEvent.Parse(BinaryData.FromBytes(message.ToArray()));
                lock (_gate)
                {
                    long bytes = Interlocked.Add(ref _bufferedBytes, item.RawData.ToMemory().Length);
                    if (Interlocked.Increment(ref _buffered) > _options.MaxBufferedEvents || (_options.MaxBufferedBytes > 0 && bytes > _options.MaxBufferedBytes))
                    {
                        Removed(item);
                        throw new InvalidDataException("The connection's buffered-event limit was reached.");
                    }
                    var queue = item.StreamId != null && _lanes.TryGetValue(item.StreamId, out var lane) ? lane : _events;
                    if (!queue.Add(item)) Removed(item);
                }
            }
        }
        // Every reader failure must reach waiting receivers, including failures from a custom socket.
        catch (Exception error) { Fail(error); }
    }

    private void Removed(ResponseWebSocketServerEvent item)
    {
        Interlocked.Decrement(ref _buffered);
        Interlocked.Add(ref _bufferedBytes, -item.RawData.ToMemory().Length);
    }

    private void ThrowIfClosed()
    {
        if (Volatile.Read(ref _closed) != 0 || Volatile.Read(ref _closing) != 0) throw new InvalidOperationException("The Responses WebSocket is closed.", _failure);
    }

    private void Fail(Exception error, bool discard = false)
    {
        lock (_gate)
        {
            if (Interlocked.Exchange(ref _closed, 1) != 0)
            {
                if (discard)
                {
                    _events.Complete(error, discard: true);
                    foreach (var lane in _lanes.Values) lane.Complete(error, discard: true);
                }
                return;
            }
            _failure = error;
            _events.Complete(error, discard);
            foreach (var lane in _lanes.Values) lane.Complete(error, discard);
        }
        // An injected transport can run cancellation callbacks. Do not invoke it under the routing lock.
        try { _lifetime.Cancel(); }
        catch (AggregateException) { } // Cancellation callbacks must not replace the error that initiated shutdown.
        try { _socket.Abort(); }
        catch (Exception) { } // A custom socket's cleanup must not replace the error that initiated shutdown.
    }

    /// <summary>Immediately ends the connection and unblocks all waiting operations.</summary>
    public void Abort() => Fail(new ObjectDisposedException(nameof(ResponseWebSocketConnection)), discard: true);

    /// <summary>Closes the connection within the configured close timeout. Repeated calls are safe.</summary>
    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        if (Volatile.Read(ref _closed) != 0 || Interlocked.Exchange(ref _closing, 1) != 0) return;
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(_options.CloseTimeout);
        bool acquired = false;
        try
        {
            await _sendLock.WaitAsync(timeout.Token).ConfigureAwait(false);
            acquired = true;
            if (_socket.State == WebSocketState.Open)
                await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", timeout.Token).ConfigureAwait(false);
            await Task.WhenAny(_reader, Task.Delay(_options.CloseTimeout, timeout.Token)).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
        }
        finally
        {
            Abort();
            if (acquired) _sendLock.Release();
        }
    }

    /// <summary>
    /// Explicitly replaces this connection and invokes application restoration. No command is replayed,
    /// and connection-local server state is lost. Restore must complete before the replacement is returned.
    /// </summary>
    /// <remarks>
    /// The replacement is a separate object owned by the caller. Closing or disposing this object affects
    /// only the old connection, even during recovery. Use <paramref name="cancellationToken"/> to cancel
    /// opening the replacement or cooperative restoration, and dispose the returned connection separately.
    /// </remarks>
    public async Task<ResponseWebSocketConnection> ReconnectAsync(
        Func<ResponseWebSocketConnection, CancellationToken, Task> restore,
        int maxAttempts = 1, CancellationToken cancellationToken = default)
    {
        if (restore == null) throw new ArgumentNullException(nameof(restore));
        if (maxAttempts < 1) throw new ArgumentOutOfRangeException(nameof(maxAttempts));
        await DisposeAsync().ConfigureAwait(false);
        for (int attempt = 1; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ResponseWebSocketConnection replacement;
            try { replacement = await _open(cancellationToken).ConfigureAwait(false); }
            // The caller opts into bounded retries; custom connectors can report opening failures with any exception type.
            catch when (attempt < maxAttempts && !cancellationToken.IsCancellationRequested) { continue; }
            replacement.HasConnectionStateLoss = true;
            try { await restore(replacement, cancellationToken).ConfigureAwait(false); return replacement; }
            catch
            {
                try { await replacement.DisposeAsync().ConfigureAwait(false); }
                catch (Exception) { } // A custom socket's cleanup must not replace the application restoration failure.
                throw;
            }
        }
    }

    /// <summary>Aborts and disposes the owned socket. It does not dispose shared client transports.</summary>
    public void Dispose() { Abort(); _socket.Dispose(); }
    /// <summary>Aborts, waits for the owned reader to stop, and disposes the socket.</summary>
    public async ValueTask DisposeAsync() { Abort(); await _reader.ConfigureAwait(false); _socket.Dispose(); }
}
