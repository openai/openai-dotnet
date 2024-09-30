using NUnit.Framework;
using OpenAI.Files;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Files;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.Fixtures)]
[Category("Files")]
public class FilesTests : SyncAsyncTestBase
{
    private static OpenAIFileClient GetTestClient() => GetTestClient<OpenAIFileClient>(TestScenario.Files);

    public FilesTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task ListFiles()
    {
        OpenAIFileClient client = GetTestClient();
        using Stream file1 = BinaryData.FromString("Hello! This is a test text file. Please delete me.").ToStream();
        using Stream file2 = BinaryData.FromString("Hello! This is another test text file. Please delete me.").ToStream();
        string filename = "test-file-delete-me.txt";
        string visionFilename = "images_dog_and_cat.png";
        string visionFilePath = Path.Combine("Assets", visionFilename);

        OpenAIFile uploadedFile1 = null;
        OpenAIFile uploadedFile2 = null;
        OpenAIFile uploadedVisionFile = null;
        OpenAIFileCollection fileInfoCollection;

        try
        {
            uploadedFile1 = await client.UploadFileAsync(file1, filename, FileUploadPurpose.Assistants);
            uploadedFile2 = await client.UploadFileAsync(file2, filename, FileUploadPurpose.Assistants);
            uploadedVisionFile = await client.UploadFileAsync(visionFilePath, FileUploadPurpose.Vision);

            fileInfoCollection = IsAsync
                ? await client.GetFilesAsync(FilePurpose.Assistants)
                : client.GetFiles(FilePurpose.Assistants);
        }
        finally
        {
            if (uploadedVisionFile != null)
            {
                await client.DeleteFileAsync(uploadedVisionFile.Id);
            }

            if (uploadedFile2 != null)
            {
                await client.DeleteFileAsync(uploadedFile2.Id);
            }

            if (uploadedFile1 != null)
            {
                await client.DeleteFileAsync(uploadedFile1.Id);
            }
        }

        OpenAIFile fileInfo1 = null;
        OpenAIFile fileInfo2 = null;
        OpenAIFile visionFileInfo = null;

        foreach (OpenAIFile item in fileInfoCollection)
        {
            if (item.Id == uploadedFile1.Id)
            {
                fileInfo1 = item;
            }
            else if (item.Id == uploadedFile2.Id)
            {
                fileInfo2 = item;
            }
            else if (item.Id == uploadedVisionFile.Id)
            {
                visionFileInfo = item;
            }
        }

        Assert.That(fileInfo1, Is.Not.Null);
        Assert.That(fileInfo1.SizeInBytes, Is.EqualTo(uploadedFile1.SizeInBytes));
        Assert.That(fileInfo1.CreatedAt, Is.EqualTo(uploadedFile1.CreatedAt));
        Assert.That(fileInfo1.Filename, Is.EqualTo(uploadedFile1.Filename));
        Assert.That(fileInfo1.Purpose, Is.EqualTo(uploadedFile1.Purpose));
#pragma warning disable CS0618
        Assert.That(fileInfo1.Status, Is.EqualTo(uploadedFile1.Status));
        Assert.That(fileInfo1.StatusDetails, Is.EqualTo(uploadedFile1.StatusDetails));
#pragma warning restore CS0618

        Assert.That(fileInfo2, Is.Not.Null);
        Assert.That(fileInfo2.SizeInBytes, Is.EqualTo(uploadedFile2.SizeInBytes));
        Assert.That(fileInfo2.CreatedAt, Is.EqualTo(uploadedFile2.CreatedAt));
        Assert.That(fileInfo2.Filename, Is.EqualTo(uploadedFile2.Filename));
        Assert.That(fileInfo2.Purpose, Is.EqualTo(uploadedFile2.Purpose));
#pragma warning disable CS0618
        Assert.That(fileInfo2.Status, Is.EqualTo(uploadedFile2.Status));
        Assert.That(fileInfo2.StatusDetails, Is.EqualTo(uploadedFile2.StatusDetails));
#pragma warning restore CS0618

        Assert.That(visionFileInfo, Is.Null);
    }

