using NUnit.Framework;
using OpenAI.Conversations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ConversationExamples
{
    [Test]
    public async Task Example01_ConversationProtocolAsync()
    {
        ConversationClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        BinaryData createConversationParameters = BinaryData.FromBytes("""
            {
               "metadata": { "topic": "test" },
               "items": [
                   {
                       "type": "message",
                       "role": "user",
                       "content": "Say 'this is a test.'"
                   },
                   {
                       "type": "message",
                       "role": "assistant",
                       "content": "This is a test."
                   }
               ]
            }
            """u8.ToArray());

        using BinaryContent createConversationRequestContent = BinaryContent.Create(createConversationParameters);
        ClientResult createConversationResult = await client.CreateConversationAsync(createConversationRequestContent);
        using JsonDocument createConversationResultAsJson = JsonDocument.Parse(createConversationResult.GetRawResponse().Content.ToString());
        string conversationId = createConversationResultAsJson.RootElement
            .GetProperty("id"u8)
            .GetString();

        Console.WriteLine($"Conversation created.");
        Console.WriteLine($"    Conversation ID: {conversationId}");
        Console.WriteLine();

        AsyncCollectionResult getConversationItemsResults = client.GetConversationItemsAsync(conversationId);
        await foreach(ClientResult result in getConversationItemsResults.GetRawPagesAsync())
        {
            using JsonDocument getConversationItemsResultAsJson = JsonDocument.Parse(result.GetRawResponse().Content.ToString());
            foreach (JsonElement element in getConversationItemsResultAsJson.RootElement.GetProperty("data").EnumerateArray())
            {
                string messageId = element.GetProperty("id"u8).ToString();

                Console.WriteLine($"Message retrieved.");
                Console.WriteLine($"    Message ID: {messageId}");
                Console.WriteLine();
            }
        }

        ClientResult deleteConversationResult = await client.DeleteConversationAsync(conversationId);
        using JsonDocument deleteConversationResultAsJson = JsonDocument.Parse(deleteConversationResult.GetRawResponse().Content.ToString());
        bool deleted = deleteConversationResultAsJson.RootElement
            .GetProperty("deleted"u8)
            .GetBoolean();

        Console.WriteLine($"Conversation deleted.");
        Console.WriteLine($"    Deleted: {deleted}");
        Console.WriteLine();
    }
}

#pragma warning restore OPENAI001