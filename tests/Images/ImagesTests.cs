using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Images;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Images;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Images")]
public class ImagesTests : SyncAsyncTestBase
{
    private const string CatPrompt = "A big cat with big round eyes and large cat ears, sitting in an empty room and looking at the camera.";

    public ImagesTests(bool isAsync) : base(isAsync)
    {
    }

    public enum ImageSourceKind
    {
        UsingStream,
        UsingFilePath
    }

    private static Array s_imageSourceKindSource = Enum.GetValues(typeof(ImageSourceKind));

    #region GenerateImages

    [Test]
    public async Task BasicGenerationWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images);

        string prompt = "An isolated stop sign.";

        GeneratedImage image = IsAsync
            ? await client.GenerateImageAsync(prompt)
            : client.GenerateImage(prompt);
        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);
        ValidateGeneratedImage(image.ImageUri, "stop");
    }

    [Test]
    public async Task GenerationWithOptionsWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images);

        string prompt = "An isolated stop sign.";

        ImageGenerationOptions options = new()
        {
            Quality = GeneratedImageQuality.Standard,
            Style = GeneratedImageStyle.Natural,
        };

        GeneratedImage image = IsAsync
            ? await client.GenerateImageAsync(prompt, options)
            : client.GenerateImage(prompt, options);
        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        ValidateGeneratedImage(image.ImageUri, "stop");
    }

    [Test]
    public async Task GenerationWithBytesResponseWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images);

        string prompt = "An isolated stop sign.";

        ImageGenerationOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        GeneratedImage image = IsAsync
            ? await client.GenerateImageAsync(prompt, options)
            : client.GenerateImage(prompt, options);
        Assert.That(image.ImageUri, Is.Null);
        Assert.That(image.ImageBytes, Is.Not.Null);

        ValidateGeneratedImage(image.ImageBytes, "stop");
    }

    [Test]
    public void GenerateImageCanParseServiceError()
    {
        ImageClient client = new("dall-e-3", new ApiKeyCredential("fake_key"));
        string prompt = "An isolated stop sign.";
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageAsync(prompt));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImage(prompt));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    public async Task GenerationOfMultipleImagesWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string prompt = "An isolated stop sign.";

        GeneratedImageCollection images = IsAsync
            ? await client.GenerateImagesAsync(prompt, 2)
            : client.GenerateImages(prompt, 2);

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2024));
        Assert.That(images.Count, Is.EqualTo(2));

        foreach (GeneratedImage image in images)
        {
            Assert.That(image.ImageUri, Is.Not.Null);
            Assert.That(image.ImageBytes, Is.Null);
            ValidateGeneratedImage(image.ImageUri, "stop");
        }
    }

    [Test]
    public async Task GenerationOfMultipleImagesWithBytesResponseWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string prompt = "An isolated stop sign.";

        ImageGenerationOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        GeneratedImageCollection images = IsAsync
            ? await client.GenerateImagesAsync(prompt, 2, options)
            : client.GenerateImages(prompt, 2, options);

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2024));
        Assert.That(images.Count, Is.EqualTo(2));

        foreach (GeneratedImage image in images)
        {
            Assert.That(image.ImageUri, Is.Null);
            Assert.That(image.ImageBytes, Is.Not.Null);
            ValidateGeneratedImage(image.ImageBytes, "stop");
        }
    }

    [Test]
    public void GenerateImagesCanParseServiceError()
    {
        ImageClient client = new("dall-e-3", new ApiKeyCredential("fake_key"));
        string prompt = "An isolated stop sign.";
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImagesAsync(prompt, 2));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImages(prompt, 2));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    #endregion

    #region GenerateImageEdits

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImage image = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream mask = File.OpenRead(maskImagePath);

            image = IsAsync
                ? await client.GenerateImageEditAsync(mask, maskFilename, CatPrompt)
                : client.GenerateImageEdit(mask, maskFilename, CatPrompt);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = IsAsync
                ? await client.GenerateImageEditAsync(maskImagePath, CatPrompt)
                : client.GenerateImageEdit(maskImagePath, CatPrompt);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);

        ValidateGeneratedImage(image.ImageUri, "cat", "Note that it likely depicts some sort of animal.");
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
            ResponseFormat = GeneratedImageFormat.Bytes
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

        ValidateGeneratedImage(image.ImageBytes, "cat", "Note that it likely depicts some sort of animal.");
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

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream originalImage = File.OpenRead(originalImagePath);
            using FileStream mask = File.OpenRead(maskImagePath);

            image = IsAsync
                ? await client.GenerateImageEditAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename)
                : client.GenerateImageEdit(mask, maskFilename, CatPrompt);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = IsAsync
                ? await client.GenerateImageEditAsync(originalImagePath, CatPrompt, maskImagePath)
                : client.GenerateImageEdit(maskImagePath, CatPrompt, maskImagePath);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);

        ValidateGeneratedImage(image.ImageUri, "cat", "Note that it likely depicts some sort of animal.");
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
            ResponseFormat = GeneratedImageFormat.Bytes
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

        ValidateGeneratedImage(image.ImageBytes, "cat", "Note that it likely depicts some sort of animal.");
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

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream mask = File.OpenRead(maskImagePath);

            images = IsAsync
                ? await client.GenerateImageEditsAsync(mask, maskFilename, CatPrompt, 2)
                : client.GenerateImageEdits(mask, maskFilename, CatPrompt, 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageEditsAsync(maskImagePath, CatPrompt, 2)
                : client.GenerateImageEdits(maskImagePath, CatPrompt, 2);
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
            ValidateGeneratedImage(image.ImageUri, "cat", "Note that it likely depicts some sort of animal.");
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
            ResponseFormat = GeneratedImageFormat.Bytes
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
            ValidateGeneratedImage(image.ImageBytes, "cat", "Note that it likely depicts some sort of animal.");
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

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream originalImage = File.OpenRead(originalImagePath);
            using FileStream mask = File.OpenRead(maskImagePath);

            images = IsAsync
                ? await client.GenerateImageEditsAsync(originalImage, originalImageFilename, CatPrompt, mask, maskFilename, 2)
                : client.GenerateImageEdits(mask, maskFilename, CatPrompt, 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageEditsAsync(originalImagePath, CatPrompt, maskImagePath, 2)
                : client.GenerateImageEdits(maskImagePath, CatPrompt, maskImagePath, 2);
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
            ValidateGeneratedImage(image.ImageUri, "cat", "Note that it likely depicts some sort of animal.");
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
            ResponseFormat = GeneratedImageFormat.Bytes
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
            ValidateGeneratedImage(image.ImageBytes, "cat", "Note that it likely depicts some sort of animal.");
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

    #endregion

    #region GenerateImageVariations

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageVariationWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImage image = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream imageFile = File.OpenRead(imagePath);

            image = IsAsync
                ? await client.GenerateImageVariationAsync(imageFile, imageFilename)
                : client.GenerateImageVariation(imageFile, imageFilename);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = IsAsync
                ? await client.GenerateImageVariationAsync(imagePath)
                : client.GenerateImageVariation(imagePath);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);

        ValidateGeneratedImage(image.ImageUri, "cat", "Note that it likely depicts some sort of animal.");
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageVariationWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImage image = null;

        ImageVariationOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream imageFile = File.OpenRead(imagePath);

            image = IsAsync
                ? await client.GenerateImageVariationAsync(imageFile, imageFilename, options)
                : client.GenerateImageVariation(imageFile, imageFilename, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = IsAsync
                ? await client.GenerateImageVariationAsync(imagePath, options)
                : client.GenerateImageVariation(imagePath, options);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.ImageUri, Is.Null);
        Assert.That(image.ImageBytes, Is.Not.Null);

        ValidateGeneratedImage(image.ImageBytes, "cat", "Note that it likely depicts some sort of animal.");
    }

    [Test]
    public void GenerateImageVariationFromStreamCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        using FileStream imageFile = File.OpenRead(imagePath);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageVariationAsync(imageFile, imageFilename));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageVariation(imageFile, imageFilename));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    public void GenerateImageVariationFromPathCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageVariationAsync(imagePath));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageVariation(imagePath));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageVariationsWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream imageFile = File.OpenRead(imagePath);

            images = IsAsync
                ? await client.GenerateImageVariationsAsync(imageFile, imageFilename, 2)
                : client.GenerateImageVariations(imageFile, imageFilename, 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageVariationsAsync(imagePath, 2)
                : client.GenerateImageVariations(imagePath, 2);
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
            ValidateGeneratedImage(image.ImageUri, "cat", "Note that it likely depicts some sort of animal.");
        }
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageVariationsWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImageCollection images = null;

        ImageVariationOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream imageFile = File.OpenRead(imagePath);

            images = IsAsync
                ? await client.GenerateImageVariationsAsync(imageFile, imageFilename, 2, options)
                : client.GenerateImageVariations(imageFile, imageFilename, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageVariationsAsync(imagePath, 2, options)
                : client.GenerateImageVariations(imagePath, 2, options);
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
            ValidateGeneratedImage(image.ImageBytes, "cat", "Note that it likely depicts some sort of animal.");
        }
    }

    [Test]
    public void GenerateMultipleImageVariationsFromStreamCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        using FileStream imageFile = File.OpenRead(imagePath);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageVariationsAsync(imageFile, imageFilename, 2));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageVariations(imageFile, imageFilename, 2));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [Test]
    public void GenerateMultipleImageVariationsFromPathCanParseServiceError()
    {
        ImageClient client = new("dall-e-2", new ApiKeyCredential("fake_key"));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);

        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageVariationsAsync(imagePath, 2));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GenerateImageVariations(imagePath, 2));
        }

        Assert.That(ex.Status, Is.EqualTo(401));
    }

    #endregion

    private void ValidateGeneratedImage(Uri imageUri, string expectedSubstring, string descriptionHint = null)
    {
        ChatClient chatClient = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart($"Describe this image for me. {descriptionHint}"),
                ChatMessageContentPart.CreateImagePart(imageUri)),
        ];
        ChatCompletionOptions chatOptions = new() { MaxOutputTokenCount = 2048 };
        ClientResult<ChatCompletion> result = chatClient.CompleteChat(messages, chatOptions);

        Assert.That(result.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring(expectedSubstring));
    }

    private void ValidateGeneratedImage(BinaryData imageBytes, string expectedSubstring, string descriptionHint = null)
    {
        ChatClient chatClient = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart($"Describe this image for me. {descriptionHint}"),
                ChatMessageContentPart.CreateImagePart(imageBytes, "image/png")),
        ];
        ChatCompletionOptions chatOptions = new() { MaxOutputTokenCount = 2048 };
        ClientResult<ChatCompletion> result = chatClient.CompleteChat(messages, chatOptions);

        Assert.That(result.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring(expectedSubstring));
    }
}
