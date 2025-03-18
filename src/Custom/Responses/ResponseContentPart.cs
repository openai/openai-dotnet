using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

[CodeGenType("ResponsesContent")]
public partial class ResponseContentPart
{
    public ResponseContentPartKind Kind
    {
        get => InternalType.ToString().ToResponseContentPartKind();
        private set => InternalType = Kind.ToSerialString();
    }
    [CodeGenMember("Type")]
    internal InternalResponsesContentType InternalType { get; set; }

    // CUSTOM: Exposed input text properties.
    public string Text
        => (this as InternalResponsesInputTextContentPart)?.InternalText
        ?? (this as InternalResponsesOutputTextContentPart)?.InternalText;

    // CUSTOM: Exposed input image properties.
    public string InputImageFileId => (this as InternalResponsesInputImageContentPart)?.FileId;
    public ResponseImageDetailLevel? InputImageDetailLevel => (this as InternalResponsesInputImageContentPart)?.Detail;

    // CUSTOM: Exposed input file properties.
    public string InputFileId => (this as InternalResponsesInputFileContentPart)?.FileId;
    public string InputFilename => (this as InternalResponsesInputFileContentPart)?.Filename;
    public BinaryData InputFileBytes => (this as InternalResponsesInputFileContentPart)?.FileBytes;

    // CUSTOM: Exposed output text properties.
    public IReadOnlyList<ResponseMessageAnnotation> OutputTextAnnotations => (this as InternalResponsesOutputTextContentPart)?.Annotations?.ToList()?.AsReadOnly();

    // CUSTOM: Exposed refusal properties.
    public string Refusal => (this as InternalResponsesOutputRefusalContentPart)?.InternalRefusal;

    public static ResponseContentPart CreateInputTextPart(string text)
    {
        return new InternalResponsesInputTextContentPart(text);
    }

    public static ResponseContentPart CreateInputImagePart(BinaryData imageBytes, string imageBytesMediaType, ResponseImageDetailLevel? imageDetailLevel = null)
    {
        string base64EncodedData = Convert.ToBase64String(imageBytes.ToArray());
        string dataUri = $"data:{imageBytesMediaType};base64,{base64EncodedData}";
        return new InternalResponsesInputImageContentPart()
        {
            ImageUrl = dataUri,
            Detail = imageDetailLevel,
        };
    }

    public static ResponseContentPart CreateInputImagePart(string imageFileId, ResponseImageDetailLevel? imageDetailLevel = null)
    {
        return new InternalResponsesInputImageContentPart()
        {
            FileId = imageFileId,
            Detail = imageDetailLevel,
        };
    }

    public static ResponseContentPart CreateInputImagePart(Uri imageUri, ResponseImageDetailLevel? imageDetailLevel = default)
    {
        return new InternalResponsesInputImageContentPart()
        {
            ImageUrl = imageUri?.AbsoluteUri,
            Detail = imageDetailLevel,
        };
    }

    public static ResponseContentPart CreateInputFilePart(string fileId, string filename, BinaryData fileBytes)
    {
        return new InternalResponsesInputFileContentPart()
        {
            FileId = fileId,
            Filename = filename,
            FileBytes = fileBytes,
        };
    }

    public static ResponseContentPart CreateOutputTextPart(string text, IEnumerable<ResponseMessageAnnotation> annotations)
    {
        return new InternalResponsesOutputTextContentPart(annotations, text);
    }

    public static ResponseContentPart CreateRefusalPart(string refusal)
    {
        return new InternalResponsesOutputRefusalContentPart(refusal);
    }
}
