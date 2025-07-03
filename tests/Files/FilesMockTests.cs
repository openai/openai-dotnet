using NUnit.Framework;
using OpenAI.Files;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Files;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Files")]
[Category("Smoke")]
public class FilesMockTests : SyncAsyncTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public FilesMockTests(bool isAsync)
        : base(isAsync)
    {
    }

    public enum FileSourceKind
    {
        UsingStream,
        UsingFilePath,
        UsingBinaryData
    }

    private static Array s_fileSourceKindSource = Enum.GetValues(typeof(FileSourceKind));

    private static object[] s_purposeSource =
    {
        ("assistants", FilePurpose.Assistants),
        ("assistants_output", FilePurpose.AssistantsOutput),
        ("batch", FilePurpose.Batch),
        ("batch_output", FilePurpose.BatchOutput),
        ("fine-tune", FilePurpose.FineTune),
        ("fine-tune-results", FilePurpose.FineTuneResults),
        ("vision", FilePurpose.Vision)
    };

#pragma warning disable CS0618
    private static object[] s_statusSource =
    {
        ("uploaded", FileStatus.Uploaded),
        ("processed", FileStatus.Processed),
        ("error", FileStatus.Error)
    };
#pragma warning restore CS0618

    [Test]
    public async Task GetFileDeserializesId()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "id": "returned_file_id"
        }
        """);
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);

        OpenAIFile fileInfo = IsAsync
            ? await client.GetFileAsync("file_id")
            : client.GetFile("file_id");

        Assert.That(fileInfo.Id, Is.EqualTo("returned_file_id"));
    }

    [Test]
    public async Task GetFileDeserializesCreatedAt()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "created_at": 1704096000
        }
        """);
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);

        OpenAIFile fileInfo = IsAsync
            ? await client.GetFileAsync("file_id")
            : client.GetFile("file_id");

        Assert.That(fileInfo.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    [TestCaseSource(nameof(s_purposeSource))]
    public async Task GetFileDeserializesPurpose((string stringValue, FilePurpose expectedValue) purpose)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "purpose": "{{purpose.stringValue}}"
        }
        """);
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);

        OpenAIFile fileInfo = IsAsync
            ? await client.GetFileAsync("file_id")
            : client.GetFile("file_id");

        Assert.That(fileInfo.Purpose, Is.EqualTo(purpose.expectedValue));
    }


#pragma warning disable CS0618
    [Test]
    [TestCaseSource(nameof(s_statusSource))]
    public async Task GetFileDeserializesStatus((string stringValue, FileStatus expectedValue) status)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "status": "{{status.stringValue}}"
        }
        """);
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);

        OpenAIFile fileInfo = IsAsync
            ? await client.GetFileAsync("file_id")
            : client.GetFile("file_id");

        Assert.That(fileInfo.Status, Is.EqualTo(status.expectedValue));
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public async Task GetFileDeserializesStatusDetails()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "status_details": "This is definitely an error."
        }
        """);
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);

        OpenAIFile fileInfo = IsAsync
            ? await client.GetFileAsync("file_id")
            : client.GetFile("file_id");

        Assert.That(fileInfo.StatusDetails, Is.EqualTo("This is definitely an error."));
    }
#pragma warning restore CS0618

    [Test]
    public void GetFileRespectsTheCancellationToken()
    {
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GetFileAsync("fileId", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GetFile("fileId", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    [Test]
    [TestCaseSource(nameof(s_fileSourceKindSource))]
    public async Task UploadFileDeserializesId(FileSourceKind fileSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "id": "returned_file_id"
        }
        """);
        OpenAIFile fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, fileSourceKind);

        Assert.That(fileInfo.Id, Is.EqualTo("returned_file_id"));
    }

    [Test]
    [TestCaseSource(nameof(s_fileSourceKindSource))]
    public async Task UploadFileDeserializesCreatedAt(FileSourceKind fileSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "created_at": 1704096000
        }
        """);
        OpenAIFile fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, fileSourceKind);

        Assert.That(fileInfo.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    public async Task UploadFileDeserializesPurpose(
        [ValueSource(nameof(s_fileSourceKindSource))] FileSourceKind fileSourceKind,
        [ValueSource(nameof(s_purposeSource))] (string stringValue, FilePurpose expectedValue) purpose)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "purpose": "{{purpose.stringValue}}"
        }
        """);
        OpenAIFile fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, fileSourceKind);

        Assert.That(fileInfo.Purpose, Is.EqualTo(purpose.expectedValue));
    }

#pragma warning disable CS0618
    [Test]
    public async Task UploadFileDeserializesStatus(
        [ValueSource(nameof(s_fileSourceKindSource))] FileSourceKind fileSourceKind,
        [ValueSource(nameof(s_statusSource))] (string stringValue, FileStatus expectedValue) status)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "status": "{{status.stringValue}}"
        }
        """);
        OpenAIFile fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, fileSourceKind);

        Assert.That(fileInfo.Status, Is.EqualTo(status.expectedValue));
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    [TestCaseSource(nameof(s_fileSourceKindSource))]
    public async Task UploadFileDeserializesStatusDetails(FileSourceKind fileSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "status_details": "This is definitely an error."
        }
        """);
        OpenAIFile fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, fileSourceKind);

        Assert.That(fileInfo.StatusDetails, Is.EqualTo("This is definitely an error."));
    }
