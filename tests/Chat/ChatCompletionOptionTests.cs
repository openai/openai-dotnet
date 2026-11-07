using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel.Primitives;
using System.Reflection;
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

    [Test]
    public void Clone_PreservesOptionalPropertyPresence()
    {
        var unsetOptions = new ChatCompletionOptions();
        var cloneMethod = typeof(ChatCompletionOptions).GetMethod("Clone", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        var unsetClone = (ChatCompletionOptions)cloneMethod.Invoke(unsetOptions, null);

        using JsonDocument unsetDocument = JsonDocument.Parse(ModelReaderWriter.Write(unsetClone).ToString());
        Assert.Multiple(() =>
        {
            Assert.That(unsetDocument.RootElement.TryGetProperty("temperature", out _), Is.False);
            Assert.That(unsetDocument.RootElement.TryGetProperty("top_p", out _), Is.False);
            Assert.That(unsetDocument.RootElement.TryGetProperty("frequency_penalty", out _), Is.False);
            Assert.That(unsetDocument.RootElement.TryGetProperty("presence_penalty", out _), Is.False);
            Assert.That(unsetDocument.RootElement.TryGetProperty("seed", out _), Is.False);
        });

        var nullOptions = new ChatCompletionOptions
        {
            Temperature = null,
            TopP = null,
            FrequencyPenalty = null,
            PresencePenalty = null,
            Seed = null,
        };
        var nullClone = (ChatCompletionOptions)cloneMethod.Invoke(nullOptions, null);

        using JsonDocument nullDocument = JsonDocument.Parse(ModelReaderWriter.Write(nullClone).ToString());
        Assert.Multiple(() =>
        {
            Assert.That(nullDocument.RootElement.GetProperty("temperature").ValueKind, Is.EqualTo(JsonValueKind.Null));
            Assert.That(nullDocument.RootElement.GetProperty("top_p").ValueKind, Is.EqualTo(JsonValueKind.Null));
            Assert.That(nullDocument.RootElement.GetProperty("frequency_penalty").ValueKind, Is.EqualTo(JsonValueKind.Null));
            Assert.That(nullDocument.RootElement.GetProperty("presence_penalty").ValueKind, Is.EqualTo(JsonValueKind.Null));
            Assert.That(nullDocument.RootElement.GetProperty("seed").ValueKind, Is.EqualTo(JsonValueKind.Null));
        });
    }
}
