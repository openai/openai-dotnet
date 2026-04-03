using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ItemContent")]
public partial class ResponseContentPart
{
    // CUSTOM: Renamed to "Kind" and converted to public enum from internal extensible type.
    [CodeGenMember("Type")]
    internal InternalItemContentType InternalType { get; set; }
    public ResponseContentPartKind Kind
    {
        get => InternalType.ToString().ToResponseContentPartKind();
        private set => InternalType = Kind.ToSerialString();
    }

    // CUSTOM: Exposed input text properties.
    public string Text
        => (this as InternalItemContentInputText)?.InternalText
        ?? (this as InternalItemContentOutputText)?.InternalText;

    // CUSTOM: Exposed input image properties.
    public string InputImageFileId => (this as InternalItemContentInputImage)?.FileId;
    public ResponseImageDetailLevel? InputImageDetailLevel => (this as InternalItemContentInputImage)?.Detail;
    public string InputImageUri => (this as InternalItemContentInputImage)?.ImageUri;

    // CUSTOM: Exposed input file properties.
    public string InputFileId => (this as InternalItemContentInputFile)?.FileId;
    public string InputFilename => (this as InternalItemContentInputFile)?.Filename;
    public Uri InputFileUri => (this as InternalItemContentInputFile)?.FileUrl;
    public BinaryData InputFileBytes => (this as InternalItemContentInputFile)?.InternalFileBytes;
    public string InputFileBytesMediaType => (this as InternalItemContentInputFile)?.InternalFileBytesMediaType;

    // CUSTOM: Exposed output text properties.
    public IReadOnlyList<ResponseMessageAnnotation> OutputTextAnnotations => (this as InternalItemContentOutputText)?.Annotations?.ToList()?.AsReadOnly();

    // CUSTOM: Exposed refusal properties.
    public string Refusal => (this as InternalItemContentRefusal)?.InternalRefusal;

    public static ResponseContentPart CreateInputTextPart(string text)
    {
        return new InternalItemContentInputText(text);
    }

    public static ResponseContentPart CreateInputImagePart(string imageFileId, ResponseImageDetailLevel? imageDetailLevel = null)
    {
        return new InternalItemContentInputImage()
        {
            FileId = imageFileId,
            Detail = imageDetailLevel,
        };
    }

    public static ResponseContentPart CreateInputImagePart(Uri imageUri, ResponseImageDetailLevel? imageDetailLevel = default)
    {
        Argument.AssertNotNull(imageUri, nameof(imageUri));

        return new InternalItemContentInputImage()
        {
            ImageUri = imageUri.AbsoluteUri,
            Detail = imageDetailLevel,
        };
    }
    public static ResponseContentPart CreateInputImagePart(BinaryData imageBytes, ResponseImageDetailLevel? imageDetailLevel = null)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNullOrEmpty(imageBytes.MediaType, nameof(imageBytes.MediaType));

        return new InternalItemContentInputImage()
        {
            ImageUri = DataEncodingHelpers.CreateDataUri(imageBytes, imageBytes.MediaType),
            Detail = imageDetailLevel,
        };
    }

    public static ResponseContentPart CreateInputFilePart(string fileId)
    {
        return new InternalItemContentInputFile()
        {
            FileId = fileId,
        };
    }

    public static ResponseContentPart CreateInputFilePart(BinaryData fileBytes, string fileBytesMediaType, string filename)
    {
        Argument.AssertNotNull(fileBytes, nameof(fileBytes));
        Argument.AssertNotNullOrEmpty(fileBytesMediaType, nameof(fileBytesMediaType));
        Argument.AssertNotNullOrEmpty(filename, nameof(filename));

        return new InternalItemContentInputFile(filename, fileBytes, fileBytesMediaType);
    }

    public static ResponseContentPart CreateInputFilePart(Uri fileUri)
    {
        Argument.AssertNotNull(fileUri, nameof(fileUri));

        return new InternalItemContentInputFile()
        {
            FileUrl = fileUri,
        };
    }

    public static ResponseContentPart CreateOutputTextPart(string text, IEnumerable<ResponseMessageAnnotation> annotations)
    {
        return new InternalItemContentOutputText(text, annotations);
    }

    public static ResponseContentPart CreateRefusalPart(string refusal)
    {
        return new InternalItemContentRefusal(refusal);
    }
}
