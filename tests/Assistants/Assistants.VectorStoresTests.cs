using Microsoft.ClientModel.TestFramework;
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
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.VectorStores;

#pragma warning disable OPENAI001

[Category("Assistants")]
public class VectorStoresTests : OpenAIRecordedTestBase
{
    private readonly List<VectorStoreFileBatch> _fileBatchToCancel = [];
    private readonly List<VectorStoreFile> _vectorStoreFilesToRemove = [];
    private readonly List<OpenAIFile> _filesToDelete = [];
    private readonly List<VectorStore> _vectorStoresToDelete = [];

    private static readonly DateTimeOffset s_2024 = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public enum ChunkingStrategyKind { Auto, Static }

    public VectorStoresTests(bool isAsync)
        : base(isAsync)
    {
        TestTimeoutInSeconds = 45;
    }

    [RecordedTest]
    public async Task CanCreateGetAndDeleteVectorStores()
    {
        VectorStoreClient client = GetTestClient();

        VectorStore vectorStore = await client.CreateVectorStoreAsync();
        Validate(vectorStore);

        VectorStoreDeletionResult deletionResult = await client.DeleteVectorStoreAsync(vectorStore.Id);
        Assert.That(deletionResult.VectorStoreId, Is.EqualTo(vectorStore.Id));
        Assert.That(deletionResult.Deleted, Is.True);
        _vectorStoresToDelete.RemoveAt(_vectorStoresToDelete.Count - 1);

        IReadOnlyList<OpenAIFile> testFiles = await GetNewTestFiles(5);
        VectorStoreCreationOptions creationOptions = new()
        {
            FileIds = { testFiles[0].Id },
            Name = "test vector store",
            ExpirationPolicy = new VectorStoreExpirationPolicy(VectorStoreExpirationAnchor.LastActiveAt, 3),
            Metadata =
            {
                ["test-key"] = "test-value",
            },
        };
        vectorStore = await client.CreateVectorStoreAsync(creationOptions);

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

        deletionResult = await client.DeleteVectorStoreAsync(vectorStore.Id);
        Assert.That(deletionResult.VectorStoreId, Is.EqualTo(vectorStore.Id));
        Assert.That(deletionResult.Deleted, Is.True);
        _vectorStoresToDelete.RemoveAt(_vectorStoresToDelete.Count - 1);

        creationOptions = new VectorStoreCreationOptions();
        foreach (var file in testFiles)
        {
            creationOptions.FileIds.Add(file.Id);
        }
        vectorStore = await client.CreateVectorStoreAsync(creationOptions);

        Validate(vectorStore);
        Assert.Multiple(() =>
        {
            Assert.That(vectorStore.Name, Is.Null.Or.Empty);
            Assert.That(vectorStore.FileCounts.Total, Is.EqualTo(5));
        });
    }

