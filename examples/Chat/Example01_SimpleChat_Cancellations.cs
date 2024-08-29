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

        ChatMessage message = ChatMessage.CreateUserMessage("Say 'this is a test.'");
        var body = new
        {
            model = "gpt-4o",
            messages = new[] {
                new
                {
                    role = "user",
                    content = "Say \u0027this is a test.\u0027"
                }
            }
        };

        BinaryData json = BinaryData.FromObjectAsJson(body);
        ClientResult result = client.CompleteChat(BinaryContent.Create(json), options);

        // The following code will be simplified in the future.
        var wireFormat = new ModelReaderWriterOptions("W");
        ChatCompletion completion = ModelReaderWriter.Read<ChatCompletion>(result.GetRawResponse().Content, wireFormat);
        Console.WriteLine($"[ASSISTANT]: {completion}");
    }
}
