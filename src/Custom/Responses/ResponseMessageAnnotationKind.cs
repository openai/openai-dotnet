namespace OpenAI.Responses;

[CodeGenType("ResponseOutputTextAnnotationType")]
public enum ResponseMessageAnnotationKind
{
    FileCitation,

    [CodeGenMember("UrlCitation")]
    UriCitation,

    FilePath
}