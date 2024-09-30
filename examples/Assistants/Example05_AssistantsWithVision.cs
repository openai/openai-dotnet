﻿using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using System;
using System.ClientModel;
using System.IO;

namespace OpenAI.Examples;

public partial class AssistantExamples
{
    [Test]
    public void Example05_AssistantsWithVision()
    {
        // Assistants is a beta API and subject to change; acknowledge its experimental status by suppressing the matching warning.
        #pragma warning disable OPENAI001
        OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
        AssistantClient assistantClient = openAIClient.GetAssistantClient();

        OpenAIFile pictureOfAppleFile = fileClient.UploadFile(
            Path.Combine("Assets", "picture-of-apple.png"),
            FileUploadPurpose.Vision);
        Uri linkToPictureOfOrange = new("https://raw.githubusercontent.com/openai/openai-dotnet/refs/heads/main/examples/Assets/picture-of-orange.png");

        Assistant assistant = assistantClient.CreateAssistant(
            "gpt-4o",
            new AssistantCreationOptions()
            {
                Instructions = "When asked a question, attempt to answer very concisely. "
                    + "Prefer one-sentence answers whenever feasible."
            });

        AssistantThread thread = assistantClient.CreateThread(new ThreadCreationOptions()
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

        CollectionResult<StreamingUpdate> streamingUpdates = assistantClient.CreateRunStreaming(
            thread.Id,
            assistant.Id,
            new RunCreationOptions()
            {
                AdditionalInstructions = "When possible, try to sneak in puns if you're asked to compare things.",
            });

        foreach (StreamingUpdate streamingUpdate in streamingUpdates)
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

        // Delete temporary resources, if desired
        _ = fileClient.DeleteFile(pictureOfAppleFile.Id);
        _ = assistantClient.DeleteThread(thread.Id);
        _ = assistantClient.DeleteAssistant(assistant.Id);
    }
}
