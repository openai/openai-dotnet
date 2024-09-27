namespace OpenAI.Images;

// CUSTOM: Renamed.
/// <summary>
///     The style of the image that will be generated. <see cref="Vivid"/> causes the model to lean towards generating
///     hyper-real and dramatic images. <see cref="Natural"> causes the model to produce more natural, less hyper-real
///     looking images.
/// </summary>
[CodeGenModel("CreateImageRequestStyle")]
public readonly partial struct GeneratedImageStyle
{
}