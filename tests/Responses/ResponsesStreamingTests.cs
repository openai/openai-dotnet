using NUnit.Framework;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses;

[Parallelizable(ParallelScope.All)]
[Category("Responses")]
[Category("Smoke")]
public class ResponsesStreamingTests
{
    // The event after the terminal [DONE] marker must never be surfaced.
    private const string Events = """
        event: response.created
        data: {"type":"response.created","sequence_number":0,"response":{"id":"resp_1","object":"response","created_at":0,"status":"in_progress","model":"mock","output":[],"tools":[]}}

        event: response.output_item.added
        data: {"type":"response.output_item.added","sequence_number":1,"output_index":0,"item":{"type":"message","id":"msg_1","role":"assistant","status":"in_progress","content":[]}}

        event: response.output_text.delta
        data: {"type":"response.output_text.delta","sequence_number":2,"item_id":"msg_1","output_index":0,"content_index":0,"delta":"Hello","logprobs":[]}

        event: response.completed
        data: {"type":"response.completed","sequence_number":3,"response":{"id":"resp_1","object":"response","created_at":0,"status":"completed","model":"mock","output":[],"tools":[]}}

        data: [DONE]

        event: response.output_text.delta
        data: {"type":"response.output_text.delta","sequence_number":4,"item_id":"msg_1","output_index":0,"content_index":0,"delta":"after terminal","logprobs":[]}


        """;

    private static readonly Type[] s_expectedUpdateTypes =
    [
        typeof(StreamingResponseCreatedUpdate),
        typeof(StreamingResponseOutputItemAddedUpdate),
        typeof(StreamingResponseOutputTextDeltaUpdate),
        typeof(StreamingResponseCompletedUpdate),
    ];

    public enum CreateOverload
    {
        Options,
        InputItems,
        UserInputText,
    }

    [Test]
    public async Task CreateResponseStreamingAsyncReturnsAnEstablishedResult([Values] CreateOverload overload)
    {
        StreamingTestServer server = new(Events);
        ResponsesClient client = CreateClient(server);

        AsyncStreamingResult<StreamingResponseUpdate> result = overload switch
        {
            CreateOverload.Options => await client.CreateResponseStreamingAsync(
                new CreateResponseOptions("mock", [ResponseItem.CreateUserMessageItem("hello")]) { StreamingEnabled = true }),
            CreateOverload.InputItems => await client.CreateResponseStreamingAsync("mock", [ResponseItem.CreateUserMessageItem("hello")]),
            _ => await client.CreateResponseStreamingAsync("mock", "hello"),
        };

        await using (result)
        {
            Assert.That(server.RequestCount, Is.EqualTo(1));
            Assert.That(server.LastBufferResponse, Is.False);
            Assert.That(server.LastRequestJson.GetProperty("stream").GetBoolean(), Is.True);
            AssertEstablished(result, server);

            List<StreamingResponseUpdate> updates = await ReadAllAsync(result);

            Assert.That(updates.Select(update => update.GetType()), Is.EqualTo(s_expectedUpdateTypes));
            Assert.That(server.RequestCount, Is.EqualTo(1));
            Assert.That(server.LastResponseStream.IsDisposed, Is.True);
            Assert.That(() => result.Status, Throws.TypeOf<ObjectDisposedException>());
            Assert.That(() => ((IAsyncEnumerable<StreamingResponseUpdate>)result).GetAsyncEnumerator(), Throws.InvalidOperationException);
        }
    }

    [Test]
    public async Task GetResponseStreamingAsyncReturnsAnEstablishedResult([Values] bool useOptions)
    {
        StreamingTestServer server = new(Events);
        ResponsesClient client = CreateClient(server);

        await using AsyncStreamingResult<StreamingResponseUpdate> result = useOptions
            ? await client.GetResponseStreamingAsync(new GetResponseOptions("resp_1") { StreamingEnabled = true, StartingAfter = 2 })
            : await client.GetResponseStreamingAsync("resp_1");

        Assert.That(server.RequestCount, Is.EqualTo(1));
        Assert.That(server.LastBufferResponse, Is.False);
        Assert.That(server.LastRequestUri.AbsolutePath, Does.EndWith("/responses/resp_1"));
        Assert.That(server.LastRequestUri.Query, Does.Contain("stream=true"));
        Assert.That(server.LastRequestUri.Query, useOptions ? Does.Contain("starting_after=2") : Does.Not.Contain("starting_after"));
        AssertEstablished(result, server);

        List<StreamingResponseUpdate> updates = await ReadAllAsync(result);

        Assert.That(updates.Select(update => update.GetType()), Is.EqualTo(s_expectedUpdateTypes));
        Assert.That(server.RequestCount, Is.EqualTo(1));
        Assert.That(server.LastResponseStream.IsDisposed, Is.True);
    }

