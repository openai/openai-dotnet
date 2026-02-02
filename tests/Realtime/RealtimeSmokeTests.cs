using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.ClientModel.Primitives;
using System.Linq;
using System.Text.Json.Nodes;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI002

[Category("Smoke")]
public class RealtimeSmokeTests : ClientTestBase
{
    public RealtimeSmokeTests(bool isAsync) : base(isAsync)
    { }

    [Test]
    public void ToolChoiceSerializationInSessionOptionsWorks()
    {
        foreach((ConversationToolChoice toolChoice, string expected) in new (ConversationToolChoice, string)[]
        {
            // GA format: empty options now serialize with "type":"realtime" and "max_output_tokens":null
            (null, @"""type"":""realtime"""),
            (ConversationToolChoice.CreateNoneToolChoice(), @"""tool_choice"":""none"""),
            (ConversationToolChoice.CreateAutoToolChoice(), @"""tool_choice"":""auto"""),
            (ConversationToolChoice.CreateRequiredToolChoice(), @"""tool_choice"":""required"""),
            (ConversationToolChoice.CreateFunctionToolChoice("foo"), @"""function"":{""name"":""foo""")
        })
        {
            ConversationSessionOptions options = new()
            {
                ToolChoice = toolChoice,
            };
            Assert.That(ModelReaderWriter.Write(options).ToString(), Does.Contain(expected));
        }

        ConversationToolChoice mrwToolChoice = ModelReaderWriter.Read<ConversationToolChoice>(
            BinaryData.FromString("""
                {
                  "type":"some_manual_type"
                }
                """));
        Assert.That(mrwToolChoice.Kind, Is.EqualTo(ConversationToolChoiceKind.Unknown));
        Assert.That(ModelReaderWriter.Write(mrwToolChoice).ToString(), Does.Contain(@"""type"":""some_manual_type"""));
    }

    [Test]
    public void ItemCreation()
    {
        RealtimeItem messageItem = RealtimeItem.CreateUserMessage(["Hello, world!"]);
        Assert.That(messageItem?.MessageContentParts?.Count, Is.EqualTo(1));
        Assert.That(messageItem.MessageContentParts[0].Text, Is.EqualTo("Hello, world!"));
    }

