using NUnit.Framework;
using OpenAI.Decisions;
using System;
using System.ClientModel.Primitives;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace OpenAI.Tests.Decisions;

[Category("Decisions")]
[Category("Smoke")]
public class DecisionsJsonDataTests
{
    [TestCase("{}")]
    [TestCase("[]")]
    [TestCase("""{"name":"test","count":1,"enabled":false,"nothing":null,"nested":{"values":[true,null,2.5,"text"]}}""")]
    [TestCase("""[{"id":1},{"id":2,"nested":{"value":true}}]""")]
    [TestCase("""{"object_value":"not a synthetic component","array_value":[false],"a.b":1,"a[b]":2,"":3,"quote\"":4}""")]
    public void JsonDataSupportsBothComponentsWithoutChangingTheWireShape(string json)
    {
        DecisionsJsonData deserialized = ModelReaderWriter.Read<DecisionsJsonData>(BinaryData.FromString(json));
        DecisionsJsonData constructed = deserialized.ObjectValue is not null
            ? new DecisionsJsonData(deserialized.ObjectValue)
            : new DecisionsJsonData(deserialized.ArrayValue);
        DecisionsJsonData converted = deserialized.ObjectValue is not null
            ? (DecisionsJsonData)deserialized.ObjectValue
            : deserialized.ArrayValue.ToArray();
        Utf8JsonReader reader = new(Encoding.UTF8.GetBytes(json));
        reader.Read();
        DecisionsJsonData created = ((IJsonModel<DecisionsJsonData>)deserialized).Create(ref reader, ModelReaderWriterOptions.Json);

        foreach (DecisionsJsonData value in new[] { deserialized, constructed, converted, created })
        {
            Assert.That((value.ObjectValue is not null) ^ (value.ArrayValue is not null), Is.True);
            foreach (string format in new[] { "J", "W" })
            {
                BinaryData serialized = ModelReaderWriter.Write(value, new(format));
                AssertJsonEqual(json, serialized);
                DecisionsJsonData roundTrip = ModelReaderWriter.Read<DecisionsJsonData>(serialized);
                Assert.That(roundTrip.ObjectValue is not null, Is.EqualTo(value.ObjectValue is not null));
                Assert.That(roundTrip.ArrayValue?.Count, Is.EqualTo(value.ArrayValue?.Count));
            }
        }
    }

    [Test]
    public void JsonDataComponentsAreReadOnlyNullableAndNotPubliclyPatchable()
    {
        Assert.That(typeof(DecisionsJsonData).GetProperty(nameof(DecisionsJsonData.ObjectValue)).SetMethod, Is.Null);
        Assert.That(typeof(DecisionsJsonData).GetProperty(nameof(DecisionsJsonData.ArrayValue)).SetMethod, Is.Null);
        Assert.That(typeof(DecisionsJsonData).GetProperty("Patch"), Is.Null);
        Assert.That(typeof(DecisionsJsonData).GetConstructor(Type.EmptyTypes), Is.Null);
        Assert.Throws<ArgumentNullException>(() => new DecisionsJsonData((DecisionsJsonObject)null));
        Assert.Throws<ArgumentNullException>(() => new DecisionsJsonData((DecisionsJsonObject[])null));
        Assert.Throws<ArgumentException>(() => new DecisionsJsonData(new DecisionsJsonObject[] { null }));
        DecisionsJsonObject nullObject = null;
        DecisionsJsonObject[] nullArray = null;
        DecisionsJsonData objectConversion = nullObject;
        DecisionsJsonData arrayConversion = nullArray;
        Assert.That(objectConversion, Is.Null);
        Assert.That(arrayConversion, Is.Null);
        Assert.That(ModelReaderWriter.Read<DecisionsJsonData>(BinaryData.FromString("null")), Is.Null);
        DecisionsJsonObject[] source = [new()];
        DecisionsJsonData copied = new(source);
        source[0] = null;
        Assert.That(copied.ArrayValue[0], Is.Not.Null);
    }

