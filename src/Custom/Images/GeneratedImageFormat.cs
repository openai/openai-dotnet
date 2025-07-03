namespace OpenAI.Images;

// CUSTOM: Renamed.
/// <summary> The response format in which the generated images are returned by the service. </summary>
[CodeGenType("CreateImageRequestResponseFormat")]
public readonly partial struct GeneratedImageFormat
{
    // CUSTOM: Renamed.
    /// <summary> Returned as bytes in a base64-encoded string. </summary>
    [CodeGenMember("B64Json")]
    public static GeneratedImageFormat Bytes { get; } = new GeneratedImageFormat(B64JsonValue);

    // CUSTOM: Renamed.
    /// <summary>
    /// Returned as a URI pointing to a temporary internet location from where the image can be downlaoded. This URI is
    /// only valid for 60 minutes after the image is generated.
    /// </summary>
    [CodeGenMember("Url")]
    public static GeneratedImageFormat Uri { get; } = new GeneratedImageFormat(UrlValue);
}