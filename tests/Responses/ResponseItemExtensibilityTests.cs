using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Tests.Responses;

[Category("Responses")]
[Category("Smoke")]
public class ResponseItemExtensibilityTests
{
    [TestCase(typeof(ApplyPatchOperation), typeof(ApplyPatchOperationKind))]
    [TestCase(typeof(CodeInterpreterCallOutput), typeof(CodeInterpreterCallOutputKind))]
    [TestCase(typeof(CodeInterpreterToolContainerConfiguration), typeof(CodeInterpreterToolContainerConfigurationKind))]
    [TestCase(typeof(ComputerCallOutput), typeof(ComputerCallOutputKind))]
    [TestCase(typeof(ReasoningSummaryPart), typeof(ReasoningSummaryPartKind))]
    [TestCase(typeof(WebSearchAction), typeof(WebSearchActionKind))]
    [TestCase(typeof(WebSearchActionSource), typeof(WebSearchActionSourceKind))]
    [TestCase(typeof(WebSearchToolLocation), typeof(WebSearchToolLocationKind))]
    public void PublicHierarchyDiscriminatorUsesNamedExtensibleKind(Type modelType, Type kindType)
    {
        var property = modelType.GetProperty("Kind");

        Assert.That(kindType.IsPublic, Is.True);
        Assert.That(kindType.Name, Does.Not.StartWith("Internal").And.EndWith("Kind"));
        Assert.That(property.PropertyType, Is.EqualTo(kindType));
        Assert.That(property.CanWrite, Is.False);
        object customKind = Activator.CreateInstance(kindType, "consumer_extension");
        Assert.That(customKind.ToString(), Is.EqualTo("consumer_extension"));
    }

    [Test]
    public void ConsumerSubclassCanSupplyCustomKind()
    {
        ResponseItemKind kind = new("consumer_extension");
        ResponseItem item = new ConsumerResponseItem(kind);

        Assert.That(item.GetType().Assembly, Is.Not.EqualTo(typeof(ResponseItem).Assembly));
        Assert.That(item.Kind, Is.EqualTo(kind));
        Assert.That(typeof(ResponseItem).GetProperty(nameof(ResponseItem.Kind)).CanWrite, Is.False);
    }

    [TestCase("J")]
    [TestCase("W")]
    public void ConsumerSubclassSerializesCustomDiscriminator(string format)
    {
        ResponseItem item = new ConsumerResponseItem(new ResponseItemKind("consumer_extension"))
        {
            Id = "custom_item_123",
        };

        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(item, new ModelReaderWriterOptions(format)));

        Assert.That(json.RootElement.GetProperty("type").GetString(), Is.EqualTo("consumer_extension"));
        Assert.That(json.RootElement.GetProperty("id").GetString(), Is.EqualTo(item.Id));
    }

    [TestCase("""{"type":"message","id":"msg_123","role":"user","content":[{"type":"input_text","text":"Hello"}]}""",
        typeof(MessageResponseItem), "message")]
    [TestCase("""{"type":"function_call","id":"fc_123","call_id":"call_123","name":"get_weather","arguments":"{}"}""",
        typeof(FunctionCallResponseItem), "function_call")]
    [TestCase("""{"type":"item_reference","id":"msg_123"}""",
        typeof(ReferenceResponseItem), "item_reference")]
    public void KnownDiscriminatorDispatchesToSdkSubclass(string json, Type expectedType, string expectedKind)
    {
        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(BinaryData.FromString(json));

        Assert.That(item, Is.InstanceOf(expectedType));
        Assert.That(item.Kind, Is.EqualTo(new ResponseItemKind(expectedKind)));

        using JsonDocument reserialized = JsonDocument.Parse(ModelReaderWriter.Write(item));
        Assert.That(reserialized.RootElement.GetProperty("type").GetString(), Is.EqualTo(expectedKind));
    }

    [TestCase("J")]
    [TestCase("W")]
    public void UnknownDiscriminatorPreservesKindAndData(string format)
    {
        BinaryData data = BinaryData.FromString("""
            {
                "type": "consumer_extension",
                "id": "custom_item_123",
                "custom_payload": {
                    "text": "Hello",
                    "values": [42, true, null]
                }
            }
            """);
        ModelReaderWriterOptions options = new(format);

        ResponseItem item = ModelReaderWriter.Read<ResponseItem>(data, options);

        Assert.That(item, Is.Not.Null);
        Assert.That(item.Kind, Is.EqualTo(new ResponseItemKind("consumer_extension")));
        Assert.That(item.Id, Is.EqualTo("custom_item_123"));

        using JsonDocument reserialized = JsonDocument.Parse(ModelReaderWriter.Write(item, options));
        Assert.That(reserialized.RootElement.GetProperty("type").GetString(), Is.EqualTo("consumer_extension"));
        Assert.That(reserialized.RootElement.GetProperty("id").GetString(), Is.EqualTo(item.Id));
        JsonElement payload = reserialized.RootElement.GetProperty("custom_payload");
        Assert.That(payload.GetProperty("text").GetString(), Is.EqualTo("Hello"));
        JsonElement values = payload.GetProperty("values");
        Assert.That(values.GetArrayLength(), Is.EqualTo(3));
        Assert.That(values[0].GetInt32(), Is.EqualTo(42));
        Assert.That(values[1].GetBoolean(), Is.True);
        Assert.That(values[2].ValueKind, Is.EqualTo(JsonValueKind.Null));
    }

    private sealed class ConsumerResponseItem : ResponseItem
    {
        public ConsumerResponseItem(ResponseItemKind kind) : base(kind)
        {
        }
    }
}
