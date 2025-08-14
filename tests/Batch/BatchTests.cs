using NUnit.Framework;
using OpenAI.Batch;
using OpenAI.Files;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Batch;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Batch")]
public class BatchTests : SyncAsyncTestBase
{
    private static BatchClient GetTestClient() => GetTestClient<BatchClient>(TestScenario.Batch);
    private static readonly DateTimeOffset s_2024 = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public BatchTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public void ListBatchesProtocol()
    {
        AssertSyncOnly();

        BatchClient client = GetTestClient();
        CollectionResult batches = client.GetBatches(after: null, limit: null, options: null);

        int pageCount = 0;
        foreach (ClientResult pageResult in batches.GetRawPages())
        {
            BinaryData response = pageResult.GetRawResponse().Content;
            using JsonDocument jsonDocument = JsonDocument.Parse(response);
            JsonElement dataElement = jsonDocument.RootElement.GetProperty("data");

            Assert.That(dataElement.GetArrayLength(), Is.GreaterThan(0));

            long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

            foreach (JsonElement batchElement in dataElement.EnumerateArray())
            {
                JsonElement createdAtElement = batchElement.GetProperty("created_at");
                long createdAt = createdAtElement.GetInt64();

                Assert.That(createdAt, Is.GreaterThan(unixTime2024));
            }
            pageCount++;
        }

        Assert.GreaterOrEqual(pageCount, 1);
    }

    [Test]
    public async Task ListBatchesProtocolAsync()
    {
        AssertAsyncOnly();

        BatchClient client = GetTestClient();
        AsyncCollectionResult batches = client.GetBatchesAsync(after: null, limit: null, options: null);

        int pageCount = 0;
        await foreach (ClientResult pageResult in batches.GetRawPagesAsync())
        {
            BinaryData response = pageResult.GetRawResponse().Content;
            using JsonDocument jsonDocument = JsonDocument.Parse(response);
            JsonElement dataElement = jsonDocument.RootElement.GetProperty("data");

            Assert.That(dataElement.GetArrayLength(), Is.GreaterThan(0));

            long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

            foreach (JsonElement batchElement in dataElement.EnumerateArray())
            {
                JsonElement createdAtElement = batchElement.GetProperty("created_at");
                long createdAt = createdAtElement.GetInt64();

                Assert.That(createdAt, Is.GreaterThan(unixTime2024));
            }
            pageCount++;
        }

        Assert.GreaterOrEqual(pageCount, 1);
    }

    [Test]
    public async Task ListBatchesAsync_WithOptions_PageSizeLimitAndItems()
    {
        BatchClient client = GetTestClient();

        BatchCollectionOptions options = new()
        {
            PageSizeLimit = 2,
        };

        int itemCount = await ValidateSomeJobsAsync(client, options, maxItems: 3);
        Assert.That(itemCount, Is.GreaterThan(0));

        int pageCount = await ValidatePageSizesAsync(client, options, maxPages: 2, maxPageSize: 2);
        Assert.That(pageCount, Is.GreaterThan(0));
    }

    [Test]
    public async Task ListBatchesAsync_WithOptions_AfterIdStartsFromNextPage()
    {
        BatchClient client = GetTestClient();

        // First fetch: get the first page and capture ids + last_id
        BatchCollectionOptions firstOptions = new()
        {
            PageSizeLimit = 2,
        };
        (string afterId, HashSet<string> firstPageIds) = await GetFirstPageCursorAndIdsAsync(client, firstOptions);
        Assert.That(afterId, Is.Not.Null.And.Not.Empty);
        Assert.That(firstPageIds.Count, Is.GreaterThan(0));

        // Second fetch: start after the last id from the first page
        BatchCollectionOptions secondOptions = new()
        {
            AfterId = afterId,
            PageSizeLimit = 2,
        };
        await AssertNoOverlapWithFirstPageAsync(client, secondOptions, firstPageIds);
    }

