namespace OpenAI.Images;

/// <summary> The media type of an image file supplied in a multipart request. </summary>
public enum ImageFileContentType
{
    /// <summary> The <c>image/png</c> media type. </summary>
    Png,

    /// <summary> The <c>image/jpeg</c> media type. </summary>
    Jpeg,

    /// <summary> The <c>image/webp</c> media type. </summary>
    Webp,
}
