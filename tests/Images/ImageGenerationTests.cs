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
public partial class ImageGenerationTests : SyncAsyncTestBase
{
    public ImageGenerationTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    [Category("skipInCI")]
    public async Task BasicGenerationWorks()
    {
        ImageClient client = GetTestClient();

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
    [Category("skipInCI")]
    public async Task GenerationWithOptionsWorks()
    {
        ImageClient client = GetTestClient();

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
    }

    [Test]
    [Category("skipInCI")]
    public async Task GenerationWithBytesResponseWorks()
    {
        ImageClient client = GetTestClient();

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
    [Category("skipInCI")]
    public async Task GenerateImageEditWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string prompt = "A big cat with big, round eyes sitting in an empty room and looking at the camera.";
        string maskImagePath = Path.Combine("Assets", "images_empty_room_with_mask.png");

        GeneratedImage image = IsAsync
            ? await client.GenerateImageEditAsync(maskImagePath, prompt)
            : client.GenerateImageEdit(maskImagePath, prompt);
        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);

        ValidateGeneratedImage(image.ImageUri, "cat");
    }

    [Test]
    [Category("skipInCI")]
    public async Task GenerateImageEditWithMaskFileWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string prompt = "A big cat with big, round eyes sitting in an empty room and looking at the camera.";
        string originalImagePath = Path.Combine("Assets", "images_empty_room.png");
        string maskImagePath = Path.Combine("Assets", "images_empty_room_with_mask.png");

        GeneratedImage image = IsAsync
            ? await client.GenerateImageEditAsync(originalImagePath, prompt, maskImagePath)
            : client.GenerateImageEdit(originalImagePath, prompt, maskImagePath);
        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);

        ValidateGeneratedImage(image.ImageUri, "cat");
    }

    [Test]
    [Category("skipInCI")]
    public async Task GenerateImageEditWithBytesResponseWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");

        string prompt = "A big cat with big, round eyes sitting in an empty room and looking at the camera.";
        string maskImagePath = Path.Combine("Assets", "images_empty_room_with_mask.png");

        ImageEditOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        GeneratedImage image = IsAsync
            ? await client.GenerateImageEditAsync(maskImagePath, prompt, options)
            : client.GenerateImageEdit(maskImagePath, prompt, options);
        Assert.That(image.ImageUri, Is.Null);
        Assert.That(image.ImageBytes, Is.Not.Null);

        ValidateGeneratedImage(image.ImageBytes, "cat");
    }

    [Test]
    [Category("skipInCI")]
    public async Task GenerateImageVariationWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");
        string imagePath = Path.Combine("Assets", "images_dog_and_cat.png");

        GeneratedImage image = IsAsync
            ? await client.GenerateImageVariationAsync(imagePath)
            : client.GenerateImageVariation(imagePath);
        Assert.That(image.ImageUri, Is.Not.Null);
        Assert.That(image.ImageBytes, Is.Null);

        Console.WriteLine(image.ImageUri.AbsoluteUri);

        ValidateGeneratedImage(image.ImageUri, "dog");
    }

    [Test]
    [Category("skipInCI")]
    public async Task GenerateImageVariationWithBytesResponseWorks()
    {
        ImageClient client = GetTestClient<ImageClient>(TestScenario.Images, "dall-e-2");
        string imagePath = Path.Combine("Assets", "images_dog_and_cat.png");

        ImageVariationOptions options = new()
        {
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        GeneratedImage image = IsAsync
            ? await client.GenerateImageVariationAsync(imagePath, options)
            : client.GenerateImageVariation(imagePath, options);
        Assert.That(image.ImageUri, Is.Null);
        Assert.That(image.ImageBytes, Is.Not.Null);

        ValidateGeneratedImage(image.ImageBytes, "cat");
    }

    private void ValidateGeneratedImage(Uri imageUri, string expectedSubstring)
    {
        ChatClient chatClient = GetTestClient<ChatClient>(TestScenario.VisionChat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextMessageContentPart("Describe this image for me."),
                ChatMessageContentPart.CreateImageMessageContentPart(imageUri)),
        ];
        ChatCompletionOptions chatOptions = new() { MaxTokens = 2048 };
        ClientResult<ChatCompletion> result = chatClient.CompleteChat(messages, chatOptions);

        Assert.That(result.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring(expectedSubstring));
    }

    private void ValidateGeneratedImage(BinaryData imageBytes, string expectedSubstring)
    {
        ChatClient chatClient = GetTestClient<ChatClient>(TestScenario.VisionChat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextMessageContentPart("Describe this image for me."),
                ChatMessageContentPart.CreateImageMessageContentPart(imageBytes, "image/png")),
        ];
        ChatCompletionOptions chatOptions = new() { MaxTokens = 2048 };
        ClientResult<ChatCompletion> result = chatClient.CompleteChat(messages, chatOptions);

        Assert.That(result.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring(expectedSubstring));
    }

    private static ImageClient GetTestClient() => GetTestClient<ImageClient>(TestScenario.Images);
}