    [Test]
    public void ListBatchesAsync_HonorsCancellationToken()
    {
        BatchClient client = GetTestClient();
        var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel();

        BatchCollectionOptions options = new() { PageSizeLimit = 1 };
        
        if (IsAsync)
        {
            var collection = client.GetBatchesAsync(options, cts.Token);
            var enumerator = collection.GetRawPagesAsync().GetAsyncEnumerator();
            Assert.ThrowsAsync<TaskCanceledException>(async () => await enumerator.MoveNextAsync().AsTask());
        }
        else
        {
            var collection = client.GetBatches(options, cts.Token);
            using var enumerator = collection.GetRawPages().GetEnumerator();
            Assert.Throws<OperationCanceledException>(() => enumerator.MoveNext());
        }
    }

    [Test]
    public async Task CreateGetAndCancelBatchProtocol()
    {
        using MemoryStream testFileStream = new();
        using StreamWriter streamWriter = new(testFileStream);
        string input = @"{""custom_id"": ""request-1"", ""method"": ""POST"", ""url"": ""/v1/chat/completions"", ""body"": {""model"": ""gpt-4o-mini"", ""messages"": [{""role"": ""system"", ""content"": ""You are a helpful assistant.""}, {""role"": ""user"", ""content"": ""What is 2+2?""}]}}";
        streamWriter.WriteLine(input);
        streamWriter.Flush();
        testFileStream.Position = 0;

        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        OpenAIFile inputFile = await fileClient.UploadFileAsync(testFileStream, "test-batch-file", FileUploadPurpose.Batch);
        Assert.That(inputFile.Id, Is.Not.Null.And.Not.Empty);

        BatchClient client = GetTestClient();
        BinaryContent content = BinaryContent.Create(BinaryData.FromObjectAsJson(new
        {
            input_file_id = inputFile.Id,
            endpoint = "/v1/chat/completions",
            completion_window = "24h",
            metadata = new
            {
                testMetadataKey = "test metadata value",
            },
        }));
        CreateBatchOperation batchOperation = IsAsync
            ? await client.CreateBatchAsync(content, waitUntilCompleted: false)
            : client.CreateBatch(content, waitUntilCompleted: false);

        BinaryData response = batchOperation.GetRawResponse().Content;
        JsonDocument jsonDocument = JsonDocument.Parse(response);

        JsonElement idElement = jsonDocument.RootElement.GetProperty("id");
        JsonElement createdAtElement = jsonDocument.RootElement.GetProperty("created_at");
        JsonElement statusElement = jsonDocument.RootElement.GetProperty("status");
        JsonElement metadataElement = jsonDocument.RootElement.GetProperty("metadata");
        JsonElement testMetadataKeyElement = metadataElement.GetProperty("testMetadataKey");

        string id = idElement.GetString();
        long createdAt = createdAtElement.GetInt64();
        string status = statusElement.GetString();
        string testMetadataKey = testMetadataKeyElement.GetString();

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(id, Is.Not.Null.And.Not.Empty);
        Assert.That(createdAt, Is.GreaterThan(unixTime2024));
        Assert.That(status, Is.EqualTo("validating"));
        Assert.That(testMetadataKey, Is.EqualTo("test metadata value"));

        JsonElement endpointElement = jsonDocument.RootElement.GetProperty("endpoint");
        string endpoint = endpointElement.GetString();

        Assert.That(endpoint, Is.EqualTo("/v1/chat/completions"));

        ClientResult clientResult = IsAsync
            ? await batchOperation.CancelAsync(options: null)
            : batchOperation.Cancel(options: null);

        statusElement = jsonDocument.RootElement.GetProperty("status");
        status = statusElement.GetString();

        Assert.That(status, Is.EqualTo("validating"));
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task CanRehydrateBatchOperation(bool useBatchId)
    {
        using MemoryStream testFileStream = new();
        using StreamWriter streamWriter = new(testFileStream);
        string input = @"{""custom_id"": ""request-1"", ""method"": ""POST"", ""url"": ""/v1/chat/completions"", ""body"": {""model"": ""gpt-4o-mini"", ""messages"": [{""role"": ""system"", ""content"": ""You are a helpful assistant.""}, {""role"": ""user"", ""content"": ""What is 2+2?""}]}}";
        streamWriter.WriteLine(input);
        streamWriter.Flush();
        testFileStream.Position = 0;

        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        OpenAIFile inputFile = await fileClient.UploadFileAsync(testFileStream, "test-batch-file", FileUploadPurpose.Batch);
        Assert.That(inputFile.Id, Is.Not.Null.And.Not.Empty);

        BatchClient client = GetTestClient();
        BinaryContent content = BinaryContent.Create(BinaryData.FromObjectAsJson(new
        {
            input_file_id = inputFile.Id,
            endpoint = "/v1/chat/completions",
            completion_window = "24h",
            metadata = new
            {
                testMetadataKey = "test metadata value",
            },
        }));

        CreateBatchOperation batchOperation = IsAsync
            ? await client.CreateBatchAsync(content, waitUntilCompleted: false)
            : client.CreateBatch(content, waitUntilCompleted: false);

        CreateBatchOperation rehydratedOperation;
        if (useBatchId)
        {
            rehydratedOperation = IsAsync ?
                await CreateBatchOperation.RehydrateAsync(client, batchOperation.BatchId) :
                CreateBatchOperation.Rehydrate(client, batchOperation.BatchId);
        }
        else {
            // Simulate rehydration of the operation
            BinaryData rehydrationBytes = batchOperation.RehydrationToken.ToBytes();
            ContinuationToken rehydrationToken = ContinuationToken.FromBytes(rehydrationBytes);

            rehydratedOperation = IsAsync ?
                await CreateBatchOperation.RehydrateAsync(client, rehydrationToken) :
                CreateBatchOperation.Rehydrate(client, rehydrationToken);
        }

        static bool Validate(CreateBatchOperation operation)
        {
            BinaryData response = operation.GetRawResponse().Content;
            using JsonDocument jsonDocument = JsonDocument.Parse(response);

            JsonElement idElement = jsonDocument.RootElement.GetProperty("id");
            JsonElement createdAtElement = jsonDocument.RootElement.GetProperty("created_at");
            JsonElement statusElement = jsonDocument.RootElement.GetProperty("status");
            JsonElement metadataElement = jsonDocument.RootElement.GetProperty("metadata");
            JsonElement testMetadataKeyElement = metadataElement.GetProperty("testMetadataKey");

            string id = idElement.GetString();
            long createdAt = createdAtElement.GetInt64();
            string status = statusElement.GetString();
            string testMetadataKey = testMetadataKeyElement.GetString();

            long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(createdAt, Is.GreaterThan(unixTime2024));
            Assert.That(status, Is.EqualTo("validating"));
            Assert.That(testMetadataKey, Is.EqualTo("test metadata value"));

            return true;
        }

        Assert.IsTrue(Validate(batchOperation));
        Assert.IsTrue(Validate(rehydratedOperation));

        // We don't test wait for completion live because this is documented to
        // sometimes take 24 hours.

        Assert.AreEqual(batchOperation.HasCompleted, rehydratedOperation.HasCompleted);

        using JsonDocument originalOperationJson = JsonDocument.Parse(batchOperation.GetRawResponse().Content);
        using JsonDocument rehydratedOperationJson = JsonDocument.Parse(rehydratedOperation.GetRawResponse().Content);

        Assert.AreEqual(originalOperationJson.RootElement.GetProperty("id").GetString(), rehydratedOperationJson.RootElement.GetProperty("id").GetString());
        Assert.AreEqual(originalOperationJson.RootElement.GetProperty("created_at").GetInt64(), rehydratedOperationJson.RootElement.GetProperty("created_at").GetInt64());
        Assert.AreEqual(originalOperationJson.RootElement.GetProperty("status").GetString(), rehydratedOperationJson.RootElement.GetProperty("status").GetString());
    }

    // Helper methods to minimize duplication between sync/async test paths
    private async Task<int> ValidateSomeJobsAsync(BatchClient client, BatchCollectionOptions options, int maxItems)
    {
        int itemCount = 0;
        if (IsAsync)
        {
            AsyncCollectionResult<BatchJob> collection = client.GetBatchesAsync(options);
            await foreach (BatchJob job in collection)
            {
                AssertBasicJobFields(job);
                itemCount++;
                if (itemCount >= maxItems) break;
            }
        }
        else
        {
            CollectionResult<BatchJob> collection = client.GetBatches(options);
            foreach (BatchJob job in collection)
            {
                AssertBasicJobFields(job);
                itemCount++;
                if (itemCount >= maxItems) break;
            }
        }
        return itemCount;
    }

    private async Task<int> ValidatePageSizesAsync(BatchClient client, BatchCollectionOptions options, int maxPages, int maxPageSize)
    {
        int pageCount = 0;
        if (IsAsync)
        {
            AsyncCollectionResult<BatchJob> collection = client.GetBatchesAsync(options);
            await foreach (ClientResult page in collection.GetRawPagesAsync())
            {
                using JsonDocument doc = JsonDocument.Parse(page.GetRawResponse().Content);
                JsonElement data = doc.RootElement.GetProperty("data");
                Assert.That(data.GetArrayLength(), Is.LessThanOrEqualTo(maxPageSize));
                pageCount++;
                if (pageCount >= maxPages) break;
            }
        }
        else
        {
            CollectionResult<BatchJob> collection = client.GetBatches(options);
            foreach (ClientResult page in collection.GetRawPages())
            {
                using JsonDocument doc = JsonDocument.Parse(page.GetRawResponse().Content);
                JsonElement data = doc.RootElement.GetProperty("data");
                Assert.That(data.GetArrayLength(), Is.LessThanOrEqualTo(maxPageSize));
                pageCount++;
                if (pageCount >= maxPages) break;
            }
        }
        return pageCount;
    }

    private async Task<(string afterId, HashSet<string> firstPageIds)> GetFirstPageCursorAndIdsAsync(BatchClient client, BatchCollectionOptions options)
    {
        ClientResult firstPageResult = null;
        if (IsAsync)
        {
            AsyncCollectionResult<BatchJob> firstCollection = client.GetBatchesAsync(options);
            await foreach (ClientResult page in firstCollection.GetRawPagesAsync())
            {
                firstPageResult = page;
                break;
            }
        }
        else
        {
            CollectionResult<BatchJob> firstCollection = client.GetBatches(options);
            foreach (ClientResult page in firstCollection.GetRawPages())
            {
                firstPageResult = page;
                break;
            }
        }
        Assert.That(firstPageResult, Is.Not.Null);

        using JsonDocument firstDoc = JsonDocument.Parse(firstPageResult.GetRawResponse().Content);
        JsonElement firstRoot = firstDoc.RootElement;
        JsonElement firstData = firstRoot.GetProperty("data");
        string afterId = firstRoot.TryGetProperty("last_id", out var lastIdProp) ? lastIdProp.GetString() : null;
        var firstPageIds = firstData.EnumerateArray().Select(e => e.GetProperty("id").GetString()).ToHashSet();
        
        return (afterId, firstPageIds);
    }

    private async Task AssertNoOverlapWithFirstPageAsync(BatchClient client, BatchCollectionOptions options, HashSet<string> firstPageIds)
    {
        ClientResult secondPageResult = null;
        if (IsAsync)
        {
            AsyncCollectionResult<BatchJob> secondCollection = client.GetBatchesAsync(options);
            await foreach (ClientResult page in secondCollection.GetRawPagesAsync())
            {
                secondPageResult = page;
                break;
            }
        }
        else
        {
            CollectionResult<BatchJob> secondCollection = client.GetBatches(options);
            foreach (ClientResult page in secondCollection.GetRawPages())
            {
                secondPageResult = page;
                break;
            }
        }
        Assert.That(secondPageResult, Is.Not.Null);
        
        using JsonDocument secondDoc = JsonDocument.Parse(secondPageResult.GetRawResponse().Content);
        JsonElement secondData = secondDoc.RootElement.GetProperty("data");
        foreach (var item in secondData.EnumerateArray())
        {
            string id = item.GetProperty("id").GetString();
            Assert.That(firstPageIds.Contains(id), Is.False, "Items after the provided cursor should not repeat the first page items.");
        }
    }

    private static void AssertBasicJobFields(BatchJob job)
    {
        Assert.That(job.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(job.CreatedAt, Is.GreaterThan(s_2024));
    }
}