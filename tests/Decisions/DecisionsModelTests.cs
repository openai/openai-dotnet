using NUnit.Framework;
using OpenAI.Decisions;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace OpenAI.Tests.Decisions;

[Category("Decisions")]
[Category("Smoke")]
public class DecisionsModelTests
{
    [Test]
    public void RenamedUsagePropertiesPreserveWireNames()
    {
        const string json = """
            {"input_tokens":10,"output_tokens":5,"total_tokens":15,
             "input_tokens_details":{"cached_tokens":3,"cache_write_tokens":2},
             "output_tokens_details":{"reasoning_tokens":4}}
            """;
        DecisionsUsage usage = Read<DecisionsUsage>(json);

        Assert.That(usage.InputTokenCount, Is.EqualTo(10));
        Assert.That(usage.OutputTokenCount, Is.EqualTo(5));
        Assert.That(usage.TotalTokenCount, Is.EqualTo(15));
        Assert.That(usage.InputTokenDetails.CachedTokenCount, Is.EqualTo(3));
        Assert.That(usage.InputTokenDetails.CacheWriteTokenCount, Is.EqualTo(2));
        Assert.That(usage.OutputTokenDetails.ReasoningTokenCount, Is.EqualTo(4));

        using JsonDocument expected = JsonDocument.Parse(json);
        using JsonDocument actual = JsonDocument.Parse(ModelReaderWriter.Write(usage));
        AssertJsonContains(expected.RootElement, actual.RootElement);
        Assert.That(actual.RootElement.EnumerateObject().Count(), Is.EqualTo(5));
    }

    [Test]
    public void RenamedInputAndImagePropertiesPreserveWireNames()
    {
        DecisionCreationOptions options = new()
        {
            Model = "test",
            InputItems =
            {
                new DecisionsMessageItem
                {
                    Content =
                    {
                        new DecisionsInputImageContentPart
                        {
                            ImageUri = "https://example.invalid/image.png",
                            ImageDetailLevel = DecisionsImageDetailLevel.High
                        }
                    }
                }
            }
        };
        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(options));
        JsonElement image = json.RootElement.GetProperty("input")[0].GetProperty("content")[0];
        Assert.That(image.GetProperty("image_url").GetString(), Is.EqualTo("https://example.invalid/image.png"));
        Assert.That(image.GetProperty("detail").GetString(), Is.EqualTo("high"));
        Assert.That(image.EnumerateObject().Count(), Is.EqualTo(3));
        Assert.That(json.RootElement.TryGetProperty("input_items", out _), Is.False);

