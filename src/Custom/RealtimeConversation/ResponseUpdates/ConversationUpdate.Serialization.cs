using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.RealtimeConversation;

public partial class ConversationUpdate
{
    internal static ConversationUpdate DeserializeConversationUpdate(JsonElement element, ModelReaderWriterOptions options = null)
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
                "conversation.item.created" => ConversationItemCreatedUpdate.DeserializeConversationItemCreatedUpdate(element, options),
                "conversation.item.deleted" => ConversationItemDeletedUpdate.DeserializeConversationItemDeletedUpdate(element, options),
                "conversation.item.input_audio_transcription.completed" => ConversationInputTranscriptionFinishedUpdate.DeserializeConversationInputTranscriptionFinishedUpdate(element, options),
                "conversation.item.input_audio_transcription.failed" => ConversationInputTranscriptionFailedUpdate.DeserializeConversationInputTranscriptionFailedUpdate(element, options),
                "conversation.item.truncated" => ConversationItemTruncatedUpdate.DeserializeConversationItemTruncatedUpdate(element, options),
                "error" => ConversationErrorUpdate.DeserializeConversationErrorUpdate(element, options),
                "input_audio_buffer.cleared" => ConversationInputAudioClearedUpdate.DeserializeConversationInputAudioClearedUpdate(element, options),
                "input_audio_buffer.committed" => ConversationInputAudioCommittedUpdate.DeserializeConversationInputAudioCommittedUpdate(element, options),
                "input_audio_buffer.speech_started" => ConversationInputSpeechStartedUpdate.DeserializeConversationInputSpeechStartedUpdate(element, options),
                "input_audio_buffer.speech_stopped" => ConversationInputSpeechFinishedUpdate.DeserializeConversationInputSpeechFinishedUpdate(element, options),
                "rate_limits.updated" => ConversationRateLimitsUpdate.DeserializeConversationRateLimitsUpdate(element, options),
                "response.created" => ConversationResponseStartedUpdate.DeserializeConversationResponseStartedUpdate(element, options),
                "response.done" => ConversationResponseFinishedUpdate.DeserializeConversationResponseFinishedUpdate(element, options),
                "session.created" => ConversationSessionStartedUpdate.DeserializeConversationSessionStartedUpdate(element, options),
                "session.updated" => ConversationSessionConfiguredUpdate.DeserializeConversationSessionConfiguredUpdate(element, options),

                "response.output_item.added" => ConversationItemStreamingStartedUpdate.DeserializeConversationItemStreamingStartedUpdate(element, options),
                "response.output_item.done" => ConversationItemStreamingFinishedUpdate.DeserializeConversationItemStreamingFinishedUpdate(element, options),

                "response.content_part.added"
                    or "response.audio_transcript.delta"
                    or "response.audio.delta"
                    or "response.text.delta"
                    or "response.function_call_arguments.delta" => ConversationItemStreamingPartDeltaUpdate.DeserializeItemStreamingPartDeltaUpdate(element, options),

                "response.text.done" => ConversationItemStreamingTextFinishedUpdate.DeserializeConversationItemStreamingTextFinishedUpdate(element, options),
                "response.audio_transcript.done" => ConversationItemStreamingAudioTranscriptionFinishedUpdate.DeserializeConversationItemStreamingAudioTranscriptionFinishedUpdate(element, options),
                "response.audio.done" => ConversationItemStreamingAudioFinishedUpdate.DeserializeConversationItemStreamingAudioFinishedUpdate(element, options),

                "response.function_call_arguments.done"
                    or "response.content_part.done" => ConversationItemStreamingPartFinishedUpdate.DeserializeItemStreamingPartFinishedUpdate(element, options),

                _ => UnknownRealtimeServerEvent.DeserializeUnknownRealtimeServerEvent(element, options),

            };
        }
        return UnknownRealtimeServerEvent.DeserializeUnknownRealtimeServerEvent(element, options);
    }
}