using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIResponsesModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Responses.OpenAIResponse"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Responses.OpenAIResponse"/> instance for mocking. </returns>
    [Experimental("OPENAI001")]
    public static OpenAIResponse OpenAIResponse(
        string id = null,
        DateTimeOffset createdAt = default,
        ResponseStatus? status = null,
        ResponseError error = null,
        ResponseTokenUsage usage = null,
        string endUserId = null,
        ResponseReasoningOptions reasoningOptions = null,
        int? maxOutputTokenCount = null,
        ResponseTextOptions textOptions = null,
        ResponseTruncationMode? truncationMode = null,
        ResponseIncompleteStatusDetails incompleteStatusDetails = null,
        IEnumerable<ResponseItem> outputItems = null,
        bool parallelToolCallsEnabled = default,
        ResponseToolChoice toolChoice = null,
        string model = null,
        IDictionary<string, string> metadata = null,
        float? temperature = null,
        float? topP = null,
        string previousResponseId = null,
        bool? background = null,
        string instructions = null,
        IEnumerable<ResponseTool> tools = null)
    {
        outputItems ??= new List<ResponseItem>();
        tools ??= new List<ResponseTool>();
        metadata ??= new Dictionary<string, string>();

        return new OpenAIResponse(
            metadata: metadata,
            temperature: temperature,
            topP: topP,
            serviceTier: null,
            previousResponseId: previousResponseId,
            background: background,
            instructions: instructions,
            tools: tools.ToList(),
            id: id,
            status: status,
            createdAt: createdAt,
            error: error,
            usage: usage,
            endUserId: endUserId,
            reasoningOptions: reasoningOptions,
            maxOutputTokenCount: maxOutputTokenCount,
            textOptions: textOptions,
            truncationMode: truncationMode,
            incompleteStatusDetails: incompleteStatusDetails,
            outputItems: outputItems.ToList(),
            parallelToolCallsEnabled: parallelToolCallsEnabled,
            toolChoice: toolChoice,
            model: model,
            @object: "response",
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Responses.MessageResponseItem"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Responses.MessageResponseItem"/> instance for mocking. </returns>
    public static MessageResponseItem MessageResponseItem(
        string id = null,
        MessageRole role = MessageRole.Assistant,
        MessageStatus? status = null)
    {
        // Convert the public MessageRole to the internal role type
        InternalResponsesMessageRole internalRole = role.ToSerialString();
        
        return new MessageResponseItem(
            id: id,
            internalRole: internalRole,
            status: status);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Responses.ReasoningResponseItem"/>. </summary>
    /// <param name="id">The ID of the reasoning response item.</param>
    /// <param name="encryptedContent">The encrypted reasoning content.</param>
    /// <param name="status">The status of the reasoning response item.</param>
    /// <param name="summaryParts">The collection of summary parts.</param>
    /// <returns> A new <see cref="OpenAI.Responses.ReasoningResponseItem"/> instance for mocking. </returns>
    public static ReasoningResponseItem ReasoningResponseItem(
        string id = null,
        string encryptedContent = null,
        ReasoningStatus? status = null,
        IEnumerable<ReasoningSummaryPart> summaryParts = null)
    {
        summaryParts ??= new List<ReasoningSummaryPart>();
        
        var item = new ReasoningResponseItem(
            kind: InternalItemType.Reasoning,
            id: id,
            additionalBinaryDataProperties: null,
            encryptedContent: encryptedContent,
            summaryParts: summaryParts.ToList());
        
        item.Status = status;
        return item;
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Responses.ReasoningResponseItem"/> with summary text. </summary>
    /// <param name="id">The ID of the reasoning response item.</param>
    /// <param name="encryptedContent">The encrypted reasoning content.</param>
    /// <param name="status">The status of the reasoning response item.</param>
    /// <param name="summaryText">The summary text to create a ReasoningSummaryTextPart from.</param>
    /// <returns> A new <see cref="OpenAI.Responses.ReasoningResponseItem"/> instance for mocking. </returns>
    public static ReasoningResponseItem ReasoningResponseItem(
        string id = null,
        string encryptedContent = null,
        ReasoningStatus? status = null,
        string summaryText = null)
    {
        var summaryParts = !string.IsNullOrEmpty(summaryText) 
            ? new List<ReasoningSummaryPart> { new ReasoningSummaryTextPart(summaryText) }
            : new List<ReasoningSummaryPart>();
        
        var item = new ReasoningResponseItem(
            kind: InternalItemType.Reasoning,
            id: id,
            additionalBinaryDataProperties: null,
            encryptedContent: encryptedContent,
            summaryParts: summaryParts);
        
        item.Status = status;
        return item;
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Responses.ReferenceResponseItem"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Responses.ReferenceResponseItem"/> instance for mocking. </returns>
    public static ReferenceResponseItem ReferenceResponseItem(
        string id = null)
    {
        return new ReferenceResponseItem(
            kind: InternalItemType.ItemReference,
            id: id,
            additionalBinaryDataProperties: null);
    }
}