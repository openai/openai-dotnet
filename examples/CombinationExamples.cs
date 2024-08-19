using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Images;
using System;
using System.ClientModel;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples.Miscellaneous;

public partial class CombinationExamples
{
    [Test]
    public void AlpacaArtAssessor()
    {
        // First, we create an image using dall-e-3:
        ImageClient imageClient = new("dall-e-3", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        ClientResult<GeneratedImage> imageResult = imageClient.GenerateImage(
            "a majestic alpaca on a mountain ridge, backed by an expansive blue sky accented with sparse clouds",
            new()
            {
                Style = GeneratedImageStyle.Vivid,
                Quality = GeneratedImageQuality.High,
                Size = GeneratedImageSize.W1792xH1024,
            });
        GeneratedImage imageGeneration = imageResult.Value;
        Console.WriteLine($"Majestic alpaca available at:\n{imageGeneration.ImageUri.AbsoluteUri}");

        // Now, we'll ask a cranky art critic to evaluate the image using gpt-4-vision-preview:
        ChatClient chatClient = new("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        ChatCompletion chatCompletion = chatClient.CompleteChat(
            [
                new SystemChatMessage("Assume the role of a cranky art critic. When asked to describe or "
                    + "evaluate imagery, focus on criticizing elements of subject, composition, and other details."),
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextMessageContentPart("describe the following image in a few sentences"),
                    ChatMessageContentPart.CreateImageMessageContentPart(imageGeneration.ImageUri)),
            ],
            new ChatCompletionOptions()
            {
                MaxTokens = 2048,
            }
            );

        string chatResponseText = chatCompletion.Content[0].Text;
        Console.WriteLine($"Art critique of majestic alpaca:\n{chatResponseText}");

        // Finally, we'll get some text-to-speech for that critical evaluation using tts-1-hd:
        AudioClient audioClient = new("tts-1-hd", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        ClientResult<BinaryData> ttsResult = audioClient.GenerateSpeechFromText(
            text: chatResponseText,
            GeneratedSpeechVoice.Fable,
            new SpeechGenerationOptions()
            {
                Speed = 0.9f,
                ResponseFormat = GeneratedSpeechFormat.Opus,
            });
        FileInfo ttsFileInfo = new($"{chatCompletion.Id}.opus");
        using (FileStream ttsFileStream = ttsFileInfo.Create())
        using (BinaryWriter ttsFileWriter = new(ttsFileStream))
        {
            ttsFileWriter.Write(ttsResult.Value);
        }
        Console.WriteLine($"Alpaca evaluation audio available at:\n{new Uri(ttsFileInfo.FullName).AbsoluteUri}");
    }

    [Test]
    public async Task CuriousCreatureCreator()
    {
        // First, we'll use gpt-4 to have a creative helper imagine a twist on a household pet
        ChatClient creativeWriterClient = new("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        ClientResult<ChatCompletion> creativeWriterResult = creativeWriterClient.CompleteChat(
            [
                new SystemChatMessage("You're a creative helper that specializes in brainstorming designs for concepts that fuse ordinary, mundane items with a fantastical touch. In particular, you can provide good one-paragraph descriptions of concept images."),
                new UserChatMessage("Imagine a household pet. Now add in a subtle touch of magic or 'different'. What do you imagine? Provide a one-paragraph description of a picture of this new creature, focusing on the details of the imagery such that it'd be suitable for creating a picture."),
            ],
            new ChatCompletionOptions()
            {
                MaxTokens = 2048,
            });
        string description = creativeWriterResult.Value.Content[0].Text;
        Console.WriteLine($"Creative helper's creature description:\n{description}");

        // Asynchronously, in parallel to the next steps, we'll get the creative description in the voice of Onyx
        AudioClient ttsClient = new("tts-1-hd", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        Task<ClientResult<BinaryData>> imageDescriptionAudioTask = ttsClient.GenerateSpeechFromTextAsync(
            description,
            GeneratedSpeechVoice.Onyx,
            new SpeechGenerationOptions()
            {
                Speed = 1.1f,
                ResponseFormat = GeneratedSpeechFormat.Opus,
            });
        _ = Task.Run(async () =>
        {
            ClientResult<BinaryData> audioResult = await imageDescriptionAudioTask;
            FileInfo audioFileInfo = new FileInfo($"{creativeWriterResult.Value.Id}-description.opus");
            using FileStream fileStream = audioFileInfo.Create();
            using BinaryWriter fileWriter = new(fileStream);
            fileWriter.Write(audioResult.Value);
            Console.WriteLine($"Spoken description available at:\n{new Uri(audioFileInfo.FullName).AbsoluteUri}");
        });

        // Meanwhile, we'll use dall-e-3 to generate a rendition of our LLM artist's vision
        ImageClient imageGenerationClient = new("dall-e-3", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        ClientResult<GeneratedImage> imageGenerationResult = await imageGenerationClient.GenerateImageAsync(
            description,
            new ImageGenerationOptions()
            {
                Size = GeneratedImageSize.W1792xH1024,
                Quality = GeneratedImageQuality.High,
            });
        Uri imageLocation = imageGenerationResult.Value.ImageUri;
        Console.WriteLine($"Creature image available at:\n{imageLocation.AbsoluteUri}");

        // Now, we'll use gpt-4-vision-preview to get a hopelessly taken assessment from a usually exigent art connoisseur
        ChatClient imageCriticClient = new("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        ClientResult<ChatCompletion> criticalAppraisalResult = await imageCriticClient.CompleteChatAsync(
            [
                new SystemChatMessage("Assume the role of an art critic. Although usually cranky and occasionally even referred to as a 'curmudgeon', you're somehow entirely smitten with the subject presented to you and, despite your best efforts, can't help but lavish praise when you're asked to appraise a provided image."),
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextMessageContentPart("Evaluate this image for me. What is it, and what do you think of it?"),
                    ChatMessageContentPart.CreateImageMessageContentPart(imageLocation)),
            ],
            new ChatCompletionOptions()
            {
                MaxTokens = 2048,
            });
        string appraisal = criticalAppraisalResult.Value.Content[0].Text;
        Console.WriteLine($"Critic's appraisal:\n{appraisal}");

        // Finally, we'll get that art expert's laudations in the voice of Fable
        ClientResult<BinaryData> appraisalAudioResult = await ttsClient.GenerateSpeechFromTextAsync(
            appraisal,
            GeneratedSpeechVoice.Fable,
            new SpeechGenerationOptions()
            {
                Speed = 0.9f,
                ResponseFormat = GeneratedSpeechFormat.Opus,
            });
        FileInfo criticAudioFileInfo = new($"{criticalAppraisalResult.Value.Id}-appraisal.opus");
        using (FileStream criticStream = criticAudioFileInfo.Create())
        using (BinaryWriter criticFileWriter = new(criticStream))
        {
            criticFileWriter.Write(appraisalAudioResult.Value);
        }
        Console.WriteLine($"Critical appraisal available at:\n{new Uri(criticAudioFileInfo.FullName).AbsoluteUri}");
    }
}
