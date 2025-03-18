namespace OpenAI.Responses;

// CUSTOM: Plain enum type, with Unknown, to convert from an underlying extensible enum
public enum ResponseTextFormatKind
{
    Unknown = 0,
    Text = 1,
    JsonObject = 2,
    JsonSchema = 3,
}