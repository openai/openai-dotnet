using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace OpenAI.Tests.Chat;

[Category("Chat")]
public class ChatCompletionOptionTests
{
    [Test]
    public void Validate_Clone()
    {
        var original = new ChatCompletionOptions();
        CloneTestHelper.ValidateCloneMethod(original);
    }

    [Test]
    public void Clone_PreservesPatch()
    {
        var original = new ChatCompletionOptions();
        original.Patch.Set("$.custom_property"u8, "custom_value");

        var cloneMethod = typeof(ChatCompletionOptions).GetMethod("Clone", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        var clone = (ChatCompletionOptions)cloneMethod.Invoke(original, null);

        BinaryData serialized = ModelReaderWriter.Write(clone);
        using JsonDocument doc = JsonDocument.Parse(serialized.ToString());
        JsonElement root = doc.RootElement;

        Assert.That(root.TryGetProperty("custom_property", out JsonElement customProperty), Is.True);
        Assert.That(customProperty.ToString(), Is.EqualTo("custom_value"));
    }

#pragma warning disable SCME0001
    [Test]
    public void StreamingPatchPreservesInjectedIncludeUsage()
    {
        var options = new ChatCompletionOptions();
        options.Patch.Set("$.stream_options.custom_property"u8, "custom_value");

        string requestBody = null;
        MockPipelineTransport transport = new(message =>
        {
            using MemoryStream stream = new();
            message.Request.Content.WriteTo(stream);
            requestBody = Encoding.UTF8.GetString(stream.ToArray());
            return new MockPipelineResponse(200).WithContent("data: [DONE]\n\n");
        });
        ChatClient client = new(
            "model",
            new ApiKeyCredential("key"),
            new OpenAIClientOptions { Transport = transport });

        foreach (StreamingChatCompletionUpdate _ in client.CompleteChatStreaming([new UserChatMessage("hello")], options))
        {
        }

        using JsonDocument document = JsonDocument.Parse(requestBody);
        JsonElement streamOptions = document.RootElement.GetProperty("stream_options");

        Assert.Multiple(() =>
        {
            Assert.That(streamOptions.GetProperty("include_usage").GetBoolean(), Is.True);
            Assert.That(streamOptions.GetProperty("custom_property").GetString(), Is.EqualTo("custom_value"));
            Assert.That(options.Patch.Contains("$.stream_options.include_usage"u8), Is.False);
        });
    }
#pragma warning restore SCME0001
}
