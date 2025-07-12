// SAMPLE: call the official MCP library for .NET

#:package OpenAI@2.2.*-*
#:package ModelContextProtocol.Core@*-*
#:property PublishAot=false

using ModelContextProtocol;
using ModelContextProtocol.Client;
using OpenAI.Chat;
using System.Text.Json;

string mcpCommand = Environment.GetEnvironmentVariable("MCP_SERVER_COMMAND")!; // path to a stdio mcp server
string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;

ChatClient chatClient = new("gpt-4.1", key);

// Create MCP client to connect to the server
IMcpClient mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "MyServer",
    Command = mcpCommand,
    Arguments = [],
}));

// get avaliable tools and add them to completion options
ChatCompletionOptions options = new();
await foreach (McpClientTool tool in mcpClient.EnumerateToolsAsync())
{
    options.Tools.Add(ChatTool.CreateFunctionTool(tool.Name, tool.Description, BinaryData.FromString(tool.JsonSchema.GetRawText())));
}

// conversation thread
List<ChatMessage> conversation = new()
{
    ChatMessage.CreateUserMessage("Use an MCP tool")
};

start:
ChatCompletion chatCompletion = chatClient.CompleteChat(conversation, options);
conversation.Add(ChatMessage.CreateAssistantMessage(chatCompletion));
switch (chatCompletion.FinishReason)
{
    case ChatFinishReason.ToolCalls:
        foreach (var call in chatCompletion.ToolCalls)
        {
            var mcpArguments = call.FunctionArguments.ToObjectFromJson<Dictionary<string, object>>();
            Console.WriteLine("Tool call detected, calling MCP server...");
            var result = await mcpClient.CallToolAsync(call.FunctionName, mcpArguments!);
            Console.WriteLine($"Tool call result {result.Content[0]}");
            conversation.Add(ChatMessage.CreateToolMessage(call.Id, JsonSerializer.Serialize(result)));
        }
        goto start;
    case ChatFinishReason.Stop:
        Console.WriteLine(chatCompletion.Content[0].Text);
        break;
    default:
        throw new NotImplementedException();
}