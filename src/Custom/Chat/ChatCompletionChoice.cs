using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ChatCompletionChoice
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal ChatCompletionChoice(ChatFinishReason finishReason, int index, ChatCompletionResponseMessage message, ChatCompletionChoiceProbabilities logprobs)
        {
            FinishReason = finishReason;
            Index = index;
            Message = message;
            Logprobs = logprobs;
        }

        internal ChatCompletionChoice(ChatFinishReason finishReason, int index, ChatCompletionResponseMessage message, ChatCompletionChoiceProbabilities logprobs, in JsonPatch patch)
        {
            FinishReason = finishReason;
            Index = index;
            Message = message;
            Logprobs = logprobs;
            _patch = patch;
        }

        public ChatFinishReason FinishReason { get; }

        public int Index { get; }

        public ChatCompletionResponseMessage Message { get; }

        public ChatCompletionChoiceProbabilities Logprobs { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;
    }
}