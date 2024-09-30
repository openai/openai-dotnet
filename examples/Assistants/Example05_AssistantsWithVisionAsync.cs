﻿using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using System;
using System.ClientModel;
using System.IO;
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
        OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
        AssistantClient assistantClient = openAIClient.GetAssistantClient();

        OpenAIFile pictureOfAppleFile = await fileClient.UploadFileAsync(
            Path.Combine("Assets", "picture-of-apple.png"),
            FileUploadPurpose.Vision);
        Uri linkToPictureOfOrange = new("https://raw.githubusercontent.com/openai/openai-dotnet/refs/heads/main/examples/Assets/picture-of-orange.png");

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
                            MessageContent.FromImageUri(linkToPictureOfOrange),
                        ]),
                }
        });

        AsyncCollectionResult<StreamingUpdate> streamingUpdates = assistantClient.CreateRunStreamingAsync(
            thread.Id,
            assistant.Id,
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
        _ = await assistantClient.DeleteThreadAsync(thread.Id);
        _ = await assistantClient.DeleteAssistantAsync(assistant.Id);
    }
}
