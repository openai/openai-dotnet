using NUnit.Framework;
using OpenAI.Batch;
using OpenAI.Files;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Batch;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Batch")]
public partial class BatchTests : SyncAsyncTestBase
{
    public BatchTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task ListBatchesProtocol()
    {
        BatchClient client = GetTestClient();
        ClientResult result = IsAsync
            ? await client.GetBatchesAsync(after: null, limit: null, options: null)
            : client.GetBatches(after: null, limit: null, options: null);

        BinaryData response = result.GetRawResponse().Content;
        JsonDocument jsonDocument = JsonDocument.Parse(response);
        JsonElement dataElement = jsonDocument.RootElement.GetProperty("data");

        Assert.That(dataElement.GetArrayLength(), Is.GreaterThan(0));

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        foreach (JsonElement batchElement in dataElement.EnumerateArray())
        {
            JsonElement createdAtElement = batchElement.GetProperty("created_at");
            long createdAt = createdAtElement.GetInt64();

            Assert.That(createdAt, Is.GreaterThan(unixTime2024));
        }

        //var dynamicResult = result.GetRawResponse().Content.ToDynamicFromJson();
        //Assert.That(dynamicResult.data.Count, Is.GreaterThan(0));
        //Assert.That(dynamicResult.data[0].createdAt, Is.GreaterThan(new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)));
    }

    [Test]
    public async Task CreateGetAndCancelBatchProtocol()
    {
        using MemoryStream testFileStream = new();
        using StreamWriter streamWriter = new (testFileStream);
        string input = @"{""custom_id"": ""request-1"", ""method"": ""POST"", ""url"": ""/v1/chat/completions"", ""body"": {""model"": ""gpt-4o-mini"", ""messages"": [{""role"": ""system"", ""content"": ""You are a helpful assistant.""}, {""role"": ""user"", ""content"": ""What is 2+2?""}]}}";
        streamWriter.WriteLine(input);
        streamWriter.Flush();
        testFileStream.Position = 0;

        FileClient fileClient = GetTestClient<FileClient>(TestScenario.Files);
        OpenAIFileInfo inputFile = await fileClient.UploadFileAsync(testFileStream, "test-batch-file", FileUploadPurpose.Batch);
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
        ClientResult batchResult = IsAsync
            ? await client.CreateBatchAsync(content)
            : client.CreateBatch(content);

        BinaryData response = batchResult.GetRawResponse().Content;
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

        batchResult = IsAsync
            ? await client.GetBatchAsync(id, options: null)
            : client.GetBatch(id, options: null);

        JsonElement endpointElement = jsonDocument.RootElement.GetProperty("endpoint");
        string endpoint = endpointElement.GetString();

        Assert.That(endpoint, Is.EqualTo("/v1/chat/completions"));

        batchResult = IsAsync
            ? await client.CancelBatchAsync(id, options: null)
            : client.CancelBatch(id, options: null);

        statusElement = jsonDocument.RootElement.GetProperty("status");
        status = statusElement.GetString();

        Assert.That(status, Is.EqualTo("validating"));

        //var newBatchDynamic = batchResult.GetRawResponse().Content.ToDynamicFromJson();

        //Assert.That(newBatchDynamic?.createdAt, Is.GreaterThan(new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)));
        //Assert.That(newBatchDynamic.status, Is.EqualTo("validating"));
        //Assert.That(newBatchDynamic.metadata["testMetadataKey"], Is.EqualTo("test metadata value"));
        //batchResult = await client.GetBatchAsync(newBatchDynamic.id, options: null);
        //newBatchDynamic = batchResult.GetRawResponse().Content.ToObjectFromJson<dynamic>();
        //Assert.That(newBatchDynamic.endpoint, Is.EqualTo("/v1/chat/completions"));
        //batchResult = await client.CancelBatchAsync(newBatchDynamic.id, options: null);
        //newBatchDynamic = batchResult.GetRawResponse().Content.ToObjectFromJson<dynamic>();
        //Assert.That(newBatchDynamic.status, Is.EqualTo("cancelling"));
    }

    private static BatchClient GetTestClient() => GetTestClient<BatchClient>(TestScenario.Batch);
}