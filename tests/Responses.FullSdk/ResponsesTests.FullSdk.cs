using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Conversations;
using OpenAI.Files;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses;

#pragma warning disable OPENAICUA001

public partial class ResponsesTests
{
    private List<string> FileIdsToDelete = [];
    private List<string> VectorStoreIdsToDelete = [];

    [OneTimeTearDown]
    protected void Cleanup()
    {
        Console.WriteLine("[Teardown]");

        // Skip cleanup if there is no API key (e.g., if we are not running live tests).
        if (Mode == RecordedTestMode.Playback || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENAI_API_KEY")))
        {
            Console.WriteLine("[WARNING] Can't clean up");
            return;
        }

        RequestOptions noThrowOptions = new() { ErrorOptions = ClientErrorBehaviors.NoThrow };
        OpenAIFileClient fileClient = TestEnvironment.GetTestClient<OpenAIFileClient>();
        VectorStoreClient vectorStoreClient = TestEnvironment.GetTestClient<VectorStoreClient>();

        foreach (string fileId in FileIdsToDelete)
        {
            Console.WriteLine($"[File cleanup] {fileId}");
            fileClient.DeleteFile(fileId, noThrowOptions);
        }

        foreach (string vectorStoreId in VectorStoreIdsToDelete)
        {
            Console.WriteLine($"[Vector store cleanup] {vectorStoreId}");
            vectorStoreClient.DeleteVectorStore(vectorStoreId, noThrowOptions);
        }
    }

    private void Validate<T>(T input) where T : class
    {
        if (input is OpenAIFile file)
        {
            FileIdsToDelete.Add(file.Id);
        }
        if (input is VectorStore vectorStore)
        {
            VectorStoreIdsToDelete.Add(vectorStore.Id);
        }
    }

    [RecordedTest]
    public async Task ResponseUsingConversations()
    {
        ConversationClient conversationClient = GetProxiedOpenAIClient<ConversationClient>();

        BinaryData createConversationParameters = BinaryData.FromBytes("""
            {
               "metadata": { "topic": "test" },
               "items": [
                   {
                       "type": "message",
                       "role": "user",
                       "content": "tell me a joke"
                   }
               ]
            }
            """u8.ToArray());

        using BinaryContent requestContent = BinaryContent.Create(createConversationParameters);
        var conversationResult = await conversationClient.CreateConversationAsync(requestContent);
        using JsonDocument conversationResultAsJson = JsonDocument.Parse(conversationResult.GetRawResponse().Content.ToString());
        string conversationId = conversationResultAsJson.RootElement.GetProperty("id"u8).GetString();

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();
        ResponseResult response = await client.CreateResponseAsync(
            new CreateResponseOptions("gpt-4.1", [ResponseItem.CreateUserMessageItem("tell me another")])
            {
                ConversationOptions = new(conversationId),
            });

        Assert.That(response, Is.Not.Null);
        Assert.That(response.ConversationOptions.ConversationId, Is.EqualTo(conversationId));

        var conversationResults = conversationClient.GetConversationItemsAsync(conversationId);
        var conversationItems = new List<JsonElement>();
        await foreach (ClientResult result in conversationResults.GetRawPagesAsync())
        {
            using JsonDocument getConversationItemsResultAsJson = JsonDocument.Parse(result.GetRawResponse().Content.ToString());
            foreach (JsonElement element in getConversationItemsResultAsJson.RootElement.GetProperty("data").EnumerateArray())
            {
                conversationItems.Add(element.Clone());
            }
        }

        Assert.That(conversationItems, Is.Not.Empty, "Expected the conversation to contain items.");
        Assert.That(conversationItems.Count, Is.GreaterThanOrEqualTo(2));

        var lastItem = conversationItems[conversationItems.Count - 1];
        var secondLastItem = conversationItems[conversationItems.Count - 2];

        string GetContentText(JsonElement item) =>
            item.GetProperty("content")[0].GetProperty("text").GetString();

        Assert.That(GetContentText(lastItem), Is.EqualTo("tell me a joke"));
        Assert.That(GetContentText(secondLastItem), Is.EqualTo("tell me another"));
    }

    [RecordedTest]
    public async Task FileInputFromIdWorks()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();
        string filePath = Path.Join("Assets", "files_travis_favorite_food.pdf");

        OpenAIFile newFileToUse;
        using (Recording.DisableRequestBodyRecording()) // Temp pending https://github.com/Azure/azure-sdk-tools/issues/11901
        {
            newFileToUse = await fileClient.UploadFileAsync(
            BinaryData.FromBytes(File.ReadAllBytes(filePath)),
            "test_favorite_foods.pdf",
            FileUploadPurpose.UserData);
        }

        Validate(newFileToUse);

        ResponseItem messageItem = ResponseItem.CreateUserMessageItem(
            [
                ResponseContentPart.CreateInputTextPart("Based on this file, what food should I order for whom?"),
                ResponseContentPart.CreateInputFilePart(newFileToUse.Id),
            ]);

        ResponseResult response = await client.CreateResponseAsync(TestModel.Responses, [messageItem]);

        Assert.That(response?.GetOutputText().ToLower(), Does.Contain("pizza"));
    }
}
