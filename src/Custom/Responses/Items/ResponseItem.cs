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
    // CUSTOM: Added setter because this is required in output scenarios and optional in input scenarios.
    [CodeGenMember("Id")]
    public string Id { get; set; }

    public static MessageResponseItem CreateUserMessageItem(IEnumerable<ResponseContentPart> contentParts)
    {
        Argument.AssertNotNullOrEmpty(contentParts, nameof(contentParts));
        return new InternalResponsesUserMessage(contentParts);
    }

    public static MessageResponseItem CreateUserMessageItem(string inputTextContent)
    {
        Argument.AssertNotNull(inputTextContent, nameof(inputTextContent));
        return new InternalResponsesUserMessage(
            internalContent: [ResponseContentPart.CreateInputTextPart(inputTextContent)]);
    }

    public static MessageResponseItem CreateDeveloperMessageItem(IEnumerable<ResponseContentPart> contentParts)
    {
        Argument.AssertNotNull(contentParts, nameof(contentParts));
        return new InternalResponsesDeveloperMessage(contentParts);
    }

    public static MessageResponseItem CreateDeveloperMessageItem(string inputTextContent)
    {
        Argument.AssertNotNull(inputTextContent, nameof(inputTextContent));
        return new InternalResponsesDeveloperMessage(
            internalContent: [ResponseContentPart.CreateInputTextPart(inputTextContent)]);
    }

    public static MessageResponseItem CreateSystemMessageItem(IEnumerable<ResponseContentPart> contentParts)
    {
        Argument.AssertNotNull(contentParts, nameof(contentParts));
        return new InternalResponsesSystemMessage(contentParts);
    }

    public static MessageResponseItem CreateSystemMessageItem(string inputTextContent)
    {
        Argument.AssertNotNull(inputTextContent, nameof(inputTextContent));
        return new InternalResponsesSystemMessage(
            internalContent: [ResponseContentPart.CreateInputTextPart(inputTextContent)]);
    }

    public static MessageResponseItem CreateAssistantMessageItem(IEnumerable<ResponseContentPart> contentParts)
    {
        Argument.AssertNotNull(contentParts, nameof(contentParts));
        return new InternalResponsesAssistantMessage(contentParts);
    }

    public static MessageResponseItem CreateAssistantMessageItem(string outputTextContent, IEnumerable<ResponseMessageAnnotation> annotations = null)
    {
        Argument.AssertNotNull(outputTextContent, nameof(outputTextContent));
        return new InternalResponsesAssistantMessage(
            internalContent:
            [
                new InternalItemContentOutputText(outputTextContent, annotations ?? []),
            ]);
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

    public static ReasoningResponseItem CreateReasoningItem(IEnumerable<ReasoningSummaryPart> summaryParts)
    {
        return new ReasoningResponseItem(summaryParts);
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
