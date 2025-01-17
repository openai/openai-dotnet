using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace OpenAI.Files;

[CodeGenSuppress("CreateFileAsync", typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CreateFile", typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("ListFilesAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("ListFiles", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("RetrieveFileAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("RetrieveFile", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DeleteFileAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DeleteFile", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DownloadFileAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DownloadFile", typeof(string), typeof(RequestOptions))]
public partial class OpenAIFileClient
{
    /// <summary>
    /// [Protocol Method] Upload a file that can be used across various endpoints. The size of all the files uploaded by
    /// one organization can be up to 100 GB.
    ///
    /// The size of individual files can be a maximum of 512 MB or 2 million tokens for Assistants. See
    /// the <see href="https://platform.openai.com/docs/assistants/tools">Assistants Tools guide</see> to
    /// learn more about the types of files supported. The Fine-tuning API only supports `.jsonl` files.
    ///
    /// Please <see href="https://help.openai.com/">contact us</see> if you need to increase these
    /// storage limits.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="contentType"> The content type of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> or <paramref name="contentType"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="contentType"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> UploadFileAsync(BinaryContent content, string contentType, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));
        Argument.AssertNotNullOrEmpty(contentType, nameof(contentType));

        using PipelineMessage message = CreateCreateFileRequest(content, contentType, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Upload a file that can be used across various endpoints. The size of all the files uploaded by
    /// one organization can be up to 100 GB.
    ///
    /// The size of individual files can be a maximum of 512 MB or 2 million tokens for Assistants. See
    /// the <see href="https://platform.openai.com/docs/assistants/tools">Assistants Tools guide</see> to
    /// learn more about the types of files supported. The Fine-tuning API only supports `.jsonl` files.
    ///
    /// Please <see href="https://help.openai.com/">contact us</see> if you need to increase these
    /// storage limits.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="contentType"> The content type of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> or <paramref name="contentType"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="contentType"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult UploadFile(BinaryContent content, string contentType, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));
        Argument.AssertNotNullOrEmpty(contentType, nameof(contentType));

        using PipelineMessage message = CreateCreateFileRequest(content, contentType, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a list of files that belong to the user's organization.
    /// </summary>
    /// <param name="purpose"> Only return files with the given purpose. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> GetFilesAsync(string purpose, RequestOptions options)
    {
        using PipelineMessage message = CreateListFilesRequest(purpose, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a list of files that belong to the user's organization.
    /// </summary>
    /// <param name="purpose"> Only return files with the given purpose. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult GetFiles(string purpose, RequestOptions options)
    {
        using PipelineMessage message = CreateListFilesRequest(purpose, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Retrieves information about a specified file.
    /// </summary>
    /// <param name="fileId"> The ID of the file to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> GetFileAsync(string fileId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        using PipelineMessage message = CreateRetrieveFileRequest(fileId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves information about a specified file.
    /// </summary>
    /// <param name="fileId"> The ID of the file to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult GetFile(string fileId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        using PipelineMessage message = CreateRetrieveFileRequest(fileId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Deletes a previously uploaded file.
    /// </summary>
    /// <param name="fileId"> The ID of the file to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> DeleteFileAsync(string fileId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        using PipelineMessage message = CreateDeleteFileRequest(fileId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Deletes a previously uploaded file.
    /// </summary>
    /// <param name="fileId"> The ID of the file to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult DeleteFile(string fileId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        using PipelineMessage message = CreateDeleteFileRequest(fileId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Downloads the binary content of the specified file.
    /// </summary>
    /// <param name="fileId"> The ID of the file to download. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> DownloadFileAsync(string fileId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        using PipelineMessage message = CreateDownloadFileRequest(fileId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Downloads the binary content of the specified file.
    /// </summary>
    /// <param name="fileId"> The ID of the file to download. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult DownloadFile(string fileId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        using PipelineMessage message = CreateDownloadFileRequest(fileId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Creates an intermediate upload to which data can be added in chunks of bytes. An upload
    /// can accept at most 8 GB in total and expires an hour after it is created.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [Experimental("OPENAI001")]
    public virtual async Task<ClientResult> CreateUploadAsync(BinaryContent content, RequestOptions options = null)
    {
        return await _internalUploadsClient.CreateUploadAsync(content, options).ConfigureAwait(false);
    }

    /// <summary>
    /// [Protocol Method] Creates an intermediate upload to which data can be added in chunks of bytes. An upload
    /// can accept at most 8 GB in total and expires an hour after it is created.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [Experimental("OPENAI001")]
    public virtual ClientResult CreateUpload(BinaryContent content, RequestOptions options = null)
    {
        return _internalUploadsClient.CreateUpload(content, options);
    }

    /// <summary>
    /// [Protocol Method] Adds a chunk of bytes to an existing upload. Each part can contain at most 64 MB, while the
    /// upload can accept at most 8 GB in total.
    /// </summary>
    /// <param name="uploadId"> The ID of the upload. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="uploadId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="uploadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [Experimental("OPENAI001")]
    public virtual async Task<ClientResult> AddUploadPartAsync(string uploadId, BinaryContent content, string contentType, RequestOptions options = null)
    {
        return await _internalUploadsClient.AddUploadPartAsync(uploadId, content, contentType, options).ConfigureAwait(false);
    }

    /// <summary>
    /// [Protocol Method] Adds a chunk of bytes to an existing upload. Each part can contain at most 64 MB, while the
    /// upload can accept at most 8 GB in total.
    /// </summary>
    /// <param name="uploadId"> The ID of the upload. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="uploadId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="uploadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [Experimental("OPENAI001")]
    public virtual ClientResult AddUploadPart(string uploadId, BinaryContent content, string contentType, RequestOptions options = null)
    {
        return _internalUploadsClient.AddUploadPart(uploadId, content, contentType, options);
    }

    /// <summary>
    /// [Protocol Method] Completes an existing upload, creating a file ready to use.
    /// </summary>
    /// <param name="uploadId"> The ID of the upload. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="uploadId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="uploadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [Experimental("OPENAI001")]
    public virtual async Task<ClientResult> CompleteUploadAsync(string uploadId, BinaryContent content, RequestOptions options = null)
    {
        return await _internalUploadsClient.CompleteUploadAsync(uploadId, content, options).ConfigureAwait(false);
    }

    /// <summary>
    /// [Protocol Method] Completes an existing upload, creating a file ready to use.
    /// </summary>
    /// <param name="uploadId"> The ID of the upload. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="uploadId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="uploadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [Experimental("OPENAI001")]
    public virtual ClientResult CompleteUpload(string uploadId, BinaryContent content, RequestOptions options = null)
    {
        return _internalUploadsClient.CompleteUpload(uploadId, content, options);
    }

    /// <summary>
    /// [Protocol Method] Cancels an existing upload.
    /// </summary>
    /// <param name="uploadId"> The ID of the upload. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="uploadId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="uploadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [Experimental("OPENAI001")]
    public virtual async Task<ClientResult> CancelUploadAsync(string uploadId, RequestOptions options = null)
    {
        return await _internalUploadsClient.CancelUploadAsync(uploadId, options).ConfigureAwait(false);
    }

    /// <summary>
    /// [Protocol Method] Cancels an existing upload.
    /// </summary>
    /// <param name="uploadId"> The ID of the upload. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="uploadId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="uploadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [Experimental("OPENAI001")]
    public virtual ClientResult CancelUpload(string uploadId, RequestOptions options = null)
    {
        return _internalUploadsClient.CancelUpload(uploadId, options);
    }
}
