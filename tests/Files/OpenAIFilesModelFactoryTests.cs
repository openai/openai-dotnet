using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Files;

namespace OpenAI.Tests.Files;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public partial class OpenAIFilesModelFactoryTests
{
    [Test]
    public void OpenAIFileInfoWithNoPropertiesWorks()
    {
        OpenAIFileInfo openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo();

        Assert.That(openAIFileInfo.Id, Is.Null);
        Assert.That(openAIFileInfo.SizeInBytes, Is.Null);
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFileInfo.Filename, Is.Null);
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(default(OpenAIFilePurpose)));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(default(OpenAIFileStatus)));
        Assert.That(openAIFileInfo.StatusDetails, Is.Null);
    }

    [Test]
    public void OpenAIFileInfoWithIdWorks()
    {
        string id = "fileId";
        OpenAIFileInfo openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo(id: id);

        Assert.That(openAIFileInfo.Id, Is.EqualTo(id));
        Assert.That(openAIFileInfo.SizeInBytes, Is.Null);
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFileInfo.Filename, Is.Null);
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(default(OpenAIFilePurpose)));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(default(OpenAIFileStatus)));
        Assert.That(openAIFileInfo.StatusDetails, Is.Null);
    }

    [Test]
    public void OpenAIFileInfoWithSizeInBytesWorks()
    {
        int sizeInBytes = 1025;
        OpenAIFileInfo openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo(sizeInBytes: sizeInBytes);

        Assert.That(openAIFileInfo.Id, Is.Null);
        Assert.That(openAIFileInfo.SizeInBytes, Is.EqualTo(sizeInBytes));
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFileInfo.Filename, Is.Null);
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(default(OpenAIFilePurpose)));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(default(OpenAIFileStatus)));
        Assert.That(openAIFileInfo.StatusDetails, Is.Null);
    }

    [Test]
    public void OpenAIFileInfoWithCreatedAtWorks()
    {
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        OpenAIFileInfo openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo(createdAt: createdAt);

        Assert.That(openAIFileInfo.Id, Is.Null);
        Assert.That(openAIFileInfo.SizeInBytes, Is.Null);
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(createdAt));
        Assert.That(openAIFileInfo.Filename, Is.Null);
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(default(OpenAIFilePurpose)));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(default(OpenAIFileStatus)));
        Assert.That(openAIFileInfo.StatusDetails, Is.Null);
    }

    [Test]
    public void OpenAIFileInfoWithFilenameWorks()
    {
        string filename = "file.png";
        OpenAIFileInfo openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo(filename: filename);

        Assert.That(openAIFileInfo.Id, Is.Null);
        Assert.That(openAIFileInfo.SizeInBytes, Is.Null);
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFileInfo.Filename, Is.EqualTo(filename));
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(default(OpenAIFilePurpose)));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(default(OpenAIFileStatus)));
        Assert.That(openAIFileInfo.StatusDetails, Is.Null);
    }

    [Test]
    public void OpenAIFileInfoWithPurposeWorks()
    {
        OpenAIFilePurpose purpose = OpenAIFilePurpose.Vision;
        OpenAIFileInfo openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo(purpose: purpose);

        Assert.That(openAIFileInfo.Id, Is.Null);
        Assert.That(openAIFileInfo.SizeInBytes, Is.Null);
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFileInfo.Filename, Is.Null);
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(purpose));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(default(OpenAIFileStatus)));
        Assert.That(openAIFileInfo.StatusDetails, Is.Null);
    }

    [Test]
    public void OpenAIFileInfoWithStatusWorks()
    {
        OpenAIFileStatus status = OpenAIFileStatus.Uploaded;
        OpenAIFileInfo openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo(status: status);

        Assert.That(openAIFileInfo.Id, Is.Null);
        Assert.That(openAIFileInfo.SizeInBytes, Is.Null);
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFileInfo.Filename, Is.Null);
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(default(OpenAIFilePurpose)));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(status));
        Assert.That(openAIFileInfo.StatusDetails, Is.Null);
    }

    [Test]
    public void OpenAIFileInfoWithStatusDetailsWorks()
    {
        string statusDetails = "There's something off about this file.";
        OpenAIFileInfo openAIFileInfo = OpenAIFilesModelFactory.OpenAIFileInfo(statusDetails: statusDetails);

        Assert.That(openAIFileInfo.Id, Is.Null);
        Assert.That(openAIFileInfo.SizeInBytes, Is.Null);
        Assert.That(openAIFileInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIFileInfo.Filename, Is.Null);
        Assert.That(openAIFileInfo.Purpose, Is.EqualTo(default(OpenAIFilePurpose)));
        Assert.That(openAIFileInfo.Status, Is.EqualTo(default(OpenAIFileStatus)));
        Assert.That(openAIFileInfo.StatusDetails, Is.EqualTo(statusDetails));
    }

    [Test]
    public void OpenAIFileInfoCollectionWithNoPropertiesWorks()
    {
        OpenAIFileInfoCollection openAIFileInfoCollection = OpenAIFilesModelFactory.OpenAIFileInfoCollection();

        Assert.That(openAIFileInfoCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void OpenAIFileInfoCollectionWithItemsWorks()
    {
        IEnumerable<OpenAIFileInfo> items = [
            OpenAIFilesModelFactory.OpenAIFileInfo(id: "firstFile"),
            OpenAIFilesModelFactory.OpenAIFileInfo(id: "secondFile")
        ];
        OpenAIFileInfoCollection openAIFileInfoCollection = OpenAIFilesModelFactory.OpenAIFileInfoCollection(items: items);

        Assert.That(openAIFileInfoCollection.SequenceEqual(items), Is.True);
    }
}
