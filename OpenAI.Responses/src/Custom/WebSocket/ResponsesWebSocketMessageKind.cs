using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

/// <summary>The wire event type of a Responses WebSocket message, including types unknown to this SDK version.</summary>
[Experimental("OPENAI001")]
public readonly partial struct ResponsesWebSocketMessageKind : IEquatable<ResponsesWebSocketMessageKind>
{
    private readonly string _value;

    /// <summary>Creates a message kind from its wire event type.</summary>
    /// <param name="value">The event type string.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
    public ResponsesWebSocketMessageKind(string value)
    {
        Argument.AssertNotNull(value, nameof(value));
        _value = value;
    }

    public static ResponsesWebSocketMessageKind ResponseAudioDelta { get; } = new ResponsesWebSocketMessageKind("response.audio.delta");

    public static ResponsesWebSocketMessageKind ResponseAudioDone { get; } = new ResponsesWebSocketMessageKind("response.audio.done");

    public static ResponsesWebSocketMessageKind ResponseAudioTranscriptDelta { get; } = new ResponsesWebSocketMessageKind("response.audio_transcript.delta");

    public static ResponsesWebSocketMessageKind ResponseAudioTranscriptDone { get; } = new ResponsesWebSocketMessageKind("response.audio_transcript.done");

    public static ResponsesWebSocketMessageKind ResponseCodeInterpreterCallCodeDelta { get; } = new ResponsesWebSocketMessageKind("response.code_interpreter_call_code.delta");

    public static ResponsesWebSocketMessageKind ResponseCodeInterpreterCallCodeDone { get; } = new ResponsesWebSocketMessageKind("response.code_interpreter_call_code.done");

    public static ResponsesWebSocketMessageKind ResponseCodeInterpreterCallCompleted { get; } = new ResponsesWebSocketMessageKind("response.code_interpreter_call.completed");

    public static ResponsesWebSocketMessageKind ResponseCodeInterpreterCallInProgress { get; } = new ResponsesWebSocketMessageKind("response.code_interpreter_call.in_progress");

    public static ResponsesWebSocketMessageKind ResponseCodeInterpreterCallInterpreting { get; } = new ResponsesWebSocketMessageKind("response.code_interpreter_call.interpreting");

    public static ResponsesWebSocketMessageKind ResponseCompleted { get; } = new ResponsesWebSocketMessageKind("response.completed");

    public static ResponsesWebSocketMessageKind ResponseContentPartAdded { get; } = new ResponsesWebSocketMessageKind("response.content_part.added");

    public static ResponsesWebSocketMessageKind ResponseContentPartDone { get; } = new ResponsesWebSocketMessageKind("response.content_part.done");

    public static ResponsesWebSocketMessageKind ResponseCreated { get; } = new ResponsesWebSocketMessageKind("response.created");

    public static ResponsesWebSocketMessageKind Error { get; } = new ResponsesWebSocketMessageKind("error");

    public static ResponsesWebSocketMessageKind ResponseFileSearchCallCompleted { get; } = new ResponsesWebSocketMessageKind("response.file_search_call.completed");

    public static ResponsesWebSocketMessageKind ResponseFileSearchCallInProgress { get; } = new ResponsesWebSocketMessageKind("response.file_search_call.in_progress");

    public static ResponsesWebSocketMessageKind ResponseFileSearchCallSearching { get; } = new ResponsesWebSocketMessageKind("response.file_search_call.searching");

    public static ResponsesWebSocketMessageKind ResponseFunctionCallArgumentsDelta { get; } = new ResponsesWebSocketMessageKind("response.function_call_arguments.delta");

    public static ResponsesWebSocketMessageKind ResponseFunctionCallArgumentsDone { get; } = new ResponsesWebSocketMessageKind("response.function_call_arguments.done");

    public static ResponsesWebSocketMessageKind ResponseInProgress { get; } = new ResponsesWebSocketMessageKind("response.in_progress");

    public static ResponsesWebSocketMessageKind ResponseFailed { get; } = new ResponsesWebSocketMessageKind("response.failed");

    public static ResponsesWebSocketMessageKind ResponseIncomplete { get; } = new ResponsesWebSocketMessageKind("response.incomplete");

    public static ResponsesWebSocketMessageKind ResponseOutputItemAdded { get; } = new ResponsesWebSocketMessageKind("response.output_item.added");

    public static ResponsesWebSocketMessageKind ResponseOutputItemDone { get; } = new ResponsesWebSocketMessageKind("response.output_item.done");

    public static ResponsesWebSocketMessageKind ResponseReasoningSummaryPartAdded { get; } = new ResponsesWebSocketMessageKind("response.reasoning_summary_part.added");

    public static ResponsesWebSocketMessageKind ResponseReasoningSummaryPartDone { get; } = new ResponsesWebSocketMessageKind("response.reasoning_summary_part.done");

    public static ResponsesWebSocketMessageKind ResponseReasoningSummaryTextDelta { get; } = new ResponsesWebSocketMessageKind("response.reasoning_summary_text.delta");

    public static ResponsesWebSocketMessageKind ResponseReasoningSummaryTextDone { get; } = new ResponsesWebSocketMessageKind("response.reasoning_summary_text.done");

    public static ResponsesWebSocketMessageKind ResponseReasoningTextDelta { get; } = new ResponsesWebSocketMessageKind("response.reasoning_text.delta");

    public static ResponsesWebSocketMessageKind ResponseReasoningTextDone { get; } = new ResponsesWebSocketMessageKind("response.reasoning_text.done");

    public static ResponsesWebSocketMessageKind ResponseRefusalDelta { get; } = new ResponsesWebSocketMessageKind("response.refusal.delta");

    public static ResponsesWebSocketMessageKind ResponseRefusalDone { get; } = new ResponsesWebSocketMessageKind("response.refusal.done");

    public static ResponsesWebSocketMessageKind ResponseOutputTextDelta { get; } = new ResponsesWebSocketMessageKind("response.output_text.delta");

    public static ResponsesWebSocketMessageKind ResponseOutputTextDone { get; } = new ResponsesWebSocketMessageKind("response.output_text.done");

    public static ResponsesWebSocketMessageKind ResponseWebSearchCallCompleted { get; } = new ResponsesWebSocketMessageKind("response.web_search_call.completed");

    public static ResponsesWebSocketMessageKind ResponseWebSearchCallInProgress { get; } = new ResponsesWebSocketMessageKind("response.web_search_call.in_progress");

    public static ResponsesWebSocketMessageKind ResponseWebSearchCallSearching { get; } = new ResponsesWebSocketMessageKind("response.web_search_call.searching");

    public static ResponsesWebSocketMessageKind ResponseImageGenerationCallCompleted { get; } = new ResponsesWebSocketMessageKind("response.image_generation_call.completed");

    public static ResponsesWebSocketMessageKind ResponseImageGenerationCallGenerating { get; } = new ResponsesWebSocketMessageKind("response.image_generation_call.generating");

    public static ResponsesWebSocketMessageKind ResponseImageGenerationCallInProgress { get; } = new ResponsesWebSocketMessageKind("response.image_generation_call.in_progress");

    public static ResponsesWebSocketMessageKind ResponseImageGenerationCallPartialImage { get; } = new ResponsesWebSocketMessageKind("response.image_generation_call.partial_image");

    public static ResponsesWebSocketMessageKind ResponseMcpCallArgumentsDelta { get; } = new ResponsesWebSocketMessageKind("response.mcp_call_arguments.delta");

    public static ResponsesWebSocketMessageKind ResponseMcpCallArgumentsDone { get; } = new ResponsesWebSocketMessageKind("response.mcp_call_arguments.done");

    public static ResponsesWebSocketMessageKind ResponseMcpCallCompleted { get; } = new ResponsesWebSocketMessageKind("response.mcp_call.completed");

    public static ResponsesWebSocketMessageKind ResponseMcpCallFailed { get; } = new ResponsesWebSocketMessageKind("response.mcp_call.failed");

    public static ResponsesWebSocketMessageKind ResponseMcpCallInProgress { get; } = new ResponsesWebSocketMessageKind("response.mcp_call.in_progress");

    public static ResponsesWebSocketMessageKind ResponseMcpListToolsCompleted { get; } = new ResponsesWebSocketMessageKind("response.mcp_list_tools.completed");

    public static ResponsesWebSocketMessageKind ResponseMcpListToolsFailed { get; } = new ResponsesWebSocketMessageKind("response.mcp_list_tools.failed");

    public static ResponsesWebSocketMessageKind ResponseMcpListToolsInProgress { get; } = new ResponsesWebSocketMessageKind("response.mcp_list_tools.in_progress");

    public static ResponsesWebSocketMessageKind ResponseOutputTextAnnotationAdded { get; } = new ResponsesWebSocketMessageKind("response.output_text.annotation.added");

    public static ResponsesWebSocketMessageKind ResponseQueued { get; } = new ResponsesWebSocketMessageKind("response.queued");

    public static ResponsesWebSocketMessageKind ResponseCustomToolCallInputDelta { get; } = new ResponsesWebSocketMessageKind("response.custom_tool_call_input.delta");

    public static ResponsesWebSocketMessageKind ResponseCustomToolCallInputDone { get; } = new ResponsesWebSocketMessageKind("response.custom_tool_call_input.done");

    public static ResponsesWebSocketMessageKind ResponseSteerAccepted { get; } = new ResponsesWebSocketMessageKind("response.steer.accepted");

    public static ResponsesWebSocketMessageKind ResponseSteerPending { get; } = new ResponsesWebSocketMessageKind("response.steer.pending");

    public static ResponsesWebSocketMessageKind ResponseSteerFailed { get; } = new ResponsesWebSocketMessageKind("response.steer.failed");

    public static bool operator ==(ResponsesWebSocketMessageKind left, ResponsesWebSocketMessageKind right) => left.Equals(right);

    public static bool operator !=(ResponsesWebSocketMessageKind left, ResponsesWebSocketMessageKind right) => !left.Equals(right);

    public static implicit operator ResponsesWebSocketMessageKind(string value) => new ResponsesWebSocketMessageKind(value);

    public static implicit operator ResponsesWebSocketMessageKind?(string value) => value == null ? null : new ResponsesWebSocketMessageKind(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is ResponsesWebSocketMessageKind other && Equals(other);

    public bool Equals(ResponsesWebSocketMessageKind other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

    public override string ToString() => _value;
}
