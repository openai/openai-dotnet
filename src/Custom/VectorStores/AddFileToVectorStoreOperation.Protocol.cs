using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

public partial class AddFileToVectorStoreOperation : OperationResult
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    private readonly string _vectorStoreId;
    private readonly string _fileId;

    /// <inheritdoc/>
    public override ContinuationToken? RehydrationToken { get; protected set; }

    /// <summary>
    /// [Protocol Method] Retrieves a vector store file.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> GetFileAssociationAsync(RequestOptions? options)
    {
        using PipelineMessage message = CreateGetVectorStoreFileRequest(_vectorStoreId, _fileId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a vector store file.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult GetFileAssociation(RequestOptions? options)
    {
        using PipelineMessage message = CreateGetVectorStoreFileRequest(_vectorStoreId, _fileId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    internal virtual PipelineMessage CreateGetVectorStoreFileRequest(string vectorStoreId, string fileId, RequestOptions? options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/vector_stores/", false);
        uri.AppendPath(vectorStoreId, true);
        uri.AppendPath("/files/", false);
        uri.AppendPath(fileId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}
