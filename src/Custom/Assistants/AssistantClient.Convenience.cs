using System.ClientModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

public partial class AssistantClient
{
    /// <summary>
    /// Gets an updated instance of an existing <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The <see cref="Assistant"/> instance to retrieve an updated representation of. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated instance of the <see cref="Assistant"/>. </returns>
    public virtual Task<ClientResult<Assistant>> GetAssistantAsync(Assistant assistant, CancellationToken cancellationToken = default)
        => GetAssistantAsync(assistant?.Id, cancellationToken);

    /// <summary>
    /// Gets an updated instance of an existing <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The <see cref="Assistant"/> instance to retrieve an updated representation of. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated instance of the <see cref="Assistant"/>. </returns>
    public virtual ClientResult<Assistant> GetAssistant(Assistant assistant, CancellationToken cancellationToken = default)
        => GetAssistant(assistant?.Id, cancellationToken);

    /// <summary>
    /// Modifies an existing <see cref="Assistant"/>. 
    /// </summary>
    /// <param name="assistant"> The assistant to modify. </param>
    /// <param name="options"> The changes to apply to the assistant. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns>
    /// An updated <see cref="Assistant"/> instance that reflects the requested changes. 
    /// </returns>
    public virtual Task<ClientResult<Assistant>> ModifyAssistantAsync(Assistant assistant, AssistantModificationOptions options, CancellationToken cancellationToken = default)
        => ModifyAssistantAsync(assistant?.Id, options, cancellationToken);

    /// <summary>
    /// Modifies an existing <see cref="Assistant"/>. 
    /// </summary>
    /// <param name="assistant"> The assistant to modify. </param>
    /// <param name="options"> The changes to apply to the assistant. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns>
    /// An updated <see cref="Assistant"/> instance that reflects the requested changes. 
    /// </returns>
    public virtual ClientResult<Assistant> ModifyAssistant(Assistant assistant, AssistantModificationOptions options, CancellationToken cancellationToken = default)
        => ModifyAssistant(assistant?.Id, options, cancellationToken);

    /// <summary>
    /// Deletes an existing <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The assistant to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual Task<ClientResult<bool>> DeleteAssistantAsync(Assistant assistant, CancellationToken cancellationToken = default)
        => DeleteAssistantAsync(assistant?.Id, cancellationToken);

    /// <summary>
    /// Deletes an existing <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The assistant to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual ClientResult<bool> DeleteAssistant(Assistant assistant, CancellationToken cancellationToken = default)
        => DeleteAssistant(assistant?.Id, cancellationToken);

    /// <summary>
    /// Gets an updated instance of an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The existing thread to refresh the state of. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated instance of the provided <see cref="ThreadMessage"/>. </returns>
    public virtual Task<ClientResult<AssistantThread>> GetThreadAsync(AssistantThread thread, CancellationToken cancellationToken = default)
        => GetThreadAsync(thread?.Id, cancellationToken);

    /// <summary>
    /// Gets an updated instance of an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The existing thread to refresh the state of. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated instance of the provided <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<AssistantThread> GetThread(AssistantThread thread, CancellationToken cancellationToken = default)
        => GetThread(thread?.Id, cancellationToken);

    /// <summary>
    /// Modifies an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to modify. </param>
    /// <param name="options"> The modifications to apply to the thread. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The updated <see cref="AssistantThread"/> instance. </returns>
    public virtual Task<ClientResult<AssistantThread>> ModifyThreadAsync(AssistantThread thread, ThreadModificationOptions options, CancellationToken cancellationToken = default)
        => ModifyThreadAsync(thread?.Id, options, cancellationToken);

    /// <summary>
    /// Modifies an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to modify. </param>
    /// <param name="options"> The modifications to apply to the thread. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The updated <see cref="AssistantThread"/> instance. </returns>
    public virtual ClientResult<AssistantThread> ModifyThread(AssistantThread thread, ThreadModificationOptions options, CancellationToken cancellationToken = default)
        => ModifyThread(thread?.Id, options, cancellationToken);

