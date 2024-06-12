using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public void Example01_SimpleChat_Cancellations()
    {
        ChatClient client = new(model: "gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        CancellationTokenSource ct = new CancellationTokenSource();
        RequestOptions options = new() { CancellationToken = ct.Token };

        // The following code will be simplified in the future.
        var wireFormat = new ModelReaderWriterOptions("W");
        ChatMessage message = ChatMessage.CreateUserMessage("Say 'this is a test.'");
        BinaryData json = ModelReaderWriter.Write(message, wireFormat);

        ClientResult result = client.CompleteChat(BinaryContent.Create(json), options);
        
        ChatCompletion completion = ModelReaderWriter.Read<ChatCompletion>(result.GetRawResponse().Content, wireFormat);
        Console.WriteLine($"[ASSISTANT]: {completion}");
    }
}
