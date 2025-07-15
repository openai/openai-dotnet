using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Images;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed methods that only take the options parameter.
/// <summary> The service client for OpenAI image operations. </summary>
[CodeGenType("Images")]
[CodeGenSuppress("ImageClient", typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress("GenerateImagesAsync", typeof(ImageGenerationOptions), typeof(CancellationToken))]
[CodeGenSuppress("GenerateImages", typeof(ImageGenerationOptions), typeof(CancellationToken))]
public partial class ImageClient
{
    private readonly string _model;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ImageClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="apiKey"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ImageClient(string model, string apiKey) : this(model, new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ImageClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ImageClient(string model, ApiKeyCredential credential) : this(model, credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ImageClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ImageClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="ImageClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> or <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    protected internal ImageClient(ClientPipeline pipeline, string model, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    #region GenerateImages

    /// <summary> Generates an image based on a prompt. </summary>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> The options to configure the image generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageAsync(string prompt, ImageGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageGenerationOptions(prompt, null, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = await GenerateImagesAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates an image based on a prompt. </summary>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> The options to configure the image generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImage> GenerateImage(string prompt, ImageGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageGenerationOptions(prompt, null, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = GenerateImages(content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates images based on a prompt. </summary>
    /// <param name="prompt"> A text description of the desired images. </param>
    /// <param name="imageCount"> The number of images to generate. </param>
    /// <param name="options"> The options to configure the image generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImagesAsync(string prompt, int imageCount, ImageGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageGenerationOptions(prompt, imageCount, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = await GenerateImagesAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Generates images based on a prompt. </summary>
    /// <param name="prompt"> A text description of the desired images. </param>
    /// <param name="imageCount"> The number of images to generate. </param>
    /// <param name="options"> The options to configure the image generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImageCollection> GenerateImages(string prompt, int imageCount, ImageGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageGenerationOptions(prompt, imageCount, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = GenerateImages(content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result), result.GetRawResponse());
    }

    #endregion

    #region GenerateImageEdits

    /// <summary> Generates an edited or extended image based on an original image and a prompt. </summary>
    /// <param name="image">
    ///     The image stream to edit. Must be a valid PNG file, less than 4MB, and square. The image must have transparency, which
    ///     will be used as the mask.
    /// </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(Stream image, string imageFilename, string prompt, ImageEditOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, null, null, null, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, null, null);
        ClientResult result = await GenerateImageEditsAsync(content, content.ContentType, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates an edited or extended image based on an original image and a prompt. </summary>
    /// <param name="image">
    ///     The image stream to edit. Must be a valid PNG file, less than 4MB, and square. The image must have transparency, which
    ///     will be used as the mask.
    /// </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImage> GenerateImageEdit(Stream image, string imageFilename, string prompt, ImageEditOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, null, null, null, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, null, null);
        ClientResult result = GenerateImageEdits(content, content.ContentType, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates an edited or extended image based on an original image and a prompt. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The image must
    ///     have transparency, which will be used as the mask. The provided file path's extension (for example: .png)
    ///     will be used to validate the format of the input image. The request may fail if the file path's extension
    ///     and the actual format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(string imageFilePath, string prompt, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return await GenerateImageEditAsync(imageStream, imageFilePath, prompt, options).ConfigureAwait(false);
    }

    /// <summary> Generates an edited or extended image based on an original image and a prompt. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The image must
    ///     have transparency, which will be used as the mask. The provided file path's extension (for example: .png)
    ///     will be used to validate the format of the input image. The request may fail if the file path's extension
    ///     and the actual format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImage> GenerateImageEdit(string imageFilePath, string prompt, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return GenerateImageEdit(imageStream, imageFilePath, prompt, options);
    }

    /// <summary> Generates an edited or extended image based on an original image, a prompt, and a mask. </summary>
    /// <param name="image"> The image stream to edit. Must be a valid PNG file, less than 4MB, and square. </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="mask">
    ///     An additional image whose fully transparent areas (i.e., where alpha is zero) indicate where the original image
    ///     should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
    /// </param>
    /// <param name="maskFilename">
    ///     The filename associated with the mask image stream. The filename's extension (for example: .png) will be
    ///     used to validate the format of the mask image. The request may fail if the filename's extension and the
    ///     actual format of the mask image do not match.
    /// </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, <paramref name="prompt"/>, <paramref name="mask"/>, or <paramref name="maskFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/>, <paramref name="prompt"/>, or <paramref name="maskFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, ImageEditOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNull(mask, nameof(mask));
        Argument.AssertNotNullOrEmpty(maskFilename, nameof(maskFilename));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, mask, maskFilename, null, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, mask, maskFilename);
        ClientResult result = await GenerateImageEditsAsync(content, content.ContentType, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates an edited or extended image based on an original image, a prompt, and a mask. </summary>
    /// <param name="image"> The image stream to edit. Must be a valid PNG file, less than 4MB, and square. </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="mask">
    ///     An additional image whose fully transparent areas (i.e., where alpha is zero) indicate where the original image
    ///     should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
    /// </param>
    /// <param name="maskFilename">
    ///     The filename associated with the mask image stream. The filename's extension (for example: .png) will be
    ///     used to validate the format of the mask image. The request may fail if the filename's extension and the
    ///     actual format of the mask image do not match.
    /// </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, <paramref name="prompt"/>, <paramref name="mask"/>, or <paramref name="maskFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/>, <paramref name="prompt"/>, or <paramref name="maskFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImage> GenerateImageEdit(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, ImageEditOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNull(mask, nameof(mask));
        Argument.AssertNotNullOrEmpty(maskFilename, nameof(maskFilename));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, mask, maskFilename, null, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, mask, maskFilename);
        ClientResult result = GenerateImageEdits(content, content.ContentType, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates an edited or extended image based on an original image, a prompt, and a mask. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The provided file
    ///     path's extension (for example: .png) will be used to validate the format of the input image. The request
    ///     may fail if the file path's extension and the actual format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="maskFilePath">
    ///     The path of the mask image file whose fully transparent areas (i.e., where alpha is zero) indicate where
    ///     the original image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions
    ///     as the original image. The provided file path's extension (for example: .png) will be used to validate the
    ///     format of the mask image. The request may fail if the file path's extension and the actual format of the
    ///     mask image do not match.
    /// </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/> or <paramref name="maskFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/>, or <paramref name="maskFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(string imageFilePath, string prompt, string maskFilePath, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNullOrEmpty(maskFilePath, nameof(maskFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        using FileStream maskStream = File.OpenRead(maskFilePath);
        return await GenerateImageEditAsync(imageStream, imageFilePath, prompt, maskStream, maskFilePath, options).ConfigureAwait(false);
    }

    /// <summary> Generates an edited or extended image based on an original image, a prompt, and a mask. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The provided file
    ///     path's extension (for example: .png) will be used to validate the format of the input image. The request
    ///     may fail if the file path's extension and the actual format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="maskFilePath">
    ///     The path of the mask image file whose fully transparent areas (i.e., where alpha is zero) indicate where
    ///     the original image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions
    ///     as the original image. The provided file path's extension (for example: .png) will be used to validate the
    ///     format of the mask image. The request may fail if the file path's extension and the actual format of the
    ///     mask image do not match.
    /// </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/> or <paramref name="maskFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/>, or <paramref name="maskFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImage> GenerateImageEdit(string imageFilePath, string prompt, string maskFilePath, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNullOrEmpty(maskFilePath, nameof(maskFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        using FileStream maskStream = File.OpenRead(maskFilePath);
        return GenerateImageEdit(imageStream, imageFilePath, prompt, maskStream, maskFilePath, options);
    }

    /// <summary> Generates edited or extended images based on an original image and a prompt. </summary>
    /// <param name="image">
    ///     The image stream to edit. Must be a valid PNG file, less than 4MB, and square. The image must have transparency, which
    ///     will be used as the mask.
    /// </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="imageCount"> The number of edited or extended images to generate. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(Stream image, string imageFilename, string prompt, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, null, null, imageCount, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, null, null);
        ClientResult result = await GenerateImageEditsAsync(content, content.ContentType, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Generates edited or extended images based on an original image and a prompt. </summary>
    /// <param name="image">
    ///     The image stream to edit. Must be a valid PNG file, less than 4MB, and square. The image must have transparency, which
    ///     will be used as the mask.
    /// </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="imageCount"> The number of edited or extended images to generate. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(Stream image, string imageFilename, string prompt, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, null, null, imageCount, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, null, null);
        ClientResult result = GenerateImageEdits(content, content.ContentType, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Generates edited or extended images based on an original image and a prompt. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The image must
    ///     have transparency, which will be used as the mask. The provided file path's extension (for example: .png)
    ///     will be used to validate the format of the input image. The request may fail if the file path's extension
    ///     and the actual format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="imageCount"> The number of edited or extended images to generate. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(string imageFilePath, string prompt, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return await GenerateImageEditsAsync(imageStream, imageFilePath, prompt, imageCount, options).ConfigureAwait(false);
    }

    /// <summary> Generates edited or extended images based on an original image and a prompt. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The image must
    ///     have transparency, which will be used as the mask. The provided file path's extension (for example: .png)
    ///     will be used to validate the format of the input image. The request may fail if the file path's extension
    ///     and the actual format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="imageCount"> The number of edited or extended images to generate. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(string imageFilePath, string prompt, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return GenerateImageEdits(imageStream, imageFilePath, prompt, imageCount, options);
    }

    /// <summary> Generates edited or extended images based on an original image, a prompt, and a mask. </summary>
    /// <param name="image"> The image stream to edit. Must be a valid PNG file, less than 4MB, and square. </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="mask">
    ///     An additional image whose fully transparent areas (i.e., where alpha is zero) indicate where the original image
    ///     should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
    /// </param>
    /// <param name="maskFilename">
    ///     The filename associated with the mask image stream. The filename's extension (for example: .png) will be
    ///     used to validate the format of the mask image. The request may fail if the filename's extension and the
    ///     actual format of the mask image do not match.
    /// </param>
    /// <param name="imageCount"> The number of edited or extended images to generate. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, <paramref name="prompt"/>, <paramref name="mask"/>, or <paramref name="maskFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/>, <paramref name="prompt"/>, or <paramref name="maskFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNull(mask, nameof(mask));
        Argument.AssertNotNullOrEmpty(maskFilename, nameof(maskFilename));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, mask, maskFilename, imageCount, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, mask, maskFilename);
        ClientResult result = await GenerateImageEditsAsync(content, content.ContentType, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Generates edited or extended images based on an original image, a prompt, and a mask. </summary>
    /// <param name="image"> The image stream to edit. Must be a valid PNG file, less than 4MB, and square. </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="mask">
    ///     An additional image whose fully transparent areas (i.e., where alpha is zero) indicate where the original image
    ///     should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
    /// </param>
    /// <param name="maskFilename">
    ///     The filename associated with the mask image stream. The filename's extension (for example: .png) will be
    ///     used to validate the format of the mask image. The request may fail if the filename's extension and the
    ///     actual format of the mask image do not match.
    /// </param>
    /// <param name="imageCount"> The number of edited or extended images to generate. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, <paramref name="prompt"/>, <paramref name="mask"/>, or <paramref name="maskFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/>, <paramref name="prompt"/>, or <paramref name="maskFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNull(mask, nameof(mask));
        Argument.AssertNotNullOrEmpty(maskFilename, nameof(maskFilename));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, mask, maskFilename, imageCount, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, mask, maskFilename);
        ClientResult result = GenerateImageEdits(content, content.ContentType, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Generates edited or extended images based on an original image, a prompt, and a mask. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The provided file
    ///     path's extension (for example: .png) will be used to validate the format of the input image. The request
    ///     may fail if the file path's extension and the actual format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="maskFilePath">
    ///     The path of the mask image file whose fully transparent areas (i.e., where alpha is zero) indicate where
    ///     the original image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions
    ///     as the original image. The provided file path's extension (for example: .png) will be used to validate the
    ///     format of the mask image. The request may fail if the file path's extension and the actual format of the
    ///     mask image do not match.
    /// </param>
    /// <param name="imageCount"> The number of edited or extended images to generate. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/> or <paramref name="maskFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/>, or <paramref name="maskFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(string imageFilePath, string prompt, string maskFilePath, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNullOrEmpty(maskFilePath, nameof(maskFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        using FileStream maskStream = File.OpenRead(maskFilePath);
        return await GenerateImageEditsAsync(imageStream, imageFilePath, prompt, maskStream, maskFilePath, imageCount, options).ConfigureAwait(false);
    }

    /// <summary> Generates edited or extended images based on an original image, a prompt, and a mask. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The provided file
    ///     path's extension (for example: .png) will be used to validate the format of the input image. The request
    ///     may fail if the file path's extension and the actual format of the input image do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="maskFilePath">
    ///     The path of the mask image file whose fully transparent areas (i.e., where alpha is zero) indicate where
    ///     the original image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions
    ///     as the original image. The provided file path's extension (for example: .png) will be used to validate the
    ///     format of the mask image. The request may fail if the file path's extension and the actual format of the
    ///     mask image do not match.
    /// </param>
    /// <param name="imageCount"> The number of edited or extended images to generate. </param>
    /// <param name="options"> The options to configure the image edit. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/> or <paramref name="maskFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/>, or <paramref name="maskFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(string imageFilePath, string prompt, string maskFilePath, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNullOrEmpty(maskFilePath, nameof(maskFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        using FileStream maskStream = File.OpenRead(maskFilePath);
        return GenerateImageEdits(imageStream, imageFilePath, prompt, maskStream, maskFilePath, imageCount, options);
    }

    #endregion

    #region GenerateImageVariations

    /// <summary> Generates a variation of a given image. </summary>
    /// <param name="image"> The image stream to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and square. </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="options"> The options to configure the image variation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/> or <paramref name="imageFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageVariationAsync(Stream image, string imageFilename, ImageVariationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));

        options ??= new();
        CreateImageVariationOptions(image, imageFilename, null, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename);
        ClientResult result = await GenerateImageVariationsAsync(content, content.ContentType, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates a variation of a given image. </summary>
    /// <param name="image"> The image stream to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and square. </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="options"> The options to configure the image variation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/> or <paramref name="imageFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImage> GenerateImageVariation(Stream image, string imageFilename, ImageVariationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));

        options ??= new();
        CreateImageVariationOptions(image, imageFilename, null, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename);
        ClientResult result = GenerateImageVariations(content, content.ContentType, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates a variation of a given image. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to use as the basis for the variation. Must be a valid PNG file, less than 4MB,
    ///     and square. The provided file path's extension (for example: .png) will be used to validate the format of
    ///     the input image. The request may fail if the file path's extension and the actual format of the input image
    ///     do not match.
    /// </param>
    /// <param name="options"> The options to configure the image variation. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageVariationAsync(string imageFilePath, ImageVariationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return await GenerateImageVariationAsync(imageStream, imageFilePath, options).ConfigureAwait(false);
    }

    /// <summary> Generates a variation of a given image. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to use as the basis for the variation. Must be a valid PNG file, less than 4MB,
    ///     and square. The provided file path's extension (for example: .png) will be used to validate the format of
    ///     the input image. The request may fail if the file path's extension and the actual format of the input image
    ///     do not match.
    /// </param>
    /// <param name="options"> The options to configure the image variation. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImage> GenerateImageVariation(string imageFilePath, ImageVariationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return GenerateImageVariation(imageStream, imageFilePath, options);
    }

    /// <summary> Generates variations of a given image. </summary>
    /// <param name="image"> The image stream to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and square. </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="imageCount"> The number of image variations to generate. </param>
    /// <param name="options"> The options to configure the image variation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/> or <paramref name="imageFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageVariationsAsync(Stream image, string imageFilename, int imageCount, ImageVariationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));

        options ??= new();
        CreateImageVariationOptions(image, imageFilename, imageCount, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename);
        ClientResult result = await GenerateImageVariationsAsync(content, content.ContentType, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Generates variations of a given image. </summary>
    /// <param name="image"> The image stream to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and square. </param>
    /// <param name="imageFilename">
    ///     The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    ///     validate the format of the input image. The request may fail if the filename's extension and the actual
    ///     format of the input image do not match.
    /// </param>
    /// <param name="imageCount"> The number of image variations to generate. </param>
    /// <param name="options"> The options to configure the image variation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/> or <paramref name="imageFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImageCollection> GenerateImageVariations(Stream image, string imageFilename, int imageCount, ImageVariationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));

        options ??= new();
        CreateImageVariationOptions(image, imageFilename, imageCount, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename);
        ClientResult result = GenerateImageVariations(content, content.ContentType, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(GeneratedImageCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Generates variations of a given image. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to use as the basis for the variation. Must be a valid PNG file, less than 4MB,
    ///     and square. The provided file path's extension (for example: .png) will be used to validate the format of
    ///     the input image. The request may fail if the file path's extension and the actual format of the input image
    ///     do not match.
    /// </param>
    /// <param name="imageCount"> The number of image variations to generate. </param>
    /// <param name="options"> The options to configure the image variation. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> was null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageVariationsAsync(string imageFilePath, int imageCount, ImageVariationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return await GenerateImageVariationsAsync(imageStream, imageFilePath, imageCount, options).ConfigureAwait(false);
    }

    /// <summary> Generates variations of a given image. </summary>
    /// <param name="imageFilePath">
    ///     The path of the image file to use as the basis for the variation. Must be a valid PNG file, less than 4MB,
    ///     and square. The provided file path's extension (for example: .png) will be used to validate the format of
    ///     the input image. The request may fail if the file path's extension and the actual format of the input image
    ///     do not match.
    /// </param>
    /// <param name="imageCount"> The number of image variations to generate. </param>
    /// <param name="options"> The options to configure the image variation. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> was null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<GeneratedImageCollection> GenerateImageVariations(string imageFilePath, int imageCount, ImageVariationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return GenerateImageVariations(imageStream, imageFilePath, imageCount, options);
    }

    #endregion

    private void CreateImageGenerationOptions(string prompt, int? imageCount, ref ImageGenerationOptions options)
    {
        options.Prompt = prompt;
        options.N = imageCount;
        options.Model = _model;
    }

    private void CreateImageEditOptions(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, int? imageCount, ref ImageEditOptions options)
    {
        options.Prompt = prompt;
        options.N = imageCount;
        options.Model = _model;
    }

    private void CreateImageVariationOptions(Stream image, string imageFilename, int? imageCount, ref ImageVariationOptions options)
    {
        options.N = imageCount;
        options.Model = _model;
    }
}
