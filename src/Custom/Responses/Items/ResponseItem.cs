using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OpenAI.Responses;

[CodeGenType("ResponsesItem")]
public partial class ResponseItem
{
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
                new InternalResponsesOutputTextContentPart(annotations ?? [], outputTextContent),
            ]);
    }

    [Experimental("OPENAICUA001")]
    public static ResponseItem CreateComputerCallItem(string callId, ComputerCallAction action, IEnumerable<ComputerCallSafetyCheck> pendingSafetyChecks)
    {
        return new ComputerCallResponseItem(callId, action, pendingSafetyChecks);
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

    public static WebSearchCallResponseItem CreateWebSearchCallItem()
    {
        return new WebSearchCallResponseItem();
    }

    public static FileSearchCallResponseItem CreateFileSearchCallItem(
        IEnumerable<string> queries,
        IEnumerable<FileSearchCallResult> results)
    {
        return new FileSearchCallResponseItem(queries, results);
    }

    public static FunctionCallResponseItem CreateFunctionCallItem(string callId, string functionName, BinaryData functionArguments)
    {
        return new FunctionCallResponseItem(callId, functionName, functionArguments);
    }

    public static FunctionCallOutputResponseItem CreateFunctionCallOutputItem(string callId, string functionOutput)
    {
        return new FunctionCallOutputResponseItem(callId, functionOutput);
    }

    public static ReasoningResponseItem CreateReasoningItem(IEnumerable<string> summaryTextParts)
    {
        return new ReasoningResponseItem(summaryTextParts);
    }

    public static ReferenceResponseItem CreateReferenceItem(string id)
    {
        return new ReferenceResponseItem(id);
    }
}
