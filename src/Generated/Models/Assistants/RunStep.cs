// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenAI;

namespace OpenAI.Assistants
{
    [Experimental("OPENAI001")]
    public partial class RunStep
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal RunStep(string id, DateTimeOffset createdAt, string assistantId, string threadId, string runId, RunStepKind kind, RunStepStatus status, RunStepError lastError, DateTimeOffset? expiredAt, DateTimeOffset? cancelledAt, DateTimeOffset? failedAt, DateTimeOffset? completedAt, RunStepTokenUsage usage, RunStepDetails details)
        {
            Id = id;
            CreatedAt = createdAt;
            AssistantId = assistantId;
            ThreadId = threadId;
            RunId = runId;
            Kind = kind;
            Status = status;
            LastError = lastError;
            ExpiredAt = expiredAt;
            CancelledAt = cancelledAt;
            FailedAt = failedAt;
            CompletedAt = completedAt;
            Metadata = new ChangeTrackingDictionary<string, string>();
            Usage = usage;
            Details = details;
        }

        internal RunStep(string id, DateTimeOffset createdAt, string assistantId, string threadId, string runId, RunStepKind kind, RunStepStatus status, RunStepError lastError, DateTimeOffset? expiredAt, DateTimeOffset? cancelledAt, DateTimeOffset? failedAt, DateTimeOffset? completedAt, IReadOnlyDictionary<string, string> metadata, RunStepTokenUsage usage, string @object, RunStepDetails details, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Id = id;
            CreatedAt = createdAt;
            AssistantId = assistantId;
            ThreadId = threadId;
            RunId = runId;
            Kind = kind;
            Status = status;
            LastError = lastError;
            ExpiredAt = expiredAt;
            CancelledAt = cancelledAt;
            FailedAt = failedAt;
            CompletedAt = completedAt;
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Usage = usage;
            Object = @object;
            Details = details;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Id { get; }

        public DateTimeOffset CreatedAt { get; }

        public string AssistantId { get; }

        public string ThreadId { get; }

        public string RunId { get; }

        public RunStepKind Kind { get; }

        public RunStepStatus Status { get; }

        public RunStepError LastError { get; }

        public DateTimeOffset? ExpiredAt { get; }

        public DateTimeOffset? CancelledAt { get; }

        public DateTimeOffset? FailedAt { get; }

        public DateTimeOffset? CompletedAt { get; }

        public IReadOnlyDictionary<string, string> Metadata { get; }

        public RunStepTokenUsage Usage { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
