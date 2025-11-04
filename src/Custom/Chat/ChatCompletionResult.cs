using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ChatCompletionResult
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal ChatCompletionResult(string id, IEnumerable<ChatCompletionChoice> choices, DateTimeOffset created, string model)
        {
            Id = id;
            Choices = choices.ToList();
            Created = created;
            Model = model;
        }

        internal ChatCompletionResult(string id, IList<ChatCompletionChoice> choices, DateTimeOffset created, string model, ChatServiceTier? serviceTier, string systemFingerprint, string @object, ChatTokenUsage usage, in JsonPatch patch)
        {
            // Plugin customization: ensure initialization of collections
            Id = id;
            Choices = choices ?? new ChangeTrackingList<ChatCompletionChoice>();
            Created = created;
            Model = model;
            ServiceTier = serviceTier;
            SystemFingerprint = systemFingerprint;
            Object = @object;
            Usage = usage;
            _patch = patch;
        }

        public string Id { get; }

        public IList<ChatCompletionChoice> Choices { get; }

        public DateTimeOffset Created { get; }

        public string Model { get; }

        public ChatServiceTier? ServiceTier { get; }

        public string SystemFingerprint { get; }

        public string Object { get; } = "chat.completion";

        public ChatTokenUsage Usage { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;
    }
}