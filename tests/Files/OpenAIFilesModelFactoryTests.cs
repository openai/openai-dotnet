using NUnit.Framework;
using OpenAI.Files;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Tests.Files;

[Parallelizable(ParallelScope.All)]
[Category("Files")]
[Category("Smoke")]
public class OpenAIFilesModelFactoryTests
{
#pragma warning disable CS0618
    [Test]
    public void FileDeletionResultWithNoPropertiesWorks()
    {
        FileDeletionResult fileDeletionResult = OpenAIFilesModelFactory.FileDeletionResult();

        Assert.That(fileDeletionResult.FileId, Is.Null);
        Assert.That(fileDeletionResult.Deleted, Is.EqualTo(false));
    }

    [Test]
    public void FileDeletionResultWithFileIdWorks()
    {
        string fileId = "fileId";
        FileDeletionResult fileDeletionResult = OpenAIFilesModelFactory.FileDeletionResult(fileId: fileId);

        Assert.That(fileDeletionResult.FileId, Is.EqualTo(fileId));
        Assert.That(fileDeletionResult.Deleted, Is.EqualTo(false));
    }

    [Test]
    public void FileDeletionResultWithDeletedWorks()
    {
        bool deleted = true;
        FileDeletionResult fileDeletionResult = OpenAIFilesModelFactory.FileDeletionResult(deleted: deleted);

        Assert.That(fileDeletionResult.FileId, Is.Null);
        Assert.That(fileDeletionResult.Deleted, Is.EqualTo(deleted));
    }

    [Test]
    public void OpenAIFileInfoWithNoPropertiesWorks()
    {
        OpenAIFile openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo();

        Assert.That(openAIFileInfo.Id, Is.Null);
        Assert.That(openAIFileInfo.SizeInBytes, Is.Null);
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFileInfo.Filename, Is.Null);
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(default(FilePurpose)));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(default(FileStatus)));
        Assert.That(openAIFileInfo.StatusDetails, Is.Null);
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public void OpenAIFileInfoWithIdWorks()
    {
        string id = "fileId";
        OpenAIFile openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo(id: id);

        Assert.That(openAIFileInfo.Id, Is.EqualTo(id));
        Assert.That(openAIFileInfo.SizeInBytes, Is.Null);
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFileInfo.Filename, Is.Null);
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(default(FilePurpose)));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(default(FileStatus)));
        Assert.That(openAIFileInfo.StatusDetails, Is.Null);
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public void OpenAIFileInfoWithSizeInBytesWorks()
    {
        int sizeInBytes = 1025;
        OpenAIFile openAIFile = OpenAIFilesModelFactory.OpenAIFileInfo(sizeInBytesLong: sizeInBytes);

        Assert.That(openAIFile.Id, Is.Null);
        Assert.That(openAIFile.SizeInBytes, Is.EqualTo(sizeInBytes));
        Assert.That(openAIFile.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFile.Filename, Is.Null);
        Assert.That(openAIFile.Purpose, Is.EqualTo(default(FilePurpose)));
        Assert.That(openAIFile.Status, Is.EqualTo(default(FileStatus)));
        Assert.That(openAIFile.StatusDetails, Is.Null);
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public void OpenAIFileInfoWithCreatedAtWorks()
    {
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        OpenAIFile openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo(createdAt: createdAt);

        Assert.That(openAIFileInfo.Id, Is.Null);
        Assert.That(openAIFileInfo.SizeInBytes, Is.Null);
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(createdAt));
        Assert.That(openAIFileInfo.Filename, Is.Null);
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(default(FilePurpose)));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(default(FileStatus)));
        Assert.That(openAIFileInfo.StatusDetails, Is.Null);
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public void OpenAIFileInfoWithFilenameWorks()
    {
        string filename = "file.png";
        OpenAIFile openAIFile = OpenAIFilesModelFactory.OpenAIFileInfo(filename: filename);

        Assert.That(openAIFile.Id, Is.Null);
        Assert.That(openAIFile.SizeInBytes, Is.Null);
        Assert.That(openAIFile.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFile.Filename, Is.EqualTo(filename));
        Assert.That(openAIFile.Purpose, Is.EqualTo(default(FilePurpose)));
        Assert.That(openAIFile.Status, Is.EqualTo(default(FileStatus)));
        Assert.That(openAIFile.StatusDetails, Is.Null);
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public void OpenAIFileInfoWithPurposeWorks()
    {
        FilePurpose purpose = FilePurpose.Vision;
        OpenAIFile openAIFile = OpenAIFilesModelFactory.OpenAIFileInfo(purpose: purpose);

        Assert.That(openAIFile.Id, Is.Null);
        Assert.That(openAIFile.SizeInBytes, Is.Null);
        Assert.That(openAIFile.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFile.Filename, Is.Null);
        Assert.That(openAIFile.Purpose, Is.EqualTo(purpose));
        Assert.That(openAIFile.Status, Is.EqualTo(default(FileStatus)));
        Assert.That(openAIFile.StatusDetails, Is.Null);
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public void OpenAIFileInfoWithStatusWorks()
    {
        FileStatus status = FileStatus.Uploaded;
        OpenAIFile openAIFile = OpenAIFilesModelFactory.OpenAIFileInfo(status: status);

        Assert.That(openAIFile.Id, Is.Null);
        Assert.That(openAIFile.SizeInBytes, Is.Null);
        Assert.That(openAIFile.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFile.Filename, Is.Null);
        Assert.That(openAIFile.Purpose, Is.EqualTo(default(FilePurpose)));
        Assert.That(openAIFile.Status, Is.EqualTo(status));
        Assert.That(openAIFile.StatusDetails, Is.Null);
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public void OpenAIFileInfoWithStatusDetailsWorks()
    {
        string statusDetails = "There's something off about this file.";
        OpenAIFile openAIFile = OpenAIFilesModelFactory.OpenAIFileInfo(statusDetails: statusDetails);

        Assert.That(openAIFile.Id, Is.Null);
        Assert.That(openAIFile.SizeInBytes, Is.Null);
        Assert.That(openAIFile.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFile.Filename, Is.Null);
        Assert.That(openAIFile.Purpose, Is.EqualTo(default(FilePurpose)));
        Assert.That(openAIFile.Status, Is.EqualTo(default(FileStatus)));
        Assert.That(openAIFile.StatusDetails, Is.EqualTo(statusDetails));
    }
#pragma warning restore CS0618

    [Test]
    public void OpenAIFileInfoCollectionWithNoPropertiesWorks()
    {
        OpenAIFileCollection openAIFileInfoCollection = OpenAIFilesModelFactory.OpenAIFileCollection();

        Assert.That(openAIFileInfoCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void OpenAIFileInfoCollectionWithItemsWorks()
    {
        IEnumerable<OpenAIFile> items = [
            OpenAIFilesModelFactory.OpenAIFileInfo(id: "firstFile"),
            OpenAIFilesModelFactory.OpenAIFileInfo(id: "secondFile")
        ];
        OpenAIFileCollection openAIFileCollection = OpenAIFilesModelFactory.OpenAIFileCollection(items: items);

        Assert.That(openAIFileCollection.SequenceEqual(items), Is.True);
    }
}
