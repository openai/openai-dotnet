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
    public string Id { get; }

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
        return new InternalResponsesSystemMessage(id: null, status: null, contentParts);
    }

    public static MessageResponseItem CreateSystemMessageItem(string inputTextContent)
    {
        Argument.AssertNotNull(inputTextContent, nameof(inputTextContent));
        return new InternalResponsesSystemMessage(
            internalContent: [ResponseContentPart.CreateInputTextPart(inputTextContent)]);
    }

    public static MessageResponseItem CreateAssistantMessageItem(
        IEnumerable<ResponseContentPart> contentParts)
    {
        Argument.AssertNotNull(contentParts, nameof(contentParts));
        return new InternalResponsesAssistantMessage(contentParts);
    }

    public static MessageResponseItem CreateAssistantMessageItem(
        string outputTextContent,
        IEnumerable<ResponseMessageAnnotation> annotations = null)
    {
        Argument.AssertNotNull(outputTextContent, nameof(outputTextContent));
        return new InternalResponsesAssistantMessage(
            internalContent:
            [
                new InternalItemContentOutputText(annotations ?? [], outputTextContent),
            ]);
    }

    [Experimental("OPENAICUA001")]
    public static ComputerCallResponseItem CreateComputerCallItem(string callId, ComputerCallAction action, IEnumerable<ComputerCallSafetyCheck> pendingSafetyChecks)
    {
        Argument.AssertNotNull(callId, nameof(callId));
        return new ComputerCallResponseItem(callId, action, pendingSafetyChecks);
    }

    [Experimental("OPENAICUA001")]
    public static ComputerCallOutputResponseItem CreateComputerCallOutputItem(string callId, IList<ComputerCallSafetyCheck> acknowledgedSafetyChecks, Uri screenshotImageUri)
    {
        Argument.AssertNotNull(callId, nameof(callId));
        ComputerCallOutputResponseItem item = new(
            callId,
            ComputerOutput.CreateScreenshotOutput(screenshotImageUri));
        foreach (ComputerCallSafetyCheck safetyCheck in acknowledgedSafetyChecks ?? [])
        {
            item.AcknowledgedSafetyChecks.Add(safetyCheck);
        }
        return item;
    }

    [Experimental("OPENAICUA001")]
    public static ComputerCallOutputResponseItem CreateComputerCallOutputItem(string callId, IList<ComputerCallSafetyCheck> acknowledgedSafetyChecks, string screenshotImageFileId)
    {
        Argument.AssertNotNull(callId, nameof(callId));
        ComputerCallOutputResponseItem item = new(
            callId,
            ComputerOutput.CreateScreenshotOutput(screenshotImageFileId));
        foreach (ComputerCallSafetyCheck safetyCheck in acknowledgedSafetyChecks ?? [])
        {
            item.AcknowledgedSafetyChecks.Add(safetyCheck);
        }
        return item;
    }

    [Experimental("OPENAICUA001")]
    public static ComputerCallOutputResponseItem CreateComputerCallOutputItem(string callId, IList<ComputerCallSafetyCheck> acknowledgedSafetyChecks, BinaryData screenshotImageBytes, string screenshotImageBytesMediaType)
    {
        Argument.AssertNotNull(callId, nameof(callId));
        ComputerCallOutputResponseItem item = new(
            callId,
            ComputerOutput.CreateScreenshotOutput(screenshotImageBytes, screenshotImageBytesMediaType));
        foreach (ComputerCallSafetyCheck safetyCheck in acknowledgedSafetyChecks ?? [])
        {
            item.AcknowledgedSafetyChecks.Add(safetyCheck);
        }
        return item;
    }

    public static WebSearchCallResponseItem CreateWebSearchCallItem()
    {
        return new WebSearchCallResponseItem();
    }

    public static FileSearchCallResponseItem CreateFileSearchCallItem(
        IEnumerable<string> queries,
        IEnumerable<FileSearchCallResult> results)
    {
        Argument.AssertNotNullOrEmpty(queries, nameof(queries));
        return new FileSearchCallResponseItem(queries, results);
    }

    public static FunctionCallResponseItem CreateFunctionCallItem(string callId, string functionName, BinaryData functionArguments)
    {
        Argument.AssertNotNull(callId, nameof(callId));
        Argument.AssertNotNull(functionName, nameof(functionName));
        Argument.AssertNotNull(functionArguments, nameof(functionArguments));

        return new FunctionCallResponseItem(callId, functionName, functionArguments);
    }

    public static FunctionCallOutputResponseItem CreateFunctionCallOutputItem(string callId, string functionOutput)
    {
        Argument.AssertNotNull(callId, nameof(callId));
        Argument.AssertNotNull(functionOutput, nameof(functionOutput));
        return new FunctionCallOutputResponseItem(callId, functionOutput);
    }

    public static ReasoningResponseItem CreateReasoningItem(IEnumerable<ReasoningSummaryPart> summaryParts)
    {
        Argument.AssertNotNullOrEmpty(summaryParts, nameof(summaryParts));
        return new ReasoningResponseItem(summaryParts);
    }

    public static ReasoningResponseItem CreateReasoningItem(string summaryText)
    {
        Argument.AssertNotNull(summaryText, nameof(summaryText));
        return new ReasoningResponseItem(summaryText);
    }

    public static ReferenceResponseItem CreateReferenceItem(string id)
    {
        return new ReferenceResponseItem(id);
    }
}
