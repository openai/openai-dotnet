using NUnit.Framework;
using OpenAI.Decisions;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Tests.Decisions;

[Category("Decisions")]
[Category("Smoke")]
public class DecisionsExtensibilityTests
{
    [Test]
    public void DefaultMessageConstructorUsesUserRole()
    {
        DecisionsMessageItem message = new()
        {
            Content = { new DecisionsInputTextContentPart { Text = "hello" } }
        };

        AssertUserMessage(message);
    }

    [Test]
    public void ContentConstructorUsesUserRole()
    {
        DecisionsMessageItem message = new([new DecisionsInputTextContentPart { Text = "hello" }]);

        AssertUserMessage(message);
    }

    [Test]
    public void InputShorthandUsesUserRole()
    {
        DecisionCreationOptions options = ModelReaderWriter.Read<DecisionCreationOptions>(
            BinaryData.FromString("""{"input":"hello","model":"test","questions":[]}"""));

        AssertUserMessage((DecisionsMessageItem)options.Input.Single());
        using JsonDocument document = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(document.RootElement.GetProperty("input")[0].GetProperty("role").GetString(), Is.EqualTo("user"));
    }

    [Test]
    public void LonghandMessagePreservesUserRole()
    {
        DecisionsMessageItem message = ModelReaderWriter.Read<DecisionsMessageItem>(
            BinaryData.FromString("""{"type":"message","role":"user","content":[{"type":"input_text","text":"hello"}]}"""));

        AssertUserMessage(message);
    }

    [TestCase("item", "J")]
    [TestCase("item", "W")]
    [TestCase("content", "J")]
    [TestCase("content", "W")]
    [TestCase("question", "J")]
    [TestCase("question", "W")]
    [TestCase("answer", "J")]
    [TestCase("answer", "W")]
    public void ExternalSubclassesPreservePatches(string hierarchy, string format)
    {
        object model = hierarchy switch
        {
            "item" => new CustomItem(),
            "content" => new CustomContentPart(),
            "question" => new CustomQuestion(),
            "answer" => new CustomAnswer(),
            _ => throw new ArgumentOutOfRangeException(nameof(hierarchy))
        };
        ModelReaderWriterOptions options = new(format);
        using (JsonDocument original = SerializeBase(model, options))
        {
            Assert.That(original.RootElement.GetProperty("type").GetString(), Is.EqualTo("custom"));
        }

        GetPatch(model).Set("$.custom_property"u8, 42);
        GetPatch(model).Set("$.type"u8, "patched");
        using (JsonDocument patched = SerializeBase(model, options))
        {
            Assert.That(patched.RootElement.GetProperty("custom_property").GetInt32(), Is.EqualTo(42));
            Assert.That(patched.RootElement.GetProperty("type").GetString(), Is.EqualTo("patched"));
            AssertNoDuplicateProperties(patched.RootElement);
        }
        using (JsonDocument persisted = JsonDocument.Parse(ModelReaderWriter.Write(model, options)))
        {
            Assert.That(persisted.RootElement.GetProperty("custom_property").GetInt32(), Is.EqualTo(42));
            Assert.That(persisted.RootElement.GetProperty("type").GetString(), Is.EqualTo("patched"));
        }

        GetPatch(model).Remove("$.type"u8);
        using (JsonDocument removed = SerializeBase(model, options))
        {
            Assert.That(removed.RootElement.TryGetProperty("type", out _), Is.False);
            Assert.That(removed.RootElement.GetProperty("custom_property").GetInt32(), Is.EqualTo(42));
        }

        GetPatch(model).Set("$"u8, """{"replacement":true}"""u8);
        using JsonDocument replaced = SerializeBase(model, options);
        Assert.That(replaced.RootElement.EnumerateObject().Count(), Is.EqualTo(1));
        Assert.That(replaced.RootElement.GetProperty("replacement").GetBoolean(), Is.True);
    }

    private static IEnumerable<TestCaseData> GeneratedModels()
    {
        yield return new TestCaseData(new DecisionsMessageItem()).SetName("{m}(Message)");
        yield return new TestCaseData(new DecisionsInputTextContentPart()).SetName("{m}(InputText)");
        yield return new TestCaseData(new DecisionsInputImageContentPart()).SetName("{m}(InputImage)");
        yield return new TestCaseData(new DecisionsChoiceQuestion()).SetName("{m}(ChoiceQuestion)");
        yield return new TestCaseData(new DecisionsPredicateQuestion()).SetName("{m}(PredicateQuestion)");
        yield return new TestCaseData(new DecisionsScoreQuestion()).SetName("{m}(ScoreQuestion)");
        yield return new TestCaseData(new DecisionsChoiceAnswer()).SetName("{m}(ChoiceAnswer)");
        yield return new TestCaseData(new DecisionsPredicateAnswer()).SetName("{m}(PredicateAnswer)");
        yield return new TestCaseData(new DecisionsRefusalAnswer()).SetName("{m}(RefusalAnswer)");
        yield return new TestCaseData(new DecisionsScoreAnswer()).SetName("{m}(ScoreAnswer)");
        BinaryData unknown = BinaryData.FromString("""{"type":"future","received":true}""");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsItem>(unknown)).SetName("{m}(UnknownItem)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsContentPart>(unknown)).SetName("{m}(UnknownContent)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsQuestion>(unknown)).SetName("{m}(UnknownQuestion)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsAnswer>(unknown)).SetName("{m}(UnknownAnswer)");
    }

