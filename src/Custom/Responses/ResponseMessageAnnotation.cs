using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("Annotation")]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class ResponseMessageAnnotation
{
    // CUSTOM: Exposed file citation properties.
    public string FileCitationFileId => (this as InternalAnnotationFileCitation)?.FileId;
    public int? FileCitationIndex => (this as InternalAnnotationFileCitation)?.Index;

    // CUSTOM: Exposed URL citation properties.
    public Uri UriCitationUri => (this as InternalAnnotationUrlCitation)?.Url;
    public string UriCitationTitle => (this as InternalAnnotationUrlCitation)?.Title;
    public int? UriCitationStartIndex => (this as InternalAnnotationUrlCitation)?.StartIndex;
    public int? UriCitationEndIndex => (this as InternalAnnotationUrlCitation)?.EndIndex;

    // CUSTOM: Exposed file path properties.
    public string FilePathFileId => (this as InternalAnnotationFilePath)?.FileId;
    public int? FilePathIndex => (this as InternalAnnotationFilePath)?.Index;
}
