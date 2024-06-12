﻿using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.VectorStores;

#pragma warning disable OPENAI001
public partial class VectorStoreTests
{
    [Test]
    public void CanCreateGetAndDeleteVectorStores()
    {
        VectorStoreClient client = GetTestClient();

        VectorStore vectorStore = client.CreateVectorStore();
        Validate(vectorStore);
        bool deleted = client.DeleteVectorStore(vectorStore);
        Assert.That(deleted, Is.True);
        _vectorStoresToDelete.RemoveAt(_vectorStoresToDelete.Count - 1);

        IReadOnlyList<OpenAIFileInfo> testFiles = GetNewTestFiles(5);

        vectorStore = client.CreateVectorStore(new()
        {
            FileIds = { testFiles[0].Id },
            Name = "test vector store",
            ExpirationPolicy = new VectorStoreExpirationPolicy()
            {
                Anchor = VectorStoreExpirationAnchor.LastActiveAt,
                Days = 3,
            },
            Metadata =
            {
                ["test-key"] = "test-value",
            },
        });
        Validate(vectorStore);
        Assert.Multiple(() =>
        {
            Assert.That(vectorStore.Name, Is.EqualTo("test vector store"));
            Assert.That(vectorStore.ExpirationPolicy?.Anchor, Is.EqualTo(VectorStoreExpirationAnchor.LastActiveAt));
            Assert.That(vectorStore.ExpirationPolicy?.Days, Is.EqualTo(3));
            Assert.That(vectorStore.FileCounts.Total, Is.EqualTo(1));
            Assert.That(vectorStore.CreatedAt, Is.GreaterThan(s_2024));
            Assert.That(vectorStore.ExpiresAt, Is.GreaterThan(s_2024));
            Assert.That(vectorStore.Status, Is.EqualTo(VectorStoreStatus.InProgress));
            Assert.That(vectorStore.Metadata?.TryGetValue("test-key", out string metadataValue) == true && metadataValue == "test-value");
        });
        vectorStore = client.GetVectorStore(vectorStore);
        Assert.Multiple(() =>
        {
            Assert.That(vectorStore.Name, Is.EqualTo("test vector store"));
            Assert.That(vectorStore.ExpirationPolicy?.Anchor, Is.EqualTo(VectorStoreExpirationAnchor.LastActiveAt));
            Assert.That(vectorStore.ExpirationPolicy?.Days, Is.EqualTo(3));
            Assert.That(vectorStore.FileCounts.Total, Is.EqualTo(1));
            Assert.That(vectorStore.CreatedAt, Is.GreaterThan(s_2024));
            Assert.That(vectorStore.ExpiresAt, Is.GreaterThan(s_2024));
            Assert.That(vectorStore.Metadata?.TryGetValue("test-key", out string metadataValue) == true && metadataValue == "test-value");
        });

        deleted = client.DeleteVectorStore(vectorStore.Id);
        Assert.That(deleted, Is.True);
        _vectorStoresToDelete.RemoveAt(_vectorStoresToDelete.Count - 1);

        vectorStore = client.CreateVectorStore(new()
        {
            FileIds = testFiles.Select(file => file.Id).ToList()
        });
        Validate(vectorStore);
        Assert.Multiple(() =>
        {
            Assert.That(vectorStore.Name, Is.Null.Or.Empty);
            Assert.That(vectorStore.FileCounts.Total, Is.EqualTo(5));
        });
    }

    [Test]
    public void CanEnumerateVectorStores()
    {
        VectorStoreClient client = GetTestClient();
        for (int i = 0; i < 10; i++)
        {
            VectorStore vectorStore = client.CreateVectorStore(new VectorStoreCreationOptions()
            {
                Name = $"Test Vector Store {i}",
            });
            Validate(vectorStore);
            Assert.That(vectorStore.Name, Is.EqualTo($"Test Vector Store {i}"));
        }

        int lastIdSeen = int.MaxValue;
        int count = 0;

        foreach (VectorStore vectorStore in client.GetVectorStores(ListOrder.NewestFirst))
        {
            Assert.That(vectorStore.Id, Is.Not.Null);
            if (vectorStore.Name?.StartsWith("Test Vector Store ") == true)
            {
                string idString = vectorStore.Name["Test Vector Store ".Length..];

                Assert.That(int.TryParse(idString, out int seenId), Is.True);
                Assert.That(seenId, Is.LessThan(lastIdSeen));
                lastIdSeen = seenId;
            }
            if (lastIdSeen == 0 || ++count >= 100)
            {
                break;
            }
        }

        Assert.That(lastIdSeen, Is.EqualTo(0));
    }

