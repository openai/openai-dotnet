using System;
using System.Buffers;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

/// <summary>Implements the WebSocket connection, message transport, and shutdown lifecycle.</summary>
public partial class ResponsesWebSocketClient
{
    private readonly SemaphoreSlim _connectSemaphore = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim _sendSemaphore = new SemaphoreSlim(1, 1);
    private readonly CancellationTokenSource _lifetimeSource = new CancellationTokenSource();
    private readonly TaskCompletionSource<bool> _transportCompletion = new TaskCompletionSource<bool>(
        TaskCreationOptions.RunContinuationsAsynchronously);
    private WebSocket _webSocket;
    private Task _receiveTask;
    private int _connectionState;
    private long _bufferedMessageBytes;
    private readonly TaskCompletionSource<bool> _transportResourcesDisposed = new TaskCompletionSource<bool>(
        TaskCreationOptions.RunContinuationsAsynchronously);
    private int _transportOperationCount = 1;
    private bool _transportStopped;

    private sealed class TransportOperation : IDisposable
    {
        private ResponsesWebSocketClient _owner;
        public TransportOperation(ResponsesWebSocketClient owner) => _owner = owner;
        public void Dispose() => Interlocked.Exchange(ref _owner, null)?.ReleaseTransportOperation();
    }

    private TransportOperation AcquireTransportOperation()
    {
        lock (_syncLock)
        {
            ThrowIfDisposed();
            Interlocked.Increment(ref _transportOperationCount);
            return new TransportOperation(this);
        }
    }

    private void ReleaseTransportOperation()
    {
        if (Interlocked.Decrement(ref _transportOperationCount) == 0)
        {
            _webSocket?.Dispose();
            _connectSemaphore.Dispose();
            _sendSemaphore.Dispose();
            _lifetimeSource.Dispose();
            _transportResourcesDisposed.TrySetResult(true);
        }
    }

    private void ReleaseBufferedBytes(MessageReceivedSubscription subscription, long bytes)
    {
        var released = Math.Min(subscription.PendingBytes, bytes);
        subscription.PendingBytes -= released;
        _bufferedMessageBytes -= released;
    }

