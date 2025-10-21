using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class CreateChatCompletionResponseChoice2
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal CreateChatCompletionResponseChoice2(ChatFinishReason finishReason, int index, ChatCompletionResponseMessage2 message, CreateChatCompletionResponseChoiceLogprobs logprobs)
        {
            FinishReason = finishReason;
            Index = index;
            Message = message;
            Logprobs = logprobs;
        }

        internal CreateChatCompletionResponseChoice2(ChatFinishReason finishReason, int index, ChatCompletionResponseMessage2 message, CreateChatCompletionResponseChoiceLogprobs logprobs, in JsonPatch patch)
        {
            FinishReason = finishReason;
            Index = index;
            Message = message;
            Logprobs = logprobs;
            _patch = patch;
        }

        public ChatFinishReason FinishReason { get; }

        public int Index { get; }

        public ChatCompletionResponseMessage2 Message { get; }

        public CreateChatCompletionResponseChoiceLogprobs Logprobs { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;
    }
}