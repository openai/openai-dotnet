using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Assistants;

// CUSTOM:
//  - Required actions are abstracted into a forward-compatible, strongly-typed conceptual
//    hierarchy and formatted into a more intuitive collection for the consumer.

[Experimental("OPENAI001")]
[CodeGenModel("RunObject")]
public partial class ThreadRun
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.run`. </summary>
    [CodeGenMember("Object")]
    internal InternalRunObjectObject Object { get; } = InternalRunObjectObject.ThreadRun;


    [CodeGenMember("RequiredAction")]
    internal readonly InternalRunRequiredAction _internalRequiredAction;

    // CUSTOM: Removed null check for `toolConstraint` and `responseFormat`.
    internal ThreadRun(string id, DateTimeOffset createdAt, string threadId, string assistantId, RunStatus status, InternalRunRequiredAction internalRequiredAction, RunError lastError, DateTimeOffset? expiresAt, DateTimeOffset? startedAt, DateTimeOffset? cancelledAt, DateTimeOffset? failedAt, DateTimeOffset? completedAt, RunIncompleteDetails incompleteDetails, string model, string instructions, IEnumerable<ToolDefinition> tools, IReadOnlyDictionary<string, string> metadata, RunTokenUsage usage, int? maxPromptTokens, int? maxCompletionTokens, RunTruncationStrategy truncationStrategy, ToolConstraint toolConstraint, bool? parallelToolCallsEnabled, AssistantResponseFormat responseFormat)
    {
        Argument.AssertNotNull(id, nameof(id));
        Argument.AssertNotNull(threadId, nameof(threadId));
        Argument.AssertNotNull(assistantId, nameof(assistantId));
        Argument.AssertNotNull(model, nameof(model));
        Argument.AssertNotNull(instructions, nameof(instructions));
        Argument.AssertNotNull(tools, nameof(tools));

        Id = id;
        CreatedAt = createdAt;
        ThreadId = threadId;
        AssistantId = assistantId;
        Status = status;
        _internalRequiredAction = internalRequiredAction;
        LastError = lastError;
        ExpiresAt = expiresAt;
        StartedAt = startedAt;
        CancelledAt = cancelledAt;
        FailedAt = failedAt;
        CompletedAt = completedAt;
        IncompleteDetails = incompleteDetails;
        Model = model;
        Instructions = instructions;
        Tools = tools.ToList();
        Metadata = metadata;
        Usage = usage;
        MaxPromptTokens = maxPromptTokens;
        MaxCompletionTokens = maxCompletionTokens;
        TruncationStrategy = truncationStrategy;
        ToolConstraint = toolConstraint;
        ParallelToolCallsEnabled = parallelToolCallsEnabled;
        ResponseFormat = responseFormat;
    }


    /// <summary>
    /// The list of required actions that must have their results submitted for the run to continue.
    /// </summary>
    /// <remarks>
    /// <see cref="Assistants.RequiredAction"/> is the abstract base type for all required actions. Its
    /// concrete type can be one of:
    /// <list type="bullet">
    /// <item> <see cref="InternalRequiredFunctionToolCall"/> </item> 
    /// </list>
    /// </remarks>
    public IReadOnlyList<RequiredAction> RequiredActions => _internalRequiredAction?.SubmitToolOutputs?.ToolCalls ?? [];

    /// <inheritdoc cref="AssistantResponseFormat"/>
    [CodeGenMember("ResponseFormat")]
    public AssistantResponseFormat ResponseFormat { get; }

    [CodeGenMember("ToolChoice")]
    public ToolConstraint ToolConstraint { get; }

    /// <summary>
    /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
    ///
    /// We generally recommend altering this or temperature but not both.
    /// </summary>
    [CodeGenMember("TopP")]
    public float? NucleusSamplingFactor { get; }

    /// <summary>
    /// Whether parallel function calling is enabled during tool use for the thread. 
    /// </summary>
    /// <remarks>
    /// Assumed <c>true</c> if not otherwise specified.
    /// </remarks>
    [CodeGenMember("ParallelToolCalls")]
    public bool? ParallelToolCallsEnabled { get; }

}