    [TestCase("true")]
    [TestCase("false")]
    [TestCase("42")]
    [TestCase("\"text\"")]
    [TestCase("[null]")]
    [TestCase("[[]]")]
    [TestCase("[true]")]
    [TestCase("""[{"valid":true},42]""")]
    public void JsonDataRejectsUnsupportedShapesDirectlyAndInContentParts(string json)
    {
        Assert.Throws<JsonException>(() => ModelReaderWriter.Read<DecisionsJsonData>(BinaryData.FromString(json)));
        BinaryData content = BinaryData.FromString($$"""{"type":"input_json","data":{{json}}}""");
        Assert.Throws<JsonException>(() => ModelReaderWriter.Read<DecisionsInputJsonContentPart>(content));
        Assert.Throws<JsonException>(() => ModelReaderWriter.Read<DecisionsInstructionJsonContentPart>(content));
    }

    [TestCase("null")]
    [TestCase("true")]
    [TestCase("42")]
    [TestCase("\"text\"")]
    [TestCase("[]")]
    public void JsonObjectConstructorRejectsNonObjects(string json)
    {
        Assert.Throws<JsonException>(() => new DecisionsJsonObject(BinaryData.FromString(json)));
    }

    [Test]
    public void JsonObjectConstructorAcceptsArbitraryPropertiesAndRejectsNullData()
    {
        const string json = """{"name":"sample","nullable":null,"nested":{"arbitrary":[1,true,"text"]}}""";
        DecisionsJsonObject value = new(BinaryData.FromString(json));
        AssertJsonEqual(json, ModelReaderWriter.Write(value));
        Assert.Throws<ArgumentNullException>(() => new DecisionsJsonObject(null));
        value.Patch.Set("$.nested.added"u8, false);
        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(value));
        Assert.That(serialized.RootElement.GetProperty("nested").GetProperty("added").GetBoolean(), Is.False);
        Assert.That(serialized.RootElement.GetProperty("nested").GetProperty("arbitrary").GetArrayLength(), Is.EqualTo(3));
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false, Ignore = "Pending generator array-path fix: https://github.com/microsoft/typespec/issues/12431")]
    [TestCase(true, true, Ignore = "Pending generator array-path fix: https://github.com/microsoft/typespec/issues/12431")]
    public void ContainingModelPatchesReachObjectAndArrayComponents(bool isArray, bool fromJson)
    {
        DecisionsInputJsonContentPart part = CreatePart(isArray, fromJson);
        byte[] path = Encoding.UTF8.GetBytes(isArray ? "$.data[0].nested.flag" : "$.data.nested.flag");
        part.Patch.Set(path, true);
        part.Patch.Remove(Encoding.UTF8.GetBytes(isArray ? "$.data[0].remove" : "$.data.remove"));
        Assert.That(part.Patch.TryGetEncodedValue(path, out _), Is.True);
        DecisionsJsonObject activeObject = isArray ? part.Data.ArrayValue[0] : part.Data.ObjectValue;
        Assert.That(activeObject.Patch.TryGetEncodedValue("$.nested.flag"u8, out _), Is.True);

        foreach (string format in new[] { "J", "W" })
        {
            using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(part, new(format)));
            JsonElement value = serialized.RootElement.GetProperty("data");
            if (isArray)
            {
                Assert.That(value.GetArrayLength(), Is.EqualTo(2));
                value = value[0];
            }
            Assert.That(value.GetProperty("nested").GetProperty("flag").GetBoolean(), Is.True);
            Assert.That(value.GetProperty("nested").GetProperty("other").GetInt32(), Is.EqualTo(7));
            Assert.That(value.TryGetProperty("remove", out _), Is.False);
            Assert.That(value.GetProperty("future").GetProperty("values")[0].GetBoolean(), Is.False);
        }
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void ContainingModelsCanReplaceTheCompleteJsonValue(bool isArray, bool fromJson)
    {
        foreach (string replacement in new[] { """{"replacement":true}""", """[{"replacement":false}]""", "[]", "null" })
        {
            DecisionsInputJsonContentPart part = CreatePart(isArray, fromJson);
            part.Patch.Set("$.data"u8, Encoding.UTF8.GetBytes(replacement).AsSpan());
            using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(part));
            using JsonDocument expected = JsonDocument.Parse(replacement);
            Assert.That(JsonElement.DeepEquals(expected.RootElement, serialized.RootElement.GetProperty("data")), Is.True);
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    [Ignore("Pending generator array-path fix: https://github.com/microsoft/typespec/issues/12431")]
    public void ArrayComponentsSupportItemReplacementRemovalAndAppends(bool fromJson)
    {
        DecisionsInputJsonContentPart part = CreatePart(true, fromJson);
        part.Patch.Set("$.data[0]"u8, """{"replacement":true}"""u8);
        part.Patch.Remove("$.data[1]"u8);
        part.Patch.Set("$.data[2]"u8, """{"appended":true}"""u8);
        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(part));
        JsonElement array = serialized.RootElement.GetProperty("data");
        Assert.That(array.GetArrayLength(), Is.EqualTo(2));
        Assert.That(array[0].GetProperty("replacement").GetBoolean(), Is.True);
        Assert.That(array[1].GetProperty("appended").GetBoolean(), Is.True);
    }

    [TestCase(0)]
    [TestCase(10)]
    [TestCase(100)]
    [Ignore("Pending generator array-path fix: https://github.com/microsoft/typespec/issues/12431")]
    public void ArrayComponentPatchesConsumeTheCompleteIndex(int index)
    {
        DecisionsInputJsonContentPart part = new()
        {
            Data = Enumerable.Range(0, index + 1).Select(_ => new DecisionsJsonObject()).ToArray(),
        };
        byte[] path = Encoding.UTF8.GetBytes($"$.data[{index}].value");
        part.Patch.Set(path, true);
        Assert.That(part.Data.ArrayValue[index].Patch.GetBoolean("$.value"u8), Is.True);
        Assert.That(part.Patch.TryGetEncodedValue(path, out _), Is.True);

        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(part));
        Assert.That(serialized.RootElement.GetProperty("data")[index].GetProperty("value").GetBoolean(), Is.True);
    }

    [Test]
    public void UserAndInstructionJsonContentUseTheSameValueRepresentation()
    {
        DecisionsJsonData value = new(new DecisionsJsonObject(BinaryData.FromString("""{"shared":{"flag":false}}""")));
        DecisionsInputJsonContentPart input = new() { Data = value };
        DecisionsInstructionJsonContentPart instruction = new() { Data = value };
        instruction.Patch.Set("$.data.shared.flag"u8, true);
        using JsonDocument inputJson = JsonDocument.Parse(ModelReaderWriter.Write(input));
        using JsonDocument instructionJson = JsonDocument.Parse(ModelReaderWriter.Write(instruction));
        Assert.That(JsonElement.DeepEquals(inputJson.RootElement, instructionJson.RootElement), Is.True);
        Assert.That(instructionJson.RootElement.GetProperty("data").GetProperty("shared").GetProperty("flag").GetBoolean(), Is.True);
    }

    private static DecisionsInputJsonContentPart CreatePart(bool isArray, bool fromJson)
    {
        const string json = """{"nested":{"flag":false,"other":7},"remove":42,"future":{"values":[false,null,"text"]}}""";
        DecisionsJsonObject value = new(BinaryData.FromString(json));
        DecisionsJsonData data = isArray ? new DecisionsJsonObject[] { value, new() } : (DecisionsJsonData)value;
        DecisionsInputJsonContentPart part = new() { Data = data };
        return fromJson ? ModelReaderWriter.Read<DecisionsInputJsonContentPart>(ModelReaderWriter.Write(part)) : part;
    }

    private static void AssertJsonEqual(string expected, BinaryData actual)
    {
        using JsonDocument expectedDocument = JsonDocument.Parse(expected);
        using JsonDocument actualDocument = JsonDocument.Parse(actual);
        Assert.That(JsonElement.DeepEquals(expectedDocument.RootElement, actualDocument.RootElement), Is.True, actual.ToString());
    }
}
