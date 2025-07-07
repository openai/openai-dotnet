using System;

namespace OpenAI.Realtime;

internal static partial class RealtimeUpdateKindExtensions
{
    public static string ToSerialString(this RealtimeUpdateKind value) => value switch
    {
        RealtimeUpdateKind.SessionStarted => "session.created",
        RealtimeUpdateKind.SessionConfigured => "session.updated",
        RealtimeUpdateKind.ConversationCreated => "conversation.created",
        RealtimeUpdateKind.ItemCreated => "conversation.item.created",
        RealtimeUpdateKind.ItemDeleted => "conversation.item.deleted",
        RealtimeUpdateKind.ItemTruncated => "conversation.item.truncated",
        RealtimeUpdateKind.ResponseStarted => "response.created",
        RealtimeUpdateKind.ResponseFinished => "response.done",
        RealtimeUpdateKind.RateLimitsUpdated => "rate_limits.updated",
        RealtimeUpdateKind.ItemStreamingStarted => "response.output_item.added",
        RealtimeUpdateKind.ItemStreamingFinished => "response.output_item.done",
        RealtimeUpdateKind.ItemContentPartStarted => "response.content_part.added",
        RealtimeUpdateKind.ItemContentPartFinished => "response.content_part.done",
        RealtimeUpdateKind.ItemStreamingPartAudioDelta => "response.audio.delta",
        RealtimeUpdateKind.ItemStreamingPartAudioFinished => "response.audio.done",
        RealtimeUpdateKind.ItemStreamingPartAudioTranscriptionDelta => "response.audio_transcript.delta",
        RealtimeUpdateKind.ItemStreamingPartAudioTranscriptionFinished => "response.audio_transcript.done",
        RealtimeUpdateKind.ItemStreamingPartTextDelta => "response.text.delta",
        RealtimeUpdateKind.ItemStreamingPartTextFinished => "response.text.done",
        RealtimeUpdateKind.ItemStreamingFunctionCallArgumentsDelta => "response.function_call_arguments.delta",
        RealtimeUpdateKind.ItemStreamingFunctionCallArgumentsFinished => "response.function_call_arguments.done",
        RealtimeUpdateKind.InputSpeechStarted => "input_audio_buffer.speech_started",
        RealtimeUpdateKind.InputSpeechStopped => "input_audio_buffer.speech_stopped",
        RealtimeUpdateKind.InputTranscriptionFinished => "conversation.item.input_audio_transcription.completed",
        RealtimeUpdateKind.InputTranscriptionDelta => "conversation.item.input_audio_transcription.delta",
        RealtimeUpdateKind.InputTranscriptionFailed => "conversation.item.input_audio_transcription.failed",
        RealtimeUpdateKind.InputAudioCommitted => "input_audio_buffer.committed",
        RealtimeUpdateKind.InputAudioCleared => "input_audio_buffer.cleared",
        RealtimeUpdateKind.TranscriptionSessionStarted => "transcription_session.created",
        RealtimeUpdateKind.TranscriptionSessionConfigured => "transcription_session.updated",
        RealtimeUpdateKind.Error => "error",
        _ => null,
    };

    public static RealtimeUpdateKind ToRealtimeUpdateKind(this string value)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "session.created")) return RealtimeUpdateKind.SessionStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "session.updated")) return RealtimeUpdateKind.SessionConfigured;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.created")) return RealtimeUpdateKind.ConversationCreated;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.created")) return RealtimeUpdateKind.ItemCreated;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.deleted")) return RealtimeUpdateKind.ItemDeleted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.truncated")) return RealtimeUpdateKind.ItemTruncated;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.created")) return RealtimeUpdateKind.ResponseStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.done")) return RealtimeUpdateKind.ResponseFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "rate_limits.updated")) return RealtimeUpdateKind.RateLimitsUpdated;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.output_item.added")) return RealtimeUpdateKind.ItemStreamingStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.output_item.done")) return RealtimeUpdateKind.ItemStreamingFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.content_part.added")) return RealtimeUpdateKind.ItemContentPartStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.content_part.done")) return RealtimeUpdateKind.ItemContentPartFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.audio.delta")) return RealtimeUpdateKind.ItemStreamingPartAudioDelta;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.audio.done")) return RealtimeUpdateKind.ItemStreamingPartAudioFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.audio_transcript.delta")) return RealtimeUpdateKind.ItemStreamingPartAudioTranscriptionDelta;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.audio_transcript.done")) return RealtimeUpdateKind.ItemStreamingPartAudioTranscriptionFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.text.delta")) return RealtimeUpdateKind.ItemStreamingPartTextDelta;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.text.done")) return RealtimeUpdateKind.ItemStreamingPartTextFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.function_call_arguments.delta")) return RealtimeUpdateKind.ItemStreamingFunctionCallArgumentsDelta;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.function_call_arguments.done")) return RealtimeUpdateKind.ItemStreamingFunctionCallArgumentsFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "input_audio_buffer.speech_started")) return RealtimeUpdateKind.InputSpeechStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "input_audio_buffer.speech_stopped")) return RealtimeUpdateKind.InputSpeechStopped;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.input_audio_transcription.completed")) return RealtimeUpdateKind.InputTranscriptionFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.input_audio_transcription.delta")) return RealtimeUpdateKind.InputTranscriptionDelta;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.input_audio_transcription.failed")) return RealtimeUpdateKind.InputTranscriptionFailed;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "input_audio_buffer.committed")) return RealtimeUpdateKind.InputAudioCommitted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "input_audio_buffer.cleared")) return RealtimeUpdateKind.InputAudioCleared;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "transcription_session.created")) return RealtimeUpdateKind.TranscriptionSessionStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "transcription_session.updated")) return RealtimeUpdateKind.TranscriptionSessionConfigured;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "error")) return RealtimeUpdateKind.Error;
        return RealtimeUpdateKind.Unknown;
    }
}
