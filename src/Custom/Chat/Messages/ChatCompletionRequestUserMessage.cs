using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ChatCompletionRequestUserMessage : ChatMessage
    {
        public ChatCompletionRequestUserMessage() : this(ChatMessageRole.User, null, default, null)
        {
        }

        internal ChatCompletionRequestUserMessage(ChatMessageRole role, ChatMessageContent content, in JsonPatch patch, string name) : base(role, content, patch)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}