namespace OpenAI.Responses;

[CodeGenType("ResponsesContentType")]
public enum ResponseContentPartKind
{
    InputText,
    InputImage,
    InputFile,
    OutputText,
    Refusal,
}