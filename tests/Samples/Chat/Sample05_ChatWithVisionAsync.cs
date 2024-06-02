using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Samples;

public partial class ChatSamples
{
    [Test]
    public async Task Sample05_ChatWithVisionAsync()
    {
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string imageFilename = "variation_sample_image.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        using Stream imageStream = File.OpenRead(imagePath);
        BinaryData image = BinaryData.FromStream(imageStream);

        List<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextMessageContentPart("Please describe the following image."),
                ChatMessageContentPart.CreateImageMessageContentPart(image, "image/png"))
        ];

        ChatCompletion chatCompletion = await client.CompleteChatAsync(messages);

        Console.WriteLine($"[ASSISTANT]:");
        Console.WriteLine($"{chatCompletion.Content[0].Text}");
    }
}