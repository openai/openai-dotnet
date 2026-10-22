using Moq;
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

[Category("Smoke")]
public class WebsocketCloseFrameTests
{
    private sealed class FixtureSession : RealtimeSessionClient
    {
        public FixtureSession(WebSocket socket)
            : base(new ApiKeyCredential("synthetic-key"), new Uri("https://example.invalid"), "synthetic-model", null, null)
        {
            WebSocket = socket;
        }
    }

    [TestCase(false, null)]
    [TestCase(true, null)]
    [TestCase(false, WebSocketCloseStatus.NormalClosure)]
    [TestCase(true, WebSocketCloseStatus.NormalClosure)]
    [TestCase(false, WebSocketCloseStatus.Empty)]
    [TestCase(true, WebSocketCloseStatus.Empty)]
    public async Task CloseFramesEndEnumerationWithoutYieldingPartialMessages(bool partial, WebSocketCloseStatus? status)
    {
        var frames = new Queue<(byte[] Bytes, WebSocketReceiveResult Result)>();
        byte[] complete = Encoding.UTF8.GetBytes("{\"type\":\"synthetic\"}");
        frames.Enqueue((complete, new(complete.Length, WebSocketMessageType.Text, true)));
        if (partial)
        {
            byte[] prefix = Encoding.UTF8.GetBytes("{\"partial\":");
            frames.Enqueue((prefix, new(prefix.Length, WebSocketMessageType.Text, false)));
        }
        frames.Enqueue(([], new(0, WebSocketMessageType.Close, true, status, null)));
        int receives = 0;
        Mock<WebSocket> socket = new();
        socket.Setup(value => value.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Returns((ArraySegment<byte> buffer, CancellationToken cancellation) =>
            {
                cancellation.ThrowIfCancellationRequested();
                receives++;
                Assert.That(frames.Count, Is.GreaterThan(0), "No receive should occur after a close frame");
                var frame = frames.Dequeue();
                frame.Bytes.CopyTo(buffer.Array!, buffer.Offset);
                return Task.FromResult(frame.Result);
            });
        using var session = new FixtureSession(socket.Object);
        var collection = session.ReceiveUpdatesAsync(new RequestOptions());
        var contents = new List<string>();
        await foreach (ClientResult result in collection)
        {
            using var response = result.GetRawResponse();
            contents.Add(response.Content.ToString());
        }
        Assert.That(contents, Is.EqualTo(new[] { Encoding.UTF8.GetString(complete) }));
        Assert.That(receives, Is.EqualTo(partial ? 3 : 2));
        Assert.That(frames.Count, Is.Zero);
        socket.Verify(value => value.Dispose(), Times.Never, "The collection does not own the socket");
    }

    [Test]
    public async Task EmptyTextMessageIsStillADataMessage()
    {
        var frames = new Queue<WebSocketReceiveResult>(new[] {
            new WebSocketReceiveResult(0, WebSocketMessageType.Text, true),
            new WebSocketReceiveResult(0, WebSocketMessageType.Close, true, WebSocketCloseStatus.NormalClosure, null),
        });
        Mock<WebSocket> socket = new();
        socket.Setup(value => value.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(frames.Dequeue()));
        using var session = new FixtureSession(socket.Object);
        var collection = session.ReceiveUpdatesAsync(new RequestOptions());
        var messages = new List<string>();
        await foreach (ClientResult result in collection)
        {
            using var response = result.GetRawResponse();
            messages.Add(response.Content.ToString());
        }
        Assert.That(messages, Is.EqualTo(new[] { "" }));
    }
}
