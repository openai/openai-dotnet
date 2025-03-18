namespace OpenAI.Responses;

// CUSTOM: Plain enum type, with Unknown, to convert from an underlying extensible enum
public enum ResponseContentPartKind
{
    Unknown,
    InputText,
    InputImage,
    InputFile,
    OutputText,
    Refusal,
}