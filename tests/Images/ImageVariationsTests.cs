using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Images;
using System;
using System.ClientModel;
using System.IO;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Images;

public partial class ImageVariationsTests : ImageTestFixtureBase
{
    public ImageVariationsTests(bool isAsync) : base(isAsync)
    {
        TestTimeoutInSeconds = 30;
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageVariationWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>(TestScenario.Images, "dall-e-2");
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImage image = null;

        ImageVariationOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Uri,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream imageFile = File.OpenRead(imagePath);

            image = await client.GenerateImageVariationAsync(imageFile, imageFilename, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = await client.GenerateImageVariationAsync(imagePath, options);
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
    public async Task GenerateImageVariationWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>(TestScenario.Images, "dall-e-2");
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImage image = null;

        ImageVariationOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream imageFile = File.OpenRead(imagePath);

            image = await client.GenerateImageVariationAsync(imageFile, imageFilename, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = await client.GenerateImageVariationAsync(imagePath, options);
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
    public void GenerateImageVariationFromStreamCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        using FileStream imageFile = File.OpenRead(imagePath);

        ClientResultException ex = null;

        ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageVariationAsync(imageFile, imageFilename));
        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [RecordedTest]
    public void GenerateImageVariationFromPathCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);

        ClientResultException ex = null;

        ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageVariationAsync(imagePath));
        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [RecordedTest]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateMultipleImageVariationsWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>(TestScenario.Images, "dall-e-2");
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImageCollection images = null;

        ImageVariationOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Uri,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream imageFile = File.OpenRead(imagePath);

            images = await client.GenerateImageVariationsAsync(imageFile, imageFilename, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageVariationsAsync(imagePath, 2, options);
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
    public async Task GenerateMultipleImageVariationsWithBytesResponseWorks(ImageSourceKind imageSourceKind)
    {
        ImageClient client = GetProxiedOpenAIClient<ImageClient>(TestScenario.Images, "dall-e-2");
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImageCollection images = null;

        ImageVariationOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes,
            Size = GeneratedImageSize.W256xH256
        };

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using FileStream imageFile = File.OpenRead(imagePath);

            images = await client.GenerateImageVariationsAsync(imageFile, imageFilename, 2, options);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageVariationsAsync(imagePath, 2, options);
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
    public void GenerateMultipleImageVariationsFromStreamCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        using FileStream imageFile = File.OpenRead(imagePath);

        ClientResultException ex = null;

        ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageVariationsAsync(imageFile, imageFilename, 2));
        Assert.That(ex.Status, Is.EqualTo(401));
    }

    [RecordedTest]
    public void GenerateMultipleImageVariationsFromPathCanParseServiceError()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("dall-e-2", new ApiKeyCredential("fake_key"), InstrumentClientOptions(new OpenAIClientOptions())));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);

        ClientResultException ex = null;

        ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GenerateImageVariationsAsync(imagePath, 2));
        Assert.That(ex.Status, Is.EqualTo(401));
    }
}
