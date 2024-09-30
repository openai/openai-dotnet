using NUnit.Framework;
using OpenAI.Batch;
using OpenAI.Files;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
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

    [Test]
    public async Task CanRehydrateBatchOperation()
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

        // Simulate rehydration of the operation
        BinaryData rehydrationBytes = batchOperation.RehydrationToken.ToBytes();
        ContinuationToken rehydrationToken = ContinuationToken.FromBytes(rehydrationBytes);

        CreateBatchOperation rehydratedOperation = IsAsync ?
            await CreateBatchOperation.RehydrateAsync(client, rehydrationToken) :
            CreateBatchOperation.Rehydrate(client, rehydrationToken);

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
}