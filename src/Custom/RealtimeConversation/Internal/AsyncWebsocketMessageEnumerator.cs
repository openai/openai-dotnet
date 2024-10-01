using System;
using System.Buffers;
using System.ClientModel;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.RealtimeConversation;

internal partial class AsyncWebsocketMessageResultEnumerator : IAsyncEnumerator<ClientResult>
{
    public ClientResult Current { get; private set; }
    private readonly CancellationToken _cancellationToken;
    private readonly ClientWebSocket _clientWebSocket;
    private readonly byte[] _receiveBuffer;

    public AsyncWebsocketMessageResultEnumerator(ClientWebSocket clientWebSocket, CancellationToken cancellationToken)
    {
        _clientWebSocket = clientWebSocket;
        // 18K buffer size based on traffic observation; the connection will appropriately negotiate and use
        // fragmented messages if the buffer size is inadequate.
        _receiveBuffer = ArrayPool<byte>.Shared.Rent(1024 * 18);
        _cancellationToken = cancellationToken;
    }

    public ValueTask DisposeAsync()
    {
        _clientWebSocket?.Dispose();
        return new ValueTask(Task.CompletedTask);
    }

    public async ValueTask<bool> MoveNextAsync()
    {
        WebsocketPipelineResponse websocketPipelineResponse = new();
        for (int partialMessageCount = 1; !websocketPipelineResponse.IsComplete; partialMessageCount++)
        {
            WebSocketReceiveResult receiveResult = await _clientWebSocket.ReceiveAsync(new(_receiveBuffer), _cancellationToken);
            if (receiveResult.CloseStatus.HasValue)
            {
                Current = null;
                return false;
            }
            ReadOnlyMemory<byte> receivedBytes = _receiveBuffer.AsMemory(0, receiveResult.Count);
            BinaryData receivedData = BinaryData.FromBytes(receivedBytes);

            websocketPipelineResponse.IngestReceivedResult(receiveResult, receivedData);
        }

        Current = ClientResult.FromResponse(websocketPipelineResponse);
        return true;
    }
}