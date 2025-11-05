using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public enum ChatRequestModality
    {
        Text,
        Audio
    }
}