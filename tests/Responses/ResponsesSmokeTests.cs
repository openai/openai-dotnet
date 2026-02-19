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
        static void AssertChoiceEqual(ResponseToolChoice choice, string expected)
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
    public void ShellToolChoiceDeserialization()
    {
        AssertToolChoiceRoundTrip(@"{""type"":""shell""}", "shell");
    }

    [Test]
    public void ApplyPatchToolChoiceDeserialization()
    {
        AssertToolChoiceRoundTrip(@"{""type"":""apply_patch""}", "apply_patch");
    }

    [Test]
    public void CustomToolChoiceDeserialization()
    {
        string json = @"{""type"":""custom"",""name"":""my_custom_tool""}";
        ResponseToolChoice choice = ModelReaderWriter.Read<ResponseToolChoice>(BinaryData.FromString(json));
        Assert.That(choice, Is.Not.Null);

        BinaryData reserialized = ModelReaderWriter.Write(choice);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("custom"));
        Assert.That(doc.RootElement.GetProperty("name").GetString(), Is.EqualTo("my_custom_tool"));
    }

    [Test]
    public void AllowedToolsToolChoiceDeserialization()
    {
        string json = @"{""type"":""allowed_tools"",""mode"":""auto"",""tools"":[{""type"":""function"",""name"":""foo""}]}";
        ResponseToolChoice choice = ModelReaderWriter.Read<ResponseToolChoice>(BinaryData.FromString(json));
        Assert.That(choice, Is.Not.Null);

        BinaryData reserialized = ModelReaderWriter.Write(choice);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("allowed_tools"));
        Assert.That(doc.RootElement.GetProperty("mode").GetString(), Is.EqualTo("auto"));
        Assert.That(doc.RootElement.GetProperty("tools").GetArrayLength(), Is.EqualTo(1));
    }

    [Test]
    public void AllowedToolsChoicePreservesToolsArray()
    {
        string json = @"{""type"":""allowed_tools"",""mode"":""required"",""tools"":[{""type"":""function"",""name"":""tool_a""},{""type"":""function"",""name"":""tool_b""}]}";
        ResponseToolChoice choice = ModelReaderWriter.Read<ResponseToolChoice>(BinaryData.FromString(json));

        BinaryData reserialized = ModelReaderWriter.Write(choice);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("mode").GetString(), Is.EqualTo("required"));
        Assert.That(doc.RootElement.GetProperty("tools").GetArrayLength(), Is.EqualTo(2));
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
    public void ShellToolDeserialization()
    {
        ResponseTool tool = ModelReaderWriter.Read<ResponseTool>(
            BinaryData.FromString(@"{""type"":""shell""}"));
        Assert.That(tool, Is.InstanceOf<ShellTool>());

        BinaryData reserialized = ModelReaderWriter.Write(tool);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("shell"));
    }

    [Test]
    public void ShellToolFactoryMethod()
    {
        ShellTool tool = ResponseTool.CreateShellTool();
        Assert.That(tool, Is.InstanceOf<ShellTool>());

        BinaryData serialized = ModelReaderWriter.Write(tool);
        using JsonDocument doc = JsonDocument.Parse(serialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("shell"));
    }

    [Test]
    public void ApplyPatchToolDeserialization()
    {
        ResponseTool tool = ModelReaderWriter.Read<ResponseTool>(
            BinaryData.FromString(@"{""type"":""apply_patch""}"));
        Assert.That(tool, Is.InstanceOf<ApplyPatchTool>());

        BinaryData reserialized = ModelReaderWriter.Write(tool);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("apply_patch"));
    }

    [Test]
    public void ApplyPatchToolFactoryMethod()
    {
        ApplyPatchTool tool = ResponseTool.CreateApplyPatchTool();
        Assert.That(tool, Is.InstanceOf<ApplyPatchTool>());

        BinaryData serialized = ModelReaderWriter.Write(tool);
        using JsonDocument doc = JsonDocument.Parse(serialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("apply_patch"));
    }

    [Test]
    public void CustomToolDeserialization()
    {
        string json = @"{""type"":""custom"",""name"":""my_tool"",""description"":""A custom tool"",""format"":{""type"":""text""}}";
        ResponseTool tool = ModelReaderWriter.Read<ResponseTool>(BinaryData.FromString(json));
        Assert.That(tool, Is.InstanceOf<CustomTool>());

        CustomTool customTool = (CustomTool)tool;
        Assert.That(customTool.Name, Is.EqualTo("my_tool"));
        Assert.That(customTool.Description, Is.EqualTo("A custom tool"));
        Assert.That(customTool.Format, Is.InstanceOf<CustomToolTextFormat>());

        BinaryData reserialized = ModelReaderWriter.Write(tool);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("custom"));
        Assert.That(doc.RootElement.GetProperty("name").GetString(), Is.EqualTo("my_tool"));
        Assert.That(doc.RootElement.GetProperty("description").GetString(), Is.EqualTo("A custom tool"));
        Assert.That(doc.RootElement.GetProperty("format").GetProperty("type").GetString(), Is.EqualTo("text"));
    }

    [Test]
    public void CustomToolFactoryMethod()
    {
        CustomTool tool = ResponseTool.CreateCustomTool("my_tool", "A custom tool");
        Assert.That(tool, Is.InstanceOf<CustomTool>());
        Assert.That(tool.Name, Is.EqualTo("my_tool"));
        Assert.That(tool.Description, Is.EqualTo("A custom tool"));

        BinaryData serialized = ModelReaderWriter.Write(tool);
        using JsonDocument doc = JsonDocument.Parse(serialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("custom"));
        Assert.That(doc.RootElement.GetProperty("name").GetString(), Is.EqualTo("my_tool"));
        Assert.That(doc.RootElement.GetProperty("description").GetString(), Is.EqualTo("A custom tool"));
    }

    [Test]
    public void CustomToolWithGrammarFormatFactoryMethod()
    {
        CustomToolGrammarFormat grammarFormat = new CustomToolGrammarFormat(GrammarSyntax.Lark, "start: rule");
        CustomTool tool = ResponseTool.CreateCustomTool("grammar_tool", "Uses grammar", grammarFormat);
        Assert.That(tool.Format, Is.InstanceOf<CustomToolGrammarFormat>());

        CustomToolGrammarFormat format = (CustomToolGrammarFormat)tool.Format;
        Assert.That(format.Syntax, Is.EqualTo(GrammarSyntax.Lark));
        Assert.That(format.Definition, Is.EqualTo("start: rule"));

        BinaryData serialized = ModelReaderWriter.Write(tool);
        using JsonDocument doc = JsonDocument.Parse(serialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("custom"));
        Assert.That(doc.RootElement.GetProperty("format").GetProperty("type").GetString(), Is.EqualTo("grammar"));
        Assert.That(doc.RootElement.GetProperty("format").GetProperty("syntax").GetString(), Is.EqualTo("lark"));
        Assert.That(doc.RootElement.GetProperty("format").GetProperty("definition").GetString(), Is.EqualTo("start: rule"));
    }

    [Test]
    public void CustomToolWithGrammarFormatDeserialization()
    {
        string json = @"{""type"":""custom"",""name"":""grammar_tool"",""description"":""Uses grammar"",""format"":{""type"":""grammar"",""syntax"":""lark"",""definition"":""start: \""hello\""""}}";
        ResponseTool tool = ModelReaderWriter.Read<ResponseTool>(BinaryData.FromString(json));
        Assert.That(tool, Is.InstanceOf<CustomTool>());

        CustomTool customTool = (CustomTool)tool;
        Assert.That(customTool.Format, Is.InstanceOf<CustomToolGrammarFormat>());
        CustomToolGrammarFormat grammarFormat = (CustomToolGrammarFormat)customTool.Format;
        Assert.That(grammarFormat.Syntax, Is.EqualTo(GrammarSyntax.Lark));
        Assert.That(grammarFormat.Definition, Does.Contain("hello"));

        BinaryData reserialized = ModelReaderWriter.Write(tool);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("format").GetProperty("type").GetString(), Is.EqualTo("grammar"));
        Assert.That(doc.RootElement.GetProperty("format").GetProperty("syntax").GetString(), Is.EqualTo("lark"));
        Assert.That(doc.RootElement.GetProperty("format").GetProperty("definition").GetString(), Does.Contain("hello"));
    }

    [Test]
    public void ShellToolPreservesAdditionalProperties()
    {
        string json = @"{""type"":""shell"",""extra_field"":""extra_value""}";
        ResponseTool tool = ModelReaderWriter.Read<ResponseTool>(BinaryData.FromString(json));
        Assert.That(tool, Is.InstanceOf<ShellTool>());

        BinaryData reserialized = ModelReaderWriter.Write(tool);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("shell"));
        Assert.That(doc.RootElement.TryGetProperty("extra_field", out JsonElement extra), Is.True);
        Assert.That(extra.GetString(), Is.EqualTo("extra_value"));
    }

    [Test]
    public void UnknownToolTypeDeserializesGracefully()
    {
        ResponseTool tool = ModelReaderWriter.Read<ResponseTool>(
            BinaryData.FromString(@"{""type"":""future_tool_type"",""some_property"":42}"));
        Assert.That(tool, Is.InstanceOf<ResponseTool>());

        BinaryData reserialized = ModelReaderWriter.Write(tool);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("future_tool_type"));
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
    public void GrammarTextFormatDeserialization()
    {
        string json = @"{""type"":""grammar"",""grammar"":""start: \""hello\""""}";
        ResponseTextFormat format = ModelReaderWriter.Read<ResponseTextFormat>(BinaryData.FromString(json));
        Assert.That(format, Is.InstanceOf<ResponseTextFormat>());

        BinaryData reserialized = ModelReaderWriter.Write(format);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("grammar"));
        Assert.That(doc.RootElement.GetProperty("grammar").GetString(), Does.Contain("hello"));
    }

    [Test]
    public void PythonTextFormatDeserialization()
    {
        string json = @"{""type"":""python""}";
        ResponseTextFormat format = ModelReaderWriter.Read<ResponseTextFormat>(BinaryData.FromString(json));
        Assert.That(format, Is.InstanceOf<ResponseTextFormat>());

        BinaryData reserialized = ModelReaderWriter.Write(format);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("python"));
    }

    [Test]
    public void UnknownTextFormatDeserializesGracefully()
    {
        ResponseTextFormat format = ModelReaderWriter.Read<ResponseTextFormat>(
            BinaryData.FromString(@"{""type"":""future_format""}"));
        Assert.That(format, Is.InstanceOf<ResponseTextFormat>());
        Assert.That(format.Kind, Is.EqualTo(ResponseTextFormatKind.Unknown));
    }

    [Test]
    public void StableInputFileDataContentPartSerialization()
    {
        static void AssertExpectedFilePart(ResponseContentPart filePart)
        {
            Assert.That(filePart.Kind, Is.EqualTo(ResponseContentPartKind.InputFile));
            Assert.That(filePart.InputFileBytesMediaType, Is.EqualTo("text/plain"));
            Assert.That(filePart.InputFilename, Is.EqualTo("test_content_part.txt"));
            Assert.That(Convert.FromBase64String(Convert.ToBase64String(filePart.InputFileBytes.ToArray())), Is.EqualTo("hello world"));
            Assert.That(filePart.InputFileId, Is.Null);
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

    [Test]
    public void StableInputFileReferenceContentPartSerialization()
    {
        static void AssertExpectedFilePart(ResponseContentPart filePart)
        {
            Assert.That(filePart.Kind, Is.EqualTo(ResponseContentPartKind.InputFile));
            Assert.That(filePart.InputFileId, Is.EqualTo("asst_123abc"));
            Assert.That(filePart.InputFileBytes, Is.Null);
            Assert.That(filePart.InputFileBytesMediaType, Is.Null);
            Assert.That(filePart.InputFilename, Is.Null);
        }

        ResponseContentPart filePart = ResponseContentPart.CreateInputFilePart("asst_123abc");

        AssertExpectedFilePart(filePart);

        BinaryData serializedFilePart = ModelReaderWriter.Write(filePart);
        Assert.That(serializedFilePart, Is.Not.Null);

        ResponseContentPart deserializedFilePart = ModelReaderWriter.Read<ResponseContentPart>(serializedFilePart);

        AssertExpectedFilePart(deserializedFilePart);
    }

    [Test]
    public void StableInputImageDataContentPartSerialization()
    {
        static void AssertExpectedImagePart(ResponseContentPart filePart)
        {
            Assert.That(filePart.Kind, Is.EqualTo(ResponseContentPartKind.InputImage));
            Assert.That(filePart.InputImageDetailLevel, Is.EqualTo(ResponseImageDetailLevel.Low));
            Assert.That(filePart.InputImageUri, Is.EqualTo($"data:image/png;base64,{Convert.ToBase64String(Encoding.UTF8.GetBytes("image data"))}"));
            Assert.That(filePart.InputImageFileId, Is.Null);
        }

        string imageMediaType = "image/png";
        BinaryData imageBytes = BinaryData.FromBytes(Encoding.UTF8.GetBytes("image data"));
        Uri imageDataUri = new($"data:{imageMediaType};base64,{Convert.ToBase64String(imageBytes.ToArray())}");

        ResponseContentPart imagePart = ResponseContentPart.CreateInputImagePart(
            imageDataUri,
            ResponseImageDetailLevel.Low);

        AssertExpectedImagePart(imagePart);

        BinaryData serializedFilePart = ModelReaderWriter.Write(imagePart);
        Assert.That(serializedFilePart, Is.Not.Null);

        ResponseContentPart deserializedFilePart = ModelReaderWriter.Read<ResponseContentPart>(serializedFilePart);

        AssertExpectedImagePart(deserializedFilePart);
    }

    [Test]
    public void StableInputImageFileContentPartSerialization()
    {
        static void AssertExpectedImagePart(ResponseContentPart filePart)
        {
            Assert.That(filePart.Kind, Is.EqualTo(ResponseContentPartKind.InputImage));
            Assert.That(filePart.InputImageDetailLevel, Is.EqualTo(ResponseImageDetailLevel.Auto));
            Assert.That(filePart.InputImageFileId, Is.EqualTo("image_123abc"));
            Assert.That(filePart.InputImageUri, Is.Null);
        }

        ResponseContentPart imagePart = ResponseContentPart.CreateInputImagePart(
            "image_123abc",
            ResponseImageDetailLevel.Auto);

        AssertExpectedImagePart(imagePart);

        BinaryData serializedFilePart = ModelReaderWriter.Write(imagePart);
        Assert.That(serializedFilePart, Is.Not.Null);

        ResponseContentPart deserializedFilePart = ModelReaderWriter.Read<ResponseContentPart>(serializedFilePart);

        AssertExpectedImagePart(deserializedFilePart);
    }

    [Test]
    public void StableInputImageUrlContentPartSerialization()
    {
        static void AssertExpectedImagePart(ResponseContentPart filePart)
        {
            Assert.That(filePart.Kind, Is.EqualTo(ResponseContentPartKind.InputImage));
            Assert.That(filePart.InputImageDetailLevel, Is.EqualTo(ResponseImageDetailLevel.High));
            Assert.That(filePart.InputImageUri, Is.EqualTo("https://example.com/image.jpg"));
            Assert.That(filePart.InputImageFileId, Is.Null);
        }

        ResponseContentPart imagePart = ResponseContentPart.CreateInputImagePart(
            new Uri("https://example.com/image.jpg"),
            ResponseImageDetailLevel.High);

        AssertExpectedImagePart(imagePart);

        BinaryData serializedFilePart = ModelReaderWriter.Write(imagePart);
        Assert.That(serializedFilePart, Is.Not.Null);

        ResponseContentPart deserializedFilePart = ModelReaderWriter.Read<ResponseContentPart>(serializedFilePart);

        AssertExpectedImagePart(deserializedFilePart);
    }

    [Test]
    public void StableInputTextContentPartSerialization()
    {
        static void AssertExpectedTextPart(ResponseContentPart filePart)
        {
            Assert.That(filePart.Kind, Is.EqualTo(ResponseContentPartKind.InputText));
            Assert.That(filePart.Text, Is.EqualTo("input message"));
        }

        ResponseContentPart filePart = ResponseContentPart.CreateInputTextPart("input message");

        AssertExpectedTextPart(filePart);

        BinaryData serializedFilePart = ModelReaderWriter.Write(filePart);
        Assert.That(serializedFilePart, Is.Not.Null);

        ResponseContentPart deserializedFilePart = ModelReaderWriter.Read<ResponseContentPart>(serializedFilePart);

        AssertExpectedTextPart(deserializedFilePart);
    }

    [Test]
    public void StableOutputTextContentPartSerialization()
    {
        static void AssertExpectedTextPart(ResponseContentPart filePart)
        {
            Assert.That(filePart.Kind, Is.EqualTo(ResponseContentPartKind.OutputText));
            Assert.That(filePart.Text, Is.EqualTo("output message"));
            Assert.That(filePart.OutputTextAnnotations.Count, Is.EqualTo(4));

            ContainerFileCitationMessageAnnotation containerFileAnnotation = filePart.OutputTextAnnotations.OfType<ContainerFileCitationMessageAnnotation>().SingleOrDefault();
            Assert.That(containerFileAnnotation, Is.Not.Null);
            Assert.That(containerFileAnnotation.ContainerId, Is.EqualTo("container67"));
            Assert.That(containerFileAnnotation.FileId, Is.EqualTo("file_123abc"));
            Assert.That(containerFileAnnotation.Filename, Is.EqualTo("example.txt"));
            Assert.That(containerFileAnnotation.StartIndex, Is.EqualTo(789));
            Assert.That(containerFileAnnotation.EndIndex, Is.EqualTo(987));

            FileCitationMessageAnnotation fileAnnotation = filePart.OutputTextAnnotations.OfType<FileCitationMessageAnnotation>().SingleOrDefault();
            Assert.That(fileAnnotation, Is.Not.Null);
            Assert.That(fileAnnotation.FileId, Is.EqualTo("file_123abc"));
            Assert.That(fileAnnotation.Filename, Is.EqualTo("example.txt"));
            Assert.That(fileAnnotation.Index, Is.EqualTo(789));

            FilePathMessageAnnotation filePathAnnotation = filePart.OutputTextAnnotations.OfType<FilePathMessageAnnotation>().SingleOrDefault();
            Assert.That(filePathAnnotation, Is.Not.Null);
            Assert.That(filePathAnnotation.FileId, Is.EqualTo("file_123abc"));
            Assert.That(filePathAnnotation.Index, Is.EqualTo(789));

            UriCitationMessageAnnotation uriAnnotation = filePart.OutputTextAnnotations.OfType<UriCitationMessageAnnotation>().SingleOrDefault();
            Assert.That(uriAnnotation, Is.Not.Null);
            Assert.That(uriAnnotation.Uri.AbsoluteUri, Is.EqualTo("https://example.com/document.md"));
            Assert.That(uriAnnotation.StartIndex, Is.EqualTo(789));
            Assert.That(uriAnnotation.EndIndex, Is.EqualTo(987));
        }

        ResponseContentPart textPart = ResponseContentPart.CreateOutputTextPart(
            "output message",
            [
                new ContainerFileCitationMessageAnnotation("container67", "file_123abc", 789, 987, "example.txt"),
                new FileCitationMessageAnnotation("file_123abc", 789, "example.txt"),
                new FilePathMessageAnnotation("file_123abc", 789),
                new UriCitationMessageAnnotation(new Uri("https://example.com/document.md"), 789, 987, "Example Title"),
            ]);

        AssertExpectedTextPart(textPart);

        BinaryData serializedFilePart = ModelReaderWriter.Write(textPart);
        Assert.That(serializedFilePart, Is.Not.Null);

        ResponseContentPart deserializedFilePart = ModelReaderWriter.Read<ResponseContentPart>(serializedFilePart);

        AssertExpectedTextPart(deserializedFilePart);
    }

    [Test]
    public void StableRefusalContentPartSerialization()
    {
        static void AssertExpectedRefusalPart(ResponseContentPart filePart)
        {
            Assert.That(filePart.Kind, Is.EqualTo(ResponseContentPartKind.Refusal));
            Assert.That(filePart.Refusal, Is.EqualTo("no bueno"));
        }

        ResponseContentPart refusalPart = ResponseContentPart.CreateRefusalPart("no bueno");

        AssertExpectedRefusalPart(refusalPart);

        BinaryData serializedFilePart = ModelReaderWriter.Write(refusalPart);
        Assert.That(serializedFilePart, Is.Not.Null);

        ResponseContentPart deserializedFilePart = ModelReaderWriter.Read<ResponseContentPart>(serializedFilePart);

        AssertExpectedRefusalPart(deserializedFilePart);
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

    private static void AssertToolChoiceRoundTrip(string json, string expectedType)
    {
        ResponseToolChoice choice = ModelReaderWriter.Read<ResponseToolChoice>(BinaryData.FromString(json));
        Assert.That(choice, Is.Not.Null);

        BinaryData reserialized = ModelReaderWriter.Write(choice);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo(expectedType));
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
            policy = GlobalMcpToolCallApprovalPolicy.AlwaysRequireApproval;
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
            policy = new CustomMcpToolCallApprovalPolicy()
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

    [Test]
    public void CanSerializeCodeInterpreterCallLogsOutput()
    {
        var customLogs = "Custom logs!";
        CodeInterpreterCallLogsOutput logOutput = new(customLogs);
        logOutput.Patch.Set("$.custom_property"u8, "custom_property");

        BinaryData serialized = ModelReaderWriter.Write(logOutput);
        using JsonDocument doc = JsonDocument.Parse(serialized.ToString());
        JsonElement root = doc.RootElement;

        Assert.That(root, Is.Not.Null);
        Assert.That(root.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(root.TryGetProperty("type", out JsonElement typeProperty), Is.True);
        Assert.That(typeProperty, Is.Not.Null);
        Assert.That(typeProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(typeProperty.ToString(), Is.EqualTo("logs"));

        Assert.That(root.TryGetProperty("logs", out JsonElement logsProperty), Is.True);
        Assert.That(logsProperty, Is.Not.Null);
        Assert.That(logsProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(logsProperty.ToString(), Is.EqualTo(customLogs));

        Assert.That(root.TryGetProperty("custom_property", out JsonElement customProperty), Is.True);
        Assert.That(customProperty, Is.Not.Null);
        Assert.That(customProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(customProperty.ToString(), Is.EqualTo("custom_property"));
    }

    [Test]
    public void ShellCallItemResourceDeserialization()
    {
        string json = @"{""type"":""shell_call"",""id"":""item_001"",""status"":""completed"",""call_id"":""call_abc"",""action"":{""type"":""exec"",""command"":[""ls"",""-la""]}}";
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(json));
        Assert.That(item, Is.InstanceOf<ResponseItem>());

        BinaryData reserialized = ModelReaderWriter.Write(item);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("shell_call"));
        Assert.That(doc.RootElement.GetProperty("id").GetString(), Is.EqualTo("item_001"));
        Assert.That(doc.RootElement.GetProperty("call_id").GetString(), Is.EqualTo("call_abc"));
        Assert.That(doc.RootElement.GetProperty("status").GetString(), Is.EqualTo("completed"));
        Assert.That(doc.RootElement.GetProperty("action").GetProperty("type").GetString(), Is.EqualTo("exec"));
    }

    [Test]
    public void ShellCallOutputItemResourceDeserialization()
    {
        string json = @"{""type"":""shell_call_output"",""id"":""item_002"",""status"":""completed"",""call_id"":""call_abc"",""output"":{""type"":""text"",""text"":""file1.txt\nfile2.txt""},""max_output_length"":1024}";
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(json));
        Assert.That(item, Is.InstanceOf<ResponseItem>());

        BinaryData reserialized = ModelReaderWriter.Write(item);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("shell_call_output"));
        Assert.That(doc.RootElement.GetProperty("id").GetString(), Is.EqualTo("item_002"));
        Assert.That(doc.RootElement.GetProperty("call_id").GetString(), Is.EqualTo("call_abc"));
    }

    [Test]
    public void ApplyPatchCallItemResourceDeserialization()
    {
        string json = @"{""type"":""apply_patch_call"",""id"":""item_003"",""status"":""completed"",""call_id"":""call_def"",""operation"":{""type"":""create_file"",""path"":""/src/new.ts""}}";
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(json));
        Assert.That(item, Is.InstanceOf<ResponseItem>());

        BinaryData reserialized = ModelReaderWriter.Write(item);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("apply_patch_call"));
        Assert.That(doc.RootElement.GetProperty("id").GetString(), Is.EqualTo("item_003"));
        Assert.That(doc.RootElement.GetProperty("call_id").GetString(), Is.EqualTo("call_def"));
        Assert.That(doc.RootElement.GetProperty("operation").GetProperty("type").GetString(), Is.EqualTo("create_file"));
        Assert.That(doc.RootElement.GetProperty("operation").GetProperty("path").GetString(), Is.EqualTo("/src/new.ts"));
    }

    [Test]
    public void ApplyPatchCallItemWithUpdateOperationDeserialization()
    {
        string json = @"{""type"":""apply_patch_call"",""id"":""item_004"",""status"":""in_progress"",""call_id"":""call_ghi"",""operation"":{""type"":""update_file"",""path"":""/src/app.ts"",""diff"":""--- a/src/app.ts\n+++ b/src/app.ts""}}";
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(json));
        Assert.That(item, Is.InstanceOf<ResponseItem>());

        BinaryData reserialized = ModelReaderWriter.Write(item);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("operation").GetProperty("type").GetString(), Is.EqualTo("update_file"));
        Assert.That(doc.RootElement.GetProperty("operation").GetProperty("path").GetString(), Is.EqualTo("/src/app.ts"));
        Assert.That(doc.RootElement.GetProperty("operation").GetProperty("diff").GetString(), Does.Contain("app.ts"));
    }

    [Test]
    public void ApplyPatchCallItemWithDeleteOperationDeserialization()
    {
        string json = @"{""type"":""apply_patch_call"",""id"":""item_005"",""status"":""completed"",""call_id"":""call_jkl"",""operation"":{""type"":""delete_file"",""path"":""/src/old.ts""}}";
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(json));
        Assert.That(item, Is.InstanceOf<ResponseItem>());

        BinaryData reserialized = ModelReaderWriter.Write(item);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("operation").GetProperty("type").GetString(), Is.EqualTo("delete_file"));
        Assert.That(doc.RootElement.GetProperty("operation").GetProperty("path").GetString(), Is.EqualTo("/src/old.ts"));
    }

    [Test]
    public void ApplyPatchCallOutputItemResourceDeserialization()
    {
        string json = @"{""type"":""apply_patch_call_output"",""id"":""item_006"",""status"":""success"",""call_id"":""call_def"",""output"":""Patch applied successfully""}";
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(json));
        Assert.That(item, Is.InstanceOf<ResponseItem>());

        BinaryData reserialized = ModelReaderWriter.Write(item);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("apply_patch_call_output"));
        Assert.That(doc.RootElement.GetProperty("id").GetString(), Is.EqualTo("item_006"));
        Assert.That(doc.RootElement.GetProperty("call_id").GetString(), Is.EqualTo("call_def"));
        Assert.That(doc.RootElement.GetProperty("output").GetString(), Is.EqualTo("Patch applied successfully"));
    }

    [Test]
    public void CustomToolCallItemResourceDeserialization()
    {
        string json = @"{""type"":""custom_tool_call"",""id"":""item_007"",""status"":""completed"",""call_id"":""call_mno"",""name"":""my_custom_tool"",""input"":""some_tool_input""}";
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(json));
        Assert.That(item, Is.InstanceOf<ResponseItem>());

        BinaryData reserialized = ModelReaderWriter.Write(item);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("custom_tool_call"));
        Assert.That(doc.RootElement.GetProperty("id").GetString(), Is.EqualTo("item_007"));
        Assert.That(doc.RootElement.GetProperty("call_id").GetString(), Is.EqualTo("call_mno"));
        Assert.That(doc.RootElement.GetProperty("name").GetString(), Is.EqualTo("my_custom_tool"));
        Assert.That(doc.RootElement.GetProperty("input").GetString(), Is.EqualTo("some_tool_input"));
    }

    [Test]
    public void CustomToolCallOutputItemResourceDeserialization()
    {
        string json = @"{""type"":""custom_tool_call_output"",""id"":""item_008"",""status"":""completed"",""call_id"":""call_mno"",""output"":""tool result""}";
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(json));
        Assert.That(item, Is.InstanceOf<ResponseItem>());

        BinaryData reserialized = ModelReaderWriter.Write(item);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("custom_tool_call_output"));
        Assert.That(doc.RootElement.GetProperty("call_id").GetString(), Is.EqualTo("call_mno"));
        Assert.That(doc.RootElement.GetProperty("output").GetString(), Is.EqualTo("tool result"));
    }

    [Test]
    public void CustomToolCallItemPreservesAllProperties()
    {
        string json = @"{""type"":""custom_tool_call"",""id"":""item_010"",""status"":""in_progress"",""call_id"":""call_xyz"",""name"":""analyzer"",""input"":""data"",""future_prop"":true}";
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(json));

        BinaryData reserialized = ModelReaderWriter.Write(item);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("custom_tool_call"));
        Assert.That(doc.RootElement.GetProperty("name").GetString(), Is.EqualTo("analyzer"));
        Assert.That(doc.RootElement.GetProperty("input").GetString(), Is.EqualTo("data"));
        Assert.That(doc.RootElement.GetProperty("status").GetString(), Is.EqualTo("in_progress"));
        Assert.That(doc.RootElement.TryGetProperty("future_prop", out JsonElement futureVal), Is.True);
        Assert.That(futureVal.GetBoolean(), Is.True);
    }

    [Test]
    public void UnknownItemTypeDeserializesGracefully()
    {
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(
            BinaryData.FromString(@"{""type"":""future_item_type"",""id"":""item_999""}"));
        Assert.That(item, Is.InstanceOf<ResponseItem>());

        BinaryData reserialized = ModelReaderWriter.Write(item);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("future_item_type"));
    }

    [Test]
    public void CustomToolCallInputDeltaEventDeserialization()
    {
        string json = @"{""type"":""response.custom_tool_call_input.delta"",""sequence_number"":5,""output_index"":0,""item_id"":""item_007"",""delta"":""partial_data""}";
        StreamingResponseUpdate update = ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(json));
        Assert.That(update, Is.InstanceOf<StreamingResponseUpdate>());
        Assert.That(update.SequenceNumber, Is.EqualTo(5));

        BinaryData reserialized = ModelReaderWriter.Write(update);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("response.custom_tool_call_input.delta"));
        Assert.That(doc.RootElement.GetProperty("output_index").GetInt32(), Is.EqualTo(0));
        Assert.That(doc.RootElement.GetProperty("item_id").GetString(), Is.EqualTo("item_007"));
        Assert.That(doc.RootElement.GetProperty("delta").GetString(), Is.EqualTo("partial_data"));
    }

    [Test]
    public void CustomToolCallInputDoneEventDeserialization()
    {
        string json = @"{""type"":""response.custom_tool_call_input.done"",""sequence_number"":10,""output_index"":0,""item_id"":""item_007"",""input"":""complete_input""}";
        StreamingResponseUpdate update = ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(json));
        Assert.That(update, Is.InstanceOf<StreamingResponseUpdate>());
        Assert.That(update.SequenceNumber, Is.EqualTo(10));

        BinaryData reserialized = ModelReaderWriter.Write(update);
        using JsonDocument doc = JsonDocument.Parse(reserialized);
        Assert.That(doc.RootElement.GetProperty("type").GetString(), Is.EqualTo("response.custom_tool_call_input.done"));
        Assert.That(doc.RootElement.GetProperty("input").GetString(), Is.EqualTo("complete_input"));
    }

    [Test]
    public void UnknownStreamingEventDeserializesGracefully()
    {
        StreamingResponseUpdate update = ModelReaderWriter.Read<StreamingResponseUpdate>(
            BinaryData.FromString(@"{""type"":""response.future_event"",""sequence_number"":99}"));
        Assert.That(update, Is.InstanceOf<StreamingResponseUpdate>());
        Assert.That(update.SequenceNumber, Is.EqualTo(99));
    }

    [Test]
    public void PromptDeserializationInCreateResponseOptions()
    {
        string optionsJson = @"{""model"":""gpt-4o"",""input"":""Hello"",""prompt"":{""id"":""prompt_abc"",""version"":""v2"",""variables"":{""name"":""Alice"",""lang"":""en""}}}";

        BinaryData data = BinaryData.FromString(optionsJson);
        using JsonDocument doc = JsonDocument.Parse(data);
        Assert.That(doc.RootElement.TryGetProperty("prompt", out JsonElement promptElement), Is.True);
        Assert.That(promptElement.GetProperty("id").GetString(), Is.EqualTo("prompt_abc"));
        Assert.That(promptElement.GetProperty("version").GetString(), Is.EqualTo("v2"));
        Assert.That(promptElement.GetProperty("variables").GetProperty("name").GetString(), Is.EqualTo("Alice"));
        Assert.That(promptElement.GetProperty("variables").GetProperty("lang").GetString(), Is.EqualTo("en"));
    }

    [Test]
    public void ValidateCreateResponseOptionsClone()
    {
        var original = new CreateResponseOptions();
        CloneTestHelper.ValidateCloneMethod(original);
    }
}