using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Realtime;

public partial class RealtimeSessionClient
{
    private readonly SemaphoreSlim _clientSendSemaphore = new(initialCount: 1, maxCount: 1);
    private readonly object _singleReceiveLock = new();
    private AsyncWebsocketMessageCollectionResult _receiveCollectionResult;

    /// <summary>
    /// Initializes an underlying <see cref="WebSocket"/> instance for communication with the /realtime endpoint and
    /// then connects to the service using this socket.
    /// </summary>
    protected internal virtual async Task ConnectAsync(string queryString = null, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
    {
        WebSocket?.Dispose();

        ClientWebSocket clientWebSocket = new();
        clientWebSocket.Options.AddSubProtocol("realtime");

        // Note: If we do not have a key credential here, it means that we do not have either an
        // OpenAI API key or an ephemeral client secret. At that point, auth is expected to be
        // handled manually by the user via custom headers or some other mechanism.
        if (_keyCredential != null)
        {
            _keyCredential.Deconstruct(out string dangerousCredential);
            clientWebSocket.Options.SetRequestHeader("Authorization", $"Bearer {dangerousCredential}");
        }

        if (headers is not null)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                clientWebSocket.Options.SetRequestHeader(header.Key, header.Value);
            }
        }

        Uri webSocketUri;

        if (string.IsNullOrEmpty(queryString))
        {
            webSocketUri = BuildSessionUri(_endpoint, _model, _intent);
        }
        else
        {
            UriBuilder uriBuilder = new(_endpoint);
            uriBuilder.Query = queryString;
            webSocketUri = uriBuilder.Uri;
        }

        await clientWebSocket.ConnectAsync(webSocketUri, cancellationToken).ConfigureAwait(false);

        WebSocket = clientWebSocket;
    }

    protected internal virtual void Connect(string queryString = null, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
    {
        ConnectAsync(queryString, headers, cancellationToken).Wait();
    }

    public virtual async Task SendCommandAsync(BinaryData data, RequestOptions options)
    {
        Argument.AssertNotNull(data, nameof(data));

        _parentClient?.RaiseOnSendingCommand(this, data);

        ArraySegment<byte> messageBytes = new(data.ToArray());

        CancellationToken cancellationToken = options?.CancellationToken ?? default;

        await _clientSendSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await WebSocket.SendAsync(
                messageBytes,
                WebSocketMessageType.Text, // TODO: extensibility for binary messages -- via "content"?
                endOfMessage: true,
                cancellationToken)
                    .ConfigureAwait(false);
        }
        finally
        {
            _clientSendSemaphore.Release();
        }
    }

    public virtual void SendCommand(BinaryData data, RequestOptions options)
    {
        // ClientWebSocket does **not** include a synchronous Send()
        SendCommandAsync(data, options).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public virtual async IAsyncEnumerable<ClientResult> ReceiveUpdatesAsync(RequestOptions options)
    {
        lock (_singleReceiveLock)
        {
            _receiveCollectionResult ??= new(WebSocket, options?.CancellationToken ?? default);
        }
        await foreach (ClientResult result in _receiveCollectionResult)
        {
            BinaryData incomingMessage = result?.GetRawResponse()?.Content;
            if (incomingMessage is not null)
            {
                _parentClient?.RaiseOnReceivingCommand(this, incomingMessage);
            }
            yield return result;
        }
    }

    public virtual IEnumerable<ClientResult> ReceiveUpdates(RequestOptions options)
    {
        throw new NotImplementedException();
    }

    private static Uri BuildSessionUri(Uri endpoint, string model, string intent)
    {
        Argument.AssertNotNull(endpoint, nameof(endpoint));

        ClientUriBuilder builder = new();

        builder.Reset(endpoint);

        if (!string.IsNullOrEmpty(model))
        {
            builder.AppendQuery("model", model, escape: true);
        }
        if (!string.IsNullOrEmpty(intent))
        {
            builder.AppendQuery("intent", intent, escape: true);
        }

        return builder.ToUri();
    }
}