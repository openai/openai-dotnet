using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace OpenAI.Files;

public partial class OpenAIFileClient
{
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
