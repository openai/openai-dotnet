using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

/// <summary>Connects to the Responses API over WebSocket and dispatches server messages to asynchronous handlers.</summary>
/// <remarks>
/// Subscribe to <see cref="MessageReceivedAsync"/> and <see cref="ErrorOccurredAsync"/> before calling
/// <see cref="ConnectAsync"/>. Each event registration has its own bounded queue; dispose the client to stop
/// receiving and cancel the tokens exposed by handler event arguments.
/// </remarks>
[Experimental("OPENAI001")]
public partial class ResponsesWebSocketClient : IDisposable, IAsyncDisposable
{
    private readonly ResponsesClient _responsesClient;
    private readonly ResponsesWebSocketClientOptions _options;
    private readonly Uri _websocketEndpoint;
    // Protects client lifetime, transport state, subscriptions, and buffered-byte accounting.
    private readonly object _syncLock = new object();
    private readonly List<MessageReceivedSubscription> _messageReceivedSubscribers = new List<MessageReceivedSubscription>();
    private readonly List<ErrorOccurredSubscription> _errorOccurredSubscribers = new List<ErrorOccurredSubscription>();
    private readonly List<Task> _retiredSubscriberCompletions = new List<Task>();
    private Task _disposeCompletion;
    private bool _disposed;

    /// <summary>Initializes a client for mocking.</summary>
    protected internal ResponsesWebSocketClient(ResponsesClient responsesClient, ResponsesWebSocketClientOptions options)
    {
        Argument.AssertNotNull(responsesClient, nameof(responsesClient));
        _responsesClient = responsesClient;
        _options = SnapshotOptions(options ?? new ResponsesWebSocketClientOptions());
        _websocketEndpoint = GetWebSocketEndpoint(responsesClient.Endpoint);
    }

