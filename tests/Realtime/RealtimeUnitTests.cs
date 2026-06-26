using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI001
#pragma warning disable OPENAI002

[Category("Smoke")]
public class RealtimeUnitTests
{
    [Test]
    public void DefaultOptionsHaveNullProperties()
    {
        RealtimeClientOptions options = new();

        Assert.That(options.Endpoint, Is.Null);
        Assert.That(options.OrganizationId, Is.Null);
        Assert.That(options.ProjectId, Is.Null);
        Assert.That(options.UserAgentApplicationId, Is.Null);
    }

    [Test]
    public void OptionsPropertiesCanBeSet()
    {
        Uri endpoint = new("https://custom.endpoint.com/v1");
        RealtimeClientOptions options = new()
        {
            Endpoint = endpoint,
            OrganizationId = "org-test123",
            ProjectId = "proj-test456",
            UserAgentApplicationId = "my-app/1.0",
        };

        Assert.That(options.Endpoint, Is.EqualTo(endpoint));
        Assert.That(options.OrganizationId, Is.EqualTo("org-test123"));
        Assert.That(options.ProjectId, Is.EqualTo("proj-test456"));
        Assert.That(options.UserAgentApplicationId, Is.EqualTo("my-app/1.0"));
    }

    [Test]
    public void OptionsInheritFromClientPipelineOptions()
    {
        RealtimeClientOptions options = new();

        Assert.That(options, Is.InstanceOf<ClientPipelineOptions>());
        Assert.That(options, Is.Not.InstanceOf<OpenAIClientOptions>());
    }

    [Test]
    public void FrozenOptionsCannotBeModified()
    {
        RealtimeClientOptions options = new()
        {
            Endpoint = new Uri("https://example.com"),
            OrganizationId = "org-1",
            ProjectId = "proj-1",
            UserAgentApplicationId = "app-1",
        };

        options.Freeze();

        Assert.Throws<InvalidOperationException>(() => options.Endpoint = new Uri("https://other.com"));
        Assert.Throws<InvalidOperationException>(() => options.OrganizationId = "org-2");
        Assert.Throws<InvalidOperationException>(() => options.ProjectId = "proj-2");
        Assert.Throws<InvalidOperationException>(() => options.UserAgentApplicationId = "app-2");
    }

    [Test]
    public void ClientAcceptsOptions()
    {
        RealtimeClientOptions options = new()
        {
            Endpoint = new Uri("https://custom.openai.com/v1"),
        };

        RealtimeClient client = new(new ApiKeyCredential("test-key"), options);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://custom.openai.com/v1")));
    }

    [Test]
    public void ClientUsesDefaultEndpointWhenOptionsOmitEndpoint()
    {
        RealtimeClient client = new(new ApiKeyCredential("test-key"), new RealtimeClientOptions());

        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://api.openai.com/v1")));
    }

    [Test]
    public void ClientAcceptsNullOptions()
    {
        RealtimeClient client = new(new ApiKeyCredential("test-key"), (RealtimeClientOptions)null);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://api.openai.com/v1")));
    }

    [Test]
    public void ClientWithAuthenticationPolicyAcceptsOptions()
    {
        RealtimeClientOptions options = new()
        {
            Endpoint = new Uri("https://custom.openai.com/v1"),
        };

        AuthenticationPolicy policy = ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(
            new ApiKeyCredential("test-key"), "Authorization", "Bearer");

        RealtimeClient client = new(policy, options);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://custom.openai.com/v1")));
    }

    [Test]
    public void AudioEndMsSerializesAsInteger()
    {
        // A TimeSpan with sub-millisecond precision that would produce a fractional double
        var truncate = new RealtimeClientCommandConversationItemTruncate(
            itemId: "item_abc",
            contentIndex: 0,
            audioEndTime: TimeSpan.FromTicks(12345678)); // 1234.5678 ms

        BinaryData json = ModelReaderWriter.Write(truncate);
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        // Should be 1235 (integer, rounded), not 1234.5678
        var audioEndMs = root.GetProperty("audio_end_ms");
        Assert.That(audioEndMs.TryGetInt64(out long value), Is.True);
        Assert.That(value, Is.EqualTo(1235));
    }

    [Test]
    public void ProtocolReceiveUpdatesCanBeEnumeratedSynchronously()
    {
        const string payload = """{"type":"response.created","event_id":"event_123"}""";
        TestRealtimeSessionClient client = CreateRealtimeSessionClient(payload);

        ClientResult update = client.ReceiveUpdates(new RequestOptions()).Single();

        Assert.That(update, Is.Not.Null);
        Assert.That(update.GetRawResponse().Content.ToString(), Is.EqualTo(payload));
    }

    [Test]
    public void TypedReceiveUpdatesCanBeEnumeratedSynchronously()
    {
        TestRealtimeSessionClient client = CreateRealtimeSessionClient("""{"type":"response.created","event_id":"event_123"}""");

        RealtimeServerUpdate update = client.ReceiveUpdates().Single();

        Assert.That(update, Is.InstanceOf<RealtimeServerUpdateResponseCreated>());
        Assert.That(((RealtimeServerUpdateResponseCreated)update).EventId, Is.EqualTo("event_123"));
    }

    private static TestRealtimeSessionClient CreateRealtimeSessionClient(string payload)
        => new(new FakeWebSocket([Encoding.UTF8.GetBytes(payload)]));

    private sealed class TestRealtimeSessionClient : RealtimeSessionClient
    {
        public TestRealtimeSessionClient(WebSocket webSocket)
            : base(new ApiKeyCredential("test-key"), new Uri("wss://example.com/v1/realtime"), "gpt-realtime", null, null)
        {
            WebSocket = webSocket;
        }
    }

    private sealed class FakeWebSocket : WebSocket
    {
        private readonly Queue<byte[]> _messages;
        private WebSocketCloseStatus? _closeStatus;
        private string _closeStatusDescription;
        private WebSocketState _state = WebSocketState.Open;

        public FakeWebSocket(IEnumerable<byte[]> messages)
        {
            _messages = new Queue<byte[]>(messages);
        }

        public override WebSocketCloseStatus? CloseStatus => _closeStatus;

        public override string CloseStatusDescription => _closeStatusDescription;

        public override WebSocketState State => _state;

        public override string SubProtocol => "realtime";

        public override void Abort()
        {
            _state = WebSocketState.Aborted;
        }

        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            _closeStatus = closeStatus;
            _closeStatusDescription = statusDescription;
            _state = WebSocketState.Closed;
            return Task.CompletedTask;
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            _closeStatus = closeStatus;
            _closeStatusDescription = statusDescription;
            _state = WebSocketState.CloseSent;
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _state = WebSocketState.Closed;
            base.Dispose();
        }

        public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_messages.Count == 0)
            {
                _closeStatus = WebSocketCloseStatus.NormalClosure;
                _closeStatusDescription = "done";
                _state = WebSocketState.CloseReceived;
                return Task.FromResult(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true, _closeStatus, _closeStatusDescription));
            }

            byte[] message = _messages.Dequeue();
            Array.Copy(message, 0, buffer.Array, buffer.Offset, message.Length);
            return Task.FromResult(new WebSocketReceiveResult(message.Length, WebSocketMessageType.Text, endOfMessage: true));
        }

        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
