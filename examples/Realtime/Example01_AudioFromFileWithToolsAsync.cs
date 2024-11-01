using NUnit.Framework;
using OpenAI.Images;
using OpenAI.RealtimeConversation;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Examples;

#pragma warning disable OPENAI002

public partial class RealtimeExamples
{
    [Test]
    public async Task Example01_AudioFromFileWithToolsAsync()
    {
        RealtimeConversationClient client = new(
            model: "gpt-4o-realtime-preview",
            credential: new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY")));
        using RealtimeConversationSession session = await client.StartConversationSessionAsync();

        // Session options control connection-wide behavior shared across all conversations,
        // including audio input format and voice activity detection settings.
        ConversationSessionOptions sessionOptions = new()
        {
            Instructions = "You are a cheerful assistant that talks like a pirate. "
                + "Always inform the user when you are about to call a tool. "
                + "Prefer to call tools whenever applicable.",
            Voice = ConversationVoice.Alloy,
            Tools = { CreateSampleWeatherTool() },
            InputAudioFormat = ConversationAudioFormat.Pcm16,
            OutputAudioFormat = ConversationAudioFormat.Pcm16,
            // Input transcription options must be provided to enable transcribed feedback for input audio
            InputTranscriptionOptions = new()
            {
                Model = "whisper-1",
            },
        };

        await session.ConfigureSessionAsync(sessionOptions);

        // Conversation history or text input are provided by adding messages to the conversation.
        // Adding a message will not automatically begin a response turn.
        await session.AddItemAsync(
            ConversationItem.CreateUserMessage(["I'm trying to decide what to wear on my trip."]));

        string inputAudioPath = FindFile("Assets\\realtime_whats_the_weather_pcm16_24khz_mono.wav");
        using Stream inputAudioStream = File.OpenRead(inputAudioPath);
        _ = session.SendInputAudioAsync(inputAudioStream);

        Dictionary<string, Stream> outputAudioStreamsById = [];

        await foreach (ConversationUpdate update in session.ReceiveUpdatesAsync())
        {
            if (update is ConversationSessionStartedUpdate sessionStartedUpdate)
            {
                Console.WriteLine($"<<< Session started. ID: {sessionStartedUpdate.SessionId}");
                Console.WriteLine();
            }

            if (update is ConversationInputSpeechStartedUpdate speechStartedUpdate)
            {
                Console.WriteLine(
                    $"  -- Voice activity detection started at {speechStartedUpdate.AudioStartTime}");
            }

            if (update is ConversationInputSpeechFinishedUpdate speechFinishedUpdate)
            {
                Console.WriteLine(
                    $"  -- Voice activity detection ended at {speechFinishedUpdate.AudioEndTime}");
            }

            // Item started updates notify that the model generation process will insert a new item into
            // the conversation and begin streaming its content via content updates.
            if (update is ConversationItemStreamingStartedUpdate itemStreamingStartedUpdate)
            {
                Console.WriteLine($"  -- Begin streaming of new item");
                if (!string.IsNullOrEmpty(itemStreamingStartedUpdate.FunctionName))
                {
                    Console.Write($"    {itemStreamingStartedUpdate.FunctionName}: ");
                }
            }

            if (update is ConversationItemStreamingPartDeltaUpdate deltaUpdate)
            {
                // With audio output enabled, the audio transcript of the delta update contains an approximation of
                // the words spoken by the model. Without audio output, the text of the delta update will contain
                // the segments making up the text content of a message.
                Console.Write(deltaUpdate.AudioTranscript);
                Console.Write(deltaUpdate.Text);
                Console.Write(deltaUpdate.FunctionArguments);
                if (deltaUpdate.AudioBytes is not null)
                {
                    if (!outputAudioStreamsById.TryGetValue(deltaUpdate.ItemId, out Stream value))
                    {
                        string filename = $"output_{sessionOptions.OutputAudioFormat}_{deltaUpdate.ItemId}.raw";
                        value = File.OpenWrite(filename);
                        outputAudioStreamsById[deltaUpdate.ItemId] = value;
                    }

                    value.Write(deltaUpdate.AudioBytes);
                }
            }

            // Item finished updates arrive when all streamed data for an item has arrived and the
            // accumulated results are available. In the case of function calls, this is the point
            // where all arguments are expected to be present.
            if (update is ConversationItemStreamingFinishedUpdate itemStreamingFinishedUpdate)
            {
                Console.WriteLine();
                Console.WriteLine($"  -- Item streaming finished, item_id={itemStreamingFinishedUpdate.ItemId}");

                if (itemStreamingFinishedUpdate.FunctionCallId is not null)
                {
                    Console.WriteLine($"    + Responding to tool invoked by item: {itemStreamingFinishedUpdate.FunctionName}");
                    ConversationItem functionOutputItem = ConversationItem.CreateFunctionCallOutput(
                        callId: itemStreamingFinishedUpdate.FunctionCallId,
                        output: "70 degrees Fahrenheit and sunny");
                    await session.AddItemAsync(functionOutputItem);
                }
                else if (itemStreamingFinishedUpdate.MessageContentParts?.Count > 0)
                {
                    Console.Write($"    + [{itemStreamingFinishedUpdate.MessageRole}]: ");
                    foreach (ConversationContentPart contentPart in itemStreamingFinishedUpdate.MessageContentParts)
                    {
                        Console.Write(contentPart.AudioTranscript);
                    }
                    Console.WriteLine();
                }
            }

            if (update is ConversationInputTranscriptionFinishedUpdate transcriptionCompletedUpdate)
            {
                Console.WriteLine();
                Console.WriteLine($"  -- User audio transcript: {transcriptionCompletedUpdate.Transcript}");
                Console.WriteLine();
            }

            if (update is ConversationResponseFinishedUpdate turnFinishedUpdate)
            {
                Console.WriteLine($"  -- Model turn generation finished. Status: {turnFinishedUpdate.Status}");

                // Here, if we processed tool calls in the course of the model turn, we finish the
                // client turn to resume model generation. The next model turn will reflect the tool
                // responses that were already provided.
                if (turnFinishedUpdate.CreatedItems.Any(item => item.FunctionName?.Length > 0))
                {
                    Console.WriteLine($"  -- Ending client turn for pending tool responses");
                    await session.StartResponseAsync();
                }
                else
                {
                    break;
                }
            }

            if (update is ConversationErrorUpdate errorUpdate)
            {
                Console.WriteLine();
                Console.WriteLine($"ERROR: {errorUpdate.Message}");
                break;
            }
        }

        foreach ((string itemId, Stream outputAudioStream) in outputAudioStreamsById)
        {
            Console.WriteLine($"Raw audio output for {itemId}: {outputAudioStream.Length} bytes");
            outputAudioStream.Dispose();
        }
    }

    private static ConversationFunctionTool CreateSampleWeatherTool()
    {
        return new ConversationFunctionTool()
        {
            Name = "get_weather_for_location",
            Description = "gets the weather for a location",
            Parameters = BinaryData.FromString("""
            {
              "type": "object",
              "properties": {
                "location": {
                  "type": "string",
                  "description": "The city and state, e.g. San Francisco, CA"
                },
                "unit": {
                  "type": "string",
                  "enum": ["c","f"]
                }
              },
              "required": ["location","unit"]
            }
            """)
        };
    }

    private static string FindFile(string fileName)
    {
        for (string currentDirectory = Directory.GetCurrentDirectory();
             currentDirectory != null && currentDirectory != Path.GetPathRoot(currentDirectory);
             currentDirectory = Directory.GetParent(currentDirectory)?.FullName!)
        {
            string filePath = Path.Combine(currentDirectory, fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }
        }

        throw new FileNotFoundException($"File '{fileName}' not found.");
    }
}
