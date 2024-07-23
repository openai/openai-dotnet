using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Images;

namespace OpenAI.Tests.Images;

[Parallelizable(ParallelScope.All)]
[Category("Images")]
public partial class OpenAIImageModelFactoryTests
{
    [Test]
    public void GeneratedImageWithNoPropertiesWorks()
    {
        GeneratedImage generatedImage = OpenAIImageModelFactory.GeneratedImage();

        Assert.That(generatedImage.ImageBytes, Is.Null);
        Assert.That(generatedImage.ImageUri, Is.Null);
        Assert.That(generatedImage.RevisedPrompt, Is.Null);
    }

    [Test]
    public void GeneratedImageWithImageBytesWorks()
    {
        BinaryData imageBytes = BinaryData.FromString("Definitely an image.");
        GeneratedImage generatedImage = OpenAIImageModelFactory.GeneratedImage(imageBytes: imageBytes);

        Assert.That(generatedImage.ImageBytes, Is.EqualTo(imageBytes));
        Assert.That(generatedImage.ImageUri, Is.Null);
        Assert.That(generatedImage.RevisedPrompt, Is.Null);
    }

    [Test]
    public void GeneratedImageWithImageUriWorks()
    {
        Uri imageUri = new Uri("https://definitely-a-website.com/");
        GeneratedImage generatedImage = OpenAIImageModelFactory.GeneratedImage(imageUri: imageUri);

        Assert.That(generatedImage.ImageBytes, Is.Null);
        Assert.That(generatedImage.ImageUri, Is.EqualTo(imageUri));
        Assert.That(generatedImage.RevisedPrompt, Is.Null);
    }

    [Test]
    public void GeneratedImageWithRevisedPromptWorks()
    {
        string revisedPrompt = "I've been revised.";
        GeneratedImage generatedImage = OpenAIImageModelFactory.GeneratedImage(revisedPrompt: revisedPrompt);

        Assert.That(generatedImage.ImageBytes, Is.Null);
        Assert.That(generatedImage.ImageUri, Is.Null);
        Assert.That(generatedImage.RevisedPrompt, Is.EqualTo(revisedPrompt));
    }

    [Test]
    public void GeneratedImageCollectionWithNoPropertiesWorks()
    {
        GeneratedImageCollection generatedImageCollection = OpenAIImageModelFactory.GeneratedImageCollection();

        Assert.That(generatedImageCollection.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(generatedImageCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void GeneratedImageCollectionWithCreatedAtWorks()
    {
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        GeneratedImageCollection generatedImageCollection = OpenAIImageModelFactory.GeneratedImageCollection(createdAt: createdAt);

        Assert.That(generatedImageCollection.CreatedAt, Is.EqualTo(createdAt));
        Assert.That(generatedImageCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void GeneratedImageCollectionWithItemsWorks()
    {
        IEnumerable<GeneratedImage> items = [
            OpenAIImageModelFactory.GeneratedImage(revisedPrompt: "This is the first prompt."),
            OpenAIImageModelFactory.GeneratedImage(revisedPrompt: "This is not the first prompt.")
        ];
        GeneratedImageCollection generatedImageCollection = OpenAIImageModelFactory.GeneratedImageCollection(items: items);

        Assert.That(generatedImageCollection.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(generatedImageCollection.SequenceEqual(items), Is.True);
    }
}
