namespace OpenAI.Responses;

// CUSTOM: Plain enum type, with Unknown, to convert from an underlying extensible enum
public enum MessageRole
{
    Unknown = 0,
    Assistant = 1,
    Developer = 2,
    System = 3,
    User = 4,
}