using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public enum ResponseFormatType
    {
        Text,
        JsonObject,
        JsonSchema
    }
}