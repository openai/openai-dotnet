using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using System;
using System.ClientModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class AssistantExamples
{
    [Test]
    public async Task Example01_RetrievalAugmentedGenerationAsync()
    {
        OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
        AssistantClient assistantClient = openAIClient.GetAssistantClient();

        // First, let's contrive a document we'll use retrieval with and upload it.
        using Stream document = BinaryData.FromBytes("""
            {
                "description": "This document contains the sale history data for Contoso products.",
                "sales": [
                    {
                        "month": "January",
                        "by_product": {
                            "113043": 15,
                            "113045": 12,
                            "113049": 2
                        }
                    },
                    {
                        "month": "February",
                        "by_product": {
                            "113045": 22
                        }
                    },
                    {
                        "month": "March",
                        "by_product": {
                            "113045": 16,
                            "113055": 5
                        }
                    }
                ]
            }
            """u8.ToArray()).ToStream();

        OpenAIFile salesFile = await fileClient.UploadFileAsync(
            document,
            "monthly_sales.json",
            FileUploadPurpose.Assistants);

        // Now, we'll create a client intended to help with that data
        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Example: Contoso sales RAG",
            Instructions =
                "You are an assistant that looks up sales data and helps visualize the information based"
                + " on user queries. When asked to generate a graph, chart, or other visualization, use"
                + " the code interpreter tool to do so.",
            Tools =
            {
                new FileSearchToolDefinition(),
                new CodeInterpreterToolDefinition(),
            },
            ToolResources = new()
            {
                FileSearch = new()
                {
                    NewVectorStores =
                    {
                        new VectorStoreCreationHelper([salesFile.Id]),
                    }
                }
            },
        };

        Assistant assistant = await assistantClient.CreateAssistantAsync("gpt-4o", assistantOptions);

        // Now we'll create a thread with a user query about the data already associated with the assistant, then run it
        ThreadCreationOptions threadOptions = new()
        {
            InitialMessages = { "How well did product 113045 sell in February? Graph its trend over time." }
        };

        ThreadRun threadRun = await assistantClient.CreateThreadAndRunAsync(assistant.Id, threadOptions);

        // Check back to see when the run is done
        do
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            threadRun = assistantClient.GetRun(threadRun.ThreadId, threadRun.Id);
        } while (!threadRun.Status.IsTerminal);

        // Finally, we'll print out the full history for the thread that includes the augmented generation
        AsyncCollectionResult<ThreadMessage> messages
            = assistantClient.GetMessagesAsync(threadRun.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });

        await foreach (ThreadMessage message in messages)
        {
            Console.Write($"[{message.Role.ToString().ToUpper()}]: ");
            foreach (MessageContent contentItem in message.Content)
            {
                if (!string.IsNullOrEmpty(contentItem.Text))
                {
                    Console.WriteLine($"{contentItem.Text}");

                    if (contentItem.TextAnnotations.Count > 0)
                    {
                        Console.WriteLine();
                    }

                    // Include annotations, if any.
                    foreach (TextAnnotation annotation in contentItem.TextAnnotations)
                    {
                        if (!string.IsNullOrEmpty(annotation.InputFileId))
                        {
                            Console.WriteLine($"* File citation, file ID: {annotation.InputFileId}");
                        }
                        if (!string.IsNullOrEmpty(annotation.OutputFileId))
                        {
                            Console.WriteLine($"* File output, new file ID: {annotation.OutputFileId}");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(contentItem.ImageFileId))
                {
                    OpenAIFile imageInfo = await fileClient.GetFileAsync(contentItem.ImageFileId);
                    BinaryData imageBytes = await fileClient.DownloadFileAsync(contentItem.ImageFileId);
                    using FileStream stream = File.OpenWrite($"{imageInfo.Filename}.png");
                    imageBytes.ToStream().CopyTo(stream);

                    Console.WriteLine($"<image: {imageInfo.Filename}.png>");
                }
            }
            Console.WriteLine();
        }

        // Optionally, delete any persistent resources you no longer need.
        _ = await assistantClient.DeleteThreadAsync(threadRun.ThreadId);
        _ = await assistantClient.DeleteAssistantAsync(assistant.Id);
        _ = await fileClient.DeleteFileAsync(salesFile.Id);
    }
}

#pragma warning restore OPENAI001