    public enum FileSourceKind
    {
        UsingStream,
        UsingFilePath,
        UsingBinaryData
    }

    private static Array s_fileSourceKindSource = Enum.GetValues(typeof(FileSourceKind));

    [Test]
    [TestCaseSource(nameof(s_fileSourceKindSource))]
    public async Task UploadFile(FileSourceKind fileSourceKind)
    {
        OpenAIFileClient client = GetTestClient();
        string filename = "images_dog_and_cat.png";
        string path = Path.Combine("Assets", filename);
        OpenAIFile fileInfo = null;

        try
        {
            if (fileSourceKind == FileSourceKind.UsingStream)
            {
                using Stream file = File.OpenRead(path);

                fileInfo = IsAsync
                    ? await client.UploadFileAsync(file, filename, FileUploadPurpose.Vision)
                    : client.UploadFile(file, filename, FileUploadPurpose.Vision);
            }
            else if (fileSourceKind == FileSourceKind.UsingFilePath)
            {
                fileInfo = IsAsync
                    ? await client.UploadFileAsync(path, FileUploadPurpose.Vision)
                    : client.UploadFile(path, FileUploadPurpose.Vision);
            }
            else if (fileSourceKind == FileSourceKind.UsingBinaryData)
            {
                using Stream file = File.OpenRead(path);
                BinaryData content = BinaryData.FromStream(file);

                fileInfo = IsAsync
                    ? await client.UploadFileAsync(content, filename, FileUploadPurpose.Vision)
                    : client.UploadFile(content, filename, FileUploadPurpose.Vision);
            }
            else
            {
                Assert.Fail("Invalid source kind.");
            }
        }
        finally
        {
            if (fileInfo != null)
            {
                await client.DeleteFileAsync(fileInfo.Id);
            }
        }

        long expectedSize = new FileInfo(path).Length;
        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();
        string expectedFilename = (fileSourceKind == FileSourceKind.UsingFilePath) ? path : filename;

        Assert.That(fileInfo, Is.Not.Null);
        Assert.That(fileInfo.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(fileInfo.SizeInBytes, Is.EqualTo(expectedSize));
        Assert.That(fileInfo.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2024));
        Assert.That(fileInfo.Filename, Is.EqualTo(expectedFilename));
        Assert.That(fileInfo.Purpose, Is.EqualTo(FilePurpose.Vision));
#pragma warning disable CS0618
        Assert.That(fileInfo.Status, Is.Not.EqualTo(default(FileStatus)));
#pragma warning restore CS0618
    }