    [TestCaseSource(nameof(GeneratedModels))]
    public void GeneratedDerivedModelsDoNotWriteDuplicatePatchProperties(object model)
    {
        GetPatch(model).Set("$.custom_property"u8, 42);
        GetPatch(model).Set("$.type"u8, "patched");
        foreach (string format in new[] { "J", "W" })
        {
            ModelReaderWriterOptions options = new(format);
            using JsonDocument serialized = SerializeBase(model, options);
            AssertNoDuplicateProperties(serialized.RootElement);
            Assert.That(serialized.RootElement.GetProperty("custom_property").GetInt32(), Is.EqualTo(42));
            Assert.That(serialized.RootElement.GetProperty("type").GetString(), Is.EqualTo("patched"));
            using JsonDocument persisted = JsonDocument.Parse(ModelReaderWriter.Write(model, options));
            AssertNoDuplicateProperties(persisted.RootElement);
            Assert.That(persisted.RootElement.GetProperty("custom_property").GetInt32(), Is.EqualTo(42));
        }

        GetPatch(model).Remove("$.type"u8);
        using (JsonDocument removed = SerializeBase(model, ModelReaderWriterOptions.Json))
        {
            Assert.That(removed.RootElement.TryGetProperty("type", out _), Is.False);
        }
        GetPatch(model).Set("$"u8, """{"replacement":true}"""u8);
        using JsonDocument replaced = SerializeBase(model, ModelReaderWriterOptions.Json);
        Assert.That(replaced.RootElement.EnumerateObject().Count(), Is.EqualTo(1));
        Assert.That(replaced.RootElement.GetProperty("replacement").GetBoolean(), Is.True);
    }

    private static void AssertUserMessage(DecisionsMessageItem message)
    {
        using JsonDocument document = JsonDocument.Parse(ModelReaderWriter.Write(message));
        Assert.That(document.RootElement.GetProperty("role").GetString(), Is.EqualTo("user"));
        Assert.That(document.RootElement.GetProperty("type").GetString(), Is.EqualTo("message"));
        Assert.That(document.RootElement.GetProperty("content")[0].GetProperty("text").GetString(), Is.EqualTo("hello"));
    }

    private static void AssertNoDuplicateProperties(JsonElement element)
    {
        string[] names = element.EnumerateObject().Select(property => property.Name).ToArray();
        Assert.That(names, Is.Unique);
    }

    private static JsonDocument SerializeBase(object model, ModelReaderWriterOptions options)
    {
        using MemoryStream stream = new();
        using (Utf8JsonWriter writer = new(stream))
        {
            switch (model)
            {
                case DecisionsItem item:
                    ((IJsonModel<DecisionsItem>)item).Write(writer, options);
                    break;
                case DecisionsContentPart part:
                    ((IJsonModel<DecisionsContentPart>)part).Write(writer, options);
                    break;
                case DecisionsQuestion question:
                    ((IJsonModel<DecisionsQuestion>)question).Write(writer, options);
                    break;
                case DecisionsAnswer answer:
                    ((IJsonModel<DecisionsAnswer>)answer).Write(writer, options);
                    break;
                default:
                    throw new ArgumentException("Expected a Decisions hierarchy model.", nameof(model));
            }
        }
        return JsonDocument.Parse(stream.ToArray());
    }

    private static ref JsonPatch GetPatch(object model)
    {
        switch (model)
        {
            case DecisionsItem item: return ref item.Patch;
            case DecisionsContentPart part: return ref part.Patch;
            case DecisionsQuestion question: return ref question.Patch;
            case DecisionsAnswer answer: return ref answer.Patch;
            default: throw new ArgumentException("Expected a Decisions hierarchy model.", nameof(model));
        }
    }

    private sealed class CustomItem() : DecisionsItem(new DecisionsItemKind("custom"));
    private sealed class CustomContentPart() : DecisionsContentPart(new DecisionsContentPartKind("custom"));
    private sealed class CustomQuestion() : DecisionsQuestion(new DecisionsQuestionKind("custom"));
    private sealed class CustomAnswer() : DecisionsAnswer(new DecisionsAnswerKind("custom"));
}
