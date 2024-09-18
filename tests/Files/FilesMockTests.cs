using System;
using System.ClientModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenAI.Files;
using OpenAI.Tests.Utility;

namespace OpenAI.Tests.Files;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public partial class FilesMockTests : SyncAsyncTestBase
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
        ("assistants", OpenAIFilePurpose.Assistants),
        ("assistants_output", OpenAIFilePurpose.AssistantsOutput),
        ("batch", OpenAIFilePurpose.Batch),
        ("batch_output", OpenAIFilePurpose.BatchOutput),
        ("fine-tune", OpenAIFilePurpose.FineTune),
        ("fine-tune-results", OpenAIFilePurpose.FineTuneResults),
        ("vision", OpenAIFilePurpose.Vision)
    };

#pragma warning disable CS0618
    private static object[] s_statusSource =
    {
        ("uploaded", OpenAIFileStatus.Uploaded),
        ("processed", OpenAIFileStatus.Processed),
        ("error", OpenAIFileStatus.Error)
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
        FileClient client = new FileClient(s_fakeCredential, clientOptions);

        OpenAIFileInfo fileInfo = IsAsync
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
        FileClient client = new FileClient(s_fakeCredential, clientOptions);

        OpenAIFileInfo fileInfo = IsAsync
            ? await client.GetFileAsync("file_id")
            : client.GetFile("file_id");

        Assert.That(fileInfo.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    [TestCaseSource(nameof(s_purposeSource))]
    public async Task GetFileDeserializesPurpose((string stringValue, OpenAIFilePurpose expectedValue) purpose)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "purpose": "{{purpose.stringValue}}"
        }
        """);
        FileClient client = new FileClient(s_fakeCredential, clientOptions);

        OpenAIFileInfo fileInfo = IsAsync
            ? await client.GetFileAsync("file_id")
            : client.GetFile("file_id");

        Assert.That(fileInfo.Purpose, Is.EqualTo(purpose.expectedValue));
    }


#pragma warning disable CS0618
    [Test]
    [TestCaseSource(nameof(s_statusSource))]
    public async Task GetFileDeserializesStatus((string stringValue, OpenAIFileStatus expectedValue) status)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "status": "{{status.stringValue}}"
        }
        """);
        FileClient client = new FileClient(s_fakeCredential, clientOptions);

        OpenAIFileInfo fileInfo = IsAsync
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
        FileClient client = new FileClient(s_fakeCredential, clientOptions);

        OpenAIFileInfo fileInfo = IsAsync
            ? await client.GetFileAsync("file_id")
            : client.GetFile("file_id");

        Assert.That(fileInfo.StatusDetails, Is.EqualTo("This is definitely an error."));
    }
#pragma warning restore CS0618

    [Test]
    public void GetFileRespectsTheCancellationToken()
    {
        FileClient client = new FileClient(s_fakeCredential);
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
        OpenAIFileInfo fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, fileSourceKind);

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
        OpenAIFileInfo fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, fileSourceKind);

        Assert.That(fileInfo.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    public async Task UploadFileDeserializesPurpose(
        [ValueSource(nameof(s_fileSourceKindSource))] FileSourceKind fileSourceKind,
        [ValueSource(nameof(s_purposeSource))] (string stringValue, OpenAIFilePurpose expectedValue) purpose)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "purpose": "{{purpose.stringValue}}"
        }
        """);
        OpenAIFileInfo fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, fileSourceKind);

        Assert.That(fileInfo.Purpose, Is.EqualTo(purpose.expectedValue));
    }

#pragma warning disable CS0618
    [Test]
    public async Task UploadFileDeserializesStatus(
        [ValueSource(nameof(s_fileSourceKindSource))] FileSourceKind fileSourceKind,
        [ValueSource(nameof(s_statusSource))] (string stringValue, OpenAIFileStatus expectedValue) status)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "status": "{{status.stringValue}}"
        }
        """);
        OpenAIFileInfo fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, fileSourceKind);

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
        OpenAIFileInfo fileInfo = await InvokeUploadFileSyncOrAsync(clientOptions, fileSourceKind);

        Assert.That(fileInfo.StatusDetails, Is.EqualTo("This is definitely an error."));
    }