    [Test]
    public void UploadFileCanParseServiceError()
    {
        OpenAIFileClient client = GetTestClient();
        string filename = "images_dog_and_cat.png";
        string path = Path.Combine("Assets", filename);
        FileUploadPurpose fakePurpose = new FileUploadPurpose("world_domination");
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.UploadFileAsync(path, fakePurpose));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.UploadFile(path, fakePurpose));
        }

        Assert.That(ex.Status, Is.EqualTo(400));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task DeleteFile(bool useFileInfoOverload)
    {
        OpenAIFileClient client = GetTestClient();
        string fileContent = "Hello! This is a test text file. Please delete me.";
        using Stream file = BinaryData.FromString(fileContent).ToStream();
        string filename = "test-file-delete-me.txt";

        OpenAIFile uploadedFile = await client.UploadFileAsync(file, filename, FileUploadPurpose.Assistants);
        FileDeletionResult result;

        if (useFileInfoOverload)
        {
            result = IsAsync
                ? await client.DeleteFileAsync(uploadedFile.Id)
                : client.DeleteFile(uploadedFile.Id);
        }
        else
        {
            result = IsAsync
                ? await client.DeleteFileAsync(uploadedFile.Id)
                : client.DeleteFile(uploadedFile.Id);
        }

        Assert.That(result.FileId, Is.EqualTo(uploadedFile.Id));
        Assert.That(result.Deleted, Is.True);
    }

    [Test]
    public void DeleteFileCanParseServiceError()
    {
        OpenAIFileClient client = GetTestClient();
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.DeleteFileAsync("fake_id"));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.DeleteFile("fake_id"));
        }

        Assert.That(ex.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task GetFile()
    {
        OpenAIFileClient client = GetTestClient();
        using Stream file = BinaryData.FromString("Hello! This is a test text file. Please delete me.").ToStream();
        string filename = "test-file-delete-me.txt";
        OpenAIFile uploadedFile = null;
        OpenAIFile fileInfo;

        try
        {
            uploadedFile = await client.UploadFileAsync(file, filename, FileUploadPurpose.Assistants);

            fileInfo = IsAsync
                ? await client.GetFileAsync(uploadedFile.Id)
                : client.GetFile(uploadedFile.Id);
        }
        finally
        {
            if (uploadedFile != null)
            {
                await client.DeleteFileAsync(uploadedFile.Id);
            }
        }

        Assert.That(fileInfo, Is.Not.Null);
        Assert.That(fileInfo.Id, Is.EqualTo(uploadedFile.Id));
        Assert.That(fileInfo.SizeInBytes, Is.EqualTo(uploadedFile.SizeInBytes));
        Assert.That(fileInfo.CreatedAt, Is.EqualTo(uploadedFile.CreatedAt));
        Assert.That(fileInfo.Filename, Is.EqualTo(uploadedFile.Filename));
        Assert.That(fileInfo.Purpose, Is.EqualTo(uploadedFile.Purpose));
#pragma warning disable CS0618
        Assert.That(fileInfo.Status, Is.EqualTo(uploadedFile.Status));
        Assert.That(fileInfo.StatusDetails, Is.EqualTo(uploadedFile.StatusDetails));
#pragma warning restore CS0618
    }

    [Test]
    public void GetFileCanParseServiceError()
    {
        OpenAIFileClient client = GetTestClient();
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GetFileAsync("fake_id"));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GetFile("fake_id"));
        }

        Assert.That(ex.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task DownloadContent()
    {
        OpenAIFileClient client = GetTestClient();
        string filename = "images_dog_and_cat.png";
        string path = Path.Combine("Assets", filename);
        using Stream file = File.OpenRead(path);
        OpenAIFile uploadedFile = null;
        BinaryData downloadedContent;

        try
        {
            uploadedFile = await client.UploadFileAsync(file, filename, FileUploadPurpose.Vision);

            downloadedContent = IsAsync
                ? await client.DownloadFileAsync(uploadedFile.Id)
                : client.DownloadFile(uploadedFile.Id);
        }
        finally
        {
            if (uploadedFile != null)
            {
                await client.DeleteFileAsync(uploadedFile.Id);
            }
        }

        byte[] originalFileBytes = File.ReadAllBytes(path);
        byte[] downloadedBytes = downloadedContent.ToArray();

        Assert.That(downloadedBytes.SequenceEqual(originalFileBytes));
    }

    [Test]
    public void DownloadFileCanParseServiceError()
    {
        OpenAIFileClient client = GetTestClient();
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.DownloadFileAsync("fake_id"));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.DownloadFile("fake_id"));
        }

        Assert.That(ex.Status, Is.EqualTo(404));
    }

    [Test]
    public void SerializeFileCollection()
    {
        // TODO: Add this test.
    }

    [Test]
    public async Task NonAsciiFilename()
    {
        OpenAIFileClient client = GetTestClient();
        string filename = "你好.txt";
        BinaryData fileContent = BinaryData.FromString("世界您好！这是个测试。");
        OpenAIFile uploadedFile = IsAsync
            ? await client.UploadFileAsync(fileContent, filename, FileUploadPurpose.Assistants)
            : client.UploadFile(fileContent, filename, FileUploadPurpose.Assistants);
        Assert.That(uploadedFile?.Filename, Is.EqualTo(filename));
    }
}