using NUnit.Framework;
using OpenAI.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.ClientModel;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses.WebSockets;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class ResponsesWebSocketTransportTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [TestCase("/v1", "")]
    [TestCase("/v1/?contract=1", "?contract=1")]
    [TestCase("/v1/responses?contract=1", "?contract=1")]
    public async Task RealUpgradeUsesParentCredentialsEndpointAndHeaders(string endpointPath, string query)
    {
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        await using WebApplication server = builder.Build();
        server.UseWebSockets();
        TaskCompletionSource<string[]> upgrade = new(TaskCreationOptions.RunContinuationsAsynchronously);
        server.Map("/v1/responses", async context =>
        {
            upgrade.TrySetResult(new[]
            {
                context.Request.Headers["Authorization"].ToString(),
                context.Request.Headers["OpenAI-Organization"].ToString(),
                context.Request.Headers["OpenAI-Project"].ToString(),
                context.Request.Headers["X-Test"].ToString(),
                context.Request.QueryString.Value ?? ""
            });
            using WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
            byte[] buffer = new byte[4096];
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), context.RequestAborted);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", context.RequestAborted);
            }
        });
        await server.StartAsync();
        string address = server.Services.GetRequiredService<IServer>().Features
            .Get<IServerAddressesFeature>().Addresses.Single();
        ApiKeyCredential credential = new("old-test-key");
        ResponsesClient parent = new(credential, new ResponsesClientOptions
        {
            Endpoint = new Uri(address + endpointPath),
            OrganizationId = "test-org",
            ProjectId = "test-project"
        });
        ResponsesWebSocketClientOptions options = new();
        options.Headers.Add("X-Test", "snapshot");
        await using ResponsesWebSocketClient client = parent.CreateWebSocketClient(options);
        options.Headers["X-Test"] = "modified";
        credential.Update("updated-test-key");
        Subscribe(client);
        await client.ConnectAsync().WaitAsync(Timeout);
        string[] headers = await upgrade.Task.WaitAsync(Timeout);
        Assert.That(headers, Is.EqualTo(new[] { "Bearer updated-test-key", "test-org", "test-project", "snapshot", query }));
        await client.CloseAsync().WaitAsync(Timeout);
        await server.StopAsync();
    }

    [Test]
    public async Task ConnectRequiresSubscribersAndDoesNotReconnectAnOpenSocket()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        await Assert.ThrowsAsync<InvalidOperationException>(() => client.ConnectAsync());
        Subscribe(client);
        await Task.WhenAll(client.ConnectAsync(), client.ConnectAsync());
        Assert.That(client.Connections, Is.EqualTo(1));
        Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Connected));
    }

    [Test]
    public async Task SendsResponseCreateEnvelopeAndSerializesConcurrentWrites()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        Subscribe(client);
        await client.ConnectAsync();
        await client.CreateAsync(new CreateResponseOptions { Model = "test-model", StoredOutputEnabled = false },
            new ResponsesWebSocketSendOptions { StreamId = "main", GenerationEnabled = false });
        using JsonDocument command = JsonDocument.Parse(socket.Sent.First());
        Assert.That(command.RootElement.GetProperty("type").GetString(), Is.EqualTo("response.create"));
        Assert.That(command.RootElement.GetProperty("model").GetString(), Is.EqualTo("test-model"));
        Assert.That(command.RootElement.GetProperty("stream_id").GetString(), Is.EqualTo("main"));
        Assert.That(command.RootElement.GetProperty("generate").GetBoolean(), Is.False);
        Assert.That(command.RootElement.TryGetProperty("stream", out _), Is.False);
        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => client.CreateAsync(BinaryData.FromString("{\"type\":\"response.create\"}"))));
        Assert.That(socket.MaximumConcurrentSends, Is.EqualTo(1));
        Assert.That(socket.Sent.Count, Is.EqualTo(21));
    }

    [Test]
    public async Task ReceivesFragmentedTypedUnknownAndProtocolErrorMessagesInOrder()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        List<ResponsesWebSocketMessage> messages = new();
        TaskCompletionSource<bool> received = Signal();
        int localErrors = 0;
        client.MessageReceivedAsync += args =>
        {
            messages.Add(args.Message);
            if (messages.Count == 3) received.TrySetResult(true);
            return Task.CompletedTask;
        };
        client.ErrorOccurredAsync += _ => { Interlocked.Increment(ref localErrors); return Task.CompletedTask; };
        await client.ConnectAsync();
        socket.Add("{\"type\":\"response.output_text.delta\",\"sequence_number\":1,\"item_id\":\"item\",");
        socket.Add("\"output_index\":0,\"content_index\":0,\"delta\":\"hello\",\"stream_id\":\"main\"}", true);
        socket.Add("{\"type\":\"future.event\",\"future_field\":42}", true);
        socket.Add("{\"type\":\"error\",\"status\":400,\"error\":{\"code\":\"invalid_request\",\"message\":\"bad request\"}}", true);
        await received.Task.WaitAsync(Timeout);
        Assert.That(messages[0].Update, Is.TypeOf<StreamingResponseOutputTextDeltaUpdate>());
        Assert.That(messages[0].StreamId, Is.EqualTo("main"));
        Assert.That(messages[1].Kind.ToString(), Is.EqualTo("future.event"));
        Assert.That(messages[1].RawData.ToString(), Does.Contain("\"future_field\":42"));
        Assert.That(messages[2].Status, Is.EqualTo(400));
        Assert.That(messages[2].Error.Message, Is.EqualTo("bad request"));
        Assert.That(localErrors, Is.Zero);
    }

    [TestCase("{", 0)]
    [TestCase("{\"type\":\"future.event\"}", 4)]
    public async Task InvalidOrOversizeMessagesReportTerminalLocalErrors(string data, int maxBytes)
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket, new ResponsesWebSocketClientOptions
        {
            MaxMessageBytes = maxBytes,
            ShouldReconnect = false
        });
        TaskCompletionSource<ResponsesWebSocketErrorEventArgs> error = new(TaskCreationOptions.RunContinuationsAsynchronously);
        client.MessageReceivedAsync += _ => Task.CompletedTask;
        client.ErrorOccurredAsync += args => { error.TrySetResult(args); return Task.CompletedTask; };
        await client.ConnectAsync();
        socket.Add(data, true);
        ResponsesWebSocketErrorEventArgs reported = await error.Task.WaitAsync(Timeout);
        Assert.That(reported.IsTerminal, Is.True);
        Assert.That(reported.Exception, Is.Not.Null);
        await Assert.CatchAsync<Exception>(async () => await client.Completion.WaitAsync(Timeout));
    }

    [Test]
    public async Task ReconnectsWithoutReplayingCommands()
    {
        FakeSocket first = new();
        FakeSocket second = new();
        await using TestClient client = new(new[] { first, second });
        TaskCompletionSource<bool> recovered = Signal();
        TaskCompletionSource<bool> disconnected = Signal();
        client.MessageReceivedAsync += _ => { recovered.TrySetResult(true); return Task.CompletedTask; };
        client.ErrorOccurredAsync += _ => { disconnected.TrySetResult(true); return Task.CompletedTask; };
        await client.ConnectAsync();
        await client.CreateAsync(BinaryData.FromString("{\"type\":\"response.create\"}"));
        first.Fail(new WebSocketException("Disconnected"));
        await disconnected.Task.WaitAsync(Timeout);
        await Assert.ThrowsAsync<InvalidOperationException>(() => client.ConnectAsync());
        Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Reconnecting));
        second.Add("{\"type\":\"future.event\"}", true);
        await recovered.Task.WaitAsync(Timeout);
        Assert.That(client.Connections, Is.EqualTo(2));
        Assert.That(second.Sent, Is.Empty);
        Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Connected));
    }

    [Test]
    public async Task CloseSendsCloseOutputAndStopsReceiving()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        Subscribe(client);
        await client.ConnectAsync();
        await client.CloseAsync();
        await client.Completion.WaitAsync(Timeout);
        Assert.That(socket.CloseOutputCalls, Is.EqualTo(1));
        Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Closed));
        await Assert.CatchAsync<Exception>(() => client.CreateAsync(BinaryData.FromString("{}")));
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task WebSocketRequestsRejectHttpTransportFlags(bool streaming)
    {
        await using TestClient client = new(new FakeSocket());
        CreateResponseOptions options = new() { Model = "test" };
        if (streaming) options.StreamingEnabled = false;
        else options.BackgroundModeEnabled = false;
        Assert.Throws<ArgumentException>(() => client.CreateAsync(options));
    }

    [Test]
    public async Task SendsSteeringWithUserMessageAndParentId()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        Subscribe(client);
        await client.ConnectAsync();
        await client.SteerAsync("resp_parent", "Change direction");
        using JsonDocument command = JsonDocument.Parse(socket.Sent.Single());
        Assert.That(command.RootElement.GetProperty("type").GetString(), Is.EqualTo("response.steer"));
        Assert.That(command.RootElement.GetProperty("previous_response_id").GetString(), Is.EqualTo("resp_parent"));
        Assert.That(command.RootElement.GetProperty("input")[0].GetProperty("role").GetString(), Is.EqualTo("user"));
    }

    [Test]
    public async Task SteeringRejectsMessageMetadataRolesAndToolOutputs()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        Subscribe(client);
        await client.ConnectAsync();
        MessageResponseItem message = ResponseItem.CreateUserMessageItem("Change direction");
        message.Status = MessageStatus.Completed;
        Assert.Throws<ArgumentException>(() => client.SteerAsync("resp_parent", new[] { message }));
        Assert.Throws<ArgumentException>(() => client.SteerAsync("resp_parent",
            new[] { ResponseItem.CreateSystemMessageItem("Change direction") }));
        Assert.Throws<ArgumentException>(() => client.SteerAsync("resp_parent",
            new[] { ResponseItem.CreateFunctionCallOutputItem("call_test", "tool result") }));
        Assert.That(socket.Sent, Is.Empty);
    }

    [Test]
    public async Task ByteBudgetCountsQueuedPayloadsAndReportsOverflow()
    {
        const string payload = "{\"type\":\"future.event\"}";
        FakeSocket socket = new();
        await using TestClient client = new(socket, new ResponsesWebSocketClientOptions
        {
            MaxBufferedBytes = Encoding.UTF8.GetByteCount(payload),
            ShouldReconnect = false
        });
        TaskCompletionSource<bool> entered = Signal();
        TaskCompletionSource<bool> release = Signal();
        TaskCompletionSource<ResponsesWebSocketErrorEventArgs> error = new(TaskCreationOptions.RunContinuationsAsynchronously);
        client.MessageReceivedAsync += async _ =>
        {
            entered.TrySetResult(true);
            await release.Task.WaitAsync(Timeout);
        };
        client.ErrorOccurredAsync += args => { error.TrySetResult(args); return Task.CompletedTask; };
        await client.ConnectAsync();
        socket.Add(payload, true);
        await entered.Task.WaitAsync(Timeout);
        try
        {
            socket.Add(payload, true);
            socket.Add(payload, true);
            ResponsesWebSocketErrorEventArgs reported = await error.Task.WaitAsync(Timeout);
            Assert.That(reported.IsTerminal, Is.True);
            Assert.That(reported.Exception, Is.TypeOf<InvalidDataException>());
            Assert.That(reported.Exception.Message, Does.Contain("MaxBufferedBytes"));
        }
        finally
        {
            release.TrySetResult(true);
        }
    }

    [Test]
    public async Task ConfiguredQueueCapacityAppliesBackpressureAndPreservesOrder()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket, new ResponsesWebSocketClientOptions { MaxBufferedMessages = 1 });
        TaskCompletionSource<bool> entered = Signal();
        TaskCompletionSource<bool> release = Signal();
        TaskCompletionSource<bool> finished = Signal();
        List<int> received = new();
        client.MessageReceivedAsync += async args =>
        {
            using JsonDocument document = JsonDocument.Parse(args.Message.RawData);
            int value = document.RootElement.GetProperty("value").GetInt32();
            received.Add(value);
            if (value == 0)
            {
                entered.TrySetResult(true);
                await release.Task.WaitAsync(Timeout);
            }
            if (value == 3) finished.TrySetResult(true);
        };
        client.ErrorOccurredAsync += _ => Task.CompletedTask;
        await client.ConnectAsync();
        socket.Add("{\"type\":\"future.event\",\"value\":0}", true);
        await entered.Task.WaitAsync(Timeout);
        try
        {
            for (int value = 1; value <= 3; value++)
            {
                socket.Add("{\"type\":\"future.event\",\"value\":" + value + "}", true);
            }
            await socket.ThirdRead.Task.WaitAsync(Timeout);
            Assert.That(socket.ReadCount, Is.EqualTo(3), "The receive loop must wait for the full subscriber queue.");
            Assert.That(received, Is.EqualTo(new[] { 0 }));
        }
        finally
        {
            release.TrySetResult(true);
        }
        await finished.Task.WaitAsync(Timeout);
        Assert.That(received, Is.EqualTo(new[] { 0, 1, 2, 3 }));
    }

    [Test]
    public async Task CallbackFailureIsReportedAndReceivingContinues()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        InvalidOperationException failure = new("Message handler failed.");
        TaskCompletionSource<ResponsesWebSocketErrorEventArgs> reported = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<bool> siblingHandled = Signal();
        TaskCompletionSource<bool> laterMessageHandled = Signal();
        int messageCount = 0;
        client.MessageReceivedAsync += _ =>
        {
            if (Interlocked.Increment(ref messageCount) == 1)
            {
                return Task.FromException(failure);
            }
            laterMessageHandled.TrySetResult(true);
            return Task.CompletedTask;
        };
        client.MessageReceivedAsync += _ =>
        {
            siblingHandled.TrySetResult(true);
            return Task.CompletedTask;
        };
        client.ErrorOccurredAsync += args => { reported.TrySetResult(args); return Task.CompletedTask; };
        await client.ConnectAsync();
        socket.Add("{\"type\":\"future.event\"}", true);
        ResponsesWebSocketErrorEventArgs error = await reported.Task.WaitAsync(Timeout);
        Assert.That(error.Exception, Is.SameAs(failure));
        Assert.That(error.IsTerminal, Is.False);
        await siblingHandled.Task.WaitAsync(Timeout);
        socket.Add("{\"type\":\"future.event\"}", true);
        await laterMessageHandled.Task.WaitAsync(Timeout);
        Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Connected));
        Assert.That(client.Completion.IsCompleted, Is.False);
    }

    [Test]
    public async Task DisposalCancelsAndAwaitsAnInProgressConnection()
    {
        TaskCompletionSource<bool> started = Signal();
        TaskCompletionSource<WebSocket> socket = new(TaskCreationOptions.RunContinuationsAsynchronously);
        await using DelayedConnectClient client = new(started, socket);
        Subscribe(client);
        Task connecting = client.ConnectAsync();
        await started.Task.WaitAsync(Timeout);
        await client.DisposeAsync().AsTask().WaitAsync(Timeout);
        await Assert.CatchAsync<OperationCanceledException>(async () => await connecting.WaitAsync(Timeout));
        Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Disposed));
        await Assert.ThrowsAsync<ObjectDisposedException>(() => client.ConnectAsync());
    }

    [TestCase("")]
    [TestCase("has spaces")]
    [TestCase("stream/id")]
    public async Task RejectsInvalidStreamIds(string streamId)
    {
        await using TestClient client = new(new FakeSocket());
        Assert.Throws<ArgumentException>(() => client.CreateAsync(new CreateResponseOptions { Model = "test" },
            new ResponsesWebSocketSendOptions { StreamId = streamId }));
    }

    private static void Subscribe(ResponsesWebSocketClient client)
    {
        client.MessageReceivedAsync += _ => Task.CompletedTask;
        client.ErrorOccurredAsync += _ => Task.CompletedTask;
    }

    private static TaskCompletionSource<bool> Signal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private sealed class TestClient : ResponsesWebSocketClient
    {
        private readonly Queue<FakeSocket> _sockets;
        public int Connections { get; private set; }
        public TestClient(FakeSocket socket, ResponsesWebSocketClientOptions options = null) : this(new[] { socket }, options) { }
        public TestClient(IEnumerable<FakeSocket> sockets, ResponsesWebSocketClientOptions options = null)
            : base(new ResponsesClient("test-key"), options ?? new ResponsesWebSocketClientOptions())
            => _sockets = new Queue<FakeSocket>(sockets);
        protected override Task<WebSocket> ConnectWebSocketAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Connections++;
            return Task.FromResult<WebSocket>(_sockets.Dequeue());
        }
    }

    private sealed class DelayedConnectClient : ResponsesWebSocketClient
    {
        private readonly TaskCompletionSource<bool> _started;
        private readonly TaskCompletionSource<WebSocket> _socket;
        public DelayedConnectClient(TaskCompletionSource<bool> started, TaskCompletionSource<WebSocket> socket)
            : base(new ResponsesClient("test-key"), new ResponsesWebSocketClientOptions())
        {
            _started = started;
            _socket = socket;
        }
        protected override async Task<WebSocket> ConnectWebSocketAsync(CancellationToken cancellationToken)
        {
            _started.TrySetResult(true);
            return await _socket.Task.WaitAsync(cancellationToken);
        }
    }

    private sealed class FakeSocket : WebSocket
    {
        private readonly ConcurrentQueue<(byte[] Data, bool End, Exception Failure)> _frames = new();
        private readonly SemaphoreSlim _ready = new(0);
        private WebSocketState _state = WebSocketState.Open;
        private int _sending;
        private int _reads;
        public int ReadCount => Volatile.Read(ref _reads);
        public TaskCompletionSource<bool> ThirdRead { get; } = Signal();
        public ConcurrentQueue<string> Sent { get; } = new();
        public int MaximumConcurrentSends { get; private set; }
        public int CloseOutputCalls { get; private set; }
        public override WebSocketCloseStatus? CloseStatus => _state == WebSocketState.CloseReceived ? WebSocketCloseStatus.NormalClosure : null;
        public override string CloseStatusDescription => null;
        public override WebSocketState State => _state;
        public override string SubProtocol => null;
        public void Add(string data, bool end = false)
        {
            _frames.Enqueue((Encoding.UTF8.GetBytes(data), end, null));
            _ready.Release();
        }
        public void Fail(Exception exception)
        {
            _frames.Enqueue((null, true, exception));
            _ready.Release();
        }
        public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            await _ready.WaitAsync(cancellationToken);
            if (!_frames.TryDequeue(out var frame)) throw new InvalidOperationException("No frame available.");
            if (frame.Failure != null) throw frame.Failure;
            if (Interlocked.Increment(ref _reads) == 3) ThirdRead.TrySetResult(true);
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
            int sending = Interlocked.Increment(ref _sending);
            MaximumConcurrentSends = Math.Max(MaximumConcurrentSends, sending);
            try
            {
                await Task.Yield();
                cancellationToken.ThrowIfCancellationRequested();
                Sent.Enqueue(Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count));
            }
            finally { Interlocked.Decrement(ref _sending); }
        }
        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            CloseOutputCalls++;
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