#pragma warning restore CS0618

    [Test]
    public async Task UploadFileDeserializesBigSizes()
    {
        long bigSize = (long)int.MaxValue + (long)int.MaxValue / 2;

        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "bytes": {{bigSize}}
        }
        """);
        OpenAIFile fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, FileSourceKind.UsingFilePath);

        Assert.That(fileInfo.SizeInBytesLong, Is.EqualTo(bigSize));
        Assert.Throws<OverflowException>(() => _ = fileInfo.SizeInBytes);
    }

    [Test]
    public void UploadFileRespectsTheCancellationToken()
    {
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential);
        using var stream = new MemoryStream(Array.Empty<byte>());
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.UploadFileAsync(stream, "filename.txt", FileUploadPurpose.Assistants, cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.UploadFile(stream, "filename.txt", FileUploadPurpose.Assistants, cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    [Test]
    public async Task GetFilesDeserializesId()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "object": "list",
            "data": [
                {
                    "id": "returned_file_id"
                }
            ]
        }
        """);
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);

        OpenAIFileCollection fileInfoCollection = IsAsync
            ? await client.GetFilesAsync(FilePurpose.Assistants)
            : client.GetFiles(FilePurpose.Assistants);
        OpenAIFile fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.Id, Is.EqualTo("returned_file_id"));
    }

    [Test]
    public async Task GetFilesDeserializesCreatedAt()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "object": "list",
            "data": [
                {
                    "created_at": 1704096000
                }
            ]
        }
        """);
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);

        OpenAIFileCollection fileInfoCollection = IsAsync
            ? await client.GetFilesAsync(FilePurpose.Assistants)
            : client.GetFiles(FilePurpose.Assistants);
        OpenAIFile fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    [TestCaseSource(nameof(s_purposeSource))]
    public async Task GetFilesDeserializesPurpose((string stringValue, FilePurpose expectedValue) purpose)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "object": "list",
            "data": [
                {
                    "purpose": "{{purpose.stringValue}}"
                }
            ]
        }
        """);
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);

        OpenAIFileCollection fileInfoCollection = IsAsync
            ? await client.GetFilesAsync(FilePurpose.Assistants)
            : client.GetFiles(FilePurpose.Assistants);
        OpenAIFile fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.Purpose, Is.EqualTo(purpose.expectedValue));
    }

#pragma warning disable CS0618
    [Test]
    [TestCaseSource(nameof(s_statusSource))]
    public async Task GetFilesDeserializesStatus((string stringValue, FileStatus expectedValue) status)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "object": "list",
            "data": [
                {
                    "status": "{{status.stringValue}}"
                }
            ]
        }
        """);
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);

        OpenAIFileCollection fileInfoCollection = IsAsync
            ? await client.GetFilesAsync(FilePurpose.Assistants)
            : client.GetFiles(FilePurpose.Assistants);
        OpenAIFile fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.Status, Is.EqualTo(status.expectedValue));
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public async Task GetFilesDeserializesStatusDetails()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "object": "list",
            "data": [
                {
                    "status_details": "This is definitely an error."
                }
            ]
        }
        """);
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);

        OpenAIFileCollection fileInfoCollection = IsAsync
            ? await client.GetFilesAsync(FilePurpose.Assistants)
            : client.GetFiles(FilePurpose.Assistants);
        OpenAIFile fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.StatusDetails, Is.EqualTo("This is definitely an error."));
    }
#pragma warning restore CS0618

    [Test]
    public void GetFilesRespectsTheCancellationToken()
    {
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GetFilesAsync(FilePurpose.Assistants, cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GetFiles(FilePurpose.Assistants, cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    [Test]
    public void DownloadFileRespectsTheCancellationToken()
    {
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.DownloadFileAsync("fileId", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.DownloadFile("fileId", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    [Test]
    public void DeleteFileRespectsTheCancellationToken()
    {
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.DeleteFileAsync("fileId", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.DeleteFile("fileId", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status);
        response.SetContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };
    }

    private async ValueTask<OpenAIFile> InvokeUploadFileSyncOrAsync(OpenAIClientOptions clientOptions, FileSourceKind fileSourceKind)
    {
        OpenAIFileClient client = new OpenAIFileClient(s_fakeCredential, clientOptions);
        string filename = "images_dog_and_cat.png";
        string path = Path.Combine("Assets", filename);

        if (fileSourceKind == FileSourceKind.UsingStream)
        {
            using FileStream file = File.OpenRead(path);

            return IsAsync
                ? await client.UploadFileAsync(file, filename, purpose: FileUploadPurpose.Assistants)
                : client.UploadFile(file, filename, purpose: FileUploadPurpose.Assistants);
        }
        else if (fileSourceKind == FileSourceKind.UsingFilePath)
        {
            return IsAsync
                ? await client.UploadFileAsync(path, purpose: FileUploadPurpose.Assistants)
                : client.UploadFile(path, purpose: FileUploadPurpose.Assistants);
        }
        else if (fileSourceKind == FileSourceKind.UsingBinaryData)
        {
            using FileStream file = File.OpenRead(path);
            BinaryData content = BinaryData.FromStream(file);

            return IsAsync
                ? await client.UploadFileAsync(content, filename, purpose: FileUploadPurpose.Assistants)
                : client.UploadFile(content, filename, purpose: FileUploadPurpose.Assistants);
        }

        Assert.Fail("Invalid source kind.");
        return null;
    }
}
