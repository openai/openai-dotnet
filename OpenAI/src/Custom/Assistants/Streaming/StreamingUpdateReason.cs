using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary>
/// The collection of values associated with the event names of streaming update payloads. These correspond to the
/// expected downcast data type of the <see cref="StreamingUpdate"/> as well as to the expected data present in the
/// payload.
/// </summary>
[Experimental("OPENAI001")]
public readonly partial struct StreamingUpdateReason : IEquatable<StreamingUpdateReason>
{
    private readonly string _value;

    private const string ThreadCreatedValue = "thread.created";
    private const string RunCreatedValue = "thread.run.created";
    private const string RunQueuedValue = "thread.run.queued";
    private const string RunInProgressValue = "thread.run.in_progress";
    private const string RunRequiresActionValue = "thread.run.requires_action";
    private const string RunCompletedValue = "thread.run.completed";
    private const string RunIncompleteValue = "thread.run.incomplete";
    private const string RunFailedValue = "thread.run.failed";
    private const string RunCancellingValue = "thread.run.cancelling";
    private const string RunCancelledValue = "thread.run.cancelled";
    private const string RunExpiredValue = "thread.run.expired";
    private const string RunStepCreatedValue = "thread.run.step.created";
    private const string RunStepInProgressValue = "thread.run.step.in_progress";
    private const string RunStepUpdatedValue = "thread.run.step.delta";
    private const string RunStepCompletedValue = "thread.run.step.completed";
    private const string RunStepFailedValue = "thread.run.step.failed";
    private const string RunStepCancelledValue = "thread.run.step.cancelled";
    private const string RunStepExpiredValue = "thread.run.step.expired";
    private const string MessageCreatedValue = "thread.message.created";
    private const string MessageInProgressValue = "thread.message.in_progress";
    private const string MessageUpdatedValue = "thread.message.delta";
    private const string MessageCompletedValue = "thread.message.completed";
    private const string MessageFailedValue = "thread.message.incomplete";
    private const string ErrorValue = "error";
    private const string DoneValue = "done";

    public StreamingUpdateReason(string value)
    {
        Argument.AssertNotNull(value, nameof(value));
        _value = value;
    }

    public static StreamingUpdateReason ThreadCreated { get; } = new StreamingUpdateReason(ThreadCreatedValue);
    public static StreamingUpdateReason RunCreated { get; } = new StreamingUpdateReason(RunCreatedValue);
    public static StreamingUpdateReason RunQueued { get; } = new StreamingUpdateReason(RunQueuedValue);
    public static StreamingUpdateReason RunInProgress { get; } = new StreamingUpdateReason(RunInProgressValue);
    public static StreamingUpdateReason RunRequiresAction { get; } = new StreamingUpdateReason(RunRequiresActionValue);
    public static StreamingUpdateReason RunCompleted { get; } = new StreamingUpdateReason(RunCompletedValue);
    public static StreamingUpdateReason RunIncomplete { get; } = new StreamingUpdateReason(RunIncompleteValue);
    public static StreamingUpdateReason RunFailed { get; } = new StreamingUpdateReason(RunFailedValue);
    public static StreamingUpdateReason RunCancelling { get; } = new StreamingUpdateReason(RunCancellingValue);
    public static StreamingUpdateReason RunCancelled { get; } = new StreamingUpdateReason(RunCancelledValue);
    public static StreamingUpdateReason RunExpired { get; } = new StreamingUpdateReason(RunExpiredValue);
    public static StreamingUpdateReason RunStepCreated { get; } = new StreamingUpdateReason(RunStepCreatedValue);
    public static StreamingUpdateReason RunStepInProgress { get; } = new StreamingUpdateReason(RunStepInProgressValue);
    public static StreamingUpdateReason RunStepUpdated { get; } = new StreamingUpdateReason(RunStepUpdatedValue);
    public static StreamingUpdateReason RunStepCompleted { get; } = new StreamingUpdateReason(RunStepCompletedValue);
    public static StreamingUpdateReason RunStepFailed { get; } = new StreamingUpdateReason(RunStepFailedValue);
    public static StreamingUpdateReason RunStepCancelled { get; } = new StreamingUpdateReason(RunStepCancelledValue);
    public static StreamingUpdateReason RunStepExpired { get; } = new StreamingUpdateReason(RunStepExpiredValue);
    public static StreamingUpdateReason MessageCreated { get; } = new StreamingUpdateReason(MessageCreatedValue);
    public static StreamingUpdateReason MessageInProgress { get; } = new StreamingUpdateReason(MessageInProgressValue);
    public static StreamingUpdateReason MessageUpdated { get; } = new StreamingUpdateReason(MessageUpdatedValue);
    public static StreamingUpdateReason MessageCompleted { get; } = new StreamingUpdateReason(MessageCompletedValue);
    public static StreamingUpdateReason MessageFailed { get; } = new StreamingUpdateReason(MessageFailedValue);
    public static StreamingUpdateReason Error { get; } = new StreamingUpdateReason(ErrorValue);
    public static StreamingUpdateReason Done { get; } = new StreamingUpdateReason(DoneValue);

    public static bool operator ==(StreamingUpdateReason left, StreamingUpdateReason right) => left.Equals(right);
    public static bool operator !=(StreamingUpdateReason left, StreamingUpdateReason right) => !left.Equals(right);
    public static implicit operator StreamingUpdateReason(string value) => new StreamingUpdateReason(value);
    public static implicit operator StreamingUpdateReason?(string value) => value == null ? null : new StreamingUpdateReason(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is StreamingUpdateReason other && Equals(other);
    public bool Equals(StreamingUpdateReason other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;
    public override string ToString() => _value;
}
