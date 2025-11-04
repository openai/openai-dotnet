using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ChatCompletionChoiceProbabilities
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal ChatCompletionChoiceProbabilities() : this(null, null, default)
        {
        }

        internal ChatCompletionChoiceProbabilities(IReadOnlyList<ChatTokenLogProbabilityDetails> content, IReadOnlyList<ChatTokenLogProbabilityDetails> refusal, in JsonPatch patch)
        {
            // Plugin customization: ensure initialization of collections
            Content = content ?? new ChangeTrackingList<ChatTokenLogProbabilityDetails>();
            Refusal = refusal ?? new ChangeTrackingList<ChatTokenLogProbabilityDetails>();
        }

        public IReadOnlyList<ChatTokenLogProbabilityDetails> Content { get; }

        public IReadOnlyList<ChatTokenLogProbabilityDetails> Refusal { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;
    }
}