using NUnit.Framework;
using OpenAI.Decisions;
using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Tests.Decisions;

[Category("Decisions")]
[Category("Smoke")]
public class DecisionsJsonContentPartTests
{
    // Verifies JSON content and instruction parts preserve multiple free-form objects as raw JSON arrays.
    [Test]
    public void JsonDataArrayPreservesEachObjectAndSerializesAsJson()
    {
        const string inputJson = """{"type":"input_json","data":[{"count":2},{"enabled":true}]}""";
        DecisionsInputJsonContentPart input = ModelReaderWriter.Read<DecisionsInputJsonContentPart>(BinaryData.FromString(inputJson));
        DecisionsInputJsonInstructionsPart instructions = ModelReaderWriter.Read<DecisionsInputJsonInstructionsPart>(BinaryData.FromString(inputJson));

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

    // Verifies JSON content and instruction data reject values that are not objects or arrays of objects.
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
                Assert.Throws<JsonException>(() => ModelReaderWriter.Read<DecisionsInputJsonContentPart>(content));
            }
            else
            {
                Assert.Throws<JsonException>(() => ModelReaderWriter.Read<DecisionsInputJsonInstructionsPart>(content));
            }
        }
    }

}
