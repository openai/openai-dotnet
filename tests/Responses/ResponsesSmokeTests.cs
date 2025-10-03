using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel.Primitives;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace OpenAI.Tests.Responses;

#pragma warning disable OPENAICUA001

[Category("Responses")]
[Category("Smoke")]
public partial class ResponsesSmokeTests
{
    [Test]
    public void SerializingMessagesWorks()
    {
        ResponseItem userMessageItem = ResponseItem.CreateUserMessageItem(
            [
                ResponseContentPart.CreateInputTextPart("hello, world!"),
            ]);
        string serializedMessage = ModelReaderWriter.Write(userMessageItem).ToString().ToLower();

        Assert.That(serializedMessage, Does.Contain("hello, world"));
        Assert.That(serializedMessage, Does.Contain("message"));
        Assert.That(serializedMessage, Does.Contain("user"));
        Console.WriteLine(serializedMessage);

        ResponseItem deserializedMessage = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(serializedMessage));
        Assert.That(deserializedMessage, Is.Not.Null);
    }

    [Test]
    public void ItemSerialization()
    {
        foreach (ResponseItem item in new ResponseItem[]
        {
            ResponseItem.CreateComputerCallItem("call_abcd", ComputerCallAction.CreateScreenshotAction(), []),
            ResponseItem.CreateComputerCallOutputItem("call_abcd", ComputerCallOutput.CreateScreenshotOutput("file_abcd")),
            ResponseItem.CreateFileSearchCallItem(["query1"]),
            ResponseItem.CreateFunctionCallItem("call_abcd", "function_name", BinaryData.Empty),
            ResponseItem.CreateFunctionCallOutputItem("call_abcd", "functionOutput"),
            ResponseItem.CreateReasoningItem("summary goes here"),
            ResponseItem.CreateReferenceItem("msg_1234"),
            ResponseItem.CreateAssistantMessageItem("Goodbye!", []),
            ResponseItem.CreateDeveloperMessageItem("Talk like a pirate"),
            ResponseItem.CreateSystemMessageItem("Talk like a ninja"),
            ResponseItem.CreateUserMessageItem("Hello, world"),
        })
        {
            BinaryData serializedItem = ModelReaderWriter.Write(item);
            Assert.That(serializedItem?.ToMemory().IsEmpty, Is.False);
            ResponseItem deserializedItem = ModelReaderWriter.Read<ResponseItem>(serializedItem);
            Assert.That(deserializedItem?.GetType(), Is.EqualTo(item.GetType()));
        }

        AssertSerializationRoundTrip<ReferenceResponseItem>(
            @"{""type"":""item_reference"",""id"":""msg_1234""}",
            referenceItem => Assert.That(referenceItem.Id, Is.EqualTo("msg_1234")));
        AssertSerializationRoundTrip<MessageResponseItem>(
            @"{""type"":""message"",""role"":""potato"",""potato_details"":{""cultivar"":""russet""}}",
            potatoMessage =>
            {
                Assert.That(potatoMessage.Role, Is.EqualTo(MessageRole.Unknown));
                Assert.That(potatoMessage.Content, Has.Count.EqualTo(0));
            });
    }

    [Test]
    public void ToolChoiceSerialization()
    {
        void AssertChoiceEqual(ResponseToolChoice choice, string expected)
        {
            string serialized = ModelReaderWriter.Write(choice).ToString();
            Assert.That(serialized, Is.EqualTo(expected));
        }
        AssertChoiceEqual(
            ResponseToolChoice.CreateAutoChoice(), @"""auto""");
        AssertChoiceEqual(
            ResponseToolChoice.CreateNoneChoice(), @"""none""");
        AssertChoiceEqual(
            ResponseToolChoice.CreateRequiredChoice(), @"""required""");
        AssertChoiceEqual(
            ResponseToolChoice.CreateFunctionChoice("foo"),
            @"{""type"":""function"",""name"":""foo""}");
        AssertChoiceEqual(
            ResponseToolChoice.CreateFileSearchChoice(),
            @"{""type"":""file_search""}");
        AssertChoiceEqual(
            ResponseToolChoice.CreateComputerChoice(),
            @"{""type"":""computer_use_preview""}");
        AssertChoiceEqual(
            ResponseToolChoice.CreateWebSearchChoice(),
            @"{""type"":""web_search_preview""}");

        AssertSerializationRoundTrip<ResponseToolChoice>(
            @"{""type"":""something_else""}",
            toolChoice => Assert.That(toolChoice.Kind, Is.EqualTo(ResponseToolChoiceKind.Unknown)));
    }

    [Test]
    public void ToolSerialization()
    {
        Assert.That(
            ModelReaderWriter.Read<ResponseTool>(
                BinaryData.FromString(@"{""type"": ""file_search""}")),
            Is.InstanceOf<ResponseTool>());
        Assert.That(
            ModelReaderWriter.Read<ResponseTool>(
                BinaryData.FromString(@"{""type"": ""something_else""}")),
            Is.InstanceOf<ResponseTool>());
    }

    [Test]
    public void ContentPartSerialization()
    {
        AssertSerializationRoundTrip<ResponseContentPart>(
            @"{""type"":""input_text"",""text"":""hello""}",
            textPart =>
            {
                Assert.That(textPart.Kind, Is.EqualTo(ResponseContentPartKind.InputText));
                Assert.That(textPart.Text, Is.EqualTo("hello"));
            });


        AssertSerializationRoundTrip<ResponseContentPart>(
            @"{""type"":""potato"",""potato_details"":{""cultivar"":""russet""}}",
            potatoPart =>
            {
                Assert.That(potatoPart.Kind, Is.EqualTo(ResponseContentPartKind.Unknown));
                Assert.That(potatoPart.Text, Is.Null);
            });
    }

    [Test]
    public void TextFormatSerialization()
    {
        AssertSerializationRoundTrip<ResponseTextFormat>(
            @"{""type"":""text""}",
            textFormat => Assert.That(textFormat.Kind == ResponseTextFormatKind.Text));
    }

    [Test]
    public void StableInputFileContentPartSerialization()
    {
        string base64HelloWorld = Convert.ToBase64String(Encoding.UTF8.GetBytes("hello world"));

        static void AssertExpectedFilePart(ResponseContentPart filePart)
        {
            Assert.That(filePart.Kind, Is.EqualTo(ResponseContentPartKind.InputFile));
            Assert.That(filePart.InputFileBytesMediaType, Is.EqualTo("text/plain"));
            Assert.That(filePart.InputFilename, Is.EqualTo("test_content_part.txt"));
            Assert.That(Convert.FromBase64String(Convert.ToBase64String(filePart.InputFileBytes.ToArray())), Is.EqualTo("hello world"));
        }

        ResponseContentPart filePart = ResponseContentPart.CreateInputFilePart(
            BinaryData.FromBytes(Encoding.UTF8.GetBytes("hello world")),
            "text/plain",
            "test_content_part.txt");

        AssertExpectedFilePart(filePart);

        BinaryData serializedFilePart = ModelReaderWriter.Write(filePart);
        Assert.That(serializedFilePart, Is.Not.Null);

        ResponseContentPart deserializedFilePart = ModelReaderWriter.Read<ResponseContentPart>(serializedFilePart);

        AssertExpectedFilePart(deserializedFilePart);
    }

    private static void AssertSerializationRoundTrip<T>(
        string serializedJson,
        Action<T> instanceAssertionsAction)
            where T : class, IJsonModel<T>
    {
        BinaryData jsonBytes = BinaryData.FromString(serializedJson);
        Assert.That(jsonBytes?.ToMemory().IsEmpty, Is.False);
        T deserializedValue = ModelReaderWriter.Read<T>(jsonBytes);
        Assert.That(deserializedValue, Is.InstanceOf<T>());
        Assert.Multiple(() =>
        {
            instanceAssertionsAction.Invoke(deserializedValue);
        });
        BinaryData reserializedBytes = ModelReaderWriter.Write(deserializedValue);
        Assert.That(reserializedBytes.ToMemory().IsEmpty, Is.False);
        Assert.That(reserializedBytes.ToString(), Is.EqualTo(serializedJson));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeMCPToolCallPolicyApprovalAsString(bool fromRawJson)
    {
        McpToolCallApprovalPolicy policy;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($"\"always\"");

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            policy = ModelReaderWriter.Read<McpToolCallApprovalPolicy>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            policy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.AlwaysRequireApproval);
        }

        BinaryData serializedPolicy = ModelReaderWriter.Write(policy);
        using JsonDocument policyAsJson = JsonDocument.Parse(serializedPolicy);
        Assert.That(policyAsJson.RootElement, Is.Not.Null);
        Assert.That(policyAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(policyAsJson.RootElement.ToString(), Is.EqualTo("always"));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeMCPToolCallApprovalPolicyAsObject(bool fromRawJson)
    {
        const string alwaysApprovedToolName = "tool_1";
        const string neverApprovedToolName = "tool_2";

        McpToolCallApprovalPolicy policy;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($$"""
            {
                "always": {
                    "tool_names": ["{{alwaysApprovedToolName}}"] 
                },
                "never": {
                    "tool_names": ["{{neverApprovedToolName}}"]
                },
                "additional_property": true
            }
            """);

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            policy = ModelReaderWriter.Read<McpToolCallApprovalPolicy>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            CustomMcpToolCallApprovalPolicy customPolicy = new()
            {
                ToolsAlwaysRequiringApproval = new McpToolFilter()
                {
                    ToolNames = { alwaysApprovedToolName }
                },
                ToolsNeverRequiringApproval = new McpToolFilter()
                {
                    ToolNames = { neverApprovedToolName }
                }
            };

            policy = new McpToolCallApprovalPolicy(customPolicy);
        }

        BinaryData serializedPolicy = ModelReaderWriter.Write(policy);
        using JsonDocument policyAsJson = JsonDocument.Parse(serializedPolicy);
        Assert.That(policyAsJson.RootElement, Is.Not.Null);
        Assert.That(policyAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(policyAsJson.RootElement.TryGetProperty("always", out JsonElement alwaysProperty), Is.True);
        Assert.That(alwaysProperty, Is.Not.Null);
        Assert.That(alwaysProperty.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(alwaysProperty.TryGetProperty("tool_names", out JsonElement alwaysToolNamesProperty), Is.True);
        Assert.That(alwaysToolNamesProperty, Is.Not.Null);
        Assert.That(alwaysToolNamesProperty.ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(alwaysToolNamesProperty.EnumerateArray().FirstOrDefault(), Is.Not.Null);
        Assert.That(alwaysToolNamesProperty.EnumerateArray().FirstOrDefault().ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(alwaysToolNamesProperty.EnumerateArray().FirstOrDefault().ToString(), Is.EqualTo(alwaysApprovedToolName));

        Assert.That(policyAsJson.RootElement.TryGetProperty("never", out JsonElement neverProperty), Is.True);
        Assert.That(neverProperty, Is.Not.Null);
        Assert.That(neverProperty.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(neverProperty.TryGetProperty("tool_names", out JsonElement neverToolNamesProperty), Is.True);
        Assert.That(neverToolNamesProperty, Is.Not.Null);
        Assert.That(neverToolNamesProperty.ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(neverToolNamesProperty.EnumerateArray().FirstOrDefault(), Is.Not.Null);
        Assert.That(neverToolNamesProperty.EnumerateArray().FirstOrDefault().ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(neverToolNamesProperty.EnumerateArray().FirstOrDefault().ToString(), Is.EqualTo(neverApprovedToolName));

        if (fromRawJson)
        {
            // Confirm that we also have the additional data.
            Assert.That(policyAsJson.RootElement.TryGetProperty("additional_property", out JsonElement additionalPropertyProperty), Is.True);
            Assert.That(additionalPropertyProperty, Is.Not.Null);
            Assert.That(additionalPropertyProperty.ValueKind, Is.EqualTo(JsonValueKind.True));
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeCodeInterpreterToolContainerAsString(bool fromRawJson)
    {
        CodeInterpreterToolContainer container;
        string containerId = "myContainerId";

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($"\"{ containerId }\"");

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            container = ModelReaderWriter.Read<CodeInterpreterToolContainer>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            container = new CodeInterpreterToolContainer(containerId);
        }

        BinaryData serializedContainer = ModelReaderWriter.Write(container);
        using JsonDocument containerAsJson = JsonDocument.Parse(serializedContainer);
        Assert.That(containerAsJson.RootElement, Is.Not.Null);
        Assert.That(containerAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(containerAsJson.RootElement.ToString(), Is.EqualTo(containerId));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeCodeInterpreterToolContainerAsObject(bool fromRawJson)
    {
        const string fileId = "myFileId";

        CodeInterpreterToolContainer container;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($$"""
            {
                "type": "auto",
                "file_ids": ["{{fileId}}"],
                "additional_property": true
            }
            """);

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            container = ModelReaderWriter.Read<CodeInterpreterToolContainer>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            AutomaticCodeInterpreterToolContainerConfiguration autoConfig = new()
            {
                FileIds = { fileId }
            };

            container = new CodeInterpreterToolContainer(autoConfig);
        }

        BinaryData serializedContainer = ModelReaderWriter.Write(container);
        using JsonDocument containerAsJson = JsonDocument.Parse(serializedContainer);
        Assert.That(containerAsJson.RootElement, Is.Not.Null);
        Assert.That(containerAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(containerAsJson.RootElement.TryGetProperty("type", out JsonElement typeProperty), Is.True);
        Assert.That(typeProperty, Is.Not.Null);
        Assert.That(typeProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(typeProperty.ToString(), Is.EqualTo("auto"));

        Assert.That(containerAsJson.RootElement.TryGetProperty("file_ids", out JsonElement fileIdsProperty), Is.True);
        Assert.That(fileIdsProperty, Is.Not.Null);
        Assert.That(fileIdsProperty.ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(fileIdsProperty.EnumerateArray().FirstOrDefault(), Is.Not.Null);
        Assert.That(fileIdsProperty.EnumerateArray().FirstOrDefault().ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(fileIdsProperty.EnumerateArray().FirstOrDefault().ToString(), Is.EqualTo(fileId));

        if (fromRawJson)
        {
            // Confirm that we also have the additional data.
            Assert.That(containerAsJson.RootElement.TryGetProperty("additional_property", out JsonElement additionalPropertyProperty), Is.True);
            Assert.That(additionalPropertyProperty, Is.Not.Null);
            Assert.That(additionalPropertyProperty.ValueKind, Is.EqualTo(JsonValueKind.True));
        }
    }
}