#pragma warning restore CS0618

    [Test]
    public void UploadFileRespectsTheCancellationToken()
    {
        FileClient client = new FileClient(s_fakeCredential);
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
            "data": [
                {
                    "id": "returned_file_id"
                }
            ]
        }
        """);
        FileClient client = new FileClient(s_fakeCredential, clientOptions);

        OpenAIFileInfoCollection fileInfoCollection = IsAsync
            ? await client.GetFilesAsync(OpenAIFilePurpose.Assistants)
            : client.GetFiles(OpenAIFilePurpose.Assistants);
        OpenAIFileInfo fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.Id, Is.EqualTo("returned_file_id"));
    }

    [Test]
    public async Task GetFilesDeserializesCreatedAt()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "created_at": 1704096000
                }
            ]
        }
        """);
        FileClient client = new FileClient(s_fakeCredential, clientOptions);

        OpenAIFileInfoCollection fileInfoCollection = IsAsync
            ? await client.GetFilesAsync(OpenAIFilePurpose.Assistants)
            : client.GetFiles(OpenAIFilePurpose.Assistants);
        OpenAIFileInfo fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    [TestCaseSource(nameof(s_purposeSource))]
    public async Task GetFilesDeserializesPurpose((string stringValue, OpenAIFilePurpose expectedValue) purpose)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "data": [
                {
                    "purpose": "{{purpose.stringValue}}"
                }
            ]
        }
        """);
        FileClient client = new FileClient(s_fakeCredential, clientOptions);

        OpenAIFileInfoCollection fileInfoCollection = IsAsync
            ? await client.GetFilesAsync(OpenAIFilePurpose.Assistants)
            : client.GetFiles(OpenAIFilePurpose.Assistants);
        OpenAIFileInfo fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.Purpose, Is.EqualTo(purpose.expectedValue));
    }

#pragma warning disable CS0618
    [Test]
    [TestCaseSource(nameof(s_statusSource))]
    public async Task GetFilesDeserializesStatus((string stringValue, OpenAIFileStatus expectedValue) status)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "data": [
                {
                    "status": "{{status.stringValue}}"
                }
            ]
        }
        """);
        FileClient client = new FileClient(s_fakeCredential, clientOptions);

        OpenAIFileInfoCollection fileInfoCollection = IsAsync
            ? await client.GetFilesAsync(OpenAIFilePurpose.Assistants)
            : client.GetFiles(OpenAIFilePurpose.Assistants);
        OpenAIFileInfo fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.Status, Is.EqualTo(status.expectedValue));
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public async Task GetFilesDeserializesStatusDetails()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "status_details": "This is definitely an error."
                }
            ]
        }
        """);
        FileClient client = new FileClient(s_fakeCredential, clientOptions);

        OpenAIFileInfoCollection fileInfoCollection = IsAsync
            ? await client.GetFilesAsync(OpenAIFilePurpose.Assistants)
            : client.GetFiles(OpenAIFilePurpose.Assistants);
        OpenAIFileInfo fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.StatusDetails, Is.EqualTo("This is definitely an error."));
    }
#pragma warning restore CS0618

    [Test]
    public void GetFilesRespectsTheCancellationToken()
    {
        FileClient client = new FileClient(s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GetFilesAsync(OpenAIFilePurpose.Assistants, cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GetFiles(OpenAIFilePurpose.Assistants, cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    [Test]
    public void DownloadFileRespectsTheCancellationToken()
    {
        FileClient client = new FileClient(s_fakeCredential);
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
        FileClient client = new FileClient(s_fakeCredential);
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

    private async ValueTask<OpenAIFileInfo> InvokeUploadFileSyncOrAsync(OpenAIClientOptions clientOptions, FileSourceKind fileSourceKind)
    {
        FileClient client = new FileClient(s_fakeCredential, clientOptions);
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
