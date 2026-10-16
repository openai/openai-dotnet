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
    public void FactoryPreservesCredentialAndPipeline()
    {
        ApiKeyCredential credential = new("test-key");
        OpenAIClient parent = new(credential, new OpenAIClientOptions
        {
            Endpoint = new Uri("https://custom.openai.com/v1"),
        });

        RealtimeClient client = parent.GetRealtimeClient();

        // The WebSocket path needs the original credential, including later key updates.
        Assert.That(GetKeyCredential(client), Is.SameAs(credential));
        Assert.That(client.Pipeline, Is.SameAs(parent.Pipeline));
        Assert.That(GetWebSocketEndpoint(client), Is.EqualTo(new Uri("wss://custom.openai.com/v1/realtime")));
    }

    [Test]
    public void FactoryWithAuthenticationPolicyPreservesPipelineWithoutKeyCredential()
    {
        AuthenticationPolicy policy = ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(
            new ApiKeyCredential("test-key"), "Authorization", "Bearer");
        OpenAIClient parent = new(policy);

        RealtimeClient client = parent.GetRealtimeClient();

        Assert.That(client.Pipeline, Is.SameAs(parent.Pipeline));
        Assert.That(GetKeyCredential(client), Is.Null);
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
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeRealtimeMaxOutputTokenCountAsString(bool fromRawJson)
    {
        RealtimeMaxOutputTokenCount value = fromRawJson
            ? ModelReaderWriter.Read<RealtimeMaxOutputTokenCount>(BinaryData.FromString("\"inf\""))
            : RealtimeDefaultMaxOutputTokenCount.Infinity;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(value.DefaultMaxOutputTokenCount, Is.EqualTo(RealtimeDefaultMaxOutputTokenCount.Infinity));
            Assert.That(value.CustomMaxOutputTokenCount, Is.Null);
            Assert.That(ModelReaderWriter.Write(value).ToString(), Is.EqualTo("\"inf\""));
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeRealtimeMaxOutputTokenCountAsNumber(bool fromRawJson)
    {
        RealtimeMaxOutputTokenCount value = fromRawJson
            ? ModelReaderWriter.Read<RealtimeMaxOutputTokenCount>(BinaryData.FromString("42"))
            : 42;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(value.DefaultMaxOutputTokenCount, Is.Null);
            Assert.That(value.CustomMaxOutputTokenCount, Is.EqualTo(42));
            Assert.That(ModelReaderWriter.Write(value).ToString(), Is.EqualTo("42"));
        }
    }

    [Test]
    public void DeserializeNullRealtimeMaxOutputTokenCount()
    {
        Assert.That(ModelReaderWriter.Read<RealtimeMaxOutputTokenCount>(BinaryData.FromString("null")), Is.Null);
    }

    [Test]
    public void RealtimeMaxOutputTokenCountSupportsRootJsonPatchFromContainingModel()
    {
        RealtimeConversationSessionOptions options = new()
        {
            MaxOutputTokenCount = 42,
        };

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        options.Patch.Set("$.max_output_tokens"u8, "\"inf\""u8);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(json.RootElement.GetProperty("max_output_tokens").GetString(), Is.EqualTo("inf"));
    }

    [TestCase("{}")]
    [TestCase("[]")]
    [TestCase("true")]
    public void DeserializeRealtimeMaxOutputTokenCountRejectsUnsupportedJsonShapes(string json)
    {
        Assert.Throws<JsonException>(() => ModelReaderWriter.Read<RealtimeMaxOutputTokenCount>(BinaryData.FromString(json)));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SerializeRealtimeToolChoiceAsString(bool fromRawJson)
    {
        RealtimeToolChoice choice = fromRawJson
            ? ModelReaderWriter.Read<RealtimeToolChoice>(BinaryData.FromString("\"required\""))
            : RealtimeDefaultToolChoice.Required;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(choice.DefaultToolChoice, Is.EqualTo(RealtimeDefaultToolChoice.Required));
            Assert.That(choice.CustomToolChoice, Is.Null);
            Assert.That(ModelReaderWriter.Write(choice).ToString(), Is.EqualTo("\"required\""));
        }
    }

    [Test]
    public void RealtimeToolChoiceRejectsNullComponents()
    {
        Assert.Throws<ArgumentNullException>(() => new RealtimeToolChoice((RealtimeCustomToolChoice)null));
    }

    [Test]
    public void RealtimeToolChoiceImplicitConversionsPreserveNull()
    {
        RealtimeCustomToolChoice customChoice = null;
        RealtimeToolChoice choice = customChoice;

        Assert.That(choice, Is.Null);
    }

    [Test]
    public void DeserializeNullRealtimeToolChoice()
    {
        Assert.That(ModelReaderWriter.Read<RealtimeToolChoice>(BinaryData.FromString("null")), Is.Null);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SerializeRealtimeToolChoiceAsObject(bool fromRawJson)
    {
        const string json = """{"type":"function","name":"search","additional_property":true}""";
        RealtimeToolChoice choice = fromRawJson
            ? ModelReaderWriter.Read<RealtimeToolChoice>(BinaryData.FromString(json))
            : new RealtimeCustomFunctionToolChoice("search");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(choice.DefaultToolChoice, Is.Null);
            Assert.That(choice.CustomToolChoice, Is.TypeOf<RealtimeCustomFunctionToolChoice>());
            Assert.That(
                ModelReaderWriter.Write(choice).ToString(),
                Is.EqualTo(fromRawJson ? json : """{"type":"function","name":"search"}"""));
        }
    }

    [Test]
    public void RealtimeToolChoicePropagatesJsonPatchToObjectComponent()
    {
        RealtimeConversationSessionOptions options = new() { ToolChoice = new RealtimeCustomFunctionToolChoice("search") };

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        options.Patch.Set("$.tool_choice.additional_property"u8, "patched");
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(json.RootElement.GetProperty("tool_choice").GetProperty("additional_property").GetString(), Is.EqualTo("patched"));
    }

    [Test]
    public void RealtimeToolChoiceSupportsRootJsonPatchFromContainingModel()
    {
        RealtimeConversationSessionOptions options = new() { ToolChoice = new RealtimeCustomFunctionToolChoice("search") };

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        options.Patch.Set("$.tool_choice"u8, "\"required\""u8);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(json.RootElement.GetProperty("tool_choice").GetString(), Is.EqualTo("required"));
    }

    [TestCase("[]")]
    [TestCase("42")]
    [TestCase("true")]
    public void DeserializeRealtimeToolChoiceRejectsUnsupportedJsonShapes(string json)
    {
        Assert.Throws<JsonException>(() => ModelReaderWriter.Read<RealtimeToolChoice>(BinaryData.FromString(json)));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SerializeRealtimeTracingAsString(bool fromRawJson)
    {
        RealtimeTracing tracing = fromRawJson
            ? ModelReaderWriter.Read<RealtimeTracing>(BinaryData.FromString("\"auto\""))
            : RealtimeDefaultTracing.Auto;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tracing.DefaultTracing, Is.EqualTo(RealtimeDefaultTracing.Auto));
            Assert.That(tracing.CustomTracing, Is.Null);
            Assert.That(ModelReaderWriter.Write(tracing).ToString(), Is.EqualTo("\"auto\""));
        }
    }

    [Test]
    public void RealtimeTracingRejectsNullComponents()
    {
        Assert.Throws<ArgumentNullException>(() => new RealtimeTracing((RealtimeCustomTracing)null));
    }

    [Test]
    public void RealtimeTracingImplicitConversionsPreserveNull()
    {
        RealtimeCustomTracing customTracing = null;
        RealtimeTracing tracing = customTracing;
        Assert.That(tracing, Is.Null);
    }

    [Test]
    public void DeserializeNullRealtimeTracing()
    {
        Assert.That(ModelReaderWriter.Read<RealtimeTracing>(BinaryData.FromString("null")), Is.Null);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SerializeRealtimeTracingAsObject(bool fromRawJson)
    {
        const string json = """{"workflow_name":"test","additional_property":true}""";
        RealtimeTracing tracing = fromRawJson
            ? ModelReaderWriter.Read<RealtimeTracing>(BinaryData.FromString(json))
            : new RealtimeCustomTracing { WorkflowName = "test" };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tracing.DefaultTracing, Is.Null);
            Assert.That(tracing.CustomTracing.WorkflowName, Is.EqualTo("test"));
            Assert.That(
                ModelReaderWriter.Write(tracing).ToString(),
                Is.EqualTo(fromRawJson ? json : """{"workflow_name":"test"}"""));
        }
    }

    [Test]
    public void RealtimeTracingPropagatesJsonPatchToObjectComponent()
    {
        RealtimeConversationSessionOptions options = new() { Tracing = new RealtimeCustomTracing() };
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        options.Patch.Set("$.tracing.additional_property"u8, "patched");
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(json.RootElement.GetProperty("tracing").GetProperty("additional_property").GetString(), Is.EqualTo("patched"));
    }

    [Test]
    public void RealtimeTracingSupportsRootJsonPatchFromContainingModel()
    {
        RealtimeConversationSessionOptions options = new() { Tracing = new RealtimeCustomTracing() };
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        options.Patch.Set("$.tracing"u8, "\"auto\""u8);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(json.RootElement.GetProperty("tracing").GetString(), Is.EqualTo("auto"));
    }

    [TestCase("[]")]
    [TestCase("42")]
    [TestCase("true")]
    public void DeserializeRealtimeTracingRejectsUnsupportedJsonShapes(string json)
    {
        Assert.Throws<JsonException>(() => ModelReaderWriter.Read<RealtimeTracing>(BinaryData.FromString(json)));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SerializeRealtimeTruncationAsString(bool fromRawJson)
    {
        RealtimeTruncation truncation = fromRawJson
            ? ModelReaderWriter.Read<RealtimeTruncation>(BinaryData.FromString("\"disabled\""))
            : RealtimeDefaultTruncation.Disabled;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(truncation.DefaultTruncation, Is.EqualTo(RealtimeDefaultTruncation.Disabled));
            Assert.That(truncation.CustomTruncation, Is.Null);
            Assert.That(ModelReaderWriter.Write(truncation).ToString(), Is.EqualTo("\"disabled\""));
        }
    }

    [Test]
    public void RealtimeTruncationRejectsNullComponents()
    {
        Assert.Throws<ArgumentNullException>(() => new RealtimeTruncation((RealtimeCustomTruncation)null));
    }

    [Test]
    public void RealtimeTruncationImplicitConversionsPreserveNull()
    {
        RealtimeCustomTruncation customTruncation = null;
        RealtimeTruncation truncation = customTruncation;
        Assert.That(truncation, Is.Null);
    }

    [Test]
    public void DeserializeNullRealtimeTruncation()
    {
        Assert.That(ModelReaderWriter.Read<RealtimeTruncation>(BinaryData.FromString("null")), Is.Null);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SerializeRealtimeTruncationAsObject(bool fromRawJson)
    {
        const string json = """{"type":"retention_ratio","retention_ratio":0.8,"additional_property":true}""";
        RealtimeTruncation truncation = fromRawJson
            ? ModelReaderWriter.Read<RealtimeTruncation>(BinaryData.FromString(json))
            : new RealtimeCustomRetentionRatioTruncation(0.8f);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(truncation.DefaultTruncation, Is.Null);
            Assert.That(truncation.CustomTruncation, Is.TypeOf<RealtimeCustomRetentionRatioTruncation>());
            Assert.That(
                ModelReaderWriter.Write(truncation).ToString(),
                Is.EqualTo(fromRawJson ? json : """{"type":"retention_ratio","retention_ratio":0.8}"""));
        }
    }

    [Test]
    public void RealtimeTruncationPropagatesJsonPatchToObjectComponent()
    {
        RealtimeConversationSessionOptions options = new() { Truncation = new RealtimeCustomRetentionRatioTruncation(0.8f) };
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        options.Patch.Set("$.truncation.additional_property"u8, "patched");
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(json.RootElement.GetProperty("truncation").GetProperty("additional_property").GetString(), Is.EqualTo("patched"));
    }

    [Test]
    public void RealtimeTruncationSupportsRootJsonPatchFromContainingModel()
    {
        RealtimeConversationSessionOptions options = new() { Truncation = new RealtimeCustomRetentionRatioTruncation(0.8f) };
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        options.Patch.Set("$.truncation"u8, "\"disabled\""u8);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(json.RootElement.GetProperty("truncation").GetString(), Is.EqualTo("disabled"));
    }

    [TestCase("[]")]
    [TestCase("42")]
    [TestCase("true")]
    public void DeserializeRealtimeTruncationRejectsUnsupportedJsonShapes(string json)
    {
        Assert.Throws<JsonException>(() => ModelReaderWriter.Read<RealtimeTruncation>(BinaryData.FromString(json)));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SerializeRealtimeMcpToolCallApprovalPolicyAsString(bool fromRawJson)
    {
        RealtimeMcpToolCallApprovalPolicy policy = fromRawJson
            ? ModelReaderWriter.Read<RealtimeMcpToolCallApprovalPolicy>(BinaryData.FromString("\"always\""))
            : RealtimeDefaultMcpToolCallApprovalPolicy.AlwaysRequireApproval;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(policy.DefaultPolicy, Is.EqualTo(RealtimeDefaultMcpToolCallApprovalPolicy.AlwaysRequireApproval));
            Assert.That(policy.CustomPolicy, Is.Null);
            Assert.That(ModelReaderWriter.Write(policy).ToString(), Is.EqualTo("\"always\""));
        }
    }

    [Test]
    public void RealtimeMcpToolCallApprovalPolicyRejectsNullComponents()
    {
        Assert.Throws<ArgumentNullException>(() => new RealtimeMcpToolCallApprovalPolicy((RealtimeCustomMcpToolCallApprovalPolicy)null));
    }

    [Test]
    public void RealtimeMcpToolCallApprovalPolicyImplicitConversionsPreserveNull()
    {
        RealtimeCustomMcpToolCallApprovalPolicy customPolicy = null;
        RealtimeMcpToolCallApprovalPolicy policy = customPolicy;
        Assert.That(policy, Is.Null);
    }

    [Test]
    public void DeserializeNullRealtimeMcpToolCallApprovalPolicy()
    {
        Assert.That(ModelReaderWriter.Read<RealtimeMcpToolCallApprovalPolicy>(BinaryData.FromString("null")), Is.Null);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SerializeRealtimeMcpToolCallApprovalPolicyAsObject(bool fromRawJson)
    {
        const string json = """{"always":{},"additional_property":true}""";
        RealtimeMcpToolCallApprovalPolicy policy = fromRawJson
            ? ModelReaderWriter.Read<RealtimeMcpToolCallApprovalPolicy>(BinaryData.FromString(json))
            : new RealtimeCustomMcpToolCallApprovalPolicy { ToolsAlwaysRequiringApproval = new RealtimeMcpToolFilter() };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(policy.DefaultPolicy, Is.Null);
            Assert.That(policy.CustomPolicy, Is.Not.Null);
            Assert.That(
                ModelReaderWriter.Write(policy).ToString(),
                Is.EqualTo(fromRawJson ? json : """{"always":{}}"""));
        }
    }

    [Test]
    public void RealtimeMcpToolCallApprovalPolicyPropagatesJsonPatchToObjectComponent()
    {
        RealtimeMcpTool tool = new("test", new Uri("https://example.com"))
        {
            ToolCallApprovalPolicy = new RealtimeCustomMcpToolCallApprovalPolicy { ToolsAlwaysRequiringApproval = new RealtimeMcpToolFilter() }
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
    public void RealtimeMcpToolCallApprovalPolicySupportsRootJsonPatchFromContainingModel()
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

    [TestCase("[]")]
    [TestCase("42")]
    [TestCase("true")]
    public void DeserializeRealtimeMcpToolCallApprovalPolicyRejectsUnsupportedJsonShapes(string json)
    {
        Assert.Throws<JsonException>(() => ModelReaderWriter.Read<RealtimeMcpToolCallApprovalPolicy>(BinaryData.FromString(json)));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void DeserializeRealtimeMcpAllowedTools(bool useShorthandInput)
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

    [Test]
    public void DeserializeRealtimeMcpAllowedToolsPreservesLonghandFields()
    {
        RealtimeMcpTool tool = ModelReaderWriter.Read<RealtimeMcpTool>(BinaryData.FromString(
            """{"type":"mcp","server_label":"test","allowed_tools":{"tool_names":["search"],"read_only":true}}"""));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tool.AllowedTools.ToolNames, Is.EqualTo(new[] { "search" }));
            Assert.That(tool.AllowedTools.IsReadOnly, Is.True);
        }

        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(tool));
        JsonElement allowedTools = json.RootElement.GetProperty("allowed_tools");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(allowedTools.GetProperty("tool_names")[0].GetString(), Is.EqualTo("search"));
            Assert.That(allowedTools.GetProperty("read_only").GetBoolean(), Is.True);
        }
    }

    [Test]
    public void DeserializeEmptyRealtimeMcpAllowedTools()
    {
        RealtimeMcpTool tool = ModelReaderWriter.Read<RealtimeMcpTool>(BinaryData.FromString(
            """{"type":"mcp","server_label":"test","allowed_tools":[]}"""));

        Assert.That(tool.AllowedTools, Is.Not.Null);
        Assert.That(tool.AllowedTools.ToolNames, Is.Empty);

    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        tool.AllowedTools.Patch.Set("$.additional_property"u8, "\"patched\""u8);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(tool));
        JsonElement allowedTools = json.RootElement.GetProperty("allowed_tools");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(allowedTools.GetProperty("tool_names").GetArrayLength(), Is.Zero);
            Assert.That(allowedTools.GetProperty("additional_property").GetString(), Is.EqualTo("patched"));
        }
    }

    [Test]
    public void DeserializeRealtimeMcpAllowedToolsWithEmptyToolNames()
    {
        RealtimeMcpTool tool = ModelReaderWriter.Read<RealtimeMcpTool>(BinaryData.FromString(
            """{"type":"mcp","server_label":"test","allowed_tools":{"tool_names":[]}}"""));

        Assert.That(tool.AllowedTools, Is.Not.Null);
        Assert.That(tool.AllowedTools.ToolNames, Is.Empty);

    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        tool.AllowedTools.Patch.Set("$.additional_property"u8, "\"patched\""u8);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(tool));
        JsonElement allowedTools = json.RootElement.GetProperty("allowed_tools");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(allowedTools.GetProperty("tool_names").GetArrayLength(), Is.Zero);
            Assert.That(allowedTools.GetProperty("additional_property").GetString(), Is.EqualTo("patched"));
        }
    }

    [TestCase("null", false)]
    [TestCase("42", true)]
    [TestCase("true", true)]
    [TestCase("[42]", true)]
    [TestCase("[{}]", true)]
    [TestCase("[true]", true)]
    public void DeserializeRealtimeMcpAllowedToolsHandlesNullAndRejectsUnsupportedJsonShapes(string allowedTools, bool shouldThrow)
    {
        BinaryData data = BinaryData.FromString($$"""{"type":"mcp","server_label":"test","allowed_tools":{{allowedTools}}}""");

        if (shouldThrow)
        {
            Assert.Throws<InvalidOperationException>(() => ModelReaderWriter.Read<RealtimeMcpTool>(data));
        }
        else
        {
            RealtimeMcpTool tool = ModelReaderWriter.Read<RealtimeMcpTool>(data);
            Assert.That(tool.AllowedTools, Is.Null);
        }
    }

    private static ApiKeyCredential GetKeyCredential(RealtimeClient client)
    {
        FieldInfo field = typeof(RealtimeClient).GetField(
            "_keyCredential",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, "RealtimeClient should retain its WebSocket credential");
        return (ApiKeyCredential)field.GetValue(client);
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
