using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class CreateChatCompletionResponseChoice
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal CreateChatCompletionResponseChoice(ChatFinishReason finishReason, int index, ChatCompletionResponseMessage message, CreateChatCompletionResponseChoiceLogprobs logprobs)
        {
            FinishReason = finishReason;
            Index = index;
            Message = message;
            Logprobs = logprobs;
        }

        internal CreateChatCompletionResponseChoice(ChatFinishReason finishReason, int index, ChatCompletionResponseMessage message, CreateChatCompletionResponseChoiceLogprobs logprobs, in JsonPatch patch)
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

        public CreateChatCompletionResponseChoiceLogprobs Logprobs { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;
    }
}