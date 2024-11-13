﻿using NUnit.Framework;
using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.Collections.Generic;

namespace OpenAI.Examples;

public partial class AssistantExamples
{
    [Test]
    public void Example03_ListAssistantsWithPagination()
    {
        // Assistants is a beta API and subject to change; acknowledge its experimental status by suppressing the matching warning.
        AssistantClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        int count = 0;

        CollectionResult<Assistant> assistants = client.GetAssistants();
        foreach (Assistant assistant in assistants)
        {
            Console.WriteLine($"[{count,3}] {assistant.Id} {assistant.CreatedAt:s} {assistant.Name}");

            count++;
        }
    }
}
