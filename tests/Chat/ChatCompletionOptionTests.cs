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
}
