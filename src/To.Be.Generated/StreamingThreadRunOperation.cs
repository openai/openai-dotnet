using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

public partial class StreamingThreadRunOperation : OperationResult
{



    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    /// </summary>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    public virtual AsyncCollectionResult<StreamingUpdate> SubmitToolOutputsToRunStreamingAsync(
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        BinaryContent content = new InternalSubmitToolOutputsRunRequest(toolOutputs.ToList(), stream: true, null)
            .ToBinaryContent();

        async Task<ClientResult> getResultAsync() =>
            await SubmitToolOutputsToRunAsync(content, cancellationToken.ToRequestOptions(streaming: true))
            .ConfigureAwait(false);

        return new AsyncStreamingUpdateCollection(getResultAsync);
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    /// </summary>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    public virtual CollectionResult<StreamingUpdate> SubmitToolOutputsToRunStreaming(
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        BinaryContent content = new InternalSubmitToolOutputsRunRequest(toolOutputs.ToList(), stream: true, null)
            .ToBinaryContent();

        ClientResult getResult() => SubmitToolOutputsToRun(content, cancellationToken.ToRequestOptions(streaming: true));

        return new StreamingUpdateCollection(getResult);
    }
}