    [Test]
    public async Task CanEnumerateVectorStoresAsync()
    {
        VectorStoreClient client = GetTestClient();
        for (int i = 0; i < 10; i++)
        {
            VectorStore vectorStore = await client.CreateVectorStoreAsync(new VectorStoreCreationOptions()
            {
                Name = $"Test Vector Store {i}",
            });
            Validate(vectorStore);
            Assert.That(vectorStore.Name, Is.EqualTo($"Test Vector Store {i}"));
        }

        int lastIdSeen = int.MaxValue;
        int count = 0;

        await foreach (VectorStore vectorStore in client.GetVectorStoresAsync(ListOrder.NewestFirst))
        {
            Assert.That(vectorStore.Id, Is.Not.Null);
            if (vectorStore.Name?.StartsWith("Test Vector Store ") == true)
            {
                string idString = vectorStore.Name["Test Vector Store ".Length..];

                Assert.That(int.TryParse(idString, out int seenId), Is.True);
                Assert.That(seenId, Is.LessThan(lastIdSeen));
                lastIdSeen = seenId;
            }
            if (lastIdSeen == 0 || ++count >= 100)
            {
                break;
            }
        }

        Assert.That(lastIdSeen, Is.EqualTo(0));
    }

    [Test]
    public void CanAssociateFiles()
    {
        VectorStoreClient client = GetTestClient();
        VectorStore vectorStore = client.CreateVectorStore();
        Validate(vectorStore);

        IReadOnlyList<OpenAIFileInfo> files = GetNewTestFiles(3);

        foreach (OpenAIFileInfo file in files)
        {
            VectorStoreFileAssociation association = client.AddFileToVectorStore(vectorStore, file);
            Validate(association);
            Assert.Multiple(() =>
            {
                Assert.That(association.FileId, Is.EqualTo(file.Id));
                Assert.That(association.VectorStoreId, Is.EqualTo(vectorStore.Id));
                Assert.That(association.LastError, Is.Null);
                Assert.That(association.CreatedAt, Is.GreaterThan(s_2024));
                Assert.That(association.Status, Is.EqualTo(VectorStoreFileAssociationStatus.InProgress));
            });
        }

        bool removed = client.RemoveFileFromStore(vectorStore, files[0]);
        Assert.True(removed);
        _associationsToRemove.RemoveAt(0);

        // Errata: removals aren't immediately reflected when requesting the list
        Thread.Sleep(1000);

        int count = 0;
        foreach (VectorStoreFileAssociation association in client.GetFileAssociations(vectorStore))
        {
            count++;
            Assert.That(association.FileId, Is.Not.EqualTo(files[0].Id));
            Assert.That(association.VectorStoreId, Is.EqualTo(vectorStore.Id));
        }
        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public void CanUseBatchIngestion()
    {
        VectorStoreClient client = GetTestClient();
        VectorStore vectorStore = client.CreateVectorStore();
        Validate(vectorStore);

        IReadOnlyList<OpenAIFileInfo> testFiles = GetNewTestFiles(5);

        VectorStoreBatchFileJob batchJob = client.CreateBatchFileJob(vectorStore, testFiles);
        Validate(batchJob);

        Assert.Multiple(() =>
        {
            Assert.That(batchJob.BatchId, Is.Not.Null);
            Assert.That(batchJob.VectorStoreId, Is.EqualTo(vectorStore.Id));
            Assert.That(batchJob.Status, Is.EqualTo(VectorStoreBatchFileJobStatus.InProgress));
        });

        for (int i = 0; i < 10 && client.GetBatchFileJob(batchJob).Value.Status != VectorStoreBatchFileJobStatus.Completed; i++)
        {
            Thread.Sleep(500);
        }

        foreach (VectorStoreFileAssociation association in client.GetFileAssociations(batchJob))
        {
            Assert.Multiple(() =>
            {
                Assert.That(association.FileId, Is.Not.Null);
                Assert.That(association.VectorStoreId, Is.EqualTo(vectorStore.Id));
                Assert.That(association.Status, Is.EqualTo(VectorStoreFileAssociationStatus.Completed));
                // Assert.That(association.Size, Is.GreaterThan(0));
                Assert.That(association.CreatedAt, Is.GreaterThan(s_2024));
                Assert.That(association.LastError, Is.Null);
            });
        }
    }

    private IReadOnlyList<OpenAIFileInfo> GetNewTestFiles(int count)
    {
        List<OpenAIFileInfo> files = [];

        FileClient client = new();
        for (int i = 0; i < count; i++)
        {
            OpenAIFileInfo file = client.UploadFile(
                BinaryData.FromString("This is a test file").ToStream(),
                $"test_file_{i.ToString().PadLeft(3, '0')}.txt",
                FileUploadPurpose.Assistants);
            Validate(file);
            files.Add(file);
        }

        return files;
    }
    [TearDown]
    protected void Cleanup()
    {
        FileClient fileClient = new();
        VectorStoreClient vectorStoreClient = new();
        RequestOptions requestOptions = new()
        {
            ErrorOptions = ClientErrorBehaviors.NoThrow,
        };
        foreach (VectorStoreBatchFileJob job in _jobsToCancel)
        {
            ClientResult protocolResult = vectorStoreClient.CancelBatchFileJob(job.VectorStoreId, job.BatchId, requestOptions);
            Console.WriteLine($"Cleanup: {job.BatchId} => {protocolResult?.GetRawResponse()?.Status}");
        }
        foreach (VectorStoreFileAssociation association in _associationsToRemove)
        {
            ClientResult protocolResult = vectorStoreClient.RemoveFileFromStore(association.VectorStoreId, association.FileId, requestOptions);
            Console.WriteLine($"Cleanup: {association.FileId}<->{association.VectorStoreId} => {protocolResult?.GetRawResponse()?.Status}");
        }
        foreach (OpenAIFileInfo file in _filesToDelete)
        {
            Console.WriteLine($"Cleanup: {file.Id} -> {fileClient.DeleteFile(file.Id, requestOptions)?.GetRawResponse()?.Status}");
        }
        foreach (VectorStore vectorStore in _vectorStoresToDelete)
        {
            Console.WriteLine($"Cleanup: {vectorStore.Id} => {vectorStoreClient.DeleteVectorStore(vectorStore.Id, requestOptions)?.GetRawResponse()?.Status}");
        }
        _filesToDelete.Clear();
        _vectorStoresToDelete.Clear();
    }

    /// <summary>
    /// Performs basic, invariant validation of a target that was just instantiated from its corresponding origination
    /// mechanism. If applicable, the instance is recorded into the test run for cleanup of persistent resources.
    /// </summary>
    /// <typeparam name="T"> Instance type being validated. </typeparam>
    /// <param name="target"> The instance to validate. </param>
    /// <exception cref="NotImplementedException"> The provided instance type isn't supported. </exception>
    private void Validate<T>(T target)
    {
        if (target is VectorStoreBatchFileJob job)
        {
            Assert.That(job.BatchId, Is.Not.Null);
            _jobsToCancel.Add(job);
        }
        else if (target is VectorStoreFileAssociation association)
        {
            Assert.That(association?.FileId, Is.Not.Null);
            Assert.That(association?.VectorStoreId, Is.Not.Null);
            _associationsToRemove.Add(association);
        }
        else if (target is OpenAIFileInfo file)
        {
            Assert.That(file?.Id, Is.Not.Null);
            _filesToDelete.Add(file);
        }
        else if (target is VectorStore vectorStore)
        {
            Assert.That(vectorStore?.Id, Is.Not.Null);
            _vectorStoresToDelete.Add(vectorStore);
        }
        else
        {
            throw new NotImplementedException($"{nameof(Validate)} helper not implemented for: {typeof(T)}");
        }
    }

    private readonly List<VectorStoreBatchFileJob> _jobsToCancel = [];
    private readonly List<VectorStoreFileAssociation> _associationsToRemove = [];
    private readonly List<OpenAIFileInfo> _filesToDelete = [];
    private readonly List<VectorStore> _vectorStoresToDelete = [];

    private static VectorStoreClient GetTestClient() => GetTestClient<VectorStoreClient>(TestScenario.VectorStores);

    private static readonly DateTimeOffset s_2024 = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
}

#pragma warning restore OPENAI001
