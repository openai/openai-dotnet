using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.TestProxy.Admin;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Images;

[Category("Images")]
public class ImageTestFixtureBase : OpenAIRecordedTestBase
{
    public ImageTestFixtureBase(bool isAsync) : base(isAsync)
    {
        // Replace large images with a small 1x1 PNG in recordings to save space
        BodyKeySanitizers.Add(new BodyKeySanitizer(
            new BodyKeySanitizerBody("$..b64_json")
            {
                Value = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAC0lEQVQYV2NgAAIAAAUAAarVyFEAAAAASUVORK5CYII="
            }));

        // Sanitize data URLs in chat messages to prevent huge base64 content
        BodyRegexSanitizers.Add(new BodyRegexSanitizer(
            new BodyRegexSanitizerBody()
            {
                Value = @"""url"": ""data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAC0lEQVQYV2NgAAIAAAUAAarVyFEAAAAASUVORK5CYII=""",
                Regex = @"""url""\s*:\s*""data:image/[^""]+"""
            }));
    }

    public enum ImageSourceKind
    {
        UsingStream,
        UsingFilePath
    }

    internal const string CatPrompt = "A big cat with big round eyes and large cat ears, sitting in an empty room and looking at the camera.";

    internal static Array s_imageSourceKindSource = Enum.GetValues(typeof(ImageSourceKind));

    public async Task ValidateGeneratedImage(Uri imageUri, IEnumerable<string> possibleExpectedSubstrings, string descriptionHint = null)
    {
        ChatClient chatClient = GetProxiedOpenAIClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart($"Describe this image for me. {descriptionHint}"),
                ChatMessageContentPart.CreateImagePart(imageUri)),
        ];
        ChatCompletionOptions chatOptions = new() { MaxOutputTokenCount = 2048 };
        ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages, chatOptions);

        Assert.That(result.Value?.Content, Has.Count.EqualTo(1));
        string contentText = result.Value.Content[0].Text.ToLowerInvariant();

        Assert.That(possibleExpectedSubstrings.Any(possibleExpectedSubstring => contentText.Contains(possibleExpectedSubstring)));
    }

    public async Task ValidateGeneratedImage(BinaryData imageBytes, IEnumerable<string> possibleExpectedSubstrings, string descriptionHint = null)
    {
        ChatClient chatClient = GetProxiedOpenAIClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart($"Describe this image for me. {descriptionHint}"),
                ChatMessageContentPart.CreateImagePart(imageBytes, "image/png")),
        ];
        ChatCompletionOptions chatOptions = new() { MaxOutputTokenCount = 2048 };
        ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages, chatOptions);

        Assert.That(result.Value?.Content, Has.Count.EqualTo(1));
        string contentText = result.Value.Content[0].Text.ToLowerInvariant();

        Assert.That(possibleExpectedSubstrings.Any(possibleExpectedSubstring => contentText.Contains(possibleExpectedSubstring)));
    }
}
