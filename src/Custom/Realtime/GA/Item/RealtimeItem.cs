using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeConversationItemGA")]
public partial class RealtimeItem
{
    public static RealtimeMessageItem CreateAssistantMessageItem(IEnumerable<RealtimeMessageContentPart> contentParts)
    {
        return new RealtimeMessageItem(RealtimeMessageRole.Assistant, contentParts);
    }

    public static RealtimeMessageItem CreateAssistantMessageItem(string outputTextContent)
    {
        IEnumerable<RealtimeMessageContentPart> contentParts = [new RealtimeOutputTextMessageContentPart(outputTextContent)];
        return new RealtimeMessageItem(RealtimeMessageRole.Assistant, contentParts);
    }

    public static RealtimeMessageItem CreateSystemMessageItem(IEnumerable<RealtimeMessageContentPart> contentParts)
    {
        return new RealtimeMessageItem(RealtimeMessageRole.System, contentParts);
    }

    public static RealtimeMessageItem CreateSystemMessageItem(string inputTextContent)
    {
        IEnumerable<RealtimeMessageContentPart> contentParts = [new RealtimeInputTextMessageContentPart(inputTextContent)];
        return new RealtimeMessageItem(RealtimeMessageRole.System, contentParts);
    }

    public static RealtimeMessageItem CreateUserMessageItem(IEnumerable<RealtimeMessageContentPart> contentParts)
    {
        return new RealtimeMessageItem(RealtimeMessageRole.User, contentParts);
    }

    public static RealtimeMessageItem CreateUserMessageItem(string inputTextContent)
    {
        IEnumerable<RealtimeMessageContentPart> contentParts = [new RealtimeInputTextMessageContentPart(inputTextContent)];
        return new RealtimeMessageItem(RealtimeMessageRole.User, contentParts);
    }

    public static RealtimeFunctionCallItem CreateFunctionCallItem(string callId, string functionName, BinaryData functionArguments)
    {
        return new RealtimeFunctionCallItem(callId, functionName, functionArguments);
    }

    public static RealtimeFunctionCallOutputItem CreateFunctionCallOutputItem(string callId, string functionOutput)
    {
        return new RealtimeFunctionCallOutputItem(callId, functionOutput);
    }

    public static RealtimeMcpToolCallApprovalRequestItem CreateMcpApprovalRequestItem(string id, string serverLabel, string name, BinaryData arguments)
    {
        return new RealtimeMcpToolCallApprovalRequestItem(id, serverLabel, name, arguments);
    }

    public static RealtimeMcpToolCallApprovalResponseItem CreateMcpApprovalResponseItem(string approvalRequestId, bool approved)
    {
        return new RealtimeMcpToolCallApprovalResponseItem(approvalRequestId, approved);
    }

    public static RealtimeMcpToolCallItem CreateMcpToolCallItem(string serverLabel, string name, BinaryData arguments)
    {
        return new RealtimeMcpToolCallItem(serverLabel, name, arguments);
    }

    public static RealtimeMcpToolDefinitionListItem CreateMcpToolDefinitionListItem(string serverLabel, IEnumerable<RealtimeMcpToolDefinition> toolDefinitions)
    {
        return new RealtimeMcpToolDefinitionListItem(serverLabel, toolDefinitions);
    }
}
