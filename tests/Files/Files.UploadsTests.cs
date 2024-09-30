using NUnit.Framework;
using OpenAI.Files;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Text.Json;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Files;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.Fixtures)]
[Category("Uploads")]
public class UploadsTests : SyncAsyncTestBase
{
    private static OpenAIFileClient GetTestClient() => GetTestClient<OpenAIFileClient>(TestScenario.Files);

    public UploadsTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreateUploadWorks(bool useTopLevelClient)
    {
        // Test with the top-level client as well to validate that both FileClient constructors
        // are setting the internal Uploads client correctly.

        OpenAIFileClient fileClient = useTopLevelClient
            ? TestHelpers.GetTestTopLevelClient().GetOpenAIFileClient()
            : GetTestClient();
        BinaryContent content = BinaryContent.Create(BinaryData.FromObjectAsJson(new
        {
            purpose = "fine-tune",
            filename = "uploads_test_file.jsonl",
            bytes = 8,
            mime_type = "text/jsonl"
        }));

        ClientResult result = IsAsync
            ? await fileClient.CreateUploadAsync(content)
            : fileClient.CreateUpload(content);

        BinaryData response = result.GetRawResponse().Content;
        using JsonDocument jsonDocument = JsonDocument.Parse(response);
        UploadDetails uploadDetails = GetUploadDetails(jsonDocument);

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(uploadDetails.Id, Does.StartWith("upload_"));
        Assert.That(uploadDetails.Object, Is.EqualTo("upload"));
        Assert.That(uploadDetails.Bytes, Is.EqualTo(8));
        Assert.That(uploadDetails.CreatedAt, Is.GreaterThan(unixTime2024));
        Assert.That(uploadDetails.Filename, Is.EqualTo("uploads_test_file.jsonl"));
        Assert.That(uploadDetails.Purpose, Is.EqualTo("fine-tune"));
        Assert.That(uploadDetails.Status, Is.EqualTo("pending"));
        Assert.That(uploadDetails.ExpiresAt, Is.GreaterThan(uploadDetails.CreatedAt));
    }

    [Test]
    public async Task AddUploadPartWorks()
    {
        OpenAIFileClient fileClient = GetTestClient();
        UploadDetails uploadDetails = await CreateTestUploadAsync(fileClient);
        using MultipartFormDataBinaryContent content = new();

        content.Add([1, 2, 3, 4], "data", "data", "application/octet-stream");

        ClientResult result = IsAsync
            ? await fileClient.AddUploadPartAsync(uploadDetails.Id, content, content.ContentType)
            : fileClient.AddUploadPart(uploadDetails.Id, content, content.ContentType);

        BinaryData response = result.GetRawResponse().Content;
        using JsonDocument jsonDocument = JsonDocument.Parse(response);
        UploadPartDetails uploadPartDetails = GetUploadPartDetails(jsonDocument);

        Assert.That(uploadPartDetails.Id, Does.StartWith("part_"));
        Assert.That(uploadPartDetails.Object, Is.EqualTo("upload.part"));
        Assert.That(uploadPartDetails.CreatedAt, Is.GreaterThanOrEqualTo(uploadDetails.CreatedAt));
        Assert.That(uploadPartDetails.UploadId, Is.EqualTo(uploadDetails.Id));
    }

    [Test]
    public async Task CompleteUploadWorks()
    {
        OpenAIFileClient fileClient = GetTestClient();
        UploadDetails createdUploadDetails = await CreateTestUploadAsync(fileClient);
        using MultipartFormDataBinaryContent firstPartContent = new();
        using MultipartFormDataBinaryContent secondPartContent = new();

        firstPartContent.Add([1, 2, 3, 4], "data", "data", "application/octet-stream");
        secondPartContent.Add([5, 6, 7, 8], "data", "data", "application/octet-stream");

        UploadPartDetails firstPartDetails = await AddTestUploadPartAsync(fileClient, createdUploadDetails.Id, firstPartContent);
        UploadPartDetails secondPartDetails = await AddTestUploadPartAsync(fileClient, createdUploadDetails.Id, secondPartContent);

        BinaryContent content = BinaryContent.Create(BinaryData.FromObjectAsJson(new
        {
            part_ids = new[] {
                firstPartDetails.Id,
                secondPartDetails.Id
            }
        }));

        ClientResult result = IsAsync
            ? await fileClient.CompleteUploadAsync(createdUploadDetails.Id, content)
            : fileClient.CompleteUpload(createdUploadDetails.Id, content);

        BinaryData response = result.GetRawResponse().Content;
        using JsonDocument jsonDocument = JsonDocument.Parse(response);
        JsonElement fileElement = jsonDocument.RootElement.GetProperty("file");
        string fileId = fileElement.GetProperty("id").GetString();

        await fileClient.DeleteFileAsync(fileId);

        UploadDetails completedUploadDetails = GetUploadDetails(jsonDocument);

        Assert.That(completedUploadDetails.Id, Is.EqualTo(createdUploadDetails.Id));
        Assert.That(completedUploadDetails.Object, Is.EqualTo(createdUploadDetails.Object));
        Assert.That(completedUploadDetails.Bytes, Is.EqualTo(createdUploadDetails.Bytes));
        Assert.That(completedUploadDetails.CreatedAt, Is.EqualTo(createdUploadDetails.CreatedAt));
        Assert.That(completedUploadDetails.Filename, Is.EqualTo(createdUploadDetails.Filename));
        Assert.That(completedUploadDetails.Purpose, Is.EqualTo(createdUploadDetails.Purpose));
        Assert.That(completedUploadDetails.Status, Is.EqualTo("completed"));
        Assert.That(completedUploadDetails.ExpiresAt, Is.EqualTo(createdUploadDetails.ExpiresAt));

        string fileObject = fileElement.GetProperty("object").GetString();
        int fileBytes = fileElement.GetProperty("bytes").GetInt32();
        long fileCreatedAt = fileElement.GetProperty("created_at").GetInt64();
        string filename = fileElement.GetProperty("filename").GetString();
        string filePurpose = fileElement.GetProperty("purpose").GetString();

        Assert.That(fileId, Does.StartWith("file-"));
        Assert.That(fileObject, Is.EqualTo("file"));
        Assert.That(fileBytes, Is.EqualTo(createdUploadDetails.Bytes));
        Assert.That(fileCreatedAt, Is.GreaterThanOrEqualTo(createdUploadDetails.CreatedAt));
        Assert.That(filename, Is.EqualTo(createdUploadDetails.Filename));
        Assert.That(filePurpose, Is.EqualTo(createdUploadDetails.Purpose));
    }

