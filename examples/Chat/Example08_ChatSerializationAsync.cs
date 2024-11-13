using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public async Task Example08_ChatSerializationAsync()
    {
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        BinaryData serializedData = BinaryData.FromBytes("""
            [
                {
                  "role": "user",
                  "content": "Who won the world series in 2020?"
                },
                {
                  "role": "assistant",
                  "content": "The Los Angeles Dodgers won the World Series in 2020."
                },
                {
                  "role": "user",
                  "content": "Where was it played?"
                }
            ]
            """u8.ToArray());

        List<ChatMessage> messages = DeserializeMessages(serializedData).ToList();

        ChatCompletion completion = await client.CompleteChatAsync(messages);

        messages.Add(new AssistantChatMessage(completion));

        foreach (ChatMessage message in messages)
        {
            switch (message)
            {
                case UserChatMessage userMessage:
                    Console.WriteLine($"[USER]:");
                    Console.WriteLine($"{message.Content[0].Text}");
                    Console.WriteLine();
                    break;

                case AssistantChatMessage assistantMessage when assistantMessage.Content.Count > 0:
                    Console.WriteLine($"[ASSISTANT]:");
                    Console.WriteLine($"{assistantMessage.Content[0].Text}");
                    Console.WriteLine();
                    break;

                default:
                    break;
            }
        }

        serializedData = SerializeMessages(messages);

        Console.WriteLine("****************************************************");
        Console.WriteLine();
        Console.WriteLine(serializedData.ToString());
    }
}
