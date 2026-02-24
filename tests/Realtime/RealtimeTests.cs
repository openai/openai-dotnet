using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.Buffers;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI002

[LiveOnly(Reason = "Test framework doesn't support recording with web sockets yet")]
public class RealtimeTests : RealtimeTestFixtureBase
{
    public enum TestAudioSendType { WithAudioStreamHelper, WithManualAudioChunks }

    public RealtimeTests(bool isAsync) : base(isAsync, RecordedTestMode.Live) { }

    [Test]
    public async Task CanConfigureSession()
    {
        RealtimeClient client = GetTestClient();

        // Step 1: Create a default session.
        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        // Step 2: Update the session.
        string instructions = "You are a helpful assistant.";
        int maxOutputTokenCount = 2048;
        GARealtimeConversationSessionOptions sessionOptions = new()
        {
            AudioOptions = new GARealtimeConversationSessionAudioOptions()
            {
                InputAudioOptions = new(),
                OutputAudioOptions = new()
                {
                    AudioFormat = new GARealtimePcmuAudioFormat(),
                    Voice = GARealtimeVoice.Echo,
                }
            },

            Instructions = instructions,

            MaxOutputTokenCount = maxOutputTokenCount,
        };

        sessionOptions.AudioOptions.InputAudioOptions.DisableTurnDetection();

        await sessionClient.ConfigureConversationSessionAsync(sessionOptions, CancellationToken);

        // Step 3: Add a message item (role = user, kind = input text)
        string userMessage = "Hello, assistant! Tell me a joke.";
        GARealtimeInputTextMessageContentPart inputTextPart = new(userMessage);
        GARealtimeMessageItem userMessageItem = new(GARealtimeMessageRole.User, [inputTextPart]);

        await sessionClient.AddItemAsync(userMessageItem, CancellationToken);

        // Step 4: Request a text response (no audio).
        GARealtimeResponseOptions responseOptions = new()
        {
            OutputModalities = { GARealtimeOutputModality.Text },
        };

        await sessionClient.StartResponseAsync(responseOptions, CancellationToken);

        bool done = false;
        List<GARealtimeServerUpdate> receivedUpdates = [];

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            receivedUpdates.Add(update);

            switch (update)
            {
                case GARealtimeServerUpdateSessionCreated sessionCreatedUpdate:
                    {
                        // Validate default values.
                        GARealtimeConversationSession session = sessionCreatedUpdate.Session as GARealtimeConversationSession;
                        Assert.That(session, Is.Not.Null);
                        Assert.That(session.AudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.AudioFormat, Is.TypeOf<GARealtimePcmAudioFormat>());
                        Assert.That(session.AudioOptions.InputAudioOptions.AudioTranscriptionOptions, Is.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.NoiseReduction, Is.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.TurnDetection, Is.TypeOf<GARealtimeServerVadTurnDetection>());
                        Assert.That(session.AudioOptions.OutputAudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.OutputAudioOptions.AudioFormat, Is.TypeOf<GARealtimePcmAudioFormat>());
                        Assert.That(session.AudioOptions.OutputAudioOptions.Speed, Is.EqualTo(1));
                        Assert.That(session.AudioOptions.OutputAudioOptions.Voice, Is.EqualTo(GARealtimeVoice.Alloy));
                        Assert.That(session.Instructions, Is.Not.Null.Or.Empty);
                        Assert.That(session.MaxOutputTokenCount.DefaultMaxOutputTokenCount, Is.EqualTo(GARealtimeDefaultMaxOutputTokenCount.Infinity));
                        Assert.That(session.MaxOutputTokenCount.CustomMaxOutputTokenCount, Is.Null);
                        Assert.That(session.Model, Is.Not.Null.Or.Empty);
                        Assert.That(session.OutputModalities, Is.Not.Null);
                        Assert.That(session.OutputModalities, Has.Count.EqualTo(1));
                        Assert.That(session.OutputModalities[0], Is.EqualTo(GARealtimeOutputModality.Audio));
                        Assert.That(session.ToolChoice.DefaultToolChoice, Is.EqualTo(GARealtimeDefaultToolChoice.Auto));
                        Assert.That(session.ToolChoice.CustomToolChoice, Is.Null);
                        Assert.That(session.Tools, Is.Not.Null);
                        Assert.That(session.Tools, Has.Count.EqualTo(0));
                        Assert.That(session.Tracing, Is.Null);
                        Assert.That(session.Truncation.DefaultTruncation, Is.EqualTo(GARealtimeDefaultTruncation.Auto));
                        Assert.That(session.Truncation.CustomTruncation, Is.Null);
                        break;
                    }
                case GARealtimeServerUpdateSessionUpdated sessionUpdatedUpdate:
                    {
                        // Validate updates.
                        GARealtimeConversationSession session = sessionUpdatedUpdate.Session as GARealtimeConversationSession;
                        Assert.That(session, Is.Not.Null);
                        Assert.That(session.AudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.TurnDetection, Is.Null);
                        Assert.That(session.AudioOptions.OutputAudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.OutputAudioOptions.AudioFormat, Is.TypeOf<GARealtimePcmuAudioFormat>());
                        Assert.That(session.AudioOptions.OutputAudioOptions.Voice, Is.EqualTo(GARealtimeVoice.Echo));
                        Assert.That(session.Instructions, Is.EqualTo(instructions));
                        Assert.That(session.MaxOutputTokenCount.DefaultMaxOutputTokenCount, Is.Null);
                        Assert.That(session.MaxOutputTokenCount.CustomMaxOutputTokenCount, Is.EqualTo(maxOutputTokenCount));
                        break;
                    }
                case GARealtimeServerUpdateConversationItemAdded conversationItemAddedUpdate:
                    {
                        Assert.That(conversationItemAddedUpdate.Item, Is.TypeOf<GARealtimeMessageItem>());
                        break;
                    }
                case GARealtimeServerUpdateConversationItemDone conversationItemDoneUpdate:
                    {
                        Assert.That(conversationItemDoneUpdate.Item, Is.TypeOf<GARealtimeMessageItem>());
                        break;
                    }
                case GARealtimeServerUpdateResponseOutputItemAdded responseOutputItemAddedUpdate:
                    {
                        GARealtimeMessageItem messageItem = responseOutputItemAddedUpdate.Item as GARealtimeMessageItem;
                        Assert.That(messageItem, Is.Not.Null);
                        Assert.That(messageItem.Status, Is.EqualTo(GARealtimeMessageStatus.InProgress));
                        Assert.That(messageItem.Content, Has.Count.EqualTo(0));
                        break;
                    }
                case GARealtimeServerUpdateResponseOutputItemDone responseOutputItemDoneUpdate:
                    {
                        GARealtimeMessageItem messageItem = responseOutputItemDoneUpdate.Item as GARealtimeMessageItem;
                        Assert.That(messageItem, Is.Not.Null);
                        Assert.That(messageItem.Status, Is.EqualTo(GARealtimeMessageStatus.Completed));
                        Assert.That(messageItem.Content, Has.Count.EqualTo(1));
                        break;
                    }
                case GARealtimeServerUpdateResponseContentPartAdded responseContentPartAddedUpdate:
                    {
                        Assert.That(responseContentPartAddedUpdate.Part.Kind, Is.EqualTo(GARealtimeResponseContentPartKind.Text));
                        break;
                    }
                case GARealtimeServerUpdateResponseContentPartDone responseContentPartDoneUpdate:
                    {
                        Assert.That(responseContentPartDoneUpdate.Part, Is.Not.Null);
                        Assert.That(responseContentPartDoneUpdate.Part.Kind, Is.EqualTo(GARealtimeResponseContentPartKind.Text));
                        Assert.That(responseContentPartDoneUpdate.Part.Text, Is.Not.Null.Or.Empty);
                        Assert.That(responseContentPartDoneUpdate.Part.Audio, Is.Null);
                        Assert.That(responseContentPartDoneUpdate.Part.Transcript, Is.Null);
                        break;
                    }
                case GARealtimeServerUpdateResponseOutputTextDelta responseOutputTextDeltaUpdate:
                    {
                        Assert.That(responseOutputTextDeltaUpdate.Delta, Is.Not.Null);
                        break;
                    }
                case GARealtimeServerUpdateResponseOutputTextDone responseOutputTextDoneUpdate:
                    {
                        Assert.That(responseOutputTextDoneUpdate.Text, Is.Not.Null.Or.Empty);
                        break;
                    }
                case GARealtimeServerUpdateResponseCreated responseCreatedUpdate:
                    {
                        Assert.That(responseCreatedUpdate.Response, Is.Not.Null);
                        Assert.That(responseCreatedUpdate.Response.Status, Is.EqualTo(GARealtimeResponseStatus.InProgress));
                        Assert.That(responseCreatedUpdate.Response.OutputModalities, Is.Not.Null);
                        Assert.That(responseCreatedUpdate.Response.OutputModalities, Has.Count.EqualTo(1));
                        Assert.That(responseCreatedUpdate.Response.OutputModalities[0], Is.EqualTo(GARealtimeOutputModality.Text));
                        break;
                    }
                case GARealtimeServerUpdateResponseDone responseDoneUpdate:
                    {
                        done = true;
                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        // Only one session is created.
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateSessionCreated>().ToList(), Has.Count.EqualTo(1));

        // The session is also updated once.
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateSessionUpdated>().ToList(), Has.Count.EqualTo(1));

        // This is a single-turn conversation, which means there is only one response from the model.
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateResponseCreated>().ToList(), Has.Count.EqualTo(1));
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateResponseDone>().ToList(), Has.Count.EqualTo(1));

        // In total, there is 1 user message item and 1 assistant message item.
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateConversationItemAdded>().ToList(), Has.Count.EqualTo(2));
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateConversationItemDone>().ToList(), Has.Count.EqualTo(2));

        // Only one output text item is generated by the model in their response.
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateResponseOutputItemAdded>().ToList(), Has.Count.EqualTo(1));
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateResponseOutputItemDone>().ToList(), Has.Count.EqualTo(1));
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateResponseContentPartAdded>().ToList(), Has.Count.EqualTo(1));
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateResponseContentPartDone>().ToList(), Has.Count.EqualTo(1));
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateResponseOutputTextDelta>().ToList(), Has.Count.GreaterThan(1));
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateResponseOutputTextDone>().ToList(), Has.Count.EqualTo(1));

        // Response was requested as text (no audio).
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateResponseOutputAudioDelta>().ToList(), Has.Count.EqualTo(0));
        Assert.That(receivedUpdates.OfType<GARealtimeServerUpdateResponseOutputAudioDone>().ToList(), Has.Count.EqualTo(0));
    }

    [Test]
    public async Task TextOnlyWorks()
    {
        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        string userInputText = "Hello, world!";

        await sessionClient.AddItemAsync(GARealtimeItem.CreateUserMessageItem(userInputText), CancellationToken);

        await sessionClient.StartResponseAsync(CancellationToken);

        string responseContent = null;
        bool gotSessionCreated = false;
        bool gotResponseDone = false;
        bool done = false;

        List<GARealtimeServerUpdate> receivedUpdates = [];

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            receivedUpdates.Add(update);

            switch (update)
            {
                case GARealtimeServerUpdateSessionCreated sessionCreatedUpdate:
                    {
                        GARealtimeConversationSession session = sessionCreatedUpdate.Session as GARealtimeConversationSession;
                        Assert.That(session, Is.Not.Null);

                        gotSessionCreated = true;
                        break;
                    }
                case GARealtimeServerUpdateConversationItemCreated conversationItemCreated:
                    {
                        GARealtimeMessageItem messageItem = conversationItemCreated.Item as GARealtimeMessageItem;
                        Assert.That(messageItem, Is.Not.Null);

                        if (messageItem.Role == GARealtimeMessageRole.Assistant)
                        {
                            // The assistant-created item should be streamed and should not have content yet when acknowledged.
                            Assert.That(messageItem.Content, Has.Count.EqualTo(0));
                        }
                        else if (messageItem.Role == GARealtimeMessageRole.User)
                        {
                            // When acknowledging an item added by the client (user), the text should already be there
                            Assert.That(messageItem.Content, Has.Count.EqualTo(1));
                            Assert.That((messageItem.Content[0] as GARealtimeInputTextMessageContentPart)?.Text, Is.EqualTo(userInputText));
                        }
                        else
                        {
                            Assert.Fail($"Error: Unexpected item.");
                        }

                        break;
                    }
                case GARealtimeServerUpdateRateLimitsUpdated rateLimitsUpdatedUpdate:
                    {
                        Assert.That(rateLimitsUpdatedUpdate.RateLimitDetails, Has.Count.EqualTo(2));
                        Assert.That(rateLimitsUpdatedUpdate.RateLimitDetails[0].Name, Is.EqualTo(GARealtimeRateLimitName.Tokens));
                        Assert.That(rateLimitsUpdatedUpdate.RateLimitDetails[0].Limit, Is.GreaterThan(0));
                        Assert.That(rateLimitsUpdatedUpdate.RateLimitDetails[0].RemainingCount, Is.GreaterThan(0));
                        Assert.That(rateLimitsUpdatedUpdate.RateLimitDetails[0].RemainingCount, Is.LessThan(rateLimitsUpdatedUpdate.RateLimitDetails[0].Limit));
                        Assert.That(rateLimitsUpdatedUpdate.RateLimitDetails[0].TimeUntilReset, Is.GreaterThan(TimeSpan.Zero));
                        Assert.That(rateLimitsUpdatedUpdate.RateLimitDetails[1].Name, Is.EqualTo(GARealtimeRateLimitName.Requests));

                        break;
                    }
                case GARealtimeServerUpdateResponseDone responseDoneUpdate:
                    {
                        Assert.That(responseDoneUpdate.Response.OutputItems, Has.Count.EqualTo(1));

                        GARealtimeMessageItem messageItem = responseDoneUpdate.Response.OutputItems[0] as GARealtimeMessageItem;
                        Assert.That(messageItem, Is.Not.Null);
                        Assert.That(messageItem.Content, Has.Count.EqualTo(1));

                        responseContent = (messageItem.Content[0] as GARealtimeOutputAudioMessageContentPart)?.Transcript;

                        Assert.That(responseDoneUpdate.Response.Usage.TotalTokenCount, Is.GreaterThan(0));
                        Assert.That(responseDoneUpdate.Response.Usage.InputTokenCount, Is.GreaterThan(0));
                        Assert.That(responseDoneUpdate.Response.Usage.OutputTokenCount, Is.GreaterThan(0));

                        gotResponseDone = true;
                        done = true;
                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        Assert.That(responseContent, Is.Not.Null.Or.Empty);

        Assert.That(gotSessionCreated, Is.True);
        // Assert.That(gotRateLimitsUpdated, Is.True); // Server is not sending rate_limits.updated event.
        Assert.That(gotResponseDone, Is.True);
    }

    [Test]
    public async Task TranscriptionOnlyWorks()
    {
        RealtimeClient client = GetTestClient();

        // Step 1: Create default session.
        using RealtimeSessionClient sessionClient = await client.StartTranscriptionSessionAsync(
            cancellationToken: CancellationToken);

        GARealtimeTranscriptionSessionOptions options = new()
        {
            AudioOptions = new()
            {
                InputAudioOptions = new()
                {
                    AudioFormat = new GARealtimePcmAudioFormat(),

                    AudioTranscriptionOptions = new()
                    {
                        Model = "gpt-4o-transcribe",
                        Prompt = "The audio contains the word 'URL'."
                    },

                    NoiseReduction = new GARealtimeNoiseReduction(GARealtimeNoiseReductionKind.NearField),

                    TurnDetection = new GARealtimeServerVadTurnDetection()
                }
            }
        };

        await sessionClient.ConfigureTranscriptionSessionAsync(options, CancellationToken);

        // Sending the audio in a delayed stream allows us to validate bidirectional behavior, i.e.
        // transcription data arriving while audio is still being sent.
        string inputPath = Path.Join("Assets", "realtime_api_description_pcm16_24khz_mono.wav");
        using TestDelayedFileReadStream inputStream = new(inputPath, TimeSpan.FromMilliseconds(50), readsBeforeDelay: 2);
        _ = sessionClient.SendInputAudioAsync(inputStream, CancellationToken);

        Stopwatch stopwatch = Stopwatch.StartNew();

        List<(GARealtimeServerUpdate, TimeSpan, long)> updatesReceived = [];

        bool gotSessionCreated = false;
        bool gotSessionUpdated = false;
        bool done = false;

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            updatesReceived.Add((update, stopwatch.Elapsed, inputStream.Position));

            switch (update)
            {
                case GARealtimeServerUpdateSessionCreated sessionCreatedUpdate:
                    {
                        GARealtimeTranscriptionSession session = sessionCreatedUpdate.Session as GARealtimeTranscriptionSession;
                        Assert.That(session, Is.Not.Null);

                        gotSessionCreated = true;
                        break;
                    }
                case GARealtimeServerUpdateSessionUpdated sessionUpdatedUpdate:
                    {
                        GARealtimeTranscriptionSession session = sessionUpdatedUpdate.Session as GARealtimeTranscriptionSession;
                        Assert.That(session, Is.Not.Null);
                        Assert.That(session.AudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.AudioTranscriptionOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.AudioTranscriptionOptions.Model, Is.Not.Null.Or.Empty);

                        gotSessionUpdated = true;
                        break;
                    }
                case GARealtimeServerUpdateConversationItemInputAudioTranscriptionCompleted conversationItemInputAudioTranscriptionCompletedUpdate:
                    {
                        if (conversationItemInputAudioTranscriptionCompletedUpdate.Transcript.Contains("the following"))
                        {
                            done = true;
                        }
                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        List<(GARealtimeServerUpdateSessionUpdated, TimeSpan, long)> sessionUpdatedUpdates = updatesReceived
            .Where(tuple => tuple.Item1 is GARealtimeServerUpdateSessionUpdated)
            .Select(tuple => (tuple.Item1 as GARealtimeServerUpdateSessionUpdated, tuple.Item2, tuple.Item3))
            .ToList();

        Assert.That(sessionUpdatedUpdates, Has.Count.EqualTo(1));

        List<(GARealtimeServerUpdateConversationItemInputAudioTranscriptionDelta, TimeSpan, long)> transcriptionDeltaUpdates = updatesReceived
            .Where(tuple => tuple.Item1 is GARealtimeServerUpdateConversationItemInputAudioTranscriptionDelta)
            .Select(tuple => (tuple.Item1 as GARealtimeServerUpdateConversationItemInputAudioTranscriptionDelta, tuple.Item2, tuple.Item3))
            .ToList();

        Assert.That(transcriptionDeltaUpdates, Has.Count.GreaterThan(1));
        Assert.That(transcriptionDeltaUpdates.Last().Item3, Is.GreaterThan(transcriptionDeltaUpdates.First().Item3));

        List<(GARealtimeServerUpdateConversationItemInputAudioTranscriptionCompleted, TimeSpan, long)> transcriptionCompletedUpdates = updatesReceived
            .Where(tuple => tuple.Item1 is GARealtimeServerUpdateConversationItemInputAudioTranscriptionCompleted)
            .Select(tuple => (tuple.Item1 as GARealtimeServerUpdateConversationItemInputAudioTranscriptionCompleted, tuple.Item2, tuple.Item3))
            .ToList();

        Assert.That(transcriptionCompletedUpdates, Has.Count.GreaterThan(1));
        Assert.That(transcriptionCompletedUpdates.Last().Item3, Is.GreaterThan(transcriptionCompletedUpdates.First().Item3));

        List<(GARealtimeServerUpdateConversationItemInputAudioTranscriptionDelta, TimeSpan, long)> deltaUpdatesAfterFirstCompletedUpdate = transcriptionDeltaUpdates
            .Where(tuple => tuple.Item2 > transcriptionCompletedUpdates[0].Item2)
            .ToList();

        Assert.That(transcriptionCompletedUpdates[1].Item1.Transcript.StartsWith(deltaUpdatesAfterFirstCompletedUpdate[0].Item1.Delta), Is.True);

        string fullTranscriptFromDeltas = string.Join(string.Empty, transcriptionDeltaUpdates.Select(deltaTuple => deltaTuple.Item1.Delta));
        Assert.That(fullTranscriptFromDeltas.ToLowerInvariant(), Does.Contain("stream the transcription"));
        Assert.That(fullTranscriptFromDeltas.ToLowerInvariant(), Does.Contain("the following"));

        Assert.That(gotSessionCreated, Is.True);
        Assert.That(gotSessionUpdated, Is.True);
    }

    [Test]
    public async Task ItemManipulationWorks()
    {
        RealtimeClient client = GetTestClient();

        // Step 1: Create default session.
        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        int maxOutputTokenCount = 4096;

        // Step 2: Update the session.
        GARealtimeConversationSessionOptions sessionOptions = new()
        {
            OutputModalities = { GARealtimeOutputModality.Text },
            MaxOutputTokenCount = maxOutputTokenCount,
        };

        sessionOptions.AudioOptions = new();
        sessionOptions.AudioOptions.InputAudioOptions = new();
        sessionOptions.AudioOptions.InputAudioOptions.DisableTurnDetection();

        await sessionClient.ConfigureConversationSessionAsync(sessionOptions, CancellationToken);

        // Step 3: Add multiple items.
        string inputText1 = "The first special word you know about is 'aardvark'.";
        GARealtimeMessageItem message1 = GARealtimeItem.CreateUserMessageItem(inputText1);
        await sessionClient.AddItemAsync(message1, CancellationToken);

        string inputText2 = "The next special word you know about is 'banana'.";
        GARealtimeMessageItem message2 = GARealtimeItem.CreateUserMessageItem(inputText2);
        await sessionClient.AddItemAsync(message2, CancellationToken);

        string inputText3 = "The next special word you know about is 'coconut'.";
        GARealtimeMessageItem message3 = GARealtimeItem.CreateUserMessageItem(inputText3);
        await sessionClient.AddItemAsync(message3, CancellationToken);

        bool done = false;
        bool gotSessionCreated = false;
        bool gotSessionUpdated = false;
        bool gotItemDeleted = false;
        bool gotResponseDone = false;

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case GARealtimeServerUpdateSessionCreated sessionCreatedUpdate:
                    {
                        Assert.That(sessionCreatedUpdate.Session, Is.Not.Null);

                        gotSessionCreated = true;
                        break;
                    }
                case GARealtimeServerUpdateSessionUpdated sessionUpdatedUpdate:
                    {
                        GARealtimeConversationSession session = sessionUpdatedUpdate.Session as GARealtimeConversationSession;
                        Assert.That(session, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.TurnDetection, Is.Null);
                        Assert.That(session.OutputModalities, Is.Not.Null);
                        Assert.That(session.OutputModalities, Has.Count.EqualTo(1));
                        Assert.That(session.OutputModalities[0], Is.EqualTo(GARealtimeOutputModality.Text));

                        gotSessionUpdated = true;
                        break;
                    }
                case GARealtimeServerUpdateConversationItemDone conversationItemDoneUpdate:
                    {
                        GARealtimeMessageItem messageItem = conversationItemDoneUpdate.Item as GARealtimeMessageItem;
                        Assert.That(messageItem, Is.Not.Null);

                        if (messageItem.Content.Count > 0)
                        {
                            GARealtimeInputTextMessageContentPart inputTextPart = messageItem.Content[0] as GARealtimeInputTextMessageContentPart;

                            // Step 4: Delete the second item.
                            if (inputTextPart != null && inputTextPart.Text == inputText2)
                            {
                                await sessionClient.DeleteItemAsync(messageItem.Id, CancellationToken);
                            }
                        }

                        break;
                    }
                case GARealtimeServerUpdateConversationItemDeleted conversationItemDeletedUpdate:
                    {
                        // Step 5: Add a new item with a question.
                        string inputText4 = "What's the second special word you know about?";
                        GARealtimeMessageItem message4 = GARealtimeItem.CreateUserMessageItem(inputText4);
                        await sessionClient.AddItemAsync(message4, CancellationToken);

                        // Step 6: Start a response.
                        await sessionClient.StartResponseAsync(CancellationToken);

                        gotItemDeleted = true;
                        break;
                    }
                case GARealtimeServerUpdateResponseDone responseDoneUpdate:
                    {
                        Assert.That(responseDoneUpdate.Response.OutputItems.Count, Is.EqualTo(1));

                        GARealtimeMessageItem messageItem = responseDoneUpdate.Response.OutputItems[0] as GARealtimeMessageItem;
                        Assert.That(messageItem, Is.Not.Null);
                        Assert.That(messageItem.Content.Count, Is.EqualTo(1));

                        GARealtimeOutputTextMessageContentPart outputTextPart = messageItem.Content[0] as GARealtimeOutputTextMessageContentPart;
                        Assert.That(outputTextPart, Is.Not.Null);
                        Assert.That(outputTextPart.Text, Does.Contain("coconut"));
                        Assert.That(outputTextPart.Text, Does.Not.Contain("banana"));

                        gotResponseDone = true;
                        done = true;
                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        Assert.That(gotSessionCreated, Is.True);
        Assert.That(gotSessionUpdated, Is.True);
        Assert.That(gotItemDeleted, Is.True);
        Assert.That(gotResponseDone, Is.True);
    }

    [Test]
    public async Task AudioStreamConvenienceBlocksCorrectly()
    {
        RealtimeClient client = GetTestClient();

        // Step 1: Create default session.
        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        // Step 2: Send input audio.
        string inputAudioFilePath = Path.Join("Assets", "realtime_whats_the_weather_pcm16_24khz_mono.wav");

        using TestDelayedFileReadStream delayedStream = new(
            path: inputAudioFilePath,
            delayBetweenReads: TimeSpan.FromMilliseconds(200),
            readsBeforeDelay: 2);

        _ = sessionClient.SendInputAudioAsync(delayedStream, CancellationToken);

        bool gotSessionCreated = false;
        bool gotSpeechStarted = false;
        bool done = false;

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case GARealtimeServerUpdateSessionCreated sessionCreatedUpdate:
                    {
                        // Validate server VAD turn detection.
                        GARealtimeConversationSession session = sessionCreatedUpdate.Session as GARealtimeConversationSession;
                        Assert.That(session, Is.Not.Null);
                        Assert.That(session.AudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.TurnDetection, Is.TypeOf<GARealtimeServerVadTurnDetection>());

                        gotSessionCreated = true;
                        break;
                    }
                case GARealtimeServerUpdateInputAudioBufferSpeechStarted:
                    {
                        Assert.ThrowsAsync<InvalidOperationException>(
                            async () =>
                            {
                                using MemoryStream dummyStream = new();
                                await sessionClient.SendInputAudioAsync(dummyStream, CancellationToken);
                            },
                            "Sending a Stream while another Stream is being sent should throw!");

                        Assert.ThrowsAsync<InvalidOperationException>(
                            async () =>
                            {
                                BinaryData dummyData = BinaryData.FromString("hello, world! this isn't audio.");
                                await sessionClient.SendInputAudioAsync(dummyData, CancellationToken);
                            },
                            "Sending BinaryData while a Stream is being sent should throw!");

                        gotSpeechStarted = true;
                        done = true;
                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        Assert.That(gotSessionCreated, Is.True);
        Assert.That(gotSpeechStarted, Is.True);
    }

    [Test]
    [TestCase(TestAudioSendType.WithAudioStreamHelper)]
    [TestCase(TestAudioSendType.WithManualAudioChunks)]
    public async Task AudioWithToolsWorks(TestAudioSendType audioSendType)
    {
        string functionName = "get_weather_for_location";

        GARealtimeFunctionTool getWeatherTool = new(functionName)
        {
            FunctionDescription = "Get the current weather in a given location",
            FunctionParameters = BinaryData.FromBytes("""
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
                """u8.ToArray()),
        };

        RealtimeClient client = GetTestClient();

        // Step 1: Create default session.
        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        // Step 2: Update the session.
        string instructions = "Call the available tools if appropriate based on the user's input.";
        int maxOutputTokenCount = 4096;
        GARealtimeConversationSessionOptions sessionOptions = new()
        {
            AudioOptions = new GARealtimeConversationSessionAudioOptions()
            {
                InputAudioOptions = new()
                {
                    AudioTranscriptionOptions = new()
                    {
                        Model = "gpt-4o-transcribe"
                    },
                }
            },

            Instructions = instructions,

            OutputModalities = { GARealtimeOutputModality.Audio },

            Tools = { getWeatherTool },

            MaxOutputTokenCount = maxOutputTokenCount,
        };

        await sessionClient.ConfigureConversationSessionAsync(sessionOptions, CancellationToken);

        // Step 3: Send input audio.
        _ = Task.Run(async () =>
        {
            string inputAudioFilePath = Path.Join("Assets", "realtime_whats_the_weather_pcm16_24khz_mono.wav");

            if (audioSendType == TestAudioSendType.WithAudioStreamHelper)
            {
                using Stream audioStream = File.OpenRead(inputAudioFilePath);

                await sessionClient.SendInputAudioAsync(audioStream, CancellationToken);
            }
            else if (audioSendType == TestAudioSendType.WithManualAudioChunks)
            {
                byte[] allAudioBytes = await File.ReadAllBytesAsync(inputAudioFilePath, CancellationToken);
                const int audioSendBufferLength = 8 * 1024;
                byte[] audioSendBuffer = null;
                try
                {
                    audioSendBuffer = ArrayPool<byte>.Shared.Rent(audioSendBufferLength);
                    for (int readPos = 0; readPos < allAudioBytes.Length; readPos += audioSendBufferLength)
                    {
                        int nextSegmentLength = Math.Min(audioSendBufferLength, allAudioBytes.Length - readPos);
                        ArraySegment<byte> nextSegment = new(allAudioBytes, readPos, nextSegmentLength);

                        await sessionClient.SendInputAudioAsync(BinaryData.FromBytes(nextSegment), CancellationToken);
                    }
                }
                finally
                {
                    if (audioSendBuffer is not null)
                    {
                        ArrayPool<byte>.Shared.Return(audioSendBuffer);
                    }
                }
            }
        });

        string inputAudioTranscript = null;

        bool gotSessionCreated = false;
        bool gotSessionUpdated = false;
        bool gotConversationItemInputAudioTranscriptionCompleted = false;
        bool gotResponseFunctionCallArgumentsDone = false;
        bool gotResponseDoneUpdate = false;
        bool done = false;

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case GARealtimeServerUpdateSessionCreated sessionCreatedUpdate:
                    {
                        // Validate default session.
                        GARealtimeConversationSession session = sessionCreatedUpdate.Session as GARealtimeConversationSession;
                        Assert.That(session, Is.Not.Null);
                        Assert.That(session.Model, Is.Not.Null.And.Not.Empty);

                        gotSessionCreated = true;
                        break;
                    }
                case GARealtimeServerUpdateSessionUpdated sessionUpdatedUpdate:
                    {
                        // Validate output modalities.
                        GARealtimeConversationSession session = sessionUpdatedUpdate.Session as GARealtimeConversationSession;
                        Assert.That(session, Is.Not.Null);
                        Assert.That(session.OutputModalities, Is.Not.Null);
                        Assert.That(session.OutputModalities, Has.Count.EqualTo(1));
                        Assert.That(session.OutputModalities[0], Is.EqualTo(GARealtimeOutputModality.Audio));
                        Assert.That(session.AudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.AudioTranscriptionOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.AudioTranscriptionOptions.Model, Is.Not.Null.Or.Empty);
                        Assert.That(session.Tools, Is.Not.Null);
                        Assert.That(session.Tools, Has.Count.EqualTo(1));
                        Assert.That(session.MaxOutputTokenCount.DefaultMaxOutputTokenCount, Is.Null);
                        Assert.That(session.MaxOutputTokenCount.CustomMaxOutputTokenCount, Is.EqualTo(maxOutputTokenCount));

                        GARealtimeFunctionTool functionTool = session.Tools[0] as GARealtimeFunctionTool;
                        Assert.That(functionTool, Is.Not.Null);
                        Assert.That(functionTool.FunctionName, Is.EqualTo(functionName));

                        gotSessionUpdated = true;
                        break;
                    }
                case GARealtimeServerUpdateConversationItemInputAudioTranscriptionCompleted conversationItemInputAudioTranscriptionCompletedUpdate:
                    {
                        // TODO: Testing input audio transcriptions should be its own test.
                        inputAudioTranscript = conversationItemInputAudioTranscriptionCompletedUpdate.Transcript;

                        gotConversationItemInputAudioTranscriptionCompleted = true;
                        break;
                    }
                case GARealtimeServerUpdateResponseFunctionCallArgumentsDone responseFunctionCallArgumentsDoneUpdate:
                    {
                        Assert.That(responseFunctionCallArgumentsDoneUpdate.FunctionName, Is.EqualTo(getWeatherTool.FunctionName));

                        // Step 3: Provide the output of the function call.
                        GARealtimeItem functionCallOutputItem = GARealtimeItem.CreateFunctionCallOutputItem(
                            callId: responseFunctionCallArgumentsDoneUpdate.CallId,
                            functionOutput: "71 degrees fahrenheit");

                        await sessionClient.AddItemAsync(functionCallOutputItem, CancellationToken);

                        // Step 4: Request a response.
                         await sessionClient.StartResponseAsync(CancellationToken);

                        gotResponseFunctionCallArgumentsDone = true;
                        break;
                    }
                case GARealtimeServerUpdateResponseDone responseDoneUpdate:
                    {
                        if (responseDoneUpdate.Response.OutputItems.OfType<GARealtimeFunctionCallItem>().ToList().Count == 0)
                        {
                            Assert.That(responseDoneUpdate.Response.OutputItems, Has.Count.EqualTo(1));

                            GARealtimeMessageItem messageItem = responseDoneUpdate.Response.OutputItems[0] as GARealtimeMessageItem;
                            Assert.That(messageItem, Is.Not.Null);
                            Assert.That(messageItem.Content, Has.Count.EqualTo(1));

                            GARealtimeOutputAudioMessageContentPart outputAudioPart = messageItem.Content[0] as GARealtimeOutputAudioMessageContentPart;
                            Assert.That(outputAudioPart, Is.Not.Null);
                            Assert.That(outputAudioPart.Transcript, Does.Contain("71"));

                            gotResponseDoneUpdate = true;
                            done = true;
                        }

                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        Assert.That(inputAudioTranscript, Is.Not.Null.And.Not.Empty);

        Assert.That(gotSessionCreated, Is.True);
        Assert.That(gotSessionUpdated, Is.True);
        Assert.That(gotConversationItemInputAudioTranscriptionCompleted, Is.True);
        Assert.That(gotResponseFunctionCallArgumentsDone, Is.True);
        Assert.That(gotResponseDoneUpdate, Is.True);
    }

    [Test]
    public async Task CanDisableVoiceActivityDetection()
    {
        RealtimeClient client = GetTestClient();

        // Step 1: Create a default session.
        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        // Step 2: Update the session.
        GARealtimeConversationSessionOptions sessionOptions = new()
        {
            AudioOptions = new()
            {
                InputAudioOptions = new(),
            },

            MaxOutputTokenCount = 4096,
        };

        sessionOptions.AudioOptions.InputAudioOptions.DisableTurnDetection();

        await sessionClient.ConfigureConversationSessionAsync(sessionOptions, CancellationToken);


        string inputAudioFilePath = Path.Join("Assets", "realtime_whats_the_weather_pcm16_24khz_mono.wav");
        using Stream audioStream = File.OpenRead(inputAudioFilePath);

        await sessionClient.SendInputAudioAsync(audioStream, CancellationToken);


        GARealtimeMessageItem userMessageItem = GARealtimeItem.CreateUserMessageItem("Hello, assistant!");

        await sessionClient.AddItemAsync(userMessageItem, CancellationToken);

        bool done = false;

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case GARealtimeServerUpdateInputAudioBufferSpeechStarted:
                case GARealtimeServerUpdateInputAudioBufferSpeechStopped:
                case GARealtimeServerUpdateInputAudioBufferTimeoutTriggered:
                case GARealtimeServerUpdateInputAudioBufferCommitted:
                case GARealtimeServerUpdateConversationItemInputAudioTranscriptionCompleted:
                case GARealtimeServerUpdateConversationItemInputAudioTranscriptionDelta:
                case GARealtimeServerUpdateConversationItemInputAudioTranscriptionSegment:
                case GARealtimeServerUpdateConversationItemInputAudioTranscriptionFailed:
                case GARealtimeServerUpdateResponseCreated:
                case GARealtimeServerUpdateResponseDone:
                    {
                        Assert.Fail($"Should not receive any VAD events or response creation.");
                        done = true;
                        break;
                    }
                case GARealtimeServerUpdateConversationItemDone conversationItemCreatedDone:
                    {
                        GARealtimeMessageItem messageItem = conversationItemCreatedDone.Item as GARealtimeMessageItem;
                        Assert.That(messageItem, Is.Not.Null);

                        if (messageItem.Role == GARealtimeMessageRole.User)
                        {
                            // TODO: If we exit after the first message, we do not get the chance
                            // to evaluate any other events that come after.
                            done = true;
                        }
                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }
    }

    [Test]
    public async Task BadCommandProvidesError()
    {
        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        BinaryData command = BinaryData.FromString("""
            {
              "type": "update_conversation_config2",
              "event_id": "event_fabricated_1234abcd"
            }
            """);

        await sessionClient.SendCommandAsync(command, CancellationOptions);

        bool gotErrorUpdate = false;
        bool done = false;

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.That(errorUpdate.Error.EventId, Is.EqualTo("event_fabricated_1234abcd"));

                        gotErrorUpdate = true;
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        Assert.That(gotErrorUpdate, Is.True);
    }

    [Test]
    public async Task CanAddItems()
    {
        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        GARealtimeConversationSessionOptions sessionOptions = new()
        {
            OutputModalities = { GARealtimeOutputModality.Text },
            MaxOutputTokenCount = 4096,
        };

        await sessionClient.ConfigureConversationSessionAsync(sessionOptions, CancellationToken);

        List<GARealtimeItem> items =
        [
            GARealtimeItem.CreateSystemMessageItem("You are a robot. Beep boop."),
            GARealtimeItem.CreateUserMessageItem("How can I pay for a joke?"),
            GARealtimeItem.CreateAssistantMessageItem("I ONLY ACCEPT CACHE"),
            GARealtimeItem.CreateSystemMessageItem("You're not a robot anymore, but instead a passionate badminton enthusiast."),
            GARealtimeItem.CreateUserMessageItem("What's a good gift to buy?"),
            GARealtimeItem.CreateFunctionCallItem("call-id-123", "product_lookup", BinaryData.FromString("{}")),
            GARealtimeItem.CreateFunctionCallOutputItem("call-id-123", "A new racquet!")
        ];

        foreach (GARealtimeItem item in items)
        {
            await sessionClient.AddItemAsync(item, CancellationToken);
        }

        await sessionClient.StartResponseAsync(CancellationToken);

        int itemDoneCount = 0;
        bool done = false;

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case GARealtimeServerUpdateConversationItemDone conversationItemDoneUpdate:
                    {
                        itemDoneCount++;
                        break;
                    }
                case GARealtimeServerUpdateResponseDone responseDoneUpdate:
                    {
                        done = true;
                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        Assert.That(itemDoneCount, Is.EqualTo(items.Count + 1));
    }

    [Test]
    public async Task CanUseOutOfBandResponses()
    {
        RealtimeClient client = GetTestClient();

        // Step 1: Create a default session.
        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        // Step 2: Update the session.
        await sessionClient.ConfigureConversationSessionAsync(
            new GARealtimeConversationSessionOptions()
            {
                OutputModalities = { GARealtimeOutputModality.Audio }
            },
            CancellationToken);

        // Step 3: Add an item to the conversation.
        await sessionClient.AddItemAsync(
            GARealtimeItem.CreateUserMessageItem("Hello! My name is Bob."),
            cancellationToken: CancellationToken);

        // Step 4: Request an out-of-band response.
        await sessionClient.StartResponseAsync(
            new GARealtimeResponseOptions()
            {
                DefaultConversationConfiguration = GARealtimeResponseDefaultConversationConfiguration.None,
                OutputModalities = { GARealtimeOutputModality.Text },
                InputItems =
                {
                    GARealtimeItem.CreateUserMessageItem("Can you tell me what my name is?"),
                },
            },
            CancellationToken);

        string firstResponseContent = null;
        string secondResponseContent = null;
        int responseDoneCount = 0;
        bool gotSessionCreated = false;
        bool gotSessionUpdated = false;
        bool done = false;

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case GARealtimeServerUpdateSessionCreated sessionCreatedUpdate:
                    {
                        GARealtimeConversationSession session = sessionCreatedUpdate.Session as GARealtimeConversationSession;
                        Assert.That(session, Is.Not.Null);

                        gotSessionCreated = true;
                        break;
                    }
                case GARealtimeServerUpdateSessionUpdated sessionUpdatedUpdate:
                    {
                        // Validate output modalities.
                        GARealtimeConversationSession session = sessionUpdatedUpdate.Session as GARealtimeConversationSession;
                        Assert.That(session, Is.Not.Null);
                        Assert.That(session.OutputModalities, Is.Not.Null);
                        Assert.That(session.OutputModalities, Has.Count.EqualTo(1));
                        Assert.That(session.OutputModalities[0], Is.EqualTo(GARealtimeOutputModality.Audio));

                        gotSessionUpdated = true;
                        break;
                    }
                case GARealtimeServerUpdateResponseDone responseDoneUpdate:
                    {
                        responseDoneCount++;

                        Assert.That(responseDoneUpdate.Response.OutputItems, Has.Count.EqualTo(1));

                        GARealtimeMessageItem messageItem = responseDoneUpdate.Response.OutputItems[0] as GARealtimeMessageItem;
                        Assert.That(messageItem, Is.Not.Null);
                        Assert.That(messageItem.Content, Has.Count.EqualTo(1));

                        if (responseDoneCount == 1)
                        {
                            // The first response should be out-of-band and be text-only.
                            GARealtimeOutputTextMessageContentPart outputTextPart = messageItem.Content[0] as GARealtimeOutputTextMessageContentPart;
                            Assert.That(outputTextPart, Is.Not.Null);

                            firstResponseContent = outputTextPart.Text;

                            // Step 5: Now request a normal response using the same session.
                            await sessionClient.StartResponseAsync(CancellationToken);
                        }
                        else if (responseDoneCount == 2)
                        {
                            // The second should be a normal response and it should be audio-only.
                            GARealtimeOutputAudioMessageContentPart outputAudioPart = messageItem.Content[0] as GARealtimeOutputAudioMessageContentPart;
                            Assert.That(outputAudioPart, Is.Not.Null);
                            Assert.That(outputAudioPart.Transcript, Is.Not.Null.Or.Empty);

                            secondResponseContent = outputAudioPart.Transcript;

                            done = true;
                        }
                        else
                        {
                            Assert.Fail($"Error: Unexpected response.done event.");
                        }

                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        Assert.That(firstResponseContent, Is.Not.Null.And.Not.Empty);
        Assert.That(firstResponseContent.ToLowerInvariant(), Does.Not.Contain("bob"));

        Assert.That(secondResponseContent, Is.Not.Null.And.Not.Empty);
        Assert.That(secondResponseContent.ToLowerInvariant(), Does.Contain("bob"));

        Assert.That(gotSessionCreated, Is.True);
        Assert.That(gotSessionUpdated, Is.True);
    }


    [Test]
    public async Task CreateConversationSessionClientSecret()
    {
        RealtimeClient client = GetTestClient();

        // Step 1: Create an ephemeral client secret.
        int maxOutputTokenCount = 2048;

        GARealtimeConversationSessionOptions conversationSessionOptions = new()
        {
            AudioOptions = new GARealtimeConversationSessionAudioOptions()
            {
                InputAudioOptions = new(),
            },

            MaxOutputTokenCount = maxOutputTokenCount,

            OutputModalities = { GARealtimeOutputModality.Text },
        };

        conversationSessionOptions.AudioOptions.InputAudioOptions.DisableTurnDetection();

        GACreateClientSecretOptions createClientSecretOptions = new()
        {
            SessionOptions = conversationSessionOptions,
        };

        GACreateClientSecretResult result = client.CreateRealtimeClientSecret(createClientSecretOptions);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Session, Is.Not.Null);
        Assert.That(result.Session, Is.TypeOf<GARealtimeConversationSession>());
        Assert.That(result.ExpiresAt, Is.GreaterThan(DateTimeOffset.Now));

        RealtimeSessionClientOptions sessionClientOptions = new()
        {
            ClientSecret = result.Value,
        };

        // Step 2: Create a session using the ephemeral client secret.
        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            options: sessionClientOptions,
            cancellationToken: CancellationToken);

        // Step 3: Add a user message item.
        await sessionClient.AddItemAsync(
            GARealtimeItem.CreateUserMessageItem("Hello, assistant! Tell me a joke."),
            CancellationToken);

        // Step 4: Request a response.
        await sessionClient.StartResponseAsync(CancellationToken);

        string responseContent = null;
        bool gotSessionCreated = false;
        bool gotResponseDone = false;
        bool done = false;

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case GARealtimeServerUpdateSessionCreated sessionCreatedUpdate:
                    {
                        GARealtimeConversationSession session = sessionCreatedUpdate.Session as GARealtimeConversationSession;
                        Assert.That(session, Is.Not.Null);
                        Assert.That(session.AudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.TurnDetection, Is.Null);
                        Assert.That(session.MaxOutputTokenCount.DefaultMaxOutputTokenCount, Is.Null);
                        Assert.That(session.MaxOutputTokenCount.CustomMaxOutputTokenCount, Is.EqualTo(maxOutputTokenCount));
                        Assert.That(session.OutputModalities, Is.Not.Null);
                        Assert.That(session.OutputModalities, Has.Count.EqualTo(1));
                        Assert.That(session.OutputModalities[0], Is.EqualTo(GARealtimeOutputModality.Text));

                        gotSessionCreated = true;
                        break;
                    }
                case GARealtimeServerUpdateSessionUpdated sessionUpdatedUpdate:
                    {
                        Assert.Fail("Error: Unexpected session.updated event.");
                        break;
                    }
                case GARealtimeServerUpdateResponseDone responseDoneUpdate:
                    {
                        Assert.That(responseDoneUpdate.Response.OutputItems, Has.Count.EqualTo(1));

                        GARealtimeMessageItem messageItem = responseDoneUpdate.Response.OutputItems[0] as GARealtimeMessageItem;
                        Assert.That(messageItem, Is.Not.Null);
                        Assert.That(messageItem.Content, Has.Count.EqualTo(1));

                        responseContent = (messageItem.Content[0] as GARealtimeOutputTextMessageContentPart)?.Text;

                        gotResponseDone = true;
                        done = true;
                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        Assert.That(responseContent, Is.Not.Null.And.Not.Empty);
        Assert.That(gotSessionCreated, Is.True);
        Assert.That(gotResponseDone, Is.True);
    }

    [Test]
    public async Task CreateTranscriptionSessionClientSecret()
    {
        RealtimeClient client = GetTestClient();

        // Step 1: Create an ephemeral client secret.
        string model = "gpt-4o-transcribe";

        GARealtimeTranscriptionSessionOptions transcriptionSessionOptions = new()
        {
            AudioOptions = new()
            {
                InputAudioOptions = new()
                {
                    AudioTranscriptionOptions = new()
                    {
                        Model = model,
                        Prompt = "The audio contains the word 'URL'."
                    },

                    NoiseReduction = new GARealtimeNoiseReduction(GARealtimeNoiseReductionKind.NearField),

                    TurnDetection = new GARealtimeServerVadTurnDetection()
                }
            }
        };

        GACreateClientSecretOptions createClientSecretOptions = new()
        {
            SessionOptions = transcriptionSessionOptions,
        };

        GACreateClientSecretResult result = client.CreateRealtimeClientSecret(createClientSecretOptions);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Session, Is.Not.Null);
        Assert.That(result.Session, Is.TypeOf<GARealtimeTranscriptionSession>());
        Assert.That(result.ExpiresAt, Is.GreaterThan(DateTimeOffset.Now));

        RealtimeSessionClientOptions sessionClientOptions = new()
        {
            ClientSecret = result.Value,
        };

        // Step 2: Create a session using the ephemeral client secret.
        using RealtimeSessionClient sessionClient = await client.StartTranscriptionSessionAsync(
            options: sessionClientOptions,
            cancellationToken: CancellationToken);

        // Sending the audio in a delayed stream allows us to validate bidirectional behavior, i.e.
        // transcription data arriving while audio is still being sent.
        string inputPath = Path.Join("Assets", "realtime_api_description_pcm16_24khz_mono.wav");
        using TestDelayedFileReadStream inputStream = new(inputPath, TimeSpan.FromMilliseconds(50), readsBeforeDelay: 2);
        _ = sessionClient.SendInputAudioAsync(inputStream, CancellationToken);

        List<string> completedTranscripts = new();
        bool gotSessionCreated = false;
        bool done = false;

        await foreach (GARealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case GARealtimeServerUpdateSessionCreated sessionCreatedUpdate:
                    {
                        GARealtimeTranscriptionSession session = sessionCreatedUpdate.Session as GARealtimeTranscriptionSession;
                        Assert.That(session, Is.Not.Null);
                        Assert.That(session.AudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.AudioTranscriptionOptions, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.AudioTranscriptionOptions.Model, Is.EqualTo(model));
                        Assert.That(session.AudioOptions.InputAudioOptions.NoiseReduction, Is.Not.Null);
                        Assert.That(session.AudioOptions.InputAudioOptions.NoiseReduction.Kind, Is.EqualTo(GARealtimeNoiseReductionKind.NearField));
                        Assert.That(session.AudioOptions.InputAudioOptions.TurnDetection, Is.TypeOf<GARealtimeServerVadTurnDetection>());

                        gotSessionCreated = true;
                        break;
                    }
                case GARealtimeServerUpdateSessionUpdated sessionUpdatedUpdate:
                    {
                        Assert.Fail("Error: Unexpected session.updated event.");
                        break;
                    }
                case GARealtimeServerUpdateConversationItemInputAudioTranscriptionCompleted conversationItemInputAudioTranscriptionCompletedUpdate:
                    {
                        completedTranscripts.Add(conversationItemInputAudioTranscriptionCompletedUpdate.Transcript);

                        if (conversationItemInputAudioTranscriptionCompletedUpdate.Transcript.Contains("the following"))
                        {
                            done = true;
                        }

                        break;
                    }
                case GARealtimeServerUpdateError errorUpdate:
                    {
                        Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        Assert.That(completedTranscripts, Has.Count.GreaterThan(0));
        Assert.That(gotSessionCreated, Is.True);
    }

    [Test]
    public async Task CanConfigureSessionWithProtocol()
    {
        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient session = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        BinaryData configureSessionCommand = BinaryData.FromString("""
            {
              "type": "session.update",
              "session": {
                "type": "realtime",
                "audio": {
                    "input": {
                        "turn_detection": null
                    }
                }
              }
            }
            """);

        await session.SendCommandAsync(configureSessionCommand, CancellationOptions);

        List<JsonNode> receivedUpdates = [];

        bool done = false;

        await foreach (ClientResult update in session.ReceiveUpdatesAsync(CancellationOptions))
        {
            BinaryData rawContentBytes = update.GetRawResponse().Content;
            JsonNode jsonNode = JsonNode.Parse(rawContentBytes);
            string updateType = jsonNode["type"]?.GetValue<string>();
            Assert.That(updateType, Is.Not.Null.And.Not.Empty);

            receivedUpdates.Add(jsonNode);

            switch (updateType)
            {
                case "session.updated":
                    {
                        BinaryData createResponseCommand = BinaryData.FromString("""
                        {
                          "type": "response.create"
                        }
                        """);

                        await session.SendCommandAsync(createResponseCommand, CancellationOptions);

                        break;
                    }
                case "response.done":
                    {
                        done = true;
                        break;
                    }
                case "error":
                    {
                        Assert.Fail($"Error: Unexpected error.");
                        done = true;
                        break;
                    }
            }

            if (done)
            {
                break;
            }
        }

        List<JsonNode> NodesOfType(string type) => receivedUpdates.Where(command => command["type"].GetValue<string>() == type).ToList();

        Assert.That(NodesOfType("session.created"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("session.updated"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("response.created"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("response.output_item.added"), Has.Count.EqualTo(1));
        Assert.That(NodesOfType("response.done"), Has.Count.EqualTo(1));
    }

    private class TestDelayedFileReadStream : FileStream
    {
        private readonly TimeSpan _delayBetweenReads;
        private readonly int _readsBeforeDelay;
        private int _readsPerformed;

        public TestDelayedFileReadStream(string path, TimeSpan delayBetweenReads, int readsBeforeDelay = 0) : base(path, FileMode.Open, FileAccess.Read)
        {
            _delayBetweenReads = delayBetweenReads;
            _readsBeforeDelay = readsBeforeDelay;
            _readsPerformed = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (++_readsPerformed > _readsBeforeDelay)
            {
                System.Threading.Thread.Sleep((int)_delayBetweenReads.TotalMilliseconds);
            }

            return base.Read(buffer, offset, count);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (++_readsPerformed > _readsBeforeDelay)
            {
                await Task.Delay(_delayBetweenReads);
            }

            return await base.ReadAsync(buffer, offset, count, cancellationToken);
        }
    }
}
