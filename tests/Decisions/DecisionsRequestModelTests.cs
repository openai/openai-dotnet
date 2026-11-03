using NUnit.Framework;
using OpenAI.Decisions;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace OpenAI.Tests.Decisions;

[Category("Decisions")]
[Category("Smoke")]
public class DecisionsRequestModelTests
{
    private static IEnumerable<TestCaseData> TextOrPartsModels()
    {
        yield return new(typeof(DecisionCreationOptions), "input", """{"input":VALUE,"model":"test","questions":[]}""", "input_text");
        yield return new(typeof(DecisionsUserMessageItem), "content", """{"type":"message","role":"user","content":VALUE}""", "input_text");
        yield return new(typeof(DecisionsAssistantMessageItem), "content", """{"type":"message","role":"assistant","content":VALUE}""", "output_text");
        yield return new(typeof(DecisionsDeveloperMessageItem), "content", """{"type":"message","role":"developer","content":VALUE}""", "input_text");
        yield return new(typeof(DecisionsSystemMessageItem), "content", """{"type":"message","role":"system","content":VALUE}""", "input_text");
        yield return new(typeof(DecisionsFunctionToolCallOutputItem), "output", """{"type":"function_call_output","call_id":"call_1","output":VALUE}""", "input_text");
        yield return new(typeof(DecisionsCustomToolCallOutputItem), "output", """{"type":"custom_tool_call_output","call_id":"call_2","output":VALUE}""", "input_text");
        yield return new(typeof(DecisionsChoiceQuestion), "instructions", """{"type":"choice","choices":[],"instructions":VALUE}""", "input_text");
        yield return new(typeof(DecisionsPredicateQuestion), "instructions", """{"type":"predicate","instructions":VALUE}""", "input_text");
        yield return new(typeof(DecisionsScoreQuestion), "instructions", """{"type":"score","levels":[],"instructions":VALUE}""", "input_text");
    }

    [TestCaseSource(nameof(TextOrPartsModels))]
    public void TextShorthandNormalizesToEquivalentLonghand(Type modelType, string property, string template, string textKind)
    {
        foreach (string text in new[] { "", "hello", "quotes \" and newline\n and Unicode \u263a" })
        {
            string encodedText = JsonSerializer.Serialize(text);
            string parts = $$"""[{"type":"{{textKind}}","text":{{encodedText}}}]""";
            if (property == "input")
            {
                parts = $$"""[{"type":"message","role":"user","content":{{parts}}}]""";
            }

            object shorthand = ModelReaderWriter.Read(BinaryData.FromString(template.Replace("VALUE", encodedText)), modelType);
            object longhand = ModelReaderWriter.Read(BinaryData.FromString(template.Replace("VALUE", parts)), modelType);
            foreach (string format in new[] { "J", "W" })
            {
                ModelReaderWriterOptions options = new(format);
                using JsonDocument actual = JsonDocument.Parse(ModelReaderWriter.Write(shorthand, options));
                using JsonDocument expected = JsonDocument.Parse(ModelReaderWriter.Write(longhand, options));
                Assert.That(JsonElement.DeepEquals(expected.RootElement, actual.RootElement), Is.True, modelType.Name);
                Assert.That(actual.RootElement.GetProperty(property).ValueKind, Is.EqualTo(JsonValueKind.Array));
                JsonElement content = property == "input"
                    ? actual.RootElement.GetProperty(property)[0].GetProperty("content")
                    : actual.RootElement.GetProperty(property);
                Assert.That(content[0].GetProperty("type").GetString(), Is.EqualTo(textKind));
                Assert.That(content[0].GetProperty("text").GetString(), Is.EqualTo(text));
            }
        }
    }

    [TestCaseSource(nameof(TextOrPartsModels))]
    public void TextOrPartsPropertiesRejectUnsupportedShapes(Type modelType, string property, string template, string textKind)
    {
        foreach (string invalid in new[] { "null", "true", "42", "{}", "[null]", "[false]", "[1]", """["text"]""" })
        {
            Assert.Throws<JsonException>(
                () => ModelReaderWriter.Read(BinaryData.FromString(template.Replace("VALUE", invalid)), modelType),
                $"{modelType.Name}.{property}: {invalid}");
        }
    }

