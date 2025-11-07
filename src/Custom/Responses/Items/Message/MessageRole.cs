using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

[Experimental("OPENAI001")]
public enum MessageRole
{
    Unknown = 0,

    Assistant = 1,

    Developer = 2,

    System = 3,

    User = 4,
}