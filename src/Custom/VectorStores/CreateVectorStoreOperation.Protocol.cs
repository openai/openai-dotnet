using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

public partial class CreateVectorStoreOperation : OperationResult
{
    private readonly VectorStoreClient _parentClient;
    private readonly Uri _endpoint;

    private readonly string _vectorStoreId;

    /// <inheritdoc/>
    public override ContinuationToken? RehydrationToken { get; protected set; }

    /// <summary>
    /// [Protocol Method] Retrieves a vector store.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetVectorStoreAsync(RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateGetVectorStoreRequest(_vectorStoreId, options);
        return ClientResult.FromResponse(await _parentClient.Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a vector store.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetVectorStore(RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateGetVectorStoreRequest(_vectorStoreId, options);
        return ClientResult.FromResponse(_parentClient.Pipeline.ProcessMessage(message, options));
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}
