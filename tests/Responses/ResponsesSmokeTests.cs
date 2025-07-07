using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel.Primitives;
using System.Text;

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
            ResponseItem.CreateComputerCallOutputItem("call_abcd", [], "file_abcd"),
            ResponseItem.CreateFileSearchCallItem(["query1"], []),
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
            @"{""type"":""message"",""potato_details"":{""cultivar"":""russet""},""role"":""potato""}",
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
}