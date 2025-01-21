using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

/// <summary>
/// </summary>
/// <remarks>
/// This type is a shared representation of the <c>response.function_call_arguments.done</c> and
/// <c>response.content_part.done</c> response commands.
/// </remarks>
public partial class ConversationItemStreamingPartFinishedUpdate : ConversationUpdate
{
    private readonly InternalRealtimeServerEventResponseContentPartDone _contentPartDone;
    private readonly InternalRealtimeServerEventResponseFunctionCallArgumentsDone _functionCallArgumentsDone;

    public string ResponseId
        => _contentPartDone?.ResponseId
        ?? _functionCallArgumentsDone?.ResponseId;

    public string ItemId
        => _contentPartDone?.ItemId
        ?? _functionCallArgumentsDone?.ItemId;

    public int ItemIndex
        => _contentPartDone?.OutputIndex
        ?? _functionCallArgumentsDone?.OutputIndex
        ?? 0;

    public int ContentPartIndex
        => _contentPartDone?.ContentIndex
        ?? 0;

    public string AudioTranscript => _contentPartDone?.AudioTranscript;

    public string Text => _contentPartDone?.Text;

    public string FunctionCallId => _functionCallArgumentsDone?.CallId;

    public string FunctionArguments => _functionCallArgumentsDone?.Arguments;


    internal ConversationItemStreamingPartFinishedUpdate(ConversationUpdate baseUpdate)
        : base(baseUpdate.EventId, baseUpdate.Kind, additionalBinaryDataProperties: null)
    {
        _contentPartDone = baseUpdate as InternalRealtimeServerEventResponseContentPartDone;
        _functionCallArgumentsDone = baseUpdate as InternalRealtimeServerEventResponseFunctionCallArgumentsDone;
    }
}
