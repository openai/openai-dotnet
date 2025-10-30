﻿using NUnit.Framework;
using OpenAI.Containers;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponseExamples
{
    [Test]
    public async Task Example10_CodeInterpreterAsync()
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
            ResponseItem.CreateUserMessageItem("Create an Excel spreadsheet that contains the mathematical times tables from 1-12 and make it available for download."),
        ];

        OpenAIResponse response = await client.CreateResponseAsync(inputItems, options);

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
        BinaryData fileBytes = await containerClient.DownloadContainerFileAsync(containerFileCitation.ContainerId, containerFileCitation.FileId);
        using FileStream stream = File.OpenWrite(containerFileCitation.Filename);
        fileBytes.ToStream().CopyTo(stream);
    }
}

#pragma warning restore OPENAI001