using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Images;

/// <summary> The service client for OpenAI image operations. </summary>
[CodeGenClient("Images")]
[CodeGenSuppress("ImageClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateImageAsync", typeof(ImageGenerationOptions))]
[CodeGenSuppress("CreateImage", typeof(ImageGenerationOptions))]
[CodeGenSuppress("CreateImageEditAsync", typeof(ImageEditOptions))]
[CodeGenSuppress("CreateImageEdit", typeof(ImageEditOptions))]
[CodeGenSuppress("CreateImageVariationAsync", typeof(ImageVariationOptions))]
[CodeGenSuppress("CreateImageVariation", typeof(ImageVariationOptions))]
public partial class ImageClient
{
    private readonly string _model;

    // CUSTOM:
    // - Added `model` parameter.
    // - Added support for retrieving credential and endpoint from environment variables.

    /// <summary>
    /// Initializes a new instance of <see cref="ImageClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="model"> The model name to use for image operations. </param>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public ImageClient(string model, ApiKeyCredential credential, OpenAIClientOptions options = default)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(credential, requireExplicitCredential: true), options),
              model,
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="ImageClient"/> that will use an API key from the OPENAI_API_KEY
    /// environment variable when authenticating.
    /// </summary>
    /// <remarks>
    /// To provide an explicit credential instead of using the environment variable, use an alternate constructor like
    /// <see cref="ImageClient(string,ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </remarks>
    /// <param name="model"> The model name to use for image operations. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="InvalidOperationException"> The OPENAI_API_KEY environment variable was not found. </exception>
    public ImageClient(string model, OpenAIClientOptions options = default)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(), options),
              model,
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    // CUSTOM:
    // - Added `model` parameter.

    /// <summary> Initializes a new instance of EmbeddingClient. </summary>
    /// <param name="pipeline"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="model"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="endpoint"> OpenAI Endpoint. </param>
    protected internal ImageClient(ClientPipeline pipeline, string model, Uri endpoint, OpenAIClientOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        _pipeline = pipeline;
        _model = model;
        _endpoint = endpoint;
    }

    #region GenerateImages

    /// <summary>
    /// Generates an image based on a given prompt.
    /// </summary>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> Additional options to tailor the image generation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated image. </returns>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageAsync(string prompt, ImageGenerationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageGenerationOptions(prompt, null, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = await GenerateImagesAsync(content, (RequestOptions)null).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary>
    /// Generates an image based on a given prompt.
    /// </summary>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> Additional options to tailor the image generation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated image. </returns>
    public virtual ClientResult<GeneratedImage> GenerateImage(string prompt, ImageGenerationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageGenerationOptions(prompt, null, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = GenerateImages(content, (RequestOptions)null);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary>
    /// Generates images based on a given prompt.
    /// </summary>
    /// <param name="prompt"> A text description of the desired images. </param>
    /// <param name="imageCount"> The number of images to generate. </param>
    /// <param name="options"> Additional options to tailor the image generation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated images. </returns>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImagesAsync(string prompt, int imageCount, ImageGenerationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageGenerationOptions(prompt, imageCount, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = await GenerateImagesAsync(content, (RequestOptions)null).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary>
    /// Generates images based on a given prompt.
    /// </summary>
    /// <param name="prompt"> A text description of the desired images. </param>
    /// <param name="imageCount"> The number of images to generate. </param>
    /// <param name="options"> Additional options to tailor the image generation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated images. </returns>
    public virtual ClientResult<GeneratedImageCollection> GenerateImages(string prompt, int imageCount, ImageGenerationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageGenerationOptions(prompt, imageCount, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = GenerateImages(content, (RequestOptions)null);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    #endregion

    #region GenerateImageEdits

    /// <summary> Generates an edited or extended image given an original image and a prompt. </summary>
    /// <param name="image">
    /// The image to edit. Must be a valid PNG file, less than 4MB, and square. The image must have transparency, which
    /// will be used as the mask.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended image. </returns>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(Stream image, string imageFilename, string prompt, ImageEditOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, null, null, null, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, null, null);
        ClientResult result = await GenerateImageEditsAsync(content, content.ContentType).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates an edited or extended image given an original image and a prompt. </summary>
    /// <param name="image">
    /// The image to edit. Must be a valid PNG file, less than 4MB, and square. The image must have transparency, which
    /// will be used as the mask.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended image. </returns>
    public virtual ClientResult<GeneratedImage> GenerateImageEdit(Stream image, string imageFilename, string prompt, ImageEditOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, null, null, null, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, null, null);
        ClientResult result = GenerateImageEdits(content, content.ContentType);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates an edited or extended image given an original image and a prompt. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The image must have
    /// transparency, which will be used as the mask. The provided file path's extension (for example: .png) will be
    /// used to validate the format of the input image. The request may fail if the file extension and input image
    /// format do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended image. </returns>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(string imageFilePath, string prompt, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return await GenerateImageEditAsync(imageStream, imageFilePath, prompt, options).ConfigureAwait(false);
    }

    /// <summary> Generates an edited or extended image given an original image and a prompt. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The image must have
    /// transparency, which will be used as the mask. The provided file path's extension (for example: .png) will be
    /// used to validate the format of the input image. The request may fail if the file extension and input image
    /// format do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended image. </returns>
    public virtual ClientResult<GeneratedImage> GenerateImageEdit(string imageFilePath, string prompt, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return GenerateImageEdit(imageStream, imageFilePath, prompt,options);
    }

    /// <summary> Generates an edited or extended image given an original image, a prompt, and a mask. </summary>
    /// <param name="image">
    /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="mask">
    /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where the original image
    /// should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
    /// </param>
    /// <param name="maskFilename">
    /// The filename associated with the mask image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the mask image. The request may fail if the file extension and mask image format
    /// do not match.
    /// </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, <paramref name="prompt"/>, <paramref name="mask"/>, or <paramref name="maskFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/>, <paramref name="prompt"/>, or <paramref name="maskFilename"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended image. </returns>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, ImageEditOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNull(mask, nameof(mask));
        Argument.AssertNotNullOrEmpty(maskFilename, nameof(maskFilename));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, mask, maskFilename, null, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, mask, maskFilename);
        ClientResult result = await GenerateImageEditsAsync(content, content.ContentType).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates an edited or extended image given an original image, a prompt, and a mask. </summary>
    /// <param name="image">
    /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="mask">
    /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where the original image
    /// should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
    /// </param>
    /// <param name="maskFilename">
    /// The filename associated with the mask image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the mask image. The request may fail if the file extension and mask image format
    /// do not match.
    /// </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, <paramref name="prompt"/>, <paramref name="mask"/>, or <paramref name="maskFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/>, <paramref name="prompt"/>, or <paramref name="maskFilename"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended image. </returns>
    public virtual ClientResult<GeneratedImage> GenerateImageEdit(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, ImageEditOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNull(mask, nameof(mask));
        Argument.AssertNotNullOrEmpty(maskFilename, nameof(maskFilename));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, mask, maskFilename, null, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, mask, maskFilename);
        ClientResult result = GenerateImageEdits(content, content.ContentType);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates an edited or extended image given an original image, a prompt, and a mask. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The provided file
    /// path's extension (for example: .png) will be used to validate the format of the input image. The request may
    /// fail if the file extension and input image format do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="maskFilePath">
    /// The path of the mask image file whose fully transparent areas (e.g. where alpha is zero) indicate where the
    /// original image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as the
    /// original image. The provided file path's extension (for example: .png) will be used to validate the format of
    /// the input image. The request may fail if the file extension and mask image format do not match.
    /// </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/> or <paramref name="maskFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/>, or <paramref name="maskFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended image. </returns>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(string imageFilePath, string prompt, string maskFilePath, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNullOrEmpty(maskFilePath, nameof(maskFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        using FileStream maskStream = File.OpenRead(maskFilePath);
        return await GenerateImageEditAsync(imageStream, imageFilePath, prompt, maskStream, maskFilePath, options).ConfigureAwait(false);
    }

    /// <summary> Generates an edited or extended image given an original image, a prompt, and a mask. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The provided file
    /// path's extension (for example: .png) will be used to validate the format of the input image. The request may
    /// fail if the file extension and input image format do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="maskFilePath">
    /// The path of the mask image file whose fully transparent areas (e.g. where alpha is zero) indicate where the
    /// original image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as the
    /// original image. The provided file path's extension (for example: .png) will be used to validate the format of
    /// the input image. The request may fail if the file extension and mask image format do not match.
    /// </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/> or <paramref name="maskFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/>, or <paramref name="maskFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended image. </returns>
    public virtual ClientResult<GeneratedImage> GenerateImageEdit(string imageFilePath, string prompt, string maskFilePath, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNullOrEmpty(maskFilePath, nameof(maskFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        using FileStream maskStream = File.OpenRead(maskFilePath);
        return GenerateImageEdit(imageStream, imageFilePath, prompt, maskStream, maskFilePath, options);
    }

    /// <summary> Generates edited or extended images given an original image and a prompt. </summary>
    /// <param name="image">
    /// The image to edit. Must be a valid PNG file, less than 4MB, and square. The image must have transparency, which
    /// will be used as the mask.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="imageCount"> The number of edit or extended images to generate. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended images. </returns>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(Stream image, string imageFilename, string prompt, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, null, null, imageCount, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, null, null);
        ClientResult result = await GenerateImageEditsAsync(content, content.ContentType).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Generates edited or extended images given an original image and a prompt. </summary>
    /// <param name="image">
    /// The image to edit. Must be a valid PNG file, less than 4MB, and square. The image must have transparency, which
    /// will be used as the mask.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="imageCount"> The number of edit or extended images to generate. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended images. </returns>
    public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(Stream image, string imageFilename, string prompt, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, null, null, imageCount, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, null, null);
        ClientResult result = GenerateImageEdits(content, content.ContentType);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Generates edited or extended images given an original image and a prompt. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The image must have
    /// transparency, which will be used as the mask. The provided file path's extension (for example: .png) will be
    /// used to validate the format of the input image. The request may fail if the file extension and input image
    /// format do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="imageCount"> The number of edit or extended images to generate. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended images. </returns>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(string imageFilePath, string prompt, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return await GenerateImageEditsAsync(imageStream, imageFilePath, prompt, imageCount, options).ConfigureAwait(false);
    }

    /// <summary> Generates edited or extended images given an original image and a prompt. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The image must have
    /// transparency, which will be used as the mask. The provided file path's extension (for example: .png) will be
    /// used to validate the format of the input image. The request may fail if the file extension and input image
    /// format do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="imageCount"> The number of edit or extended images to generate. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> or <paramref name="prompt"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended images. </returns>
    public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(string imageFilePath, string prompt, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return GenerateImageEdits(imageStream, imageFilePath, prompt, imageCount, options);
    }

    /// <summary> Generates edited or extended images given an original image, a prompt, and a mask. </summary>
    /// <param name="image">
    /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="mask">
    /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where the original image
    /// should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
    /// </param>
    /// <param name="maskFilename">
    /// The filename associated with the mask image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the mask image. The request may fail if the file extension and mask image format
    /// do not match.
    /// </param>
    /// <param name="imageCount"> The number of edit or extended images to generate. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, <paramref name="prompt"/>, <paramref name="mask"/>, or <paramref name="maskFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/>, <paramref name="prompt"/>, or <paramref name="maskFilename"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended images. </returns>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNull(mask, nameof(mask));
        Argument.AssertNotNullOrEmpty(maskFilename, nameof(maskFilename));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, mask, maskFilename, imageCount, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, mask, maskFilename);
        ClientResult result = await GenerateImageEditsAsync(content, content.ContentType).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Generates edited or extended images given an original image, a prompt, and a mask. </summary>
    /// <param name="image">
    /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="mask">
    /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where the original image
    /// should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
    /// </param>
    /// <param name="maskFilename">
    /// The filename associated with the mask image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the mask image. The request may fail if the file extension and mask image format
    /// do not match.
    /// </param>
    /// <param name="imageCount"> The number of edit or extended images to generate. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/>, <paramref name="imageFilename"/>, <paramref name="prompt"/>, <paramref name="mask"/>, or <paramref name="maskFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/>, <paramref name="prompt"/>, or <paramref name="maskFilename"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended images. </returns>
    public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNull(mask, nameof(mask));
        Argument.AssertNotNullOrEmpty(maskFilename, nameof(maskFilename));

        options ??= new();
        CreateImageEditOptions(image, imageFilename, prompt, mask, maskFilename, imageCount, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename, mask, maskFilename);
        ClientResult result = GenerateImageEdits(content, content.ContentType);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Generates edited or extended images given an original image, a prompt, and a mask. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The provided file
    /// path's extension (for example: .png) will be used to validate the format of the input image. The request may
    /// fail if the file extension and input image format do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="maskFilePath">
    /// The path of the mask image file whose fully transparent areas (e.g. where alpha is zero) indicate where the
    /// original image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as the
    /// original image. The provided file path's extension (for example: .png) will be used to validate the format of
    /// the input image. The request may fail if the file extension and mask image format do not match.
    /// </param>
    /// <param name="imageCount"> The number of edit or extended images to generate. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/> or <paramref name="maskFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/>, or <paramref name="maskFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended images. </returns>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(string imageFilePath, string prompt, string maskFilePath, int imageCount, ImageEditOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));
        Argument.AssertNotNullOrEmpty(prompt, nameof(prompt));
        Argument.AssertNotNullOrEmpty(maskFilePath, nameof(maskFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        using FileStream maskStream = File.OpenRead(maskFilePath);
        return await GenerateImageEditsAsync(imageStream, imageFilePath, prompt, maskStream, maskFilePath, imageCount, options).ConfigureAwait(false);
    }

    /// <summary> Generates edited or extended images given an original image, a prompt, and a mask. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to edit. Must be a valid PNG file, less than 4MB, and square. The provided file
    /// path's extension (for example: .png) will be used to validate the format of the input image. The request may
    /// fail if the file extension and input image format do not match.
    /// </param>
    /// <param name="prompt"> A text description of the desired image. </param>
    /// <param name="maskFilePath">
    /// The path of the mask image file whose fully transparent areas (e.g. where alpha is zero) indicate where the
    /// original image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as the
    /// original image. The provided file path's extension (for example: .png) will be used to validate the format of
    /// the input image. The request may fail if the file extension and mask image format do not match.
    /// </param>
    /// <param name="imageCount"> The number of edit or extended images to generate. </param>
    /// <param name="options"> Additional options to tailor the image edit request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/> or <paramref name="maskFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/>, <paramref name="prompt"/>, or <paramref name="maskFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The edited or extended images. </returns>
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
    /// <param name="image">
    /// The image to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and square.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="options"> Additional options to tailor the image variation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/> or <paramref name="imageFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated image variation. </returns>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageVariationAsync(Stream image, string imageFilename, ImageVariationOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));

        options ??= new();
        CreateImageVariationOptions(image, imageFilename, null, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename);
        ClientResult result = await GenerateImageVariationsAsync(content, content.ContentType).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates a variation of a given image. </summary>
    /// <param name="image">
    /// The image to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and square.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="options"> Additional options to tailor the image variation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/> or <paramref name="imageFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated image variation. </returns>
    public virtual ClientResult<GeneratedImage> GenerateImageVariation(Stream image, string imageFilename, ImageVariationOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));

        options ??= new();
        CreateImageVariationOptions(image, imageFilename, null, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename);
        ClientResult result = GenerateImageVariations(content, content.ContentType);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Generates a variation of a given image. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and
    /// square. The provided file path's extension (for example: .png) will be used to validate the format of the input
    /// image. The request may fail if the file extension and input image format do not match.
    /// </param>
    /// <param name="options"> Additional options to tailor the image variation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated image variation. </returns>
    public virtual async Task<ClientResult<GeneratedImage>> GenerateImageVariationAsync(string imageFilePath, ImageVariationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return await GenerateImageVariationAsync(imageStream, imageFilePath, options).ConfigureAwait(false);
    }

    /// <summary> Generates a variation of a given image. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and
    /// square. The provided file path's extension (for example: .png) will be used to validate the format of the input
    /// image. The request may fail if the file extension and input image format do not match.
    /// </param>
    /// <param name="options"> Additional options to tailor the image variation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated image variation. </returns>
    public virtual ClientResult<GeneratedImage> GenerateImageVariation(string imageFilePath, ImageVariationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return GenerateImageVariation(imageStream, imageFilePath, options);
    }

    /// <summary> Generates variations of a given image. </summary>
    /// <param name="image">
    /// The image to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and square.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="imageCount"> The number of image variations to generate. </param>
    /// <param name="options"> Additional options to tailor the image variation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/> or <paramref name="imageFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated image variations. </returns>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageVariationsAsync(Stream image, string imageFilename, int imageCount, ImageVariationOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));

        options ??= new();
        CreateImageVariationOptions(image, imageFilename, imageCount, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename);
        ClientResult result = await GenerateImageVariationsAsync(content, content.ContentType).ConfigureAwait(false);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Generates variations of a given image. </summary>
    /// <param name="image">
    /// The image to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and square.
    /// </param>
    /// <param name="imageFilename">
    /// The filename associated with the image stream. The filename's extension (for example: .png) will be used to
    /// validate the format of the input image. The request may fail if the file extension and input image format do
    /// not match.
    /// </param>
    /// <param name="imageCount"> The number of image variations to generate. </param>
    /// <param name="options"> Additional options to tailor the image variation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="image"/> or <paramref name="imageFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilename"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated image variations. </returns>
    public virtual ClientResult<GeneratedImageCollection> GenerateImageVariations(Stream image, string imageFilename, int imageCount, ImageVariationOptions options = null)
    {
        Argument.AssertNotNull(image, nameof(image));
        Argument.AssertNotNullOrEmpty(imageFilename, nameof(imageFilename));

        options ??= new();
        CreateImageVariationOptions(image, imageFilename, imageCount, ref options);

        using MultipartFormDataBinaryContent content = options.ToMultipartContent(image, imageFilename);
        ClientResult result = GenerateImageVariations(content, content.ContentType);
        return ClientResult.FromValue(GeneratedImageCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Generates variations of a given image. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and
    /// square. The provided file path's extension (for example: .png) will be used to validate the format of the input
    /// image. The request may fail if the file extension and input image format do not match.
    /// </param>
    /// <param name="imageCount"> The number of image variations to generate. </param>
    /// <param name="options"> Additional options to tailor the image variation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> was null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated image variations. </returns>
    public virtual async Task<ClientResult<GeneratedImageCollection>> GenerateImageVariationsAsync(string imageFilePath, int imageCount, ImageVariationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(imageFilePath, nameof(imageFilePath));

        using FileStream imageStream = File.OpenRead(imageFilePath);
        return await GenerateImageVariationsAsync(imageStream, imageFilePath, imageCount, options).ConfigureAwait(false);
    }

    /// <summary> Generates variations of a given image. </summary>
    /// <param name="imageFilePath">
    /// The path of the image file to use as the basis for the variation. Must be a valid PNG file, less than 4MB, and
    /// square. The provided file path's extension (for example: .png) will be used to validate the format of the input
    /// image. The request may fail if the file extension and input image format do not match.
    /// </param>
    /// <param name="imageCount"> The number of image variations to generate. </param>
    /// <param name="options"> Additional options to tailor the image variation request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="imageFilePath"/> was null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="imageFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    /// <returns> The generated image variations. </returns>
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
