using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    #region
    public static List<ChatMessage> DeserializeMessages(BinaryData data)
    {
        List<ChatMessage> messages = [];
        using JsonDocument messagesAsJson = JsonDocument.Parse(data.ToString());

        foreach (JsonElement jsonElement in messagesAsJson.RootElement.EnumerateArray())
        {
            BinaryData jsonElementAsData = BinaryData.FromString(jsonElement.ToString());
            ChatMessage message = ModelReaderWriter.Read<ChatMessage>(jsonElementAsData, ModelReaderWriterOptions.Json);
            messages.Add(message);
        }

        return messages;
    }
    #endregion

    #region
    public static BinaryData SerializeMessages(IEnumerable<ChatMessage> messages)
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);

        writer.WriteStartArray();
        foreach (ChatMessage message in messages)
        {
            (message as IJsonModel<ChatMessage>).Write(writer, ModelReaderWriterOptions.Json);
        }
        writer.WriteEndArray();
        writer.Flush();

        return BinaryData.FromBytes(stream.ToArray());
    }
    #endregion

    [Test]
    public void Example06_ChatSerialization()
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

        List<ChatMessage> messages = DeserializeMessages(serializedData);

        ChatCompletion completion = client.CompleteChat(messages);
        messages.Add(new AssistantChatMessage(completion));

        foreach (ChatMessage message in messages)
        {
            switch (message)
            {
                case UserChatMessage userMessage:
                    Console.WriteLine($"[USER]:");
                    break;

                case AssistantChatMessage assistantMessage when assistantMessage.Content.Count > 0:
                    Console.WriteLine($"[ASSISTANT]:");
                    break;

                default:
                    break;
            }

            Console.WriteLine($"{message.Content[0].Text}");
            Console.WriteLine();
        }

        serializedData = SerializeMessages(messages);

        Console.WriteLine("****************************************************");
        Console.WriteLine();
        Console.WriteLine(serializedData.ToString());
    }
}
