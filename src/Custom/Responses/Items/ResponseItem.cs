using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ItemResource")]
public partial class ResponseItem
{
    // CUSTOM: Specify read-only semantics for ID
    [CodeGenMember("Id")]
    public string Id { get; internal set; }

    public static UserMessageResponseItem CreateUserMessageItem(IEnumerable<ResponseContentPart> content)
    {
        return new UserMessageResponseItem(content);
    }

    public static UserMessageResponseItem CreateUserMessageItem(string inputTextContent)
    {
        return new UserMessageResponseItem(inputTextContent);
    }

    public static DeveloperMessageResponseItem CreateDeveloperMessageItem(IEnumerable<ResponseContentPart> content)
    {
        return new DeveloperMessageResponseItem(content);
    }

    public static DeveloperMessageResponseItem CreateDeveloperMessageItem(string inputTextContent)
    {;
        return new DeveloperMessageResponseItem(inputTextContent);
    }

    public static SystemMessageResponseItem CreateSystemMessageItem(IEnumerable<ResponseContentPart> content)
    {
        return new SystemMessageResponseItem(content);
    }

    public static SystemMessageResponseItem CreateSystemMessageItem(string inputTextContent)
    {
        return new SystemMessageResponseItem(inputTextContent);
    }

    public static AssistantMessageResponseItem CreateAssistantMessageItem(IEnumerable<ResponseContentPart> content)
    {
        return new AssistantMessageResponseItem(content);
    }

    public static AssistantMessageResponseItem CreateAssistantMessageItem(string outputTextContent)
    {
        return new AssistantMessageResponseItem(outputTextContent);
    }

    [Experimental("OPENAICUA001")]
    public static ComputerCallResponseItem CreateComputerCallItem(string callId, ComputerCallAction action, IEnumerable<ComputerCallSafetyCheck> pendingSafetyChecks)
    {
        return new ComputerCallResponseItem(callId, action, pendingSafetyChecks);
    }

    [Experimental("OPENAICUA001")]
    public static ComputerCallOutputResponseItem CreateComputerCallOutputItem(string callId, ComputerCallOutput output)
    {
        return new ComputerCallOutputResponseItem(callId, output);
    }

    public static WebSearchCallResponseItem CreateWebSearchCallItem()
    {
        return new WebSearchCallResponseItem();
    }

    public static FileSearchCallResponseItem CreateFileSearchCallItem(IEnumerable<string> queries)
    {
        return new FileSearchCallResponseItem(queries);
    }

    public static FunctionCallResponseItem CreateFunctionCallItem(string callId, string functionName, BinaryData functionArguments)
    {
        return new FunctionCallResponseItem(callId, functionName, functionArguments);
    }

    public static FunctionCallOutputResponseItem CreateFunctionCallOutputItem(string callId, string functionOutput)
    {
        return new FunctionCallOutputResponseItem(callId, functionOutput);
    }

    public static ReasoningResponseItem CreateReasoningItem(IEnumerable<ReasoningSummaryPart> summary)
    {
        return new ReasoningResponseItem(summary);
    }

    public static ReasoningResponseItem CreateReasoningItem(string summaryText)
    {
        return new ReasoningResponseItem(summaryText);
    }

    public static ReferenceResponseItem CreateReferenceItem(string id)
    {
        return new ReferenceResponseItem(id);
    }

    public static McpToolCallApprovalRequestItem CreateMcpApprovalRequestItem(string id, string serverLabel, string name, BinaryData arguments)
    {
        return new McpToolCallApprovalRequestItem(id, serverLabel, name, arguments);
    }

    public static McpToolCallApprovalResponseItem CreateMcpApprovalResponseItem(string approvalRequestId, bool approved)
    {
        return new McpToolCallApprovalResponseItem(approvalRequestId, approved);
    }

    public static McpToolCallItem CreateMcpToolCallItem(string serverLabel, string name, BinaryData arguments)
    {
        return new McpToolCallItem(serverLabel, name, arguments);
    }

    public static McpToolDefinitionListItem CreateMcpToolDefinitionListItem(string serverLabel, IEnumerable<McpToolDefinition> toolDefinitions)
    {
        return new McpToolDefinitionListItem(serverLabel, toolDefinitions);
    }

}
