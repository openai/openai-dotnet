using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventType")]
public enum ConversationUpdateKind
{
    /// <summary>
    /// A response command that does not map to a <c>type</c> currently handled by the library.
    /// </summary>
    Unknown,
    /// <summary>
    /// The <c>session.created</c> response command.
    /// </summary>
    [CodeGenMember("SessionCreated")]
    SessionStarted,
    /// <summary>
    /// The <c>session.updated</c> response command.
    /// </summary>
    [CodeGenMember("SessionUpdated")]
    SessionConfigured,
    /// <summary>
    /// The <c>conversation.item.created</c> response command.
    /// </summary>
    [CodeGenMember("ConversationItemCreated")]
    ItemCreated,
    /// <summary>
    /// The <c>conversation.c.reated</c> response command.
    /// </summary>
    /// <remarks>
    /// This update kind is currently unused.
    /// </remarks>
    ConversationCreated,
    /// <summary>
    /// The <c>conversation.item.deleted</c> response command.
    /// </summary>
    [CodeGenMember("ConversationItemDeleted")]
    ItemDeleted,
    /// <summary>
    /// The <c>conversation.item.truncated</c> response command.
    /// </summary>
    [CodeGenMember("ConversationItemTruncated")]
    ItemTruncated,
    /// <summary>
    /// The <c>response.created</c> response command.
    /// </summary>
    [CodeGenMember("ResponseCreated")]
    ResponseStarted,
    /// <summary>
    /// The <c>response.done</c> response command.
    /// </summary>
    [CodeGenMember("ResponseDone")]
    ResponseFinished,
    /// <summary>
    /// The <c>rate_limits.updated</c> response command.
    /// </summary>
    [CodeGenMember("RateLimitsUpdated")]
    RateLimitsUpdated,
    /// <summary>
    /// The <c>response.output_item.added</c> response command.
    /// </summary>
    [CodeGenMember("ResponseOutputItemAdded")]
    ItemStreamingStarted,
    /// <summary>
    /// The <c>response.output_item.done</c> response command.
    /// </summary>
    [CodeGenMember("ResponseOutputItemDone")]
    ItemStreamingFinished,
    /// <summary>
    /// The <c>response.content_part.added</c> response command.
    /// </summary>
    [CodeGenMember("ResponseContentPartAdded")]
    ItemContentPartStarted,
    /// <summary>
    /// The <c>response.content_part.done</c> response command.
    /// </summary>
    [CodeGenMember("ResponseContentPartDone")]
    ItemContentPartFinished,
    /// <summary>
    /// The <c>response.audio.delta</c> response command.
    /// </summary>
    [CodeGenMember("ResponseAudioDelta")]
    ItemStreamingPartAudioDelta,
    /// <summary>
    /// The <c>response.audio.done</c> response command.
    /// </summary>
    [CodeGenMember("ResponseAudioDone")]
    ItemStreamingPartAudioFinished,
    /// <summary>
    /// The <c>response.audio_transcript.delta</c> response command.
    /// </summary>
    [CodeGenMember("ResponseAudioTranscriptDelta")]
    ItemStreamingPartAudioTranscriptionDelta,
    /// <summary>
    /// The <c>response.audio_transcript.done</c> response command.
    /// </summary>
    [CodeGenMember("ResponseAudioTranscriptDone")]
    ItemStreamingPartAudioTranscriptionFinished,
    /// <summary>
    /// The <c>response.text.delta</c> response command.
    /// </summary>
    [CodeGenMember("ResponseTextDelta")]
    ItemStreamingPartTextDelta,
    /// <summary>
    /// The <c>response.text.done</c> response command.
    /// </summary>
    [CodeGenMember("ResponseTextDone")]
    ItemStreamingPartTextFinished,
    /// <summary>
    /// The <c>response.function_call_arguments.delta</c> response command.
    /// </summary>
    [CodeGenMember("ResponseFunctionCallArgumentsDelta")]
    ItemStreamingFunctionCallArgumentsDelta,
    /// <summary>
    /// The <c>response.function_call_arguments.done</c> response command.
    /// </summary>
    [CodeGenMember("ResponseFunctionCallArgumentsDone")]
    ItemStreamingFunctionCallArgumentsFinished,
    /// <summary>
    /// The <c>input_audio_buffer.speech_started</c> response command.
    /// </summary>
    [CodeGenMember("InputAudioBufferSpeechStarted")]
    InputSpeechStarted,
    /// <summary>
    /// The <c>input_audio_buffer.speech_stopped</c> response command.
    /// </summary>
    [CodeGenMember("InputAudioBufferSpeechStopped")]
    InputSpeechStopped,
    /// <summary>
    /// The <c>conversation.item.input_audio_transcription.completed</c> response command.
    /// </summary>
    [CodeGenMember("ConversationItemInputAudioTranscriptionCompleted")]
    InputTranscriptionFinished,
    /// <summary>
    /// The <c>conversation.item.input_audio_transcription.failed</c> response command.
    /// </summary>
    [CodeGenMember("ConversationItemInputAudioTranscriptionFailed")]
    InputTranscriptionFailed,
    /// <summary>
    /// The <c>input_audio_buffer.committed</c> response command.
    /// </summary>
    [CodeGenMember("InputAudioBufferCommitted")]
    InputAudioCommitted,
    /// <summary>
    /// The <c>input_audio_buffer.cleared</c> response command.
    /// </summary>
    [CodeGenMember("InputAudioBufferCleared")]
    InputAudioCleared,
    /// <summary>
    /// The <c>error</c> response command.
    /// </summary>
    [CodeGenMember("Error")]
    Error
}