using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Images;
using System;
using System.ClientModel;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests.Images;

public partial class ImageEditsTests : ImageTestFixtureBase
{
    public ImageEditsTests(bool isAsync) : base(isAsync)
    {
        TestTimeoutInSeconds = 120;
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>("dall-e-2");

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

            image = await client.GenerateImageEditAsync(mask, maskFilename, CatPrompt, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = await client.GenerateImageEditAsync(maskImagePath, CatPrompt, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);

        await ValidateGeneratedImage(image.ImageUri, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GptImage1Works(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>("gpt-image-1");

        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImage image = null;

        ImageEditOptions options = new()
        {
            Background = GeneratedImageBackground.Opaque,
            Quality = GeneratedImageQuality.LowQuality,
            Size = GeneratedImageSize.W1024xH1024
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream mask = File.OpenRead(maskImagePath);

            image = await client.GenerateImageEditAsync(mask, maskFilename, CatPrompt, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = await client.GenerateImageEditAsync(maskImagePath, CatPrompt, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Null);
        Assert.That(image.ImageBytes, Is.Not.Null);

        await ValidateGeneratedImage(image.ImageBytes, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>("dall-e-2");

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

            image = await client.GenerateImageEditAsync(mask, maskFilename, CatPrompt, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = await client.GenerateImageEditAsync(maskImagePath, CatPrompt, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Null);
        Assert.That(image.ImageBytes, Is.Not.Null);

        await ValidateGeneratedImage(image.ImageBytes, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
    }

    [RecordedTest]
    public void GenerateImageEditFromStreamCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        using FileStream mask = File.OpenRead(maskImagePath);

        ClientResultException ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditAsync(mask, maskFilename, CatPrompt));

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [RecordedTest]
    public void GenerateImageEditFromPathCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);

        ClientResultException ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditAsync(maskImagePath, CatPrompt));
        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditWithMaskFileWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>("dall-e-2");

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

            image = await client.GenerateImageEditAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = await client.GenerateImageEditAsync(originalImagePath, CatPrompt, maskImagePath, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);
        Console.WriteLine(image.ImageUri.AbsoluteUri);

        await ValidateGeneratedImage(image.ImageUri, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditWithMaskFileWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>("dall-e-2");

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

            image = await client.GenerateImageEditAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = await client.GenerateImageEditAsync(originalImagePath, CatPrompt, maskImagePath, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Null);
        Assert.That(image.ImageBytes, Is.Not.Null);

        await ValidateGeneratedImage(image.ImageBytes, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
    }

    [RecordedTest]
    public void GenerateImageEditWithMaskFileFromStreamCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        using FileStream originalImage = File.OpenRead(originalImagePath);
        using FileStream mask = File.OpenRead(maskImagePath);

        ClientResultException ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename));
        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [RecordedTest]
    public void GenerateImageEditWithMaskFileFromPathCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);

        ClientResultException ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditAsync(originalImagePath, CatPrompt, maskImagePath));
        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageEditsWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>("dall-e-2");

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

            images = await client.GenerateImageEditsAsync(mask, maskFilename, CatPrompt, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageEditsAsync(maskImagePath, CatPrompt, 2, options);
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
            await ValidateGeneratedImage(image.ImageUri, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
        }
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageEditsWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>("dall-e-2");

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

            images = await client.GenerateImageEditsAsync(mask, maskFilename, CatPrompt, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageEditsAsync(maskImagePath, CatPrompt, 2, options);
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
            await ValidateGeneratedImage(image.ImageBytes, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
        }
    }

    [RecordedTest]
    public void GenerateMultipleImageEditsFromStreamCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        using FileStream mask = File.OpenRead(maskImagePath);

        ClientResultException ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditsAsync(mask, maskFilename, CatPrompt, 2));
        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [RecordedTest]
    public void GenerateMultipleImageEditsFromPathCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);

        ClientResultException ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditsAsync(maskImagePath, CatPrompt, 2));
        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageEditsWithMaskFileWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>("dall-e-2");

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

            images = await client.GenerateImageEditsAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageEditsAsync(originalImagePath, CatPrompt, maskImagePath, 2, options);
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
            await ValidateGeneratedImage(image.ImageUri, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
        }
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageEditsWithMaskFileWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>("dall-e-2");

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

            images = await client.GenerateImageEditsAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageEditsAsync(originalImagePath, CatPrompt, maskImagePath, 2, options);
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
            await ValidateGeneratedImage(image.ImageBytes, ["cat", "owl", "animal"], "Note that it likely depicts some sort of animal.");
        }
    }

    [RecordedTest]
    public void GenerateMultipleImageEditsWithMaskFileFromStreamCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        using FileStream originalImage = File.OpenRead(originalImagePath);
        using FileStream mask = File.OpenRead(maskImagePath);

        ClientResultException ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditsAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, 2));
        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [RecordedTest]
    public void GenerateMultipleImageEditsWithMaskFileFromPathCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);

        ClientResultException ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageEditsAsync(originalImagePath, CatPrompt, maskImagePath, 2));
        Assert.That(ex.Status, Is.EqualTo(401));
    }
}
