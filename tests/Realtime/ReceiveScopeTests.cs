using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Realtime;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class ReceiveScopeTests
{
    private sealed class Session(WebSocket socket) : RealtimeSessionClient(
        new ApiKeyCredential("synthetic-key"), new Uri("https://example.invalid"), "synthetic-model", null, null)
    {
        public Session Initialize() { WebSocket = socket; return this; }
        public void ReplaceSocket(WebSocket value) => WebSocket = value;
    }

    private sealed class Socket(string message) : WebSocket
    {
        public List<CancellationToken> Tokens { get; } = [];
        public override WebSocketCloseStatus? CloseStatus => null;
        public override string CloseStatusDescription => null;
        public override WebSocketState State => WebSocketState.Open;
        public override string SubProtocol => null;
        public override void Abort() { }
        public override void Dispose() { }
        public override Task CloseAsync(WebSocketCloseStatus status, string description, CancellationToken token) => Task.CompletedTask;
        public override Task CloseOutputAsync(WebSocketCloseStatus status, string description, CancellationToken token) => Task.CompletedTask;
        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType type, bool end, CancellationToken token) => Task.CompletedTask;
        public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token)
        {
            Tokens.Add(token);
            token.ThrowIfCancellationRequested();
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            bytes.CopyTo(buffer.Array, buffer.Offset);
            return Task.FromResult(new WebSocketReceiveResult(bytes.Length, WebSocketMessageType.Text, true));
        }
    }

    private static async Task<string> ReadOne(Session session, CancellationToken token)
    {
        await using IAsyncEnumerator<ClientResult> reader = session.ReceiveUpdatesAsync(new RequestOptions { CancellationToken = token }).GetAsyncEnumerator();
        Assert.That(await reader.MoveNextAsync(), Is.True);
        return reader.Current.GetRawResponse().Content.ToString();
    }

    [Test]
    public async Task EachReceiveCallUsesItsOwnCancellationToken()
    {
        using CancellationTokenSource first = new();
        using CancellationTokenSource second = new();
        using Socket socket = new("{\"synthetic\":true}");
        using Session session = new Session(socket).Initialize();
        await ReadOne(session, first.Token);
        await ReadOne(session, second.Token);
        Assert.That(socket.Tokens, Is.EqualTo(new[] { first.Token, second.Token }));
    }

    [Test]
    public async Task CancellingAnEarlierCompletedEnumerationDoesNotCancelALaterOne()
    {
        using CancellationTokenSource first = new();
        using Socket socket = new("{\"synthetic\":true}");
        using Session session = new Session(socket).Initialize();
        await ReadOne(session, first.Token);
        first.Cancel();
        Assert.That(await ReadOne(session, CancellationToken.None), Is.EqualTo("{\"synthetic\":true}"));
    }

    [Test]
    public async Task LaterReceiveCallHonorsAnAlreadyCancelledToken()
    {
        using CancellationTokenSource cancelled = new();
        using Socket socket = new("{}");
        using Session session = new Session(socket).Initialize();
        await ReadOne(session, CancellationToken.None);
        cancelled.Cancel();
        Assert.ThrowsAsync<OperationCanceledException>(async () => await ReadOne(session, cancelled.Token));
    }

    [Test]
    public async Task LaterReceiveCallUsesTheCurrentSocket()
    {
        using Socket first = new("first");
        using Socket second = new("second");
        using Session session = new Session(first).Initialize();
        Assert.That(await ReadOne(session, CancellationToken.None), Is.EqualTo("first"));
        session.ReplaceSocket(second);
        Assert.That(await ReadOne(session, CancellationToken.None), Is.EqualTo("second"));
        Assert.That(first.Tokens.Count, Is.EqualTo(1));
        Assert.That(second.Tokens.Count, Is.EqualTo(1));
    }

    [Test]
    public void FirstReceiveCallStillHonorsCancellation()
    {
        using CancellationTokenSource cancelled = new();
        cancelled.Cancel();
        using Socket socket = new("{}");
        using Session session = new Session(socket).Initialize();
        Assert.ThrowsAsync<OperationCanceledException>(async () => await ReadOne(session, cancelled.Token));
    }

    [Test]
    public async Task RepeatedCallsWithTheSameTokenRemainUsable()
    {
        using CancellationTokenSource source = new();
        using Socket socket = new("{}");
        using Session session = new Session(socket).Initialize();
        for (int count = 0; count < 3; count++) Assert.That(await ReadOne(session, source.Token), Is.EqualTo("{}"));
        Assert.That(socket.Tokens, Is.EqualTo(new[] { source.Token, source.Token, source.Token }));
    }
}
