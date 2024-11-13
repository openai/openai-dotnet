using System;

namespace OpenAI.RealtimeConversation;
internal static partial class ConversationUpdateKindExtensions
{
    public static string ToSerialString(this ConversationUpdateKind value) => value switch
    {
        ConversationUpdateKind.SessionStarted => "session.created",
        ConversationUpdateKind.SessionConfigured => "session.updated",
        // ConversationUpdateKind.ConversationCreated => "conversation.created",
        ConversationUpdateKind.ItemCreated => "conversation.item.created",
        ConversationUpdateKind.ItemDeleted => "conversation.item.deleted",
        ConversationUpdateKind.ItemTruncated => "conversation.item.truncated",
        ConversationUpdateKind.ResponseStarted => "response.created",
        ConversationUpdateKind.ResponseFinished => "response.done",
        ConversationUpdateKind.RateLimitsUpdated => "rate_limits.updated",
        ConversationUpdateKind.ItemStreamingStarted => "response.output_item.added",
        ConversationUpdateKind.ItemStreamingFinished => "response.output_item.done",
        ConversationUpdateKind.ItemContentPartStarted => "response.content_part.added",
        ConversationUpdateKind.ItemContentPartFinished => "response.content_part.done",
        ConversationUpdateKind.ItemStreamingPartAudioDelta => "response.audio.delta",
        ConversationUpdateKind.ItemStreamingPartAudioFinished => "response.audio.done",
        ConversationUpdateKind.ItemStreamingPartAudioTranscriptionDelta => "response.audio_transcript.delta",
        ConversationUpdateKind.ItemStreamingPartAudioTranscriptionFinished => "response.audio_transcript.done",
        ConversationUpdateKind.ItemStreamingPartTextDelta => "response.text.delta",
        ConversationUpdateKind.ItemStreamingPartTextFinished => "response.text.done",
        ConversationUpdateKind.ItemStreamingFunctionCallArgumentsDelta => "response.function_call_arguments.delta",
        ConversationUpdateKind.ItemStreamingFunctionCallArgumentsFinished => "response.function_call_arguments.done",
        ConversationUpdateKind.InputSpeechStarted => "input_audio_buffer.speech_started",
        ConversationUpdateKind.InputSpeechStopped => "input_audio_buffer.speech_stopped",
        ConversationUpdateKind.InputTranscriptionFinished => "conversation.item.input_audio_transcription.completed",
        ConversationUpdateKind.InputTranscriptionFailed => "conversation.item.input_audio_transcription.failed",
        ConversationUpdateKind.InputAudioCommitted => "input_audio_buffer.committed",
        ConversationUpdateKind.InputAudioCleared => "input_audio_buffer.cleared",
        ConversationUpdateKind.Error => "error",
        _ => null,
    };

    public static ConversationUpdateKind ToConversationUpdateKind(this string value)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "session.created")) return ConversationUpdateKind.SessionStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "session.updated")) return ConversationUpdateKind.SessionConfigured;
        // if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.created")) return ConversationUpdateKind.ConversationCreated;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.created")) return ConversationUpdateKind.ItemCreated;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.deleted")) return ConversationUpdateKind.ItemDeleted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.truncated")) return ConversationUpdateKind.ItemTruncated;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.created")) return ConversationUpdateKind.ResponseStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.done")) return ConversationUpdateKind.ResponseFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "rate_limits.updated")) return ConversationUpdateKind.RateLimitsUpdated;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.output_item.added")) return ConversationUpdateKind.ItemStreamingStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.output_item.done")) return ConversationUpdateKind.ItemStreamingFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.content_part.added")) return ConversationUpdateKind.ItemContentPartStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.content_part.done")) return ConversationUpdateKind.ItemContentPartFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.audio.delta")) return ConversationUpdateKind.ItemStreamingPartAudioDelta;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.audio.done")) return ConversationUpdateKind.ItemStreamingPartAudioFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.audio_transcript.delta")) return ConversationUpdateKind.ItemStreamingPartAudioTranscriptionDelta;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.audio_transcript.done")) return ConversationUpdateKind.ItemStreamingPartAudioTranscriptionFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.text.delta")) return ConversationUpdateKind.ItemStreamingPartTextDelta;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.text.done")) return ConversationUpdateKind.ItemStreamingPartTextFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.function_call_arguments.delta")) return ConversationUpdateKind.ItemStreamingFunctionCallArgumentsDelta;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "response.function_call_arguments.done")) return ConversationUpdateKind.ItemStreamingFunctionCallArgumentsFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "input_audio_buffer.speech_started")) return ConversationUpdateKind.InputSpeechStarted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "input_audio_buffer.speech_stopped")) return ConversationUpdateKind.InputSpeechStopped;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.input_audio_transcription.completed")) return ConversationUpdateKind.InputTranscriptionFinished;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "conversation.item.input_audio_transcription.failed")) return ConversationUpdateKind.InputTranscriptionFailed;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "input_audio_buffer.committed")) return ConversationUpdateKind.InputAudioCommitted;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "input_audio_buffer.cleared")) return ConversationUpdateKind.InputAudioCleared;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "error")) return ConversationUpdateKind.Error;
        return ConversationUpdateKind.Unknown;
    }
}