    [Test]
    public void OptionsSerializationWorks()
    {
        ConversationSessionOptions options = new()
        {
            ContentModalities = RealtimeContentModalities.Text,
            Audio = new RealtimeSessionAudioConfiguration()
            {
                Input = new RealtimeSessionAudioInputConfiguration()
                {
                    Format = RealtimeAudioFormat.G711Alaw,
                    Transcription = new InputTranscriptionOptions()
                    {
                        Model = "whisper-1",
                    },
                    TurnDetection = TurnDetectionOptions.CreateServerVoiceActivityTurnDetectionOptions(
                        detectionThreshold: 0.42f,
                        prefixPaddingDuration: TimeSpan.FromMilliseconds(234),
                        silenceDuration: TimeSpan.FromMilliseconds(345)),
                },
                Output = new RealtimeSessionAudioOutputConfiguration()
                {
                    Format = RealtimeAudioFormat.G711Ulaw,
                    Voice = ConversationVoice.Echo,
                },
            },
            Instructions = "test instructions",
            MaxOutputTokens = 42,
            Temperature = 0.42f,
            ToolChoice = ConversationToolChoice.CreateFunctionToolChoice("test-function"),
            Tools =
            {
                ConversationTool.CreateFunctionTool(
                    name: "test-function-tool-name",
                    description: "description of test function tool",
                    parameters: BinaryData.FromString("""
                        {
                          "type": "object",
                          "properties": {}
                        }
                        """)),
            },
        };
        BinaryData serializedOptions = ModelReaderWriter.Write(options);
        JsonNode jsonNode = JsonNode.Parse(serializedOptions.ToString());
        Assert.That(jsonNode["output_modalities"]?.AsArray()?.ToList(), Has.Count.EqualTo(1));
        Assert.That(jsonNode["output_modalities"].AsArray().First().GetValue<string>(), Is.EqualTo("text"));
        // GA format: audio format is now an object with "type" discriminator
        Assert.That(jsonNode["audio"]?["input"]?["format"]?["type"]?.GetValue<string>(), Is.EqualTo("audio/pcma"));
        Assert.That(jsonNode["audio"]?["input"]?["transcription"]?["model"]?.GetValue<string>(), Is.EqualTo("whisper-1"));
        Assert.That(jsonNode["instructions"]?.GetValue<string>(), Is.EqualTo("test instructions"));
        Assert.That(jsonNode["max_output_tokens"]?.GetValue<int>(), Is.EqualTo(42));
        // GA format: audio format is now an object with "type" discriminator
        Assert.That(jsonNode["audio"]?["output"]?["format"]?["type"]?.GetValue<string>(), Is.EqualTo("audio/pcmu"));
        Assert.That(jsonNode["temperature"]?.GetValue<float>(), Is.EqualTo(0.42f));
        Assert.That(jsonNode["tools"]?.AsArray()?.ToList(), Has.Count.EqualTo(1));
        Assert.That(jsonNode["tools"].AsArray().First()["name"]?.GetValue<string>(), Is.EqualTo("test-function-tool-name"));
        Assert.That(jsonNode["tools"].AsArray().First()["description"]?.GetValue<string>(), Is.EqualTo("description of test function tool"));
        Assert.That(jsonNode["tools"].AsArray().First()["parameters"]?["type"]?.GetValue<string>(), Is.EqualTo("object"));
        Assert.That(jsonNode["tool_choice"]?["function"]?["name"]?.GetValue<string>(), Is.EqualTo("test-function"));
        Assert.That(jsonNode["audio"]?["input"]?["turn_detection"]?["threshold"]?.GetValue<float>(), Is.EqualTo(0.42f));
        Assert.That(jsonNode["audio"]?["input"]?["turn_detection"]?["prefix_padding_ms"]?.GetValue<int>(), Is.EqualTo(234));
        Assert.That(jsonNode["audio"]?["input"]?["turn_detection"]?["silence_duration_ms"]?.GetValue<int>(), Is.EqualTo(345));
        // GA format: voice is now inside audio.output
        Assert.That(jsonNode["audio"]?["output"]?["voice"]?.GetValue<string>(), Is.EqualTo("echo"));
        ConversationSessionOptions deserializedOptions = ModelReaderWriter.Read<ConversationSessionOptions>(serializedOptions);
        Assert.That(deserializedOptions.ContentModalities.HasFlag(RealtimeContentModalities.Text));
        Assert.That(deserializedOptions.ContentModalities.HasFlag(RealtimeContentModalities.Audio), Is.False);
        Assert.That(deserializedOptions.Audio?.Input?.Format, Is.EqualTo(RealtimeAudioFormat.G711Alaw));
        Assert.That(deserializedOptions.Audio?.Input?.Transcription?.Model, Is.EqualTo(InputTranscriptionModel.Whisper1));
        Assert.That(deserializedOptions.Instructions, Is.EqualTo("test instructions"));
        Assert.That(deserializedOptions.MaxOutputTokens.NumericValue, Is.EqualTo(42));
        Assert.That(deserializedOptions.Audio?.Output?.Format, Is.EqualTo(RealtimeAudioFormat.G711Ulaw));
        Assert.That(deserializedOptions.Tools, Has.Count.EqualTo(1));
        Assert.That(deserializedOptions.Tools[0].Kind, Is.EqualTo(ConversationToolKind.Function));
        Assert.That((deserializedOptions.Tools[0] as ConversationFunctionTool)?.Name, Is.EqualTo("test-function-tool-name"));
        Assert.That((deserializedOptions.Tools[0] as ConversationFunctionTool)?.Description, Is.EqualTo("description of test function tool"));
        Assert.That((deserializedOptions.Tools[0] as ConversationFunctionTool)?.Parameters?.ToString(), Does.Contain("properties"));
        Assert.That(deserializedOptions.ToolChoice?.Kind, Is.EqualTo(ConversationToolChoiceKind.Function));
        Assert.That(deserializedOptions.ToolChoice?.FunctionName, Is.EqualTo("test-function"));
        Assert.That(deserializedOptions.Audio?.Input?.TurnDetection?.Kind, Is.EqualTo(TurnDetectionKind.ServerVoiceActivityDetection));
        // GA format: voice is now inside Audio.Output
        Assert.That(deserializedOptions.Audio?.Output?.Voice, Is.EqualTo(ConversationVoice.Echo));

        ConversationSessionOptions emptyOptions = new();
        Assert.That(emptyOptions.ContentModalities.HasFlag(RealtimeContentModalities.Audio), Is.False);
        Assert.That(ModelReaderWriter.Write(emptyOptions).ToString(), Does.Not.Contain("modal"));
        emptyOptions.ContentModalities |= RealtimeContentModalities.Audio;
        Assert.That(emptyOptions.ContentModalities.HasFlag(RealtimeContentModalities.Audio), Is.True);
        Assert.That(emptyOptions.ContentModalities.HasFlag(RealtimeContentModalities.Text), Is.False);
        Assert.That(ModelReaderWriter.Write(emptyOptions).ToString(), Does.Contain("modal"));
    }

