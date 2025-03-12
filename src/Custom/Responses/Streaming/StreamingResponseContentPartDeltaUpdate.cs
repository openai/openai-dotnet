using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

/// <summary>
/// </summary>
/// <remarks>
/// This type is a shared representation of the following response event types:
/// <list type="bullet">
/// <item><c>response.content_part.added</c></item>
/// <item><c>response.output_text.delta</c></item>
/// <item><c>response.function_call_arguments.delta</c></item>
/// </list>
/// </remarks>
public partial class StreamingResponseContentPartDeltaUpdate : StreamingResponseUpdate
{
    public string ItemId
        => _contentPartAdded?.ItemId
        ?? _outputTextDelta?.ItemId
        ?? _functionArgumentsDelta?.ItemId;

    public int ItemIndex
        => _contentPartAdded?.OutputIndex
        ?? _outputTextDelta?.OutputIndex
        ?? _functionArgumentsDelta?.OutputIndex
        ?? 0;

    public int ContentPartIndex
        => _contentPartAdded?.ContentIndex
        ?? _outputTextDelta?.ContentIndex
        ?? 0;

    public string Text
        => _outputTextDelta?.Delta
        ?? _contentPartAdded?.Part?.Text;

    public string FunctionArguments
        => _functionArgumentsDelta?.Delta;

    public string Refusal
        => _refusalDelta?.Delta;

    private readonly InternalResponsesResponseStreamEventResponseContentPartAdded _contentPartAdded;
    private readonly InternalResponsesResponseStreamEventResponseOutputTextDelta _outputTextDelta;
    private readonly InternalResponsesResponseStreamEventResponseFunctionCallArgumentsDelta _functionArgumentsDelta;
    private readonly InternalResponsesResponseStreamEventResponseRefusalDelta _refusalDelta;

    internal StreamingResponseContentPartDeltaUpdate(StreamingResponseUpdate baseUpdate)
        : base(baseUpdate.Kind)
    {
        _contentPartAdded = baseUpdate as InternalResponsesResponseStreamEventResponseContentPartAdded;
        _outputTextDelta = baseUpdate as InternalResponsesResponseStreamEventResponseOutputTextDelta;
        _functionArgumentsDelta = baseUpdate as InternalResponsesResponseStreamEventResponseFunctionCallArgumentsDelta;
        _refusalDelta = baseUpdate as InternalResponsesResponseStreamEventResponseRefusalDelta;
    }

    internal StreamingResponseContentPartDeltaUpdate()
    {
    }
}
