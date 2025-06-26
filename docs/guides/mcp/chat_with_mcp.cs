// SAMPLE: call the official MCP library for .NET

#:package OpenAI@2.2.*-*
#:package ModelContextProtocol.Core@*-*
#:property PublishAot=false

using ModelContextProtocol.Client;
using OpenAI.Chat;

string mcpCommand = Environment.GetEnvironmentVariable("MCP_SERVER_COMMAND")!; // path to a stdio mcp server
string key = Environment.GetEnvironmentVariable("OPENAI_KEY")!;

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
await foreach (McpClientTool chatTool in mcpClient.EnumerateToolsAsync())
{
    options.Tools.Add(chatTool.AsOpenAIChatTool());
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
        foreach(var call in chatCompletion.ToolCalls)
        {
            var mcpArguments = call.FunctionArguments.ToObjectFromJson<Dictionary<string, object>>();
            Console.WriteLine("Tool call detected, calling MCP server...");
            ModelContextProtocol.Protocol.CallToolResult result = await mcpClient.CallToolAsync(call.FunctionName, mcpArguments!);
            Console.WriteLine($"tool call result {result.Content[0]}");
            ChatMessage message = result.ToMessage(call);
            conversation.Add(message);
        }     
        goto start;
    case ChatFinishReason.Stop:
        Console.WriteLine(chatCompletion.Content[0].Text);
        break;
    default:
        throw new NotImplementedException();
}

#region TEMPORARY
// this is temporary. all these APIs will endup being in one of the packages used here. 
public static class TemporaryExtensions
{
    public static ChatMessage ToMessage(this ModelContextProtocol.Protocol.CallToolResult mcpCallResult, ChatToolCall openaiCall)
    {
        List<ChatMessageContentPart> parts = new();
        var sc = mcpCallResult.StructuredContent;

        foreach (ModelContextProtocol.Protocol.ContentBlock block in mcpCallResult.Content)
        {
            if (block is ModelContextProtocol.Protocol.TextContentBlock textContent)
            {
                parts.Add(ChatMessageContentPart.CreateTextPart(textContent.Text));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        ToolChatMessage message = ChatMessage.CreateToolMessage(openaiCall.Id, parts);
        return message;
    }

    // this is in the adapter package; waiting for package to be dropped. 
    public static ChatTool AsOpenAIChatTool(this Microsoft.Extensions.AI.AIFunction mcpTool)
    {
        return ChatTool.CreateFunctionTool(
            mcpTool.Name,
            mcpTool.Description,
            BinaryData.FromString(mcpTool.JsonSchema.GetRawText()));
    }
}
#endregion