using Microsoft.ClientModel.TestFramework;
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

namespace OpenAI.Tests.Batch;

[Category("Batch")]
[TestFixture(true)]
[TestFixture(false)]
public class BatchTests : OpenAIRecordedTestBase
{
    private static readonly DateTimeOffset s_2024 = new(2024, 01, 01, 0, 0, 0, TimeSpan.Zero);

    public BatchTests(bool isAsync) : base(isAsync)
    {
        TestTimeoutInSeconds = 65;
    }

    [RecordedTest]
    public async Task ListBatchesProtocol()
    {
        BatchClient client = GetProxiedOpenAIClient<BatchClient>();
        AsyncCollectionResult batches = client.GetBatchesAsync(after: null, limit: null, options: null);

        int pageCount = 0;
        await foreach (ClientResult pageResult in batches.GetRawPagesAsync())
        {
            BinaryData response = pageResult.GetRawResponse().Content;
            using JsonDocument jsonDocument = JsonDocument.Parse(response);
            JsonElement dataElement = jsonDocument.RootElement.GetProperty("data");

            Assert.That(dataElement.GetArrayLength(), Is.GreaterThan(0));

            foreach (JsonElement batchElement in dataElement.EnumerateArray())
            {
                JsonElement createdAtElement = batchElement.GetProperty("created_at");
                long createdAt = createdAtElement.GetInt64();

                Assert.That(createdAt, Is.GreaterThan(s_2024.ToUnixTimeSeconds()));
            }
            pageCount++;
        }

        Assert.That(pageCount, Is.GreaterThanOrEqualTo(1));
    }

    [RecordedTest]
    public async Task ListBatchesAsync_WithOptions_PageSizeLimitAndItems()
    {
        BatchClient client = GetProxiedOpenAIClient<BatchClient>();
        BatchCollectionOptions options = new()
        {
            PageSizeLimit = 2,
        };

        int itemCount = await ValidateSomeJobsAsync(client, options, maxItems: 3);
        Assert.That(itemCount, Is.GreaterThan(0));

        int pageCount = await ValidatePageSizesAsync(client, options, maxPages: 2, maxPageSize: 2);
        Assert.That(pageCount, Is.GreaterThan(0));
    }

