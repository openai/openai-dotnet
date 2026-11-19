using NUnit.Framework;
using OpenAI.Examples;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses.WebSockets;

#pragma warning disable OPENAI001

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class ResponsesWebSocketExampleTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [TestCase(true)]
    [TestCase(false)]
    public async Task SteeringExampleWaitsForContinuation(bool initialResponseIsSteered)
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        using CancellationTokenSource timeout = new(Timeout);
        Task<ResponseResult> response = ResponsesWebSocketExamples.CreateSteeredResponseAsync(client, timeout.Token);

        await WaitForSentAsync(socket, 1, timeout.Token);
        using JsonDocument create = JsonDocument.Parse(socket.Sent.First());
        Assert.That(create.RootElement.GetProperty("model").GetString(), Is.EqualTo("gpt-6-astra"));

        socket.Add(ResponseEvent("response.created", "resp_initial", "in_progress"), true);
        await WaitForSentAsync(socket, 2, timeout.Token);
        using JsonDocument steer = JsonDocument.Parse(socket.Sent.Last());
        Assert.That(steer.RootElement.GetProperty("type").GetString(), Is.EqualTo("response.steer"));
        Assert.That(steer.RootElement.GetProperty("previous_response_id").GetString(), Is.EqualTo("resp_initial"));

        socket.Add("""{"type":"response.steer.accepted","sequence_number":2,"steer":{"id":"steer_1","previous_response_id":"resp_initial"}}""", true);
        socket.Add(initialResponseIsSteered
            ? ResponseEvent("response.incomplete", "resp_initial", "incomplete", ""","incomplete_details":{"reason":"steered"}""")
            : ResponseEvent("response.completed", "resp_initial", "completed"), true);
        socket.Add(ResponseEvent("response.created", "resp_continuation", "in_progress"), true);
        socket.Add(ResponseEvent("response.completed", "resp_continuation", "completed"), true);

        ResponseResult result = await response.WaitAsync(timeout.Token);
        Assert.That(result.Id, Is.EqualTo("resp_continuation"));
        Assert.That(socket.Sent, Has.Count.EqualTo(2));
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task SteeringExampleReportsFailures(bool steeringRejected)
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        using CancellationTokenSource timeout = new(Timeout);
        Task<ResponseResult> response = ResponsesWebSocketExamples.CreateSteeredResponseAsync(client, timeout.Token);

        await WaitForSentAsync(socket, 1, timeout.Token);
        socket.Add(ResponseEvent("response.created", "resp_initial", "in_progress"), true);
        await WaitForSentAsync(socket, 2, timeout.Token);
        socket.Add(steeringRejected
            ? """{"type":"response.steer.failed","sequence_number":2,"steer":{"previous_response_id":"resp_initial"},"error":{"code":"steering_not_supported","message":"Unsupported model"}}"""
            : ResponseEvent("response.incomplete", "resp_initial", "incomplete", ""","incomplete_details":{"reason":"max_output_tokens"}"""), true);

        InvalidOperationException error = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await response.WaitAsync(timeout.Token));
        Assert.That(error.Message, Does.Contain(steeringRejected ? "rejected" : "before completion"));
    }

    [Test]
    public async Task SteeringExampleReportsTimeoutState()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        using CancellationTokenSource timeout = new(Timeout);
        Task<ResponseResult> response = ResponsesWebSocketExamples.CreateSteeredResponseAsync(client, timeout.Token);

        await WaitForSentAsync(socket, 1, timeout.Token);
        socket.Add(ResponseEvent("response.created", "resp_initial", "in_progress"), true);
        await WaitForSentAsync(socket, 2, timeout.Token);
        timeout.Cancel();

        TimeoutException error = await Assert.ThrowsAsync<TimeoutException>(async () => await response);
        Assert.That(error.Message, Does.Contain("resp_initial").And.Contain("continuation: none"));
    }

    [Test]
    public async Task LongRunningExampleReusesOneConnectionForFiftyResponses()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket, new ResponsesWebSocketClientOptions { ShouldReconnect = false });
        using CancellationTokenSource timeout = new(Timeout);
        Task session = ResponsesWebSocketExamples.RunLongRunningSessionAsync(
            client, 50, TimeSpan.Zero, timeout.Token);

        for (int index = 1; index <= 50; index++)
        {
            await WaitForSentAsync(socket, index, timeout.Token);
            socket.Add(ResponseEvent("response.completed", $"resp_{index}", "completed"), true);
        }

        await session.WaitAsync(timeout.Token);
        Assert.That(socket.Sent, Has.Count.EqualTo(50));
        Assert.That(client.Connections, Is.EqualTo(1));
        Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Connected));
    }

    [TestCase("protocol")]
    [TestCase("close")]
    [TestCase("transport")]
    public async Task LongRunningExampleStopsWhenConnectionClosesOrServerRejectsRequest(string failure)
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket, new ResponsesWebSocketClientOptions { ShouldReconnect = false });
        using CancellationTokenSource timeout = new(Timeout);
        Task session = ResponsesWebSocketExamples.RunLongRunningSessionAsync(
            client, 50, TimeSpan.Zero, timeout.Token);

        for (int index = 1; index <= 3; index++)
        {
            await WaitForSentAsync(socket, index, timeout.Token);
            socket.Add(ResponseEvent("response.completed", $"resp_{index}", "completed"), true);
        }

        await WaitForSentAsync(socket, 4, timeout.Token);
        if (failure == "protocol")
        {
            socket.Add("""{"type":"error","error":{"code":"rate_limit_exceeded","message":"Too many requests"}}""", true);
            ChannelClosedException error = await Assert.ThrowsAsync<ChannelClosedException>(
                async () => await session.WaitAsync(timeout.Token));
            Assert.That(error.InnerException?.Message, Does.Contain("Too many requests"));
        }
        else if (failure == "transport")
        {
            socket.Fail(new WebSocketException("Connection lost"));
            await Assert.ThrowsAsync<WebSocketException>(
                async () => await session.WaitAsync(timeout.Token));
        }
        else
        {
            socket.CloseFromServer();
            InvalidOperationException error = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await session.WaitAsync(timeout.Token));
            Assert.That(error.Message, Does.Contain("response 4/50"));
        }

        Assert.That(socket.Sent, Has.Count.EqualTo(4));
        Assert.That(client.Connections, Is.EqualTo(1));
    }

    [Test]
    public async Task ParallelStreamsExampleRoutesOutOfOrderResponses()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        using CancellationTokenSource timeout = new(Timeout);
        Task<ResponseResult[]> responses = ResponsesWebSocketExamples.CreateParallelResponsesAsync(client, timeout.Token);

        await WaitForSentAsync(socket, 2, timeout.Token);
        string[] sent = socket.Sent.ToArray();
        using JsonDocument travel = JsonDocument.Parse(sent[0]);
        using JsonDocument food = JsonDocument.Parse(sent[1]);
        Assert.That(travel.RootElement.GetProperty("stream_id").GetString(), Is.EqualTo("travel"));
        Assert.That(food.RootElement.GetProperty("stream_id").GetString(), Is.EqualTo("food"));

        socket.Add(StreamResponseEvent("response.created", "resp_travel", "in_progress", "travel"), true);
        socket.Add(StreamResponseEvent("response.created", "resp_food", "in_progress", "food"), true);
        socket.Add(StreamResponseEvent("response.completed", "resp_food", "completed", "food"), true);
        socket.Add(StreamResponseEvent("response.completed", "resp_travel", "completed", "travel"), true);

        ResponseResult[] results = await responses.WaitAsync(timeout.Token);
        Assert.That(results[0].Id, Is.EqualTo("resp_travel"));
        Assert.That(results[1].Id, Is.EqualTo("resp_food"));
        Assert.That(client.Connections, Is.EqualTo(1));
    }

    [TestCase("protocol")]
    [TestCase("unknown-stream")]
    [TestCase("close")]
    public async Task ParallelStreamsExampleReportsErrors(string failure)
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket, new ResponsesWebSocketClientOptions { ShouldReconnect = false });
        using CancellationTokenSource timeout = new(Timeout);
        Task<ResponseResult[]> responses = ResponsesWebSocketExamples.CreateParallelResponsesAsync(client, timeout.Token);

        await WaitForSentAsync(socket, 2, timeout.Token);
        if (failure == "protocol")
        {
            socket.Add("""{"type":"error","error":{"code":"rate_limit_exceeded","message":"Too many requests"}}""", true);
        }
        else if (failure == "unknown-stream")
        {
            socket.Add(StreamResponseEvent("response.completed", "resp_unknown", "completed", "unknown"), true);
        }
        else
        {
            socket.CloseFromServer();
        }

        InvalidOperationException error = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await responses.WaitAsync(timeout.Token));
        Assert.That(error.Message, Does.Contain(failure switch
        {
            "protocol" => "Too many requests",
            "unknown-stream" => "unknown stream",
            _ => "closed before both responses completed",
        }));
    }

    private static async Task WaitForSentAsync(FakeSocket socket, int count, CancellationToken cancellationToken)
    {
        while (socket.Sent.Count < count)
        {
            await Task.Delay(10, cancellationToken);
        }
    }

    private static string ResponseEvent(string type, string id, string status, string extra = "")
        => $$"""
            {
                "type": "{{type}}",
                "sequence_number": 1,
                "response": {
                    "id": "{{id}}",
                    "object": "response",
                    "created_at": 0,
                    "status": "{{status}}",
                    "model": "gpt-6-astra",
                    "output": [],
                    "tools": []{{extra}}
                }
            }
            """;

    private static string StreamResponseEvent(string type, string id, string status, string streamId)
        => ResponseEvent(type, id, status).Replace("\"type\":", $"\"stream_id\":\"{streamId}\",\"type\":");

    private sealed class TestClient : ResponsesWebSocketClient
    {
        private readonly FakeSocket _socket;

        public TestClient(FakeSocket socket, ResponsesWebSocketClientOptions options = null)
            : base(new ApiKeyCredential("test-key"), options ?? new ResponsesWebSocketClientOptions())
            => _socket = socket;

        public int Connections { get; private set; }

        protected override Task<WebSocket> ConnectWebSocketAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Connections++;
            return Task.FromResult<WebSocket>(_socket);
        }
    }

    private sealed class FakeSocket : WebSocket
    {
        private readonly ConcurrentQueue<(byte[] Data, bool End, Exception Failure)> _frames = new();
        private readonly SemaphoreSlim _ready = new(0);
        private WebSocketState _state = WebSocketState.Open;

        public ConcurrentQueue<string> Sent { get; } = new();
        public override WebSocketCloseStatus? CloseStatus => _state == WebSocketState.CloseReceived ? WebSocketCloseStatus.NormalClosure : null;
        public override string CloseStatusDescription => null;
        public override WebSocketState State => _state;
        public override string SubProtocol => null;

        public void Add(string data, bool end)
        {
            _frames.Enqueue((Encoding.UTF8.GetBytes(data), end, null));
            _ready.Release();
        }

        public void Fail(Exception exception)
        {
            _frames.Enqueue((null, true, exception));
            _ready.Release();
        }

        public void CloseFromServer()
        {
            _frames.Enqueue((null, true, null));
            _ready.Release();
        }

        public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            await _ready.WaitAsync(cancellationToken);
            if (!_frames.TryDequeue(out var frame)) throw new InvalidOperationException("No frame available.");
            if (frame.Failure != null) throw frame.Failure;
            if (frame.Data is null)
            {
                _state = _state == WebSocketState.CloseSent ? WebSocketState.Closed : WebSocketState.CloseReceived;
                return new WebSocketReceiveResult(0, WebSocketMessageType.Close, true, WebSocketCloseStatus.NormalClosure, "");
            }
            Array.Copy(frame.Data, 0, buffer.Array, buffer.Offset, frame.Data.Length);
            return new WebSocketReceiveResult(frame.Data.Length, WebSocketMessageType.Text, frame.End);
        }

        public override async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage,
            CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            Sent.Enqueue(Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count));
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            if (_state == WebSocketState.CloseReceived)
            {
                _state = WebSocketState.Closed;
            }
            else
            {
                _state = WebSocketState.CloseSent;
                _frames.Enqueue((null, true, null));
                _ready.Release();
            }
            return Task.CompletedTask;
        }

        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
            => CloseOutputAsync(closeStatus, statusDescription, cancellationToken);

        public override void Abort() => _state = WebSocketState.Aborted;
        public override void Dispose() => _state = WebSocketState.Closed;
    }
}

#pragma warning restore OPENAI001
