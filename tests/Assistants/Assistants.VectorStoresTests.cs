using NUnit.Framework;
using OpenAI.Files;
using OpenAI.Tests.Utility;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.VectorStores;

#pragma warning disable OPENAI001

[TestFixture(true)]
[TestFixture(false)]
[Category("Assistants")]
public class VectorStoresTests : SyncAsyncTestBase
{
    private readonly List<CreateBatchFileJobOperation> _jobsToCancel = [];
    private readonly List<VectorStoreFileAssociation> _associationsToRemove = [];
    private readonly List<OpenAIFile> _filesToDelete = [];
    private readonly List<VectorStore> _vectorStoresToDelete = [];

    private static readonly DateTimeOffset s_2024 = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static VectorStoreClient GetTestClient() => GetTestClient<VectorStoreClient>(TestScenario.VectorStores);

    public VectorStoresTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public async Task CanCreateGetAndDeleteVectorStores()
    {
        VectorStoreClient client = GetTestClient();

        CreateVectorStoreOperation createOperation = IsAsync
            ? await client.CreateVectorStoreAsync(waitUntilCompleted: false)
            : client.CreateVectorStore(waitUntilCompleted: false);
        VectorStore vectorStore = createOperation.Value;
        Validate(vectorStore);

        VectorStoreDeletionResult deletionResult = IsAsync
            ? await client.DeleteVectorStoreAsync(vectorStore.Id)
            : client.DeleteVectorStore(vectorStore.Id);
        Assert.That(deletionResult.VectorStoreId, Is.EqualTo(vectorStore.Id));
        Assert.That(deletionResult.Deleted, Is.True);
        _vectorStoresToDelete.RemoveAt(_vectorStoresToDelete.Count - 1);

        IReadOnlyList<OpenAIFile> testFiles = GetNewTestFiles(5);
        VectorStoreCreationOptions creationOptions = new VectorStoreCreationOptions()
        {
            FileIds = { testFiles[0].Id },
            Name = "test vector store",
            ExpirationPolicy = new VectorStoreExpirationPolicy(VectorStoreExpirationAnchor.LastActiveAt, 3),
            Metadata =
            {
                ["test-key"] = "test-value",
            },
        };
        createOperation = IsAsync
           ? await client.CreateVectorStoreAsync(waitUntilCompleted: false, creationOptions)
           : client.CreateVectorStore(waitUntilCompleted: false, creationOptions);

        Validate(createOperation.Value);
        Assert.Multiple(() =>
        {
            Assert.That(createOperation.Value.Name, Is.EqualTo("test vector store"));
            Assert.That(createOperation.Value.ExpirationPolicy?.Anchor, Is.EqualTo(VectorStoreExpirationAnchor.LastActiveAt));
            Assert.That(createOperation.Value.ExpirationPolicy?.Days, Is.EqualTo(3));
            Assert.That(createOperation.Value.FileCounts.Total, Is.EqualTo(1));
            Assert.That(createOperation.Value.CreatedAt, Is.GreaterThan(s_2024));
            Assert.That(createOperation.Value.ExpiresAt, Is.GreaterThan(s_2024));
            Assert.That(createOperation.Status, Is.EqualTo(VectorStoreStatus.InProgress));
            Assert.That(createOperation.Value.Metadata?.TryGetValue("test-key", out string metadataValue) == true && metadataValue == "test-value");
        });

        vectorStore = createOperation.Value;
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

        deletionResult = IsAsync
            ? await client.DeleteVectorStoreAsync(vectorStore.Id)
            : client.DeleteVectorStore(vectorStore.Id);
        Assert.That(deletionResult.VectorStoreId, Is.EqualTo(vectorStore.Id));
        Assert.That(deletionResult.Deleted, Is.True);
        _vectorStoresToDelete.RemoveAt(_vectorStoresToDelete.Count - 1);

        creationOptions = new VectorStoreCreationOptions();
        foreach (var file in testFiles)
        {
            creationOptions.FileIds.Add(file.Id);
        }
        createOperation = IsAsync
            ? await client.CreateVectorStoreAsync(waitUntilCompleted: true, creationOptions)
            : client.CreateVectorStore(waitUntilCompleted: true, creationOptions);
        vectorStore = createOperation.Value;

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
        AssertSyncOnly();

        VectorStoreClient client = GetTestClient();
        for (int i = 0; i < 10; i++)
        {
            CreateVectorStoreOperation createOperation = client.CreateVectorStore(waitUntilCompleted: true, new VectorStoreCreationOptions()
            {
                Name = $"Test Vector Store {i}",
            });
            VectorStore vectorStore = createOperation.Value;
            Validate(vectorStore);
            Assert.That(vectorStore.Name, Is.EqualTo($"Test Vector Store {i}"));
        }

        int lastIdSeen = int.MaxValue;
        int count = 0;

        foreach (VectorStore vectorStore in client.GetVectorStores(new VectorStoreCollectionOptions() { Order = VectorStoreCollectionOrder.Descending }))
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
        AssertAsyncOnly();

        VectorStoreClient client = GetTestClient();
        for (int i = 0; i < 5; i++)
        {
            CreateVectorStoreOperation createOperation = await client.CreateVectorStoreAsync(waitUntilCompleted: true,
                new VectorStoreCreationOptions()
                {
                    Name = $"Test Vector Store {i}",
                });
            VectorStore vectorStore = createOperation.Value;
            Validate(vectorStore);

            Assert.That(vectorStore.Name, Is.EqualTo($"Test Vector Store {i}"));
        }

        Thread.Sleep(5000);

        int lastIdSeen = int.MaxValue;
        int count = 0;

        await foreach (VectorStore vectorStore in client.GetVectorStoresAsync(new VectorStoreCollectionOptions() { Order = VectorStoreCollectionOrder.Descending }))
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
    public async Task CanAssociateFiles()
    {
        VectorStoreClient client = GetTestClient();
        CreateVectorStoreOperation createOperation = IsAsync
            ? await client.CreateVectorStoreAsync(waitUntilCompleted: true)
            : client.CreateVectorStore(waitUntilCompleted: true);
        VectorStore vectorStore = createOperation.Value;
        Validate(vectorStore);

        IReadOnlyList<OpenAIFile> files = GetNewTestFiles(3);

        foreach (OpenAIFile file in files)
        {
            AddFileToVectorStoreOperation addOperation = IsAsync
                ? await client.AddFileToVectorStoreAsync(vectorStore.Id, file.Id, waitUntilCompleted: false)
                : client.AddFileToVectorStore(vectorStore.Id, file.Id, waitUntilCompleted: false);
            VectorStoreFileAssociation association = addOperation.Value;
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

        FileFromStoreRemovalResult removalResult = IsAsync
            ? await client.RemoveFileFromStoreAsync(vectorStore.Id, files[0].Id)
            : client.RemoveFileFromStore(vectorStore.Id, files[0].Id);
        Assert.That(removalResult.FileId, Is.EqualTo(files[0].Id));
        Assert.True(removalResult.Removed);
        _associationsToRemove.RemoveAt(0);

        // Errata: removals aren't immediately reflected when requesting the list
        Thread.Sleep(2000);

        int count = 0;

        if (IsAsync)
        {
            await foreach (VectorStoreFileAssociation association in client.GetFileAssociationsAsync(vectorStore.Id))
            {
                count++;
                Assert.That(association.FileId, Is.Not.EqualTo(files[0].Id));
                Assert.That(association.VectorStoreId, Is.EqualTo(vectorStore.Id));
            }
        }
        else
        {
            foreach (VectorStoreFileAssociation association in client.GetFileAssociations(vectorStore.Id))
            {
                count++;
                Assert.That(association.FileId, Is.Not.EqualTo(files[0].Id));
                Assert.That(association.VectorStoreId, Is.EqualTo(vectorStore.Id));
            }
        }
        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public async Task Pagination_CanRehydrateFileAssociationCollectionAsync()
    {
        AssertAsyncOnly();

        VectorStoreClient client = GetTestClient();
        CreateVectorStoreOperation createOperation = await client.CreateVectorStoreAsync(waitUntilCompleted: false);
        VectorStore vectorStore = createOperation.Value;
        Validate(vectorStore);

        IReadOnlyList<OpenAIFile> files = GetNewTestFiles(6);

        foreach (OpenAIFile file in files)
        {
            AddFileToVectorStoreOperation addOperation = await client.AddFileToVectorStoreAsync(vectorStore.Id, file.Id, waitUntilCompleted: false);
            VectorStoreFileAssociation association = addOperation.Value;
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

        FileFromStoreRemovalResult removalResult = await client.RemoveFileFromStoreAsync(vectorStore.Id, files[0].Id);
        Assert.That(removalResult.FileId, Is.EqualTo(files[0].Id));
        Assert.True(removalResult.Removed);
        _associationsToRemove.RemoveAt(0);

        // Errata: removals aren't immediately reflected when requesting the list
        Thread.Sleep(2000);

        // We added 6 files and will get pages with 2 items, so expect three pages in the collection.

        // Use enumerators instead of enumerables to faciliate advancing the collections
        // at the same time.
        AsyncCollectionResult<VectorStoreFileAssociation> fileAssociations = client.GetFileAssociationsAsync(vectorStore.Id, new VectorStoreFileAssociationCollectionOptions() { PageSizeLimit = 2 });
        IAsyncEnumerable<ClientResult> pages = fileAssociations.GetRawPagesAsync();
        IAsyncEnumerator<ClientResult> pageEnumerator = pages.GetAsyncEnumerator();
        await pageEnumerator.MoveNextAsync();
        ClientResult firstPage = pageEnumerator.Current;
        ContinuationToken nextPageToken = fileAssociations.GetContinuationToken(firstPage);

        // Simulate rehydration of the collection
        BinaryData rehydrationBytes = nextPageToken.ToBytes();
        ContinuationToken rehydrationToken = ContinuationToken.FromBytes(rehydrationBytes);

        AsyncCollectionResult<VectorStoreFileAssociation> rehydratedFileAssociations = client.GetFileAssociationsAsync(rehydrationToken);
        IAsyncEnumerable<ClientResult> rehydratedPages = rehydratedFileAssociations.GetRawPagesAsync();
        IAsyncEnumerator<ClientResult> rehydratedPageEnumerator = rehydratedPages.GetAsyncEnumerator();

        int pageCount = 0;

        while (await pageEnumerator.MoveNextAsync() && await rehydratedPageEnumerator.MoveNextAsync())
        {
            ClientResult page = pageEnumerator.Current;
            ClientResult rehydratedPage = rehydratedPageEnumerator.Current;

            List<VectorStoreFileAssociation> itemsInPage = GetFileAssociationsFromPage(page).ToList();
            List<VectorStoreFileAssociation> itemsInRehydratedPage = GetFileAssociationsFromPage(rehydratedPage).ToList();

            Assert.AreEqual(itemsInPage.Count, itemsInRehydratedPage.Count);

            for (int i = 0; i < itemsInPage.Count; i++)
            {
                Assert.AreEqual(itemsInPage[0].FileId, itemsInRehydratedPage[0].FileId);
                Assert.AreEqual(itemsInPage[0].VectorStoreId, itemsInRehydratedPage[0].VectorStoreId);
                Assert.AreEqual(itemsInPage[0].CreatedAt, itemsInRehydratedPage[0].CreatedAt);
                Assert.AreEqual(itemsInPage[0].Size, itemsInRehydratedPage[0].Size);
            }

            pageCount++;
        }

        // Since we rehydrated the collection at the second page, we expect to
        // see two of the remaining three pages in the collection.
        Assert.That(pageCount, Is.EqualTo(2));
    }

    [Test]
    public void Pagination_CanRehydrateFileAssociationCollection()
    {
        AssertSyncOnly();

        VectorStoreClient client = GetTestClient();
        CreateVectorStoreOperation createOperation = client.CreateVectorStore(waitUntilCompleted: true);
        VectorStore vectorStore = createOperation.Value;
        Validate(vectorStore);

        IReadOnlyList<OpenAIFile> files = GetNewTestFiles(6);

        foreach (OpenAIFile file in files)
        {
            AddFileToVectorStoreOperation addOperation = client.AddFileToVectorStore(vectorStore.Id, file.Id, waitUntilCompleted: false);
            VectorStoreFileAssociation association = addOperation.Value;
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

        FileFromStoreRemovalResult removalResult = client.RemoveFileFromStore(vectorStore.Id, files[0].Id);
        Assert.That(removalResult.FileId, Is.EqualTo(files[0].Id));
        Assert.True(removalResult.Removed);
        _associationsToRemove.RemoveAt(0);

        // Errata: removals aren't immediately reflected when requesting the list
        Thread.Sleep(2000);

        CollectionResult<VectorStoreFileAssociation> fileAssociations = client.GetFileAssociations(vectorStore.Id, new VectorStoreFileAssociationCollectionOptions() { PageSizeLimit = 2 });
        IEnumerable<ClientResult> pages = fileAssociations.GetRawPages();
        IEnumerator<ClientResult> pageEnumerator = pages.GetEnumerator();
        pageEnumerator.MoveNext();
        ClientResult firstPage = pageEnumerator.Current;
        ContinuationToken nextPageToken = fileAssociations.GetContinuationToken(firstPage);

        // Simulate rehydration of the collection
        BinaryData rehydrationBytes = nextPageToken.ToBytes();
        ContinuationToken rehydrationToken = ContinuationToken.FromBytes(rehydrationBytes);

        CollectionResult<VectorStoreFileAssociation> rehydratedFileAssociations = client.GetFileAssociations(rehydrationToken);
        IEnumerable<ClientResult> rehydratedPages = rehydratedFileAssociations.GetRawPages();
        IEnumerator<ClientResult> rehydratedPageEnumerator = rehydratedPages.GetEnumerator();

        int pageCount = 0;

        while (pageEnumerator.MoveNext() && rehydratedPageEnumerator.MoveNext())
        {
            ClientResult page = pageEnumerator.Current;
            ClientResult rehydratedPage = rehydratedPageEnumerator.Current;

            List<VectorStoreFileAssociation> itemsInPage = GetFileAssociationsFromPage(page).ToList();
            List<VectorStoreFileAssociation> itemsInRehydratedPage = GetFileAssociationsFromPage(rehydratedPage).ToList();

            Assert.AreEqual(itemsInPage.Count, itemsInRehydratedPage.Count);

            for (int i = 0; i < itemsInPage.Count; i++)
            {
                Assert.AreEqual(itemsInPage[0].FileId, itemsInRehydratedPage[0].FileId);
                Assert.AreEqual(itemsInPage[0].VectorStoreId, itemsInRehydratedPage[0].VectorStoreId);
                Assert.AreEqual(itemsInPage[0].CreatedAt, itemsInRehydratedPage[0].CreatedAt);
                Assert.AreEqual(itemsInPage[0].Size, itemsInRehydratedPage[0].Size);
            }

            pageCount++;
        }

        Assert.That(pageCount, Is.GreaterThanOrEqualTo(1));
    }

    private static IEnumerable<VectorStoreFileAssociation> GetFileAssociationsFromPage(ClientResult page)
    {
        PipelineResponse response = page.GetRawResponse();
        JsonDocument doc = JsonDocument.Parse(response.Content);
        IEnumerable<JsonElement> els = doc.RootElement.GetProperty("data").EnumerateArray();

        // TODO: improve perf
        return els.Select(el => ModelReaderWriter.Read<VectorStoreFileAssociation>(BinaryData.FromString(el.GetRawText())));
    }

    [Test]
    public async Task CanUseBatchIngestion()
    {
        VectorStoreClient client = GetTestClient();
        CreateVectorStoreOperation createOperation = client.CreateVectorStore(waitUntilCompleted: true);
        VectorStore vectorStore = createOperation.Value;
        Validate(vectorStore);

        IReadOnlyList<OpenAIFile> testFiles = GetNewTestFiles(5);

        CreateBatchFileJobOperation batchFileJobOperation = client.CreateBatchFileJob(vectorStore.Id, testFiles?.Select(file => file.Id), waitUntilCompleted: false);
        Validate(batchFileJobOperation);

        Assert.Multiple(() =>
        {
            Assert.That(batchFileJobOperation.BatchId, Is.Not.Null);
            Assert.That(batchFileJobOperation.VectorStoreId, Is.EqualTo(vectorStore.Id));
            Assert.That(batchFileJobOperation.Status, Is.EqualTo(VectorStoreBatchFileJobStatus.InProgress));
        });

        batchFileJobOperation.WaitForCompletion();

        if (IsAsync)
        {
            await foreach (VectorStoreFileAssociation association in client.GetFileAssociationsAsync(batchFileJobOperation.VectorStoreId))
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
        else
        {
            foreach (VectorStoreFileAssociation association in client.GetFileAssociations(batchFileJobOperation.VectorStoreId))
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
    }

    [Test]
    public void CanRehydrateBatchFileJob()
    {
        VectorStoreClient client = GetTestClient();
        CreateVectorStoreOperation createOperation = client.CreateVectorStore(waitUntilCompleted: true);
        VectorStore vectorStore = createOperation.Value;
        Validate(vectorStore);

        IReadOnlyList<OpenAIFile> testFiles = GetNewTestFiles(5);

        CreateBatchFileJobOperation batchOperation = client.CreateBatchFileJob(vectorStore.Id, testFiles?.Select(file => file.Id), waitUntilCompleted: false);
        Validate(batchOperation);

        // Simulate rehydration of the operation
        BinaryData rehydrationBytes = batchOperation.RehydrationToken.ToBytes();
        ContinuationToken rehydrationToken = ContinuationToken.FromBytes(rehydrationBytes);

        CreateBatchFileJobOperation rehydratedOperation = CreateBatchFileJobOperation.Rehydrate(client, rehydrationToken);
        Validate(rehydratedOperation);

        Assert.Multiple(() =>
        {
            Assert.That(batchOperation.BatchId, Is.Not.Null);
            Assert.That(batchOperation.VectorStoreId, Is.EqualTo(vectorStore.Id));
            Assert.That(batchOperation.Status, Is.EqualTo(VectorStoreBatchFileJobStatus.InProgress));

            Assert.That(rehydratedOperation.BatchId, Is.EqualTo(batchOperation.BatchId));
            Assert.That(rehydratedOperation.VectorStoreId, Is.EqualTo(vectorStore.Id));
            Assert.That(rehydratedOperation.Status, Is.EqualTo(VectorStoreBatchFileJobStatus.InProgress));
        });

        Task.WaitAll(
            Task.Run(() => batchOperation.WaitForCompletion()),
            Task.Run(() => rehydratedOperation.WaitForCompletion()));

        Assert.IsTrue(batchOperation.HasCompleted);
        Assert.IsTrue(rehydratedOperation.HasCompleted);
        Assert.AreEqual(batchOperation.Status, rehydratedOperation.Status);
    }

    public enum ChunkingStrategyKind { Auto, Static }

    [Test]
    [TestCase(ChunkingStrategyKind.Auto)]
    [TestCase(ChunkingStrategyKind.Static)]
    public async Task CanApplyChunkingStrategy(ChunkingStrategyKind strategyKind)
    {
        IReadOnlyList<OpenAIFile> testFiles = GetNewTestFiles(5);

        VectorStoreClient client = GetTestClient();

        FileChunkingStrategy chunkingStrategy = strategyKind switch
        {
            ChunkingStrategyKind.Auto => FileChunkingStrategy.Auto,
            ChunkingStrategyKind.Static => FileChunkingStrategy.CreateStaticStrategy(1200, 250),
            _ => throw new NotImplementedException(),
        };

        if (chunkingStrategy is StaticFileChunkingStrategy inputStaticStrategy)
        {
            Assert.That(inputStaticStrategy.MaxTokensPerChunk, Is.EqualTo(1200));
            Assert.That(inputStaticStrategy.OverlappingTokenCount, Is.EqualTo(250));
        }

        VectorStoreCreationOptions creationOptions = new VectorStoreCreationOptions()
        {
            ChunkingStrategy = chunkingStrategy,
        };
        foreach (var file in testFiles)
        {
            creationOptions.FileIds.Add(file.Id);
        }
        CreateVectorStoreOperation createOperation = IsAsync
            ? await client.CreateVectorStoreAsync(waitUntilCompleted: true, creationOptions)
            : client.CreateVectorStore(waitUntilCompleted: true, creationOptions);
        VectorStore vectorStore = createOperation.Value;
        Validate(vectorStore);
        Assert.That(vectorStore.FileCounts.Total, Is.EqualTo(5));

        if (IsAsync)
        {
            AsyncCollectionResult<VectorStoreFileAssociation> associations = client.GetFileAssociationsAsync(vectorStore.Id);

            await foreach (VectorStoreFileAssociation association in associations)
            {
                Assert.That(testFiles.Any(file => file.Id == association.FileId), Is.True);
                Assert.That(association.ChunkingStrategy, Is.InstanceOf<StaticFileChunkingStrategy>());
                StaticFileChunkingStrategy staticStrategy = association.ChunkingStrategy as StaticFileChunkingStrategy;

                Assert.That(staticStrategy.MaxTokensPerChunk, Is.EqualTo(strategyKind switch
                {
                    ChunkingStrategyKind.Auto => 800,
                    ChunkingStrategyKind.Static => 1200,
                    _ => throw new NotImplementedException()
                }));
                Assert.That(staticStrategy.OverlappingTokenCount, Is.EqualTo(strategyKind switch
                {
                    ChunkingStrategyKind.Auto => 400,
                    ChunkingStrategyKind.Static => 250,
                    _ => throw new NotImplementedException()
                }));
            }
        }
        else
        {
            CollectionResult<VectorStoreFileAssociation> associations = client.GetFileAssociations(vectorStore.Id);

            foreach (VectorStoreFileAssociation association in associations)
            {
                Assert.That(testFiles.Any(file => file.Id == association.FileId), Is.True);
                Assert.That(association.ChunkingStrategy, Is.InstanceOf<StaticFileChunkingStrategy>());
                StaticFileChunkingStrategy staticStrategy = association.ChunkingStrategy as StaticFileChunkingStrategy;

                Assert.That(staticStrategy.MaxTokensPerChunk, Is.EqualTo(strategyKind switch
                {
                    ChunkingStrategyKind.Auto => 800,
                    ChunkingStrategyKind.Static => 1200,
                    _ => throw new NotImplementedException()
                }));
                Assert.That(staticStrategy.OverlappingTokenCount, Is.EqualTo(strategyKind switch
                {
                    ChunkingStrategyKind.Auto => 400,
                    ChunkingStrategyKind.Static => 250,
                    _ => throw new NotImplementedException()
                }));
            }
        }
    }

    private IReadOnlyList<OpenAIFile> GetNewTestFiles(int count)
    {
        List<OpenAIFile> files = [];

        OpenAIFileClient client = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        for (int i = 0; i < count; i++)
        {
            OpenAIFile file = client.UploadFile(
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
        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        VectorStoreClient vectorStoreClient = GetTestClient<VectorStoreClient>(TestScenario.VectorStores);
        RequestOptions requestOptions = new()
        {
            ErrorOptions = ClientErrorBehaviors.NoThrow,
        };

        foreach (CreateBatchFileJobOperation job in _jobsToCancel)
        {
            ClientResult protocolResult = job.Cancel(requestOptions);
            Console.WriteLine($"Cleanup: {job.BatchId} => {protocolResult?.GetRawResponse()?.Status}");
        }
        foreach (VectorStoreFileAssociation association in _associationsToRemove)
        {
            ClientResult protocolResult = vectorStoreClient.RemoveFileFromStore(association.VectorStoreId, association.FileId, requestOptions);
            Console.WriteLine($"Cleanup: {association.FileId}<->{association.VectorStoreId} => {protocolResult?.GetRawResponse()?.Status}");
        }
        foreach (OpenAIFile file in _filesToDelete)
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
        if (target is CreateBatchFileJobOperation job)
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
        else if (target is OpenAIFile file)
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
}

#pragma warning restore OPENAI001
