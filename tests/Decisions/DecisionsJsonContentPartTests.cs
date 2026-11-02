using NUnit.Framework;
using OpenAI.Decisions;
using System;
using System.ClientModel.Primitives;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Tests.Decisions;

[Category("Decisions")]
[Category("Smoke")]
public class DecisionsJsonContentPartTests
{
    [TestCase("""{"type":"input_json","data":{"count":2}}""")]
    [TestCase("""{"type":"input_json","data":[{"count":2}]}""")]
    public void InputJsonDataUsesRawJsonObjects(string json)
    {
        DecisionsContentInputJsonPart part = ModelReaderWriter.Read<DecisionsContentInputJsonPart>(BinaryData.FromString(json));

        Assert.That(part.Data, Has.Count.EqualTo(1));
        using JsonDocument data = JsonDocument.Parse(part.Data.Single());
        Assert.That(data.RootElement.GetProperty("count").GetInt32(), Is.EqualTo(2));
        AssertDataIsObjectArray(ModelReaderWriter.Write(part));
    }

    [TestCase("""{"type":"input_json","data":{"count":2}}""")]
    [TestCase("""{"type":"input_json","data":[{"count":2}]}""")]
    public void InstructionJsonDataUsesRawJsonObjects(string json)
    {
        DecisionsInstructionsInputJsonPart part = ModelReaderWriter.Read<DecisionsInstructionsInputJsonPart>(BinaryData.FromString(json));

        Assert.That(part.Data, Has.Count.EqualTo(1));
        using JsonDocument data = JsonDocument.Parse(part.Data.Single());
        Assert.That(data.RootElement.GetProperty("count").GetInt32(), Is.EqualTo(2));
        AssertDataIsObjectArray(ModelReaderWriter.Write(part));
    }

    [Test]
    public void JsonDataArrayPreservesEachObjectAndSerializesAsJson()
    {
        const string inputJson = """{"type":"input_json","data":[{"count":2},{"enabled":true}]}""";
        DecisionsContentInputJsonPart input = ModelReaderWriter.Read<DecisionsContentInputJsonPart>(BinaryData.FromString(inputJson));
        DecisionsInstructionsInputJsonPart instructions = ModelReaderWriter.Read<DecisionsInstructionsInputJsonPart>(BinaryData.FromString(inputJson));

        Assert.That(input.Data, Has.Count.EqualTo(2));
        Assert.That(instructions.Data, Has.Count.EqualTo(2));
        foreach (object part in new object[] { input, instructions })
        {
            using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(part));
            JsonElement data = serialized.RootElement.GetProperty("data");
            Assert.That(data.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(data[0].GetProperty("count").GetInt32(), Is.EqualTo(2));
            Assert.That(data[1].GetProperty("enabled").GetBoolean(), Is.True);
        }
    }

    [TestCase("null")]
    [TestCase("true")]
    [TestCase("42")]
    [TestCase("\"text\"")]
    [TestCase("[null]")]
    [TestCase("[42]")]
    [TestCase("[[]]")]
    public void JsonDataRejectsValuesOtherThanObjectsAndObjectArrays(string json)
    {
        foreach (string contentType in new[] { "input_json", "instruction_json" })
        {
            BinaryData content = BinaryData.FromString($$"""{"type":"{{contentType}}","data":{{json}}}""");
            if (contentType == "input_json")
            {
                Assert.Throws<JsonException>(() => ModelReaderWriter.Read<DecisionsContentInputJsonPart>(content));
            }
            else
            {
                Assert.Throws<JsonException>(() => ModelReaderWriter.Read<DecisionsInstructionsInputJsonPart>(content));
            }
        }
    }

    private static void AssertDataIsObjectArray(BinaryData serialized)
    {
        using JsonDocument document = JsonDocument.Parse(serialized);
        JsonElement data = document.RootElement.GetProperty("data");
        Assert.That(data.ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(data[0].GetProperty("count").GetInt32(), Is.EqualTo(2));
    }
}
