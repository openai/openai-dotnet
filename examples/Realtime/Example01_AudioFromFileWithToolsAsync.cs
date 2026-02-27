using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Examples;

#pragma warning disable OPENAI002

public partial class RealtimeExamples
{
    private static string GetCurrentWeather(string location, string unit = "celsius")
    {
        // Call the weather API here.
        return $"31 {unit}";
    }

    private static readonly RealtimeFunctionTool getCurrentWeatherTool = new(functionName: nameof(GetCurrentWeather))
    {
        FunctionDescription = "gets the weather for a location",
        FunctionParameters = BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "location": {
                        "type": "string",
                        "description": "The city and state, e.g. Boston, MA"
                    },
                    "unit": {
                        "type": "string",
                        "enum": [ "celsius", "fahrenheit" ],
                        "description": "The temperature unit to use. Infer this from the specified location."
                    }
                },
                "required": [ "location" ]
            }
            """)
    };

    [Test]
    public async Task Example01_AudioFromFileWithToolsAsync()
    {
        RealtimeClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(model: "gpt-realtime");

        RealtimeConversationSessionOptions sessionOptions = new()
        {
            Instructions = "You are a cheerful assistant that talks like a pirate. "
                + "Always inform the user when you are about to call a tool. "
                + "Prefer to call tools whenever applicable.",

            Tools = { getCurrentWeatherTool },

            AudioOptions = new()
            {
                InputAudioOptions = new()
                {
                    // AudioFormat = new GARealtimePcmAudioFormat(),
                    AudioTranscriptionOptions = new()
                    {
                        Model = "gpt-4o-transcribe",
                    },
                    TurnDetection = new RealtimeServerVadTurnDetection(),
                },
                OutputAudioOptions = new()
                {
                    // AudioFormat = new GARealtimePcmAudioFormat(),
                    Voice = RealtimeVoice.Alloy,
                },
            },
        };

        await sessionClient.ConfigureConversationSessionAsync(sessionOptions);

        // The conversation history (if applicable) can be provided by adding messages to the
        // conversation one by one. Note that adding a message will not automatically initiate
        // a response from the model.
        await sessionClient.AddItemAsync(RealtimeItem.CreateUserMessageItem("I'm trying to decide what to wear on my trip."));

        string inputAudioFilePath = Path.Join("Assets", "realtime_whats_the_weather_pcm16_24khz_mono.wav");
        using Stream inputAudioStream = File.OpenRead(inputAudioFilePath);
        _ = sessionClient.SendInputAudioAsync(inputAudioStream);

        string outputAudioFilePath = Path.Join("output.raw");
        using Stream outputAudioStream = File.OpenWrite(outputAudioFilePath);

        bool done = false;

        await foreach (RealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync())
        {
            switch (update)
            {
                case RealtimeServerUpdateSessionCreated sessionCreatedUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {sessionCreatedUpdate.EventId}]");
                        Console.WriteLine($">> Session created.");
                        Console.WriteLine();
                        break;
                    }
                case RealtimeServerUpdateSessionUpdated sessionUpdatedUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {sessionUpdatedUpdate.EventId}]");
                        Console.WriteLine($">> Session updated.");
                        Console.WriteLine();
                        break;
                    }
                case RealtimeServerUpdateInputAudioBufferSpeechStarted inputAudioBufferSpeechStartedUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {inputAudioBufferSpeechStartedUpdate.EventId}]");
                        Console.WriteLine($">> Speech started at {inputAudioBufferSpeechStartedUpdate.AudioStartTime}.");
                        Console.WriteLine();
                        break;
                    }
                case RealtimeServerUpdateInputAudioBufferSpeechStopped inputAudioBufferSpeechStoppedUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {inputAudioBufferSpeechStoppedUpdate.EventId}]");
                        Console.WriteLine($">> Speech stopped at {inputAudioBufferSpeechStoppedUpdate.AudioEndTime}.");
                        Console.WriteLine();
                        break;
                    }
                case RealtimeServerUpdateConversationItemDone conversationItemDoneUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {conversationItemDoneUpdate.EventId}]");
                        Console.WriteLine($">> Conversation item done. Type: {conversationItemDoneUpdate.Item.Patch.GetString("$.type"u8)}.");

                        RealtimeMessageItem messageItem = conversationItemDoneUpdate.Item as RealtimeMessageItem;

                        if (messageItem != null)
                        {
                            foreach (RealtimeMessageContentPart contentPart in messageItem.Content)
                            {
                                switch (contentPart)
                                {
                                    case RealtimeInputTextMessageContentPart inputTextPart:
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine($"++ [{messageItem.Role.ToString().ToUpperInvariant()}]:");
                                            Console.WriteLine(inputTextPart.Text);
                                            break;
                                        }
                                    case RealtimeOutputTextMessageContentPart outputTextPart:
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine($"++ [{messageItem.Role.ToString().ToUpperInvariant()}]:");
                                            Console.WriteLine(outputTextPart.Text);
                                            break;
                                        }
                                }
                            }
                        }

                        Console.WriteLine();
                        break;
                    }
                case RealtimeServerUpdateConversationItemInputAudioTranscriptionCompleted conversationItemInputAudioTranscriptionCompletedUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {conversationItemInputAudioTranscriptionCompletedUpdate.EventId}]");
                        Console.WriteLine($">> Conversation item input audio transcription completed.");

                        Console.WriteLine();
                        Console.WriteLine($"++ [USER]:");
                        Console.WriteLine($"{conversationItemInputAudioTranscriptionCompletedUpdate.Transcript}");
                        Console.WriteLine();
                        break;
                    }
                case RealtimeServerUpdateResponseOutputAudioDelta responseOutputAudioDeltaUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {responseOutputAudioDeltaUpdate.EventId}]");
                        Console.WriteLine($">> Response output audio delta. Bytes: {responseOutputAudioDeltaUpdate.Delta.Length}.");

                        outputAudioStream.Write(responseOutputAudioDeltaUpdate.Delta);

                        break;
                    }
                case RealtimeServerUpdateResponseOutputAudioDone responseOutputAudioDoneUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {responseOutputAudioDoneUpdate.EventId}]");
                        Console.WriteLine($">> Response output audio done. Bytes: {outputAudioStream.Length}.");
                        Console.WriteLine();
                        break;
                    }
                case RealtimeServerUpdateResponseOutputAudioTranscriptDone responseOutputAudioTranscriptionDoneUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {responseOutputAudioTranscriptionDoneUpdate.EventId}]");
                        Console.WriteLine($">> Response output audio transcription done.");

                        Console.WriteLine();
                        Console.WriteLine($"++ [ASSISTANT]:");
                        Console.WriteLine($"{responseOutputAudioTranscriptionDoneUpdate.Transcript}");
                        Console.WriteLine();
                        break;
                    }
                case RealtimeServerUpdateResponseDone responseDoneUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {responseDoneUpdate.EventId}]");
                        Console.WriteLine($">> Response done. Status: {responseDoneUpdate.Response.Status}.");

                        bool hasToolCalls = false;

                        List<RealtimeFunctionCallItem> functionCallItems = responseDoneUpdate.Response.OutputItems
                            .OfType<RealtimeFunctionCallItem>()
                            .ToList();

                        foreach (RealtimeFunctionCallItem functionCallItem in functionCallItems)
                        {
                            hasToolCalls = true;

                            Console.WriteLine($">> Calling {functionCallItem.FunctionName} function...");

                            string output = GetCurrentWeather(location: "San Francisco, CA");

                            RealtimeItem functionCallOutputItem = RealtimeItem.CreateFunctionCallOutputItem(
                                callId: functionCallItem.CallId,
                                functionOutput: output);

                            Console.WriteLine($">> Adding function call output item...");

                            await sessionClient.AddItemAsync(functionCallOutputItem);
                        }

                        if (hasToolCalls)
                        {
                            // If we need the model to process the output of a tool call, we instruct
                            // the server to create another responses.
                            Console.WriteLine($">> Requesting follow up response...");

                            await sessionClient.StartResponseAsync();
                        }
                        else
                        {
                            done = true;
                            break;
                        }

                        Console.WriteLine();
                        break;
                    }
                case RealtimeServerUpdateError errorUpdate:
                    {
                        Console.WriteLine($"[EVENT ID: {errorUpdate.EventId}]");
                        Console.WriteLine($"Error: {errorUpdate.Error.Message}");

                        done = true;

                        Console.WriteLine();
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }
    }
}

#pragma warning restore OPENAI002