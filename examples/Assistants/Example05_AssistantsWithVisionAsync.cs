using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using System;
using System.ClientModel;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class AssistantExamples
{
    [Test]
    public async Task Example05_AssistantsWithVisionAsync()
    {
        // Assistants is a beta API and subject to change; acknowledge its experimental status by suppressing the matching warning.
#pragma warning disable OPENAI001
        OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        FileClient fileClient = openAIClient.GetFileClient();
        AssistantClient assistantClient = openAIClient.GetAssistantClient();

        OpenAIFileInfo pictureOfAppleFile = await fileClient.UploadFileAsync(
            "picture-of-apple.jpg",
            FileUploadPurpose.Vision);
        Uri linkToPictureOfOrange = new("https://platform.openai.com/fictitious-files/picture-of-orange.png");

        Assistant assistant = await assistantClient.CreateAssistantAsync(
            "gpt-4o",
            new AssistantCreationOptions()
            {
                Instructions = "When asked a question, attempt to answer very concisely. "
                    + "Prefer one-sentence answers whenever feasible."
            });

        AssistantThread thread = await assistantClient.CreateThreadAsync(new ThreadCreationOptions()
        {
            InitialMessages =
                {
                    new ThreadInitializationMessage(
                        MessageRole.User,
                        [
                            "Hello, assistant! Please compare these two images for me:",
                            MessageContent.FromImageFileId(pictureOfAppleFile.Id),
                            MessageContent.FromImageUrl(linkToPictureOfOrange),
                        ]),
                }
        });

        AsyncCollectionResult<StreamingUpdate> streamingUpdates = assistantClient.CreateRunStreamingAsync(
            thread,
            assistant,
            new RunCreationOptions()
            {
                AdditionalInstructions = "When possible, try to sneak in puns if you're asked to compare things.",
            });

        await foreach (StreamingUpdate streamingUpdate in streamingUpdates)
        {
            if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
            {
                Console.WriteLine($"--- Run started! ---");
            }
            if (streamingUpdate is MessageContentUpdate contentUpdate)
            {
                Console.Write(contentUpdate.Text);
            }
        }

        _ = await fileClient.DeleteFileAsync(pictureOfAppleFile.Id);
        _ = await assistantClient.DeleteThreadAsync(thread);
        _ = await assistantClient.DeleteAssistantAsync(assistant);
    }
}