    [Test]
    public void CanceledOperationTokenPreventsTheRequest()
    {
        StreamingTestServer server = new(Events);
        ResponsesClient client = CreateClient(server);
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        Assert.That(
            async () => await client.CreateResponseStreamingAsync("mock", "hello", cancellationToken: cancellation.Token),
            Throws.InstanceOf<OperationCanceledException>());
        Assert.That(server.RequestCount, Is.Zero);
    }

    [Test]
    public async Task OperationCancellationStopsAnEstablishedStream()
    {
        StreamingTestServer server = new(Events);
        ResponsesClient client = CreateClient(server);
        using CancellationTokenSource cancellation = new();
        AsyncStreamingResult<StreamingResponseUpdate> result = await client.CreateResponseStreamingAsync("mock", "hello", cancellationToken: cancellation.Token);
        IAsyncEnumerator<StreamingResponseUpdate> enumerator = ((IAsyncEnumerable<StreamingResponseUpdate>)result).GetAsyncEnumerator();

        Assert.That(await enumerator.MoveNextAsync(), Is.True);
        cancellation.Cancel();

        Assert.That(async () => await enumerator.MoveNextAsync(), Throws.InstanceOf<OperationCanceledException>());
        Assert.That(server.LastResponseStream.IsDisposed, Is.True);
        Assert.That(server.RequestCount, Is.EqualTo(1));
        await enumerator.DisposeAsync();
    }

    [Test]
    public async Task EnumerationCancellationStopsTheStream()
    {
        StreamingTestServer server = new(Events);
        ResponsesClient client = CreateClient(server);
        using CancellationTokenSource cancellation = new();
        AsyncStreamingResult<StreamingResponseUpdate> result = await client.CreateResponseStreamingAsync("mock", "hello");
        int received = 0;

        Assert.That(
            async () =>
            {
                await foreach (StreamingResponseUpdate update in result.WithCancellation(cancellation.Token))
                {
                    received++;
                    cancellation.Cancel();
                }
            },
            Throws.InstanceOf<OperationCanceledException>());
        Assert.That(received, Is.EqualTo(1));
        Assert.That(server.LastResponseStream.IsDisposed, Is.True);
    }

    [Test]
    public async Task BreakingOutOfEnumerationReleasesTheResponse()
    {
        StreamingTestServer server = new(Events);
        ResponsesClient client = CreateClient(server);
        AsyncStreamingResult<StreamingResponseUpdate> result = await client.CreateResponseStreamingAsync("mock", "hello");

        await foreach (StreamingResponseUpdate update in result)
        {
            Assert.That(update, Is.InstanceOf<StreamingResponseCreatedUpdate>());
            break;
        }

        Assert.That(server.LastResponseStream.IsDisposed, Is.True);
        Assert.That(() => result.Status, Throws.TypeOf<ObjectDisposedException>());
        Assert.That(server.RequestCount, Is.EqualTo(1));
    }

    [Test]
    public async Task DisposingWithoutEnumeratingReleasesTheResponse()
    {
        StreamingTestServer server = new(Events);
        ResponsesClient client = CreateClient(server);
        AsyncStreamingResult<StreamingResponseUpdate> result = await client.CreateResponseStreamingAsync("mock", "hello");

        await result.DisposeAsync();

        Assert.That(server.LastResponseStream.IsDisposed, Is.True);
        Assert.That(server.LastResponseStream.BytesRead, Is.Zero);
        Assert.That(server.RequestCount, Is.EqualTo(1));
    }

    [Test]
    public async Task ServiceErrorIsThrownBeforeAResultIsReturned()
    {
        StreamingTestServer server = new("""{"error":{"message":"bad request","type":"invalid_request_error"}}""", status: 400);
        ResponsesClient client = CreateClient(server);

        ClientResultException exception = await Assert.ThrowsAsync<ClientResultException>(
            async () => await client.CreateResponseStreamingAsync("mock", "hello"));

        Assert.That(exception.Status, Is.EqualTo(400));
        Assert.That(server.RequestCount, Is.EqualTo(1));
    }

