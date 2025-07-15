using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Realtime;

public partial class RealtimeSession
{
    private readonly SemaphoreSlim _clientSendSemaphore = new(initialCount: 1, maxCount: 1);
    private readonly object _singleReceiveLock = new();
    private AsyncWebsocketMessageCollectionResult _receiveCollectionResult;

    /// <summary>
    /// Initializes an underlying <see cref="WebSocket"/> instance for communication with the /realtime endpoint and
    /// then connects to the service using this socket.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    protected internal virtual async Task ConnectAsync(RequestOptions options)
    {
        WebSocket?.Dispose();
        _credential.Deconstruct(out string dangerousCredential);
        ClientWebSocket clientWebSocket = new();
        clientWebSocket.Options.AddSubProtocol("realtime");
        clientWebSocket.Options.SetRequestHeader("openai-beta", $"realtime=v1");
        clientWebSocket.Options.SetRequestHeader("Authorization", $"Bearer {dangerousCredential}");

        await clientWebSocket.ConnectAsync(_endpoint, options?.CancellationToken ?? default)
            .ConfigureAwait(false);

        WebSocket = clientWebSocket;
    }

    protected internal virtual void Connect(RequestOptions options)
    {
        ConnectAsync(options).Wait();
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
}