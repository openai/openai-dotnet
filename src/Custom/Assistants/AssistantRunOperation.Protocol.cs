using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

public class AssistantRunOperation // TODO: inherit from SCM ResultValueOperation type
{
    private readonly string _threadId;
    private readonly string _runId;
    private readonly InternalAssistantRunClient _runSubClient;

    internal AssistantRunOperation(string threadId, string runId, InternalAssistantRunClient runSubClient)
    {
        _threadId = threadId;
        _runId = runId;
        _runSubClient = runSubClient;
    }

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunAsync"/>
    public virtual Task<ClientResult> GetRunAsync(RequestOptions options)
        => _runSubClient.GetRunAsync(_threadId, _runId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.GetRun"/>
    public virtual ClientResult GetRun(RequestOptions options)
        => _runSubClient.GetRun(_threadId, _runId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.ModifyRunAsync"/>
    public virtual Task<ClientResult> ModifyRunAsync(BinaryContent content, RequestOptions options = null)
        => _runSubClient.ModifyRunAsync(_threadId, _runId, content, options);

    /// <inheritdoc cref="InternalAssistantRunClient.ModifyRun"/>
    public virtual ClientResult ModifyRun(BinaryContent content, RequestOptions options = null)
        => _runSubClient.ModifyRun(_threadId, _runId, content, options);

    /// <inheritdoc cref="InternalAssistantRunClient.CancelRunAsync"/>
    public virtual Task<ClientResult> CancelRunAsync(RequestOptions options)
        => _runSubClient.CancelRunAsync(_threadId, _runId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.CancelRun"/>
    public virtual ClientResult CancelRun(RequestOptions options)
        => _runSubClient.CancelRun(_threadId, _runId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.SubmitToolOutputsToRunAsync"/>
    public virtual Task<ClientResult> SubmitToolOutputsToRunAsync(BinaryContent content, RequestOptions options = null)
        => _runSubClient.SubmitToolOutputsToRunAsync(_threadId, _runId, content, options);

    /// <inheritdoc cref="InternalAssistantRunClient.SubmitToolOutputsToRun"/>
    public virtual ClientResult SubmitToolOutputsToRun(BinaryContent content, RequestOptions options = null)
        => _runSubClient.SubmitToolOutputsToRun(_threadId, _runId, content, options);

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunStepsAsync"/>
    public virtual Task<ClientResult> GetRunStepsAsync(int? limit, string order, string after, string before, RequestOptions options)
        => _runSubClient.GetRunStepsAsync(_threadId, _runId, limit, order, after, before, options);

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunSteps"/>
    public virtual ClientResult GetRunSteps(int? limit, string order, string after, string before, RequestOptions options)
        => _runSubClient.GetRunSteps(_threadId, _runId, limit, order, after, before, options);

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunStepAsync"/>
    public virtual Task<ClientResult> GetRunStepAsync(string stepId, RequestOptions options)
        => _runSubClient.GetRunStepAsync(_threadId, _runId, stepId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunStep"/>
    public virtual ClientResult GetRunStep(string stepId, RequestOptions options)
        => _runSubClient.GetRunStep(_threadId, _runId, stepId, options);

}
