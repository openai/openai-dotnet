using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Reflection;
using System.Text.Json;

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
    public void ClientDoesNotDuplicateRealtimePathWithTrailingSlash()
    {
        RealtimeClientOptions options = new()
        {
            Endpoint = new Uri("https://custom.openai.com/v1/realtime/"),
        };

        RealtimeClient client = new(new ApiKeyCredential("test-key"), options);

        Assert.That(GetWebSocketEndpoint(client), Is.EqualTo(new Uri("wss://custom.openai.com/v1/realtime")));
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
    public void NonDiscriminatedUnionComponentsSerializeAndDeserialize()
    {
        RealtimeMaxOutputTokenCount maxOutputTokenCount =
            ModelReaderWriter.Read<RealtimeMaxOutputTokenCount>(BinaryData.FromString("42"));
        RealtimeToolChoice toolChoice =
            ModelReaderWriter.Read<RealtimeToolChoice>(BinaryData.FromString("""{"type":"function","name":"search"}"""));
        RealtimeTracing tracing =
            ModelReaderWriter.Read<RealtimeTracing>(BinaryData.FromString("""{"workflow_name":"test"}"""));
        RealtimeTruncation truncation =
            ModelReaderWriter.Read<RealtimeTruncation>(BinaryData.FromString("""{"type":"retention_ratio","retention_ratio":0.8}"""));
        RealtimeMcpToolCallApprovalPolicy approvalPolicy =
            ModelReaderWriter.Read<RealtimeMcpToolCallApprovalPolicy>(BinaryData.FromString("""{"always":{}}"""));

        Assert.Multiple(() =>
        {
            Assert.That(maxOutputTokenCount.CustomMaxOutputTokenCount, Is.EqualTo(42));
            Assert.That(maxOutputTokenCount.DefaultMaxOutputTokenCount, Is.Null);
            Assert.That(toolChoice.CustomToolChoice, Is.TypeOf<RealtimeCustomFunctionToolChoice>());
            Assert.That(toolChoice.DefaultToolChoice, Is.Null);
            Assert.That(tracing.CustomTracing.WorkflowName, Is.EqualTo("test"));
            Assert.That(tracing.DefaultTracing, Is.Null);
            Assert.That(truncation.CustomTruncation, Is.TypeOf<RealtimeCustomRetentionRatioTruncation>());
            Assert.That(truncation.DefaultTruncation, Is.Null);
            Assert.That(approvalPolicy.CustomPolicy, Is.Not.Null);
            Assert.That(approvalPolicy.DefaultPolicy, Is.Null);
            Assert.That(ModelReaderWriter.Write(maxOutputTokenCount).ToString(), Is.EqualTo("42"));
            Assert.That(ModelReaderWriter.Write((RealtimeMaxOutputTokenCount)RealtimeDefaultMaxOutputTokenCount.Infinity).ToString(), Is.EqualTo("\"inf\""));
            Assert.That(ModelReaderWriter.Write((RealtimeToolChoice)RealtimeDefaultToolChoice.Required).ToString(), Is.EqualTo("\"required\""));
            Assert.That(ModelReaderWriter.Write((RealtimeTracing)RealtimeDefaultTracing.Auto).ToString(), Is.EqualTo("\"auto\""));
            Assert.That(ModelReaderWriter.Write((RealtimeTruncation)RealtimeDefaultTruncation.Disabled).ToString(), Is.EqualTo("\"disabled\""));
            Assert.That(ModelReaderWriter.Write((RealtimeMcpToolCallApprovalPolicy)RealtimeDefaultMcpToolCallApprovalPolicy.NeverRequireApproval).ToString(), Is.EqualTo("\"never\""));
        });
    }

    [Test]
    public void NonDiscriminatedUnionsRejectUnsupportedJsonShapes()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<JsonException>(() => ModelReaderWriter.Read<RealtimeMaxOutputTokenCount>(BinaryData.FromString("{}")));
            Assert.Throws<JsonException>(() => ModelReaderWriter.Read<RealtimeToolChoice>(BinaryData.FromString("42")));
            Assert.Throws<JsonException>(() => ModelReaderWriter.Read<RealtimeTracing>(BinaryData.FromString("[]")));
            Assert.Throws<JsonException>(() => ModelReaderWriter.Read<RealtimeTruncation>(BinaryData.FromString("true")));
            Assert.Throws<JsonException>(() => ModelReaderWriter.Read<RealtimeMcpToolCallApprovalPolicy>(BinaryData.FromString("42")));
        });
    }

    [Test]
    public void NonDiscriminatedUnionsPropagateJsonPatch()
    {
        RealtimeConversationSessionOptions options = new()
        {
            MaxOutputTokenCount = 42,
            ToolChoice = new RealtimeCustomFunctionToolChoice("search"),
            Tracing = new RealtimeCustomTracing(),
            Truncation = new RealtimeCustomRetentionRatioTruncation(0.8f),
        };

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        options.Patch.Set("$.max_output_tokens"u8, "\"inf\""u8);
        options.Patch.Set("$.tool_choice.additional_property"u8, "tool choice");
        options.Patch.Set("$.tracing.additional_property"u8, "tracing");
        options.Patch.Set("$.truncation.additional_property"u8, "truncation");
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.Multiple(() =>
        {
            Assert.That(json.RootElement.GetProperty("max_output_tokens").GetString(), Is.EqualTo("inf"));
            Assert.That(json.RootElement.GetProperty("tool_choice").GetProperty("additional_property").GetString(), Is.EqualTo("tool choice"));
            Assert.That(json.RootElement.GetProperty("tracing").GetProperty("additional_property").GetString(), Is.EqualTo("tracing"));
            Assert.That(json.RootElement.GetProperty("truncation").GetProperty("additional_property").GetString(), Is.EqualTo("truncation"));
        });
    }

    [Test]
    public void McpToolCallApprovalPolicyPropagatesJsonPatch()
    {
        RealtimeMcpTool tool = new("test", new Uri("https://example.com"))
        {
            ToolCallApprovalPolicy = new RealtimeCustomMcpToolCallApprovalPolicy
            {
                ToolsAlwaysRequiringApproval = new RealtimeMcpToolFilter()
            }
        };

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        tool.Patch.Set("$.require_approval.always.additional_property"u8, "patched");
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(tool));
        Assert.That(
            json.RootElement.GetProperty("require_approval").GetProperty("always").GetProperty("additional_property").GetString(),
            Is.EqualTo("patched"));
    }

    [Test]
    public void McpToolCallApprovalPolicySupportsRootJsonPatch()
    {
        RealtimeMcpTool tool = new("test", new Uri("https://example.com"))
        {
            ToolCallApprovalPolicy = new RealtimeCustomMcpToolCallApprovalPolicy()
        };

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        tool.Patch.Set("$.require_approval"u8, "\"never\""u8);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(tool));
        Assert.That(json.RootElement.GetProperty("require_approval").GetString(), Is.EqualTo("never"));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void DeserializeMcpAllowedTools(bool useShorthandInput)
    {
        BinaryData data = BinaryData.FromString(useShorthandInput
            ? """{"type":"mcp","server_label":"test","allowed_tools":["search"]}"""
            : """{"type":"mcp","server_label":"test","allowed_tools":{"tool_names":["search"],"read_only":true}}""");

        RealtimeMcpTool tool = ModelReaderWriter.Read<RealtimeMcpTool>(data);

        Assert.That(tool.AllowedTools.ToolNames, Is.EqualTo(new[] { "search" }));
        Assert.That(tool.AllowedTools.IsReadOnly, Is.EqualTo(useShorthandInput ? null : true));
        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(tool));
        Assert.That(json.RootElement.GetProperty("allowed_tools").ValueKind, Is.EqualTo(JsonValueKind.Object));
    }

    private static Uri GetWebSocketEndpoint(RealtimeClient client)
    {
        FieldInfo field = typeof(RealtimeClient).GetField(
            "_webSocketEndpoint",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, "RealtimeClient should expose its WebSocket endpoint field");
        return (Uri)field.GetValue(client);
    }
}
