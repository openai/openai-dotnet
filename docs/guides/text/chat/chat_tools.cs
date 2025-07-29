// SAMPLE: Generate text from a simple prompt
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start 
#:package OpenAI@2.2.*-*
#:property PublishAot=false

using OpenAI.Chat; 

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ChatClient client = new("gpt-4.1", key);

ChatCompletionOptions options = new();
options.Tools.Add(ChatTool.CreateFunctionTool(
    functionName: nameof(MyTools.GetCurrentTime),
    functionDescription: "Get the current time in HH:mm format",
    functionParameters: BinaryData.FromObjectAsJson(new
    {
        type = "object",
        properties = new 
        {
            utc = new
            {
                type = "boolean",
                description = "If true, return the time in UTC. If false, return the local time."
            }
        },
        required = new object[0]
    })
));

List<ChatMessage> messages = [
    ChatMessage.CreateUserMessage("what is the current local time?")
];

complete:
ChatCompletion completion = client.CompleteChat(messages, options);
messages.AddRange(ChatMessage.CreateAssistantMessage(completion));
switch(completion.FinishReason)
{
    case ChatFinishReason.ToolCalls:
        Console.WriteLine($"{completion.ToolCalls.Count} tool call[s] detected.");
        foreach (ChatToolCall toolCall in completion.ToolCalls)
        {
            if (toolCall.FunctionName == nameof(MyTools.GetCurrentTime))
            {
                string result = MyTools.GetCurrentTime();
                messages.Add(ChatMessage.CreateToolMessage(toolCall.Id, result));
            }
            else
            {
                Console.WriteLine($"Unknown tool call: {toolCall.FunctionName}");
            }
        }
        goto complete;
    case ChatFinishReason.Stop:
        Console.WriteLine(completion.Content[0].Text);
        return;
    default:
        Console.WriteLine("Unexpected finish reason: " + completion.FinishReason);
        return;
}

public static class MyTools
{
    public static string GetCurrentTime(bool utc = false) => utc ? DateTime.UtcNow.ToString("HH:mm") : DateTime.Now.ToString("HH:mm");
}