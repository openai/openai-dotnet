using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ChatExamples
{
    [Test]
    public async Task Example09_ChatWithAudioAsync()
    {
        // Chat audio input and output is only supported on specific models, beginning with gpt-4o-audio-preview
        ChatClient client = new("gpt-4o-audio-preview", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        // Input audio is provided to a request by adding an audio content part to a user message
        string audioFilePath = Path.Combine("Assets", "realtime_whats_the_weather_pcm16_24khz_mono.wav");
        byte[] audioFileRawBytes = await File.ReadAllBytesAsync(audioFilePath);
        BinaryData audioData = BinaryData.FromBytes(audioFileRawBytes);
        List<ChatMessage> messages =
            [
                new UserChatMessage(ChatMessageContentPart.CreateInputAudioPart(audioData, ChatInputAudioFormat.Wav)),
            ];

        // Output audio is requested by configuring ChatCompletionOptions to include the appropriate
        // ResponseModalities values and corresponding AudioOptions.
        ChatCompletionOptions options = new()
        {
            ResponseModalities = ChatResponseModalities.Text | ChatResponseModalities.Audio,
            AudioOptions = new(ChatOutputAudioVoice.Alloy, ChatOutputAudioFormat.Mp3),
        };

        ChatCompletion completion = await client.CompleteChatAsync(messages, options);

        async Task PrintAudioContentAsync()
        {
            if (completion.OutputAudio is ChatOutputAudio outputAudio)
            {
                Console.WriteLine($"Response audio transcript: {outputAudio.Transcript}");
                string outputFilePath = $"{outputAudio.Id}.mp3";
                using (FileStream outputFileStream = File.OpenWrite(outputFilePath))
                {
                    await outputFileStream.WriteAsync(outputAudio.AudioBytes);
                }
                Console.WriteLine($"Response audio written to file: {outputFilePath}");
                Console.WriteLine($"Valid on followup requests until: {outputAudio.ExpiresAt}");
            }
        }

        await PrintAudioContentAsync();

        // To refer to past audio output, create an assistant message from the earlier ChatCompletion, use the earlier
        // response content part, or use ChatMessageContentPart.CreateAudioPart(string) to manually instantiate a part.

        messages.Add(new AssistantChatMessage(completion));
        messages.Add("Can you say that like a pirate?");

        completion = await client.CompleteChatAsync(messages, options);

        await PrintAudioContentAsync();
    }
}

#pragma warning restore OPENAI001