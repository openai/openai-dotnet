using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeConversationItemGA")]
public partial class GARealtimeItem
{
    public static GARealtimeMessageItem CreateAssistantMessageItem(IEnumerable<GARealtimeMessageContentPart> contentParts)
    {
        return new GARealtimeMessageItem(GARealtimeMessageRole.Assistant, contentParts);
    }

    public static GARealtimeMessageItem CreateAssistantMessageItem(string outputTextContent)
    {
        IEnumerable<GARealtimeMessageContentPart> contentParts = [new GARealtimeOutputTextMessageContentPart(outputTextContent)];
        return new GARealtimeMessageItem(GARealtimeMessageRole.Assistant, contentParts);
    }

    public static GARealtimeMessageItem CreateSystemMessageItem(IEnumerable<GARealtimeMessageContentPart> contentParts)
    {
        return new GARealtimeMessageItem(GARealtimeMessageRole.System, contentParts);
    }

    public static GARealtimeMessageItem CreateSystemMessageItem(string inputTextContent)
    {
        IEnumerable<GARealtimeMessageContentPart> contentParts = [new GARealtimeInputTextMessageContentPart(inputTextContent)];
        return new GARealtimeMessageItem(GARealtimeMessageRole.System, contentParts);
    }

    public static GARealtimeMessageItem CreateUserMessageItem(IEnumerable<GARealtimeMessageContentPart> contentParts)
    {
        return new GARealtimeMessageItem(GARealtimeMessageRole.User, contentParts);
    }

    public static GARealtimeMessageItem CreateUserMessageItem(string inputTextContent)
    {
        IEnumerable<GARealtimeMessageContentPart> contentParts = [new GARealtimeInputTextMessageContentPart(inputTextContent)];
        return new GARealtimeMessageItem(GARealtimeMessageRole.User, contentParts);
    }

    public static GARealtimeFunctionCallItem CreateFunctionCallItem(string callId, string functionName, BinaryData functionArguments)
    {
        return new GARealtimeFunctionCallItem(callId, functionName, functionArguments);
    }

    public static GARealtimeFunctionCallOutputItem CreateFunctionCallOutputItem(string callId, string functionOutput)
    {
        return new GARealtimeFunctionCallOutputItem(callId, functionOutput);
    }

    public static GARealtimeMcpToolCallApprovalRequestItem CreateMcpApprovalRequestItem(string id, string serverLabel, string name, BinaryData arguments)
    {
        return new GARealtimeMcpToolCallApprovalRequestItem(id, serverLabel, name, arguments);
    }

    public static GARealtimeMcpToolCallApprovalResponseItem CreateMcpApprovalResponseItem(string approvalRequestId, bool approved)
    {
        return new GARealtimeMcpToolCallApprovalResponseItem(approvalRequestId, approved);
    }

    public static GARealtimeMcpToolCallItem CreateMcpToolCallItem(string serverLabel, string name, BinaryData arguments)
    {
        return new GARealtimeMcpToolCallItem(serverLabel, name, arguments);
    }

    public static GARealtimeMcpToolDefinitionListItem CreateMcpToolDefinitionListItem(string serverLabel, IEnumerable<GARealtimeMcpToolDefinition> toolDefinitions)
    {
        return new GARealtimeMcpToolDefinitionListItem(serverLabel, toolDefinitions);
    }
}
