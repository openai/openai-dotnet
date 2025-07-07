using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

namespace OpenAI.Evals;

[CodeGenSuppress("GetEvals", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetEvalsAsync", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CreateEval", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateEvalAsync", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("GetEval", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetEvalAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("UpdateEval", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("UpdateEvalAsync", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("DeleteEval", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DeleteEvalAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetEvalRuns", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetEvalRunsAsync", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CreateEvalRun", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateEvalRunAsync", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("GetEvalRun", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetEvalRunAsync", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CancelEvalRun", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CancelEvalRunAsync", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DeleteEvalRun", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DeleteEvalRunAsync", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetEvalRunOutputItems", typeof(string), typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetEvalRunOutputItemsAsync", typeof(string), typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetEvalRunOutputItem", typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetEvalRunOutputItemAsync", typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
public partial class EvaluationClient
{
    public virtual ClientResult GetEvaluations(int? limit, string orderBy, string order, string after, RequestOptions options)
    {
        using PipelineMessage message = CreateGetEvalsRequest(after, limit, order, orderBy, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> GetEvaluationsAsync(int? limit, string orderBy, string order, string after, RequestOptions options)
    {
        using PipelineMessage message = CreateGetEvalsRequest(after, limit, order, orderBy, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult CreateEvaluation(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateEvalRequest(content, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> CreateEvaluationAsync(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateEvalRequest(content, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult GetEvaluation(string evaluationId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));

        using PipelineMessage message = CreateGetEvalRequest(evaluationId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> GetEvaluationAsync(string evaluationId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));

        using PipelineMessage message = CreateGetEvalRequest(evaluationId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult UpdateEvaluation(string evaluationId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateUpdateEvalRequest(evaluationId, content, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> UpdateEvaluationAsync(string evaluationId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateUpdateEvalRequest(evaluationId, content, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult DeleteEvaluation(string evaluationId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));

        using PipelineMessage message = CreateDeleteEvalRequest(evaluationId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> DeleteEvaluationAsync(string evaluationId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));

        using PipelineMessage message = CreateDeleteEvalRequest(evaluationId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult GetEvaluationRuns(string evaluationId, int? limit, string order, string after, string evaluationRunStatus, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));

        using PipelineMessage message = CreateGetEvalRunsRequest(evaluationId, after, limit, order, evaluationRunStatus, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> GetEvaluationRunsAsync(string evaluationId, int? limit,  string order, string after, string evaluationRunStatus, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));

        using PipelineMessage message = CreateGetEvalRunsRequest(evaluationId, after, limit, order, evaluationRunStatus, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult CreateEvaluationRun(string evaluationId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateEvalRunRequest(evaluationId, content, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> CreateEvaluationRunAsync(string evaluationId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateEvalRunRequest(evaluationId, content, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult GetEvaluationRun(string evaluationId, string evaluationRunId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(evaluationRunId, nameof(evaluationRunId));

        using PipelineMessage message = CreateGetEvalRunRequest(evaluationId, evaluationRunId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> GetEvaluationRunAsync(string evaluationId, string evaluationRunId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(evaluationRunId, nameof(evaluationRunId));

        using PipelineMessage message = CreateGetEvalRunRequest(evaluationId, evaluationRunId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult CancelEvaluationRun(string evaluationId, string evaluationRunId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(evaluationRunId, nameof(evaluationRunId));

        using PipelineMessage message = CreateCancelEvalRunRequest(evaluationId, evaluationRunId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> CancelEvaluationRunAsync(string evaluationId, string evaluationRunId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(evaluationRunId, nameof(evaluationRunId));

        using PipelineMessage message = CreateCancelEvalRunRequest(evaluationId, evaluationRunId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult DeleteEvaluationRun(string evaluationId, string evaluationRunId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(evaluationRunId, nameof(evaluationRunId));

        using PipelineMessage message = CreateDeleteEvalRunRequest(evaluationId, evaluationRunId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> DeleteEvaluationRunAsync(string evaluationId, string evaluationRunId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(evaluationRunId, nameof(evaluationRunId));

        using PipelineMessage message = CreateDeleteEvalRunRequest(evaluationId, evaluationRunId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult GetEvaluationRunOutputItems(string evaluationId, string evaluationRunId, int? limit, string order, string after, string outputItemStatus, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(evaluationRunId, nameof(evaluationRunId));

        using PipelineMessage message = CreateGetEvalRunOutputItemsRequest(evaluationId, evaluationRunId, after, limit, outputItemStatus, order, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> GetEvaluationRunOutputItemsAsync(string evaluationId, string evaluationRunId, int? limit, string order, string after, string outputItemStatus, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(evaluationRunId, nameof(evaluationRunId));

        using PipelineMessage message = CreateGetEvalRunOutputItemsRequest(evaluationId, evaluationRunId, after, limit, outputItemStatus, order, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult GetEvaluationRunOutputItem(string evaluationId, string evaluationRunId, string outputItemId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(evaluationRunId, nameof(evaluationRunId));
        Argument.AssertNotNull(outputItemId, nameof(outputItemId));

        using PipelineMessage message = CreateGetEvalRunOutputItemRequest(evaluationId, evaluationRunId, outputItemId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> GetEvaluationRunOutputItemAsync(string evaluationId, string evaluationRunId, string outputItemId, RequestOptions options)
    {
        Argument.AssertNotNull(evaluationId, nameof(evaluationId));
        Argument.AssertNotNull(evaluationRunId, nameof(evaluationRunId));
        Argument.AssertNotNull(outputItemId, nameof(outputItemId));

        using PipelineMessage message = CreateGetEvalRunOutputItemRequest(evaluationId, evaluationRunId, outputItemId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }
}
