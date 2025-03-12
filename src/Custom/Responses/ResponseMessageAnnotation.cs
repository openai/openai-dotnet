namespace OpenAI.Responses;

[CodeGenType("ResponsesOutputTextAnnotation")]
public partial class ResponseMessageAnnotation
{
    // CUSTOM:
    // - Renamed.
    // - Made public.
    // - Removed setter.
    [CodeGenMember("Type")]
    public ResponseMessageAnnotationKind Kind { get; }

    // CUSTOM: Exposed file citation properties.
    public string FileCitationFileId => (this as InternalResponsesMessageAnnotationFileCitation)?.FileId;
    public int? FileCitationIndex => (this as InternalResponsesMessageAnnotationFileCitation)?.Index;

    // CUSTOM: Exposed URL citation properties.
    public string UriCitationUri => (this as InternalResponsesMessageAnnotationUrlCitation).Url;
    public string UriCitationTitle => (this as InternalResponsesMessageAnnotationUrlCitation)?.Title;
    public int? UriCitationStartIndex => (this as InternalResponsesMessageAnnotationUrlCitation)?.StartIndex;
    public int? UriCitationEndIndex => (this as InternalResponsesMessageAnnotationUrlCitation)?.EndIndex;

    // CUSTOM: Exposed file path properties.
    public string FilePathFileId => (this as InternalResponsesMessageAnnotationFilePath)?.FileId;
    public int? FilePathIndex => (this as InternalResponsesMessageAnnotationFilePath)?.Index;
}
