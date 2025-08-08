using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public async Task Example10_GetStoredChatMessagesAsync()
    {
        #pragma warning disable OPENAI001
        // Create a chat client with your API key and desired model.
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        // Enable stored output so the service persists the completion's generated
        // messages. Those stored messages can then be enumerated later by ID.
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true
        };

        // Request a chat completion. The returned completion contains an Id we
        // will use to retrieve the stored messages.
        ChatCompletion completion = await client.CompleteChatAsync(
            [
                "Provide a short (2-3 sentence) fun fact about octopuses, then briefly explain why it's interesting."
            ],
            createOptions);

        Console.WriteLine($"Created stored chat completion: {completion.Id}\n");

        // NOTE: The stored messages may take a brief moment to become queryable
        // immediately after creation. In robust scenarios you might add a small
        // retry with backoff (see ChatStoreTests for patterns). Here we simply
        // enumerate what is available and stop after a few messages.
        Console.WriteLine("Enumerating stored chat messages (first few):\n");

        int shown = 0;
        await foreach (ChatCompletionMessageListDatum message in client.GetChatCompletionMessagesAsync(completion.Id))
        {
            // Each enumerated item provides the message Id and unified text Content
            // for that turn (tool call outputs, etc., are consolidated server-side).
            Console.WriteLine($"[MESSAGE ID]: {message.Id}");
            Console.WriteLine(message.Content);
            Console.WriteLine();

            if (++shown >= 5)
            {
                // Keep the example concise; real code can stream/page all results.
                break;
            }
        }

        // (Optional) If you no longer require the stored completion, you may delete it:
        // await client.DeleteChatCompletionAsync(completion.Id);
        #pragma warning restore OPENAI001
    }
}