    [RecordedTest]
    public async Task ListBatchesAsync_WithOptions_AfterIdStartsFromNextPage()
    {
        BatchClient client = GetProxiedOpenAIClient<BatchClient>();

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

    [RecordedTest]
    public void ListBatchesAsync_HonorsCancellationToken()
    {
        BatchClient client = GetProxiedOpenAIClient<BatchClient>();
        var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel();

        BatchCollectionOptions options = new() { PageSizeLimit = 1 };

        var collection = client.GetBatchesAsync(options, cts.Token);
        var enumerator = collection.GetRawPagesAsync().GetAsyncEnumerator();
        Assert.ThrowsAsync<TaskCanceledException>(async () => await enumerator.MoveNextAsync().AsTask());
    }

    [RecordedTest]
    public async Task CreateGetAndCancelBatchProtocol()
    {
        using MemoryStream testFileStream = new();
        using StreamWriter streamWriter = new(testFileStream);
        string input = @"{""custom_id"": ""request-1"", ""method"": ""POST"", ""url"": ""/v1/chat/completions"", ""body"": {""model"": ""gpt-4o-mini"", ""messages"": [{""role"": ""system"", ""content"": ""You are a helpful assistant.""}, {""role"": ""user"", ""content"": ""What is 2+2?""}]}}";
        streamWriter.WriteLine(input);
        streamWriter.Flush();
        testFileStream.Position = 0;

        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();
        OpenAIFile inputFile = await fileClient.UploadFileAsync(testFileStream, "test-batch-file", FileUploadPurpose.Batch);
        Assert.That(inputFile.Id, Is.Not.Null.And.Not.Empty);

        BatchClient client = GetProxiedOpenAIClient<BatchClient>();
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
        CreateBatchOperation batchOperation = await client.CreateBatchAsync(content, waitUntilCompleted: false);

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

        Assert.That(id, Is.Not.Null.And.Not.Empty);
        Assert.That(createdAt, Is.GreaterThan(s_2024.ToUnixTimeSeconds()));
        Assert.That(status, Is.EqualTo("validating"));
        Assert.That(testMetadataKey, Is.EqualTo("test metadata value"));

        JsonElement endpointElement = jsonDocument.RootElement.GetProperty("endpoint");
        string endpoint = endpointElement.GetString();

        Assert.That(endpoint, Is.EqualTo("/v1/chat/completions"));

        ClientResult clientResult = await batchOperation.CancelAsync(options: null);

        statusElement = jsonDocument.RootElement.GetProperty("status");
        status = statusElement.GetString();

        Assert.That(status, Is.EqualTo("validating"));
    }

    [RecordedTest]
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

        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();
        OpenAIFile inputFile = await fileClient.UploadFileAsync(testFileStream, "test-batch-file", FileUploadPurpose.Batch);
        Assert.That(inputFile.Id, Is.Not.Null.And.Not.Empty);

        BatchClient client = GetProxiedOpenAIClient<BatchClient>();
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

        CreateBatchOperation batchOperation = await client.CreateBatchAsync(content, waitUntilCompleted: false);

        CreateBatchOperation rehydratedOperation;
        if (useBatchId)
        {
            rehydratedOperation = await CreateBatchOperation.RehydrateAsync(client, batchOperation.BatchId);
        }
        else
        {
            // Simulate rehydration of the operation
            BinaryData rehydrationBytes = batchOperation.RehydrationToken.ToBytes();
            ContinuationToken rehydrationToken = ContinuationToken.FromBytes(rehydrationBytes);
            rehydratedOperation = await CreateBatchOperation.RehydrateAsync(client, rehydrationToken);
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

            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(createdAt, Is.GreaterThan(s_2024.ToUnixTimeSeconds()));
            Assert.That(status, Is.EqualTo("validating"));
            Assert.That(testMetadataKey, Is.EqualTo("test metadata value"));

            return true;
        }

        Assert.That(Validate(batchOperation));
        Assert.That(Validate(rehydratedOperation));

        // We don't test wait for completion live because this is documented to
        // sometimes take 24 hours.

        Assert.That(rehydratedOperation.HasCompleted, Is.EqualTo(batchOperation.HasCompleted));

        using JsonDocument originalOperationJson = JsonDocument.Parse(batchOperation.GetRawResponse().Content);
        using JsonDocument rehydratedOperationJson = JsonDocument.Parse(rehydratedOperation.GetRawResponse().Content);

        Assert.That(rehydratedOperationJson.RootElement.GetProperty("id").GetString(), Is.EqualTo(originalOperationJson.RootElement.GetProperty("id").GetString()));
        Assert.That(rehydratedOperationJson.RootElement.GetProperty("created_at").GetInt64(), Is.EqualTo(originalOperationJson.RootElement.GetProperty("created_at").GetInt64()));
        Assert.That(rehydratedOperationJson.RootElement.GetProperty("status").GetString(), Is.EqualTo(originalOperationJson.RootElement.GetProperty("status").GetString()));
    }

    private async Task<int> ValidateSomeJobsAsync(BatchClient client, BatchCollectionOptions options, int maxItems)
    {
        int itemCount = 0;
        AsyncCollectionResult<BatchJob> collection = client.GetBatchesAsync(options);
        await foreach (BatchJob job in collection)
        {
            AssertBasicJobFields(job);
            itemCount++;
            if (itemCount >= maxItems) break;
        }
        return itemCount;
    }

    private async Task<int> ValidatePageSizesAsync(BatchClient client, BatchCollectionOptions options, int maxPages, int maxPageSize)
    {
        int pageCount = 0;

        AsyncCollectionResult<BatchJob> collection = client.GetBatchesAsync(options);
        await foreach (ClientResult page in collection.GetRawPagesAsync())
        {
            using JsonDocument doc = JsonDocument.Parse(page.GetRawResponse().Content);
            JsonElement data = doc.RootElement.GetProperty("data");
            Assert.That(data.GetArrayLength(), Is.LessThanOrEqualTo(maxPageSize));
            pageCount++;
            if (pageCount >= maxPages) break;
        }

        return pageCount;
    }

    private async Task<(string afterId, HashSet<string> firstPageIds)> GetFirstPageCursorAndIdsAsync(BatchClient client, BatchCollectionOptions options)
    {
        ClientResult firstPageResult = null;

        AsyncCollectionResult<BatchJob> firstCollection = client.GetBatchesAsync(options);
        await foreach (ClientResult page in firstCollection.GetRawPagesAsync())
        {
            firstPageResult = page;
            break;
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

        AsyncCollectionResult<BatchJob> secondCollection = client.GetBatchesAsync(options);
        await foreach (ClientResult page in secondCollection.GetRawPagesAsync())
        {
            secondPageResult = page;
            break;
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