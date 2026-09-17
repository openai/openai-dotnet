using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses;

[Category("Smoke")]
[Parallelizable(ParallelScope.All)]
public class ResponsesWebSocketTests
{
    private const string Create = "{\"type\":\"response.create\",\"model\":\"test-model\",\"input\":\"hello\"}";
    private static string Terminal(string status, string id, string stream = null) =>
        "{\"type\":\"response." + status + "\",\"sequence_number\":1," +
        (stream == null ? "" : "\"stream_id\":\"" + stream + "\",") +
        "\"response\":{\"id\":\"" + id + "\",\"object\":\"response\",\"created_at\":1,\"model\":\"test-model\",\"status\":\"" + status +
        "\",\"output\":[],\"parallel_tool_calls\":false,\"tool_choice\":\"auto\",\"tools\":[]}}";

    [TestCase(false)]
    [TestCase(true)]
    public async Task DirectAndFactoryClientsPreservePoliciesAndRefreshCredentials(bool factory)
    {
        await using var server = await LocalServer.Start(async (context, socket) =>
        {
            Assert.That(context.Request.Path.Value, Is.EqualTo("/custom/v1/responses"));
            Assert.That(context.Request.Query["version"].ToString(), Is.EqualTo("test"));
            Assert.That(context.Request.Headers.Authorization.ToString(), Is.EqualTo("Bearer refreshed"));
            Assert.That(context.Request.Headers["OpenAI-Organization"].ToString(), Is.EqualTo("organization"));
            Assert.That(context.Request.Headers["OpenAI-Project"].ToString(), Is.EqualTo("project"));
            Assert.That(context.Request.Headers["X-Policy"].ToString(), Is.EqualTo("ran"));
            Assert.That(context.Request.Headers["X-Custom"].ToString(), Is.EqualTo("custom"));
            Assert.That(await Receive(socket), Is.EqualTo(Create));
            await Send(socket, Terminal("completed", "resp_first"));
            Assert.That(await Receive(socket), Is.EqualTo(Create));
            await Send(socket, Terminal("completed", "resp_second"));
            await AwaitClose(socket);
        });
        var endpoint = new Uri(server.Endpoint + "custom/v1?version=test");
        var credential = new ApiKeyCredential("original");
        ResponsesClient client;
        if (factory)
        {
            var options = new OpenAIClientOptions { Endpoint = endpoint, OrganizationId = "organization", ProjectId = "project" };
            options.AddPolicy(new HeaderPolicy(), PipelinePosition.PerTry);
            client = new OpenAIClient(credential, options).GetResponsesClient();
        }
        else
        {
            var options = new ResponsesClientOptions { Endpoint = endpoint, OrganizationId = "organization", ProjectId = "project" };
            options.AddPolicy(new HeaderPolicy(), PipelinePosition.PerTry);
            client = new ResponsesClient(credential, options);
        }
        credential.Update("refreshed");
        var connectionOptions = new ResponseWebSocketOptions();
        connectionOptions.Headers["X-Custom"] = "custom";
        await using (var connection = await client.ConnectWebSocketAsync(connectionOptions))
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That(((StreamingResponseCompletedUpdate)(await connection.ReceiveAsync()).Update).Response.Id, Is.EqualTo("resp_first"));
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That(((StreamingResponseCompletedUpdate)(await connection.ReceiveAsync()).Update).Response.Id, Is.EqualTo("resp_second"));
        }
        await server.Completed;
        Assert.That(server.ConnectionCount, Is.EqualTo(1));
    }

    [TestCase("failed")]
    [TestCase("incomplete")]
    [TestCase("completed")]
    public async Task TerminalEventsDoNotCloseConnection(string status)
    {
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, Terminal(status, "resp_terminal"));
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_next"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            var terminal = await connection.ReceiveAsync();
            Assert.That(terminal.RawData.ToString(), Does.Contain("resp_terminal"));
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That((await connection.ReceiveAsync()).Update, Is.TypeOf<StreamingResponseCompletedUpdate>());
        }
        await server.Completed;
    }

    [Test]
    public async Task NestedErrorsUnknownFieldsAndFragmentsRemainObservable()
    {
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, "{\"type\":\"error\",\"stream_id\":\"lane\",\"status\":400,\"error\":{\"type\":\"invalid_request_error\",\"message\":\"bad input\",\"code\":\"invalid_value\",\"param\":\"input\"}}");
            var bytes = Encoding.UTF8.GetBytes("{\"type\":\"future.event\",\"stream_id\":\"lane\",\"future\":{\"preserved\":true}}");
            await socket.SendAsync(bytes.AsMemory(0, 13), WebSocketMessageType.Text, false, CancellationToken.None);
            await socket.SendAsync(bytes.AsMemory(13), WebSocketMessageType.Text, true, CancellationToken.None);
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_after_error"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            var error = (ResponseWebSocketErrorEvent)await connection.ReceiveAsync();
            Assert.That(error.Error.Code, Is.EqualTo("invalid_value"));
            Assert.That(error.Error.Param, Is.EqualTo("input"));
            Assert.That(error.Status, Is.EqualTo(400));
            Assert.That(error.StreamId, Is.EqualTo("lane"));
            var future = await connection.ReceiveAsync();
            Assert.That(future.Update, Is.Null);
            Assert.That(future.RawData.ToString(), Does.Contain("\"preserved\":true"));
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That((await connection.ReceiveAsync()).Update, Is.TypeOf<StreamingResponseCompletedUpdate>());
        }
        await server.Completed;
    }

    [Test]
    public async Task LanesShareOneReaderAndCanceledWaitDoesNotStealEvent()
    {
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_b", "b"));
            await Send(socket, Terminal("incomplete", "resp_a", "a"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            using var a = connection.OpenLane("a");
            using var b = connection.OpenLane("b");
            using var cancellation = new CancellationTokenSource();
            var canceled = a.ReceiveAsync(cancellation.Token);
            cancellation.Cancel();
            Assert.ThrowsAsync<TaskCanceledException>(async () => await canceled);
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That((await b.ReceiveResponseAsync()).Id, Is.EqualTo("resp_b"));
            Assert.That((await a.ReceiveResponseAsync()).Id, Is.EqualTo("resp_a"));
        }
        await server.Completed;
    }

    [Test]
    public async Task DetachedLaneDoesNotCloseSocket()
    {
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_after_detach", "a"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            var lane = connection.OpenLane("a");
            lane.Dispose();
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That((await connection.ReceiveAsync()).StreamId, Is.EqualTo("a"));
        }
        await server.Completed;
    }

    [Test]
    public async Task DefaultFinalResponseIgnoresDetachedAndUnregisteredStreamEvents()
    {
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_raw", "detached"));
            await Send(socket, Terminal("completed", "resp_detached", "detached"));
            await Send(socket, "{\"type\":\"error\",\"stream_id\":\"unregistered\",\"error\":{\"type\":\"invalid_request_error\",\"message\":\"other stream failed\",\"code\":\"invalid_value\"}}");
            await Send(socket, Terminal("completed", "resp_default"));
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_next"));
            await AwaitClose(socket);
        });
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            connection.OpenLane("detached").Dispose();
            await connection.SendAsync(BinaryData.FromString(Create), timeout.Token);
            var raw = await connection.ReceiveAsync(timeout.Token);
            Assert.That(raw.StreamId, Is.EqualTo("detached"));
            Assert.That(((StreamingResponseCompletedUpdate)raw.Update).Response.Id, Is.EqualTo("resp_raw"));
            Assert.That((await connection.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_default"));
            await connection.SendAsync(BinaryData.FromString(Create), timeout.Token);
            Assert.That((await connection.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_next"));
        }
        await server.Completed;
        Assert.That(server.ConnectionCount, Is.EqualTo(1));
    }

    [Test]
    public async Task MessageLimitRejectsOversizedFragmentedMessage()
    {
        var allowFailure = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await allowFailure.Task.WaitAsync(TimeSpan.FromSeconds(10));
            try
            {
                await socket.SendAsync(Encoding.UTF8.GetBytes(new string('x', 100)), WebSocketMessageType.Text, false, CancellationToken.None);
                await socket.SendAsync(Encoding.UTF8.GetBytes(new string('x', 100)), WebSocketMessageType.Text, true, CancellationToken.None);
                await AwaitClose(socket);
            }
            catch (WebSocketException) { }
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync(new() { MaxMessageBytes = 128 }))
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            allowFailure.TrySetResult();
            Assert.ThrowsAsync<InvalidDataException>(async () => await connection.ReceiveAsync());
        }
        await server.Completed;
    }

    [Test]
    [NonParallelizable]
    public async Task DefaultByteLimitsAllowLargeCommandsAndFinalResponses()
    {
        // These sizes probe the former default ceilings; they are not API maxima.
        const int inputLength = 17 * 1024 * 1024;
        const int outputLength = 65 * 1024 * 1024;
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            using (var request = JsonDocument.Parse(await Receive(socket, timeoutSeconds: 60)))
            {
                Assert.That(request.RootElement.GetProperty("type").GetString(), Is.EqualTo("response.create"));
                var input = request.RootElement.GetProperty("input").GetString();
                Assert.That(input.Length, Is.EqualTo(inputLength));
                Assert.That(input.All(character => character == 'i'), Is.True);
            }
            var payload = JsonSerializer.SerializeToUtf8Bytes(new
            {
                type = "response.completed",
                sequence_number = 1,
                response = new
                {
                    id = "resp_large",
                    @object = "response",
                    created_at = 1,
                    model = "test-model",
                    status = "completed",
                    parallel_tool_calls = false,
                    tool_choice = "auto",
                    tools = Array.Empty<object>(),
                    output = new[]
                    {
                        new
                        {
                            type = "message", id = "msg_large", role = "assistant", status = "completed",
                            content = new[] { new { type = "output_text", text = new string('x', outputLength), annotations = Array.Empty<object>() } },
                        },
                    },
                },
            });
            await socket.SendAsync(payload.AsMemory(), WebSocketMessageType.Text, true, timeout.Token);
            Assert.That(await Receive(socket, timeoutSeconds: 60), Is.EqualTo(Create));
            await Send(socket, Terminal("completed", "resp_after_large"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync(cancellationToken: timeout.Token))
        {
            var command = JsonSerializer.SerializeToUtf8Bytes(new
            {
                type = "response.create",
                model = "test-model",
                input = new string('i', inputLength),
            });
            await connection.SendAsync(BinaryData.FromBytes(command), timeout.Token);
            var response = await connection.ReceiveResponseAsync(timeout.Token);
            Assert.That(response.Id, Is.EqualTo("resp_large"));
            var text = response.GetOutputText();
            Assert.That(text.Length, Is.EqualTo(outputLength));
            Assert.That(text.All(character => character == 'x'), Is.True);
            await connection.SendAsync(BinaryData.FromString(Create), timeout.Token);
            Assert.That((await connection.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_after_large"));
        }
        await server.Completed;
        Assert.That(server.ConnectionCount, Is.EqualTo(1));
    }

    [Test]
    public async Task DeepCustomPayloadsArePreservedAcrossSendAndReceivePaths()
    {
        string payload = new string('[', 80) + "true" + new string(']', 80);
        string incoming = "{\"type\":\"response.future\",\"sequence_number\":1,\"custom_payload\":" + payload + "}";
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            for (int i = 0; i < 3; i++)
            {
                using var json = JsonDocument.Parse(await Receive(socket), new JsonDocumentOptions { MaxDepth = 256 });
                Assert.That(json.RootElement.GetProperty("custom_payload").GetRawText(), Is.EqualTo(payload));
            }
            await Send(socket, incoming);
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync(cancellationToken: timeout.Token))
        {
            await connection.SendAsync(BinaryData.FromString("{\"type\":\"response.create\",\"model\":\"test-model\",\"custom_payload\":" + payload + "}"), timeout.Token);
            var command = new ResponseWebSocketCreateCommand { Model = "test-model" };
            command.Patch.Set("$.custom_payload"u8, Encoding.UTF8.GetBytes(payload));
            await connection.SendAsync(command, timeout.Token);
            using var lane = connection.OpenLane("nested");
            await lane.SendAsync(command, timeout.Token);

            var received = await connection.ReceiveAsync(timeout.Token);
            Assert.That(received.RawData.ToString(), Is.EqualTo(incoming));
            Assert.That(received.Update, Is.Not.Null);
            var roundTrip = ModelReaderWriter.Write(received.Update, ModelReaderWriterOptions.Json, OpenAIContext.Default);
            using var json = JsonDocument.Parse(roundTrip.ToMemory(), new JsonDocumentOptions { MaxDepth = 256 });
            Assert.That(json.RootElement.GetProperty("custom_payload").GetRawText(), Is.EqualTo(payload));
        }
        await server.Completed;
    }

    [Test]
    public async Task ConcurrentSendsRemainCompleteMessages()
    {
        const int count = 8;
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            var values = new HashSet<int>();
            for (int i = 0; i < count; i++)
            {
                using var json = JsonDocument.Parse(await Receive(socket));
                values.Add(json.RootElement.GetProperty("value").GetInt32());
            }
            Assert.That(values.Count, Is.EqualTo(count));
            await Send(socket, Terminal("completed", "resp_sent"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            await Task.WhenAll(Enumerable.Range(0, count).Select(i => connection.SendAsync(BinaryData.FromString("{\"value\":" + i + "}"))));
            await connection.ReceiveAsync();
            await connection.CloseAsync();
            await connection.CloseAsync();
        }
        await server.Completed;
    }

    [Test]
    public async Task PrematureCloseIsNotResponseCompletion()
    {
        var allowFailure = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await allowFailure.Task.WaitAsync(TimeSpan.FromSeconds(10));
            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "done", CancellationToken.None);
        });
        await using var connection = await Client(server).ConnectWebSocketAsync();
        await connection.SendAsync(BinaryData.FromString(Create));
        allowFailure.TrySetResult();
        Assert.ThrowsAsync<EndOfStreamException>(async () => await connection.ReceiveAsync());
        await server.Completed;
    }

    [Test]
    public async Task ExplicitRecoveryRefreshesCredentialsWithoutReplayingCreates()
    {
        int upgrades = 0;
        var credential = new ApiKeyCredential("first-key");
        var restored = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var allowFailure = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var server = await LocalServer.Start(async (context, socket) =>
        {
            int attempt = Interlocked.Increment(ref upgrades);
            Assert.That(context.Request.Headers.Authorization.ToString(), Is.EqualTo(attempt == 1 ? "Bearer first-key" : "Bearer second-key"));
            Assert.That(context.Request.Headers["X-Custom"].ToString(), Is.EqualTo(attempt == 1 ? "first-header" : "second-header"));
            Assert.That(await Receive(socket), Is.EqualTo(Create));
            await allowFailure.Task.WaitAsync(TimeSpan.FromSeconds(10));
            if (attempt == 1)
            {
                await socket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, "disconnect", CancellationToken.None);
            }
            else
            {
                await Send(socket, Terminal("completed", "resp_restored"));
                restored.TrySetResult();
                await AwaitClose(socket);
            }
        });
        var client = new ResponsesClient(credential, new ResponsesClientOptions { Endpoint = new Uri(server.Endpoint, "v1") });
        var options = new ResponseWebSocketOptions();
        options.Headers["X-Custom"] = "first-header";
        await using var original = await client.ConnectWebSocketAsync(options);
        await original.SendAsync(BinaryData.FromString(Create));
        allowFailure.TrySetResult();
        Assert.ThrowsAsync<EndOfStreamException>(async () => await original.ReceiveAsync());
        credential.Update("second-key");
        options.Headers["x-custom"] = "second-header";
        await using var replacement = await original.ReconnectAsync(async (connection, cancellationToken) =>
        {
            Assert.That(connection.HasConnectionStateLoss, Is.True);
            await connection.SendAsync(BinaryData.FromString(Create), cancellationToken);
        });
        Assert.That(((StreamingResponseCompletedUpdate)(await replacement.ReceiveAsync()).Update).Response.Id, Is.EqualTo("resp_restored"));
        await restored.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.That(upgrades, Is.EqualTo(2));
    }

    [Test]
    public async Task CustomTransportRequiresExplicitConnector()
    {
        var client = new ResponsesClient(new ApiKeyCredential("test-key"), new ResponsesClientOptions
        {
            Transport = new HttpClientPipelineTransport(new System.Net.Http.HttpClient()),
        });
        Assert.ThrowsAsync<NotSupportedException>(async () => await client.ConnectWebSocketAsync());
        await using var server = await LocalServer.Start(async (context, socket) =>
        {
            Assert.That(context.Request.Headers.Authorization.ToString(), Is.EqualTo("Bearer test-key"));
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_custom"));
            await AwaitClose(socket);
        });
        client = new ResponsesClient(new ApiKeyCredential("test-key"), new ResponsesClientOptions
        {
            Endpoint = new Uri(server.Endpoint, "v1"),
            Transport = new HttpClientPipelineTransport(new System.Net.Http.HttpClient()),
        });
        var options = new ResponseWebSocketOptions
        {
            Connector = async (uri, headers, cancellationToken) =>
            {
                var socket = new ClientWebSocket();
                foreach (var header in headers) socket.Options.SetRequestHeader(header.Key, header.Value);
                await socket.ConnectAsync(uri, cancellationToken);
                return socket;
            },
        };
        await using (var connection = await client.ConnectWebSocketAsync(options))
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            await connection.ReceiveAsync();
        }
        await server.Completed;
    }

    [Test]
    public async Task BufferedEventsOverflowIsExplicitAndKeepsAlreadyQueuedEvents()
    {
        var sent = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var allowEvents = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await allowEvents.Task.WaitAsync(TimeSpan.FromSeconds(10));
            for (int i = 0; i < 3; i++) await Send(socket, "{\"type\":\"future.event\"}");
            sent.TrySetResult();
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync(new() { MaxBufferedEvents = 2 }))
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            allowEvents.TrySetResult();
            await sent.Task.WaitAsync(TimeSpan.FromSeconds(10));
            await server.Completed; // the reader aborts the socket on overflow
            await connection.ReceiveAsync();
            await connection.ReceiveAsync();
            Assert.ThrowsAsync<InvalidDataException>(async () => await connection.ReceiveAsync());
        }
    }

    [Test]
    public async Task ExplicitBufferedByteLimitKeepsAlreadyQueuedEvents()
    {
        const string message = "{\"type\":\"future.event\"}";
        var allowEvents = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await allowEvents.Task.WaitAsync(TimeSpan.FromSeconds(10));
            await Send(socket, message);
            await Send(socket, message);
            await AwaitClose(socket);
        });
        var options = new ResponseWebSocketOptions { MaxBufferedBytes = Encoding.UTF8.GetByteCount(message) };
        await using (var connection = await Client(server).ConnectWebSocketAsync(options))
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            allowEvents.TrySetResult();
            await server.Completed; // The second event exceeds the caller's byte budget.
            Assert.That((await connection.ReceiveAsync()).RawData.ToString(), Is.EqualTo(message));
            Assert.ThrowsAsync<InvalidDataException>(async () => await connection.ReceiveAsync());
        }
    }

    [TestCase("invalid json")]
    [TestCase("{}")]
    public async Task MalformedEventsFailTheConnection(string message)
    {
        var allowFailure = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await allowFailure.Task.WaitAsync(TimeSpan.FromSeconds(10));
            await Send(socket, message);
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            allowFailure.TrySetResult();
            var error = Assert.CatchAsync<Exception>(async () => await connection.ReceiveAsync());
            Assert.That(error, Is.InstanceOf<JsonException>().Or.InstanceOf<InvalidDataException>());
            Assert.ThrowsAsync<InvalidOperationException>(async () => await connection.SendAsync(BinaryData.FromString(Create)));
        }
        await server.Completed;
    }

    [Test]
    public async Task RedirectDoesNotTransmitHeadersToAnotherOrigin()
    {
        await using var target = await LocalServer.Start(async (_, socket) => await AwaitClose(socket));
        await using var origin = await LocalServer.Start((_, _) => Task.CompletedTask, target.Endpoint);
        var options = new ResponseWebSocketOptions();
        options.Headers["X-Custom-Credential"] = "secret-for-original-origin";
        Exception failure = null;
        ResponseWebSocketConnection connection = null;
        try { connection = await Client(origin).ConnectWebSocketAsync(options); }
        catch (Exception error) { failure = error; }
        finally { if (connection != null) await connection.DisposeAsync(); }
        Assert.That(failure, Is.Not.Null);
        Assert.That(target.ConnectionCount, Is.Zero);
    }

    [Test]
    public async Task ConfiguredTlsTransportIsUsedForUpgrade()
    {
        using var key = RSA.Create(2048);
        var request = new CertificateRequest("CN=localhost", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1));
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_tls"));
            await AwaitClose(socket);
        }, certificate: certificate);
        bool validated = false;
        var options = new ResponseWebSocketOptions
        {
            ConfigureTransport = handler => handler.ServerCertificateCustomValidationCallback = (_, actual, _, _) =>
            {
                validated = actual.Thumbprint == certificate.Thumbprint;
                return validated;
            },
        };
        await using (var connection = await Client(server).ConnectWebSocketAsync(options))
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That((await connection.ReceiveResponseAsync()).Id, Is.EqualTo("resp_tls"));
        }
        await server.Completed;
        Assert.That(validated, Is.True);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task AuthenticationPolicyIsUsedForDirectAndFactoryClients(bool factory)
    {
        await using var server = await LocalServer.Start(async (context, socket) =>
        {
            Assert.That(context.Request.Headers["X-Api-Key"].ToString(), Is.EqualTo("refreshed-custom-key"));
            Assert.That(context.Request.Headers.Authorization.ToString(), Is.Empty);
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_auth_policy"));
            await AwaitClose(socket);
        });
        var credential = new ApiKeyCredential("custom-key");
        var policy = ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(credential, "X-Api-Key");
        var endpoint = new Uri(server.Endpoint, "v1");
        ResponsesClient client = factory
            ? new OpenAIClient(policy, new OpenAIClientOptions { Endpoint = endpoint }).GetResponsesClient()
            : new ResponsesClient(policy, new ResponsesClientOptions { Endpoint = endpoint });
        credential.Update("refreshed-custom-key");
        await using (var connection = await client.ConnectWebSocketAsync())
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That((await connection.ReceiveResponseAsync()).Id, Is.EqualTo("resp_auth_policy"));
        }
        await server.Completed;
    }

    [Test]
    public void ConstructedErrorEventSerializesProtocolDiscriminator()
    {
        var error = new ResponseWebSocketErrorEvent
        {
            Error = new ResponseWebSocketErrorDetails { Kind = "server_error", Message = "Test error" },
        };
        var data = ModelReaderWriter.Write(error, ModelReaderWriterOptions.Json, OpenAIContext.Default);
        using var json = JsonDocument.Parse(data.ToMemory());
        Assert.That(json.RootElement.GetProperty("type").GetString(), Is.EqualTo("error"));
    }

    [Test]
    public async Task TypedWarmupAndLaneCommandsHaveWebSocketShape()
    {
        await using var server = await LocalServer.Start(async (context, socket) =>
        {
            using var json = JsonDocument.Parse(await Receive(socket));
            var root = json.RootElement;
            Assert.That(root.GetProperty("type").GetString(), Is.EqualTo("response.create"));
            Assert.That(root.GetProperty("generate").GetBoolean(), Is.False);
            Assert.That(root.GetProperty("stream_id").GetString(), Is.EqualTo("warmup"));
            Assert.That(root.TryGetProperty("stream", out _), Is.False);
            Assert.That(root.TryGetProperty("background", out _), Is.False);
            await Send(socket, Terminal("completed", "resp_warmup", "warmup"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            using var lane = connection.OpenLane("warmup");
            var command = new ResponseWebSocketCreateCommand { Model = "test-model", Generate = false };
            await lane.SendAsync(command);
            Assert.That((await lane.ReceiveResponseAsync()).Id, Is.EqualTo("resp_warmup"));
            Assert.That(command.StreamId, Is.Null);
        }
        await server.Completed;
    }

    [TestCase(false)]
    [TestCase(true)]
    public Task CustomRetryHooksAreRejectedBeforeAuthentication(bool factory)
    {
        var authentication = new CountingAuthenticationPolicy();
        var retry = new HeaderRetryPolicy();
        ResponsesClient client = factory
            ? new OpenAIClient(authentication, new OpenAIClientOptions { RetryPolicy = retry }).GetResponsesClient()
            : new ResponsesClient(authentication, new ResponsesClientOptions { RetryPolicy = retry });
        int connections = 0;
        var options = new ResponseWebSocketOptions
        {
            Connector = (_, _, _) =>
            {
                connections++;
                return Task.FromResult<WebSocket>(null);
            },
        };
        var error = Assert.ThrowsAsync<NotSupportedException>(async () => await client.ConnectWebSocketAsync(options));
        Assert.That(error.Message, Does.Contain("custom retry policy"));
        Assert.That(authentication.Calls, Is.Zero);
        Assert.That(retry.Calls, Is.Zero);
        Assert.That(connections, Is.Zero);
        return Task.CompletedTask;
    }

    [Test]
    public async Task PendingSendLimitAndCanceledWaitDoNotSendCommands()
    {
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            Assert.That(await Receive(socket), Is.EqualTo(Create));
            Assert.That(await Receive(socket), Is.EqualTo(Create));
            await Send(socket, Terminal("completed", "resp_two_commands"));
            await AwaitClose(socket);
        });
        PausedSendSocket transport = null;
        var options = new ResponseWebSocketOptions
        {
            MaxPendingSends = 2,
            Connector = async (uri, headers, token) =>
            {
                var socket = new ClientWebSocket();
                foreach (var header in headers) socket.Options.SetRequestHeader(header.Key, header.Value);
                await socket.ConnectAsync(uri, token);
                return transport = new PausedSendSocket(socket);
            },
        };
        await using (var connection = await Client(server).ConnectWebSocketAsync(options))
        {
            // Preparation failures release their slot without closing the socket.
            Assert.CatchAsync<JsonException>(async () => await connection.SendAsync(BinaryData.FromString("invalid json")));
            Assert.ThrowsAsync<ArgumentException>(async () => await connection.SendAsync(BinaryData.FromString("[]")));
            var first = connection.SendAsync(BinaryData.FromString(Create));
            await transport.Started.Task.WaitAsync(TimeSpan.FromSeconds(10));
            using var cancellation = new CancellationTokenSource();
            var canceled = connection.SendAsync(BinaryData.FromString(Create), cancellation.Token);
            cancellation.Cancel();
            Assert.CatchAsync<OperationCanceledException>(async () => await canceled);
            Assert.CatchAsync<OperationCanceledException>(async () => await connection.SendAsync(BinaryData.FromString("invalid json"), cancellation.Token));
            var second = connection.SendAsync(BinaryData.FromString(Create));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await connection.SendAsync(BinaryData.FromString(Create)));
            // Saturated calls must be rejected before parsing their payload.
            Assert.ThrowsAsync<InvalidOperationException>(async () => await connection.SendAsync(BinaryData.FromString("invalid json")));
            transport.Release.TrySetResult();
            await Task.WhenAll(first, second);
            Assert.That((await connection.ReceiveResponseAsync()).Id, Is.EqualTo("resp_two_commands"));
        }
        await server.Completed;
    }

    private sealed class PausedSendSocket : WebSocket
    {
        private readonly WebSocket _socket;
        private int _sendCount;
        internal TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal PausedSendSocket(WebSocket socket) => _socket = socket;
        public override WebSocketCloseStatus? CloseStatus => _socket.CloseStatus;
        public override string CloseStatusDescription => _socket.CloseStatusDescription;
        public override WebSocketState State => _socket.State;
        public override string SubProtocol => _socket.SubProtocol;
        public override void Abort() => _socket.Abort();
        public override void Dispose() => _socket.Dispose();
        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
            => _socket.CloseAsync(closeStatus, statusDescription, cancellationToken);
        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
            => _socket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);
        public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
            => _socket.ReceiveAsync(buffer, cancellationToken);
        public override async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _sendCount) == 1)
            {
                Started.TrySetResult();
                await Release.Task.WaitAsync(cancellationToken);
            }
            await _socket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
        }
    }

    private static ResponsesClient Client(LocalServer server) => new(new ApiKeyCredential("test-key"), new ResponsesClientOptions { Endpoint = new Uri(server.Endpoint, "v1") });
    private static async Task Send(WebSocket socket, string message) => await socket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
    private static async Task<string> Receive(WebSocket socket, int timeoutSeconds = 10)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        using var content = new MemoryStream();
        var buffer = new byte[1024];
        WebSocketReceiveResult frame;
        do
        {
            frame = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), timeout.Token);
            if (frame.MessageType == WebSocketMessageType.Close) throw new EndOfStreamException();
            content.Write(buffer, 0, frame.Count);
        } while (!frame.EndOfMessage);
        return Encoding.UTF8.GetString(content.ToArray());
    }
    private static async Task AwaitClose(WebSocket socket)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        try
        {
            var frame = await socket.ReceiveAsync(new ArraySegment<byte>(new byte[128]), timeout.Token);
            Assert.That(frame.MessageType, Is.EqualTo(WebSocketMessageType.Close));
            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", timeout.Token);
        }
        catch (WebSocketException) { } // Abort/dispose is also a supported local shutdown.
    }

    private sealed class HeaderPolicy : PipelinePolicy
    {
        public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
        { message.Request.Headers.Set("X-Policy", "ran"); ProcessNext(message, pipeline, currentIndex); }
        public override ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
        { message.Request.Headers.Set("X-Policy", "ran"); return ProcessNextAsync(message, pipeline, currentIndex); }
    }

    private sealed class HeaderRetryPolicy : ClientRetryPolicy
    {
        internal int Calls { get; private set; }
        internal HeaderRetryPolicy() : base(0) { }
        protected override ValueTask OnSendingRequestAsync(PipelineMessage message)
        {
            Calls++;
            message.Request.Headers.Set("X-Retry-Policy", "required");
            return default;
        }
    }

    private sealed class CountingAuthenticationPolicy : AuthenticationPolicy
    {
        internal int Calls { get; private set; }
        public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
        { Calls++; ProcessNext(message, pipeline, currentIndex); }
        public override ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
        { Calls++; return ProcessNextAsync(message, pipeline, currentIndex); }
    }

    private sealed class LocalServer : IAsyncDisposable
    {
        private readonly WebApplication _application;
        private readonly TaskCompletionSource _completed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _connections;
        public Uri Endpoint { get; private set; }
        public int ConnectionCount => _connections;
        public Task Completed => _completed.Task.WaitAsync(TimeSpan.FromSeconds(15));
        private LocalServer(WebApplication application) => _application = application;
        public static async Task<LocalServer> Start(Func<HttpContext, WebSocket, Task> handler, Uri redirectTo = null, X509Certificate2 certificate = null)
        {
            var builder = WebApplication.CreateBuilder();
            builder.Logging.ClearProviders();
            if (certificate == null) builder.WebHost.UseUrls("http://127.0.0.1:0");
            else builder.WebHost.ConfigureKestrel(options => options.Listen(System.Net.IPAddress.Loopback, 0, listener => listener.UseHttps(certificate)));
            var server = new LocalServer(builder.Build());
            server._application.UseWebSockets();
            server._application.Run(async context =>
            {
                try
                {
                    if (redirectTo != null)
                    {
                        context.Response.StatusCode = 307;
                        context.Response.Headers.Location = redirectTo.ToString();
                        return;
                    }
                    using var socket = await context.WebSockets.AcceptWebSocketAsync();
                    Interlocked.Increment(ref server._connections);
                    await handler(context, socket);
                    server._completed.TrySetResult();
                }
                catch (Exception error) { server._completed.TrySetException(error); }
            });
            await server._application.StartAsync();
            server.Endpoint = new Uri(server._application.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>().Addresses.Single() + "/");
            return server;
        }
        public async ValueTask DisposeAsync()
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            await _application.StopAsync(timeout.Token);
            await _application.DisposeAsync();
        }
    }
}
