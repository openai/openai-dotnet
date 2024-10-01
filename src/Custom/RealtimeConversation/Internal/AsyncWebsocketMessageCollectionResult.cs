using OpenAI.Chat;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.RealtimeConversation;

internal partial class AsyncWebsocketMessageCollectionResult : AsyncCollectionResult<ClientResult>
{
    private readonly ClientWebSocket _clientWebSocket;
    private readonly CancellationToken _cancellationToken;

    public AsyncWebsocketMessageCollectionResult(
        ClientWebSocket clientWebSocket,
        CancellationToken cancellationToken)
    {
        Argument.AssertNotNull(clientWebSocket, nameof(clientWebSocket));

        _clientWebSocket = clientWebSocket;
        _cancellationToken = cancellationToken;
    }

    public override ContinuationToken GetContinuationToken(ClientResult page)

        // Continuation is not supported for SSE streams.
        => null;

    public override async IAsyncEnumerable<ClientResult> GetRawPagesAsync()
    {
        await using IAsyncEnumerator<ClientResult> enumerator = new AsyncWebsocketMessageResultEnumerator(_clientWebSocket, _cancellationToken);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield return enumerator.Current;
        }
    }

    protected override async IAsyncEnumerable<ClientResult> GetValuesFromPageAsync(ClientResult page)
    {
        await Task.FromResult(Task.CompletedTask);
        yield return page;
    }
}