    [TestCaseSource(nameof(TextOrPartsModels))]
    public void EmptyLonghandCollectionsRemainEmpty(Type modelType, string property, string template, string textKind)
    {
        object model = ModelReaderWriter.Read(BinaryData.FromString(template.Replace("VALUE", "[]")), modelType);
        using JsonDocument document = JsonDocument.Parse(ModelReaderWriter.Write(model));
        Assert.That(document.RootElement.GetProperty(property).GetArrayLength(), Is.Zero);
    }

    [TestCase("user", typeof(DecisionsUserMessageItem))]
    [TestCase("assistant", typeof(DecisionsAssistantMessageItem))]
    [TestCase("developer", typeof(DecisionsDeveloperMessageItem))]
    [TestCase("system", typeof(DecisionsSystemMessageItem))]
    public void MessageRolesDispatchThroughBothDiscriminators(string role, Type expectedType)
    {
        BinaryData json = BinaryData.FromString($$"""{"type":"message","role":"{{role}}","content":"hello","future":42}""");
        foreach (Type baseType in new[] { typeof(DecisionsItem), typeof(DecisionsMessageItem) })
        {
            DecisionsMessageItem message = (DecisionsMessageItem)ModelReaderWriter.Read(json, baseType);
            Assert.That(message.GetType(), Is.EqualTo(expectedType));
            Assert.That(message.Kind, Is.EqualTo(DecisionsItemKind.Message));
            Assert.That(message.Role.ToString(), Is.EqualTo(role));
            using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(message));
            Assert.That(serialized.RootElement.EnumerateObject().Select(property => property.Name), Is.Unique);
            Assert.That(serialized.RootElement.GetProperty("type").GetString(), Is.EqualTo("message"));
            Assert.That(serialized.RootElement.GetProperty("role").GetString(), Is.EqualTo(role));
            Assert.That(serialized.RootElement.GetProperty("future").GetInt32(), Is.EqualTo(42));
        }
    }

    [TestCase(typeof(DecisionsUserMessageItem), "user")]
    [TestCase(typeof(DecisionsAssistantMessageItem), "assistant")]
    [TestCase(typeof(DecisionsDeveloperMessageItem), "developer")]
    [TestCase(typeof(DecisionsSystemMessageItem), "system")]
    public void DefaultMessageConstructorsInitializeBothDiscriminators(Type modelType, string role)
    {
        DecisionsMessageItem message = (DecisionsMessageItem)Activator.CreateInstance(modelType);
        Assert.That(message.Kind, Is.EqualTo(DecisionsItemKind.Message));
        Assert.That(message.Role.ToString(), Is.EqualTo(role));
        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(message));
        Assert.That(serialized.RootElement.GetProperty("type").GetString(), Is.EqualTo("message"));
        Assert.That(serialized.RootElement.GetProperty("role").GetString(), Is.EqualTo(role));
        Assert.That(serialized.RootElement.GetProperty("content").GetArrayLength(), Is.Zero);
        Assert.That(serialized.RootElement.EnumerateObject().Select(property => property.Name), Is.Unique);
    }

    [Test]
    public void ContentConstructorsInitializeBothDiscriminators()
    {
        DecisionsMessageItem[] messages =
        [
            new DecisionsUserMessageItem([new DecisionsInputTextContentPart { Text = "user" }]),
            new DecisionsAssistantMessageItem([new DecisionsOutputTextContentPart { Text = "assistant" }]),
            new DecisionsDeveloperMessageItem([new DecisionsInputTextContentPart { Text = "developer" }]),
            new DecisionsSystemMessageItem([new DecisionsInputTextContentPart { Text = "system" }]),
        ];
        foreach (DecisionsMessageItem message in messages)
        {
            using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(message));
            Assert.That(message.Kind, Is.EqualTo(DecisionsItemKind.Message));
            Assert.That(serialized.RootElement.GetProperty("type").GetString(), Is.EqualTo("message"));
            Assert.That(serialized.RootElement.GetProperty("role").GetString(), Is.EqualTo(message.Role.ToString()));
            Assert.That(serialized.RootElement.GetProperty("content")[0].GetProperty("text").GetString(), Is.EqualTo(message.Role.ToString()));
            Assert.That(serialized.RootElement.EnumerateObject().Select(property => property.Name), Is.Unique);
        }
    }

    [Test]
    public void UnknownMessageRolesRoundTripThroughTheItemHierarchy()
    {
        const string json = """{"type":"message","role":"future_role","content":[{"type":"future_content","value":false}],"future":{"nested":true}}""";
        DecisionsMessageItem message = (DecisionsMessageItem)ModelReaderWriter.Read<DecisionsItem>(BinaryData.FromString(json));
        Assert.That(message.Kind, Is.EqualTo(DecisionsItemKind.Message));
        Assert.That(message.Role.ToString(), Is.EqualTo("future_role"));
        foreach (string format in new[] { "J", "W" })
        {
            using JsonDocument expected = JsonDocument.Parse(json);
            using JsonDocument actual = JsonDocument.Parse(ModelReaderWriter.Write(message, new(format)));
            Assert.That(JsonElement.DeepEquals(expected.RootElement, actual.RootElement), Is.True);
            Assert.That(actual.RootElement.EnumerateObject().Select(property => property.Name), Is.Unique);
        }
    }

    [Test]
    public void ExternalMessagesCanSpecifyFutureRolesAndPatchBothDiscriminators()
    {
        PropertyInfo role = typeof(DecisionsMessageItem).GetProperty(nameof(DecisionsMessageItem.Role));
        Assert.That(role.GetMethod.IsPublic, Is.True);
        Assert.That(role.SetMethod, Is.Null);
        ConstructorInfo constructor = typeof(DecisionsMessageItem).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, [typeof(DecisionsMessageRole)], null);
        Assert.That(constructor.IsFamilyOrAssembly, Is.True);

        DecisionsMessageItem message = new ExternalMessage();
        Assert.That(message.Kind, Is.EqualTo(DecisionsItemKind.Message));
        Assert.That(message.Role.ToString(), Is.EqualTo("external"));
        message.Patch.Set("$.role"u8, "patched_role");
        message.Patch.Set("$.type"u8, "patched_type");
        message.Patch.Set("$.extra"u8, 42);
        foreach (string format in new[] { "J", "W" })
        {
            using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(message, new(format)));
            Assert.That(serialized.RootElement.GetProperty("type").GetString(), Is.EqualTo("patched_type"));
            Assert.That(serialized.RootElement.GetProperty("role").GetString(), Is.EqualTo("patched_role"));
            Assert.That(serialized.RootElement.GetProperty("extra").GetInt32(), Is.EqualTo(42));
            Assert.That(serialized.RootElement.EnumerateObject().Select(property => property.Name), Is.Unique);
        }
    }

    [Test]
    [Ignore("Pending generator array-path fix: https://github.com/microsoft/typespec/issues/12431")]
    public void ToolHistoryPreservesAllFieldsAndConcreteTypes()
    {
        const string json = """
            {"model":"test","questions":[],"input":[
                {"type":"function_call","call_id":"f","name":"lookup","arguments":"{\"id\":1}","namespace":"functions","id":"fc","status":"in_progress"},
                {"type":"function_call_output","call_id":"f","output":[{"type":"input_text","text":"done"},{"type":"input_image","image_url":"data:image/png;base64,AA==","detail":"low","prompt_cache_breakpoint":{"mode":"explicit"}}],"id":"fo","status":"completed"},
                {"type":"custom_tool_call","call_id":"c","name":"search","input":"query","namespace":"tools","id":"ctc","status":"incomplete"},
                {"type":"custom_tool_call_output","call_id":"c","output":"found","id":"co","status":"future_status"}
            ]}
            """;
        DecisionCreationOptions options = ModelReaderWriter.Read<DecisionCreationOptions>(BinaryData.FromString(json));
        DecisionsFunctionToolCallItem function = (DecisionsFunctionToolCallItem)options.InputItems[0];
        Assert.That(function.Arguments, Is.EqualTo("""{"id":1}"""));
        Assert.That(function.Namespace, Is.EqualTo("functions"));
        Assert.That(function.Status, Is.EqualTo(DecisionsToolCallStatus.InProgress));
        DecisionsFunctionToolCallOutputItem output = (DecisionsFunctionToolCallOutputItem)options.InputItems[1];
        Assert.That(output.Output.Select(part => part.GetType()), Is.EqualTo(new[] { typeof(DecisionsToolOutputTextContentPart), typeof(DecisionsToolOutputImageContentPart) }));
        DecisionsCustomToolCallItem custom = (DecisionsCustomToolCallItem)options.InputItems[2];
        Assert.That(custom.Input, Is.EqualTo("query"));
        Assert.That(custom.Namespace, Is.EqualTo("tools"));
        Assert.That(custom.Status, Is.EqualTo(DecisionsToolCallStatus.Incomplete));
        Assert.That(((DecisionsCustomToolCallOutputItem)options.InputItems[3]).Status.ToString(), Is.EqualTo("future_status"));

        options.Patch.Set("$.input[1].output[0].text"u8, "patched");
        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(serialized.RootElement.GetProperty("input")[1].GetProperty("output")[0].GetProperty("text").GetString(), Is.EqualTo("patched"));
        foreach (DecisionsItem item in options.InputItems)
        {
            using JsonDocument itemJson = JsonDocument.Parse(ModelReaderWriter.Write(item));
            Assert.That(itemJson.RootElement.GetProperty("id").GetString(), Is.Not.Empty);
            Assert.That(itemJson.RootElement.GetProperty("call_id").GetString(), Is.Not.Empty);
        }
    }

    [Test]
    public void AssistantContentPreservesOutputTextRefusalsAndPhase()
    {
        const string json = """{"type":"message","role":"assistant","content":[{"type":"output_text","text":"answer"},{"type":"refusal","refusal":"No."}],"phase":"future_phase"}""";
        DecisionsAssistantMessageItem message = (DecisionsAssistantMessageItem)ModelReaderWriter.Read<DecisionsItem>(BinaryData.FromString(json));
        Assert.That(message.Content[0], Is.TypeOf<DecisionsOutputTextContentPart>());
        Assert.That(message.Content[1], Is.TypeOf<DecisionsRefusalContentPart>());
        Assert.That(((DecisionsRefusalContentPart)message.Content[1]).Refusal, Is.EqualTo("No."));
        Assert.That(message.Phase.ToString(), Is.EqualTo("future_phase"));
        using JsonDocument expected = JsonDocument.Parse(json);
        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(message));
        Assert.That(JsonElement.DeepEquals(expected.RootElement, serialized.RootElement), Is.True);
    }

    [TestCase("choice")]
    [TestCase("predicate")]
    [TestCase("score")]
    public void QuestionsAcceptMultimodalInstructions(string kind)
    {
        string json = $$"""
            {"type":"{{kind}}","instructions":[
                {"type":"input_text","text":"evaluate"},
                {"type":"input_image","image_url":"https://example.invalid/reference.png","detail":"original"},
                {"type":"input_json","data":[{"expected":true},{"value":null}]}
            ]
            """;
        json += kind switch
        {
            "choice" => ",\"choices\":[]}",
            "score" => ",\"levels\":[]}",
            _ => "}",
        };
        DecisionsQuestion question = ModelReaderWriter.Read<DecisionsQuestion>(BinaryData.FromString(json));
        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(question));
        JsonElement instructions = serialized.RootElement.GetProperty("instructions");
        Assert.That(instructions.GetArrayLength(), Is.EqualTo(3));
        Assert.That(instructions[0].GetProperty("text").GetString(), Is.EqualTo("evaluate"));
        Assert.That(instructions[1].GetProperty("image_url").GetString(), Is.EqualTo("https://example.invalid/reference.png"));
        Assert.That(instructions[1].GetProperty("detail").GetString(), Is.EqualTo("original"));
        Assert.That(instructions[2].GetProperty("data")[0].GetProperty("expected").GetBoolean(), Is.True);
        Assert.That(typeof(DecisionsInstructionsInputTextPart).GetProperty("PromptCacheBreakpoint"), Is.Null);
        Assert.That(typeof(DecisionsInstructionsInputImagePart).GetProperty("PromptCacheBreakpoint"), Is.Null);
    }

    [Test]
    [Ignore("Pending generator array-path fix: https://github.com/microsoft/typespec/issues/12431")]
    public void PromptCacheOptionsAndBreakpointsRoundTripAndPropagatePatches()
    {
        const string json = """
            {"model":"test","questions":[],"prompt_cache_key":"cache-test","prompt_cache_options":{"mode":"explicit","ttl":"30m","future":true},"input":[
                {"type":"message","role":"system","content":[{"type":"input_text","text":"context","prompt_cache_breakpoint":{"mode":"explicit"}}]},
                {"type":"message","role":"user","content":[{"type":"input_text","text":"text","prompt_cache_breakpoint":{"mode":"explicit"}},{"type":"input_image","image_url":"https://example.invalid/image.png","prompt_cache_breakpoint":{"mode":"explicit"}}]}
            ]}
            """;
        DecisionCreationOptions options = ModelReaderWriter.Read<DecisionCreationOptions>(BinaryData.FromString(json));
        Assert.That(options.PromptCacheKey, Is.EqualTo("cache-test"));
        Assert.That(options.PromptCacheOptions.Mode, Is.EqualTo(DecisionsPromptCacheMode.Explicit));
        Assert.That(options.PromptCacheOptions.TimeToLive, Is.EqualTo(DecisionsPromptCacheTimeToLive.ThirtyMinutes));
        DecisionsSystemMessageItem system = (DecisionsSystemMessageItem)options.InputItems[0];
        Assert.That(((DecisionsInputTextContentPart)system.Content[0]).PromptCacheBreakpoint.Mode, Is.EqualTo(DecisionsPromptCacheBreakpointMode.Explicit));
        DecisionsUserMessageItem user = (DecisionsUserMessageItem)options.InputItems[1];
        Assert.That(((DecisionsInputTextContentPart)user.Content[0]).PromptCacheBreakpoint.Mode, Is.EqualTo(DecisionsPromptCacheBreakpointMode.Explicit));
        Assert.That(((DecisionsInputImageContentPart)user.Content[1]).PromptCacheBreakpoint.Mode, Is.EqualTo(DecisionsPromptCacheBreakpointMode.Explicit));

        options.Patch.Set("$.prompt_cache_options.ttl"u8, "future_ttl");
        options.Patch.Set("$.input[1].content[1].prompt_cache_breakpoint.mode"u8, "future_mode");
        using JsonDocument serialized = JsonDocument.Parse(ModelReaderWriter.Write(options));
        Assert.That(serialized.RootElement.GetProperty("prompt_cache_options").GetProperty("ttl").GetString(), Is.EqualTo("future_ttl"));
        Assert.That(serialized.RootElement.GetProperty("prompt_cache_options").GetProperty("future").GetBoolean(), Is.True);
        Assert.That(serialized.RootElement.GetProperty("input")[1].GetProperty("content")[1].GetProperty("prompt_cache_breakpoint").GetProperty("mode").GetString(), Is.EqualTo("future_mode"));
        Assert.That(new DecisionsPromptCacheOptions { Mode = DecisionsPromptCacheMode.Implicit }.Mode, Is.EqualTo(DecisionsPromptCacheMode.Implicit));
    }

    [Test]
    public void NullableRequestSettingsRemainNull()
    {
        const string json = """{"model":"test","questions":[],"input":[],"prompt_cache_key":null,"prompt_cache_options":null,"safety_identifier":null}""";
        DecisionCreationOptions options = ModelReaderWriter.Read<DecisionCreationOptions>(BinaryData.FromString(json));
        Assert.That(options.PromptCacheKey, Is.Null);
        Assert.That(options.PromptCacheOptions, Is.Null);
        Assert.That(options.SafetyIdentifier, Is.Null);
        DecisionsInputImageContentPart image = ModelReaderWriter.Read<DecisionsInputImageContentPart>(
            BinaryData.FromString("""{"type":"input_image","image_url":"https://example.invalid/image.png","detail":null,"prompt_cache_breakpoint":null}"""));
        Assert.That(image.ImageDetailLevel, Is.Null);
        Assert.That(image.PromptCacheBreakpoint, Is.Null);
    }

    [Test]
#pragma warning disable SCME0002
    public void DecisionsTypesRemainExperimental()
    {
        Type[] types = typeof(DecisionsClient).Assembly.GetExportedTypes().Where(type => type.Namespace == "OpenAI.Decisions").ToArray();
        Assert.That(types, Is.Not.Empty);
        foreach (Type type in types)
        {
            ExperimentalAttribute attribute = type.GetCustomAttribute<ExperimentalAttribute>();
            Assert.That(attribute, Is.Not.Null, type.Name);
            Assert.That(attribute.DiagnosticId, Is.EqualTo(type == typeof(DecisionsClientSettings) ? "SCME0002" : "OPENAI001"), type.Name);
        }
    }
#pragma warning restore SCME0002

    private sealed class ExternalMessage() : DecisionsMessageItem(new DecisionsMessageRole("external"))
    {
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            base.JsonModelWriteCore(writer, options);
            Patch.WriteTo(writer);
        }
    }
}
