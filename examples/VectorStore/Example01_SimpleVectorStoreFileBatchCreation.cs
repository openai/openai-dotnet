using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenAI.Files;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class VectorStoreExamples
{
    [Test]
    public void Example01_SimpleVectorStoreFileBatchCreation()
    {
        var options = new OpenAIClientOptions();
        options.AddPolicy(new StainlessPollingHeadersPolicy(), PipelinePosition.PerCall);
        VectorStoreClient client = new(new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY")), options);

        VectorStore store = client.CreateVectorStore();

        IReadOnlyList<OpenAIFile> files = GetSampleFiles(10);
        VectorStoreFileBatch fileBatch = client.AddFileBatchToVectorStore(store.Id, files?.Select(file => file.Id));

        fileBatch = PollVectorStoreFileBatch(client, store.Id, fileBatch.BatchId);

        Console.WriteLine($"Vector store {fileBatch.BatchId} is ready (status: {fileBatch.Status})");
    }

    /// <summary>
    /// Waits for the vector store batch file to finish processing.
    /// </summary>
    private static VectorStoreFileBatch PollVectorStoreFileBatch(
        VectorStoreClient client,
        string vectorStoreId,
        string batchId)
    {
        while (true)
        {
            ClientResult<VectorStoreFileBatch> result = client.GetVectorStoreFileBatch(vectorStoreId, batchId);

            VectorStoreFileBatch fileBatch = result.Value;
            var response = result.GetRawResponse();

            if (fileBatch.Status == VectorStoreFileBatchStatus.InProgress)
            {
                var pollIntervalMs = 1000; 
                if (response.Headers.TryGetValue("openai-poll-after-ms", out var msStr) &&
                        int.TryParse(msStr, out var ms))
                {
                    pollIntervalMs = ms;
                }

                Thread.Sleep(pollIntervalMs);
            }
            else if (fileBatch.Status == VectorStoreFileBatchStatus.Completed
                  || fileBatch.Status == VectorStoreFileBatchStatus.Cancelled
                  || fileBatch.Status == VectorStoreFileBatchStatus.Failed)
            {
                return fileBatch;
            }
            else
            {
                return fileBatch; // any unforeseen state -> return as-is
            }
        }
    }

    private IReadOnlyList<OpenAIFile> GetSampleFiles(int count)
    {
        List<OpenAIFile> files = [];

        OpenAIFileClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        for (int i = 0; i < count; i++)
        {
            OpenAIFile file = client.UploadFile(
                BinaryData.FromString($"This is a sample file {i}").ToStream(),
                $"sample_file_{i.ToString().PadLeft(3, '0')}.txt",
                FileUploadPurpose.Assistants);
            files.Add(file);
        }

        return files;
    }
}

#pragma warning restore OPENAI001