    /// <summary>Receives every server message, including protocol errors and unknown event types.</summary>
    /// <remarks>
    /// Each event registration has an independent bounded queue. Registrations are dispatched concurrently;
    /// handlers within one multicast registration are also awaited concurrently. Exceptions from individual
    /// message handlers are reported to <see cref="ErrorOccurredAsync"/> without stopping other handlers or receiving.
    /// The callback's <see cref="ResponsesWebSocketMessageReceivedEventArgs.CancellationToken"/> is canceled when the client shuts down.
    /// </remarks>
    public event Func<ResponsesWebSocketMessageReceivedEventArgs, Task> MessageReceivedAsync
    {
        add
        {
            if (value is null)
            {
                return;
            }

            lock (_syncLock)
            {
                ThrowIfDisposed();
                MessageReceivedSubscription subscription = null;
                var subscriber = new MessageReceivedSubscriber<ResponsesWebSocketMessageReceivedEventArgs>(
                    message =>
                    {
                        var bytes = message.Message?.RawData?.ToMemory().Length ?? 0;

                        lock (_syncLock)
                        {
                            ReleaseBufferedBytes(subscription, bytes);
                        }

                        return InvokeMessageHandlersAsync(value, message);
                    }, _options.MaxBufferedMessages);
                subscription = new MessageReceivedSubscription(value, subscriber);
                _messageReceivedSubscribers.Add(subscription);
                _ = ObserveSubscriberAsync(subscriber.Completion, false);
            }
        }
        remove
        {
            if (value is null)
            {
                return;
            }

            lock (_syncLock)
            {
                if (_disposed)
                {
                    return;
                }

                for (var i = _messageReceivedSubscribers.Count - 1; i >= 0; i--)
                {
                    if (_messageReceivedSubscribers[i].Handler == value)
                    {
                        var subscriber = _messageReceivedSubscribers[i].Subscriber;
                        ReleaseBufferedBytes(_messageReceivedSubscribers[i], _messageReceivedSubscribers[i].PendingBytes);
                        _messageReceivedSubscribers.RemoveAt(i);
                        subscriber.Dispose();
                        _retiredSubscriberCompletions.Add(subscriber.Completion);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>Reports local transport, buffering, and callback failures. Protocol error messages use MessageReceivedAsync.</summary>
    /// <remarks>
    /// Each event registration has an independent bounded queue. Registrations are dispatched concurrently.
    /// A failing error handler terminates the connection and is not reported recursively.
    /// The callback's <see cref="ResponsesWebSocketErrorEventArgs.CancellationToken"/> is canceled when the client shuts down.
    /// </remarks>
    public event Func<ResponsesWebSocketErrorEventArgs, Task> ErrorOccurredAsync
    {
        add
        {
            if (value is null)
            {
                return;
            }

            lock (_syncLock)
            {
                ThrowIfDisposed();
                var subscriber = new MessageReceivedSubscriber<ResponsesWebSocketErrorEventArgs>(
                    error => InvokeErrorHandlersAsync(value, error), _options.MaxBufferedMessages);
                _errorOccurredSubscribers.Add(new ErrorOccurredSubscription(value, subscriber));
                _ = ObserveSubscriberAsync(subscriber.Completion, true);
            }
        }
        remove
        {
            if (value is null)
            {
                return;
            }

            lock (_syncLock)
            {
                if (_disposed)
                {
                    return;
                }

                for (var i = _errorOccurredSubscribers.Count - 1; i >= 0; i--)
                {
                    if (_errorOccurredSubscribers[i].Handler == value)
                    {
                        var subscriber = _errorOccurredSubscribers[i].Subscriber;
                        _errorOccurredSubscribers.RemoveAt(i);
                        subscriber.Dispose();
                        _retiredSubscriberCompletions.Add(subscriber.Completion);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>The current logical connection state.</summary>
    public virtual ResponsesWebSocketConnectionState State => (ResponsesWebSocketConnectionState)Volatile.Read(ref _connectionState);

    /// <summary>Opens the initial connection and starts dispatching messages. If already connected, returns immediately.</summary>
    /// <remarks>
    /// Requires both message and error subscribers before opening the connection.
    /// Cancellation bounds opening, not the connected lifetime.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// No handler is subscribed to <see cref="MessageReceivedAsync"/> or <see cref="ErrorOccurredAsync"/>.
    /// </exception>
    public virtual Task ConnectAsync(CancellationToken cancellationToken = default)
        => ConnectTransportAsync(cancellationToken);

    /// <summary>Creates a response on the default stream using the existing Responses request model.</summary>
    /// <remarks>
    /// Adds the response.create discriminator internally. StreamingEnabled and BackgroundModeEnabled must be unset.
    /// Completion means a local write, not server acceptance or response completion.
    /// Rejects sends while disconnected or reconnecting. Requests are never automatically replayed.
    /// </remarks>
    public virtual Task CreateAsync(CreateResponseOptions options, CancellationToken cancellationToken = default)
        => CreateAsync(options, null, cancellationToken);

    /// <summary>Creates a response with WebSocket-specific stream or warmup settings.</summary>
    /// <remarks>Uses the same request validation and delivery semantics as the default-stream overload.</remarks>
    public virtual Task CreateAsync(
        CreateResponseOptions options,
        ResponsesWebSocketSendOptions webSocketOptions,
        CancellationToken cancellationToken = default)
        => SendCreateAsync(options, webSocketOptions, cancellationToken);

    /// <summary>Creates a response on the default stream from a model and input items, optionally continuing a previous response.</summary>
    /// <remarks>Uses the same request validation and delivery semantics as the CreateResponseOptions overload.</remarks>
    public virtual Task CreateAsync(
        string model,
        IEnumerable<ResponseItem> inputItems,
        string previousResponseId = null,
        CancellationToken cancellationToken = default)
        => CreateAsync(CreateOptions(model, inputItems, previousResponseId), cancellationToken);

    /// <summary>Creates a response from a model and input items with WebSocket-specific stream or warmup settings.</summary>
    /// <remarks>Uses the same request validation and delivery semantics as the CreateResponseOptions overload.</remarks>
    public virtual Task CreateAsync(
        string model,
        IEnumerable<ResponseItem> inputItems,
        ResponsesWebSocketSendOptions webSocketOptions,
        string previousResponseId = null,
        CancellationToken cancellationToken = default)
        => CreateAsync(CreateOptions(model, inputItems, previousResponseId), webSocketOptions, cancellationToken);

    /// <summary>Creates a response on the default stream from a model and user text, optionally continuing a previous response.</summary>
    /// <remarks>Uses the same request validation and delivery semantics as the CreateResponseOptions overload.</remarks>
    public virtual Task CreateAsync(
        string model,
        string userInputText,
        string previousResponseId = null,
        CancellationToken cancellationToken = default)
        => CreateAsync(model, new[] { ResponseItem.CreateUserMessageItem(userInputText) }, previousResponseId, cancellationToken);

    /// <summary>Creates a response from a model and user text with WebSocket-specific stream or warmup settings.</summary>
    /// <remarks>Uses the same request validation and delivery semantics as the CreateResponseOptions overload.</remarks>
    public virtual Task CreateAsync(
        string model,
        string userInputText,
        ResponsesWebSocketSendOptions webSocketOptions,
        string previousResponseId = null,
        CancellationToken cancellationToken = default)
        => CreateAsync(model, new[] { ResponseItem.CreateUserMessageItem(userInputText) }, webSocketOptions, previousResponseId, cancellationToken);

    /// <summary>Sends one UTF-8 JSON object, including fields or command types not yet modeled by the SDK.</summary>
    /// <remarks>A failed or canceled physical write has uncertain delivery. It must not be retried blindly.</remarks>
    public virtual Task CreateAsync(BinaryData command, CancellationToken cancellationToken = default)
        => SendJsonAsync(command, cancellationToken);

    /// <summary>Queues user input to steer a response on this connection.</summary>
    /// <remarks>
    /// The response identifier determines the stream; no stream identifier is supplied separately.
    /// Input must contain nonempty user messages with only text, image, or file content.
    /// Message items may contain only type, role, and content; identifiers and status are not supported.
    /// Adds the response.steer discriminator internally. Completion means a local write, not steering acceptance.
    /// Rejects sends while disconnected or reconnecting. Requests are never automatically replayed.
    /// </remarks>
    public virtual Task SteerAsync(
        string previousResponseId,
        IEnumerable<ResponseItem> inputItems,
        CancellationToken cancellationToken = default)
        => SendSteerAsync(previousResponseId, inputItems, cancellationToken);

    /// <summary>Queues user text to steer a response on this connection.</summary>
    /// <remarks>Uses the same targeting and delivery semantics as the input-item overload.</remarks>
    public virtual Task SteerAsync(
        string previousResponseId,
        string userInputText,
        CancellationToken cancellationToken = default)
        => SteerAsync(previousResponseId, new[] { ResponseItem.CreateUserMessageItem(userInputText) }, cancellationToken);

    /// <summary>Stops reconnecting and closes gracefully within an internal timeout. Repeated calls are safe.</summary>
    /// <remarks>
    /// Cancellation interrupts graceful shutdown but still terminates the connection. Does not cancel a server response.
    /// If called from a message handler while receive is blocked by a full subscriber queue, it may time out.
    /// </remarks>
    public virtual Task CloseAsync(CancellationToken cancellationToken = default)
        => CloseTransportAsync(cancellationToken);

    /// <summary>Immediately terminates the connection and releases owned resources without waiting for callbacks.</summary>
    public virtual void Dispose()
    {
        StopTransport();
        _ = StopMessageReceivedSubscribers();
    }

    /// <summary>Closes the connection and awaits processing shutdown before releasing owned resources.</summary>
    /// <remarks>
    /// A message or error handler must not await this method, because disposal waits for that handler to finish.
    /// Call <see cref="Dispose"/> from a handler to request shutdown without awaiting callback cleanup.
    /// </remarks>
    public virtual async ValueTask DisposeAsync()
    {
        StopTransport();
        try
        {
            if (_receiveTask != null)
            {
                await _receiveTask.ConfigureAwait(false);
            }
        }
        finally
        {
            try
            {
                await StopMessageReceivedSubscribers().ConfigureAwait(false);
            }
            finally
            {
                await _transportResourcesDisposed.Task.ConfigureAwait(false);
            }
        }
    }

    private Task StopMessageReceivedSubscribers()
    {
        lock (_syncLock)
        {
            if (_disposeCompletion != null)
            {
                return _disposeCompletion;
            }

            _disposed = true;
            foreach (MessageReceivedSubscription subscription in _messageReceivedSubscribers)
            {
                ReleaseBufferedBytes(subscription, subscription.PendingBytes);
                subscription.Subscriber.Dispose();
                _retiredSubscriberCompletions.Add(subscription.Subscriber.Completion);
            }
            _messageReceivedSubscribers.Clear();
            foreach (ErrorOccurredSubscription subscription in _errorOccurredSubscribers)
            {
                subscription.Subscriber.Dispose();
                _retiredSubscriberCompletions.Add(subscription.Subscriber.Completion);
            }
            _errorOccurredSubscribers.Clear();
            _disposeCompletion = Task.WhenAll(_retiredSubscriberCompletions);
            return _disposeCompletion;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ResponsesWebSocketClient));
        }
    }

    internal Task DispatchMessageAsync(
        ResponsesWebSocketMessageReceivedEventArgs message,
        CancellationToken cancellationToken = default)
    {
        Task[] enqueues;
        lock (_syncLock)
        {
            ThrowIfDisposed();
            enqueues = new Task[_messageReceivedSubscribers.Count];
            for (int i = 0; i < _messageReceivedSubscribers.Count; i++)
            {
                enqueues[i] = EnqueueMessageAsync(_messageReceivedSubscribers[i], message, cancellationToken);
            }
        }

        return Task.WhenAll(enqueues);
    }

    internal Task DispatchErrorAsync(
        ResponsesWebSocketErrorEventArgs error,
        CancellationToken cancellationToken = default)
    {
        Task[] enqueues;
        lock (_syncLock)
        {
            ThrowIfDisposed();
            enqueues = new Task[_errorOccurredSubscribers.Count];
            for (int i = 0; i < _errorOccurredSubscribers.Count; i++)
            {
                enqueues[i] = _errorOccurredSubscribers[i].Subscriber.EnqueueAsync(error, cancellationToken);
            }
        }

        return Task.WhenAll(enqueues);
    }

    private Task InvokeMessageHandlersAsync(
        Func<ResponsesWebSocketMessageReceivedEventArgs, Task> handlers,
        ResponsesWebSocketMessageReceivedEventArgs message)
    {
        return InvokeHandlersAsync(handlers, message, message.CancellationToken, exception =>
            _lifetimeSource.IsCancellationRequested
                ? Task.CompletedTask
                : DispatchErrorAsync(new ResponsesWebSocketErrorEventArgs(exception, false, _lifetimeSource.Token), _lifetimeSource.Token));
    }

    private Task InvokeErrorHandlersAsync(
        Func<ResponsesWebSocketErrorEventArgs, Task> handlers,
        ResponsesWebSocketErrorEventArgs error)
    {
        return InvokeHandlersAsync(handlers, error, error.CancellationToken);
    }

    private static Task InvokeHandlersAsync<TEventArgs>(
        Func<TEventArgs, Task> handlers,
        TEventArgs eventArgs,
        CancellationToken cancellationToken,
        Func<Exception, Task> onHandlerException = null)
    {
        var invocationList = handlers.GetInvocationList();
        var invocations = new Task[invocationList.Length];
        for (var i = 0; i < invocationList.Length; i++)
        {
            var handler = (Func<TEventArgs, Task>)invocationList[i];
            invocations[i] = InvokeHandlerAsync(handler, eventArgs, cancellationToken, onHandlerException);
        }

        return Task.WhenAll(invocations);
    }

    private static async Task InvokeHandlerAsync<TEventArgs>(
        Func<TEventArgs, Task> handler,
        TEventArgs eventArgs,
        CancellationToken cancellationToken,
        Func<Exception, Task> onHandlerException)
    {
        try
        {
            await handler(eventArgs).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // A handler honoring shutdown cancellation is not a callback failure.
        }
        catch (Exception exception) when (onHandlerException != null)
        {
            await onHandlerException(exception).ConfigureAwait(false);
        }
    }

    private static Uri GetWebSocketEndpoint(Uri endpoint)
    {
        var uriBuilder = new UriBuilder(OpenAIClientUtilities.GetEndpoint(endpoint));
        uriBuilder.Scheme = uriBuilder.Scheme.ToLowerInvariant() switch
        {
            "http" => "ws",
            "https" => "wss",
            _ => uriBuilder.Scheme
        };
        string path = uriBuilder.Path.TrimEnd('/');
        if (!path.EndsWith("/responses", StringComparison.Ordinal))
        {
            path += "/responses";
        }
        uriBuilder.Path = path;

        return uriBuilder.Uri;
    }

    private sealed class MessageReceivedSubscription
    {
        public Func<ResponsesWebSocketMessageReceivedEventArgs, Task> Handler { get; }
        public MessageReceivedSubscriber<ResponsesWebSocketMessageReceivedEventArgs> Subscriber { get; }
        public long PendingBytes { get; set; }

        public MessageReceivedSubscription(
            Func<ResponsesWebSocketMessageReceivedEventArgs, Task> handler,
            MessageReceivedSubscriber<ResponsesWebSocketMessageReceivedEventArgs> subscriber)
        {
            Handler = handler;
            Subscriber = subscriber;
        }
    }

    private sealed class ErrorOccurredSubscription
    {
        public Func<ResponsesWebSocketErrorEventArgs, Task> Handler { get; }
        public MessageReceivedSubscriber<ResponsesWebSocketErrorEventArgs> Subscriber { get; }

        public ErrorOccurredSubscription(
            Func<ResponsesWebSocketErrorEventArgs, Task> handler,
            MessageReceivedSubscriber<ResponsesWebSocketErrorEventArgs> subscriber)
        {
            Handler = handler;
            Subscriber = subscriber;
        }
    }

}
