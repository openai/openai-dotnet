using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public enum ChatCompletionRequestModality
    {
        Text,
        Audio
    }
}