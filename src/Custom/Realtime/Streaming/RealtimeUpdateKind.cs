using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeServerEventType")]
public enum RealtimeUpdateKind
{
    /// <summary>
    /// A server event that does not map to a <c>type</c> currently handled by the library.
    /// </summary>
    Unknown,
    /// <summary>
    /// The <c>session.created</c> server event.
    /// </summary>
    [CodeGenMember("SessionCreated")]
    SessionStarted,
    /// <summary>
    /// The <c>session.updated</c> server event.
    /// </summary>
    [CodeGenMember("SessionUpdated")]
    SessionConfigured,
    /// <summary>
    /// The <c>conversation.item.created</c> server event.
    /// </summary>
    [CodeGenMember("ConversationItemCreated")]
    ItemCreated,
    /// <summary>
    /// The <c>conversation.created</c> server event.
    /// </summary>
    [CodeGenMember("ConversationCreated")]
    ConversationCreated,
    /// <summary>
    /// The <c>conversation.item.retrieved</c> server event.
    /// </summary>
    [CodeGenMember("ConversationItemRetrieved")]
    ItemRetrieved,
    /// <summary>
    /// The <c>conversation.item.deleted</c> server event.
    /// </summary>
    [CodeGenMember("ConversationItemDeleted")]
    ItemDeleted,
    /// <summary>
    /// The <c>conversation.item.truncated</c> server event.
    /// </summary>
    [CodeGenMember("ConversationItemTruncated")]
    ItemTruncated,
    /// <summary>
    /// The <c>response.created</c> server event.
    /// </summary>
    [CodeGenMember("ResponseCreated")]
    ResponseStarted,
    /// <summary>
    /// The <c>response.done</c> server event.
    /// </summary>
    [CodeGenMember("ResponseDone")]
    ResponseFinished,
    /// <summary>
    /// The <c>rate_limits.updated</c> server event.
    /// </summary>
    [CodeGenMember("RateLimitsUpdated")]
    RateLimitsUpdated,
    /// <summary>
    /// The <c>response.output_item.added</c> server event.
    /// </summary>
    [CodeGenMember("ResponseOutputItemAdded")]
    ItemStreamingStarted,
    /// <summary>
    /// The <c>response.output_item.done</c> server event.
    /// </summary>
    [CodeGenMember("ResponseOutputItemDone")]
    ItemStreamingFinished,
    /// <summary>
    /// The <c>response.content_part.added</c> server event.
    /// </summary>
    [CodeGenMember("ResponseContentPartAdded")]
    ItemContentPartStarted,
    /// <summary>
    /// The <c>response.content_part.done</c> server event.
    /// </summary>
    [CodeGenMember("ResponseContentPartDone")]
    ItemContentPartFinished,
    /// <summary>
    /// The <c>response.audio.delta</c> server event.
    /// </summary>
    [CodeGenMember("ResponseAudioDelta")]
    ItemStreamingPartAudioDelta,
    /// <summary>
    /// The <c>response.audio.done</c> server event.
    /// </summary>
    [CodeGenMember("ResponseAudioDone")]
    ItemStreamingPartAudioFinished,
    /// <summary>
    /// The <c>response.audio_transcript.delta</c> server event.
    /// </summary>
    [CodeGenMember("ResponseAudioTranscriptDelta")]
    ItemStreamingPartAudioTranscriptionDelta,
    /// <summary>
    /// The <c>response.audio_transcript.done</c> server event.
    /// </summary>
    [CodeGenMember("ResponseAudioTranscriptDone")]
    ItemStreamingPartAudioTranscriptionFinished,
    /// <summary>
    /// The <c>response.text.delta</c> server event.
    /// </summary>
    [CodeGenMember("ResponseTextDelta")]
    ItemStreamingPartTextDelta,
    /// <summary>
    /// The <c>response.text.done</c> server event.
    /// </summary>
    [CodeGenMember("ResponseTextDone")]
    ItemStreamingPartTextFinished,
    /// <summary>
    /// The <c>response.function_call_arguments.delta</c> server event.
    /// </summary>
    [CodeGenMember("ResponseFunctionCallArgumentsDelta")]
    ItemStreamingFunctionCallArgumentsDelta,
    /// <summary>
    /// The <c>response.function_call_arguments.done</c> server event.
    /// </summary>
    [CodeGenMember("ResponseFunctionCallArgumentsDone")]
    ItemStreamingFunctionCallArgumentsFinished,
    /// <summary>
    /// The <c>input_audio_buffer.speech_started</c> server event.
    /// </summary>
    [CodeGenMember("InputAudioBufferSpeechStarted")]
    InputSpeechStarted,
    /// <summary>
    /// The <c>input_audio_buffer.speech_stopped</c> server event.
    /// </summary>
    [CodeGenMember("InputAudioBufferSpeechStopped")]
    InputSpeechStopped,
    /// <summary>
    /// The <c>conversation.item.input_audio_transcription.completed</c> server event.
    /// </summary>
    [CodeGenMember("ConversationItemInputAudioTranscriptionCompleted")]
    InputTranscriptionFinished,
    /// <summary>
    /// The <c>conversation.item.input_audio_transcription.delta</c> server event.
    /// </summary>
    [CodeGenMember("ConversationItemInputAudioTranscriptionDelta")]
    InputTranscriptionDelta,
    /// <summary>
    /// The <c>conversation.item.input_audio_transcription.failed</c> server event.
    /// </summary>
    [CodeGenMember("ConversationItemInputAudioTranscriptionFailed")]
    InputTranscriptionFailed,
    /// <summary>
    /// The <c>input_audio_buffer.committed</c> server event.
    /// </summary>
    [CodeGenMember("InputAudioBufferCommitted")]
    InputAudioCommitted,
    /// <summary>
    /// The <c>input_audio_buffer.cleared</c> server event.
    /// </summary>
    [CodeGenMember("InputAudioBufferCleared")]
    InputAudioCleared,
    /// <summary>
    /// The <c>output_audio_buffer.cleared</c> server event.
    /// </summary>
    /// <remarks>
    /// This server event type is currently only used by WebRTC and not applicable to the WebSocket client.
    /// </remarks>
    [CodeGenMember("OutputAudioBufferCleared")]
    OutputAudioBufferCleared,
    /// <summary>
    /// The <c>output_audio_buffer.started</c> server event.
    /// </summary>
    /// <remarks>
    /// This server event type is currently only used by WebRTC and not applicable to the WebSocket client.
    /// </remarks>
    [CodeGenMember("OutputAudioBufferStarted")]
    OutputAudioBufferStarted,
    /// <summary>
    /// The <c>output_audio_buffer.stopped</c> server event.
    /// </summary>
    /// <remarks>
    /// This server event type is currently only used by WebRTC and not applicable to the WebSocket client.
    /// </remarks>
    [CodeGenMember("OutputAudioBufferStopped")]
    OutputAudioBufferStopped,
    /// <summary>
    /// The <c>transcription_session.created</c> server event.
    /// </summary>
    [CodeGenMember("TranscriptionSessionCreated")]
    TranscriptionSessionStarted,
    /// <summary>
    /// The <c>transcription_session.updated</c> server event.
    /// </summary>
    [CodeGenMember("TranscriptionSessionUpdated")]
    TranscriptionSessionConfigured,
    /// <summary>
    /// The <c>error</c> server event.
    /// </summary>
    [CodeGenMember("Error")]
    Error,
}