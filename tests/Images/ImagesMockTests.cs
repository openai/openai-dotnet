using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Images;
using System;
using System.ClientModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Images;

[Parallelizable(ParallelScope.All)]
[Category("Images")]
[Category("Smoke")]
public class ImagesMockTests : ClientTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public ImagesMockTests(bool isAsync)
        : base(isAsync)
    {
    }

    public enum ImageSourceKind
    {
        UsingStream,
        UsingFilePath
    }

    private static Array s_imageSourceKindSource = Enum.GetValues(typeof(ImageSourceKind));

    [Test]
    public async Task GenerateImageDeserializesRevisedPrompt()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "revised_prompt": "new prompt"
                }
            ]
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));

        GeneratedImage image = await client.GenerateImageAsync("prompt");

        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImageRespectsTheCancellationToken()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();
        Assert.That(async () => await client.GenerateImageAsync("prompt", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public async Task GenerateImagesDeserializesCreatedAt()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "created": 1704096000,
            "data": []
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));

        GeneratedImageCollection images = await client.GenerateImagesAsync("prompt", 2);
        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    public async Task GenerateImagesDeserializesRevisedPrompt()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "revised_prompt": "new prompt"
                }
            ]
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));

        GeneratedImageCollection images = await client.GenerateImagesAsync("prompt", 2);
        GeneratedImage image = images.Single();

        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImagesRespectsTheCancellationToken()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateImagesAsync("prompt", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditDeserializesRevisedPrompt(ImageSourceKind imageSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "revised_prompt": "new prompt"
                }
            ]
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImage image = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            image = await client.GenerateImageEditAsync(stream, "filename", "prompt");
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = await client.GenerateImageEditAsync(maskImagePath, "prompt");
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImageEditFromStreamRespectsTheCancellationToken()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential));
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateImageEditAsync(stream, "filename", "prompt", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditWithMaskDeserializesRevisedPrompt(ImageSourceKind imageSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "revised_prompt": "new prompt"
                }
            ]
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImage image = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();
            using Stream mask = new MemoryStream();

            image = await client.GenerateImageEditAsync(stream, "filename", "prompt", mask, "maskFilename");
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = await client.GenerateImageEditAsync(originalImagePath, "prompt", maskImagePath);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImageEditFromStreamWithMaskRespectsTheCancellationToken()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential));
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateImageEditAsync(stream, "filename", "prompt", stream, "maskFilename", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditsDeserializesCreatedAt(ImageSourceKind imageSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "created": 1704096000,
            "data": []
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            images = await client.GenerateImageEditsAsync(stream, "filename", "prompt", 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageEditsAsync(maskImagePath, "prompt", 2);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditsDeserializesRevisedPrompt(ImageSourceKind imageSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "revised_prompt": "new prompt"
                }
            ]
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            images = await client.GenerateImageEditsAsync(stream, "filename", "prompt", 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageEditsAsync(maskImagePath, "prompt", 2);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        GeneratedImage image = images.Single();
        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImageEditsFromStreamRespectsTheCancellationToken()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential));
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateImageEditsAsync(stream, "filename", "prompt", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageEditsWithMaskDeserializesRevisedPrompt(ImageSourceKind imageSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "revised_prompt": "new prompt"
                }
            ]
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();
            using Stream mask = new MemoryStream();

            images = await client.GenerateImageEditsAsync(stream, "filename", "prompt", mask, "maskFilename", 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageEditsAsync(originalImagePath, "prompt", maskImagePath, 2);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        GeneratedImage image = images.Single();
        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImageEditsFromStreamWithMaskRespectsTheCancellationToken()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential));
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateImageEditsAsync(stream, "filename", "prompt", stream, "maskFilename", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageVariationDeserializesRevisedPrompt(ImageSourceKind imageSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "revised_prompt": "new prompt"
                }
            ]
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImage image = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            image = await client.GenerateImageVariationAsync(stream, "filename");
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = await client.GenerateImageVariationAsync(imagePath);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImageVariationFromStreamRespectsTheCancellationToken()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential));
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();
        Assert.That(async () => await client.GenerateImageVariationAsync(stream, "filename", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageVariationsDeserializesCreatedAt(ImageSourceKind imageSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "created": 1704096000,
            "data": []
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            images = await client.GenerateImageVariationsAsync(stream, "filename", 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageVariationsAsync(imagePath, 2);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        Assert.That(images.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    [TestCaseSource(nameof(s_imageSourceKindSource))]
    public async Task GenerateImageVariationsDeserializesRevisedPrompt(ImageSourceKind imageSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [
                {
                    "revised_prompt": "new prompt"
                }
            ]
        }
        """);
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential, clientOptions));
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            images = await client.GenerateImageVariationsAsync(stream, "filename", 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = await client.GenerateImageVariationsAsync(imagePath, 2);
        }
        else
        {
            Assert.Fail("Invalid source kind.");
        }

        GeneratedImage image = images.Single();
        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImageVariationsFromStreamRespectsTheCancellationToken()
    {
        ImageClient client = CreateProxyFromClient(new ImageClient("model", s_fakeCredential));
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateImageVariationsAsync(stream, "filename", 2, cancellationToken: cancellationSource.Token),
            Throws.InstanceOf<OperationCanceledException>());
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status).WithContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync,
            }
        };
    }
}
