using NUnit.Framework;
using OpenAI.Containers;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponseExamples
{
    [Test]
    public void Example10_CodeInterpreter()
    {
        OpenAIResponseClient client = new(model: "gpt-5", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        CodeInterpreterToolContainer container = new(CodeInterpreterToolContainerConfiguration.CreateAutomaticContainerConfiguration());
        CodeInterpreterTool codeInterpreterTool = new(container);
        ResponseCreationOptions options = new()
        {
            Tools = { codeInterpreterTool }
        };

        List<ResponseItem> inputItems =
        [
            ResponseItem.CreateUserMessageItem("Create an Excel spreadsheet that contains the mathematical times tables from 1-12."),
        ];

        OpenAIResponse response = client.CreateResponse(inputItems, options);

        MessageResponseItem message = response.OutputItems
            .OfType<MessageResponseItem>()
            .FirstOrDefault();

        ResponseContentPart contentPart = message.Content
            .Where(part => part.Kind == ResponseContentPartKind.OutputText)
            .FirstOrDefault();

        ContainerFileCitationMessageAnnotation containerFileCitation = contentPart.OutputTextAnnotations
            .OfType<ContainerFileCitationMessageAnnotation>()
            .FirstOrDefault();

        // Download the file from the container and save it.
        ContainerClient containerClient = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        BinaryData fileBytes = containerClient.DownloadContainerFile(containerFileCitation.ContainerId, containerFileCitation.FileId);
        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.xlsx");
        fileBytes.ToStream().CopyTo(stream);
    }
}

#pragma warning restore OPENAI001