    [Test]
    public async Task MalformedEventFailsEnumerationAndReleasesTheResponse()
    {
        StreamingTestServer server = new("data: {not json\n\n");
        ResponsesClient client = CreateClient(server);
        AsyncStreamingResult<StreamingResponseUpdate> result = await client.CreateResponseStreamingAsync("mock", "hello");

        Assert.That(async () => await ReadAllAsync(result), Throws.InstanceOf<JsonException>());
        Assert.That(server.LastResponseStream.IsDisposed, Is.True);
    }

    [Test]
    public async Task DerivedClientTransformsUpdatesOfTheSameResponse([Values] bool getResponse)
    {
        StreamingTestServer server = new(Events);
        TransformingResponsesClient derived = new(CreateOptions(server));
        ResponsesClient client = derived;

        await using AsyncStreamingResult<StreamingResponseUpdate> result = getResponse
            ? await client.GetResponseStreamingAsync("resp_1")
            : await client.CreateResponseStreamingAsync("mock", "hello");

        Assert.That(derived.ResultsCreated, Is.EqualTo(1));
        Assert.That(derived.ObservedStatus, Is.EqualTo(200));
        Assert.That(derived.TransformedCount, Is.Zero);
        AssertEstablished(result, server);

        List<StreamingResponseUpdate> updates = await ReadAllAsync(result);

        Assert.That(updates.Select(update => update.GetType()), Is.EqualTo(s_expectedUpdateTypes));
        Assert.That(updates.OfType<StreamingResponseOutputItemAddedUpdate>().Single().Item, Is.SameAs(derived.ReplacementItem));
        Assert.That(derived.TransformedCount, Is.EqualTo(updates.Count));
        Assert.That(server.RequestCount, Is.EqualTo(1));
        Assert.That(server.LastResponseStream.IsDisposed, Is.True);
    }

    private static void AssertEstablished(AsyncStreamingResult<StreamingResponseUpdate> result, StreamingTestServer server)
    {
        Assert.That(result.Status, Is.EqualTo(200));
        Assert.That(result.ReasonPhrase, Is.EqualTo("OK"));
        Assert.That(result.Headers.TryGetValue("x-request-id", out string requestId), Is.True);
        Assert.That(requestId, Is.EqualTo(StreamingTestServer.RequestId));
        Assert.That(server.LastResponseStream.BytesRead, Is.Zero);
        Assert.That(server.LastResponseStream.IsDisposed, Is.False);
    }

    private static async Task<List<StreamingResponseUpdate>> ReadAllAsync(AsyncStreamingResult<StreamingResponseUpdate> result)
    {
        List<StreamingResponseUpdate> updates = [];

        await foreach (StreamingResponseUpdate update in result)
        {
            updates.Add(update);
        }

        return updates;
    }

    private static ResponsesClientOptions CreateOptions(StreamingTestServer server)
        => new() { Transport = server.Transport };

    private static ResponsesClient CreateClient(StreamingTestServer server)
        => new(new ApiKeyCredential("key"), CreateOptions(server));

    private sealed class TransformingResponsesClient : ResponsesClient
    {
        public TransformingResponsesClient(ResponsesClientOptions options)
            : base(new ApiKeyCredential("key"), options)
        {
        }

        public int ResultsCreated { get; private set; }

        public int? ObservedStatus { get; private set; }

        public int TransformedCount { get; private set; }

        public ResponseItem ReplacementItem { get; } = ResponseItem.CreateAssistantMessageItem("replaced");

        protected override AsyncStreamingResult<StreamingResponseUpdate> CreateStreamingResult(
            PipelineResponse response,
            Func<Stream, CancellationToken, IAsyncEnumerable<StreamingResponseUpdate>> updateProducer,
            CancellationToken cancellationToken)
        {
            ResultsCreated++;
            ObservedStatus = response.Status;

            return base.CreateStreamingResult(
                response,
                (stream, token) => TransformAsync(updateProducer(stream, token), token),
                cancellationToken);
        }

        private async IAsyncEnumerable<StreamingResponseUpdate> TransformAsync(
            IAsyncEnumerable<StreamingResponseUpdate> updates,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (StreamingResponseUpdate update in updates.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                TransformedCount++;

                if (update is StreamingResponseOutputItemAddedUpdate added)
                {
                    added.Item = ReplacementItem;
                }

                yield return update;
            }
        }
    }
}