    /// <summary>
    /// Deletes an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual Task<ClientResult<bool>> DeleteThreadAsync(AssistantThread thread, CancellationToken cancellationToken = default)
        => DeleteThreadAsync(thread?.Id, cancellationToken);

    /// <summary>
    /// Deletes an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual ClientResult<bool> DeleteThread(AssistantThread thread, CancellationToken cancellationToken = default)
        => DeleteThread(thread?.Id, cancellationToken);

    /// <summary>
    /// Creates a new <see cref="ThreadMessage"/> on an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to associate the new message with. </param>
    /// <param name="content"> The collection of <see cref="MessageContent"/> items for the message. </param>
    /// <param name="options"> Additional options to apply to the new message. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A new <see cref="ThreadMessage"/>. </returns>
    public virtual Task<ClientResult<ThreadMessage>> CreateMessageAsync(
        AssistantThread thread,
        IEnumerable<MessageContent> content,
        MessageCreationOptions options = null, CancellationToken cancellationToken = default)
            => CreateMessageAsync(thread?.Id, content, options, cancellationToken);

    /// <summary>
    /// Creates a new <see cref="ThreadMessage"/> on an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to associate the new message with. </param>
    /// <param name="content"> The collection of <see cref="MessageContent"/> items for the message. </param>
    /// <param name="options"> Additional options to apply to the new message. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A new <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<ThreadMessage> CreateMessage(
        AssistantThread thread,
        IEnumerable<MessageContent> content,
        MessageCreationOptions options = null, CancellationToken cancellationToken = default)
            => CreateMessage(thread?.Id, content, options, cancellationToken);

    /// <summary>
    /// Returns a collection of <see cref="ThreadMessage"/> instances from an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to list messages from. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A collection of messages that can be enumerated using <c>await foreach</c>. </returns>
    public virtual AsyncPageableCollection<ThreadMessage> GetMessagesAsync(
        AssistantThread thread,
        ListOrder? resultOrder = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(thread, nameof(thread));

        return GetMessagesAsync(thread.Id, resultOrder, cancellationToken);
    }

    /// <summary>
    /// Returns a collection of <see cref="ThreadMessage"/> instances from an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to list messages from. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A collection of messages that can be enumerated using <c>foreach</c>. </returns>
    public virtual PageableCollection<ThreadMessage> GetMessages(
        AssistantThread thread, 
        ListOrder? resultOrder = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(thread, nameof(thread));

        return GetMessages(thread.Id, resultOrder, cancellationToken);
    }

    /// <summary>
    /// Gets an updated instance of an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The existing message to refresh the state of. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated instance of the provided <see cref="ThreadMessage"/>. </returns>
    public virtual Task<ClientResult<ThreadMessage>> GetMessageAsync(ThreadMessage message, CancellationToken cancellationToken = default)
        => GetMessageAsync(message?.ThreadId, message?.Id, cancellationToken);

    /// <summary>
    /// Gets an updated instance of an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The existing message to refresh the state of. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated instance of the provided <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<ThreadMessage> GetMessage(ThreadMessage message, CancellationToken cancellationToken = default)
        => GetMessage(message?.ThreadId, message?.Id, cancellationToken);

    /// <summary>
    /// Modifies an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The message to modify. </param>
    /// <param name="options"> The changes to apply to the message. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The updated <see cref="ThreadMessage"/>. </returns>
    public virtual Task<ClientResult<ThreadMessage>> ModifyMessageAsync(ThreadMessage message, MessageModificationOptions options, CancellationToken cancellationToken = default)
        => ModifyMessageAsync(message?.ThreadId, message?.Id, options, cancellationToken);

    /// <summary>
    /// Modifies an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The message to modify. </param>
    /// <param name="options"> The changes to apply to the message. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The updated <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<ThreadMessage> ModifyMessage(ThreadMessage message, MessageModificationOptions options, CancellationToken cancellationToken = default)
        => ModifyMessage(message?.ThreadId, message?.Id, options, cancellationToken);

    /// <summary>
    /// Deletes an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The message to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual Task<ClientResult<bool>> DeleteMessageAsync(ThreadMessage message, CancellationToken cancellationToken = default)
        => DeleteMessageAsync(message?.ThreadId, message?.Id, cancellationToken);

    /// <summary>
    /// Deletes an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The message to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual ClientResult<bool> DeleteMessage(ThreadMessage message, CancellationToken cancellationToken = default)
        => DeleteMessage(message?.ThreadId, message?.Id, cancellationToken);

    /// <summary>
    /// Begins a new <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="thread"> The thread that the run should evaluate. </param>
    /// <param name="assistant"> The assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A new <see cref="ThreadRun"/> instance. </returns>
    public virtual Task<ClientResult<ThreadRun>> CreateRunAsync(AssistantThread thread, Assistant assistant, RunCreationOptions options = null, CancellationToken cancellationToken = default)
        => CreateRunAsync(thread?.Id, assistant?.Id, options, cancellationToken);

    /// <summary>
    /// Begins a new <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="thread"> The thread that the run should evaluate. </param>
    /// <param name="assistant"> The assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A new <see cref="ThreadRun"/> instance. </returns>
    public virtual ClientResult<ThreadRun> CreateRun(AssistantThread thread, Assistant assistant, RunCreationOptions options = null, CancellationToken cancellationToken = default)
        => CreateRun(thread?.Id, assistant?.Id, options, cancellationToken);

    /// <summary>
    /// Begins a new streaming <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="thread"> The thread that the run should evaluate. </param>
    /// <param name="assistant"> The assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    public virtual AsyncResultCollection<StreamingUpdate> CreateRunStreamingAsync(
        AssistantThread thread,
        Assistant assistant,
        RunCreationOptions options = null, CancellationToken cancellationToken = default)
            => CreateRunStreamingAsync(thread?.Id, assistant?.Id, options, cancellationToken);

    /// <summary>
    /// Begins a new streaming <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="thread"> The thread that the run should evaluate. </param>
    /// <param name="assistant"> The assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    public virtual ResultCollection<StreamingUpdate> CreateRunStreaming(
        AssistantThread thread,
        Assistant assistant,
        RunCreationOptions options = null, CancellationToken cancellationToken = default)
            => CreateRunStreaming(thread?.Id, assistant?.Id, options, cancellationToken);

    /// <summary>
    /// Creates a new thread and immediately begins a run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A new <see cref="ThreadRun"/>. </returns>
    public virtual Task<ClientResult<ThreadRun>> CreateThreadAndRunAsync(
        Assistant assistant,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null, CancellationToken cancellationToken = default)
            => CreateThreadAndRunAsync(assistant?.Id, threadOptions, runOptions, cancellationToken);

    /// <summary>
    /// Creates a new thread and immediately begins a run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A new <see cref="ThreadRun"/>. </returns>
    public virtual ClientResult<ThreadRun> CreateThreadAndRun(
        Assistant assistant,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null, CancellationToken cancellationToken = default)
            => CreateThreadAndRun(assistant?.Id, threadOptions, runOptions, cancellationToken);

    /// <summary>
    /// Creates a new thread and immediately begins a streaming run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    public virtual AsyncResultCollection<StreamingUpdate> CreateThreadAndRunStreamingAsync(
        Assistant assistant,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null, CancellationToken cancellationToken = default)
            => CreateThreadAndRunStreamingAsync(assistant?.Id, threadOptions, runOptions, cancellationToken);

    /// <summary>
    /// Creates a new thread and immediately begins a streaming run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    public virtual ResultCollection<StreamingUpdate> CreateThreadAndRunStreaming(
        Assistant assistant,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null, CancellationToken cancellationToken = default)
            => CreateThreadAndRunStreaming(assistant?.Id, threadOptions, runOptions, cancellationToken);

    /// <summary>
    /// Returns a collection of <see cref="ThreadRun"/> instances associated with an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread that runs in the list should be associated with. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A collection of runs that can be enumerated using <c>await foreach</c>. </returns>
    public virtual AsyncPageableCollection<ThreadRun> GetRunsAsync(
        AssistantThread thread,
        ListOrder? resultOrder = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(thread, nameof(thread));

        return GetRunsAsync(thread.Id, resultOrder, cancellationToken);
    }

    /// <summary>
    /// Returns a collection of <see cref="ThreadRun"/> instances associated with an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread that runs in the list should be associated with. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A collection of runs that can be enumerated using <c>foreach</c>. </returns>
    public virtual PageableCollection<ThreadRun> GetRuns(
        AssistantThread thread,
        ListOrder? resultOrder = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(thread, nameof(thread));

        return GetRuns(thread.Id, resultOrder, cancellationToken);
    }

    /// <summary>
    /// Gets a refreshed instance of an existing <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to get a refreshed instance of. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A new <see cref="ThreadRun"/> instance with updated information. </returns>
    public virtual Task<ClientResult<ThreadRun>> GetRunAsync(ThreadRun run, CancellationToken cancellationToken = default)
        => GetRunAsync(run?.ThreadId, run?.Id, cancellationToken);

    /// <summary>
    /// Gets a refreshed instance of an existing <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to get a refreshed instance of. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A new <see cref="ThreadRun"/> instance with updated information. </returns>
    public virtual ClientResult<ThreadRun> GetRun(ThreadRun run, CancellationToken cancellationToken = default)
        => GetRun(run?.ThreadId, run?.Id, cancellationToken);

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="run"> The run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual Task<ClientResult<ThreadRun>> SubmitToolOutputsToRunAsync(
        ThreadRun run,
        IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default)
            => SubmitToolOutputsToRunAsync(run?.ThreadId, run?.Id, toolOutputs, cancellationToken);

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="run"> The run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual ClientResult<ThreadRun> SubmitToolOutputsToRun(
        ThreadRun run,
        IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default)
            => SubmitToolOutputsToRun(run?.ThreadId, run?.Id, toolOutputs, cancellationToken);

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    /// </summary>
    /// <param name="run"> The run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    public virtual AsyncResultCollection<StreamingUpdate> SubmitToolOutputsToRunStreamingAsync(
        ThreadRun run,
        IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default)
            => SubmitToolOutputsToRunStreamingAsync(run?.ThreadId, run?.Id, toolOutputs, cancellationToken);

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    /// </summary>
    /// <param name="run"> The run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    public virtual ResultCollection<StreamingUpdate> SubmitToolOutputsToRunStreaming(
        ThreadRun run,
        IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default)
            => SubmitToolOutputsToRunStreaming(run?.ThreadId, run?.Id, toolOutputs, cancellationToken);

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to cancel. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual Task<ClientResult<ThreadRun>> CancelRunAsync(ThreadRun run, CancellationToken cancellationToken = default)
        => CancelRunAsync(run?.ThreadId, run?.Id, cancellationToken);

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to cancel. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual ClientResult<ThreadRun> CancelRun(ThreadRun run, CancellationToken cancellationToken = default)
        => CancelRun(run?.ThreadId, run?.Id, cancellationToken);

    /// <summary>
    /// Gets a collection of <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to list run steps from. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A collection of run steps that can be enumerated using <c>await foreach</c>. </returns>
    public virtual PageableCollection<RunStep> GetRunSteps(
        ThreadRun run,
        ListOrder? resultOrder = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(run, nameof(run));

        return GetRunSteps(run.ThreadId, run.Id, resultOrder, cancellationToken);
    }

    /// <summary>
    /// Gets a collection of <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="run"> The run to list run steps from. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A collection of run steps that can be enumerated using <c>foreach</c>. </returns>
    public virtual AsyncPageableCollection<RunStep> GetRunStepsAsync(
        ThreadRun run,
        ListOrder? resultOrder = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(run, nameof(run));

        return GetRunStepsAsync(run.ThreadId, run.Id, resultOrder, cancellationToken);
    }
}
