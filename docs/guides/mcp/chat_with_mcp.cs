// SAMPLE: call the official MCP library for .NET

#:package OpenAI@2.2.*-*
#:package ModelContextProtocol.Core@*-*
#:property PublishAot=false

using System.Text.Json;
using ModelContextProtocol;
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
            ChatMessage message = call.ToMessage(result.Content.ToAIContents());
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
    // this needs to be in the adapter package
    public static ChatMessage ToMessage(this ChatToolCall openaiCall, IEnumerable<Microsoft.Extensions.AI.AIContent> contents)
    {
        List<ChatMessageContentPart> parts = new();
        foreach (Microsoft.Extensions.AI.AIContent content in contents)
        {
            string serialized = JsonSerializer.Serialize(content.RawRepresentation);
            using JsonDocument json = JsonDocument.Parse(serialized); 
            JsonElement text = json.RootElement.GetProperty("text");
            string textValue = text.GetString() ?? string.Empty;
            parts.Add(ChatMessageContentPart.CreateTextPart(textValue));
        }
        ToolChatMessage message = ChatMessage.CreateToolMessage(openaiCall.Id, parts);
        return message;
    }

    // this is in the adapter package
    public static ChatTool AsOpenAIChatTool(this Microsoft.Extensions.AI.AIFunction mcpTool)
    {
        return ChatTool.CreateFunctionTool(
            mcpTool.Name,
            mcpTool.Description,
            BinaryData.FromString(mcpTool.JsonSchema.GetRawText()));
    }
}
#endregion