        DecisionCreationOptions roundTrip = ModelReaderWriter.Read<DecisionCreationOptions>(ModelReaderWriter.Write(options));
        DecisionsInputImageContentPart part = (DecisionsInputImageContentPart)((DecisionsMessageItem)roundTrip.InputItems.Single()).Content.Single();
        Assert.That(part.ImageUri, Is.EqualTo("https://example.invalid/image.png"));
        Assert.That(part.ImageDetailLevel, Is.EqualTo(DecisionsImageDetailLevel.High));
    }

    private static IEnumerable<TestCaseData> ProtocolModels()
    {
        yield return Model<DecisionCreationOptions>("""{"input":[],"model":"test-model","questions":[],"safety_identifier":"user"}""");
        yield return Model<DecisionResource>("""{"answers":[],"model":"test-model","usage":{"input_tokens":1,"output_tokens":2,"total_tokens":3,"input_tokens_details":{"cache_write_tokens":0,"cached_tokens":1},"output_tokens_details":{"reasoning_tokens":2}}}""");
        yield return Model<DecisionsMessageItem>("""{"type":"message","content":[{"type":"input_text","text":"hello"}]}""");
        yield return Model<DecisionsInputTextContentPart>("""{"type":"input_text","text":"hello"}""");
        yield return Model<DecisionsInputImageContentPart>("""{"type":"input_image","image_url":"https://example.invalid/image.png","detail":"original"}""");
        yield return Model<DecisionsChoice>("""{"value":false,"description":"negative"}""");
        yield return Model<DecisionsChoiceQuestion>("""{"type":"choice","instructions":"choose","name":"choice","choices":[{"value":"yes","description":"positive"},{"value":false}]}""");
        yield return Model<DecisionsPredicateQuestion>("""{"type":"predicate","instructions":"evaluate","name":"predicate"}""");
        yield return Model<DecisionsScoreQuestion>("""{"type":"score","instructions":"score","name":"score","levels":[{"label":"low","description":"low score"}]}""");
        yield return Model<DecisionsScoreLevel>("""{"label":"high","description":"high score"}""");
        yield return Model<DecisionsChoiceAnswer>("""{"type":"choice","choice":false,"confidence":0.75,"name":"choice","probabilities":[{"value":false,"probability":0.75},{"value":"yes","probability":0.25}]}""");
        yield return Model<DecisionsPredicateAnswer>("""{"type":"predicate","name":"predicate","probability":0.5}""");
        yield return Model<DecisionsScoreAnswer>("""{"type":"score","name":"score","score":1.5,"confidence":0.75,"probabilities":[{"label":"high","value":2,"probability":0.75}]}""");
        yield return Model<DecisionsRefusalAnswer>("""{"type":"refusal","name":"refusal"}""");
        yield return Model<DecisionsChoiceProbability>("""{"value":"","probability":0.25}""");
        yield return Model<DecisionsScoreProbability>("""{"label":"low","value":0,"probability":0.25}""");
        yield return Model<DecisionsUsage>("""{"input_tokens":3,"output_tokens":4,"total_tokens":7,"input_tokens_details":{"cache_write_tokens":1,"cached_tokens":2},"output_tokens_details":{"reasoning_tokens":3}}""");
        yield return Model<DecisionsInputTokenUsageDetails>("""{"cache_write_tokens":1,"cached_tokens":2}""");
        yield return Model<DecisionsOutputTokenUsageDetails>("""{"reasoning_tokens":3}""");
    }

    private static TestCaseData Model<T>(string json) => new TestCaseData(typeof(T), json).SetName($"{{m}}({typeof(T).Name})");

    [TestCaseSource(nameof(ProtocolModels))]
    public void ProtocolModelsHaveDefaultConstructorsMutablePropertiesAndJsonPatch(Type modelType, string json)
    {
        object empty = Activator.CreateInstance(modelType);
        Assert.That(empty, Is.Not.Null);
        PropertyInfo patchProperty = modelType.GetProperty("Patch");
        Assert.That(patchProperty, Is.Not.Null);
        Assert.That(patchProperty.PropertyType, Is.EqualTo(typeof(JsonPatch).MakeByRefType()));

        foreach (PropertyInfo property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.Name is "Kind" or "Patch")
            {
                Assert.That(property.SetMethod?.IsPublic ?? false, Is.False, property.Name);
            }
            else if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                Assert.That(property.GetValue(empty), Is.Not.Null, property.Name);
                Assert.That(property.SetMethod?.IsPublic ?? false, Is.False, property.Name);
            }
            else
            {
                Assert.That(property.SetMethod?.IsPublic, Is.True, property.Name);
            }
        }

        object populated = ModelReaderWriter.Read(BinaryData.FromString(json), modelType);
        foreach (PropertyInfo property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.SetMethod?.IsPublic == true))
        {
            property.SetValue(empty, property.GetValue(populated));
            Assert.That(property.GetValue(empty), Is.EqualTo(property.GetValue(populated)), property.Name);
        }
    }

    [TestCaseSource(nameof(ProtocolModels))]
    public void ProtocolModelsRoundTripUnknownPropertiesAndSupportJsonPatch(Type modelType, string json)
    {
        string extendedJson = """{"future_property":{"nested":[true,"value",7]},""" + json.Substring(1);
        object model = ModelReaderWriter.Read(BinaryData.FromString(extendedJson), modelType);
        BinaryData serialized = ModelReaderWriter.Write(model);
        using JsonDocument expected = JsonDocument.Parse(extendedJson);
        using JsonDocument actual = JsonDocument.Parse(serialized);
        AssertJsonContains(expected.RootElement, actual.RootElement);

        object roundTrip = ModelReaderWriter.Read(serialized, modelType);
        using JsonDocument reserialized = JsonDocument.Parse(ModelReaderWriter.Write(roundTrip));
        AssertJsonContains(actual.RootElement, reserialized.RootElement);
        GetPatch(model).Set("$.patched_property"u8, 42);

        using JsonDocument patched = JsonDocument.Parse(ModelReaderWriter.Write(model));
        AssertJsonContains(expected.RootElement, patched.RootElement);
        Assert.That(patched.RootElement.GetProperty("patched_property").GetInt32(), Is.EqualTo(42));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void CreationOptionsConvertImplicitlyToBinaryContent(bool fromJson)
    {
        DecisionCreationOptions options = new()
        {
            Model = "test",
            InputItems = { new DecisionsMessageItem { Content = { new DecisionsInputTextContentPart { Text = "hello" } } } },
            Questions = { new DecisionsPredicateQuestion { Instructions = "evaluate" } }
        };
        if (fromJson)
        {
            options = Read<DecisionCreationOptions>("""
                {"model":"test","input":"hello","questions":[{"type":"predicate","instructions":"evaluate"}],"future_received":{"enabled":true}}
                """);
        }
        options.Patch.Set("$.future_property"u8, true);
        using BinaryContent content = options;
        using MemoryStream stream = new();
        content.WriteTo(stream);
        using JsonDocument expected = JsonDocument.Parse(ModelReaderWriter.Write(options));
        using JsonDocument actual = JsonDocument.Parse(stream.ToArray());
        AssertJsonContains(expected.RootElement, actual.RootElement);
        Assert.That(actual.RootElement.GetProperty("future_property").GetBoolean(), Is.True);
        if (fromJson)
        {
            Assert.That(actual.RootElement.GetProperty("future_received").GetProperty("enabled").GetBoolean(), Is.True);
        }
        DecisionCreationOptions nullOptions = null;
        BinaryContent nullContent = nullOptions;
        Assert.That(nullContent, Is.Null);
    }

    [TestCase("")]
    [TestCase("hello")]
    [TestCase("quotes \" and newline\n and Unicode \u263a")]
    public void InputShorthandNormalizesToLonghand(string text)
    {
        string encodedText = JsonSerializer.Serialize(text);
        DecisionCreationOptions shorthand = Read<DecisionCreationOptions>($$"""{"input":{{encodedText}},"model":"test","questions":[]}""");
        DecisionCreationOptions longhand = Read<DecisionCreationOptions>($$"""{"input":[{"type":"message","role":"user","content":[{"type":"input_text","text":{{encodedText}}}]}],"model":"test","questions":[]}""");

        Assert.That(shorthand.InputItems, Has.Count.EqualTo(1));
        DecisionsMessageItem message = shorthand.InputItems.Single() as DecisionsMessageItem;
        Assert.That(message, Is.Not.Null);
        Assert.That(message.Kind, Is.EqualTo(DecisionsItemKind.Message));
        Assert.That(message.Content, Has.Count.EqualTo(1));
        Assert.That(((DecisionsInputTextContentPart)message.Content.Single()).Text, Is.EqualTo(text));

        using JsonDocument normalized = JsonDocument.Parse(ModelReaderWriter.Write(shorthand));
        using JsonDocument expanded = JsonDocument.Parse(ModelReaderWriter.Write(longhand));
        AssertJsonContains(expanded.RootElement, normalized.RootElement);
        Assert.That(normalized.RootElement.GetProperty("input").ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(normalized.RootElement.GetProperty("input")[0].GetProperty("content").ValueKind, Is.EqualTo(JsonValueKind.Array));
    }

    [TestCase("")]
    [TestCase("hello")]
    public void MessageContentShorthandNormalizesToLonghand(string text)
    {
        string encodedText = JsonSerializer.Serialize(text);
        DecisionsMessageItem shorthand = Read<DecisionsMessageItem>($$"""{"type":"message","content":{{encodedText}}}""");
        DecisionsMessageItem longhand = Read<DecisionsMessageItem>($$"""{"type":"message","content":[{"type":"input_text","text":{{encodedText}}}]}""");
        Assert.That(shorthand.Content, Has.Count.EqualTo(1));
        Assert.That(shorthand.Content[0], Is.TypeOf<DecisionsInputTextContentPart>());
        Assert.That(((DecisionsInputTextContentPart)shorthand.Content[0]).Text, Is.EqualTo(text));
        using JsonDocument normalized = JsonDocument.Parse(ModelReaderWriter.Write(shorthand));
        using JsonDocument expanded = JsonDocument.Parse(ModelReaderWriter.Write(longhand));
        AssertJsonContains(expanded.RootElement, normalized.RootElement);
        Assert.That(normalized.RootElement.GetProperty("content").ValueKind, Is.EqualTo(JsonValueKind.Array));
    }

    [Test]
    public void LonghandInputPreservesMultipleItemsContentAndUnknownFields()
    {
        const string json = """
            {"input":[
                {"type":"message","content":[
                    {"type":"input_text","text":"describe","future_text":true},
                    {"type":"input_image","image_url":"https://example.invalid/image.png","detail":"high"}
                ],"future_message":42},
                {"type":"message","content":"another message"}
            ],"model":"test","questions":[]}
            """;
        DecisionCreationOptions options = Read<DecisionCreationOptions>(json);
        Assert.That(options.InputItems, Has.Count.EqualTo(2));
        DecisionsMessageItem message = (DecisionsMessageItem)options.InputItems[0];
        Assert.That(message.Content, Has.Count.EqualTo(2));
        Assert.That(((DecisionsInputImageContentPart)message.Content[1]).ImageDetailLevel, Is.EqualTo(DecisionsImageDetailLevel.High));
        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(options));
        JsonElement firstMessage = serialized.RootElement.GetProperty("input")[0];
        Assert.That(firstMessage.GetProperty("future_message").GetInt32(), Is.EqualTo(42));
        Assert.That(firstMessage.GetProperty("content")[0].GetProperty("future_text").GetBoolean(), Is.True);
        Assert.That(serialized.RootElement.GetProperty("input")[1].GetProperty("content")[0].GetProperty("text").GetString(), Is.EqualTo("another message"));
    }

    [TestCase("null")]
    [TestCase("true")]
    [TestCase("false")]
    [TestCase("42")]
    [TestCase("{}")]
    public void RequiredShorthandPropertiesRejectUnsupportedShapes(string value)
    {
        Assert.Throws<JsonException>(() => Read<DecisionCreationOptions>($$"""{"input":{{value}},"model":"test","questions":[]}"""));
        Assert.Throws<JsonException>(() => Read<DecisionsMessageItem>($$"""{"type":"message","content":{{value}}}"""));
    }

    [Test]
    public void EmptyLonghandCollectionsRemainEmpty()
    {
        Assert.That(Read<DecisionCreationOptions>("""{"input":[],"model":"test","questions":[]}""").InputItems, Is.Empty);
        Assert.That(Read<DecisionsMessageItem>("""{"type":"message","content":[]}""").Content, Is.Empty);
    }

    [TestCase("\"yes\"")]
    [TestCase("\"\"")]
    [TestCase("\"quoted \\\"value\\\"\"")]
    [TestCase("true")]
    [TestCase("false")]
    public void ChoiceValueSupportsEveryScalarShape(string json)
    {
        using JsonDocument expected = JsonDocument.Parse(json);
        DecisionsChoiceValue deserialized = Read<DecisionsChoiceValue>(json);
        DecisionsChoiceValue constructed = expected.RootElement.ValueKind == JsonValueKind.String
            ? new DecisionsChoiceValue(expected.RootElement.GetString())
            : new DecisionsChoiceValue(expected.RootElement.GetBoolean());
        DecisionsChoiceValue converted = expected.RootElement.ValueKind == JsonValueKind.String
            ? (DecisionsChoiceValue)expected.RootElement.GetString()
            : expected.RootElement.GetBoolean();

        foreach (DecisionsChoiceValue value in new[] { deserialized, constructed, converted })
        {
            Assert.That((value.StringValue is not null) ^ value.BooleanValue.HasValue, Is.True);
            if (expected.RootElement.ValueKind == JsonValueKind.String)
            {
                Assert.That(value.StringValue, Is.EqualTo(expected.RootElement.GetString()));
                Assert.That(value.BooleanValue, Is.Null);
            }
            else
            {
                Assert.That(value.BooleanValue, Is.EqualTo(expected.RootElement.GetBoolean()));
                Assert.That(value.StringValue, Is.Null);
            }
            using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(value));
            AssertJsonContains(expected.RootElement, serialized.RootElement);
            DecisionsChoiceValue roundTrip = Read<DecisionsChoiceValue>(serialized.RootElement.GetRawText());
            Assert.That(roundTrip.StringValue, Is.EqualTo(value.StringValue));
            Assert.That(roundTrip.BooleanValue, Is.EqualTo(value.BooleanValue));
        }
    }

    [Test]
    public void ChoiceValueHasReadOnlyComponentsAndNoPublicSyntheticPatch()
    {
        Assert.That(typeof(DecisionsChoiceValue).GetProperty(nameof(DecisionsChoiceValue.StringValue)).SetMethod, Is.Null);
        Assert.That(typeof(DecisionsChoiceValue).GetProperty(nameof(DecisionsChoiceValue.BooleanValue)).SetMethod, Is.Null);
        Assert.That(typeof(DecisionsChoiceValue).GetProperty("Patch"), Is.Null);
        Assert.That(typeof(DecisionsChoiceValue).GetConstructor(Type.EmptyTypes), Is.Null);
        Assert.Throws<ArgumentNullException>(() => new DecisionsChoiceValue((string)null));
        string nullableString = null;
        DecisionsChoiceValue value = nullableString;
        Assert.That(value, Is.Null);
        Assert.That(Read<DecisionsChoiceValue>("null"), Is.Null);
    }

    [TestCase("\"yes\"")]
    [TestCase("\"\"")]
    [TestCase("true")]
    [TestCase("false")]
    [TestCase("null")]
    public void ChoiceValuePropertiesRoundTripSupportedShapes(string scalar)
    {
        DecisionsChoice choice = Read<DecisionsChoice>($$"""{"value":{{scalar}}}""");
        DecisionsChoiceAnswer answer = Read<DecisionsChoiceAnswer>($$"""{"type":"choice","choice":{{scalar}},"confidence":0.5,"name":"test","probabilities":[]}""");
        DecisionsChoiceProbability probability = Read<DecisionsChoiceProbability>($$"""{"value":{{scalar}},"probability":0.5}""");
        using JsonDocument expected = JsonDocument.Parse(scalar);
        using JsonDocument choiceJson = JsonDocument.Parse(ModelReaderWriter.Write(choice));
        using JsonDocument answerJson = JsonDocument.Parse(ModelReaderWriter.Write(answer));
        using JsonDocument probabilityJson = JsonDocument.Parse(ModelReaderWriter.Write(probability));
        AssertJsonContains(expected.RootElement, choiceJson.RootElement.GetProperty("value"));
        AssertJsonContains(expected.RootElement, answerJson.RootElement.GetProperty("choice"));
        AssertJsonContains(expected.RootElement, probabilityJson.RootElement.GetProperty("value"));
        if (scalar == "null")
        {
            Assert.That(choice.Value, Is.Null);
            Assert.That(answer.Choice, Is.Null);
            Assert.That(probability.Value, Is.Null);
        }
        else
        {
            Assert.That(choice.Value.StringValue, Is.EqualTo(answer.Choice.StringValue));
            Assert.That(choice.Value.BooleanValue, Is.EqualTo(probability.Value.BooleanValue));
        }
    }

    [TestCase("42")]
    [TestCase("0")]
    [TestCase("1.25")]
    [TestCase("{}")]
    [TestCase("[]")]
    [TestCase("""{"string_value":"yes"}""")]
    public void ChoiceValueRejectsUnsupportedShapesDirectlyAndInProperties(string value)
    {
        Assert.Throws<JsonException>(() => Read<DecisionsChoiceValue>(value));
        Assert.Throws<JsonException>(() => Read<DecisionsChoice>($$"""{"value":{{value}}}"""));
        Assert.Throws<JsonException>(() => Read<DecisionsChoiceAnswer>($$"""{"type":"choice","choice":{{value}}}"""));
        Assert.Throws<JsonException>(() => Read<DecisionsChoiceProbability>($$"""{"value":{{value}}}"""));
    }

    [TestCase(false, "\"replacement\"")]
    [TestCase(false, "false")]
    [TestCase(false, "null")]
    [TestCase(true, "\"replacement\"")]
    [TestCase(true, "false")]
    [TestCase(true, "null")]
    public void ContainingModelsCanReplaceEntireChoiceValue(bool fromJson, string replacement)
    {
        DecisionsChoice choice = fromJson
            ? Read<DecisionsChoice>("""{"value":"original"}""")
            : new DecisionsChoice { Value = "original" };
        DecisionsChoiceAnswer answer = fromJson
            ? Read<DecisionsChoiceAnswer>("""{"type":"choice","choice":true,"name":"test","confidence":1,"probabilities":[]}""")
            : new DecisionsChoiceAnswer { Choice = true };
        DecisionsChoiceProbability probability = fromJson
            ? Read<DecisionsChoiceProbability>("""{"value":"original","probability":1}""")
            : new DecisionsChoiceProbability { Value = "original" };
        byte[] rawValue = Encoding.UTF8.GetBytes(replacement);
        choice.Patch.Set("$.value"u8, rawValue.AsSpan());
        answer.Patch.Set("$.choice"u8, rawValue.AsSpan());
        probability.Patch.Set("$.value"u8, rawValue.AsSpan());

        using JsonDocument expected = JsonDocument.Parse(replacement);
        using JsonDocument choiceJson = JsonDocument.Parse(ModelReaderWriter.Write(choice));
        using JsonDocument answerJson = JsonDocument.Parse(ModelReaderWriter.Write(answer));
        using JsonDocument probabilityJson = JsonDocument.Parse(ModelReaderWriter.Write(probability));
        AssertJsonContains(expected.RootElement, choiceJson.RootElement.GetProperty("value"));
        AssertJsonContains(expected.RootElement, answerJson.RootElement.GetProperty("choice"));
        AssertJsonContains(expected.RootElement, probabilityJson.RootElement.GetProperty("value"));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void JsonPatchUpdatesNestedModelsAndPropagatesThroughModelProperties(bool fromJson)
    {
        DecisionCreationOptions options = new()
        {
            Model = "test",
            InputItems = { new DecisionsMessageItem { Content = { new DecisionsInputTextContentPart { Text = "original" } } } },
            Questions = { new DecisionsChoiceQuestion { Instructions = "choose", Choices = { new DecisionsChoice { Value = true } } } }
        };
        if (fromJson)
        {
            options = ModelReaderWriter.Read<DecisionCreationOptions>(ModelReaderWriter.Write(options));
        }
        ((DecisionsMessageItem)options.InputItems[0]).Content[0].Patch.Set("$.text"u8, "patched text");
        DecisionsChoiceQuestion question = (DecisionsChoiceQuestion)options.Questions[0];
        question.Choices[0].Patch.Set("$.value"u8, "\"patched choice\""u8);
        question.Patch.Set("$.future_question"u8, "preserved");
        using JsonDocument json = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(json.RootElement.GetProperty("input")[0].GetProperty("content")[0].GetProperty("text").GetString(), Is.EqualTo("patched text"));
        Assert.That(json.RootElement.GetProperty("questions")[0].GetProperty("choices")[0].GetProperty("value").GetString(), Is.EqualTo("patched choice"));
        Assert.That(json.RootElement.GetProperty("questions")[0].GetProperty("future_question").GetString(), Is.EqualTo("preserved"));

        DecisionResource resource = new()
        {
            Answers = { new DecisionsChoiceAnswer { Choice = true, Probabilities = { new DecisionsChoiceProbability { Value = "original" } } } },
            Usage = new DecisionsUsage { InputTokenDetails = new DecisionsInputTokenUsageDetails() }
        };
        if (fromJson)
        {
            resource = ModelReaderWriter.Read<DecisionResource>(ModelReaderWriter.Write(resource));
        }
        DecisionsChoiceAnswer answer = (DecisionsChoiceAnswer)resource.Answers[0];
        answer.Patch.Set("$.choice"u8, "false"u8);
        answer.Probabilities[0].Patch.Set("$.value"u8, "\"replacement\""u8);
        resource.Patch.Set("$.usage.input_tokens_details.cached_tokens"u8, 17);
        using JsonDocument resultJson = JsonDocument.Parse(ModelReaderWriter.Write(resource));
        Assert.That(resultJson.RootElement.GetProperty("answers")[0].GetProperty("choice").GetBoolean(), Is.False);
        Assert.That(resultJson.RootElement.GetProperty("answers")[0].GetProperty("probabilities")[0].GetProperty("value").GetString(), Is.EqualTo("replacement"));
        Assert.That(resultJson.RootElement.GetProperty("usage").GetProperty("input_tokens_details").GetProperty("cached_tokens").GetInt32(), Is.EqualTo(17));
    }

    [Test]
    public void JsonPatchCanOverrideNormalizedShorthandWireRepresentation()
    {
        DecisionCreationOptions options = Read<DecisionCreationOptions>("""{"input":"original","model":"test","questions":[]}""");
        options.Patch.Set("$.input"u8, "\"raw shorthand\""u8);
        using JsonDocument optionsJson = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(optionsJson.RootElement.GetProperty("input").GetString(), Is.EqualTo("raw shorthand"));
        DecisionsMessageItem message = Read<DecisionsMessageItem>("""{"type":"message","content":"original"}""");
        message.Patch.Set("$.content"u8, "\"raw content\""u8);
        using JsonDocument messageJson = JsonDocument.Parse(ModelReaderWriter.Write(message));
        Assert.That(messageJson.RootElement.GetProperty("content").GetString(), Is.EqualTo("raw content"));
    }

    [TestCase(typeof(DecisionsItem))]
    [TestCase(typeof(DecisionsContentPart))]
    [TestCase(typeof(DecisionsQuestion))]
    [TestCase(typeof(DecisionsAnswer))]
    public void UnknownDiscriminatorsRoundTripWithKindAndAdditionalProperties(Type modelType)
    {
        const string json = """{"type":"future_kind","future_data":{"values":[false,1,"text"]}}""";
        object model = ModelReaderWriter.Read(BinaryData.FromString(json), modelType);
        Assert.That(modelType.GetProperty("Kind").GetValue(model).ToString(), Is.EqualTo("future_kind"));
        using JsonDocument expected = JsonDocument.Parse(json);
        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(model));
        AssertJsonContains(expected.RootElement, serialized.RootElement);
        object roundTrip = ModelReaderWriter.Read(ModelReaderWriter.Write(model), modelType);
        Assert.That(modelType.GetProperty("Kind").GetValue(roundTrip).ToString(), Is.EqualTo("future_kind"));
    }

    [Test]
    public void ExternalSubclassesCanSpecifyExtensibleKinds()
    {
        DecisionsItem item = new ExternalItem();
        DecisionsContentPart content = new ExternalContentPart();
        DecisionsQuestion question = new ExternalQuestion();
        DecisionsAnswer answer = new ExternalAnswer();
        Assert.That(item.Kind, Is.EqualTo(new DecisionsItemKind("external_item")));
        Assert.That(content.Kind, Is.EqualTo(new DecisionsContentPartKind("external_content")));
        Assert.That(question.Kind, Is.EqualTo(new DecisionsQuestionKind("external_question")));
        Assert.That(answer.Kind, Is.EqualTo(new DecisionsAnswerKind("external_answer")));
        AssertSerializedKind(item, "external_item");
        AssertSerializedKind(content, "external_content");
        AssertSerializedKind(question, "external_question");
        AssertSerializedKind(answer, "external_answer");
    }

    [TestCase(typeof(DecisionsItem), typeof(DecisionsItemKind))]
    [TestCase(typeof(DecisionsContentPart), typeof(DecisionsContentPartKind))]
    [TestCase(typeof(DecisionsQuestion), typeof(DecisionsQuestionKind))]
    [TestCase(typeof(DecisionsAnswer), typeof(DecisionsAnswerKind))]
    public void DiscriminatorBasesExposeReadOnlyKindsAndProtectedInternalConstructors(Type modelType, Type kindType)
    {
        PropertyInfo kind = modelType.GetProperty("Kind");
        Assert.That(kind.GetMethod.IsPublic, Is.True);
        Assert.That(kind.SetMethod, Is.Null);
        ConstructorInfo constructor = modelType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, [kindType], null);
        Assert.That(constructor, Is.Not.Null);
        Assert.That(constructor.IsFamilyOrAssembly, Is.True);
        Assert.That(kindType.IsPublic, Is.True);
    }

    [Test]
    public void KnownDiscriminatorsDeserializeToConcreteModels()
    {
        Assert.That(Read<DecisionsItem>("""{"type":"message","content":[]}"""), Is.TypeOf<DecisionsMessageItem>());
        Assert.That(Read<DecisionsContentPart>("""{"type":"input_text","text":"hello"}"""), Is.TypeOf<DecisionsInputTextContentPart>());
        Assert.That(Read<DecisionsContentPart>("""{"type":"input_image","image_url":"https://example.invalid/image.png"}"""), Is.TypeOf<DecisionsInputImageContentPart>());
        Assert.That(Read<DecisionsQuestion>("""{"type":"choice","instructions":"choose","choices":[]}"""), Is.TypeOf<DecisionsChoiceQuestion>());
        Assert.That(Read<DecisionsQuestion>("""{"type":"predicate","instructions":"evaluate"}"""), Is.TypeOf<DecisionsPredicateQuestion>());
        Assert.That(Read<DecisionsQuestion>("""{"type":"score","instructions":"score","levels":[]}"""), Is.TypeOf<DecisionsScoreQuestion>());
        Assert.That(Read<DecisionsAnswer>("""{"type":"choice","choice":true,"confidence":1,"name":"test","probabilities":[]}"""), Is.TypeOf<DecisionsChoiceAnswer>());
        Assert.That(Read<DecisionsAnswer>("""{"type":"predicate","name":"test","probability":0.5}"""), Is.TypeOf<DecisionsPredicateAnswer>());
        Assert.That(Read<DecisionsAnswer>("""{"type":"score","name":"test","score":1,"confidence":1,"probabilities":[]}"""), Is.TypeOf<DecisionsScoreAnswer>());
        Assert.That(Read<DecisionsAnswer>("""{"type":"refusal","name":"test"}"""), Is.TypeOf<DecisionsRefusalAnswer>());
    }

    private static T Read<T>(string json) => ModelReaderWriter.Read<T>(BinaryData.FromString(json));

    private static ref JsonPatch GetPatch(object model)
    {
        switch (model)
        {
            case DecisionCreationOptions value: return ref value.Patch;
            case DecisionResource value: return ref value.Patch;
            case DecisionsItem value: return ref value.Patch;
            case DecisionsContentPart value: return ref value.Patch;
            case DecisionsQuestion value: return ref value.Patch;
            case DecisionsAnswer value: return ref value.Patch;
            case DecisionsChoice value: return ref value.Patch;
            case DecisionsScoreLevel value: return ref value.Patch;
            case DecisionsChoiceProbability value: return ref value.Patch;
            case DecisionsScoreProbability value: return ref value.Patch;
            case DecisionsUsage value: return ref value.Patch;
            case DecisionsInputTokenUsageDetails value: return ref value.Patch;
            case DecisionsOutputTokenUsageDetails value: return ref value.Patch;
            default: throw new ArgumentException("Not a Decisions protocol model.", nameof(model));
        }
    }

    private static void AssertSerializedKind<T>(T model, string kind)
    {
        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(model));
        Assert.That(serialized.RootElement.GetProperty("type").GetString(), Is.EqualTo(kind));
    }

    private static void AssertJsonContains(JsonElement expected, JsonElement actual)
    {
        Assert.That(actual.ValueKind, Is.EqualTo(expected.ValueKind));
        switch (expected.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (JsonProperty property in expected.EnumerateObject())
                {
                    Assert.That(actual.TryGetProperty(property.Name, out JsonElement value), Is.True, property.Name);
                    AssertJsonContains(property.Value, value);
                }
                break;
            case JsonValueKind.Array:
                Assert.That(actual.GetArrayLength(), Is.EqualTo(expected.GetArrayLength()));
                for (int i = 0; i < expected.GetArrayLength(); i++)
                {
                    AssertJsonContains(expected[i], actual[i]);
                }
                break;
            case JsonValueKind.String:
                Assert.That(actual.GetString(), Is.EqualTo(expected.GetString()));
                break;
            case JsonValueKind.Number:
                Assert.That(actual.GetDouble(), Is.EqualTo(expected.GetDouble()));
                break;
        }
    }

    private sealed class ExternalItem() : DecisionsItem(new DecisionsItemKind("external_item")) { }
    private sealed class ExternalContentPart() : DecisionsContentPart(new DecisionsContentPartKind("external_content")) { }
    private sealed class ExternalQuestion() : DecisionsQuestion(new DecisionsQuestionKind("external_question")) { }
    private sealed class ExternalAnswer() : DecisionsAnswer(new DecisionsAnswerKind("external_answer")) { }
}
