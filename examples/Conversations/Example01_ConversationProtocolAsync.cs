using NUnit.Framework;
using OpenAI.Conversations;
using System;
using System.ClientModel;
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

        Console.WriteLine($"Conversation created. Conversation ID: {conversationId}");
        Console.WriteLine();

        ClientResult getConversationItemsResult = await client.GetConversationItemsAsync(conversationId);
        using JsonDocument getConversationItemsResultAsJson = JsonDocument.Parse(getConversationItemsResult.GetRawResponse().Content.ToString());
        string messageId = getConversationItemsResultAsJson.RootElement
            .GetProperty("data"u8)[0]
            .GetProperty("id"u8)
            .ToString();

        Console.WriteLine($"Message retrieved. Message ID: {messageId}");
        Console.WriteLine();

        ClientResult deleteConversationResult = await client.DeleteConversationAsync(conversationId);
        using JsonDocument deleteConversationResultAsJson = JsonDocument.Parse(deleteConversationResult.GetRawResponse().Content.ToString());
        bool deleted = deleteConversationResultAsJson.RootElement
            .GetProperty("deleted"u8)
            .GetBoolean();

        Console.WriteLine($"Conversation deleted: {deleted}");
        Console.WriteLine();
    }
}

#pragma warning restore OPENAI001