    /// <summary>Accounts for a message against the shared byte budget before placing it in a subscriber queue.</summary>
    private async Task EnqueueMessageAsync(MessageReceivedSubscription subscription,
        ResponsesWebSocketMessageReceivedEventArgs message, CancellationToken cancellationToken)
    {
        var bytes = message.Message?.RawData?.ToMemory().Length ?? 0;
        if (_options.MaxBufferedBytes > 0 && bytes > _options.MaxBufferedBytes - _bufferedMessageBytes)
        {
            throw new InvalidDataException("The incoming message queues exceed MaxBufferedBytes.");
        }
        _bufferedMessageBytes += bytes;
        subscription.PendingBytes += bytes;
        try
        {
            await subscription.Subscriber.EnqueueAsync(message, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            lock (_syncLock)
            {
                ReleaseBufferedBytes(subscription, bytes);
            }
            throw;
        }
    }

    /// <summary>Observes a subscriber's completion and stops the connection if a callback fails.</summary>
    private async Task ObserveSubscriberAsync(Task completion, bool isErrorHandler)
    {
        using var operation = AcquireTransportOperation();
        try
        {
            await completion.ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _transportCompletion.TrySetException(exception);
            if (!isErrorHandler && !_lifetimeSource.IsCancellationRequested)
            {
                try
                {
                    await DispatchErrorAsync(new ResponsesWebSocketErrorEventArgs(exception, true, _lifetimeSource.Token), _lifetimeSource.Token)
                        .ConfigureAwait(false);
                }
                catch (Exception reportingException)
                {
                    _transportCompletion.TrySetException(reportingException);
                }
            }
            _lifetimeSource.Cancel();
            _webSocket?.Abort();
        }
    }

    /// <summary>Completes when the connection ends; faults on terminal transport or callback failures.</summary>
    /// <remarks>Does not wait for user callbacks. Use DisposeAsync to await subscriber cleanup.</remarks>
    public Task Completion => _transportCompletion.Task;

    /// <summary>Creates and connects the underlying socket. Override for testing with an in-memory socket.</summary>
    protected virtual async Task<WebSocket> ConnectWebSocketAsync(CancellationToken cancellationToken)
    {
        var socket = new ClientWebSocket();
        try
        {
            foreach (var header in _options.Headers)
            {
                socket.Options.SetRequestHeader(header.Key, header.Value);
            }
            await _responsesClient.AuthenticateWebSocketAsync(socket, _websocketEndpoint, cancellationToken).ConfigureAwait(false);
            await socket.ConnectAsync(_websocketEndpoint, cancellationToken).ConfigureAwait(false);
            return socket;
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }

    /// <summary>Opens the initial socket once and starts the single receive loop.</summary>
    private async Task ConnectTransportAsync(CancellationToken cancellationToken)
    {
        using var operation = AcquireTransportOperation();
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, _lifetimeSource.Token);
        await _connectSemaphore.WaitAsync(linked.Token).ConfigureAwait(false);
        var opening = false;
        try
        {
            lock (_syncLock)
            {
                ThrowIfDisposed();
                if (State == ResponsesWebSocketConnectionState.Connected)
                {
                    return;
                }
                if (State != ResponsesWebSocketConnectionState.Created)
                {
                    throw new InvalidOperationException("This connection lifetime has already ended or is reconnecting.");
                }
                if (_messageReceivedSubscribers.Count == 0 || _errorOccurredSubscribers.Count == 0)
                {
                    throw new InvalidOperationException("Subscribe to both MessageReceivedAsync and ErrorOccurredAsync before connecting.");
                }
                SetState(ResponsesWebSocketConnectionState.Connecting);
                opening = true;
            }

            var socket = await ConnectWebSocketAsync(linked.Token).ConfigureAwait(false);
            lock (_syncLock)
            {
                if (_lifetimeSource.IsCancellationRequested)
                {
                    socket.Dispose();
                    linked.Token.ThrowIfCancellationRequested();
                }
                _webSocket = socket;
                SetState(ResponsesWebSocketConnectionState.Connected);
                var receiveOperation = AcquireTransportOperation();
                _receiveTask = Task.Run(async () =>
                {
                    using (receiveOperation)
                    {
                        await ReceiveTransportAsync().ConfigureAwait(false);
                    }
                });
            }
        }
        catch
        {
            lock (_syncLock)
            {
                if (opening && !_lifetimeSource.IsCancellationRequested && !_disposed)
                {
                    SetState(ResponsesWebSocketConnectionState.Created);
                }
            }
            throw;
        }
        finally
        {
            _connectSemaphore.Release();
        }
    }

    /// <summary>
    /// Reads and dispatches server messages in wire order. Recoverable socket failures enter the reconnect
    /// loop; protocol, parsing, and callback failures terminate the client.
    /// </summary>
    private async Task ReceiveTransportAsync()
    {
        var token = _lifetimeSource.Token;
        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var data = await ReceiveMessageAsync(_webSocket, token).ConfigureAwait(false);
                    if (data is null)
                    {
                        await HandleCloseReceivedAsync(token).ConfigureAwait(false);
                        return;
                    }
                    var message = ParseMessage(data);
                    await DispatchMessageAsync(new ResponsesWebSocketMessageReceivedEventArgs(message, token), token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    break;
                }
                catch (WebSocketException exception) when (_options.ShouldReconnect && !token.IsCancellationRequested
                    && State != ResponsesWebSocketConnectionState.Closing)
                {
                    lock (_syncLock)
                    {
                        if (_disposed || token.IsCancellationRequested
                            || State == ResponsesWebSocketConnectionState.Closing)
                        {
                            break;
                        }
                        SetState(ResponsesWebSocketConnectionState.Reconnecting);
                    }
                    await DispatchErrorAsync(new ResponsesWebSocketErrorEventArgs(exception, false, token), token).ConfigureAwait(false);
                    await ReconnectAsync(token).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Lifetime cancellation is expected during shutdown; the finally block performs cleanup.
        }
        catch (WebSocketException) when (token.IsCancellationRequested)
        {
            // Aborting the socket during shutdown can surface as WebSocketException rather than cancellation.
        }
        catch (Exception exception)
        {
            _transportCompletion.TrySetException(exception);
            if (!token.IsCancellationRequested)
            {
                await DispatchErrorAsync(new ResponsesWebSocketErrorEventArgs(exception, true, token), token).ConfigureAwait(false);
            }
        }
        finally
        {
            _lifetimeSource.Cancel();
            _webSocket?.Abort();
            lock (_syncLock)
            {
                SetState(_disposed ? ResponsesWebSocketConnectionState.Disposed : ResponsesWebSocketConnectionState.Closed);
            }
            _transportCompletion.TrySetResult(true);
        }
    }

    /// <summary>Acknowledges an expected close or throws so an unexpected close follows transport failure handling.</summary>
    private async Task HandleCloseReceivedAsync(CancellationToken cancellationToken)
    {
        if (_webSocket.CloseStatus != WebSocketCloseStatus.NormalClosure
            && State != ResponsesWebSocketConnectionState.Closing)
        {
            throw new WebSocketException("The server closed the Responses WebSocket unexpectedly.");
        }

        await _sendSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_webSocket.State != WebSocketState.CloseReceived)
            {
                return;
            }

            await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Close acknowledged", cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _sendSemaphore.Release();
        }
    }

    /// <summary>Reopens the socket after a transport failure without replaying commands.</summary>
    private async Task ReconnectAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
            try
            {
                var replacement = await ConnectWebSocketAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    await _sendSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    replacement.Dispose();
                    throw;
                }
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        replacement.Dispose();
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    lock (_syncLock)
                    {
                        if (State != ResponsesWebSocketConnectionState.Reconnecting)
                        {
                            replacement.Dispose();
                            return;
                        }
                        _webSocket.Dispose();
                        _webSocket = replacement;
                        SetState(ResponsesWebSocketConnectionState.Connected);
                    }
                    return;
                }
                finally
                {
                    _sendSemaphore.Release();
                }
            }
            catch (WebSocketException exception)
            {
                await DispatchErrorAsync(new ResponsesWebSocketErrorEventArgs(exception, false, cancellationToken), cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }

    /// <summary>Collects one complete text message, handling fragmentation and enforcing the configured size limit.</summary>
    private async Task<BinaryData> ReceiveMessageAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(16 * 1024);
        try
        {
            using var payload = new MemoryStream();
            while (true)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken)
                    .ConfigureAwait(false);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    if (payload.Length != 0)
                    {
                        throw new InvalidDataException("The WebSocket closed during a fragmented message.");
                    }
                    return null;
                }
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    throw new InvalidDataException("Responses WebSocket messages must be UTF-8 JSON text.");
                }
                if (_options.MaxMessageBytes > 0 && payload.Length + result.Count > _options.MaxMessageBytes)
                {
                    throw new InvalidDataException("The incoming WebSocket message exceeds MaxMessageBytes.");
                }
                payload.Write(buffer, 0, result.Count);
                if (result.EndOfMessage)
                {
                    return BinaryData.FromBytes(payload.ToArray());
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>Reads the event envelope, deserializes known streaming updates, and preserves the original JSON.</summary>
    private static ResponsesWebSocketMessage ParseMessage(BinaryData data)
    {
        using var document = JsonDocument.Parse(data);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("type", out var type)
            || type.ValueKind != JsonValueKind.String)
        {
            throw new InvalidDataException("A Responses WebSocket message must be an object with a string type.");
        }
        var kind = type.GetString();
        var streamId = root.TryGetProperty("stream_id", out var stream) ? stream.GetString() : null;
        var error = kind == "error" && root.TryGetProperty("error", out var errorJson)
            ? ResponseError.DeserializeResponseError(errorJson, BinaryData.FromString(errorJson.GetRawText()), ModelReaderWriterOptions.Json)
            : null;
        int? status = root.TryGetProperty("status", out var statusJson) && statusJson.ValueKind != JsonValueKind.Null
            ? statusJson.GetInt32() : null;
        var update = kind != "error" && root.TryGetProperty("sequence_number", out _)
            ? StreamingResponseUpdate.DeserializeStreamingResponseUpdate(root, data, ModelReaderWriterOptions.Json)
            : null;
        return new ResponsesWebSocketMessage(new ResponsesWebSocketMessageKind(kind), streamId, update, error, status, data);
    }

    /// <summary>Validates and sends one JSON object while serializing access to the socket.</summary>
    private async Task SendJsonAsync(BinaryData command, CancellationToken cancellationToken)
    {
        using var operation = AcquireTransportOperation();
        Argument.AssertNotNull(command, nameof(command));
        using var document = JsonDocument.Parse(command);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("A WebSocket command must be one JSON object.", nameof(command));
        }
        var bytes = command.ToArray();
        if (_options.MaxMessageBytes > 0 && bytes.Length > _options.MaxMessageBytes)
        {
            throw new ArgumentException("The outgoing command exceeds MaxMessageBytes.", nameof(command));
        }
        lock (_syncLock)
        {
            ThrowIfDisposed();
            if (State != ResponsesWebSocketConnectionState.Connected)
            {
                throw new InvalidOperationException("The Responses WebSocket is not connected.");
            }
        }
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, _lifetimeSource.Token);
        await _sendSemaphore.WaitAsync(linked.Token).ConfigureAwait(false);
        try
        {
            lock (_syncLock)
            {
                ThrowIfDisposed();
                if (State != ResponsesWebSocketConnectionState.Connected)
                {
                    throw new InvalidOperationException("The Responses WebSocket is not connected.");
                }
            }
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, linked.Token)
                .ConfigureAwait(false);
        }
        catch (WebSocketException)
        {
            _webSocket?.Abort();
            throw;
        }
        catch (OperationCanceledException)
        {
            // A canceled physical write has uncertain delivery and cannot safely be retried.
            _webSocket?.Abort();
            throw;
        }
        finally
        {
            _sendSemaphore.Release();
        }
    }

    /// <summary>Builds the WebSocket response.create envelope from request and WebSocket-specific options.</summary>
    private Task SendCreateAsync(CreateResponseOptions options, ResponsesWebSocketSendOptions webSocketOptions,
        CancellationToken cancellationToken)
    {
        Argument.AssertNotNull(options, nameof(options));
        if (options.StreamingEnabled.HasValue || options.BackgroundModeEnabled.HasValue)
        {
            throw new ArgumentException("StreamingEnabled and BackgroundModeEnabled must be unset for WebSocket requests.", nameof(options));
        }
        if (webSocketOptions?.StreamId == "")
        {
            throw new ArgumentException("StreamId must not be empty.", nameof(webSocketOptions));
        }
        if (webSocketOptions?.StreamId is string streamId)
        {
            if (streamId.Length > 256)
            {
                throw new ArgumentException("StreamId must be at most 256 characters.", nameof(webSocketOptions));
            }
            foreach (var character in streamId)
            {
                if (!(character >= 'a' && character <= 'z') && !(character >= 'A' && character <= 'Z')
                    && !(character >= '0' && character <= '9') && character != '_' && character != '-' && character != '.')
                {
                    throw new ArgumentException("StreamId may contain only ASCII letters, digits, underscores, hyphens, and periods.", nameof(webSocketOptions));
                }
            }
        }
        var body = ModelReaderWriter.Write(options, new ModelReaderWriterOptions("W"), OpenAIContext.Default);
        using var document = JsonDocument.Parse(body);
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("type", "response.create");
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (property.Name == "type" || property.Name == "stream_id" || property.Name == "generate"
                    || property.Name == "stream" || property.Name == "background")
                {
                    throw new ArgumentException("WebSocket envelope properties must be supplied through WebSocket send options.", nameof(options));
                }
                property.WriteTo(writer);
            }
            if (webSocketOptions?.StreamId != null)
            {
                writer.WriteString("stream_id", webSocketOptions.StreamId);
            }
            if (webSocketOptions?.GenerationEnabled is bool generate)
            {
                writer.WriteBoolean("generate", generate);
            }
            writer.WriteEndObject();
        }
        return SendJsonAsync(BinaryData.FromBytes(stream.ToArray()), cancellationToken);
    }

    /// <summary>Validates steering input and builds a response.steer command.</summary>
    private Task SendSteerAsync(string previousResponseId, IEnumerable<ResponseItem> inputItems,
        CancellationToken cancellationToken)
    {
        Argument.AssertNotNullOrEmpty(previousResponseId, nameof(previousResponseId));
        Argument.AssertNotNull(inputItems, nameof(inputItems));
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("type", "response.steer");
            writer.WriteString("previous_response_id", previousResponseId);
            writer.WriteStartArray("input");
            var count = 0;
            foreach (var item in inputItems)
            {
                Argument.AssertNotNull(item, nameof(inputItems));
                var data = ModelReaderWriter.Write(item, new ModelReaderWriterOptions("W"), OpenAIContext.Default);
                using var itemDocument = JsonDocument.Parse(data);
                var root = itemDocument.RootElement;
                foreach (var property in root.EnumerateObject())
                {
                    if (property.Name != "type" && property.Name != "role" && property.Name != "content")
                    {
                        throw new ArgumentException("Steering message items may contain only type, role, and content.", nameof(inputItems));
                    }
                }
                if (root.TryGetProperty("type", out var itemType) && itemType.GetString() != "message")
                {
                    throw new ArgumentException("Steering accepts only message items.", nameof(inputItems));
                }
                if (!root.TryGetProperty("role", out var role) || role.GetString() != "user")
                {
                    throw new ArgumentException("Steering accepts only user messages.", nameof(inputItems));
                }
                if (!root.TryGetProperty("content", out var content) || content.GetArrayLength() == 0)
                {
                    throw new ArgumentException("Steering user messages require nonempty content.", nameof(inputItems));
                }
                foreach (var part in content.EnumerateArray())
                {
                    var type = part.GetProperty("type").GetString();
                    if (type != "input_text" && type != "input_image" && type != "input_file")
                    {
                        throw new ArgumentException("Steering content must be text, images, or files.", nameof(inputItems));
                    }
                }
                root.WriteTo(writer);
                count++;
            }
            if (count == 0)
            {
                throw new ArgumentException("Steering input must not be empty.", nameof(inputItems));
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
        return SendJsonAsync(BinaryData.FromBytes(stream.ToArray()), cancellationToken);
    }

    private static CreateResponseOptions CreateOptions(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(inputItems, nameof(inputItems));
        var options = new CreateResponseOptions { Model = model, PreviousResponseId = previousResponseId };
        foreach (var item in inputItems)
        {
            Argument.AssertNotNull(item, nameof(inputItems));
            options.InputItems.Add(item);
        }
        return options;
    }

    /// <summary>Performs a bounded close handshake, then aborts the socket to ensure shutdown completes.</summary>
    private async Task CloseTransportAsync(CancellationToken cancellationToken)
    {
        // Closed/disposed clients may be closed repeatedly without touching released resources.
        lock (_syncLock)
        {
            if (_disposed || State == ResponsesWebSocketConnectionState.Closed)
            {
                return;
            }
        }
        using var operation = AcquireTransportOperation();
        lock (_syncLock)
        {
            if (_disposed || State == ResponsesWebSocketConnectionState.Closed)
            {
                return;
            }
            SetState(ResponsesWebSocketConnectionState.Closing);
        }
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(10));
        try
        {
            await _sendSemaphore.WaitAsync(timeout.Token).ConfigureAwait(false);
            try
            {
                if (_webSocket?.State == WebSocketState.Open || _webSocket?.State == WebSocketState.CloseReceived)
                {
                    await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Client closing", timeout.Token)
                        .ConfigureAwait(false);
                }
            }
            finally
            {
                _sendSemaphore.Release();
            }
            if (_receiveTask != null)
            {
                var deadline = Task.Delay(System.Threading.Timeout.Infinite, timeout.Token);
                var completed = await Task.WhenAny(_receiveTask, deadline).ConfigureAwait(false);
                if (completed != _receiveTask)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    throw new TimeoutException("The Responses WebSocket close handshake timed out.");
                }
                await _receiveTask.ConfigureAwait(false);
            }
        }
        finally
        {
            _lifetimeSource.Cancel();
            _webSocket?.Abort();
            lock (_syncLock)
            {
                SetState(_disposed ? ResponsesWebSocketConnectionState.Disposed : ResponsesWebSocketConnectionState.Closed);
            }
            _transportCompletion.TrySetResult(true);
        }
    }

    /// <summary>Cancels transport operations and aborts the socket without waiting for callbacks.</summary>
    private void StopTransport()
    {
        lock (_syncLock)
        {
            if (_transportStopped)
            {
                return;
            }
            _transportStopped = true;
            _disposed = true;
            _lifetimeSource.Cancel();
            _webSocket?.Abort();
            SetState(ResponsesWebSocketConnectionState.Disposed);
            if (_receiveTask is null)
            {
                _transportCompletion.TrySetResult(true);
            }
        }
        // Keep the lifetime reference alive until cancellation and socket abort have completed.
        ReleaseTransportOperation();
    }

    private void SetState(ResponsesWebSocketConnectionState state) => Volatile.Write(ref _connectionState, (int)state);

    /// <summary>Copies mutable settings and validates buffer limits and caller-supplied headers.</summary>
    private static ResponsesWebSocketClientOptions SnapshotOptions(ResponsesWebSocketClientOptions options)
    {
        if (options.MaxBufferedMessages <= 0 || options.MaxBufferedBytes < 0 || options.MaxMessageBytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Buffer limits must be positive or zero where zero disables a limit.");
        }
        var snapshot = new ResponsesWebSocketClientOptions
        {
            ShouldReconnect = options.ShouldReconnect,
            MaxBufferedMessages = options.MaxBufferedMessages,
            MaxBufferedBytes = options.MaxBufferedBytes,
            MaxMessageBytes = options.MaxMessageBytes,
        };
        foreach (var header in options.Headers)
        {
            if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Authentication headers are owned by the parent client.", nameof(options));
            }
            snapshot.Headers.Add(header);
        }
        return snapshot;
    }
}