    [RecordedTest]
    public async Task CanEnumerateVectorStores()
    {
        VectorStoreClient client = GetTestClient();

        int vectorStoreCount = 3;

        for (int i = 0; i < vectorStoreCount; i++)
        {
            VectorStore vectorStore = await client.CreateVectorStoreAsync(
                new VectorStoreCreationOptions()
                {
                    Name = $"Test Vector Store {i}",
                });
            Validate(vectorStore);

            Assert.That(vectorStore.Name, Is.EqualTo($"Test Vector Store {i}"));
        }

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

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

    [RecordedTest]
    public async Task CanAssociateFiles()
    {
        VectorStoreClient client = GetTestClient();
        VectorStore vectorStore = await client.CreateVectorStoreAsync();
        Validate(vectorStore);

        int fileCount = 3;

        IReadOnlyList<OpenAIFile> files = await GetNewTestFiles(fileCount);

        foreach (OpenAIFile file in files)
        {
            VectorStoreFile vectorStoreFile = await client.AddFileToVectorStoreAsync(vectorStore.Id, file.Id);
            Validate(vectorStoreFile);
            Assert.Multiple(() =>
            {
                Assert.That(vectorStoreFile.FileId, Is.EqualTo(file.Id));
                Assert.That(vectorStoreFile.VectorStoreId, Is.EqualTo(vectorStore.Id));
                Assert.That(vectorStoreFile.LastError, Is.Null);
                Assert.That(vectorStoreFile.CreatedAt, Is.GreaterThan(s_2024));
                Assert.That(vectorStoreFile.Status, Is.EqualTo(VectorStoreFileStatus.InProgress));
            });
        }
        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        FileFromStoreRemovalResult removalResult = await client.RemoveFileFromVectorStoreAsync(vectorStore.Id, files[0].Id);
        Assert.That(removalResult.FileId, Is.EqualTo(files[0].Id));
        Assert.That(removalResult.Removed);
        _vectorStoreFilesToRemove.RemoveAt(0);

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        int count = 0;

        await foreach (VectorStoreFile vectorStoreFile in client.GetVectorStoreFilesAsync(vectorStore.Id))
        {
            count++;
            Assert.That(vectorStoreFile.FileId, Is.Not.EqualTo(files[0].Id));
            Assert.That(vectorStoreFile.VectorStoreId, Is.EqualTo(vectorStore.Id));
        }
        Assert.That(count, Is.EqualTo(fileCount - 1));
    }

    [RecordedTest]
    public async Task Pagination_CanRehydrateVectorStoreFileCollection()
    {
        VectorStoreClient client = GetTestClient();
        VectorStore vectorStore = await client.CreateVectorStoreAsync();
        Validate(vectorStore);

        IReadOnlyList<OpenAIFile> files = await GetNewTestFiles(6);

        foreach (OpenAIFile file in files)
        {
            VectorStoreFile vectorStoreFile = await client.AddFileToVectorStoreAsync(vectorStore.Id, file.Id);
            Validate(vectorStoreFile);
            Assert.Multiple(() =>
            {
                Assert.That(vectorStoreFile.FileId, Is.EqualTo(file.Id));
                Assert.That(vectorStoreFile.VectorStoreId, Is.EqualTo(vectorStore.Id));
                Assert.That(vectorStoreFile.LastError, Is.Null);
                Assert.That(vectorStoreFile.CreatedAt, Is.GreaterThan(s_2024));
                Assert.That(vectorStoreFile.Status, Is.EqualTo(VectorStoreFileStatus.InProgress));
            });
        }

        FileFromStoreRemovalResult removalResult = await client.RemoveFileFromVectorStoreAsync(vectorStore.Id, files[0].Id);
        Assert.That(removalResult.FileId, Is.EqualTo(files[0].Id));

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        // We added 6 files and will get pages with 2 items, so expect three pages in the collection.

        // Use enumerators instead of enumerables to faciliate advancing the collections
        // at the same time.
        AsyncCollectionResult<VectorStoreFile> vectorStoreFiles = client.GetVectorStoreFilesAsync(vectorStore.Id, new VectorStoreFileCollectionOptions() { PageSizeLimit = 2 });
        IAsyncEnumerable<ClientResult> pages = vectorStoreFiles.GetRawPagesAsync();
        IAsyncEnumerator<ClientResult> pageEnumerator = pages.GetAsyncEnumerator();
        await pageEnumerator.MoveNextAsync();
        ClientResult firstPage = pageEnumerator.Current;
        ContinuationToken nextPageToken = vectorStoreFiles.GetContinuationToken(firstPage);

        // Simulate rehydration of the collection
        BinaryData rehydrationBytes = nextPageToken.ToBytes();
        ContinuationToken rehydrationToken = ContinuationToken.FromBytes(rehydrationBytes);

        AsyncCollectionResult<VectorStoreFile> rehydratedFile = client.GetVectorStoreFilesAsync(vectorStore.Id, new VectorStoreFileCollectionOptions { AfterId = rehydrationToken.ToBytes().ToString(), PageSizeLimit = 2 });
        IAsyncEnumerable<ClientResult> rehydratedPages = rehydratedFile.GetRawPagesAsync();
        IAsyncEnumerator<ClientResult> rehydratedPageEnumerator = rehydratedPages.GetAsyncEnumerator();

        int pageCount = 0;

        while (await pageEnumerator.MoveNextAsync() && await rehydratedPageEnumerator.MoveNextAsync())
        {
            ClientResult page = pageEnumerator.Current;
            ClientResult rehydratedPage = rehydratedPageEnumerator.Current;

            List<VectorStoreFile> itemsInPage = GetVectorStoreFilesFromPage(page).ToList();
            List<VectorStoreFile> itemsInRehydratedPage = GetVectorStoreFilesFromPage(rehydratedPage).ToList();

            Assert.That(itemsInRehydratedPage.Count, Is.EqualTo(itemsInPage.Count));

            for (int i = 0; i < itemsInPage.Count; i++)
            {
                Assert.That(itemsInRehydratedPage[0].FileId, Is.EqualTo(itemsInPage[0].FileId));
                Assert.That(itemsInRehydratedPage[0].VectorStoreId, Is.EqualTo(itemsInPage[0].VectorStoreId));
                Assert.That(itemsInRehydratedPage[0].CreatedAt, Is.EqualTo(itemsInPage[0].CreatedAt));
                Assert.That(itemsInRehydratedPage[0].Size, Is.EqualTo(itemsInPage[0].Size));
            }

            pageCount++;
        }

        // Since we rehydrated the collection at the second page, we expect to
        // see two of the remaining three pages in the collection.
        Assert.That(pageCount, Is.EqualTo(2));
    }

    [RecordedTest]
    public async Task CanPaginateGetVectorStoreFilesInBatch()
    {
        VectorStoreClient client = GetTestClient();
        VectorStore vectorStore = await client.CreateVectorStoreAsync();
        Validate(vectorStore);

        // Create enough files to ensure we get multiple pages
        IReadOnlyList<OpenAIFile> testFiles = await GetNewTestFiles(10);

        VectorStoreFileBatch fileBatch = await client.AddFileBatchToVectorStoreAsync(vectorStore.Id, testFiles?.Select(file => file.Id));

        Assert.Multiple(() =>
        {
            Assert.That(fileBatch.BatchId, Is.Not.Null);
            Assert.That(fileBatch.VectorStoreId, Is.EqualTo(vectorStore.Id));
        });

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        // Test basic pagination with PageSizeLimit
        var options = new VectorStoreFileCollectionOptions { PageSizeLimit = 3 };
        AsyncCollectionResult<VectorStoreFile> vectorStoreFiles = client.GetVectorStoreFilesInBatchAsync(
            vectorStore.Id, fileBatch.BatchId, options);

        int totalItemsCount = 0;
        int pageCount = 0;
        List<string> seenFileIds = new List<string>();

        await foreach (VectorStoreFile vectorStoreFile in vectorStoreFiles)
        {
            totalItemsCount++;
            seenFileIds.Add(vectorStoreFile.FileId);

            Assert.Multiple(() =>
            {
                Assert.That(vectorStoreFile.FileId, Is.Not.Null);
                Assert.That(vectorStoreFile.VectorStoreId, Is.EqualTo(vectorStore.Id));
            });
        }

        // Verify we got all the files
        Assert.That(totalItemsCount, Is.EqualTo(10));
        Assert.That(seenFileIds.Distinct().Count(), Is.EqualTo(10));

        // Now test pagination by examining raw pages
        AsyncCollectionResult<VectorStoreFile> pagedVectorStoreFiles = client.GetVectorStoreFilesInBatchAsync(
            vectorStore.Id, fileBatch.BatchId, new VectorStoreFileCollectionOptions { PageSizeLimit = 3 });

        IAsyncEnumerable<ClientResult> pages = pagedVectorStoreFiles.GetRawPagesAsync();
        IAsyncEnumerator<ClientResult> pageEnumerator = pages.GetAsyncEnumerator();

        pageCount = 0;
        int itemsInPages = 0;

        while (await pageEnumerator.MoveNextAsync())
        {
            ClientResult page = pageEnumerator.Current;
            pageCount++;

            IEnumerable<VectorStoreFile> itemsInPage = GetVectorStoreFilesFromPage(page);
            int pageItemCount = itemsInPage.Count();
            itemsInPages += pageItemCount;

            // Each page should have at most 3 items (except possibly the last page)
            Assert.That(pageItemCount, Is.LessThanOrEqualTo(3));
            Assert.That(pageItemCount, Is.GreaterThan(0));
        }

        // We should have at least 4 pages (10 items with page size 3)
        Assert.That(pageCount, Is.GreaterThanOrEqualTo(4));
        Assert.That(itemsInPages, Is.EqualTo(10));
    }

    [RecordedTest]
    public async Task CanTestGetVectorStoreFilesInBatchCollectionOptions()
    {
        VectorStoreClient client = GetTestClient();
        VectorStore vectorStore = await client.CreateVectorStoreAsync();
        Validate(vectorStore);
        int pageSizeLimit = 2;
        int fileCount = pageSizeLimit + 1;

        IReadOnlyList<OpenAIFile> testFiles = await GetNewTestFiles(fileCount);

        VectorStoreFileBatch fileBatch = await client.AddFileBatchToVectorStoreAsync(vectorStore.Id, testFiles?.Select(file => file.Id));
        Validate(fileBatch);

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        // Test Order property - Ascending vs Descending
        var ascendingOptions = new VectorStoreFileCollectionOptions 
        { 
            Order = VectorStoreFileCollectionOrder.Ascending,
            PageSizeLimit = pageSizeLimit
        };
        var descendingOptions = new VectorStoreFileCollectionOptions 
        { 
            Order = VectorStoreFileCollectionOrder.Descending,
            PageSizeLimit = pageSizeLimit
        };

        List<string> ascendingIds = [];
        List<string> descendingIds = [];

        await foreach (VectorStoreFile vectorStoreFile in client.GetVectorStoreFilesInBatchAsync(vectorStore.Id, fileBatch.BatchId, ascendingOptions))
        {
            ascendingIds.Add(vectorStoreFile.FileId);
        }

        await foreach (VectorStoreFile vectorStoreFile in client.GetVectorStoreFilesInBatchAsync(vectorStore.Id, fileBatch.BatchId, descendingOptions))
        {
            descendingIds.Add(vectorStoreFile.FileId);
        }

        Assert.That(ascendingIds.Count, Is.EqualTo(fileCount));
        Assert.That(ascendingIds.Count, Is.EqualTo(descendingIds.Count));

        // Test Filter property - only get completed files (which should be all of them after batch completion)
        var filterOptions = new VectorStoreFileCollectionOptions 
        { 
            Filter = VectorStoreFileStatusFilter.Completed 
        };

        int completedCount = 0;
        await foreach (VectorStoreFile vectorStoreFile in client.GetVectorStoreFilesInBatchAsync(vectorStore.Id, fileBatch.BatchId, filterOptions))
        {
            completedCount++;
            Assert.That(vectorStoreFile.Status, Is.EqualTo(VectorStoreFileStatus.Completed));
        }

        Assert.That(completedCount, Is.EqualTo(fileCount)); // Should match the number of files we uploaded

        // Test AfterId property - get vector store files after a specific ID
        var firstVectorStoreFile = ascendingIds.FirstOrDefault();
        if (!string.IsNullOrEmpty(firstVectorStoreFile))
        {
            var afterOptions = new VectorStoreFileCollectionOptions 
            { 
                AfterId = firstVectorStoreFile,
                Order = VectorStoreFileCollectionOrder.Ascending 
            };

            List<string> afterIds = new List<string>();
            await foreach (VectorStoreFile vectorStoreFile in client.GetVectorStoreFilesInBatchAsync(vectorStore.Id, fileBatch.BatchId, afterOptions))
            {
                afterIds.Add(vectorStoreFile.FileId);
            }

            // Should have one less item (excluding the first one)
            Assert.That(afterIds.Count, Is.EqualTo(ascendingIds.Count - 1));
            // Should not contain the first ID
            Assert.That(afterIds.Contains(firstVectorStoreFile), Is.False);
        }

        // Test BeforeId property - get vector store files before a specific ID
        var lastVectorStoreFile = ascendingIds.LastOrDefault();
        if (!string.IsNullOrEmpty(lastVectorStoreFile))
        {
            var beforeOptions = new VectorStoreFileCollectionOptions 
            { 
                BeforeId = lastVectorStoreFile,
                Order = VectorStoreFileCollectionOrder.Ascending 
            };

            List<string> beforeIds = new List<string>();
            await foreach (VectorStoreFile vectorStoreFile in client.GetVectorStoreFilesInBatchAsync(vectorStore.Id, fileBatch.BatchId, beforeOptions))
            {
                beforeIds.Add(vectorStoreFile.FileId);
            }

            // Should have one less item (excluding the last one)
            Assert.That(beforeIds.Count, Is.EqualTo(ascendingIds.Count - 1));
            // Should not contain the last ID
            Assert.That(beforeIds.Contains(lastVectorStoreFile), Is.False);
        }
    }

    [RecordedTest]
    public async Task CanRehydrateGetVectorStoreFilesInBatchPagination()
    {
        VectorStoreClient client = GetTestClient();
        VectorStore vectorStore = await client.CreateVectorStoreAsync();
        Validate(vectorStore);
        
        IReadOnlyList<OpenAIFile> testFiles = await GetNewTestFiles(6);

        VectorStoreFileBatch fileBatch = await client.AddFileBatchToVectorStoreAsync(vectorStore.Id, testFiles?.Select(file => file.Id));
        Validate(fileBatch);

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        // We added 6 files and will get pages with 2 items, so expect three pages in the collection.
        AsyncCollectionResult<VectorStoreFile> vectorStoreFiles = client.GetVectorStoreFilesInBatchAsync(
            vectorStore.Id, fileBatch.BatchId, new VectorStoreFileCollectionOptions { PageSizeLimit = 2 });

        IAsyncEnumerable<ClientResult> pages = vectorStoreFiles.GetRawPagesAsync();
        IAsyncEnumerator<ClientResult> pageEnumerator = pages.GetAsyncEnumerator();
        await pageEnumerator.MoveNextAsync();
        ClientResult firstPage = pageEnumerator.Current;
        ContinuationToken nextPageToken = vectorStoreFiles.GetContinuationToken(firstPage);

        // Simulate rehydration of the collection
        BinaryData rehydrationBytes = nextPageToken.ToBytes();
        ContinuationToken rehydrationToken = ContinuationToken.FromBytes(rehydrationBytes);

        AsyncCollectionResult<VectorStoreFile> rehydratedFiles = client.GetVectorStoreFilesInBatchAsync(
            vectorStore.Id, fileBatch.BatchId, new VectorStoreFileCollectionOptions { AfterId = rehydrationToken.ToBytes().ToString(), PageSizeLimit = 2 });

        IAsyncEnumerable<ClientResult> rehydratedPages = rehydratedFiles.GetRawPagesAsync();
        IAsyncEnumerator<ClientResult> rehydratedPageEnumerator = rehydratedPages.GetAsyncEnumerator();

        int pageCount = 0;

        while (await pageEnumerator.MoveNextAsync() && await rehydratedPageEnumerator.MoveNextAsync())
        {
            ClientResult page = pageEnumerator.Current;
            ClientResult rehydratedPage = rehydratedPageEnumerator.Current;

            List<VectorStoreFile> itemsInPage = GetVectorStoreFilesFromPage(page).ToList();
            List<VectorStoreFile> itemsInRehydratedPage = GetVectorStoreFilesFromPage(rehydratedPage).ToList();

            Assert.That(itemsInPage.Count, Is.EqualTo(itemsInRehydratedPage.Count));

            for (int i = 0; i < itemsInPage.Count; i++)
            {
                Assert.That(itemsInPage[i].FileId, Is.EqualTo(itemsInRehydratedPage[i].FileId));
                Assert.That(itemsInPage[i].VectorStoreId, Is.EqualTo(itemsInRehydratedPage[i].VectorStoreId));
                Assert.That(itemsInPage[i].CreatedAt, Is.EqualTo(itemsInRehydratedPage[i].CreatedAt));
                Assert.That(itemsInPage[i].Status, Is.EqualTo(itemsInRehydratedPage[i].Status));
            }

            pageCount++;
        }

        // Since we rehydrated the collection at the second page, we expect to
        // see two of the remaining three pages in the collection.
        Assert.That(pageCount, Is.EqualTo(2));
    }

    private static IEnumerable<VectorStoreFile> GetVectorStoreFilesFromPage(ClientResult page)
    {
        PipelineResponse response = page.GetRawResponse();
        JsonDocument doc = JsonDocument.Parse(response.Content);
        IEnumerable<JsonElement> els = doc.RootElement.GetProperty("data").EnumerateArray();

        // TODO: improve perf
        return els.Select(el => ModelReaderWriter.Read<VectorStoreFile>(BinaryData.FromString(el.GetRawText())));
    }

    [RecordedTest]
    public async Task CanUseBatchIngestion()
    {
        VectorStoreClient client = GetTestClient();
        VectorStore vectorStore = await client.CreateVectorStoreAsync();
        Validate(vectorStore);

        IReadOnlyList<OpenAIFile> testFiles = await GetNewTestFiles(5);

        VectorStoreFileBatch fileBatch = await client.AddFileBatchToVectorStoreAsync(vectorStore.Id, testFiles?.Select(file => file.Id));
        Validate(fileBatch);

        Assert.Multiple(() =>
        {
            Assert.That(fileBatch.BatchId, Is.Not.Null);
            Assert.That(fileBatch.VectorStoreId, Is.EqualTo(vectorStore.Id));
            Assert.That(fileBatch.Status, Is.EqualTo(VectorStoreFileBatchStatus.InProgress));
        });

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        await foreach (VectorStoreFile vectorStoreFile in client.GetVectorStoreFilesAsync(fileBatch.VectorStoreId))
        {
            Assert.Multiple(() =>
            {
                Assert.That(vectorStoreFile.FileId, Is.Not.Null);
                Assert.That(vectorStoreFile.VectorStoreId, Is.EqualTo(vectorStore.Id));
                Assert.That(vectorStoreFile.Status, Is.EqualTo(VectorStoreFileStatus.Completed));
                // Assert.That(vectorStoreFile.Size, Is.GreaterThan(0));
                Assert.That(vectorStoreFile.CreatedAt, Is.GreaterThan(s_2024));
                Assert.That(vectorStoreFile.LastError, Is.Null);
            });
        }
    }

    [RecordedTest]
    [TestCase(ChunkingStrategyKind.Auto)]
    [TestCase(ChunkingStrategyKind.Static)]
    public async Task CanApplyChunkingStrategy(ChunkingStrategyKind strategyKind)
    {
        IReadOnlyList<OpenAIFile> testFiles = await GetNewTestFiles(5);

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
        VectorStore vectorStore = await client.CreateVectorStoreAsync(creationOptions);
        Validate(vectorStore);
        Assert.That(vectorStore.FileCounts.Total, Is.EqualTo(5));

        AsyncCollectionResult<VectorStoreFile> vectorStoreFiles = client.GetVectorStoreFilesAsync(vectorStore.Id);

        await foreach (VectorStoreFile vectorStoreFile in vectorStoreFiles)
        {
            Assert.That(testFiles.Any(file => file.Id == vectorStoreFile.FileId), Is.True);
            Assert.That(vectorStoreFile.ChunkingStrategy, Is.InstanceOf<StaticFileChunkingStrategy>());
            StaticFileChunkingStrategy staticStrategy = vectorStoreFile.ChunkingStrategy as StaticFileChunkingStrategy;

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

    [RecordedTest]
    public async Task CanGetVectorStores()
    {
        VectorStoreClient client = GetTestClient();

        IReadOnlyList<OpenAIFile> testFiles = await GetNewTestFiles(5);
        VectorStoreCreationOptions creationOptions = new()
        {
            Name = "test vector store",
            ExpirationPolicy = new VectorStoreExpirationPolicy(VectorStoreExpirationAnchor.LastActiveAt, 3),
            Metadata =
            {
                ["test-key"] = "test-value",
            },
        };
        foreach (var file in testFiles)
        {
            creationOptions.FileIds.Add(file.Id);
        }

        VectorStore vectorStore = await client.CreateVectorStoreAsync(creationOptions);

        Validate(vectorStore);

        if (IsAsync)
        {
            await foreach (VectorStoreFile vectorStoreFile in client.GetVectorStoreFilesAsync(vectorStore.Id))
            {
                Assert.Multiple(() =>
                {
                    Assert.That(vectorStoreFile.FileId, Is.Not.Null);
                    Assert.That(vectorStoreFile.VectorStoreId, Is.EqualTo(vectorStore.Id));
                    Assert.That(vectorStoreFile.LastError, Is.Null);
                    Assert.That(vectorStoreFile.CreatedAt, Is.GreaterThan(s_2024));
                    Assert.That(vectorStoreFile.Status, Is.EqualTo(VectorStoreFileStatus.InProgress));
                });
            }
        }

        var deletionResult = await client.DeleteVectorStoreAsync(vectorStore.Id);
        Assert.That(deletionResult.Value.VectorStoreId, Is.EqualTo(vectorStore.Id));
        Assert.That(deletionResult.Value.Deleted, Is.True);
        _vectorStoresToDelete.RemoveAt(_vectorStoresToDelete.Count - 1);

        creationOptions = new VectorStoreCreationOptions();
        foreach (var file in testFiles)
        {
            creationOptions.FileIds.Add(file.Id);
        }

    }

    private async Task<IReadOnlyList<OpenAIFile>> GetNewTestFiles(int count)
    {
        List<OpenAIFile> files = [];

        using (Recording.DisableRequestBodyRecording()) // Temp pending https://github.com/Azure/azure-sdk-tools/issues/11901
        {
            OpenAIFileClient client = GetProxiedOpenAIClient<OpenAIFileClient>(TestScenario.Files);
            for (int i = 0; i < count; i++)
            {
                OpenAIFile file = await client.UploadFileAsync(
                    BinaryData.FromString($"This is a test file {i}").ToStream(),
                    $"test_file_{i.ToString().PadLeft(3, '0')}.txt",
                    FileUploadPurpose.Assistants);
                Validate(file);
                files.Add(file);
            }
        }

        return files;
    }

    [TearDown]
    protected async Task Cleanup()
    {
        if (Mode == RecordedTestMode.Playback)
        {
            return;
        }

        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        VectorStoreClient vectorStoreClient = GetTestClient<VectorStoreClient>(TestScenario.VectorStores);
        RequestOptions requestOptions = new()
        {
            ErrorOptions = ClientErrorBehaviors.NoThrow,
        };

        foreach (VectorStoreFileBatch fileBatch in _fileBatchToCancel)
        {
            Console.WriteLine($"Cleanup: {fileBatch.BatchId} => {(await vectorStoreClient.CancelVectorStoreFileBatchAsync(fileBatch.VectorStoreId, fileBatch.BatchId, requestOptions))?.GetRawResponse()?.Status}");
        }
        foreach (VectorStoreFile vectorStoreFile in _vectorStoreFilesToRemove)
        {
            ClientResult protocolResult = await vectorStoreClient.RemoveFileFromVectorStoreAsync(vectorStoreFile.VectorStoreId, vectorStoreFile.FileId, requestOptions);
            Console.WriteLine($"Cleanup: {vectorStoreFile.FileId}<->{vectorStoreFile.VectorStoreId} => {protocolResult?.GetRawResponse()?.Status}");
        }
        foreach (OpenAIFile file in _filesToDelete)
        {
            Console.WriteLine($"Cleanup: {file.Id} -> {(await fileClient.DeleteFileAsync(file.Id, requestOptions))?.GetRawResponse()?.Status}");
        }
        foreach (VectorStore vectorStore in _vectorStoresToDelete)
        {
            Console.WriteLine($"Cleanup: {vectorStore.Id} => {(await vectorStoreClient.DeleteVectorStoreAsync(vectorStore.Id, requestOptions))?.GetRawResponse()?.Status}");
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
        if (target is VectorStoreFileBatch job)
        {
            Assert.That(job.BatchId, Is.Not.Null);
            _fileBatchToCancel.Add(job);
        }
        else if (target is VectorStoreFile vectorStoreFile)
        {
            Assert.That(vectorStoreFile?.FileId, Is.Not.Null);
            Assert.That(vectorStoreFile?.VectorStoreId, Is.Not.Null);
            _vectorStoreFilesToRemove.Add(vectorStoreFile);
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

    private VectorStoreClient GetTestClient() => GetProxiedOpenAIClient<VectorStoreClient>(TestScenario.VectorStores);
}

#pragma warning restore OPENAI001
