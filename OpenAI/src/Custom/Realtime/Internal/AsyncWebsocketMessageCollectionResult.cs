using System.ClientModel;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Realtime;

internal partial class AsyncWebsocketMessageCollectionResult : AsyncCollectionResult<ClientResult>
{
    private readonly WebSocket _webSocket;
    private readonly CancellationToken _cancellationToken;

    public AsyncWebsocketMessageCollectionResult(
        WebSocket webSocket,
        CancellationToken cancellationToken)
    {
        Argument.AssertNotNull(webSocket, nameof(webSocket));

        _webSocket = webSocket;
        _cancellationToken = cancellationToken;
    }

    public override ContinuationToken GetContinuationToken(ClientResult page)

        // Continuation is not supported for SSE streams.
        => null;

    public override async IAsyncEnumerable<ClientResult> GetRawPagesAsync()
    {
        await using IAsyncEnumerator<ClientResult> enumerator = new AsyncWebsocketMessageResultEnumerator(_webSocket, _cancellationToken);
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
