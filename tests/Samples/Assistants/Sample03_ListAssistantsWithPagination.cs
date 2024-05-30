using NUnit.Framework;
using OpenAI.Assistants;
using System;
using System.ClientModel;

namespace OpenAI.Samples;
public partial class AssistantSamples
{
    [Test]
    [Ignore("Compilation validation only")]
    public void Sample03_ListAssistantsWithPagination()
    {
        // Assistants is a beta API and subject to change; acknowledge its experimental status by suppressing the matching warning.
#pragma warning disable OPENAI001
        AssistantClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        int count = 0;

        PageableCollection<Assistant> assistants = client.GetAssistants();
        foreach (Assistant assistant in assistants)
        {
            Console.WriteLine($"[{count,3}] {assistant.Id} {assistant.CreatedAt:s} {assistant.Name}");

            count++;
        }
    }
}
