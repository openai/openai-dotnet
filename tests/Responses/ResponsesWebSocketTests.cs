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
        connectionOptions.Headers["authorization"] = "Bearer overridden";
        connectionOptions.Headers["openai-organization"] = "overridden-organization";
        connectionOptions.Headers["openai-project"] = "overridden-project";
        connectionOptions.Headers["x-policy"] = "overridden-policy";
        connectionOptions.Headers["X-Custom"] = "original-custom";
        connectionOptions.Headers["x-custom"] = "custom";
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
            await Send(socket, "{\"type\":\"response.output_text.delta\",\"sequence_number\":0,\"item_id\":\"msg_test\",\"output_index\":0,\"content_index\":0,\"delta\":\"Checking.\",\"logprobs\":[]}");
            await Send(socket, "{\"type\":\"response.function_call_arguments.delta\",\"sequence_number\":1,\"item_id\":\"fc_test\",\"output_index\":1,\"delta\":\"{}\"}");
            await Send(socket, "{\"type\":\"response.future_update\",\"sequence_number\":2,\"extra\":true}");
            await Send(socket, Terminal(status, "resp_terminal").Replace("\"output\":[]", "\"output\":[" +
                "{\"type\":\"message\",\"id\":\"msg_test\",\"role\":\"assistant\",\"status\":\"completed\",\"content\":[{\"type\":\"output_text\",\"text\":\"Checking.\",\"annotations\":[]},{\"type\":\"output_text\",\"text\":\"Done.\",\"annotations\":[]}]}," +
                "{\"type\":\"function_call\",\"id\":\"fc_test\",\"call_id\":\"call_test\",\"name\":\"lookup\",\"arguments\":\"{}\",\"status\":\"completed\"}]"));
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_next"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            var terminal = await connection.ReceiveResponseAsync();
            Assert.That(terminal.Id, Is.EqualTo("resp_terminal"));
            Assert.That(terminal.Status, Is.EqualTo(Enum.Parse<ResponseStatus>(status, ignoreCase: true)));
            Assert.That(terminal.GetOutputText(), Is.EqualTo("Checking.Done."));
            Assert.That(((MessageResponseItem)terminal.OutputItems[0]).Content.Count, Is.EqualTo(2));
            Assert.That(terminal.OutputItems.Count, Is.EqualTo(2));
            Assert.That(((FunctionCallResponseItem)terminal.OutputItems[1]).CallId, Is.EqualTo("call_test"));
            Assert.That(((FunctionCallResponseItem)terminal.OutputItems[1]).FunctionArguments.ToString(), Is.EqualTo("{}"));
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That((await connection.ReceiveAsync()).Update, Is.TypeOf<StreamingResponseCompletedUpdate>());
        }
        await server.Completed;
    }

    [Test]
    public async Task MissingTerminalResponseFailsHelperAndPreservesRawEventsAndConnection(
        [Values("completed", "failed", "incomplete")] string status,
        [Values(false, true)] bool nullResponse,
        [Values(false, true)] bool useLane)
    {
        string streamId = useLane ? "answer" : null;
        string message = "{\"type\":\"response." + status + "\",\"sequence_number\":1" +
            (useLane ? ",\"stream_id\":\"answer\"" : "") +
            (nullResponse ? ",\"response\":null}" : "}");
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, message);
            await Receive(socket);
            await Send(socket, message);
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_after_missing_response", streamId));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            using var lane = useLane ? connection.OpenLane(streamId) : null;
            var command = new ResponseWebSocketCreateCommand { Model = "test-model" };
            Task SendCommand() => useLane ? lane.SendAsync(command) : connection.SendAsync(command);
            Task<ResponseResult> ReceiveResponse() => useLane ? lane.ReceiveResponseAsync() : connection.ReceiveResponseAsync();

            await SendCommand();
            Assert.ThrowsAsync<InvalidDataException>(async () => await ReceiveResponse());
            await SendCommand();
            var rawEvent = await (useLane ? lane.ReceiveAsync() : connection.ReceiveAsync());
            Assert.That(rawEvent.RawData.ToString(), Is.EqualTo(message));
            await SendCommand();
            Assert.That((await ReceiveResponse()).Id, Is.EqualTo("resp_after_missing_response"));
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

    [TestCase("{\"type\":\"error\"}")]
    [TestCase("{\"type\":\"error\",\"error\":null}")]
    public async Task MissingErrorDetailsPreserveProtocolExceptionAndConnection(string message)
    {
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, message);
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_after_error"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            await connection.SendAsync(BinaryData.FromString(Create));
            var error = Assert.ThrowsAsync<ResponseWebSocketException>(async () => await connection.ReceiveResponseAsync());
            Assert.That(error.Error.RawData.ToString(), Is.EqualTo(message));
            Assert.That(error.Message, Is.EqualTo("The server returned a WebSocket protocol error without a message."));
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That((await connection.ReceiveResponseAsync()).Id, Is.EqualTo("resp_after_error"));
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
    public async Task EventEnumeratorOwnsDefaultStreamUntilDisposed()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, "{\"type\":\"future.event\"}");
            await Send(socket, Terminal("completed", "resp_lane", "other"));
            await Send(socket, Terminal("completed", "resp_default"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            using var lane = connection.OpenLane("other");
            await connection.SendAsync(BinaryData.FromString(Create), timeout.Token);
            await using (var events = connection.GetEventsAsync(timeout.Token).GetAsyncEnumerator())
            {
                Assert.That(await events.MoveNextAsync(), Is.True);
                Assert.That(events.Current.RawData.ToString(), Is.EqualTo("{\"type\":\"future.event\"}"));
                Assert.ThrowsAsync<InvalidOperationException>(async () => await connection.ReceiveResponseAsync(timeout.Token));
                Assert.ThrowsAsync<InvalidOperationException>(async () => await connection.ReceiveAsync(timeout.Token));
                await using var competing = connection.GetEventsAsync(timeout.Token).GetAsyncEnumerator();
                Assert.ThrowsAsync<InvalidOperationException>(async () => await competing.MoveNextAsync());
                Assert.That((await lane.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_lane"));
            }
            Assert.That((await connection.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_default"));
        }
        await server.Completed;
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task CanceledResponseHelperReleasesStreamOwnership(bool namedLane)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_after_cancel", namedLane ? "lane" : null));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            using var lane = namedLane ? connection.OpenLane("lane") : null;
            Func<CancellationToken, Task<ResponseResult>> receiveResponse = namedLane ? lane.ReceiveResponseAsync : connection.ReceiveResponseAsync;
            Func<CancellationToken, Task<ResponseWebSocketServerEvent>> receive = namedLane ? lane.ReceiveAsync : connection.ReceiveAsync;
            using var cancellation = new CancellationTokenSource();
            var pending = receiveResponse(cancellation.Token);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await receive(timeout.Token));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await receiveResponse(timeout.Token));
            cancellation.Cancel();
            Assert.CatchAsync<OperationCanceledException>(async () => await pending);
            await connection.SendAsync(BinaryData.FromString(Create), timeout.Token);
            Assert.That((await receiveResponse(timeout.Token)).Id, Is.EqualTo("resp_after_cancel"));
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
    public async Task DefaultEventLimitAllowsBufferedBurstAndConnectionReuse()
    {
        const int eventCount = 200;
        string Event(int index) => "{\"type\":\"future.event\",\"stream_id\":\"burst\",\"index\":" + index + "}";
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            for (int i = 0; i < eventCount; i++) await Send(socket, Event(i));
            await Send(socket, Terminal("completed", "resp_burst_buffered"));
            await Receive(socket);
            await Send(socket, Terminal("completed", "resp_after_burst"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            using var lane = connection.OpenLane("burst");
            await connection.SendAsync(BinaryData.FromString(Create));
            // The shared reader must queue the whole lane burst before it reaches this default-stream event.
            Assert.That((await connection.ReceiveResponseAsync()).Id, Is.EqualTo("resp_burst_buffered"));
            for (int i = 0; i < eventCount; i++)
                Assert.That((await lane.ReceiveAsync()).RawData.ToString(), Is.EqualTo(Event(i)));
            await connection.SendAsync(BinaryData.FromString(Create));
            Assert.That((await connection.ReceiveResponseAsync()).Id, Is.EqualTo("resp_after_burst"));
        }
        await server.Completed;
        Assert.That(server.ConnectionCount, Is.EqualTo(1));
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

    [TestCase(false)]
    [TestCase(true)]
    public async Task DefaultEventBufferOverflowsUnlessExplicitlyUnbounded(bool unbounded)
    {
        const int defaultLimit = 1024;
        string Event(int index) => "{\"type\":\"future.event\",\"stream_id\":\"burst\",\"index\":" + index + "}";
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            for (int i = 0; i <= defaultLimit; i++) await Send(socket, Event(i));
            if (unbounded) await Send(socket, Terminal("completed", "resp_buffered"));
            await AwaitClose(socket);
        });
        var options = new ResponseWebSocketOptions();
        if (unbounded) options.MaxBufferedEvents = int.MaxValue;
        await using (var connection = await Client(server).ConnectWebSocketAsync(options))
        {
            using var lane = connection.OpenLane("burst");
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await connection.SendAsync(BinaryData.FromString(Create), timeout.Token);
            if (unbounded)
                Assert.That((await connection.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_buffered"));
            else
                Assert.ThrowsAsync<InvalidDataException>(async () => await connection.ReceiveAsync(timeout.Token));
            // No lane consumer has run: all accepted events must still be queued, in order.
            for (int i = 0; i < defaultLimit + (unbounded ? 1 : 0); i++)
                Assert.That((await lane.ReceiveAsync(timeout.Token)).RawData.ToString(), Is.EqualTo(Event(i)));
            if (!unbounded)
            {
                Assert.ThrowsAsync<InvalidDataException>(async () => await lane.ReceiveAsync(timeout.Token));
                Assert.ThrowsAsync<InvalidOperationException>(async () => await connection.SendAsync(BinaryData.FromString(Create)));
            }
        }
        await server.Completed;
        Assert.That(server.ConnectionCount, Is.EqualTo(1));
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
        Assert.ThrowsAsync<WebSocketException>(async () =>
        {
            await using var connection = await Client(origin).ConnectWebSocketAsync(options);
        });
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
            using var next = JsonDocument.Parse(await Receive(socket));
            Assert.That(next.RootElement.GetProperty("previous_response_id").GetString(), Is.EqualTo("resp_warmup"));
            Assert.That(next.RootElement.GetProperty("input").GetArrayLength(), Is.EqualTo(1));
            Assert.That(next.RootElement.GetProperty("input")[0].GetProperty("content")[0].GetProperty("text").GetString(), Is.EqualTo("Check the weather."));
            await Send(socket, Terminal("completed", "resp_tool", "warmup").Replace("\"output\":[]",
                "\"output\":[{\"type\":\"function_call\",\"id\":\"fc_weather\",\"call_id\":\"call_weather\",\"name\":\"weather\",\"arguments\":\"{}\"}]"));
            using var tool = JsonDocument.Parse(await Receive(socket));
            Assert.That(tool.RootElement.GetProperty("previous_response_id").GetString(), Is.EqualTo("resp_tool"));
            var input = tool.RootElement.GetProperty("input");
            Assert.That(input.GetArrayLength(), Is.EqualTo(1));
            Assert.That(input[0].GetProperty("type").GetString(), Is.EqualTo("function_call_output"));
            Assert.That(input[0].GetProperty("call_id").GetString(), Is.EqualTo("call_weather"));
            Assert.That(input[0].GetProperty("output").GetString(), Is.EqualTo("Sunny"));
            await Send(socket, Terminal("completed", "resp_after_tool", "warmup"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            using var lane = connection.OpenLane("warmup");
            var command = new ResponseWebSocketCreateCommand { Model = "test-model", Generate = false };
            await lane.SendAsync(command);
            var warmup = await lane.ReceiveResponseAsync();
            Assert.That(warmup.Id, Is.EqualTo("resp_warmup"));
            Assert.That(command.StreamId, Is.Null);
            var next = new ResponseWebSocketCreateCommand { Model = "test-model", PreviousResponseId = warmup.Id };
            next.InputItems.Add(ResponseItem.CreateUserMessageItem("Check the weather."));
            await lane.SendAsync(next);
            var response = await lane.ReceiveResponseAsync();
            var tool = new ResponseWebSocketCreateCommand { Model = "test-model", PreviousResponseId = response.Id };
            tool.InputItems.Add(ResponseItem.CreateFunctionCallOutputItem(((FunctionCallResponseItem)response.OutputItems[0]).CallId, "Sunny"));
            await lane.SendAsync(tool);
            Assert.That((await lane.ReceiveResponseAsync()).Id, Is.EqualTo("resp_after_tool"));
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
    public async Task SteeringCommandsSupportTypedContentAndRawPayloads()
    {
        const string text = "Continue with \"details\"\nand examples.";
        const string raw = "{\"type\":\"response.steer\",\"previous_response_id\":\"resp_active\",\"input\":\"raw text\",\"custom\":true}";
        await using var server = await LocalServer.Start(async (context, socket) =>
        {
            using (var json = JsonDocument.Parse(await Receive(socket)))
            {
                var root = json.RootElement;
                Assert.That(root.GetProperty("type").GetString(), Is.EqualTo("response.steer"));
                Assert.That(root.TryGetProperty("stream_id", out _), Is.False);
                Assert.That(root.GetProperty("previous_response_id").GetString(), Is.EqualTo("resp_active"));
                var message = root.GetProperty("input")[0];
                Assert.That(message.GetProperty("role").GetString(), Is.EqualTo("user"));
                Assert.That(message.GetProperty("content")[0].GetProperty("type").GetString(), Is.EqualTo("input_text"));
                Assert.That(message.GetProperty("content")[0].GetProperty("text").GetString(), Is.EqualTo(text));
            }
            using (var json = JsonDocument.Parse(await Receive(socket)))
            {
                var message = json.RootElement.GetProperty("input")[0];
                Assert.That(message.GetProperty("role").GetString(), Is.EqualTo("user"));
                var content = message.GetProperty("content");
                Assert.That(content[0].GetProperty("text").GetString(), Is.EqualTo("Look at these files."));
                Assert.That(content[1].GetProperty("type").GetString(), Is.EqualTo("input_image"));
                Assert.That(content[1].GetProperty("file_id").GetString(), Is.EqualTo("file_image"));
                Assert.That(content[2].GetProperty("type").GetString(), Is.EqualTo("input_file"));
                Assert.That(content[2].GetProperty("file_id").GetString(), Is.EqualTo("file_document"));
            }
            Assert.That(await Receive(socket), Is.EqualTo(raw));
            await Send(socket, Terminal("completed", "resp_steered"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            using var lane = connection.OpenLane("active");
            await lane.SendAsync(new ResponseWebSocketSteerCommand("resp_active", text));
            var message = new ResponseWebSocketSteerMessage();
            message.Content.Add(ResponseContentPart.CreateInputTextPart("Look at these files."));
            message.Content.Add(ResponseContentPart.CreateInputImagePart("file_image"));
            message.Content.Add(ResponseContentPart.CreateInputFilePart("file_document"));
            var command = new ResponseWebSocketSteerCommand("resp_active", new[] { message });
            await connection.SendAsync(command);
            await connection.SendAsync(BinaryData.FromString(raw));
            Assert.That((await connection.ReceiveResponseAsync()).Id, Is.EqualTo("resp_steered"));
        }
        await server.Completed;
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

    [TestCase("previous_response_not_found", "invalid_stream_id")]
    [TestCase("websocket_stream_limit_reached", "websocket_connection_limit_reached")]
    public async Task ProtocolErrorsKeepTheirScopeAndAllowExplicitRecovery(string laneCode, string connectionCode)
    {
        string Error(string code, string stream = null) => "{\"type\":\"error\",\"status\":400," +
            (stream == null ? "" : "\"stream_id\":\"" + stream + "\",") +
            "\"error\":{\"type\":\"invalid_request_error\",\"code\":\"" + code + "\",\"message\":\"Test error\"}}";
        await using var server = await LocalServer.Start(async (context, socket) =>
        {
            await Receive(socket);
            await Send(socket, Error(laneCode, "main"));
            await Send(socket, Error(connectionCode));
            await Send(socket, Terminal("completed", "resp_other", "other"));
            using var recovery = JsonDocument.Parse(await Receive(socket));
            Assert.That(recovery.RootElement.TryGetProperty("previous_response_id", out _), Is.False);
            Assert.That(recovery.RootElement.GetProperty("input")[0].GetProperty("content")[0].GetProperty("text").GetString(), Is.EqualTo("Retained context"));
            await Send(socket, Terminal("completed", "resp_recovered", "main"));
            await Send(socket, Terminal("completed", "resp_default"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            using var main = connection.OpenLane("main");
            using var other = connection.OpenLane("other");
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await main.SendAsync(new ResponseWebSocketCreateCommand { Model = "test-model", PreviousResponseId = "resp_parent" }, timeout.Token);
            var error = Assert.ThrowsAsync<ResponseWebSocketException>(async () => await main.ReceiveResponseAsync(timeout.Token));
            Assert.That(error.Error.Error.Code, Is.EqualTo(laneCode));
            Assert.That(((ResponseWebSocketErrorEvent)await connection.ReceiveAsync(timeout.Token)).Error.Code, Is.EqualTo(connectionCode));
            Assert.That((await other.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_other"));
            var recovery = new ResponseWebSocketCreateCommand { Model = "test-model", Store = false };
            recovery.InputItems.Add(ResponseItem.CreateUserMessageItem("Retained context"));
            await main.SendAsync(recovery, timeout.Token);
            Assert.That((await main.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_recovered"));
            Assert.That((await connection.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_default"));
        }
        await server.Completed;
    }

    [Test]
    public async Task ForkBarrierAndCompactionPayloadsRemainApplicationControlled()
    {
        const string compacted = "[{\"type\":\"compaction\",\"encrypted_content\":\"opaque-context\"}]";
        await using var server = await LocalServer.Start(async (context, socket) =>
        {
            using var fork = JsonDocument.Parse(await Receive(socket));
            Assert.That(fork.RootElement.GetProperty("stream_id").GetString(), Is.EqualTo("fork"));
            Assert.That(fork.RootElement.GetProperty("previous_response_id").GetString(), Is.EqualTo("resp_parent"));
            await Send(socket, Terminal("in_progress", "resp_fork", "fork"));
            using var source = JsonDocument.Parse(await Receive(socket));
            Assert.That(source.RootElement.GetProperty("stream_id").GetString(), Is.EqualTo("main"));
            Assert.That(source.RootElement.GetProperty("previous_response_id").GetString(), Is.EqualTo("resp_parent"));
            Assert.That(source.RootElement.GetProperty("context_management")[0].GetProperty("compact_threshold").GetInt32(), Is.EqualTo(10000));
            await Send(socket, Terminal("completed", "resp_main", "main"));
            await Send(socket, Terminal("completed", "resp_fork", "fork"));
            using var fresh = JsonDocument.Parse(await Receive(socket));
            Assert.That(fresh.RootElement.TryGetProperty("previous_response_id", out _), Is.False);
            Assert.That(fresh.RootElement.GetProperty("input").GetRawText(), Is.EqualTo(compacted));
            await Send(socket, Terminal("completed", "resp_compacted"));
            await AwaitClose(socket);
        });
        await using (var connection = await Client(server).ConnectWebSocketAsync())
        {
            using var main = connection.OpenLane("main");
            using var fork = connection.OpenLane("fork");
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var command = new ResponseWebSocketCreateCommand { Model = "test-model", Store = false, PreviousResponseId = "resp_parent" };
            await fork.SendAsync(command, timeout.Token);
            Assert.That((await fork.ReceiveAsync(timeout.Token)).Update, Is.TypeOf<StreamingResponseInProgressUpdate>());
            command.ContextManagement.Add(new ResponseContextManagement("compaction") { CompactThreshold = 10000 });
            await main.SendAsync(command, timeout.Token);
            Assert.That((await main.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_main"));
            Assert.That((await fork.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_fork"));
            await connection.SendAsync(BinaryData.FromString("{\"type\":\"response.create\",\"model\":\"test-model\",\"store\":false,\"input\":" + compacted + "}"), timeout.Token);
            Assert.That((await connection.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_compacted"));
        }
        await server.Completed;
    }

    [Test]
    public async Task CancelingAnActiveWriteAbortsWaitersWithoutReplaying()
    {
        await using var server = await LocalServer.Start((_, socket) => AwaitClose(socket));
        PausedSendSocket transport = null;
        var options = new ResponseWebSocketOptions
        {
            Connector = async (uri, headers, token) =>
            {
                var socket = new ClientWebSocket();
                await socket.ConnectAsync(uri, token);
                return transport = new PausedSendSocket(socket);
            },
        };
        await using (var connection = await Client(server).ConnectWebSocketAsync(options))
        {
            using var cancellation = new CancellationTokenSource();
            var send = connection.SendAsync(BinaryData.FromString(Create), cancellation.Token);
            await transport.Started.Task.WaitAsync(TimeSpan.FromSeconds(10));
            var receive = connection.ReceiveAsync();
            cancellation.Cancel();
            Assert.CatchAsync<OperationCanceledException>(async () => await send);
            Assert.CatchAsync<OperationCanceledException>(async () => await receive.WaitAsync(TimeSpan.FromSeconds(10)));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await connection.SendAsync(BinaryData.FromString(Create)));
            await server.Completed; // No command may reach the server, even after shutdown.
        }
        Assert.That(server.ConnectionCount, Is.EqualTo(1));
    }

    [Test]
    public async Task RecoveryBoundsOpeningRetriesAndDoesNotRetryRestoration()
    {
        await using var server = await LocalServer.Start((_, socket) => AwaitClose(socket));
        var options = new ResponseWebSocketOptions();
        await using var original = await Client(server).ConnectWebSocketAsync(options);
        int attempts = 0;
        int restorations = 0;
        var openingFailure = new IOException("Opening failed");
        options.Connector = (_, _, _) => { attempts++; return Task.FromException<WebSocket>(openingFailure); };
        Task Restore(ResponseWebSocketConnection _, CancellationToken token) { restorations++; return Task.CompletedTask; }
        Assert.That(Assert.ThrowsAsync<IOException>(async () => await original.ReconnectAsync(Restore, maxAttempts: 3)), Is.SameAs(openingFailure));
        Assert.That(attempts, Is.EqualTo(3));
        Assert.That(restorations, Is.Zero);
        using var canceled = new CancellationTokenSource();
        canceled.Cancel();
        Assert.CatchAsync<OperationCanceledException>(async () => await original.ReconnectAsync(Restore, 3, canceled.Token));
        Assert.That(attempts, Is.EqualTo(3));

        var restorationFailure = new InvalidOperationException("Restoration failed");
        options.Connector = async (uri, headers, token) =>
        {
            attempts++;
            var socket = new ClientWebSocket();
            await socket.ConnectAsync(uri, token);
            return new PausedSendSocket(socket) { DisposeFailure = new IOException("Cleanup failed") };
        };
        var error = Assert.ThrowsAsync<InvalidOperationException>(async () => await original.ReconnectAsync(
            (_, _) => { restorations++; throw restorationFailure; }, maxAttempts: 3));
        Assert.That(error, Is.SameAs(restorationFailure));
        Assert.That(attempts, Is.EqualTo(4));
        Assert.That(restorations, Is.EqualTo(1));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task RecoveryCancellationAndOldConnectionCloseRespectReplacementOwnership(bool cancelRestoration)
    {
        var replacementClosed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        int accepted = 0;
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            if (Interlocked.Increment(ref accepted) == 1) { await AwaitClose(socket); return; }
            try
            {
                await Receive(socket);
                await Send(socket, Terminal("completed", "resp_replacement", "answer"));
                await AwaitClose(socket);
                replacementClosed.TrySetResult();
            }
            catch (Exception error) { replacementClosed.TrySetException(error); throw; }
        });
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token);
        var restoring = new TaskCompletionSource<ResponseWebSocketConnection>(TaskCreationOptions.RunContinuationsAsynchronously);
        var resume = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var original = await Client(server).ConnectWebSocketAsync(cancellationToken: timeout.Token);
        using var oldLane = original.OpenLane("answer");
        var oldReceive = oldLane.ReceiveAsync(timeout.Token);
        var recovery = original.ReconnectAsync(async (replacement, token) =>
        {
            await replacement.SendAsync(BinaryData.FromString(Create), token);
            restoring.SetResult(replacement);
            await resume.Task.WaitAsync(token);
        }, cancellationToken: cancellation.Token);
        var opened = await restoring.Task.WaitAsync(timeout.Token);
        Assert.ThrowsAsync<ObjectDisposedException>(async () => await oldReceive);
        await original.CloseAsync(timeout.Token);
        original.Dispose();
        if (cancelRestoration)
        {
            cancellation.Cancel();
            Assert.CatchAsync<OperationCanceledException>(async () => await recovery);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await opened.SendAsync(BinaryData.FromString(Create), timeout.Token));
        }
        else
        {
            resume.SetResult();
            await using var replacement = await recovery;
            Assert.That(replacement, Is.SameAs(opened));
            // Old lane registrations and disposal cannot claim events on the new connection.
            var result = await replacement.ReceiveAsync(timeout.Token);
            Assert.That(result.StreamId, Is.EqualTo("answer"));
            Assert.That(((StreamingResponseCompletedUpdate)result.Update).Response.Id, Is.EqualTo("resp_replacement"));
        }
        await replacementClosed.Task.WaitAsync(timeout.Token);
        Assert.That(server.ConnectionCount, Is.EqualTo(2));
    }

    [Test]
    public async Task LaneAdmissionAndDetachmentLeaveOtherResponseHelperRunning()
    {
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, "{\"type\":\"response.output_text.delta\",\"sequence_number\":1,\"stream_id\":\"b\",\"item_id\":\"msg_b\",\"output_index\":0,\"content_index\":0,\"delta\":\"B\",\"logprobs\":[]}");
            await Send(socket, Terminal("completed", "resp_a", "a"));
            await Receive(socket); // B continues only after A completes and its helper is detached.
            await Send(socket, Terminal("completed", "resp_b", "b"));
            await Send(socket, Terminal("completed", "resp_unclaimed", "a"));
            await AwaitClose(socket);
        });
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await using (var connection = await Client(server).ConnectWebSocketAsync(new ResponseWebSocketOptions { MaxLanes = 2 }, timeout.Token))
        {
            using var a = connection.OpenLane("a");
            using var b = connection.OpenLane("b");
            Assert.Throws<InvalidOperationException>(() => connection.OpenLane("c"));
            var aResponse = a.ReceiveResponseAsync(timeout.Token);
            var bResponse = b.ReceiveResponseAsync(timeout.Token);
            await connection.SendAsync(BinaryData.FromString(Create), timeout.Token);
            Assert.That((await aResponse).Id, Is.EqualTo("resp_a"));
            Assert.That(bResponse.IsCompleted, Is.False);
            a.Dispose();
            using var c = connection.OpenLane("c"); // Only the local helper slot is reclaimed.
            await connection.SendAsync(BinaryData.FromString(Create), timeout.Token);
            Assert.That((await bResponse).Id, Is.EqualTo("resp_b"));
            var unclaimed = await connection.ReceiveAsync(timeout.Token);
            Assert.That(unclaimed.StreamId, Is.EqualTo("a"));
            Assert.That(((StreamingResponseCompletedUpdate)unclaimed.Update).Response.Id, Is.EqualTo("resp_unclaimed"));
        }
        await server.Completed;
        Assert.That(server.ConnectionCount, Is.EqualTo(1));
    }

    // Version 1 shared synthetic protocol scenarios; exercised through the default transport.
    private static IEnumerable<TestCaseData> WebSocketScenarios()
    {
        using var resource = typeof(ResponsesWebSocketTests).Assembly.GetManifestResourceStream("ResponsesWebSocketScenarios.json");
        using var corpus = JsonDocument.Parse(resource);
        Assert.That(corpus.RootElement.GetProperty("version").GetInt32(), Is.EqualTo(1));
        foreach (var scenario in corpus.RootElement.GetProperty("scenarios").EnumerateArray())
            yield return new TestCaseData(scenario.Clone()).SetName("WebSocketContract_" + scenario.GetProperty("id").GetString());
    }

    [TestCaseSource(nameof(WebSocketScenarios))]
    public async Task SharedWebSocketContract(JsonElement scenario)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        static void AssertJson(string actual, JsonElement expected) => Assert.That(
            System.Text.Json.Nodes.JsonNode.DeepEquals(System.Text.Json.Nodes.JsonNode.Parse(actual),
                System.Text.Json.Nodes.JsonNode.Parse(expected.GetRawText())), Is.True, "JSON fields must be preserved.");
        await using var server = await LocalServer.Start(async (context, socket) =>
        {
            Assert.That(context.Request.Path.Value, Is.EqualTo("/v1/responses"));
            Assert.That(context.Request.Query["contract"].ToString(), Is.EqualTo("1"));
            Assert.That(context.Request.Headers.Authorization.ToString(), Is.EqualTo("Bearer fake-contract-key"));
            Assert.That(context.Request.Headers["X-Contract-Test"].ToString(), Is.EqualTo("synthetic"));
            foreach (var turn in scenario.GetProperty("turns").EnumerateArray())
            {
                AssertJson(await Receive(socket), turn.GetProperty("request"));
                foreach (var frame in turn.GetProperty("frames").EnumerateArray())
                    await Send(socket, frame.ValueKind == JsonValueKind.String ? frame.GetString() : frame.GetRawText());
            }
            if (scenario.TryGetProperty("close_code", out var closeCode))
                await socket.CloseOutputAsync((WebSocketCloseStatus)closeCode.GetInt32(), "synthetic", timeout.Token);
            else
                await AwaitClose(socket); // Extra commands, including automatic replay, fail this assertion.
        });
        var client = new ResponsesClient(new ApiKeyCredential("fake-contract-key"),
            new ResponsesClientOptions { Endpoint = new Uri(server.Endpoint, "v1?contract=1") });
        var options = new ResponseWebSocketOptions();
        options.Headers["X-Contract-Test"] = "synthetic";
        await using (var connection = await client.ConnectWebSocketAsync(options, timeout.Token))
        {
            foreach (var turn in scenario.GetProperty("turns").EnumerateArray())
            {
                var request = turn.GetProperty("request");
                var command = new ResponseWebSocketCreateCommand
                {
                    Model = request.GetProperty("model").GetString(),
                    StreamId = request.GetProperty("stream_id").GetString(),
                };
                if (request.TryGetProperty("previous_response_id", out var previous))
                    command.PreviousResponseId = previous.GetString();
                // .NET exposes typed item arrays; Patch preserves the protocol's string input and explicit null.
                command.Patch.Set("$.input"u8, Encoding.UTF8.GetBytes(request.GetProperty("input").GetRawText()));
                if (request.TryGetProperty("instructions", out var instructions))
                    command.Patch.Set("$.instructions"u8, Encoding.UTF8.GetBytes(instructions.GetRawText()));
                await connection.SendAsync(command, timeout.Token);
                foreach (var frame in turn.GetProperty("frames").EnumerateArray())
                {
                    if (frame.ValueKind == JsonValueKind.String)
                    {
                        Assert.CatchAsync<JsonException>(async () => await connection.ReceiveAsync(timeout.Token));
                        continue;
                    }
                    var actual = await connection.ReceiveAsync(timeout.Token);
                    AssertJson(actual.RawData.ToString(), frame);
                    Assert.That(actual.StreamId, Is.EqualTo(frame.GetProperty("stream_id").GetString()));
                    string eventType = frame.GetProperty("type").GetString();
                    if (eventType == "error")
                    {
                        Assert.That(actual, Is.TypeOf<ResponseWebSocketErrorEvent>());
                        var error = (ResponseWebSocketErrorEvent)actual;
                        var expected = frame.GetProperty("error");
                        Assert.That(error.Status, Is.EqualTo(frame.GetProperty("status").GetInt32()));
                        Assert.That(error.Error.Code, Is.EqualTo(expected.GetProperty("code").GetString()));
                        Assert.That(error.Error.Param, Is.EqualTo(expected.GetProperty("param").GetString()));
                        Assert.That(error.Error.Message, Is.EqualTo(expected.GetProperty("message").GetString()));
                        continue;
                    }
                    Assert.That(actual.Update, Is.Not.Null);
                    Assert.That(actual.Update.SequenceNumber, Is.EqualTo(frame.GetProperty("sequence_number").GetInt32()));
                    Type expectedType = eventType switch
                    {
                        "response.created" => typeof(StreamingResponseCreatedUpdate),
                        "response.completed" => typeof(StreamingResponseCompletedUpdate),
                        "response.failed" => typeof(StreamingResponseFailedUpdate),
                        "response.incomplete" => typeof(StreamingResponseIncompleteUpdate),
                        "response.output_text.delta" => typeof(StreamingResponseOutputTextDeltaUpdate),
                        _ => null,
                    };
                    if (expectedType != null) Assert.That(actual.Update.GetType(), Is.EqualTo(expectedType));
                    if (actual.Update is StreamingResponseOutputTextDeltaUpdate delta)
                        Assert.That(delta.Delta, Is.EqualTo(frame.GetProperty("delta").GetString()));
                    ResponseResult response = actual.Update switch
                    {
                        StreamingResponseCreatedUpdate created => created.Response,
                        StreamingResponseCompletedUpdate completed => completed.Response,
                        StreamingResponseFailedUpdate failed => failed.Response,
                        StreamingResponseIncompleteUpdate incomplete => incomplete.Response,
                        _ => null,
                    };
                    if (frame.TryGetProperty("response", out var expectedResponse))
                    {
                        Assert.That(response, Is.Not.Null);
                        Assert.That(response.Id, Is.EqualTo(expectedResponse.GetProperty("id").GetString()));
                        Assert.That(response.Status, Is.EqualTo(Enum.Parse<ResponseStatus>(expectedResponse.GetProperty("status").GetString().Replace("_", ""), true)));
                        if (expectedResponse.TryGetProperty("error", out var error))
                            Assert.That(response.Error.Code.ToString(), Is.EqualTo(error.GetProperty("code").GetString()));
                        if (expectedResponse.TryGetProperty("incomplete_details", out var incomplete))
                            Assert.That(response.IncompleteStatusDetails.Reason.ToString(), Is.EqualTo(incomplete.GetProperty("reason").GetString()));
                        if (expectedResponse.GetProperty("output").GetArrayLength() > 0)
                            Assert.That(response.GetOutputText(), Is.EqualTo("Synthetic text"));
                    }
                }
            }
            // Both clean and error EOF are distinct from response completion in the .NET profile.
            if (scenario.TryGetProperty("close_code", out _))
                Assert.ThrowsAsync<EndOfStreamException>(async () => await connection.ReceiveAsync(timeout.Token));
            await connection.CloseAsync(timeout.Token);
            await connection.CloseAsync(timeout.Token);
        }
        await server.Completed;
        Assert.That(server.ConnectionCount, Is.EqualTo(1));
    }


    [Test]
    public async Task SteeringOutcomesPreserveTypedSubmissionsAndRequiredInputs()
    {
        const string requiredInput = """
            [{"type":"function_call_output","call_id":"f","name":"lookup"},
             {"type":"custom_tool_call_output","call_id":"c"},{"type":"computer_call_output","call_id":"pc"},
             {"type":"shell_call_output","call_id":"s"},{"type":"apply_patch_call_output","call_id":"p"},
             {"type":"tool_search_output","call_id":"t","execution":"client"},
             {"type":"mcp_approval_response","approval_request_id":"approval"},{"type":"future_result","extra":true}]
            """;
        const string returnedInput = """[{"role":"user","content":"keep this input"}]""";
        var submission = new { id = "steer_test", previous_response_id = "resp_active" };
        await using var server = await LocalServer.Start(async (_, socket) =>
        {
            await Receive(socket);
            await Send(socket, JsonSerializer.Serialize(new { type = "response.steer.accepted", sequence_number = 1, stream_id = "answer", steer = submission }));
            using var required = JsonDocument.Parse(requiredInput);
            await Send(socket, JsonSerializer.Serialize(new { type = "response.steer.pending", sequence_number = 2, stream_id = "answer", steer = submission, reason = "waiting_for_required_input", required_input = required.RootElement }));
            using var input = JsonDocument.Parse(returnedInput);
            await Send(socket, JsonSerializer.Serialize(new { type = "response.steer.failed", sequence_number = 3, stream_id = "answer",
                steer = new { previous_response_id = "resp_active", input = input.RootElement },
                error = new { type = "invalid_request_error", code = "future_error", message = "not committed" } }));
            await Send(socket, JsonSerializer.Serialize(new { type = "response.steer.pending", sequence_number = 4, stream_id = "answer", steer = submission, reason = "future_reason", required_input = required.RootElement }));
            await Send(socket, Terminal("completed", "resp_next", "answer"));
            await AwaitClose(socket);
        });
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await using (var connection = await Client(server).ConnectWebSocketAsync(cancellationToken: timeout.Token))
        {
            using var lane = connection.OpenLane("answer");
            await lane.SendAsync(new ResponseWebSocketSteerCommand("resp_active", "keep this input"), timeout.Token);
            var accepted = (StreamingResponseSteerAcceptedUpdate)(await lane.ReceiveAsync(timeout.Token)).Update;
            Assert.That(accepted.Steer.Id, Is.EqualTo("steer_test"));
            Assert.That(accepted.Steer.PreviousResponseId, Is.EqualTo("resp_active"));
            Assert.That(accepted.StreamId, Is.EqualTo("answer"));
            var pending = (StreamingResponseSteerPendingUpdate)(await lane.ReceiveAsync(timeout.Token)).Update;
            Assert.That(pending.Reason, Is.EqualTo(ResponseSteerPendingReason.WaitingForRequiredInput));
            Assert.That(pending.RequiredInput.Take(7).Select(item => item.GetType()), Is.EqualTo(new[] {
                typeof(ResponseSteerFunctionCallOutput), typeof(ResponseSteerCustomToolCallOutput), typeof(ResponseSteerComputerCallOutput),
                typeof(ResponseSteerShellCallOutput), typeof(ResponseSteerApplyPatchCallOutput), typeof(ResponseSteerToolSearchOutput),
                typeof(ResponseSteerMcpApprovalResponse) }));
            Assert.That(((ResponseSteerFunctionCallOutput)pending.RequiredInput[0]).Name, Is.EqualTo("lookup"));
            Assert.That(((ResponseSteerMcpApprovalResponse)pending.RequiredInput[6]).ApprovalRequestId, Is.EqualTo("approval"));
            Assert.That(pending.RequiredInput[7].Kind.ToString(), Is.EqualTo("future_result"));
            var failed = (StreamingResponseSteerFailedUpdate)(await lane.ReceiveAsync(timeout.Token)).Update;
            Assert.That(failed.Steer.Id, Is.Null);
            Assert.That(failed.Steer.Input.ToString(), Is.EqualTo(returnedInput));
            Assert.That(failed.Error.Code.ToString(), Is.EqualTo("future_error"));
            var future = (StreamingResponseSteerPendingUpdate)(await lane.ReceiveAsync(timeout.Token)).Update;
            Assert.That(future.Reason.ToString(), Is.EqualTo("future_reason"));
            Assert.That((await lane.ReceiveResponseAsync(timeout.Token)).Id, Is.EqualTo("resp_next"));
        }
        await server.Completed;
    }

    private sealed class PausedSendSocket : WebSocket
    {
        private readonly WebSocket _socket;
        private int _sendCount;
        internal TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal Exception DisposeFailure { get; set; }
        internal PausedSendSocket(WebSocket socket) => _socket = socket;
        public override WebSocketCloseStatus? CloseStatus => _socket.CloseStatus;
        public override string CloseStatusDescription => _socket.CloseStatusDescription;
        public override WebSocketState State => _socket.State;
        public override string SubProtocol => _socket.SubProtocol;
        public override void Abort() => _socket.Abort();
        public override void Dispose() { _socket.Dispose(); if (DisposeFailure != null) throw DisposeFailure; }
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
                // Propagate every handler failure, including NUnit assertions, to the awaiting test.
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
