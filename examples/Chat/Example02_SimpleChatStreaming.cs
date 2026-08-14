#pragma warning disable SCME0005
﻿using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Threading.Tasks;
using System.ClientModel;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public async Task Example02_SimpleChatStreaming()
    {
        ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        AsyncStreamingClientResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreaming("Say 'this is a test.'");

        Console.Write($"[ASSISTANT]: ");
        await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
        {
            if (completionUpdate.ContentUpdate.Count > 0)
            {
                Console.Write(completionUpdate.ContentUpdate[0].Text);
            }
        }
    }
}
