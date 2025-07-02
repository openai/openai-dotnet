using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

[CodeGenSuppress("CreateRunAsync", typeof(string), typeof(BinaryContent), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(RequestOptions))]
[CodeGenSuppress("CreateRun", typeof(string), typeof(BinaryContent), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(RequestOptions))]
[CodeGenSuppress("CreateThreadAndRunAsync", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateThreadAndRun", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("SubmitToolOutputsToRunAsync", typeof(string), typeof(string), typeof(string), typeof(BinaryContent),typeof(RequestOptions))]
[CodeGenSuppress("SubmitToolOutputsToRun", typeof(string), typeof(string), typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("GetRunStepsAsync", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(RequestOptions))]
[CodeGenSuppress("GetRunSteps", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(RequestOptions))]
[CodeGenSuppress("GetRunStepAsync", typeof(string), typeof(string), typeof(string), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(RequestOptions))]
[CodeGenSuppress("GetRunStep", typeof(string), typeof(string), typeof(string), typeof(IEnumerable<InternalIncludedRunStepProperty>), typeof(RequestOptions))]
internal partial class InternalAssistantRunClient
{
    /// <summary>
    /// [Protocol Method] Create a thread and run it in one request.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CreateThreadAndRunAsync(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage message = null;
        try
        {
            message = CreateCreateThreadAndRunRequest(content, options);
            return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] Create a thread and run it in one request.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CreateThreadAndRun(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage message = null;
        try
        {
            message = CreateCreateThreadAndRunRequest(content, options);
            return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] Create a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to run. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CreateRunAsync(string threadId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage message = null;
        try
        {
            // Always request the included properties.
            IEnumerable<InternalIncludedRunStepProperty> includedRunStepProperties = [InternalIncludedRunStepProperty.FileSearchResultContent];

            message = CreateCreateRunRequest(threadId, content, includedRunStepProperties, options);
            return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] Create a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to run. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CreateRun(string threadId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage message = null;
        try
        {
            // Always request the included properties.
            IEnumerable<InternalIncludedRunStepProperty> includedRunStepProperties = [InternalIncludedRunStepProperty.FileSearchResultContent];

            message = CreateCreateRunRequest(threadId, content, includedRunStepProperties, options);
            return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) that was run. </param>
    /// <param name="runId"> The ID of the run to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunAsync(string threadId, string runId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        using PipelineMessage message = CreateGetRunRequest(threadId, runId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) that was run. </param>
    /// <param name="runId"> The ID of the run to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRun(string threadId, string runId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        using PipelineMessage message = CreateGetRunRequest(threadId, runId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Modifies a run.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) that was run. </param>
    /// <param name="runId"> The ID of the run to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> ModifyRunAsync(string threadId, string runId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyRunRequest(threadId, runId, content, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Modifies a run.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) that was run. </param>
    /// <param name="runId"> The ID of the run to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult ModifyRun(string threadId, string runId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyRunRequest(threadId, runId, content, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Cancels a run that is `in_progress`.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which this run belongs. </param>
    /// <param name="runId"> The ID of the run to cancel. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CancelRunAsync(string threadId, string runId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        using PipelineMessage message = CreateCancelRunRequest(threadId, runId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Cancels a run that is `in_progress`.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which this run belongs. </param>
    /// <param name="runId"> The ID of the run to cancel. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CancelRun(string threadId, string runId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        using PipelineMessage message = CreateCancelRunRequest(threadId, runId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] When a run has the `status: "requires_action"` and `required_action.type` is
    /// `submit_tool_outputs`, this endpoint can be used to submit the outputs from the tool calls once
    /// they're all completed. All outputs must be submitted in a single request.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) to which this run belongs. </param>
    /// <param name="runId"> The ID of the run that requires the tool output submission. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> SubmitToolOutputsToRunAsync(string threadId, string runId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage message = null;
        try
        {
            message = CreateSubmitToolOutputsToRunRequest(threadId, runId, content, options);
            return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] When a run has the `status: "requires_action"` and `required_action.type` is
    /// `submit_tool_outputs`, this endpoint can be used to submit the outputs from the tool calls once
    /// they're all completed. All outputs must be submitted in a single request.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) to which this run belongs. </param>
    /// <param name="runId"> The ID of the run that requires the tool output submission. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult SubmitToolOutputsToRun(string threadId, string runId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage message = null;
        try
        {
            message = CreateSubmitToolOutputsToRunRequest(threadId, runId, content, options);
            return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message.Dispose();
            }
        }
    }

    public virtual async Task<ClientResult> GetRunStepsAsync(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        // Always request the included properties.
        IEnumerable<InternalIncludedRunStepProperty> includedRunStepProperties = [InternalIncludedRunStepProperty.FileSearchResultContent];

        using PipelineMessage message = CreateGetRunStepsRequest(threadId, runId, limit, order, after, before, includedRunStepProperties, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult GetRunSteps(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        // Always request the included properties.
        IEnumerable<InternalIncludedRunStepProperty> includedRunStepProperties = [InternalIncludedRunStepProperty.FileSearchResultContent];

        using PipelineMessage message = CreateGetRunStepsRequest(threadId, runId, limit, order, after, before, includedRunStepProperties, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run step.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which the run and run step belongs. </param>
    /// <param name="runId"> The ID of the run to which the run step belongs. </param>
    /// <param name="stepId"> The ID of the run step to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="stepId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="stepId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunStepAsync(string threadId, string runId, string stepId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNullOrEmpty(stepId, nameof(stepId));

        // Always request the included properties.
        IEnumerable<InternalIncludedRunStepProperty> includedRunStepProperties = [InternalIncludedRunStepProperty.FileSearchResultContent];

        using PipelineMessage message = CreateGetRunStepRequest(threadId, runId, stepId, includedRunStepProperties, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run step.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which the run and run step belongs. </param>
    /// <param name="runId"> The ID of the run to which the run step belongs. </param>
    /// <param name="stepId"> The ID of the run step to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="stepId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/>, <paramref name="runId"/> or <paramref name="stepId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRunStep(string threadId, string runId, string stepId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));
        Argument.AssertNotNullOrEmpty(stepId, nameof(stepId));

        // Always request the included properties.
        IEnumerable<InternalIncludedRunStepProperty> includedRunStepProperties = [InternalIncludedRunStepProperty.FileSearchResultContent];

        using PipelineMessage message = CreateGetRunStepRequest(threadId, runId, stepId, includedRunStepProperties, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }
}
