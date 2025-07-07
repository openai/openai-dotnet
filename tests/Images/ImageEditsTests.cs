using NUnit.Framework;
using OpenAI.Images;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.IO;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Images;

public partial class ImageEditsTests : ImageTestFixtureBase
{
    public ImageEditsTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImage image = null;

        ImageEditOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Uri,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream mask = File.OpenRead(maskImagePath);

            image = IsAsync
                ? await client.GenerateImageEditAsync(mask, maskFilename, CatPrompt, options)
                : client.GenerateImageEdit(mask, maskFilename, CatPrompt, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = IsAsync
                ? await client.GenerateImageEditAsync(maskImagePath, CatPrompt, options)
                : client.GenerateImageEdit(maskImagePath, CatPrompt, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);

        ValidateGeneratedImage(image.ImageUri, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImage image = null;

        ImageEditOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream mask = File.OpenRead(maskImagePath);

            image = IsAsync
                ? await client.GenerateImageEditAsync(mask, maskFilename, CatPrompt, options)
                : client.GenerateImageEdit(mask, maskFilename, CatPrompt, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = IsAsync
                ? await client.GenerateImageEditAsync(maskImagePath, CatPrompt, options)
                : client.GenerateImageEdit(maskImagePath, CatPrompt, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Null);
        Assert.That(image.ImageBytes, Is.Not.Null);

        ValidateGeneratedImage(image.ImageBytes, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
    }

    [Test]
    public void GenerateImageEditFromStreamCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        using FileStream mask = File.OpenRead(maskImagePath);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditAsync(mask, maskFilename, CatPrompt));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageEdit(mask, maskFilename, CatPrompt));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    public void GenerateImageEditFromPathCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditAsync(maskImagePath, CatPrompt));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageEdit(maskImagePath, CatPrompt));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditWithMaskFileWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImage image = null;

        ImageEditOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Uri,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream originalImage = File.OpenRead(originalImagePath);
            using FileStream mask = File.OpenRead(maskImagePath);

            image = IsAsync
                ? await client.GenerateImageEditAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, options)
                : client.GenerateImageEdit(mask, maskFilename, CatPrompt, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = IsAsync
                ? await client.GenerateImageEditAsync(originalImagePath, CatPrompt, maskImagePath, options)
                : client.GenerateImageEdit(maskImagePath, CatPrompt, maskImagePath, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);

        ValidateGeneratedImage(image.ImageUri, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditWithMaskFileWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImage image = null;

        ImageEditOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream originalImage = File.OpenRead(originalImagePath);
            using FileStream mask = File.OpenRead(maskImagePath);

            image = IsAsync
                ? await client.GenerateImageEditAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, options)
                : client.GenerateImageEdit(mask, maskFilename, CatPrompt, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = IsAsync
                ? await client.GenerateImageEditAsync(originalImagePath, CatPrompt, maskImagePath, options)
                : client.GenerateImageEdit(maskImagePath, CatPrompt, maskImagePath, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Null);
        Assert.That(image.ImageBytes, Is.Not.Null);

        ValidateGeneratedImage(image.ImageBytes, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
    }

    [Test]
    public void GenerateImageEditWithMaskFileFromStreamCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        using FileStream originalImage = File.OpenRead(originalImagePath);
        using FileStream mask = File.OpenRead(maskImagePath);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageEdit(originalImage, originalImageFilename, CatPrompt, mask, maskFilename));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    public void GenerateImageEditWithMaskFileFromPathCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditAsync(originalImagePath, CatPrompt, maskImagePath));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageEdit(originalImagePath, CatPrompt, maskImagePath));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageEditsWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImageCollection images = null;

        ImageEditOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Uri,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream mask = File.OpenRead(maskImagePath);

            images = IsAsync
                ? await client.GenerateImageEditsAsync(mask, maskFilename, CatPrompt, 2, options)
                : client.GenerateImageEdits(mask, maskFilename, CatPrompt, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageEditsAsync(maskImagePath, CatPrompt, 2, options)
                : client.GenerateImageEdits(maskImagePath, CatPrompt, 2, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2024));
        Assert.That(images.Count, Is.EqualTo(2));

        foreach (GeneratedImage image in images)
        {
            Assert.That(image.ImageUri, Is.Not.Null);
            Assert.That(image.ImageBytes, Is.Null);
            Console.WriteLine(image.ImageUri.AbsoluteUri);
            ValidateGeneratedImage(image.ImageUri, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
        }
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageEditsWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImageCollection images = null;

        ImageEditOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream mask = File.OpenRead(maskImagePath);

            images = IsAsync
                ? await client.GenerateImageEditsAsync(mask, maskFilename, CatPrompt, 2, options)
                : client.GenerateImageEdits(mask, maskFilename, CatPrompt, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageEditsAsync(maskImagePath, CatPrompt, 2, options)
                : client.GenerateImageEdits(maskImagePath, CatPrompt, 2, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2024));
        Assert.That(images.Count, Is.EqualTo(2));

        foreach (GeneratedImage image in images)
        {
            Assert.That(image.ImageUri, Is.Null);
            Assert.That(image.ImageBytes, Is.Not.Null);
            ValidateGeneratedImage(image.ImageBytes, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
        }
    }

    [Test]
    public void GenerateMultipleImageEditsFromStreamCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        using FileStream mask = File.OpenRead(maskImagePath);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditsAsync(mask, maskFilename, CatPrompt, 2));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageEdits(mask, maskFilename, CatPrompt, 2));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    public void GenerateMultipleImageEditsFromPathCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditsAsync(maskImagePath, CatPrompt, 2));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageEdits(maskImagePath, CatPrompt, 2));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageEditsWithMaskFileWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImageCollection images = null;

        ImageEditOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Uri,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream originalImage = File.OpenRead(originalImagePath);
            using FileStream mask = File.OpenRead(maskImagePath);

            images = IsAsync
                ? await client.GenerateImageEditsAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, 2, options)
                : client.GenerateImageEdits(mask, maskFilename, CatPrompt, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageEditsAsync(originalImagePath, CatPrompt, maskImagePath, 2, options)
                : client.GenerateImageEdits(maskImagePath, CatPrompt, maskImagePath, 2, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2024));
        Assert.That(images.Count, Is.EqualTo(2));

        foreach (GeneratedImage image in images)
        {
            Assert.That(image.ImageUri, Is.Not.Null);
            Assert.That(image.ImageBytes, Is.Null);
            Console.WriteLine(image.ImageUri.AbsoluteUri);
            ValidateGeneratedImage(image.ImageUri, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
        }
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageEditsWithMaskFileWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImageCollection images = null;

        ImageEditOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream originalImage = File.OpenRead(originalImagePath);
            using FileStream mask = File.OpenRead(maskImagePath);

            images = IsAsync
                ? await client.GenerateImageEditsAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, 2, options)
                : client.GenerateImageEdits(mask, maskFilename, CatPrompt, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageEditsAsync(originalImagePath, CatPrompt, maskImagePath, 2, options)
                : client.GenerateImageEdits(maskImagePath, CatPrompt, maskImagePath, 2, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2024));
        Assert.That(images.Count, Is.EqualTo(2));

        foreach (GeneratedImage image in images)
        {
            Assert.That(image.ImageUri, Is.Null);
            Assert.That(image.ImageBytes, Is.Not.Null);
            ValidateGeneratedImage(image.ImageBytes, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
        }
    }

    [Test]
    public void GenerateMultipleImageEditsWithMaskFileFromStreamCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        using FileStream originalImage = File.OpenRead(originalImagePath);
        using FileStream mask = File.OpenRead(maskImagePath);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditsAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, 2));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageEdits(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, 2));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    public void GenerateMultipleImageEditsWithMaskFileFromPathCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditsAsync(originalImagePath, CatPrompt, maskImagePath, 2));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageEdits(originalImagePath, CatPrompt, maskImagePath, 2));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }
}
