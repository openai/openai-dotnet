using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Files;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Files;

[Parallelizable(ParallelScope.All)]
[Category("Files")]
[Category("Smoke")]
public class FilesMockTests : ClientTestBase
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
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));

        OpenAIFile fileInfo = await client.GetFileAsync("file_id");
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
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));

        OpenAIFile fileInfo = await client.GetFileAsync("file_id");
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
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));
        OpenAIFile fileInfo = await client.GetFileAsync("file_id");

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
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));
        OpenAIFile fileInfo = await client.GetFileAsync("file_id");
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
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));
        OpenAIFile fileInfo = await client.GetFileAsync("file_id");
        Assert.That(fileInfo.StatusDetails, Is.EqualTo("This is definitely an error."));
    }
#pragma warning restore CS0618

    [Test]
    public void GetFileRespectsTheCancellationToken()
    {
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GetFileAsync("fileId", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
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
    public async Task UploadFileWithPathUsesFileNameOnly()
    {
        string requestBody = null;
        MockPipelineResponse response = new MockPipelineResponse(200).WithContent("""
        {
            "id": "returned_file_id"
        }
        """);

        OpenAIClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(message =>
            {
                using MemoryStream stream = new();
                message.Request.Content.WriteTo(stream);
                requestBody = BinaryData.FromBytes(stream.ToArray()).ToString();
                return response;
            })
            {
                ExpectSyncPipeline = !IsAsync
            }
        };

        await InvokeUploadFileSyncOrAsync(clientOptions, FileSourceKind.UsingFilePath);

        string fileContentDisposition = requestBody
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Single(line => line.StartsWith("Content-Disposition: form-data; name=file", StringComparison.Ordinal));

        Assert.That(fileContentDisposition, Does.Contain("filename=files_travis_favorite_food.pdf"));
        Assert.That(fileContentDisposition, Does.Not.Contain("Assets"));
    }

    [Test]
    public void UploadFileRespectsTheCancellationToken()
    {
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential));
        using var stream = new MemoryStream(Array.Empty<byte>());
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.UploadFileAsync(stream, "filename.txt", FileUploadPurpose.Assistants, cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
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
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));

        OpenAIFileCollection fileInfoCollection = await client.GetFilesAsync(FilePurpose.Assistants);
        OpenAIFile fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.Id, Is.EqualTo("returned_file_id"));
    }

    [Test]
    [TestCaseSource(nameof(s_purposeSource))]
    public async Task GetFilesPaginatesWithCollectionOptions((string stringValue, FilePurpose expectedValue) purpose)
    {
        string[] responseContents =
        [
            """
            {
                "object": "list",
                "data": [{ "id": "file_1" }],
                "first_id": "file_1",
                "last_id": "file_1",
                "has_more": true
            }
            """,
            """
            {
                "object": "list",
                "data": [{ "id": "file_2" }],
                "first_id": "file_2",
                "last_id": "file_2",
                "has_more": false
            }
            """
        ];
        List<Uri> requestUris = [];
        int responseIndex = 0;
        OpenAIClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(message =>
            {
                requestUris.Add(message.Request.Uri);
                return new MockPipelineResponse(200).WithContent(responseContents[responseIndex++]);
            })
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
        OpenAIFileClient client = new(s_fakeCredential, clientOptions);
        FileCollectionOptions options = new()
        {
            Purpose = purpose.expectedValue,
            PageSizeLimit = 1,
            Order = FileCollectionOrder.Ascending,
            AfterId = "file_start"
        };
        List<OpenAIFile> files = [];

        if (IsAsync)
        {
            await foreach (OpenAIFile file in client.GetFilesAsync(options))
            {
                files.Add(file);
            }
        }
        else
        {
            files.AddRange(client.GetFiles(options));
        }

        Assert.That(files.Select(file => file.Id), Is.EqualTo(new[] { "file_1", "file_2" }));
        Assert.That(requestUris, Has.Count.EqualTo(2));
        Assert.That(requestUris[0].Query, Does.Contain($"purpose={purpose.stringValue}"));
        Assert.That(requestUris[0].Query, Does.Contain("limit=1"));
        Assert.That(requestUris[0].Query, Does.Contain("order=asc"));
        Assert.That(requestUris[0].Query, Does.Contain("after=file_start"));
        Assert.That(requestUris[1].Query, Does.Contain("after=file_1"));
    }

    [Test]
    public async Task GetFilesReturnsNoContinuationTokenForFinalPage()
    {
        string[] responseContents =
        [
            """
            {
                "object": "list",
                "data": [{ "id": "file_1" }],
                "first_id": "file_1",
                "last_id": "file_1",
                "has_more": true
            }
            """,
            """
            {
                "object": "list",
                "data": [{ "id": "file_2" }],
                "first_id": "file_2",
                "last_id": "file_2",
                "has_more": false
            }
            """
        ];
        int responseIndex = 0;
        OpenAIClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(_ =>
                new MockPipelineResponse(200).WithContent(responseContents[responseIndex++]))
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
        OpenAIFileClient client = new(s_fakeCredential, clientOptions);
        FileCollectionOptions options = new()
        {
            Purpose = FilePurpose.Assistants,
            PageSizeLimit = 1
        };

        if (IsAsync)
        {
            AsyncCollectionResult<OpenAIFile> result = client.GetFilesAsync(options);
            List<ClientResult> pages = [];
            await foreach (ClientResult page in result.GetRawPagesAsync())
            {
                pages.Add(page);
            }

            Assert.That(pages, Has.Count.EqualTo(2));
            Assert.That(result.GetContinuationToken(pages[0]), Is.Not.Null);
            Assert.That(result.GetContinuationToken(pages[1]), Is.Null);
        }
        else
        {
            CollectionResult<OpenAIFile> result = client.GetFiles(options);
            List<ClientResult> pages = [.. result.GetRawPages()];

            Assert.That(pages, Has.Count.EqualTo(2));
            Assert.That(result.GetContinuationToken(pages[0]), Is.Not.Null);
            Assert.That(result.GetContinuationToken(pages[1]), Is.Null);
        }
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
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));

        OpenAIFileCollection fileInfoCollection = await client.GetFilesAsync(FilePurpose.Assistants);
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
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));

        OpenAIFileCollection fileInfoCollection = await client.GetFilesAsync(FilePurpose.Assistants);
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
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));

        OpenAIFileCollection fileInfoCollection = await client.GetFilesAsync(FilePurpose.Assistants);
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
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));

        OpenAIFileCollection fileInfoCollection = await client.GetFilesAsync(FilePurpose.Assistants);
        OpenAIFile fileInfo = fileInfoCollection.Single();

        Assert.That(fileInfo.StatusDetails, Is.EqualTo("This is definitely an error."));
    }
