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
using System.ClientModel.Primitives;
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

    [TestCase("/v1", "", false)]
    [TestCase("/v1", "", true)]
    [TestCase("/v1/?contract=1", "?contract=1", false)]
    [TestCase("/v1/?contract=1", "?contract=1", true)]
    [TestCase("/v1/responses?contract=1", "?contract=1", false)]
    [TestCase("/v1/responses?contract=1", "?contract=1", true)]
    public async Task RealUpgradeUsesCredentialsEndpointAndHeaders(
        string endpointPath, string query, bool useAuthenticationPolicy)
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
                context.Request.QueryString.Value ?? "",
                context.Request.Headers["X-Per-Call"].ToString(),
                context.Request.Headers["X-Per-Try"].ToString(),
                context.Request.Headers["X-Before-Transport"].ToString(),
                context.Request.Headers["User-Agent"].ToString(),
            });
            context.Response.Headers["X-Handshake"] = "accepted";
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
        ResponsesWebSocketClientOptions options = new()
        {
            Endpoint = new Uri(address + endpointPath),
            OrganizationId = "test-org",
            ProjectId = "test-project",
            UserAgentApplicationId = "websocket-test-app",
        };
        HeaderPolicy perCall = new("X-Per-Call");
        HeaderPolicy perTry = new("X-Per-Try");
        HeaderPolicy beforeTransport = new("X-Before-Transport");
        options.AddPolicy(perCall, PipelinePosition.PerCall);
        options.AddPolicy(perTry, PipelinePosition.PerTry);
        options.AddPolicy(beforeTransport, PipelinePosition.BeforeTransport);
        options.Headers.Add("X-Test", "snapshot");
        await using ResponsesWebSocketClient client = useAuthenticationPolicy
            ? new ResponsesWebSocketClient(
                ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(credential, "Authorization", "Bearer"), options)
            : new ResponsesWebSocketClient(credential, options);
        options.Headers["X-Test"] = "modified";
        credential.Update("updated-test-key");
        Subscribe(client);
        await client.ConnectAsync().WaitAsync(Timeout);
        string[] headers = await upgrade.Task.WaitAsync(Timeout);
        Assert.That(headers.Take(8), Is.EqualTo(new[]
        {
            "Bearer updated-test-key", "test-org", "test-project", "snapshot", query, "policy", "policy", "policy",
        }));
        if (!PlatformTelemetry.IsTelemetryDisabled())
        {
            Assert.That(headers[8], Does.Contain("websocket-test-app"));
        }
        Assert.That(new[] { perCall, perTry, beforeTransport }, Has.All.Property(nameof(HeaderPolicy.Status)).EqualTo(101));
        Assert.That(new[] { perCall, perTry, beforeTransport }, Has.All.Property(nameof(HeaderPolicy.ResponseHeader)).EqualTo("accepted"));
        await client.CloseAsync().WaitAsync(Timeout);
        await server.StopAsync();
    }

    [Test]
    public async Task ResponseAwareAuthenticationCanRetryARejectedUpgrade()
    {
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        await using WebApplication server = builder.Build();
        server.UseWebSockets();
        ConcurrentQueue<string> authorization = new();
        server.Map("/v1/responses", async context =>
        {
            string value = context.Request.Headers["Authorization"].ToString();
            authorization.Enqueue(value);
            if (value != "Bearer refreshed")
            {
                context.Response.StatusCode = 401;
                context.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\"";
                return;
            }

            context.Response.Headers["X-Handshake"] = "accepted";
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
        ResponseAwareAuthenticationPolicy policy = new();
        await using ResponsesWebSocketClient client = new(policy, new ResponsesWebSocketClientOptions
        {
            Endpoint = new Uri(address + "/v1"),
        });
        Subscribe(client);

        await client.ConnectAsync().WaitAsync(Timeout);
        Assert.That(authorization, Is.EqualTo(new[] { "Bearer stale", "Bearer refreshed" }));
        Assert.That(policy.Statuses, Is.EqualTo(new[] { 401, 101 }));
        Assert.That(policy.Challenge, Is.EqualTo("Bearer error=\"invalid_token\""));
        Assert.That(policy.ResponseHeader, Is.EqualTo("accepted"));
        Assert.That(policy.FailedResponseBodyUnavailable, Is.True);
        await client.CloseAsync().WaitAsync(Timeout);
        await server.StopAsync();
    }

    [Test]
    public async Task RejectedUpgradeWithoutRetryPreservesTheHandshakeException()
    {
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        await using WebApplication server = builder.Build();
        server.Map("/v1/responses", context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        });
        await server.StartAsync();
        string address = server.Services.GetRequiredService<IServer>().Features
            .Get<IServerAddressesFeature>().Addresses.Single();
        await using ResponsesWebSocketClient client = new(new ApiKeyCredential("test-key"), new ResponsesWebSocketClientOptions
        {
            Endpoint = new Uri(address + "/v1"),
        });
        Subscribe(client);

        WebSocketException error = await Assert.ThrowsAsync<WebSocketException>(
            async () => await client.ConnectAsync().WaitAsync(Timeout));
        Assert.That(error.Message, Does.Contain("401"));
        await server.StopAsync();
    }

    [Test]
    public async Task UpgradeHonorsThePipelineNetworkTimeout()
    {
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        await using WebApplication server = builder.Build();
        TaskCompletionSource<bool> received = Signal();
        server.Map("/v1/responses", async context =>
        {
            received.TrySetResult(true);
            try
            {
                await Task.Delay(System.Threading.Timeout.Infinite, context.RequestAborted);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
            }
        });
        await server.StartAsync();
        string address = server.Services.GetRequiredService<IServer>().Features
            .Get<IServerAddressesFeature>().Addresses.Single();
        await using ResponsesWebSocketClient client = new(new ApiKeyCredential("test-key"), new ResponsesWebSocketClientOptions
        {
            Endpoint = new Uri(address + "/v1"),
            NetworkTimeout = TimeSpan.FromMilliseconds(500),
            RetryPolicy = new ClientRetryPolicy(maxRetries: 0),
        });
        Subscribe(client);

        Task connecting = client.ConnectAsync();
        await received.Task.WaitAsync(Timeout);
        await Assert.CatchAsync<OperationCanceledException>(async () => await connecting.WaitAsync(TimeSpan.FromSeconds(5)));
    }

    [Test]
    public async Task UpgradeRejectsUnsupportedEndpointSchemes()
    {
        await using ResponsesWebSocketClient client = new(new ApiKeyCredential("test-key"), new ResponsesWebSocketClientOptions
        {
            Endpoint = new Uri("ftp://127.0.0.1/v1"),
        });
        Subscribe(client);

        await Assert.ThrowsAsync<NotSupportedException>(() => client.ConnectAsync());
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
        await client.CreateResponseAsync(new CreateResponseOptions { Model = "test-model", StoredOutputEnabled = false },
            new ResponsesWebSocketSendOptions { StreamId = "main", GenerationEnabled = false });
        using JsonDocument command = JsonDocument.Parse(socket.Sent.First());
        Assert.That(command.RootElement.GetProperty("type").GetString(), Is.EqualTo("response.create"));
        Assert.That(command.RootElement.GetProperty("model").GetString(), Is.EqualTo("test-model"));
        Assert.That(command.RootElement.GetProperty("stream_id").GetString(), Is.EqualTo("main"));
        Assert.That(command.RootElement.GetProperty("generate").GetBoolean(), Is.False);
        Assert.That(command.RootElement.TryGetProperty("stream", out _), Is.False);
        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => client.CreateResponseAsync(BinaryData.FromString("{\"type\":\"response.create\"}"))));
        Assert.That(socket.MaximumConcurrentSends, Is.EqualTo(1));
        Assert.That(socket.Sent.Count, Is.EqualTo(21));
    }

    [Test]
    public async Task ConvenienceCreateSendsSameEnvelopeAsOptions(
        [Values] bool useText,
        [Values] bool useSendOptions,
        [Values(null, "resp_prev")] string previousResponseId)
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        Subscribe(client);
        await client.ConnectAsync();
        ResponsesWebSocketSendOptions sendOptions = useSendOptions ? new() { StreamId = "main" } : null;

        // Built via property setters to stay independent of the constructor under test.
        CreateResponseOptions expected = new() { Model = "test-model" };
        if (previousResponseId is not null)
        {
            expected.PreviousResponseId = previousResponseId;
        }
        expected.InputItems.Add(ResponseItem.CreateUserMessageItem("hello"));
        await client.CreateResponseAsync(expected, sendOptions);

        ResponseItem[] items = [ResponseItem.CreateUserMessageItem("hello")];
        await ((useText, useSendOptions) switch
        {
            (true, true) => client.CreateResponseAsync("test-model", "hello", sendOptions, previousResponseId),
            (true, false) => client.CreateResponseAsync("test-model", "hello", previousResponseId),
            (false, true) => client.CreateResponseAsync("test-model", items, sendOptions, previousResponseId),
            _ => client.CreateResponseAsync("test-model", items, previousResponseId),
        });

        Assert.That(socket.Sent, Has.Count.EqualTo(2));
        Assert.That(socket.Sent.Last(), Is.EqualTo(socket.Sent.First()));
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
        socket.Add("{\"type\":\"future.event\",\"sequence_number\":2,\"status\":\"in_progress\",\"future_field\":42}", true);
        socket.Add("{\"type\":\"error\",\"status\":400,\"error\":{\"code\":\"invalid_request\",\"message\":\"bad request\"}}", true);
        await received.Task.WaitAsync(Timeout);
        Assert.That(messages[0].Update, Is.TypeOf<StreamingResponseOutputTextDeltaUpdate>());
        Assert.That(messages[0].StreamId, Is.EqualTo("main"));
        Assert.That(messages[1].Kind.ToString(), Is.EqualTo("future.event"));
        Assert.That(messages[1].RawData.ToString(), Does.Contain("\"future_field\":42"));
        Assert.That(messages[1].Status, Is.Null);
        Assert.That(messages[1].Update, Is.Not.Null);
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
        await client.CreateResponseAsync(BinaryData.FromString("{\"type\":\"response.create\"}"));
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
        await Assert.CatchAsync<Exception>(() => client.CreateResponseAsync(BinaryData.FromString("{}")));
    }

    [Test]
    public async Task CloseCancelsAnInProgressReconnect()
    {
        FakeSocket socket = new();
        await using RetryingClient client = new(socket);
        Subscribe(client);
        await client.ConnectAsync();
        socket.Fail(new WebSocketException("Disconnected"));
        await client.RetryStarted.Task.WaitAsync(Timeout);

        await client.CloseAsync().WaitAsync(TimeSpan.FromSeconds(3));
        await client.RetryCanceled.Task.WaitAsync(Timeout);
        await client.Completion.WaitAsync(Timeout);
        Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Closed));
    }

    [Test]
    public async Task DisposalRacingAReceivedMessageDoesNotFaultCompletion()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        Subscribe(client);
        await client.ConnectAsync();
        socket.OnTextReceived = client.Dispose;
        socket.Add("{\"type\":\"future.event\"}", true);

        await client.Completion.WaitAsync(Timeout);
        await client.DisposeAsync().AsTask().WaitAsync(Timeout);
        Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Disposed));
    }

    [Test]
    public async Task ThrowingCancellationCallbackDoesNotBlockActiveTransportDisposal()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        InvalidOperationException failure = new("Cancellation callback failed.");
        TaskCompletionSource<bool> registered = Signal();
        client.MessageReceivedAsync += args =>
        {
            args.CancellationToken.Register(() => throw failure);
            registered.TrySetResult(true);
            return Task.CompletedTask;
        };
        client.ErrorOccurredAsync += _ => Task.CompletedTask;
        await client.ConnectAsync();
        socket.Add("{\"type\":\"future.event\"}", true);
        await registered.Task.WaitAsync(Timeout);

        Assert.DoesNotThrow(client.Dispose);
        AggregateException error = await Assert.ThrowsAsync<AggregateException>(
            async () => await client.Completion.WaitAsync(Timeout));
        Assert.That(error.InnerExceptions, Does.Contain(failure));
        await client.DisposeAsync().AsTask().WaitAsync(Timeout);
        Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Disposed));
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task WebSocketRequestsRejectHttpTransportFlags(bool streaming)
    {
        await using TestClient client = new(new FakeSocket());
        CreateResponseOptions options = new() { Model = "test" };
        if (streaming) options.StreamingEnabled = false;
        else options.BackgroundModeEnabled = false;
        Assert.Throws<ArgumentException>(() => client.CreateResponseAsync(options));
    }

    [Test]
    public async Task SendsSteeringWithUserMessageAndParentId()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket);
        Subscribe(client);
        await client.ConnectAsync();
        await client.SteerResponseAsync("resp_parent", "Change direction");
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
        Assert.Throws<ArgumentException>(() => client.SteerResponseAsync("resp_parent", new[] { message }));
        Assert.Throws<ArgumentException>(() => client.SteerResponseAsync("resp_parent",
            new[] { ResponseItem.CreateSystemMessageItem("Change direction") }));
        Assert.Throws<ArgumentException>(() => client.SteerResponseAsync("resp_parent",
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
    public async Task RemovingBackpressuredSubscriberDoesNotStopReceiving()
    {
        FakeSocket socket = new();
        await using TestClient client = new(socket, new ResponsesWebSocketClientOptions { MaxBufferedMessages = 1 });
        TaskCompletionSource<bool> entered = Signal();
        TaskCompletionSource<bool> release = Signal();
        TaskCompletionSource<bool> thirdReceived = Signal();
        TaskCompletionSource<bool> fourthReceived = Signal();
        Func<ResponsesWebSocketMessageReceivedEventArgs, Task> removed = async _ =>
        {
            entered.TrySetResult(true);
            await release.Task.WaitAsync(Timeout);
        };
        int retainedCount = 0;
        client.MessageReceivedAsync += removed;
        client.MessageReceivedAsync += _ =>
        {
            int count = Interlocked.Increment(ref retainedCount);
            if (count == 3) thirdReceived.TrySetResult(true);
            if (count == 4) fourthReceived.TrySetResult(true);
            return Task.CompletedTask;
        };
        client.ErrorOccurredAsync += _ => Task.CompletedTask;
        await client.ConnectAsync();
        socket.Add("{\"type\":\"future.event\"}", true);
        await entered.Task.WaitAsync(Timeout);
        try
        {
            socket.Add("{\"type\":\"future.event\"}", true);
            socket.Add("{\"type\":\"future.event\"}", true);
            await thirdReceived.Task.WaitAsync(Timeout);

            client.MessageReceivedAsync -= removed;
            socket.Add("{\"type\":\"future.event\"}", true);
            await fourthReceived.Task.WaitAsync(Timeout);
            Assert.That(client.State, Is.EqualTo(ResponsesWebSocketConnectionState.Connected));
            Assert.That(client.Completion.IsCompleted, Is.False);
        }
        finally
        {
            release.TrySetResult(true);
        }
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
        Assert.Throws<ArgumentException>(() => client.CreateResponseAsync(new CreateResponseOptions { Model = "test" },
            new ResponsesWebSocketSendOptions { StreamId = streamId }));
    }

    private static void Subscribe(ResponsesWebSocketClient client)
    {
        client.MessageReceivedAsync += _ => Task.CompletedTask;
        client.ErrorOccurredAsync += _ => Task.CompletedTask;
    }

    private static TaskCompletionSource<bool> Signal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private sealed class HeaderPolicy(string headerName) : PipelinePolicy
    {
        public int Status { get; private set; }
        public string ResponseHeader { get; private set; }

        public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
            => throw new NotSupportedException();

        public override async ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
        {
            message.Request.Headers.Set(headerName, "policy");
            await ProcessNextAsync(message, pipeline, currentIndex);
            Status = message.Response.Status;
            message.Response.Headers.TryGetValue("X-Handshake", out string value);
            ResponseHeader = value;
        }
    }

    private sealed class ResponseAwareAuthenticationPolicy : AuthenticationPolicy
    {
        public List<int> Statuses { get; } = new();
        public string Challenge { get; private set; }
        public string ResponseHeader { get; private set; }
        public bool FailedResponseBodyUnavailable { get; private set; }

        public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
            => throw new NotSupportedException();

        public override async ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
        {
            message.Request.Headers.Set("Authorization", "Bearer stale");
            await ProcessNextAsync(message, pipeline, currentIndex);
            Statuses.Add(message.Response.Status);
            if (message.Response.Status == 401)
            {
                message.Response.Headers.TryGetValue("WWW-Authenticate", out string challenge);
                Challenge = challenge;
                try
                {
                    _ = message.Response.Content;
                }
                catch (NotSupportedException)
                {
                    FailedResponseBodyUnavailable = true;
                }
                message.Request.Headers.Set("Authorization", "Bearer refreshed");
                await ProcessNextAsync(message, pipeline, currentIndex);
                Statuses.Add(message.Response.Status);
            }
            message.Response.Headers.TryGetValue("X-Handshake", out string responseHeader);
            ResponseHeader = responseHeader;
        }
    }

    private sealed class TestClient : ResponsesWebSocketClient
    {
        private readonly Queue<FakeSocket> _sockets;
        public int Connections { get; private set; }
        public TestClient(FakeSocket socket, ResponsesWebSocketClientOptions options = null) : this(new[] { socket }, options) { }
        public TestClient(IEnumerable<FakeSocket> sockets, ResponsesWebSocketClientOptions options = null)
            : base(new ApiKeyCredential("test-key"), options ?? new ResponsesWebSocketClientOptions())
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
            : base("test-key")
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

    private sealed class RetryingClient : ResponsesWebSocketClient
    {
        private readonly FakeSocket _socket;
        private int _connections;

        public RetryingClient(FakeSocket socket) : base("test-key") => _socket = socket;

        public TaskCompletionSource<bool> RetryStarted { get; } = Signal();
        public TaskCompletionSource<bool> RetryCanceled { get; } = Signal();

        protected override async Task<WebSocket> ConnectWebSocketAsync(CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _connections) == 1)
            {
                return _socket;
            }

            RetryStarted.TrySetResult(true);
            try
            {
                await Task.Delay(System.Threading.Timeout.Infinite, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                RetryCanceled.TrySetResult(true);
                throw;
            }

            throw new InvalidOperationException("The reconnect delay ended unexpectedly.");
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
        public Action OnTextReceived { get; set; }
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
            OnTextReceived?.Invoke();
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