    [Test]
    public async Task CancelUploadWorks()
    {
        OpenAIFileClient fileClient = GetTestClient();
        UploadDetails createdUploadDetails = await CreateTestUploadAsync(fileClient);

        ClientResult result = IsAsync
            ? await fileClient.CancelUploadAsync(createdUploadDetails.Id)
            : fileClient.CancelUpload(createdUploadDetails.Id);

        BinaryData response = result.GetRawResponse().Content;
        using JsonDocument jsonDocument = JsonDocument.Parse(response);
        UploadDetails canceledUploadDetails = GetUploadDetails(jsonDocument);

        Assert.That(canceledUploadDetails.Id, Is.EqualTo(createdUploadDetails.Id));
        Assert.That(canceledUploadDetails.Object, Is.EqualTo(createdUploadDetails.Object));
        Assert.That(canceledUploadDetails.Bytes, Is.EqualTo(createdUploadDetails.Bytes));
        Assert.That(canceledUploadDetails.CreatedAt, Is.EqualTo(createdUploadDetails.CreatedAt));
        Assert.That(canceledUploadDetails.Filename, Is.EqualTo(createdUploadDetails.Filename));
        Assert.That(canceledUploadDetails.Purpose, Is.EqualTo(createdUploadDetails.Purpose));
        Assert.That(canceledUploadDetails.Status, Is.EqualTo("cancelled"));
        Assert.That(canceledUploadDetails.ExpiresAt, Is.EqualTo(createdUploadDetails.ExpiresAt));
    }

    private async Task<UploadDetails> CreateTestUploadAsync(OpenAIFileClient fileClient)
    {
        BinaryContent content = BinaryContent.Create(BinaryData.FromObjectAsJson(new
        {
            purpose = "fine-tune",
            filename = "uploads_test_file" + Guid.NewGuid() + ".jsonl",
            bytes = 8,
            mime_type = "text/jsonl"
        }));

        ClientResult result = await fileClient.CreateUploadAsync(content);
        BinaryData response = result.GetRawResponse().Content;
        using JsonDocument jsonDocument = JsonDocument.Parse(response);

        return GetUploadDetails(jsonDocument);
    }

    private async Task<UploadPartDetails> AddTestUploadPartAsync(OpenAIFileClient fileClient, string uploadId, MultipartFormDataBinaryContent content)
    {
        ClientResult result = await fileClient.AddUploadPartAsync(uploadId, content, content.ContentType);
        BinaryData response = result.GetRawResponse().Content;
        using JsonDocument jsonDocument = JsonDocument.Parse(response);

        return GetUploadPartDetails(jsonDocument);
    }

    private UploadDetails GetUploadDetails(JsonDocument jsonDocument)
    {
        string id = jsonDocument.RootElement.GetProperty("id").GetString();
        string @object = jsonDocument.RootElement.GetProperty("object").GetString();
        int bytes = jsonDocument.RootElement.GetProperty("bytes").GetInt32();
        long createdAt = jsonDocument.RootElement.GetProperty("created_at").GetInt64();
        string filename = jsonDocument.RootElement.GetProperty("filename").GetString();
        string purpose = jsonDocument.RootElement.GetProperty("purpose").GetString();
        string status = jsonDocument.RootElement.GetProperty("status").GetString();
        long expiresAt = jsonDocument.RootElement.GetProperty("expires_at").GetInt64();

        return new UploadDetails()
        {
            Id = id,
            Object = @object,
            Bytes = bytes,
            CreatedAt = createdAt,
            Filename = filename,
            Purpose = purpose,
            Status = status,
            ExpiresAt = expiresAt
        };
    }

    private UploadPartDetails GetUploadPartDetails(JsonDocument jsonDocument)
    {
        string id = jsonDocument.RootElement.GetProperty("id").GetString();
        string @object = jsonDocument.RootElement.GetProperty("object").GetString();
        long createdAt = jsonDocument.RootElement.GetProperty("created_at").GetInt64();
        string uploadId = jsonDocument.RootElement.GetProperty("upload_id").GetString();

        return new UploadPartDetails()
        {
            Id = id,
            Object = @object,
            CreatedAt = createdAt,
            UploadId = uploadId
        };
    }

    private readonly struct UploadDetails
    {
        public string Id { get; init; }
        public string Object { get; init; }
        public int Bytes { get; init; }
        public long CreatedAt { get; init; }
        public string Filename { get; init; }
        public string Purpose { get; init; }
        public string Status { get; init; }
        public long ExpiresAt { get; init; }
    }

    private readonly struct UploadPartDetails
    {
        public string Id { get; init; }
        public string Object { get; init; }
        public long CreatedAt { get; init; }
        public string UploadId { get; init; }
    }
}
