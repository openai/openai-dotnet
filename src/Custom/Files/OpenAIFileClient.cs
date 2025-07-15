using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Files;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
/// <summary> The service client for OpenAI file operations. </summary>
[CodeGenType("Files")]
[CodeGenSuppress("OpenAIFileClient", typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress("GetFilesAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetFiles", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetFileAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetFile", typeof(string), typeof(CancellationToken))]
public partial class OpenAIFileClient
{
    private InternalUploadsClient _internalUploadsClient;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="OpenAIFileClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public OpenAIFileClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="OpenAIFileClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public OpenAIFileClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="OpenAIFileClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public OpenAIFileClient(ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        Pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
        _internalUploadsClient = new(Pipeline, options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="OpenAIFileClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal OpenAIFileClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
        _internalUploadsClient = new(pipeline, options);
    }

    /// <summary> Uploads a file that can be used across various operations. </summary>
    /// <remarks> Individual files can be up to 512 MB, and the size of all files uploaded by one organization can be up to 100 GB. </remarks>
    /// <param name="file"> The file stream to upload. </param>
    /// <param name="filename">
    ///     The filename associated with the file stream. The filename's extension (for example: .json) will be used to
    ///     validate the file format. The request may fail if the filename's extension and the actual file format do
    ///     not match.
    /// </param>
    /// <param name="purpose"> The intended purpose of the uploaded file. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="file"/> or <paramref name="filename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="filename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<OpenAIFile>> UploadFileAsync(Stream file, string filename, FileUploadPurpose purpose, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(file, nameof(file));
        Argument.AssertNotNullOrEmpty(filename, nameof(filename));

        InternalFileUploadOptions options = new()
        {
            Purpose = purpose
        };

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(file, filename);
        ClientResult result = await UploadFileAsync(content, content.ContentType, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(OpenAIFile.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Uploads a file that can be used across various operations. </summary>
    /// <remarks> Individual files can be up to 512 MB, and the size of all files uploaded by one organization can be up to 100 GB. </remarks>
    /// <param name="file"> The file stream to upload. </param>
    /// <param name="filename">
    ///     The filename associated with the file stream. The filename's extension (for example: .json) will be used to
    ///     validate the file format. The request may fail if the filename's extension and the actual file format do
    ///     not match.
    /// </param>
    /// <param name="purpose"> The intended purpose of the uploaded file. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="file"/> or <paramref name="filename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="filename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<OpenAIFile> UploadFile(Stream file, string filename, FileUploadPurpose purpose, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(file, nameof(file));
        Argument.AssertNotNullOrEmpty(filename, nameof(filename));

        InternalFileUploadOptions options = new()
        {
            Purpose = purpose
        };

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(file, filename);
        ClientResult result = UploadFile(content, content.ContentType, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(OpenAIFile.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Uploads a file that can be used across various operations. </summary>
    /// <remarks> Individual files can be up to 512 MB, and the size of all files uploaded by one organization can be up to 100 GB. </remarks>
    /// <param name="file"> The file bytes to upload. </param>
    /// <param name="filename">
    ///     The filename associated with the file bytes. The filename's extension (for example: .json) will be used to
    ///     validate the file format. The request may fail if the filename's extension and the actual file format do
    ///     not match.
    /// </param>
    /// <param name="purpose"> The intended purpose of the uploaded file. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="file"/> or <paramref name="filename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="filename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual Task<ClientResult<OpenAIFile>> UploadFileAsync(BinaryData file, string filename, FileUploadPurpose purpose)
    {
        Argument.AssertNotNull(file, nameof(file));
        Argument.AssertNotNullOrEmpty(filename, nameof(filename));

        return UploadFileAsync(file?.ToStream(), filename, purpose);
    }

    /// <summary> Uploads a file that can be used across various operations. </summary>
    /// <remarks> Individual files can be up to 512 MB, and the size of all files uploaded by one organization can be up to 100 GB. </remarks>
    /// <param name="file"> The file bytes to upload. </param>
    /// <param name="filename">
    ///     The filename associated with the file bytes. The filename's extension (for example: .json) will be used to
    ///     validate the file format. The request may fail if the filename's extension and the actual file format do
    ///     not match.
    /// </param>
    /// <param name="purpose"> The intended purpose of the uploaded file. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="file"/> or <paramref name="filename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="filename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<OpenAIFile> UploadFile(BinaryData file, string filename, FileUploadPurpose purpose)
    {
        Argument.AssertNotNull(file, nameof(file));
        Argument.AssertNotNullOrEmpty(filename, nameof(filename));

        return UploadFile(file?.ToStream(), filename, purpose);
    }

    /// <summary> Uploads a file that can be used across various operations. </summary>
    /// <remarks> Individual files can be up to 512 MB, and the size of all files uploaded by one organization can be up to 100 GB. </remarks>
    /// <param name="filePath">
    ///     The path of the file to upload. The provided file path's extension (for example: .json) will be used to
    ///     validate the file format. The request may fail if the file path's extension and the actual file format do
    ///     not match.
    /// </param>
    /// <param name="purpose"> The intended purpose of the uploaded file. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="filePath"/> was null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<OpenAIFile>> UploadFileAsync(string filePath, FileUploadPurpose purpose)
    {
        Argument.AssertNotNullOrEmpty(filePath, nameof(filePath));

        using FileStream stream = File.OpenRead(filePath);
        return await UploadFileAsync(stream, filePath, purpose).ConfigureAwait(false);
    }

    /// <summary> Uploads a file that can be used across various operations. </summary>
    /// <remarks> Individual files can be up to 512 MB, and the size of all files uploaded by one organization can be up to 100 GB. </remarks>
    /// <param name="filePath">
    ///     The path of the file to upload. The provided file path's extension (for example: .json) will be used to
    ///     validate the file format. The request may fail if the file path's extension and the actual file format do
    ///     not match.
    /// </param>
    /// <param name="purpose"> The intended purpose of the uploaded file. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="filePath"/> was null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<OpenAIFile> UploadFile(string filePath, FileUploadPurpose purpose)
    {
        Argument.AssertNotNullOrEmpty(filePath, nameof(filePath));

        using FileStream stream = File.OpenRead(filePath);
        return UploadFile(stream, filePath, purpose);
    }

    /// <summary> Gets basic information about each of the files belonging to the user's organization. </summary>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    public virtual async Task<ClientResult<OpenAIFileCollection>> GetFilesAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await GetFilesAsync(null, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(OpenAIFileCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Gets basic information about each of the files belonging to the user's organization. </summary>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    public virtual ClientResult<OpenAIFileCollection> GetFiles(CancellationToken cancellationToken = default)
    {
        ClientResult result = GetFiles(null, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(OpenAIFileCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Gets basic information about each of the files belonging to the user's organization. </summary>
    /// <param name="purpose"> Only return files with the given purpose. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    public virtual async Task<ClientResult<OpenAIFileCollection>> GetFilesAsync(FilePurpose purpose, CancellationToken cancellationToken = default)
    {
        ClientResult result = await GetFilesAsync(purpose.ToSerialString(), cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(OpenAIFileCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Gets basic information about each of the files belonging to the user's organization. </summary>
    /// <param name="purpose"> Only return files with the given purpose. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    public virtual ClientResult<OpenAIFileCollection> GetFiles(FilePurpose purpose, CancellationToken cancellationToken = default)
    {
        ClientResult result = GetFiles(purpose.ToSerialString(), cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(OpenAIFileCollection.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Gets basic information about the specified file. </summary>
    /// <param name="fileId"> The ID of the desired file. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<OpenAIFile>> GetFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        ClientResult result = await GetFileAsync(fileId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(OpenAIFile.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Gets basic information about the specified file. </summary>
    /// <param name="fileId"> The ID of the desired file. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<OpenAIFile> GetFile(string fileId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        ClientResult result = GetFile(fileId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(OpenAIFile.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Deletes the specified file. </summary>
    /// <param name="fileId"> The ID of the file to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<FileDeletionResult>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        ClientResult result = await DeleteFileAsync(fileId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(FileDeletionResult.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Deletes the specified file. </summary>
    /// <param name="fileId"> The ID of the file to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<FileDeletionResult> DeleteFile(string fileId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        ClientResult result = DeleteFile(fileId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(FileDeletionResult.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary> Downloads the content of the specified file. </summary>
    /// <param name="fileId"> The ID of the file to download. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<BinaryData>> DownloadFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        ClientResult result = await DownloadFileAsync(fileId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(result.GetRawResponse().Content, result.GetRawResponse());
    }

    /// <summary> Downloads the content of the specified file. </summary>
    /// <param name="fileId"> The ID of the file to download. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<BinaryData> DownloadFile(string fileId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        ClientResult result = DownloadFile(fileId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(result.GetRawResponse().Content, result.GetRawResponse());
    }
}
