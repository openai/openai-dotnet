using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.RealtimeConversation;

public partial class RealtimeConversationSession
{
    protected ClientWebSocket _clientWebSocket;
    private readonly SemaphoreSlim _clientSendSemaphore = new(initialCount: 1, maxCount: 1);
    private readonly object _singleReceiveLock = new();
    private AsyncWebsocketMessageCollectionResult _receiveCollectionResult;

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual async Task ConnectAsync(RequestOptions options)
    {
        _clientWebSocket.Options.AddSubProtocol("realtime");
        await _clientWebSocket.ConnectAsync(_endpoint, options?.CancellationToken ?? default)
            .ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void Connect(RequestOptions options)
    {
        ConnectAsync(options).Wait();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task SendCommandAsync(BinaryData data, RequestOptions options)
    {
        Argument.AssertNotNull(data, nameof(data));

        _parentClient?.RaiseOnSendingCommand(this, data);

        ArraySegment<byte> messageBytes = new(data.ToArray());

        CancellationToken cancellationToken = options?.CancellationToken ?? default;

        await _clientSendSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _clientWebSocket.SendAsync(
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendCommand(BinaryData data, RequestOptions options)
    {
        // ClientWebSocket does **not** include a synchronous Send()
        SendCommandAsync(data, options).Wait();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async IAsyncEnumerable<ClientResult> ReceiveUpdatesAsync(RequestOptions options)
    {
        lock (_singleReceiveLock)
        {
            _receiveCollectionResult ??= new(_clientWebSocket, options?.CancellationToken ?? default);
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual IEnumerable<ClientResult> ReceiveUpdates(RequestOptions options)
    {
        throw new NotImplementedException();
    }
}