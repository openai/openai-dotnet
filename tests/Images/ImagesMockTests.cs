using NUnit.Framework;
using OpenAI.Images;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Images;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Images")]
[Category("Smoke")]
public class ImagesMockTests : SyncAsyncTestBase
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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);

        GeneratedImage image = IsAsync
            ? await client.GenerateImageAsync("prompt")
            : client.GenerateImage("prompt");

        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImageRespectsTheCancellationToken()
    {
        ImageClient client = new ImageClient("model", s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateImageAsync("prompt", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateImage("prompt", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);

        GeneratedImageCollection images = IsAsync
            ? await client.GenerateImagesAsync("prompt", 2)
            : client.GenerateImages("prompt", 2);

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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);

        GeneratedImageCollection images = IsAsync
            ? await client.GenerateImagesAsync("prompt", 2)
            : client.GenerateImages("prompt", 2);
        GeneratedImage image = images.Single();

        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImagesRespectsTheCancellationToken()
    {
        ImageClient client = new ImageClient("model", s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateImagesAsync("prompt", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateImages("prompt", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImage image = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            image = IsAsync
                ? await client.GenerateImageEditAsync(stream, "filename", "prompt")
                : client.GenerateImageEdit(stream, "filename", "prompt");
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = IsAsync
                ? await client.GenerateImageEditAsync(maskImagePath, "prompt")
                : client.GenerateImageEdit(maskImagePath, "prompt");
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
        ImageClient client = new ImageClient("model", s_fakeCredential);
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateImageEditAsync(stream, "filename", "prompt", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateImageEdit(stream, "filename", "prompt", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImage image = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();
            using Stream mask = new MemoryStream();

            image = IsAsync
                ? await client.GenerateImageEditAsync(stream, "filename", "prompt", mask, "maskFilename")
                : client.GenerateImageEdit(stream, "filename", "prompt", mask, "maskFilename");
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            image = IsAsync
                ? await client.GenerateImageEditAsync(originalImagePath, "prompt", maskImagePath)
                : client.GenerateImageEdit(originalImagePath, "prompt", maskImagePath);
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
        ImageClient client = new ImageClient("model", s_fakeCredential);
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateImageEditAsync(stream, "filename", "prompt", stream, "maskFilename", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateImageEdit(stream, "filename", "prompt", stream, "maskFilename", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            images = IsAsync
                ? await client.GenerateImageEditsAsync(stream, "filename", "prompt", 2)
                : client.GenerateImageEdits(stream, "filename", "prompt", 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageEditsAsync(maskImagePath, "prompt", 2)
                : client.GenerateImageEdits(maskImagePath, "prompt", 2);
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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);
        string maskFilename = "images_empty_room_with_mask.png";
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            images = IsAsync
                ? await client.GenerateImageEditsAsync(stream, "filename", "prompt", 2)
                : client.GenerateImageEdits(stream, "filename", "prompt", 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageEditsAsync(maskImagePath, "prompt", 2)
                : client.GenerateImageEdits(maskImagePath, "prompt", 2);
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
        ImageClient client = new ImageClient("model", s_fakeCredential);
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateImageEditsAsync(stream, "filename", "prompt", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateImageEdits(stream, "filename", "prompt", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);
        string originalImageFilename = "images_empty_room.png";
        string maskFilename = "images_empty_room_with_mask.png";
        string originalImagePath = Path.Combine("Assets", originalImageFilename);
        string maskImagePath = Path.Combine("Assets", maskFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();
            using Stream mask = new MemoryStream();

            images = IsAsync
                ? await client.GenerateImageEditsAsync(stream, "filename", "prompt", mask, "maskFilename", 2)
                : client.GenerateImageEdits(stream, "filename", "prompt", mask, "maskFilename", 2);
        }
        else if (imageSourceKind == ImageSourceKind.UsingFilePath)
        {
            images = IsAsync
                ? await client.GenerateImageEditsAsync(originalImagePath, "prompt", maskImagePath, 2)
                : client.GenerateImageEdits(originalImagePath, "prompt", maskImagePath, 2);
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
        ImageClient client = new ImageClient("model", s_fakeCredential);
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateImageEditsAsync(stream, "filename", "prompt", stream, "maskFilename", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateImageEdits(stream, "filename", "prompt", stream, "maskFilename", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImage image = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            image = IsAsync
                ? await client.GenerateImageVariationAsync(stream, "filename")
                : client.GenerateImageVariation(stream, "filename");
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

        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImageVariationFromStreamRespectsTheCancellationToken()
    {
        ImageClient client = new ImageClient("model", s_fakeCredential);
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateImageVariationAsync(stream, "filename", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateImageVariation(stream, "filename", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            images = IsAsync
                ? await client.GenerateImageVariationsAsync(stream, "filename", 2)
                : client.GenerateImageVariations(stream, "filename", 2);
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
        ImageClient client = new ImageClient("model", s_fakeCredential, clientOptions);
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        GeneratedImageCollection images = null;

        if (imageSourceKind == ImageSourceKind.UsingStream)
        {
            using Stream stream = new MemoryStream();

            images = IsAsync
                ? await client.GenerateImageVariationsAsync(stream, "filename", 2)
                : client.GenerateImageVariations(stream, "filename", 2);
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

        GeneratedImage image = images.Single();
        Assert.That(image.RevisedPrompt, Is.EqualTo("new prompt"));
    }

    [Test]
    public void GenerateImageVariationsFromStreamRespectsTheCancellationToken()
    {
        ImageClient client = new ImageClient("model", s_fakeCredential);
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateImageVariationsAsync(stream, "filename", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateImageVariations(stream, "filename", 2, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status);
        response.SetContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };
    }
}