    [Test]
    public void MaxTokensSerializationWorks()
    {
        // GA format: max_output_tokens is always serialized (as null for default)
        // Implicit omission
        ConversationSessionOptions options = new() { };
        BinaryData serializedOptions = ModelReaderWriter.Write(options);
        Assert.That(serializedOptions.ToString(), Does.Contain(@"""max_output_tokens"":null"));

        // Explicit omission (null)
        options = new()
        {
            MaxOutputTokens = null
        };
        serializedOptions = ModelReaderWriter.Write(options);
        Assert.That(serializedOptions.ToString(), Does.Contain(@"""max_output_tokens"":null"));

        // Explicit default (null)
        options = new()
        {
            MaxOutputTokens = ConversationMaxTokensChoice.CreateDefaultMaxTokensChoice()
        };
        serializedOptions = ModelReaderWriter.Write(options);
        Assert.That(serializedOptions.ToString(), Does.Contain(@"""max_output_tokens"":null"));

        // Numeric literal
        options = new()
        {
            MaxOutputTokens = 42,
        };
        serializedOptions = ModelReaderWriter.Write(options);
        Assert.That(serializedOptions.ToString(), Does.Contain(@"""max_output_tokens"":42"));

        // Numeric by factory
        options = new()
        {
            MaxOutputTokens = ConversationMaxTokensChoice.CreateNumericMaxTokensChoice(42)
        };
        serializedOptions = ModelReaderWriter.Write(options);
        Assert.That(serializedOptions.ToString(), Does.Contain(@"""max_output_tokens"":42"));
    }

    [Test]
    public void TurnDetectionSerializationWorks()
    {
        // GA format: turn_detection is now inside audio.input
        // Implicit omission
        ConversationSessionOptions sessionOptions = new();
        BinaryData serializedOptions = ModelReaderWriter.Write(sessionOptions);
        Assert.That(serializedOptions.ToString(), Does.Not.Contain("turn_detection"));

        sessionOptions = new()
        {
            Audio = new RealtimeSessionAudioConfiguration()
            {
                Input = new RealtimeSessionAudioInputConfiguration()
                {
                    TurnDetection = TurnDetectionOptions.CreateDisabledTurnDetectionOptions(),
                },
            },
        };
        serializedOptions = ModelReaderWriter.Write(sessionOptions);
        Assert.That(serializedOptions.ToString(), Does.Contain(@"""turn_detection"":null"));

        sessionOptions = new()
        {
            Audio = new RealtimeSessionAudioConfiguration()
            {
                Input = new RealtimeSessionAudioInputConfiguration()
                {
                    TurnDetection = TurnDetectionOptions.CreateServerVoiceActivityTurnDetectionOptions(
                        detectionThreshold: 0.42f)
                },
            },
        };
        serializedOptions = ModelReaderWriter.Write(sessionOptions);
        JsonNode serializedNode = JsonNode.Parse(serializedOptions);
        Assert.That(serializedNode["audio"]?["input"]?["turn_detection"]?["type"]?.GetValue<string>(), Is.EqualTo("server_vad"));
        Assert.That(serializedNode["audio"]?["input"]?["turn_detection"]?["threshold"]?.GetValue<float>(), Is.EqualTo(0.42f));

        sessionOptions = new()
        {
            Audio = new RealtimeSessionAudioConfiguration()
            {
                Input = new RealtimeSessionAudioInputConfiguration()
                {
                    TurnDetection = TurnDetectionOptions.CreateSemanticVoiceActivityTurnDetectionOptions(
                        SemanticEagernessLevel.Medium, true, true)
                },
            },
        };
        serializedOptions = ModelReaderWriter.Write(sessionOptions);
        serializedNode = JsonNode.Parse(serializedOptions);
        Assert.That(serializedNode["audio"]?["input"]?["turn_detection"]?["type"]?.GetValue<string>(), Is.EqualTo("semantic_vad"));
        Assert.That(serializedNode["audio"]?["input"]?["turn_detection"]?["eagerness"]?.GetValue<string>(), Is.EqualTo("medium"));
        Assert.That(serializedNode["audio"]?["input"]?["turn_detection"]?["create_response"]?.GetValue<bool>(), Is.EqualTo(true));
        Assert.That(serializedNode["audio"]?["input"]?["turn_detection"]?["interrupt_response"]?.GetValue<bool>(), Is.EqualTo(true));
    }

    [Test]
    public void UnknownCommandSerializationWorks()
    {
        BinaryData serializedUnknownCommand = BinaryData.FromString("""
        {
          "type": "unknown_command_type_for_test"
        }
        """);
        RealtimeUpdate deserializedUpdate = ModelReaderWriter.Read<RealtimeUpdate>(serializedUnknownCommand);
        Assert.That(deserializedUpdate, Is.Not.Null);
    }

    [Test]
    public void ResponseFinishedUpdateCreatedItemsRoundTrip()
    {
        // Create sample items to include in the response
        RealtimeItem userMessageItem = RealtimeItem.CreateUserMessage(["Hello, world!"]);
        userMessageItem.Id = "item_123";

        RealtimeItem functionCallItem = RealtimeItem.CreateFunctionCall("test_function", "call_456", "{\"param\":\"value\"}");
        functionCallItem.Id = "item_456";

        RealtimeItem functionOutputItem = RealtimeItem.CreateFunctionCallOutput("call_456", "Function result");
        functionOutputItem.Id = "item_789";

        BinaryData serializedResponse = BinaryData.FromString($$"""
        {
          "type": "response.done",
          "event_id": "event_123",
          "response": {
            "id": "response_abc",
            "object": "realtime.response",
            "status": "completed",
            "status_details": null,
            "output": [
              {{ModelReaderWriter.Write(userMessageItem)}},
              {{ModelReaderWriter.Write(functionCallItem)}},
              {{ModelReaderWriter.Write(functionOutputItem)}}
            ],
            "usage": {
              "total_tokens": 100,
              "input_tokens": 50,
              "output_tokens": 50
            }
          }
        }
        """);

        // Deserialize the response
        ResponseFinishedUpdate deserializedUpdate = ModelReaderWriter.Read<ResponseFinishedUpdate>(serializedResponse);
        
        // Validate CreatedItems property
        Assert.That(deserializedUpdate, Is.Not.Null);
        Assert.That(deserializedUpdate.CreatedItems, Is.Not.Null);
        Assert.That(deserializedUpdate.CreatedItems, Has.Count.EqualTo(3));
        
        // Validate first item (user message)
        Assert.That(deserializedUpdate.CreatedItems[0].Id, Is.EqualTo("item_123"));
        Assert.That(deserializedUpdate.CreatedItems[0].MessageRole, Is.EqualTo(ConversationMessageRole.User));
        Assert.That(deserializedUpdate.CreatedItems[0].MessageContentParts, Has.Count.EqualTo(1));
        Assert.That(deserializedUpdate.CreatedItems[0].MessageContentParts[0].Text, Is.EqualTo("Hello, world!"));
        
        // Validate second item (function call)
        Assert.That(deserializedUpdate.CreatedItems[1].Id, Is.EqualTo("item_456"));
        Assert.That(deserializedUpdate.CreatedItems[1].FunctionName, Is.EqualTo("test_function"));
        Assert.That(deserializedUpdate.CreatedItems[1].FunctionCallId, Is.EqualTo("call_456"));
        Assert.That(deserializedUpdate.CreatedItems[1].FunctionArguments, Is.EqualTo("{\"param\":\"value\"}"));
        
        // Validate third item (function output) - just verify it was deserialized
        Assert.That(deserializedUpdate.CreatedItems[2].Id, Is.EqualTo("item_789"));

        // Serialize back and validate round-trip
        BinaryData reserializedResponse = ModelReaderWriter.Write(deserializedUpdate);
        var reserializedString = reserializedResponse.ToString();
        Assert.That(reserializedString, Does.Contain(@"""object"":""realtime.response"""));

        JsonNode reserializedNode = JsonNode.Parse(reserializedString);
        Assert.That(reserializedNode["response"]?["output"]?.AsArray()?.Count, Is.EqualTo(3));
        Assert.That(reserializedNode["response"]?["output"]?[0]?["id"]?.GetValue<string>(), Is.EqualTo("item_123"));
        Assert.That(reserializedNode["response"]?["output"]?[1]?["id"]?.GetValue<string>(), Is.EqualTo("item_456"));
        Assert.That(reserializedNode["response"]?["output"]?[2]?["id"]?.GetValue<string>(), Is.EqualTo("item_789"));
    }
}
