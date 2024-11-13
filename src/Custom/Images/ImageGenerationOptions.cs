namespace OpenAI.Images;

/// <summary>
/// Represents additional options available to control the behavior of an image generation operation.
/// </summary>
[CodeGenModel("CreateImageRequest")]
[CodeGenSuppress("ImageGenerationOptions", typeof(string))]
public partial class ImageGenerationOptions
{
    // CUSTOM: Made internal. The model is specified by the client.
    /// <summary> The model to use for image generation. </summary>
    internal InternalCreateImageRequestModel? Model { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary>
    /// A text description of the desired image(s). The maximum length is 1000 characters for
    /// `dall-e-2` and 4000 characters for `dall-e-3`.
    /// </summary>
    internal string Prompt { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    /// <summary>
    /// The number of images to generate. Must be between 1 and 10. For `dall-e-3`, only `n=1` is
    /// supported.
    /// </summary>
    internal long? N { get; set; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="ImageGenerationOptions"/>. </summary>
    public ImageGenerationOptions()
    {
    }

    // CUSTOM: Renamed.
    /// <summary>
    ///     A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    ///     <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Learn more</see>.
    /// </summary>
    [CodeGenMember("User")]
    public string EndUserId { get; set; }
}