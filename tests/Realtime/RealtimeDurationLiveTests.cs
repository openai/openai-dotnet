using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.ClientModel.Primitives;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI002

[LiveOnly(Reason = "Test framework doesn't support recording with web sockets yet")]
public class RealtimeDurationLiveTests : RealtimeTestFixtureBase
{
    public RealtimeDurationLiveTests(bool isAsync) : base(isAsync, RecordedTestMode.Live) { }

    [Test]
    public async Task SpeechStartedAndStopped_DurationsAreWholeMilliseconds()
    {
        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        // Send audio with server VAD (default) to trigger SpeechStarted and SpeechStopped
        string inputAudioFilePath = Path.Join("Assets", "realtime_whats_the_weather_pcm16_24khz_mono.wav");
        using Stream audioStream = File.OpenRead(inputAudioFilePath);
        await sessionClient.SendInputAudioAsync(audioStream, CancellationToken);

        TimeSpan? speechStartTime = null;
        TimeSpan? speechEndTime = null;
        bool done = false;

        await foreach (RealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case RealtimeServerUpdateInputAudioBufferSpeechStarted speechStarted:
                    speechStartTime = speechStarted.AudioStartTime;
                    break;
                case RealtimeServerUpdateInputAudioBufferSpeechStopped speechStopped:
                    speechEndTime = speechStopped.AudioEndTime;
                    break;
                case RealtimeServerUpdateResponseDone:
                    done = true;
                    break;
                case RealtimeServerUpdateError errorUpdate:
                    Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                    done = true;
                    break;
            }

            if (done) break;
        }

        Assert.That(speechStartTime, Is.Not.Null, "Should have received SpeechStarted");
        Assert.That(speechEndTime, Is.Not.Null, "Should have received SpeechStopped");

        // Verify durations are whole milliseconds (no sub-ms fractional component)
        Assert.That(speechStartTime.Value.Ticks % TimeSpan.TicksPerMillisecond, Is.EqualTo(0),
            $"AudioStartTime {speechStartTime} should be a whole number of milliseconds");
        Assert.That(speechEndTime.Value.Ticks % TimeSpan.TicksPerMillisecond, Is.EqualTo(0),
            $"AudioEndTime {speechEndTime} should be a whole number of milliseconds");
    }

    [Test]
    public async Task ConversationItemTruncated_AudioEndTimeIsWholeMilliseconds()
    {
        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        // Configure session to produce audio output so we can truncate it
        RealtimeConversationSessionOptions sessionOptions = new()
        {
            AudioOptions = new RealtimeConversationSessionAudioOptions()
            {
                InputAudioOptions = new(),
                OutputAudioOptions = new()
                {
                    Voice = RealtimeVoice.Alloy,
                }
            },
            Instructions = "You are a helpful assistant. Always respond with audio.",
        };

        sessionOptions.AudioOptions.InputAudioOptions.DisableTurnDetection();

        await sessionClient.ConfigureConversationSessionAsync(sessionOptions, CancellationToken);

        // Add a text message and request an audio response to ensure audio content is generated
        RealtimeInputTextMessageContentPart inputTextPart = new("Tell me a short joke.");
        RealtimeMessageItem userMessageItem = new(RealtimeMessageRole.User, [inputTextPart]);
        await sessionClient.AddItemAsync(userMessageItem, CancellationToken);
        await sessionClient.StartResponseAsync(CancellationToken);

        string responseItemId = null;
        bool gotResponseDone = false;
        TimeSpan? truncatedAudioEndTime = null;
        bool done = false;

        await foreach (RealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            switch (update)
            {
                case RealtimeServerUpdateResponseDone responseDone:
                    {
                        // Find the assistant audio output item to truncate
                        foreach (var item in responseDone.Response.OutputItems)
                        {
                            if (item is RealtimeMessageItem messageItem)
                            {
                                responseItemId = messageItem.Id;
                            }
                        }

                        if (responseItemId is not null)
                        {
                            // Truncate at 500ms
                            await sessionClient.TruncateItemAsync(
                                responseItemId,
                                contentPartIndex: 0,
                                audioDuration: TimeSpan.FromMilliseconds(500),
                                CancellationToken);

                            gotResponseDone = true;
                        }
                        else
                        {
                            Assert.Fail("No output item found in response to truncate");
                            done = true;
                        }

                        break;
                    }
                case RealtimeServerUpdateConversationItemTruncated truncated:
                    {
                        truncatedAudioEndTime = truncated.AudioEndTime;
                        done = true;
                        break;
                    }
                case RealtimeServerUpdateError errorUpdate:
                    Assert.Fail($"Error: {ModelReaderWriter.Write(errorUpdate)}");
                    done = true;
                    break;
            }

            if (done) break;
        }

        Assert.That(gotResponseDone, Is.True, "Should have received ResponseDone");
        Assert.That(truncatedAudioEndTime, Is.Not.Null, "Should have received ConversationItemTruncated");

        // Verify the truncated duration is a whole number of milliseconds
        Assert.That(truncatedAudioEndTime.Value.Ticks % TimeSpan.TicksPerMillisecond, Is.EqualTo(0),
            $"AudioEndTime {truncatedAudioEndTime} should be a whole number of milliseconds");
    }
}
