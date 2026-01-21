using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenAI.Files;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class VectorStoreExamples
{
    [Test]
    public void Example01_SimpleVectorStoreCreation()
    {
        VectorStoreClient client = new(new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY")));

        IReadOnlyList<OpenAIFile> files = GetSampleFiles(5);
        VectorStoreCreationOptions storeOptions = new VectorStoreCreationOptions();
        foreach (var file in files)
        {
            storeOptions.FileIds.Add(file.Id);
        }
        VectorStore store = client.CreateVectorStore(storeOptions);

        store = PollVectorStore(client, store.Id);

        Assert.That(store.Status, Is.EqualTo(VectorStoreStatus.Completed));
        Console.WriteLine($"Vector store {store.Id} is ready (status: {store.Status})");
    }

    /// <summary>
    /// Wait for the vector store to finish processing.
    /// </summary>
    private static VectorStore PollVectorStore(VectorStoreClient client, string vectorStoreId)
    {
        while (true)
        {
            ClientResult<VectorStore> result = client.GetVectorStore(vectorStoreId);
            var store = result.Value;
            var response = result.GetRawResponse();

            if (store.Status == VectorStoreStatus.InProgress)
            {
                // Default 1s; override if server hints are present
                TimeSpan pollDelay = TimeSpan.FromSeconds(1);
                if (response.Headers.TryGetValue("Retry-After", out var retryAfter))
                {
                    if (int.TryParse(retryAfter, out var delaySeconds))
                    {
                        pollDelay = TimeSpan.FromSeconds(delaySeconds);
                    }
                    else if (DateTimeOffset.TryParse(retryAfter, out var retryAfterDate))
                    {
                        pollDelay = retryAfterDate - DateTimeOffset.Now;
                    }
                }
                else if (response.Headers.TryGetValue("openai-poll-after-ms", out var msStr) &&
                        int.TryParse(msStr, out var ms))
                {
                    pollDelay = TimeSpan.FromMilliseconds(ms);
                }

                Thread.Sleep(pollDelay);
            }
            else
            {
                return store;
            }
        }
    }

    private static IReadOnlyList<OpenAIFile> GetSampleFiles(int count)
    {
        var files = new List<OpenAIFile>();
        var fileClient = new OpenAIFileClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        for (int i = 0; i < count; i++)
        {
            using var stream = BinaryData.FromString($"This is a sample file {i}").ToStream();
            var file = fileClient.UploadFile(
                stream,
                $"sample_file_{i:000}.txt",
                FileUploadPurpose.Assistants);
            files.Add(file);
        }

        return files;
    }
}

#pragma warning restore OPENAI001