#pragma warning restore CS0618

    [Test]
    public void GetFilesRespectsTheCancellationToken()
    {
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GetFilesAsync(FilePurpose.Assistants, cancellationSource.Token),
            Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public void DownloadFileRespectsTheCancellationToken()
    {
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.DownloadFileAsync("fileId", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public void DeleteFileRespectsTheCancellationToken()
    {
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.DeleteFileAsync("fileId", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status).WithContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
    }

    private async ValueTask<OpenAIFile> InvokeUploadFileSyncOrAsync(OpenAIClientOptions clientOptions, FileSourceKind fileSourceKind)
    {
        OpenAIFileClient client = CreateProxyFromClient(new OpenAIFileClient(s_fakeCredential, clientOptions));
        string filename = "test-file.txt";

        if (fileSourceKind == FileSourceKind.UsingStream)
        {
            using Stream file = new MemoryStream([0x01, 0x02, 0x03, 0x04, 0x05]);

            return await client.UploadFileAsync(file, filename, purpose: FileUploadPurpose.Assistants);
        }
        else if (fileSourceKind == FileSourceKind.UsingFilePath)
        {
            string path = Path.Combine("Assets", "files_travis_favorite_food.pdf");

            return await client.UploadFileAsync(path, purpose: FileUploadPurpose.Assistants);
        }
        else if (fileSourceKind == FileSourceKind.UsingBinaryData)
        {
            BinaryData content = BinaryData.FromBytes([0x01, 0x02, 0x03, 0x04, 0x05]);

            return await client.UploadFileAsync(content, filename, purpose: FileUploadPurpose.Assistants);
        }

        Assert.Fail("Invalid source kind.");
        return null;
    }
}
