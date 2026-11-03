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
    // Verifies external subclasses can serialize custom kinds and apply property, removal, and root patches.
    [TestCase("item", "J")]
    [TestCase("item", "W")]
    [TestCase("content", "J")]
    [TestCase("content", "W")]
    [TestCase("assistant-content", "J")]
    [TestCase("assistant-content", "W")]
    [TestCase("instruction-content", "J")]
    [TestCase("instruction-content", "W")]
    [TestCase("instruction-message-content", "J")]
    [TestCase("instruction-message-content", "W")]
    [TestCase("tool-output-content", "J")]
    [TestCase("tool-output-content", "W")]
    [TestCase("question", "J")]
    [TestCase("question", "W")]
    [TestCase("answer", "J")]
    [TestCase("answer", "W")]
    public void ExternalSubclassesCanImplementPatchSerialization(string hierarchy, string format)
    {
        object model = hierarchy switch
        {
            "item" => new CustomItem(),
            "content" => new CustomContentPart(),
            "assistant-content" => new CustomAssistantContentPart(),
            "instruction-content" => new CustomInstructionContentPart(),
            "instruction-message-content" => new CustomInstructionMessageContentPart(),
            "tool-output-content" => new CustomToolOutputContentPart(),
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
        yield return new TestCaseData(new DecisionsUserMessageItem()).SetName("{m}(UserMessage)");
        yield return new TestCaseData(new DecisionsAssistantMessageItem()).SetName("{m}(AssistantMessage)");
        yield return new TestCaseData(new DecisionsDeveloperMessageItem()).SetName("{m}(DeveloperMessage)");
        yield return new TestCaseData(new DecisionsSystemMessageItem()).SetName("{m}(SystemMessage)");
        yield return new TestCaseData(new DecisionsFunctionCallItem()).SetName("{m}(FunctionCall)");
        yield return new TestCaseData(new DecisionsFunctionCallOutputItem()).SetName("{m}(FunctionCallOutput)");
        yield return new TestCaseData(new DecisionsCustomToolCallItem()).SetName("{m}(CustomToolCall)");
        yield return new TestCaseData(new DecisionsCustomToolCallOutputItem()).SetName("{m}(CustomToolCallOutput)");
        yield return new TestCaseData(new DecisionsContentInputTextPart()).SetName("{m}(InputText)");
        yield return new TestCaseData(new DecisionsContentInputImagePart()).SetName("{m}(InputImage)");
        yield return new TestCaseData(new DecisionsContentInputJsonPart()).SetName("{m}(InputJson)");
        yield return new TestCaseData(new DecisionsContentOutputTextPart()).SetName("{m}(OutputText)");
        yield return new TestCaseData(new DecisionsContentRefusalPart()).SetName("{m}(RefusalContent)");
        yield return new TestCaseData(new DecisionsInstructionsInputTextPart()).SetName("{m}(InstructionText)");
        yield return new TestCaseData(new DecisionsInstructionsInputImagePart()).SetName("{m}(InstructionImage)");
        yield return new TestCaseData(new DecisionsInstructionsInputJsonPart()).SetName("{m}(InstructionJson)");
        yield return new TestCaseData(new DecisionsInstructionMessageTextContentPart()).SetName("{m}(InstructionMessageText)");
        yield return new TestCaseData(new DecisionsToolOutputTextContentPart()).SetName("{m}(ToolOutputText)");
        yield return new TestCaseData(new DecisionsToolOutputImageContentPart()).SetName("{m}(ToolOutputImage)");
        yield return new TestCaseData(new DecisionsChoiceQuestion()).SetName("{m}(ChoiceQuestion)");
        yield return new TestCaseData(new DecisionsPredicateQuestion()).SetName("{m}(PredicateQuestion)");
        yield return new TestCaseData(new DecisionsScoreQuestion()).SetName("{m}(ScoreQuestion)");
        yield return new TestCaseData(new DecisionsChoiceAnswer()).SetName("{m}(ChoiceAnswer)");
        yield return new TestCaseData(new DecisionsPredicateAnswer()).SetName("{m}(PredicateAnswer)");
        yield return new TestCaseData(new DecisionsRefusalAnswer()).SetName("{m}(RefusalAnswer)");
        yield return new TestCaseData(new DecisionsScoreAnswer()).SetName("{m}(ScoreAnswer)");
        BinaryData unknown = BinaryData.FromString("""{"type":"future","received":true}""");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsItem>(unknown)).SetName("{m}(UnknownItem)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsContentInputPart>(unknown)).SetName("{m}(UnknownContent)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsContentOutputPart>(unknown)).SetName("{m}(UnknownAssistantContent)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsInstructionsPart>(unknown)).SetName("{m}(UnknownInstructionContent)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsInstructionMessageContentPart>(unknown)).SetName("{m}(UnknownInstructionMessageContent)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsToolOutputContentPart>(unknown)).SetName("{m}(UnknownToolOutputContent)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsItem>(BinaryData.FromString("""{"type":"message","role":"future","received":true}"""))).SetName("{m}(UnknownMessage)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsQuestion>(unknown)).SetName("{m}(UnknownQuestion)");
        yield return new TestCaseData(ModelReaderWriter.Read<DecisionsAnswer>(unknown)).SetName("{m}(UnknownAnswer)");
    }

    // Verifies generated and unknown derived models emit each patched property once in JSON and wire formats.
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
                case DecisionsContentInputPart part:
                    ((IJsonModel<DecisionsContentInputPart>)part).Write(writer, options);
                    break;
                case DecisionsContentOutputPart part:
                    ((IJsonModel<DecisionsContentOutputPart>)part).Write(writer, options);
                    break;
                case DecisionsInstructionsPart part:
                    ((IJsonModel<DecisionsInstructionsPart>)part).Write(writer, options);
                    break;
                case DecisionsInstructionMessageContentPart part:
                    ((IJsonModel<DecisionsInstructionMessageContentPart>)part).Write(writer, options);
                    break;
                case DecisionsToolOutputContentPart part:
                    ((IJsonModel<DecisionsToolOutputContentPart>)part).Write(writer, options);
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
            case DecisionsContentInputPart part: return ref part.Patch;
            case DecisionsContentOutputPart part: return ref part.Patch;
            case DecisionsInstructionsPart part: return ref part.Patch;
            case DecisionsInstructionMessageContentPart part: return ref part.Patch;
            case DecisionsToolOutputContentPart part: return ref part.Patch;
            case DecisionsQuestion question: return ref question.Patch;
            case DecisionsAnswer answer: return ref answer.Patch;
            default: throw new ArgumentException("Expected a Decisions hierarchy model.", nameof(model));
        }
    }

    private sealed class CustomItem() : DecisionsItem(new DecisionsItemKind("custom"))
    {
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            base.JsonModelWriteCore(writer, options);
            Patch.WriteTo(writer);
        }
    }

    private sealed class CustomContentPart() : DecisionsContentInputPart(new DecisionsContentInputPartKind("custom"))
    {
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            base.JsonModelWriteCore(writer, options);
            Patch.WriteTo(writer);
        }
    }

    private sealed class CustomQuestion() : DecisionsQuestion(new DecisionsQuestionKind("custom"))
    {
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            base.JsonModelWriteCore(writer, options);
            Patch.WriteTo(writer);
        }
    }

    private sealed class CustomAssistantContentPart() : DecisionsContentOutputPart(new DecisionsContentOutputPartKind("custom"))
    {
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            base.JsonModelWriteCore(writer, options);
            Patch.WriteTo(writer);
        }
    }

    private sealed class CustomInstructionContentPart() : DecisionsInstructionsPart(new DecisionsInstructionsPartKind("custom"))
    {
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            base.JsonModelWriteCore(writer, options);
            Patch.WriteTo(writer);
        }
    }

    private sealed class CustomInstructionMessageContentPart() : DecisionsInstructionMessageContentPart(new DecisionsInstructionMessageContentPartKind("custom"))
    {
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            base.JsonModelWriteCore(writer, options);
            Patch.WriteTo(writer);
        }
    }

    private sealed class CustomToolOutputContentPart() : DecisionsToolOutputContentPart(new DecisionsToolOutputContentPartKind("custom"))
    {
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            base.JsonModelWriteCore(writer, options);
            Patch.WriteTo(writer);
        }
    }

    private sealed class CustomAnswer() : DecisionsAnswer(new DecisionsAnswerKind("custom"))
    {
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            base.JsonModelWriteCore(writer, options);
            Patch.WriteTo(writer);
        }
    }
}
