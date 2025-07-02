using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// </summary>
/// <remarks>
/// This type is a shared representation of the following response command types:
/// <list type="bullet">
/// <item><c>response.content_part.added</c></item>
/// <item><c>response.audio.delta</c></item>
/// <item><c>response.text.delta</c></item>
/// <item><c>response.audio_transcript.delta</c></item>
/// <item><c>response.function_call_arguments.delta</c></item>
/// </list>
/// </remarks>
[Experimental("OPENAI002")]
public partial class OutputDeltaUpdate : RealtimeUpdate
{
    private readonly InternalRealtimeServerEventResponseContentPartAdded _contentPartAdded;
    private readonly InternalRealtimeServerEventResponseAudioDelta _audioDelta;
    private readonly InternalRealtimeServerEventResponseAudioTranscriptDelta _outputTranscriptionDelta;
    private readonly InternalRealtimeServerEventResponseTextDelta _textDelta;
    private readonly InternalRealtimeServerEventResponseFunctionCallArgumentsDelta _functionArgumentsDelta;

    public string ResponseId
        => _contentPartAdded?.ResponseId
        ?? _audioDelta?.ResponseId
        ?? _outputTranscriptionDelta?.ResponseId
        ?? _textDelta?.ResponseId
        ?? _functionArgumentsDelta?.ResponseId;

    public string ItemId
        => _contentPartAdded?.ItemId
        ?? _audioDelta?.ItemId
        ?? _outputTranscriptionDelta?.ItemId
        ?? _textDelta?.ItemId
        ?? _functionArgumentsDelta?.ItemId;

    public int ItemIndex
        => _contentPartAdded?.OutputIndex
        ?? _audioDelta?.OutputIndex
        ?? _outputTranscriptionDelta?.OutputIndex
        ?? _textDelta?.OutputIndex
        ?? _functionArgumentsDelta?.OutputIndex
        ?? 0;

    public int ContentPartIndex
        => _contentPartAdded?.ContentIndex
        ?? _audioDelta?.ContentIndex
        ?? _outputTranscriptionDelta?.ContentIndex
        ?? _textDelta?.ContentIndex
        ?? 0;

    public BinaryData AudioBytes
        => _audioDelta?.Delta;

    public string AudioTranscript
        => _contentPartAdded?.AudioTranscript
        ?? _outputTranscriptionDelta?.Delta;

    public string Text
        => _contentPartAdded?.Text
        ?? _textDelta?.Delta;

    public string FunctionCallId
        => _functionArgumentsDelta?.CallId;

    public string FunctionArguments
        => _functionArgumentsDelta?.Delta;

    internal OutputDeltaUpdate() : base(RealtimeUpdateKind.Unknown) { }

    internal OutputDeltaUpdate(RealtimeUpdate baseUpdate)
        : base(baseUpdate.Kind, baseUpdate.EventId, additionalBinaryDataProperties: null)
    {
        _contentPartAdded = baseUpdate as InternalRealtimeServerEventResponseContentPartAdded;
        _audioDelta = baseUpdate as InternalRealtimeServerEventResponseAudioDelta;
        _outputTranscriptionDelta = baseUpdate as InternalRealtimeServerEventResponseAudioTranscriptDelta;
        _textDelta = baseUpdate as InternalRealtimeServerEventResponseTextDelta;
        _functionArgumentsDelta = baseUpdate as InternalRealtimeServerEventResponseFunctionCallArgumentsDelta;
    }
}
