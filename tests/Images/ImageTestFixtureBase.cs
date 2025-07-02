using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Images;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Images")]
public class ImageTestFixtureBase : SyncAsyncTestBase
{
    public ImageTestFixtureBase(bool isAsync) : base(isAsync)
    {
    }

    public enum ImageSourceKind
    {
        UsingStream,
        UsingFilePath
    }

    internal const string CatPrompt = "A big cat with big round eyes and large cat ears, sitting in an empty room and looking at the camera.";

    internal static Array s_imageSourceKindSource = Enum.GetValues(typeof(ImageSourceKind));

    public void ValidateGeneratedImage(Uri imageUri, IEnumerable<string> possibleExpectedSubstrings, string descriptionHint = null)
    {
        ChatClient chatClient = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart($"Describe this image for me. {descriptionHint}"),
                ChatMessageContentPart.CreateImagePart(imageUri)),
        ];
        ChatCompletionOptions chatOptions = new() { MaxOutputTokenCount = 2048 };
        ClientResult<ChatCompletion> result = chatClient.CompleteChat(messages, chatOptions);

        Assert.That(result.Value?.Content, Has.Count.EqualTo(1));
        string contentText = result.Value.Content[0].Text.ToLowerInvariant();

        Assert.That(possibleExpectedSubstrings.Any(possibleExpectedSubstring => contentText.Contains(possibleExpectedSubstring)));
    }

    public void ValidateGeneratedImage(BinaryData imageBytes, IEnumerable<string> possibleExpectedSubstrings, string descriptionHint = null)
    {
        ChatClient chatClient = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart($"Describe this image for me. {descriptionHint}"),
                ChatMessageContentPart.CreateImagePart(imageBytes, "image/png")),
        ];
        ChatCompletionOptions chatOptions = new() { MaxOutputTokenCount = 2048 };
        ClientResult<ChatCompletion> result = chatClient.CompleteChat(messages, chatOptions);

        Assert.That(result.Value?.Content, Has.Count.EqualTo(1));
        string contentText = result.Value.Content[0].Text.ToLowerInvariant();

        Assert.That(possibleExpectedSubstrings.Any(possibleExpectedSubstring => contentText.Contains(possibleExpectedSubstring)));
    }
}
