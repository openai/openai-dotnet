using NUnit.Framework;
using OpenAI.Images;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Images;

public partial class ImagesTests : ImageTestFixtureBase
{
    public ImagesTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task BasicGenerationWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-3");

        string prompt = "An isolated stop sign.";

        GeneratedImage image = IsAsync
            ? await client.GenerateImageAsync(prompt)
            : client.GenerateImage(prompt);
        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);
        ValidateGeneratedImage(image.ImageUri, ["stop"]);
    }

    [Test]
    public async Task GenerationWithOptionsWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-3");

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

        ValidateGeneratedImage(image.ImageUri, ["stop"]);
    }

    [Test]
    public async Task GenerationWithBytesResponseWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-3");

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

        ValidateGeneratedImage(image.ImageBytes, ["stop"]);
    }

    [Test]
    public void GenerateImageCanParseServiceError()
    {
        ImageClient client = new("gpt-image-1", new ApiKeyCredential("fake_key"));
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

        ImageGenerationOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Uri,
            Size = GeneratedImageSize.W256xH256
        };

        GeneratedImageCollection images = IsAsync
            ? await client.GenerateImagesAsync(prompt, 2, options)
            : client.GenerateImages(prompt, 2, options);

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2024));
        Assert.That(images.Count, Is.EqualTo(2));

        foreach (GeneratedImage image in images)
        {
            Assert.That(image.ImageUri, Is.Not.Null);
            Assert.That(image.ImageBytes, Is.Null);
            ValidateGeneratedImage(image.ImageUri, ["stop"]);
        }
    }

    [Test]
    public async Task GenerationOfMultipleImagesWithBytesResponseWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images);

        string prompt = "An isolated stop sign.";

        GeneratedImageCollection images = IsAsync
            ? await client.GenerateImagesAsync(prompt, 2)
            : client.GenerateImages(prompt, 2);

        long unixTime2024 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2024));
        Assert.That(images.Count, Is.EqualTo(2));

        foreach (GeneratedImage image in images)
        {
            Assert.That(image.ImageUri, Is.Null);
            Assert.That(image.ImageBytes, Is.Not.Null);
            ValidateGeneratedImage(image.ImageBytes, ["stop"]);
        }
    }

    [Test]
    public void GenerateImagesCanParseServiceError()
    {
        ImageClient client = new("gpt-image-1", new ApiKeyCredential("fake_key"));
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

    [Test]
    public async Task GptImage1Works()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "gpt-image-1");

        string prompt = "An isolated stop sign.";

        ImageGenerationOptions options = new()
        {
            OutputFileFormat = GeneratedImageFileFormat.Webp,
            OutputCompressionFactor = 42,
            Background = GeneratedImageBackground.Transparent,
            ModerationLevel = GeneratedImageModerationLevel.Low,
            Size = GeneratedImageSize.W1024xH1536,
        };

        GeneratedImageCollection images = IsAsync
            ? await client.GenerateImagesAsync(prompt, 2, options)
            : client.GenerateImages(prompt, 2, options);

        long unixTime2025 = (new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2025));
        Assert.That(images.Count, Is.EqualTo(2));

        Assert.That(images.Usage.InputTokenCount, Is.GreaterThan(0));
        Assert.That(images.Usage.OutputTokenCount, Is.GreaterThan(0));
        Assert.That(images.Usage.TotalTokenCount, Is.GreaterThan(0));
        Assert.That(images.Usage.InputTokenDetails.TextTokenCount, Is.GreaterThan(0));
        Assert.That(images.Usage.InputTokenDetails.ImageTokenCount, Is.EqualTo(0));

        foreach (GeneratedImage image in images)
        {
            Assert.That(image.ImageUri, Is.Null);
            Assert.That(image.ImageBytes, Is.Not.Null);
        }
    }
}
