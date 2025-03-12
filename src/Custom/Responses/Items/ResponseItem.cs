using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

[CodeGenType("ResponsesItem")]
public partial class ResponseItem
{
    // CUSTOM: Added custom constructor to be able to set the ID for those items where it is required.
    private protected ResponseItem(InternalResponsesItemType @type, string id)
    {
        Type = @type;
        Id = id;
    }

    public static MessageResponseItem CreateUserMessageItem(string content)
    {
        Argument.AssertNotNull(content, nameof(content));

        return new InternalResponsesSystemMessage(
            type: InternalResponsesItemType.Message,
            id: null,
            additionalBinaryDataProperties: null,
            status: null,
            MessageRole.User,
            internalContent: [ResponseContentPart.CreateInputTextPart(content)]);
    }

    public static MessageResponseItem CreateUserMessageItem(IEnumerable<ResponseContentPart> contentParts)
    {
        Argument.AssertNotNullOrEmpty(contentParts, nameof(contentParts));

        return new InternalResponsesSystemMessage(
            type: InternalResponsesItemType.Message,
            id: null,
            additionalBinaryDataProperties: null,
            status: null,
            MessageRole.User,
            internalContent: [.. contentParts]);
    }

    public static MessageResponseItem CreateDeveloperMessageItem(string content)
    {
        Argument.AssertNotNull(content, nameof(content));

        return new InternalResponsesSystemMessage(
            type: InternalResponsesItemType.Message,
            id: null,
            additionalBinaryDataProperties: null,
            status: null,
            MessageRole.Developer,
            internalContent: [ResponseContentPart.CreateInputTextPart(content)]);
    }

    public static MessageResponseItem CreateSystemMessageItem(string content)
    {
        Argument.AssertNotNull(content, nameof(content));

        return new InternalResponsesSystemMessage(
            type: InternalResponsesItemType.Message,
            id: null,
            additionalBinaryDataProperties: null,
            status: null,
            MessageRole.System,
            internalContent: [ResponseContentPart.CreateInputTextPart(content)]);
    }

    public static MessageResponseItem CreateAssistantMessageItem(string id, string content)
    {
        Argument.AssertNotNull(id, nameof(id));
        Argument.AssertNotNull(content, nameof(content));

        return new InternalResponsesAssistantMessage(
            type: InternalResponsesItemType.Message,
            id: id,
            additionalBinaryDataProperties: null,
            status: null,
            MessageRole.Assistant,
            internalContent: [ResponseContentPart.CreateInputTextPart(content)]);
    }

    public static FileSearchCallResponseItem CreateFileSearchCallResponseItem(string id, IEnumerable<string> queries, IEnumerable<FileSearchCallResult> results)
    {
        return new FileSearchCallResponseItem(id, queries, results);
    }

    public static ResponseItem CreateComputerCallItem(string id, string callId, ComputerCallAction action, IEnumerable<ComputerCallSafetyCheck> pendingSafetyChecks)
    {
        return new ComputerCallResponseItem(id, callId, action, pendingSafetyChecks);
    }

    [Experimental("OPENAICUA001")]
    public static ResponseItem CreateComputerCallOutputItem(string callId, IList<ComputerCallSafetyCheck> acknowledgedSafetyChecks, Uri screenshotImageUri)
    {
        return new ComputerCallOutputResponseItem(
            callId,
            acknowledgedSafetyChecks,
            ComputerOutput.CreateScreenshotOutput(screenshotImageUri));
    }

    [Experimental("OPENAICUA001")]
    public static ResponseItem CreateComputerCallOutputItem(string callId, IList<ComputerCallSafetyCheck> acknowledgedSafetyChecks, string screenshotImageFileId)
    {
        return new ComputerCallOutputResponseItem(
            callId,
            acknowledgedSafetyChecks,
            ComputerOutput.CreateScreenshotOutput(screenshotImageFileId));
    }

    [Experimental("OPENAICUA001")]
    public static ResponseItem CreateComputerCallOutputItem(string callId, IList<ComputerCallSafetyCheck> acknowledgedSafetyChecks, BinaryData screenshotImageBytes, string screenshotImageBytesMediaType)
    {
        return new ComputerCallOutputResponseItem(
            callId,
            acknowledgedSafetyChecks,
            ComputerOutput.CreateScreenshotOutput(screenshotImageBytes, screenshotImageBytesMediaType));
    }

    public static WebSearchCallResponseItem CreateWebSearchCallItem(string id)
    {
        return new WebSearchCallResponseItem(id);
    }

    public static FunctionCallResponseItem CreateFunctionCall(string id, string callId, string functionName, BinaryData functionArguments)
    {
        return new FunctionCallResponseItem(id, callId, functionName, functionArguments);
    }

    public static FunctionCallOutputResponseItem CreateFunctionCallOutputItem(string callId, string functionOutput)
    {
        return new FunctionCallOutputResponseItem(callId, functionOutput);
    }

    public static ReasoningResponseItem CreateReasoningItem(string id, IEnumerable<string> summaryTextParts)
    {
        return new ReasoningResponseItem(id, summaryTextParts);
    }

    public static ReferenceResponseItem CreateReferenceItem(string id)
    {
        return new ReferenceResponseItem(id);
    }
}
