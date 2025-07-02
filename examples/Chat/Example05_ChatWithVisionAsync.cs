using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public async Task Example05_ChatWithVisionAsync()
    {
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string imageFilePath = Path.Combine("Assets", "images_dog_and_cat.png");
        using Stream imageStream = File.OpenRead(imageFilePath);
        BinaryData imageBytes = BinaryData.FromStream(imageStream);

        List<ChatMessage> messages =
        [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart("Please describe the following image."),
                ChatMessageContentPart.CreateImagePart(imageBytes, "image/png")),
        ];

        ChatCompletion completion = await client.CompleteChatAsync(messages);

        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
    }
}