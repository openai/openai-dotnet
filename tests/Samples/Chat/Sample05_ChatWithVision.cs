using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenAI.Samples
{
    public partial class ChatSamples
    {
        [Test]
        [Ignore("Compilation validation only")]
        public void Sample05_ChatWithVision()
        {
            ChatClient client = new("gpt-4-vision-preview", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            string imageFilename = "variation_sample_image.png";
            string imagePath = Path.Combine("Assets", imageFilename);
            using Stream imageStream = File.OpenRead(imagePath);
            BinaryData image = BinaryData.FromStream(imageStream);

            List<ChatMessage> messages = [
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextMessageContentPart("Please describe the following image."),
                    ChatMessageContentPart.CreateImageMessageContentPart(image, "image/png"))
            ];

            ChatCompletion chatCompletion = client.CompleteChat(messages);

            Console.WriteLine($"[ASSISTANT]:");
            Console.WriteLine($"{chatCompletion.Content[0].Text}");
        }
    }
}