using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Chat;

[TestFixture(true)]
[TestFixture(false)]
public partial class ChatWithVision : SyncAsyncTestBase
{
    public ChatWithVision(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public async Task DescribeAnImage()
    {
        string mediaType = "image/png";
        string filePath = Path.Combine("Assets", "images_dog_and_cat.png");
        using Stream stream = File.OpenRead(filePath);
        BinaryData imageData = BinaryData.FromStream(stream);

        ChatClient client = GetTestClient<ChatClient>(TestScenario.VisionChat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextMessageContentPart("Describe this image for me."),
                ChatMessageContentPart.CreateImageMessageContentPart(imageData, mediaType)),
        ];
        ChatCompletionOptions options = new() { MaxTokens = 2048 };

        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages, options)
            : client.CompleteChat(messages, options);
        Console.WriteLine(result.Value.Content[0].Text);
        Assert.That(result.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring("dog"));
    }
}
