using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

public partial class RealtimeUpdate
{
    internal static RealtimeUpdate DeserializeRealtimeUpdate(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        if (element.TryGetProperty("type", out JsonElement discriminator))
        {
            return discriminator.GetString() switch
            {
                "conversation.created" => InternalRealtimeServerEventConversationCreated.DeserializeInternalRealtimeServerEventConversationCreated(element, options),
                "conversation.item.created" => ItemCreatedUpdate.DeserializeItemCreatedUpdate(element, options),
                "conversation.item.deleted" => ItemDeletedUpdate.DeserializeItemDeletedUpdate(element, options),
                "conversation.item.input_audio_transcription.completed" => InputAudioTranscriptionFinishedUpdate.DeserializeInputAudioTranscriptionFinishedUpdate(element, options),
                "conversation.item.input_audio_transcription.delta" => InputAudioTranscriptionDeltaUpdate.DeserializeInputAudioTranscriptionDeltaUpdate(element, options),
                "conversation.item.input_audio_transcription.failed" => InputAudioTranscriptionFailedUpdate.DeserializeInputAudioTranscriptionFailedUpdate(element, options),
                "conversation.item.truncated" => ItemTruncatedUpdate.DeserializeItemTruncatedUpdate(element, options),
                "error" => RealtimeErrorUpdate.DeserializeRealtimeErrorUpdate(element, options),
                "input_audio_buffer.cleared" => InputAudioClearedUpdate.DeserializeInputAudioClearedUpdate(element, options),
                "input_audio_buffer.committed" => InputAudioCommittedUpdate.DeserializeInputAudioCommittedUpdate(element, options),
                "input_audio_buffer.speech_started" => InputAudioSpeechStartedUpdate.DeserializeInputAudioSpeechStartedUpdate(element, options),
                "input_audio_buffer.speech_stopped" => InputAudioSpeechFinishedUpdate.DeserializeInputAudioSpeechFinishedUpdate(element, options),
                "rate_limits.updated" => RateLimitsUpdate.DeserializeRateLimitsUpdate(element, options),
                "response.created" => ResponseStartedUpdate.DeserializeResponseStartedUpdate(element, options),
                "response.done" => ResponseFinishedUpdate.DeserializeResponseFinishedUpdate(element, options),
                "session.created" => ConversationSessionStartedUpdate.DeserializeConversationSessionStartedUpdate(element, options),
                "session.updated" => ConversationSessionConfiguredUpdate.DeserializeConversationSessionConfiguredUpdate(element, options),
                "transcription_session.updated" => TranscriptionSessionConfiguredUpdate.DeserializeTranscriptionSessionConfiguredUpdate(element, options),

                "response.output_item.added" => OutputStreamingStartedUpdate.DeserializeOutputStreamingStartedUpdate(element, options),
                "response.output_item.done" => OutputStreamingFinishedUpdate.DeserializeOutputStreamingFinishedUpdate(element, options),

                "response.content_part.added"
                    or "response.audio_transcript.delta"
                    or "response.audio.delta"
                    or "response.text.delta"
                    or "response.function_call_arguments.delta" => OutputDeltaUpdate.DeserializeOutputDeltaUpdate(element, options),

                "response.text.done" => OutputTextFinishedUpdate.DeserializeOutputTextFinishedUpdate(element, options),
                "response.audio_transcript.done" => OutputAudioTranscriptionFinishedUpdate.DeserializeOutputAudioTranscriptionFinishedUpdate(element, options),
                "response.audio.done" => OutputAudioFinishedUpdate.DeserializeOutputAudioFinishedUpdate(element, options),

                "response.function_call_arguments.done"
                    or "response.content_part.done" => OutputPartFinishedUpdate.DeserializeOutputPartFinishedUpdate(element, options),

                _ => UnknownRealtimeServerEvent.DeserializeUnknownRealtimeServerEvent(element, options),

            };

        }
        return UnknownRealtimeServerEvent.DeserializeUnknownRealtimeServerEvent(element, options);
    }
}