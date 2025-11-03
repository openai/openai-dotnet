using NUnit.Framework;
using OpenAI.Conversations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ConversationExamples
{
    [Test]
    public void Example01_ConversationProtocol()
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
        ClientResult createConversationResult = client.CreateConversation(createConversationRequestContent);
        using JsonDocument createConversationResultAsJson = JsonDocument.Parse(createConversationResult.GetRawResponse().Content.ToString());
        string conversationId = createConversationResultAsJson.RootElement
            .GetProperty("id"u8)
            .GetString();

        Console.WriteLine($"Conversation created.");
        Console.WriteLine($"    Conversation ID: {conversationId}");
        Console.WriteLine();

        CollectionResult getConversationItemsResults = client.GetConversationItems(conversationId);
        foreach (ClientResult result in getConversationItemsResults.GetRawPages())
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

        ClientResult deleteConversationResult = client.DeleteConversation(conversationId);
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