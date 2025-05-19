namespace OpenAI.Images;

// CUSTOM: Renamed.
/// <summary>
///     The quality of the image that will be generated. <see cref="Hd"/> creates images with finer details and
///     greater consistency across the image.
///     <para>
///     High, medium and low qualities are only supported for gpt-image-1.
///     </para>
///     <para>
///     dall-e-2 only supports standard quality.
///     </para>
///     <para>
///     Defaults to auto.
///     </para>
/// </summary>
[CodeGenType("CreateImageRequestQuality")]
public readonly partial struct GeneratedImageQuality
{

    [CodeGenMember("Hd")]
    public static GeneratedImageQuality Hd { get; } = new GeneratedImageQuality(HdValue);

    /// <summary>
    /// High quality level - creates images with finer details and greater consistency.
    /// <para>
    /// Only supported for <c>gpt-image-1</c>.
    /// </para>
    /// </summary>
    /// 
    [CodeGenMember("High")]
    public static GeneratedImageQuality High { get; } = new GeneratedImageQuality(HdValue);
    /// <summary>
    /// Medium quality level.
    /// <para>
    /// Only supported for <c>gpt-image-1</c>.
    /// </para>
    /// </summary>
    [CodeGenMember("Medium")]
    public static GeneratedImageQuality Medium { get; } = new GeneratedImageQuality(MediumValue);

    /// <summary>
    /// Low quality level.
    /// <para>
    /// Only supported for <c>gpt-image-1</c>.
    /// </para>
    /// </summary>
    [CodeGenMember("Low")]
    public static GeneratedImageQuality Low { get; } = new GeneratedImageQuality(LowValue);

    /// <summary>
    /// Auto quality level - the system will automatically choose the appropriate quality level.
    /// <para>
    /// This is the default value.
    /// </para>
    /// </summary>
    [CodeGenMember("Auto")]
    public static GeneratedImageQuality Auto { get; } = new GeneratedImageQuality(AutoValue);
}