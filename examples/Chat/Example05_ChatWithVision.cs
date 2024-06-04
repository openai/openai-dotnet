using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public void Example05_ChatWithVision()
    {
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string imageFilePath = Path.Combine("Assets", "images_dog_and_cat.png");
        using Stream imageStream = File.OpenRead(imageFilePath);
        BinaryData imageBytes = BinaryData.FromStream(imageStream);

        List<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextMessageContentPart("Please describe the following image."),
                ChatMessageContentPart.CreateImageMessageContentPart(imageBytes, "image/png"))
        ];

        ChatCompletion chatCompletion = client.CompleteChat(messages);

        Console.WriteLine($"[ASSISTANT]:");
        Console.WriteLine($"{chatCompletion.Content[0].Text}");
    }
}