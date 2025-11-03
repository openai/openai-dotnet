using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ChatCompletionResponseChoice
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal ChatCompletionResponseChoice(ChatFinishReason finishReason, int index, ChatCompletionResponseMessage message, ChatCompletionResponseChoiceLogprobs logprobs)
        {
            FinishReason = finishReason;
            Index = index;
            Message = message;
            Logprobs = logprobs;
        }

        internal ChatCompletionResponseChoice(ChatFinishReason finishReason, int index, ChatCompletionResponseMessage message, ChatCompletionResponseChoiceLogprobs logprobs, in JsonPatch patch)
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

        public ChatCompletionResponseChoiceLogprobs Logprobs { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;
    }
}