using NUnit.Framework;
using OpenAI.Files;
using OpenAI.Tests.Utility;
using System;
using System.IO;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Files;

[TestFixture(true)]
[TestFixture(false)]
public partial class FileTests : SyncAsyncTestBase
{
    public FileTests(bool isAsync) 
        : base(isAsync)
    {
    }

    [Test]
    public async Task ListFiles()
    {
        FileClient client = GetTestClient();

        OpenAIFileInfoCollection allFiles = IsAsync
            ? await client.GetFilesAsync()
            : client.GetFiles();
        Assert.That(allFiles.Count, Is.GreaterThan(0));
        Console.WriteLine($"Total files count: {allFiles.Count}");

        OpenAIFileInfoCollection assistantsFiles = IsAsync
            ? await client.GetFilesAsync(OpenAIFilePurpose.Assistants)
            : client.GetFiles(OpenAIFilePurpose.Assistants);
        Assert.That(assistantsFiles.Count, Is.GreaterThan(0).And.LessThan(allFiles.Count));
        Console.WriteLine($"Assistant files count: {assistantsFiles.Count}");
    }

    [Test]
    public async Task UploadAndDelete()
    {
        FileClient client = GetTestClient();
        using Stream file = BinaryData.FromString("Hello! This is a test text file. Please delete me.").ToStream();
        string filename = "test-file-delete-me.txt";

        OpenAIFileInfo uploadedFile = IsAsync
            ? await client.UploadFileAsync(file, filename, FileUploadPurpose.Assistants)
            : client.UploadFile(file, filename, FileUploadPurpose.Assistants);
        Assert.That(uploadedFile, Is.Not.Null);
        Assert.That(uploadedFile.Filename, Is.EqualTo(filename));
        Assert.That(uploadedFile.Purpose, Is.EqualTo(OpenAIFilePurpose.Assistants));

        OpenAIFileInfo fileInfo = IsAsync
            ? await client.GetFileAsync(uploadedFile.Id)
            : client.GetFile(uploadedFile.Id);
        Assert.That(fileInfo.Id, Is.EqualTo(uploadedFile.Id));
        Assert.That(fileInfo.Filename, Is.EqualTo(uploadedFile.Filename));

        bool deleted = IsAsync
            ? await client.DeleteFileAsync(uploadedFile.Id)
            : client.DeleteFile(uploadedFile.Id);
        Assert.That(deleted, Is.True);
    }

    [Test]
    public async Task DownloadContent()
    {
        FileClient client = GetTestClient();

        OpenAIFileInfo fileInfo = IsAsync
            ? await client.GetFileAsync("file-S7roYWamZqfMK9D979HU4q6m")
            : client.GetFile("file-S7roYWamZqfMK9D979HU4q6m");
        Assert.That(fileInfo, Is.Not.Null);

        BinaryData downloadedContent = IsAsync
            ? await client.DownloadFileAsync("file-S7roYWamZqfMK9D979HU4q6m")
            : client.DownloadFile("file-S7roYWamZqfMK9D979HU4q6m");
        Assert.That(downloadedContent, Is.Not.Null);
    }

    [Test]
    public void SerializeFileCollection()
    {
        // TODO: Add this test.
    }

    private static FileClient GetTestClient() => GetTestClient<FileClient>(TestScenario.Files);
}