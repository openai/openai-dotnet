namespace OpenAI {
    public class OpenAIClient {
        protected OpenAIClient();
        public OpenAIClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public OpenAIClient(ApiKeyCredential credential);
        protected internal OpenAIClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public OpenAIClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        [Experimental("OPENAI001")]
        public virtual AssistantClient GetAssistantClient();
        public virtual AudioClient GetAudioClient(string model);
        [Experimental("OPENAI001")]
        public virtual BatchClient GetBatchClient();
        public virtual ChatClient GetChatClient(string model);
        public virtual EmbeddingClient GetEmbeddingClient(string model);
        [Experimental("OPENAI001")]
        public virtual FineTuningClient GetFineTuningClient();
        public virtual ImageClient GetImageClient(string model);
        public virtual ModerationClient GetModerationClient(string model);
        public virtual OpenAIFileClient GetOpenAIFileClient();
        public virtual OpenAIModelClient GetOpenAIModelClient();
        [Experimental("OPENAI002")]
        public virtual RealtimeConversation.RealtimeConversationClient GetRealtimeConversationClient(string model);
        [Experimental("OPENAI001")]
        public virtual VectorStoreClient GetVectorStoreClient();
    }
    public class OpenAIClientOptions : ClientPipelineOptions {
        public Uri Endpoint { get; set; }
        public string OrganizationId { get; set; }
        public string ProjectId { get; set; }
        public string UserAgentApplicationId { get; set; }
    }
}
namespace OpenAI.Assistants {
    [Experimental("OPENAI001")]
    public class Assistant : IJsonModel<Assistant>, IPersistableModel<Assistant> {
        public DateTimeOffset CreatedAt { get; }
        public string Description { get; }
        public string Id { get; }
        public string Instructions { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public string Model { get; }
        public string Name { get; }
        public float? NucleusSamplingFactor { get; }
        public AssistantResponseFormat ResponseFormat { get; }
        public float? Temperature { get; }
        public ToolResources ToolResources { get; }
        public IReadOnlyList<ToolDefinition> Tools { get; }
        public static explicit operator Assistant(ClientResult result);
        public static implicit operator BinaryContent(Assistant assistant);
    }
    [Experimental("OPENAI001")]
    public class AssistantClient {
        protected AssistantClient();
        public AssistantClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public AssistantClient(ApiKeyCredential credential);
        protected internal AssistantClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public AssistantClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult CancelRun(string threadId, string runId, RequestOptions options);
        public virtual ClientResult<ThreadRun> CancelRun(string threadId, string runId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> CancelRunAsync(string threadId, string runId, RequestOptions options);
        public virtual Task<ClientResult<ThreadRun>> CancelRunAsync(string threadId, string runId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult CreateAssistant(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<Assistant> CreateAssistant(string model, AssistantCreationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> CreateAssistantAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<Assistant>> CreateAssistantAsync(string model, AssistantCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<ThreadMessage> CreateMessage(string threadId, MessageRole role, IEnumerable<MessageContent> content, MessageCreationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult CreateMessage(string threadId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ThreadMessage>> CreateMessageAsync(string threadId, MessageRole role, IEnumerable<MessageContent> content, MessageCreationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> CreateMessageAsync(string threadId, BinaryContent content, RequestOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult CreateRun(string threadId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ThreadRun> CreateRun(string threadId, string assistantId, RunCreationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> CreateRunAsync(string threadId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ThreadRun>> CreateRunAsync(string threadId, string assistantId, RunCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingUpdate> CreateRunStreaming(string threadId, string assistantId, RunCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingUpdate> CreateRunStreamingAsync(string threadId, string assistantId, RunCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<AssistantThread> CreateThread(ThreadCreationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult CreateThread(BinaryContent content, RequestOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult CreateThreadAndRun(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ThreadRun> CreateThreadAndRun(string assistantId, ThreadCreationOptions threadOptions = null, RunCreationOptions runOptions = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> CreateThreadAndRunAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ThreadRun>> CreateThreadAndRunAsync(string assistantId, ThreadCreationOptions threadOptions = null, RunCreationOptions runOptions = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingUpdate> CreateThreadAndRunStreaming(string assistantId, ThreadCreationOptions threadOptions = null, RunCreationOptions runOptions = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingUpdate> CreateThreadAndRunStreamingAsync(string assistantId, ThreadCreationOptions threadOptions = null, RunCreationOptions runOptions = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<AssistantThread>> CreateThreadAsync(ThreadCreationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> CreateThreadAsync(BinaryContent content, RequestOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult DeleteAssistant(string assistantId, RequestOptions options);
        public virtual ClientResult<AssistantDeletionResult> DeleteAssistant(string assistantId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> DeleteAssistantAsync(string assistantId, RequestOptions options);
        public virtual Task<ClientResult<AssistantDeletionResult>> DeleteAssistantAsync(string assistantId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult DeleteMessage(string threadId, string messageId, RequestOptions options);
        public virtual ClientResult<MessageDeletionResult> DeleteMessage(string threadId, string messageId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> DeleteMessageAsync(string threadId, string messageId, RequestOptions options);
        public virtual Task<ClientResult<MessageDeletionResult>> DeleteMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult DeleteThread(string threadId, RequestOptions options);
        public virtual ClientResult<ThreadDeletionResult> DeleteThread(string threadId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> DeleteThreadAsync(string threadId, RequestOptions options);
        public virtual Task<ClientResult<ThreadDeletionResult>> DeleteThreadAsync(string threadId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetAssistant(string assistantId, RequestOptions options);
        public virtual ClientResult<Assistant> GetAssistant(string assistantId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetAssistantAsync(string assistantId, RequestOptions options);
        public virtual Task<ClientResult<Assistant>> GetAssistantAsync(string assistantId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<Assistant> GetAssistants(AssistantCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<Assistant> GetAssistants(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CollectionResult GetAssistants(int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<Assistant> GetAssistantsAsync(AssistantCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<Assistant> GetAssistantsAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AsyncCollectionResult GetAssistantsAsync(int? limit, string order, string after, string before, RequestOptions options);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetMessage(string threadId, string messageId, RequestOptions options);
        public virtual ClientResult<ThreadMessage> GetMessage(string threadId, string messageId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetMessageAsync(string threadId, string messageId, RequestOptions options);
        public virtual Task<ClientResult<ThreadMessage>> GetMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ThreadMessage> GetMessages(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ThreadMessage> GetMessages(string threadId, MessageCollectionOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CollectionResult GetMessages(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<ThreadMessage> GetMessagesAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<ThreadMessage> GetMessagesAsync(string threadId, MessageCollectionOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AsyncCollectionResult GetMessagesAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetRun(string threadId, string runId, RequestOptions options);
        public virtual ClientResult<ThreadRun> GetRun(string threadId, string runId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetRunAsync(string threadId, string runId, RequestOptions options);
        public virtual Task<ClientResult<ThreadRun>> GetRunAsync(string threadId, string runId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ThreadRun> GetRuns(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ThreadRun> GetRuns(string threadId, RunCollectionOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CollectionResult GetRuns(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<ThreadRun> GetRunsAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<ThreadRun> GetRunsAsync(string threadId, RunCollectionOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AsyncCollectionResult GetRunsAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetRunStep(string threadId, string runId, string stepId, RequestOptions options);
        public virtual ClientResult<RunStep> GetRunStep(string threadId, string runId, string stepId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetRunStepAsync(string threadId, string runId, string stepId, RequestOptions options);
        public virtual Task<ClientResult<RunStep>> GetRunStepAsync(string threadId, string runId, string stepId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<RunStep> GetRunSteps(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult<RunStep> GetRunSteps(string threadId, string runId, RunStepCollectionOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CollectionResult GetRunSteps(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<RunStep> GetRunStepsAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<RunStep> GetRunStepsAsync(string threadId, string runId, RunStepCollectionOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AsyncCollectionResult GetRunStepsAsync(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetThread(string threadId, RequestOptions options);
        public virtual ClientResult<AssistantThread> GetThread(string threadId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetThreadAsync(string threadId, RequestOptions options);
        public virtual Task<ClientResult<AssistantThread>> GetThreadAsync(string threadId, CancellationToken cancellationToken = default);
        public virtual ClientResult<Assistant> ModifyAssistant(string assistantId, AssistantModificationOptions options, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult ModifyAssistant(string assistantId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<Assistant>> ModifyAssistantAsync(string assistantId, AssistantModificationOptions options, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> ModifyAssistantAsync(string assistantId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ThreadMessage> ModifyMessage(string threadId, string messageId, MessageModificationOptions options, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult ModifyMessage(string threadId, string messageId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ThreadMessage>> ModifyMessageAsync(string threadId, string messageId, MessageModificationOptions options, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> ModifyMessageAsync(string threadId, string messageId, BinaryContent content, RequestOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult ModifyRun(string threadId, string runId, BinaryContent content, RequestOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> ModifyRunAsync(string threadId, string runId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<AssistantThread> ModifyThread(string threadId, ThreadModificationOptions options, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult ModifyThread(string threadId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<AssistantThread>> ModifyThreadAsync(string threadId, ThreadModificationOptions options, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> ModifyThreadAsync(string threadId, BinaryContent content, RequestOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult SubmitToolOutputsToRun(string threadId, string runId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ThreadRun> SubmitToolOutputsToRun(string threadId, string runId, IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> SubmitToolOutputsToRunAsync(string threadId, string runId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ThreadRun>> SubmitToolOutputsToRunAsync(string threadId, string runId, IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingUpdate> SubmitToolOutputsToRunStreaming(string threadId, string runId, IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingUpdate> SubmitToolOutputsToRunStreamingAsync(string threadId, string runId, IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default);
    }
    [Experimental("OPENAI001")]
    public class AssistantCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public AssistantCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    [Experimental("OPENAI001")]
    public readonly partial struct AssistantCollectionOrder : IEquatable<AssistantCollectionOrder> {
        public AssistantCollectionOrder(string value);
        public static AssistantCollectionOrder Ascending { get; }
        public static AssistantCollectionOrder Descending { get; }
        public readonly bool Equals(AssistantCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(AssistantCollectionOrder left, AssistantCollectionOrder right);
        public static implicit operator AssistantCollectionOrder(string value);
        public static bool operator !=(AssistantCollectionOrder left, AssistantCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class AssistantCreationOptions : IJsonModel<AssistantCreationOptions>, IPersistableModel<AssistantCreationOptions> {
        public string Description { get; set; }
        public string Instructions { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public string Name { get; set; }
        public float? NucleusSamplingFactor { get; set; }
        public AssistantResponseFormat ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        public ToolResources ToolResources { get; set; }
        public IList<ToolDefinition> Tools { get; }
        public static explicit operator AssistantCreationOptions(ClientResult result);
        public static implicit operator BinaryContent(AssistantCreationOptions assistantCreationOptions);
    }
    [Experimental("OPENAI001")]
    public class AssistantDeletionResult : IJsonModel<AssistantDeletionResult>, IPersistableModel<AssistantDeletionResult> {
        public string AssistantId { get; }
        public bool Deleted { get; }
        public static explicit operator AssistantDeletionResult(ClientResult result);
        public static implicit operator BinaryContent(AssistantDeletionResult assistantDeletionResult);
    }
    [Experimental("OPENAI001")]
    public class AssistantModificationOptions : IJsonModel<AssistantModificationOptions>, IPersistableModel<AssistantModificationOptions> {
        public IList<ToolDefinition> DefaultTools { get; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public string Model { get; set; }
        public string Name { get; set; }
        public float? NucleusSamplingFactor { get; set; }
        public AssistantResponseFormat ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        public ToolResources ToolResources { get; set; }
        public static explicit operator AssistantModificationOptions(ClientResult result);
        public static implicit operator BinaryContent(AssistantModificationOptions assistantModificationOptions);
    }
    [Experimental("OPENAI001")]
    public abstract class AssistantResponseFormat : IEquatable<AssistantResponseFormat>, IEquatable<string>, IJsonModel<AssistantResponseFormat>, IPersistableModel<AssistantResponseFormat> {
        public static AssistantResponseFormat Auto { get; }
        public static AssistantResponseFormat JsonObject { get; }
        public static AssistantResponseFormat Text { get; }
        public static AssistantResponseFormat CreateAutoFormat();
        public static AssistantResponseFormat CreateJsonObjectFormat();
        public static AssistantResponseFormat CreateJsonSchemaFormat(string name, BinaryData jsonSchema, string description = null, bool? strictSchemaEnabled = null);
        public static AssistantResponseFormat CreateTextFormat();
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode();
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator ==(AssistantResponseFormat first, AssistantResponseFormat second);
        public static explicit operator AssistantResponseFormat(ClientResult result);
        public static implicit operator BinaryContent(AssistantResponseFormat assistantResponseFormat);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static implicit operator AssistantResponseFormat(string plainTextFormat);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator !=(AssistantResponseFormat first, AssistantResponseFormat second);
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool IEquatable<AssistantResponseFormat>.Equals(AssistantResponseFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool IEquatable<string>.Equals(string other);
        public override string ToString();
    }
    [Experimental("OPENAI001")]
    public class AssistantThread : IJsonModel<AssistantThread>, IPersistableModel<AssistantThread> {
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public ToolResources ToolResources { get; }
        public static explicit operator AssistantThread(ClientResult result);
        public static implicit operator BinaryContent(AssistantThread assistantThread);
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterToolDefinition : ToolDefinition, IJsonModel<CodeInterpreterToolDefinition>, IPersistableModel<CodeInterpreterToolDefinition> {
        public new static explicit operator CodeInterpreterToolDefinition(ClientResult result);
        public static implicit operator BinaryContent(CodeInterpreterToolDefinition codeInterpreterToolDefinition);
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterToolResources : IJsonModel<CodeInterpreterToolResources>, IPersistableModel<CodeInterpreterToolResources> {
        public IList<string> FileIds { get; }
        public static explicit operator CodeInterpreterToolResources(ClientResult result);
        public static implicit operator BinaryContent(CodeInterpreterToolResources codeInterpreterToolResources);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct FileSearchRanker : IEquatable<FileSearchRanker> {
        public FileSearchRanker(string value);
        public static FileSearchRanker Auto { get; }
        public static FileSearchRanker Default20240821 { get; }
        public readonly bool Equals(FileSearchRanker other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(FileSearchRanker left, FileSearchRanker right);
        public static implicit operator FileSearchRanker(string value);
        public static bool operator !=(FileSearchRanker left, FileSearchRanker right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class FileSearchRankingOptions : IJsonModel<FileSearchRankingOptions>, IPersistableModel<FileSearchRankingOptions> {
        public FileSearchRankingOptions();
        [SetsRequiredMembers]
        public FileSearchRankingOptions(float scoreThreshold);
        public FileSearchRanker? Ranker { get; set; }
        public required float ScoreThreshold { get; set; }
        public static explicit operator FileSearchRankingOptions(ClientResult result);
        public static implicit operator BinaryContent(FileSearchRankingOptions fileSearchRankingOptions);
    }
    [Experimental("OPENAI001")]
    public class FileSearchToolDefinition : ToolDefinition, IJsonModel<FileSearchToolDefinition>, IPersistableModel<FileSearchToolDefinition> {
        public int? MaxResults { get; set; }
        public FileSearchRankingOptions RankingOptions { get; set; }
        public new static explicit operator FileSearchToolDefinition(ClientResult result);
        public static implicit operator BinaryContent(FileSearchToolDefinition fileSearchToolDefinition);
    }
    [Experimental("OPENAI001")]
    public class FileSearchToolResources : IJsonModel<FileSearchToolResources>, IPersistableModel<FileSearchToolResources> {
        public IList<VectorStoreCreationHelper> NewVectorStores { get; }
        public IList<string> VectorStoreIds { get; }
        public static explicit operator FileSearchToolResources(ClientResult result);
        public static implicit operator BinaryContent(FileSearchToolResources fileSearchToolResources);
    }
    [Experimental("OPENAI001")]
    public class FunctionToolDefinition : ToolDefinition, IJsonModel<FunctionToolDefinition>, IPersistableModel<FunctionToolDefinition> {
        public FunctionToolDefinition();
        [SetsRequiredMembers]
        public FunctionToolDefinition(string name);
        public string Description { get; set; }
        public required string FunctionName { get; set; }
        public BinaryData Parameters { get; set; }
        public bool? StrictParameterSchemaEnabled { get; set; }
        public new static explicit operator FunctionToolDefinition(ClientResult result);
        public static implicit operator BinaryContent(FunctionToolDefinition functionToolDefinition);
    }
    [Experimental("OPENAI001")]
    public class MessageCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public MessageCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    [Experimental("OPENAI001")]
    public readonly partial struct MessageCollectionOrder : IEquatable<MessageCollectionOrder> {
        public MessageCollectionOrder(string value);
        public static MessageCollectionOrder Ascending { get; }
        public static MessageCollectionOrder Descending { get; }
        public readonly bool Equals(MessageCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(MessageCollectionOrder left, MessageCollectionOrder right);
        public static implicit operator MessageCollectionOrder(string value);
        public static bool operator !=(MessageCollectionOrder left, MessageCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public abstract class MessageContent : IJsonModel<MessageContent>, IPersistableModel<MessageContent> {
        public MessageImageDetail? ImageDetail { get; }
        public string ImageFileId { get; }
        public Uri ImageUri { get; }
        public string Refusal { get; }
        public string Text { get; }
        public IReadOnlyList<TextAnnotation> TextAnnotations { get; }
        public static MessageContent FromImageFileId(string imageFileId, MessageImageDetail? detail = null);
        public static MessageContent FromImageUri(Uri imageUri, MessageImageDetail? detail = null);
        public static MessageContent FromText(string text);
        public static explicit operator MessageContent(ClientResult result);
        public static implicit operator BinaryContent(MessageContent messageContent);
        public static implicit operator MessageContent(string value);
    }
    [Experimental("OPENAI001")]
    public class MessageContentUpdate : StreamingUpdate {
        public MessageImageDetail? ImageDetail { get; }
        public string ImageFileId { get; }
        public string MessageId { get; }
        public int MessageIndex { get; }
        public string RefusalUpdate { get; }
        public MessageRole? Role { get; }
        public string Text { get; }
        public TextAnnotationUpdate TextAnnotation { get; }
    }
    [Experimental("OPENAI001")]
    public class MessageCreationAttachment : IJsonModel<MessageCreationAttachment>, IPersistableModel<MessageCreationAttachment> {
        public MessageCreationAttachment(string fileId, IEnumerable<ToolDefinition> tools);
        public string FileId { get; }
        public IReadOnlyList<ToolDefinition> Tools { get; }
        public static explicit operator MessageCreationAttachment(ClientResult result);
        public static implicit operator BinaryContent(MessageCreationAttachment messageCreationAttachment);
    }
    [Experimental("OPENAI001")]
    public class MessageCreationOptions : IJsonModel<MessageCreationOptions>, IPersistableModel<MessageCreationOptions> {
        public IList<MessageCreationAttachment> Attachments { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public static explicit operator MessageCreationOptions(ClientResult result);
        public static implicit operator BinaryContent(MessageCreationOptions messageCreationOptions);
    }
    [Experimental("OPENAI001")]
    public class MessageDeletionResult : IJsonModel<MessageDeletionResult>, IPersistableModel<MessageDeletionResult> {
        public bool Deleted { get; }
        public string MessageId { get; }
        public static explicit operator MessageDeletionResult(ClientResult result);
        public static implicit operator BinaryContent(MessageDeletionResult messageDeletionResult);
    }
    [Experimental("OPENAI001")]
    public class MessageFailureDetails : IJsonModel<MessageFailureDetails>, IPersistableModel<MessageFailureDetails> {
        public MessageFailureReason Reason { get; }
        public static explicit operator MessageFailureDetails(ClientResult result);
        public static implicit operator BinaryContent(MessageFailureDetails messageFailureDetails);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct MessageFailureReason : IEquatable<MessageFailureReason> {
        public MessageFailureReason(string value);
        public static MessageFailureReason ContentFilter { get; }
        public static MessageFailureReason MaxTokens { get; }
        public static MessageFailureReason RunCancelled { get; }
        public static MessageFailureReason RunExpired { get; }
        public static MessageFailureReason RunFailed { get; }
        public readonly bool Equals(MessageFailureReason other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(MessageFailureReason left, MessageFailureReason right);
        public static implicit operator MessageFailureReason(string value);
        public static bool operator !=(MessageFailureReason left, MessageFailureReason right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public enum MessageImageDetail {
        Auto = 0,
        Low = 1,
        High = 2
    }
    [Experimental("OPENAI001")]
    public class MessageModificationOptions : IJsonModel<MessageModificationOptions>, IPersistableModel<MessageModificationOptions> {
        public IDictionary<string, string> Metadata { get; }
        public static explicit operator MessageModificationOptions(ClientResult result);
        public static implicit operator BinaryContent(MessageModificationOptions messageModificationOptions);
    }
    [Experimental("OPENAI001")]
    public enum MessageRole {
        User = 0,
        Assistant = 1
    }
    [Experimental("OPENAI001")]
    public readonly partial struct MessageStatus : IEquatable<MessageStatus> {
        public MessageStatus(string value);
        public static MessageStatus Completed { get; }
        public static MessageStatus Incomplete { get; }
        public static MessageStatus InProgress { get; }
        public readonly bool Equals(MessageStatus other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(MessageStatus left, MessageStatus right);
        public static implicit operator MessageStatus(string value);
        public static bool operator !=(MessageStatus left, MessageStatus right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class MessageStatusUpdate : StreamingUpdate<ThreadMessage> {
    }
    [Experimental("OPENAI001")]
    public abstract class RequiredAction {
        public string FunctionArguments { get; }
        public string FunctionName { get; }
        public string ToolCallId { get; }
    }
    [Experimental("OPENAI001")]
    public class RequiredActionUpdate : RunUpdate {
        public string FunctionArguments { get; }
        public string FunctionName { get; }
        public string ToolCallId { get; }
        public ThreadRun GetThreadRun();
    }
    [Experimental("OPENAI001")]
    public class RunCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public RunCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    [Experimental("OPENAI001")]
    public readonly partial struct RunCollectionOrder : IEquatable<RunCollectionOrder> {
        public RunCollectionOrder(string value);
        public static RunCollectionOrder Ascending { get; }
        public static RunCollectionOrder Descending { get; }
        public readonly bool Equals(RunCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(RunCollectionOrder left, RunCollectionOrder right);
        public static implicit operator RunCollectionOrder(string value);
        public static bool operator !=(RunCollectionOrder left, RunCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunCreationOptions : IJsonModel<RunCreationOptions>, IPersistableModel<RunCreationOptions> {
        public string AdditionalInstructions { get; set; }
        public IList<ThreadInitializationMessage> AdditionalMessages { get; }
        public bool? AllowParallelToolCalls { get; set; }
        public string InstructionsOverride { get; set; }
        public int? MaxInputTokenCount { get; set; }
        public int? MaxOutputTokenCount { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public string ModelOverride { get; set; }
        public float? NucleusSamplingFactor { get; set; }
        public AssistantResponseFormat ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        public ToolConstraint ToolConstraint { get; set; }
        public IList<ToolDefinition> ToolsOverride { get; }
        public RunTruncationStrategy TruncationStrategy { get; set; }
        public static explicit operator RunCreationOptions(ClientResult result);
        public static implicit operator BinaryContent(RunCreationOptions runCreationOptions);
    }
    [Experimental("OPENAI001")]
    public class RunError : IJsonModel<RunError>, IPersistableModel<RunError> {
        public RunErrorCode Code { get; }
        public string Message { get; }
        public static explicit operator RunError(ClientResult result);
        public static implicit operator BinaryContent(RunError runError);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct RunErrorCode : IEquatable<RunErrorCode> {
        public RunErrorCode(string value);
        public static RunErrorCode InvalidPrompt { get; }
        public static RunErrorCode RateLimitExceeded { get; }
        public static RunErrorCode ServerError { get; }
        public readonly bool Equals(RunErrorCode other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(RunErrorCode left, RunErrorCode right);
        public static implicit operator RunErrorCode(string value);
        public static bool operator !=(RunErrorCode left, RunErrorCode right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunIncompleteDetails : IJsonModel<RunIncompleteDetails>, IPersistableModel<RunIncompleteDetails> {
        public RunIncompleteReason? Reason { get; }
        public static explicit operator RunIncompleteDetails(ClientResult result);
        public static implicit operator BinaryContent(RunIncompleteDetails runIncompleteDetails);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct RunIncompleteReason : IEquatable<RunIncompleteReason> {
        public RunIncompleteReason(string value);
        public static RunIncompleteReason MaxInputTokenCount { get; }
        public static RunIncompleteReason MaxOutputTokenCount { get; }
        public readonly bool Equals(RunIncompleteReason other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(RunIncompleteReason left, RunIncompleteReason right);
        public static implicit operator RunIncompleteReason(string value);
        public static bool operator !=(RunIncompleteReason left, RunIncompleteReason right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunModificationOptions : IJsonModel<RunModificationOptions>, IPersistableModel<RunModificationOptions> {
        public IDictionary<string, string> Metadata { get; }
        public static explicit operator RunModificationOptions(ClientResult result);
        public static implicit operator BinaryContent(RunModificationOptions runModificationOptions);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct RunStatus : IEquatable<RunStatus> {
        public RunStatus(string value);
        public static RunStatus Cancelled { get; }
        public static RunStatus Cancelling { get; }
        public static RunStatus Completed { get; }
        public static RunStatus Expired { get; }
        public static RunStatus Failed { get; }
        public static RunStatus Incomplete { get; }
        public static RunStatus InProgress { get; }
        public bool IsTerminal { get; }
        public static RunStatus Queued { get; }
        public static RunStatus RequiresAction { get; }
        public readonly bool Equals(RunStatus other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(RunStatus left, RunStatus right);
        public static implicit operator RunStatus(string value);
        public static bool operator !=(RunStatus left, RunStatus right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunStep : IJsonModel<RunStep>, IPersistableModel<RunStep> {
        public string AssistantId { get; }
        public DateTimeOffset? CancelledAt { get; }
        public DateTimeOffset? CompletedAt { get; }
        public DateTimeOffset CreatedAt { get; }
        public RunStepDetails Details { get; }
        public DateTimeOffset? ExpiredAt { get; }
        public DateTimeOffset? FailedAt { get; }
        public string Id { get; }
        public RunStepKind Kind { get; }
        public RunStepError LastError { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public string RunId { get; }
        public RunStepStatus Status { get; }
        public string ThreadId { get; }
        public RunStepTokenUsage Usage { get; }
        public static explicit operator RunStep(ClientResult result);
        public static implicit operator BinaryContent(RunStep runStep);
    }
    [Experimental("OPENAI001")]
    public abstract class RunStepCodeInterpreterOutput : IJsonModel<RunStepCodeInterpreterOutput>, IPersistableModel<RunStepCodeInterpreterOutput> {
        public string ImageFileId { get; }
        public string Logs { get; }
        public static explicit operator RunStepCodeInterpreterOutput(ClientResult result);
        public static implicit operator BinaryContent(RunStepCodeInterpreterOutput runStepCodeInterpreterOutput);
    }
    [Experimental("OPENAI001")]
    public class RunStepCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public RunStepCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    [Experimental("OPENAI001")]
    public readonly partial struct RunStepCollectionOrder : IEquatable<RunStepCollectionOrder> {
        public RunStepCollectionOrder(string value);
        public static RunStepCollectionOrder Ascending { get; }
        public static RunStepCollectionOrder Descending { get; }
        public readonly bool Equals(RunStepCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(RunStepCollectionOrder left, RunStepCollectionOrder right);
        public static implicit operator RunStepCollectionOrder(string value);
        public static bool operator !=(RunStepCollectionOrder left, RunStepCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public abstract class RunStepDetails : IJsonModel<RunStepDetails>, IPersistableModel<RunStepDetails> {
        public string CreatedMessageId { get; }
        public IReadOnlyList<RunStepToolCall> ToolCalls { get; }
        public static explicit operator RunStepDetails(ClientResult result);
        public static implicit operator BinaryContent(RunStepDetails runStepDetails);
    }
    [Experimental("OPENAI001")]
    public class RunStepDetailsUpdate : StreamingUpdate {
        public string CodeInterpreterInput { get; }
        public IReadOnlyList<RunStepUpdateCodeInterpreterOutput> CodeInterpreterOutputs { get; }
        public string CreatedMessageId { get; }
        public FileSearchRankingOptions FileSearchRankingOptions { get; }
        public IReadOnlyList<RunStepFileSearchResult> FileSearchResults { get; }
        public string FunctionArguments { get; }
        public string FunctionName { get; }
        public string FunctionOutput { get; }
        public string StepId { get; }
        public string ToolCallId { get; }
        public int? ToolCallIndex { get; }
    }
    [Experimental("OPENAI001")]
    public class RunStepError : IJsonModel<RunStepError>, IPersistableModel<RunStepError> {
        public RunStepErrorCode Code { get; }
        public string Message { get; }
        public static explicit operator RunStepError(ClientResult result);
        public static implicit operator BinaryContent(RunStepError runStepError);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct RunStepErrorCode : IEquatable<RunStepErrorCode> {
        public RunStepErrorCode(string value);
        public static RunStepErrorCode RateLimitExceeded { get; }
        public static RunStepErrorCode ServerError { get; }
        public readonly bool Equals(RunStepErrorCode other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(RunStepErrorCode left, RunStepErrorCode right);
        public static implicit operator RunStepErrorCode(string value);
        public static bool operator !=(RunStepErrorCode left, RunStepErrorCode right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunStepFileSearchResult : IJsonModel<RunStepFileSearchResult>, IPersistableModel<RunStepFileSearchResult> {
        public IReadOnlyList<RunStepFileSearchResultContent> Content { get; }
        public string FileId { get; }
        public string FileName { get; }
        public float Score { get; }
        public static explicit operator RunStepFileSearchResult(ClientResult result);
        public static implicit operator BinaryContent(RunStepFileSearchResult runStepFileSearchResult);
    }
    [Experimental("OPENAI001")]
    public class RunStepFileSearchResultContent : IJsonModel<RunStepFileSearchResultContent>, IPersistableModel<RunStepFileSearchResultContent> {
        public RunStepFileSearchResultContentKind Kind { get; }
        public string Text { get; }
        public static explicit operator RunStepFileSearchResultContent(ClientResult result);
        public static implicit operator BinaryContent(RunStepFileSearchResultContent runStepFileSearchResultContent);
    }
    [Experimental("OPENAI001")]
    public enum RunStepFileSearchResultContentKind {
        Text = 0
    }
    [Experimental("OPENAI001")]
    public enum RunStepKind {
        CreatedMessage = 0,
        ToolCall = 1
    }
    [Experimental("OPENAI001")]
    public readonly partial struct RunStepStatus : IEquatable<RunStepStatus> {
        public RunStepStatus(string value);
        public static RunStepStatus Cancelled { get; }
        public static RunStepStatus Completed { get; }
        public static RunStepStatus Expired { get; }
        public static RunStepStatus Failed { get; }
        public static RunStepStatus InProgress { get; }
        public readonly bool Equals(RunStepStatus other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(RunStepStatus left, RunStepStatus right);
        public static implicit operator RunStepStatus(string value);
        public static bool operator !=(RunStepStatus left, RunStepStatus right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunStepTokenUsage : IJsonModel<RunStepTokenUsage>, IPersistableModel<RunStepTokenUsage> {
        public int InputTokenCount { get; }
        public int OutputTokenCount { get; }
        public int TotalTokenCount { get; }
        public static explicit operator RunStepTokenUsage(ClientResult result);
        public static implicit operator BinaryContent(RunStepTokenUsage runStepTokenUsage);
    }
    [Experimental("OPENAI001")]
    public abstract class RunStepToolCall : IJsonModel<RunStepToolCall>, IPersistableModel<RunStepToolCall> {
        public string CodeInterpreterInput { get; }
        public IReadOnlyList<RunStepCodeInterpreterOutput> CodeInterpreterOutputs { get; }
        public FileSearchRankingOptions FileSearchRankingOptions { get; }
        public IReadOnlyList<RunStepFileSearchResult> FileSearchResults { get; }
        public string FunctionArguments { get; }
        public string FunctionName { get; }
        public string FunctionOutput { get; }
        public string Id { get; }
        public RunStepToolCallKind Kind { get; }
        public static explicit operator RunStepToolCall(ClientResult result);
        public static implicit operator BinaryContent(RunStepToolCall runStepToolCall);
    }
    [Experimental("OPENAI001")]
    public enum RunStepToolCallKind {
        CodeInterpreter = 0,
        FileSearch = 1,
        Function = 2
    }
    [Experimental("OPENAI001")]
    public class RunStepUpdate : StreamingUpdate<RunStep> {
    }
    [Experimental("OPENAI001")]
    public abstract class RunStepUpdateCodeInterpreterOutput : IJsonModel<RunStepUpdateCodeInterpreterOutput>, IPersistableModel<RunStepUpdateCodeInterpreterOutput> {
        public string ImageFileId { get; }
        public string Logs { get; }
        public int OutputIndex { get; }
        public static explicit operator RunStepUpdateCodeInterpreterOutput(ClientResult result);
        public static implicit operator BinaryContent(RunStepUpdateCodeInterpreterOutput runStepUpdateCodeInterpreterOutput);
    }
    [Experimental("OPENAI001")]
    public class RunTokenUsage : IJsonModel<RunTokenUsage>, IPersistableModel<RunTokenUsage> {
        public int InputTokenCount { get; }
        public int OutputTokenCount { get; }
        public int TotalTokenCount { get; }
        public static explicit operator RunTokenUsage(ClientResult result);
        public static implicit operator BinaryContent(RunTokenUsage runTokenUsage);
    }
    [Experimental("OPENAI001")]
    public class RunTruncationStrategy : IJsonModel<RunTruncationStrategy>, IPersistableModel<RunTruncationStrategy> {
        public static RunTruncationStrategy Auto { get; }
        public int? LastMessages { get; }
        public static RunTruncationStrategy CreateLastMessagesStrategy(int lastMessageCount);
        public static explicit operator RunTruncationStrategy(ClientResult result);
        public static implicit operator BinaryContent(RunTruncationStrategy runTruncationStrategy);
    }
    [Experimental("OPENAI001")]
    public class RunUpdate : StreamingUpdate<ThreadRun> {
    }
    [Experimental("OPENAI001")]
    public abstract class StreamingUpdate {
        public StreamingUpdateReason UpdateKind { get; }
    }
    [Experimental("OPENAI001")]
    public enum StreamingUpdateReason {
        Unknown = 0,
        ThreadCreated = 1,
        RunCreated = 2,
        RunQueued = 3,
        RunInProgress = 4,
        RunRequiresAction = 5,
        RunCompleted = 6,
        RunIncomplete = 7,
        RunFailed = 8,
        RunCancelling = 9,
        RunCancelled = 10,
        RunExpired = 11,
        RunStepCreated = 12,
        RunStepInProgress = 13,
        RunStepUpdated = 14,
        RunStepCompleted = 15,
        RunStepFailed = 16,
        RunStepCancelled = 17,
        RunStepExpired = 18,
        MessageCreated = 19,
        MessageInProgress = 20,
        MessageUpdated = 21,
        MessageCompleted = 22,
        MessageFailed = 23,
        Error = 24,
        Done = 25
    }
    [Experimental("OPENAI001")]
    public class StreamingUpdate<T> : StreamingUpdate where T : class {
        public T Value { get; }
        public static implicit operator T(StreamingUpdate<T> update);
    }
    [Experimental("OPENAI001")]
    public class TextAnnotation {
        public int EndIndex { get; }
        public string InputFileId { get; }
        public string OutputFileId { get; }
        public int StartIndex { get; }
        public string TextToReplace { get; }
    }
    [Experimental("OPENAI001")]
    public class TextAnnotationUpdate {
        public int ContentIndex { get; }
        public int? EndIndex { get; }
        public string InputFileId { get; }
        public string OutputFileId { get; }
        public int? StartIndex { get; }
        public string TextToReplace { get; }
    }
    [Experimental("OPENAI001")]
    public class ThreadCreationOptions : IJsonModel<ThreadCreationOptions>, IPersistableModel<ThreadCreationOptions> {
        public IList<ThreadInitializationMessage> InitialMessages { get; }
        public IDictionary<string, string> Metadata { get; }
        public ToolResources ToolResources { get; set; }
        public static explicit operator ThreadCreationOptions(ClientResult result);
        public static implicit operator BinaryContent(ThreadCreationOptions threadCreationOptions);
    }
    [Experimental("OPENAI001")]
    public class ThreadDeletionResult : IJsonModel<ThreadDeletionResult>, IPersistableModel<ThreadDeletionResult> {
        public bool Deleted { get; }
        public string ThreadId { get; }
        public static explicit operator ThreadDeletionResult(ClientResult result);
        public static implicit operator BinaryContent(ThreadDeletionResult threadDeletionResult);
    }
    [Experimental("OPENAI001")]
    public class ThreadInitializationMessage : MessageCreationOptions {
        public ThreadInitializationMessage(MessageRole role, IEnumerable<MessageContent> content);
        public static implicit operator ThreadInitializationMessage(string initializationMessage);
    }
    [Experimental("OPENAI001")]
    public class ThreadMessage : IJsonModel<ThreadMessage>, IPersistableModel<ThreadMessage> {
        public string AssistantId { get; }
        public IReadOnlyList<MessageCreationAttachment> Attachments { get; }
        public DateTimeOffset? CompletedAt { get; }
        public IReadOnlyList<MessageContent> Content { get; }
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public DateTimeOffset? IncompleteAt { get; }
        public MessageFailureDetails IncompleteDetails { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public MessageRole Role { get; }
        public string RunId { get; }
        public MessageStatus Status { get; }
        public string ThreadId { get; }
        public static explicit operator ThreadMessage(ClientResult result);
        public static implicit operator BinaryContent(ThreadMessage threadMessage);
    }
    [Experimental("OPENAI001")]
    public class ThreadModificationOptions : IJsonModel<ThreadModificationOptions>, IPersistableModel<ThreadModificationOptions> {
        public IDictionary<string, string> Metadata { get; }
        public ToolResources ToolResources { get; set; }
        public static explicit operator ThreadModificationOptions(ClientResult result);
        public static implicit operator BinaryContent(ThreadModificationOptions threadModificationOptions);
    }
    [Experimental("OPENAI001")]
    public class ThreadRun : IJsonModel<ThreadRun>, IPersistableModel<ThreadRun> {
        public bool? AllowParallelToolCalls { get; }
        public string AssistantId { get; }
        public DateTimeOffset? CancelledAt { get; }
        public DateTimeOffset? CompletedAt { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset? ExpiresAt { get; }
        public DateTimeOffset? FailedAt { get; }
        public string Id { get; }
        public RunIncompleteDetails IncompleteDetails { get; }
        public string Instructions { get; }
        public RunError LastError { get; }
        public int? MaxInputTokenCount { get; }
        public int? MaxOutputTokenCount { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public string Model { get; }
        public float? NucleusSamplingFactor { get; }
        public IReadOnlyList<RequiredAction> RequiredActions { get; }
        public AssistantResponseFormat ResponseFormat { get; }
        public DateTimeOffset? StartedAt { get; }
        public RunStatus Status { get; }
        public float? Temperature { get; }
        public string ThreadId { get; }
        public ToolConstraint ToolConstraint { get; }
        public IReadOnlyList<ToolDefinition> Tools { get; }
        public RunTruncationStrategy TruncationStrategy { get; }
        public RunTokenUsage Usage { get; }
        public static explicit operator ThreadRun(ClientResult result);
        public static implicit operator BinaryContent(ThreadRun threadRun);
    }
    [Experimental("OPENAI001")]
    public class ThreadUpdate : StreamingUpdate<AssistantThread> {
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public ToolResources ToolResources { get; }
    }
    [Experimental("OPENAI001")]
    public class ToolConstraint : IJsonModel<ToolConstraint>, IPersistableModel<ToolConstraint> {
        public ToolConstraint(ToolDefinition toolDefinition);
        public static ToolConstraint Auto { get; }
        public static ToolConstraint None { get; }
        public static ToolConstraint Required { get; }
        public static explicit operator ToolConstraint(ClientResult result);
        public static implicit operator BinaryContent(ToolConstraint toolConstraint);
    }
    [Experimental("OPENAI001")]
    public abstract class ToolDefinition : IJsonModel<ToolDefinition>, IPersistableModel<ToolDefinition> {
        protected ToolDefinition();
        protected ToolDefinition(string type);
        public static CodeInterpreterToolDefinition CreateCodeInterpreter();
        public static FileSearchToolDefinition CreateFileSearch(int? maxResults = null);
        public static FunctionToolDefinition CreateFunction(string name, string description = null, BinaryData parameters = null, bool? strictParameterSchemaEnabled = null);
        public static explicit operator ToolDefinition(ClientResult result);
        public static implicit operator BinaryContent(ToolDefinition toolDefinition);
    }
    [Experimental("OPENAI001")]
    public class ToolOutput : IJsonModel<ToolOutput>, IPersistableModel<ToolOutput> {
        public ToolOutput();
        public ToolOutput(string toolCallId, string output);
        public string Output { get; set; }
        public string ToolCallId { get; set; }
        public static explicit operator ToolOutput(ClientResult result);
        public static implicit operator BinaryContent(ToolOutput toolOutput);
    }
    [Experimental("OPENAI001")]
    public class ToolResources : IJsonModel<ToolResources>, IPersistableModel<ToolResources> {
        public CodeInterpreterToolResources CodeInterpreter { get; set; }
        public FileSearchToolResources FileSearch { get; set; }
        public static explicit operator ToolResources(ClientResult result);
        public static implicit operator BinaryContent(ToolResources toolResources);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreCreationHelper : IJsonModel<VectorStoreCreationHelper>, IPersistableModel<VectorStoreCreationHelper> {
        public VectorStoreCreationHelper();
        public VectorStoreCreationHelper(IEnumerable<OpenAIFile> files);
        public VectorStoreCreationHelper(IEnumerable<string> fileIds);
        public FileChunkingStrategy ChunkingStrategy { get; set; }
        public IList<string> FileIds { get; }
        public IDictionary<string, string> Metadata { get; }
        public static explicit operator VectorStoreCreationHelper(ClientResult result);
        public static implicit operator BinaryContent(VectorStoreCreationHelper vectorStoreCreationHelper);
    }
}
namespace OpenAI.Audio {
    public class AudioClient {
        protected AudioClient();
        protected internal AudioClient(ClientPipeline pipeline, string model, OpenAIClientOptions options);
        public AudioClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public AudioClient(string model, ApiKeyCredential credential);
        public AudioClient(string model, string apiKey);
        public ClientPipeline Pipeline { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GenerateSpeech(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<BinaryData> GenerateSpeech(string text, GeneratedSpeechVoice voice, SpeechGenerationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GenerateSpeechAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<BinaryData>> GenerateSpeechAsync(string text, GeneratedSpeechVoice voice, SpeechGenerationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult TranscribeAudio(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult<AudioTranscription> TranscribeAudio(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<AudioTranscription> TranscribeAudio(string audioFilePath, AudioTranscriptionOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> TranscribeAudioAsync(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult<AudioTranscription>> TranscribeAudioAsync(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<AudioTranscription>> TranscribeAudioAsync(string audioFilePath, AudioTranscriptionOptions options = null);
        public virtual ClientResult TranslateAudio(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult<AudioTranslation> TranslateAudio(Stream audio, string audioFilename, AudioTranslationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<AudioTranslation> TranslateAudio(string audioFilePath, AudioTranslationOptions options = null);
        public virtual Task<ClientResult> TranslateAudioAsync(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult<AudioTranslation>> TranslateAudioAsync(Stream audio, string audioFilename, AudioTranslationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<AudioTranslation>> TranslateAudioAsync(string audioFilePath, AudioTranslationOptions options = null);
    }
    [Flags]
    public enum AudioTimestampGranularities {
        Default = 0,
        Word = 1,
        Segment = 2
    }
    public class AudioTranscription : IJsonModel<AudioTranscription>, IPersistableModel<AudioTranscription> {
        public TimeSpan? Duration { get; }
        public string Language { get; }
        public IReadOnlyList<TranscribedSegment> Segments { get; }
        public string Text { get; }
        public IReadOnlyList<TranscribedWord> Words { get; }
        public static explicit operator AudioTranscription(ClientResult result);
        public static implicit operator BinaryContent(AudioTranscription audioTranscription);
    }
    public readonly partial struct AudioTranscriptionFormat : IEquatable<AudioTranscriptionFormat> {
        public AudioTranscriptionFormat(string value);
        public static AudioTranscriptionFormat Simple { get; }
        public static AudioTranscriptionFormat Srt { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static AudioTranscriptionFormat Text { get; }
        public static AudioTranscriptionFormat Verbose { get; }
        public static AudioTranscriptionFormat Vtt { get; }
        public readonly bool Equals(AudioTranscriptionFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(AudioTranscriptionFormat left, AudioTranscriptionFormat right);
        public static implicit operator AudioTranscriptionFormat(string value);
        public static bool operator !=(AudioTranscriptionFormat left, AudioTranscriptionFormat right);
        public override readonly string ToString();
    }
    public class AudioTranscriptionOptions : IJsonModel<AudioTranscriptionOptions>, IPersistableModel<AudioTranscriptionOptions> {
        public string Language { get; set; }
        public string Prompt { get; set; }
        public AudioTranscriptionFormat? ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        public AudioTimestampGranularities TimestampGranularities { get; set; }
        public static explicit operator AudioTranscriptionOptions(ClientResult result);
        public static implicit operator BinaryContent(AudioTranscriptionOptions audioTranscriptionOptions);
    }
    public class AudioTranslation : IJsonModel<AudioTranslation>, IPersistableModel<AudioTranslation> {
        public TimeSpan? Duration { get; }
        public string Language { get; }
        public IReadOnlyList<TranscribedSegment> Segments { get; }
        public string Text { get; }
        public static explicit operator AudioTranslation(ClientResult result);
        public static implicit operator BinaryContent(AudioTranslation audioTranslation);
    }
    public readonly partial struct AudioTranslationFormat : IEquatable<AudioTranslationFormat> {
        public AudioTranslationFormat(string value);
        public static AudioTranslationFormat Simple { get; }
        public static AudioTranslationFormat Srt { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static AudioTranslationFormat Text { get; }
        public static AudioTranslationFormat Verbose { get; }
        public static AudioTranslationFormat Vtt { get; }
        public readonly bool Equals(AudioTranslationFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(AudioTranslationFormat left, AudioTranslationFormat right);
        public static implicit operator AudioTranslationFormat(string value);
        public static bool operator !=(AudioTranslationFormat left, AudioTranslationFormat right);
        public override readonly string ToString();
    }
    public class AudioTranslationOptions : IJsonModel<AudioTranslationOptions>, IPersistableModel<AudioTranslationOptions> {
        public string Prompt { get; set; }
        public AudioTranslationFormat? ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        public static explicit operator AudioTranslationOptions(ClientResult result);
        public static implicit operator BinaryContent(AudioTranslationOptions audioTranslationOptions);
    }
    public readonly partial struct GeneratedSpeechFormat : IEquatable<GeneratedSpeechFormat> {
        public GeneratedSpeechFormat(string value);
        public static GeneratedSpeechFormat Aac { get; }
        public static GeneratedSpeechFormat Flac { get; }
        public static GeneratedSpeechFormat Mp3 { get; }
        public static GeneratedSpeechFormat Opus { get; }
        public static GeneratedSpeechFormat Pcm { get; }
        public static GeneratedSpeechFormat Wav { get; }
        public readonly bool Equals(GeneratedSpeechFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedSpeechFormat left, GeneratedSpeechFormat right);
        public static implicit operator GeneratedSpeechFormat(string value);
        public static bool operator !=(GeneratedSpeechFormat left, GeneratedSpeechFormat right);
        public override readonly string ToString();
    }
    public readonly partial struct GeneratedSpeechVoice : IEquatable<GeneratedSpeechVoice> {
        public GeneratedSpeechVoice(string value);
        public static GeneratedSpeechVoice Alloy { get; }
        public static GeneratedSpeechVoice Ash { get; }
        public static GeneratedSpeechVoice Coral { get; }
        public static GeneratedSpeechVoice Echo { get; }
        public static GeneratedSpeechVoice Fable { get; }
        public static GeneratedSpeechVoice Nova { get; }
        public static GeneratedSpeechVoice Onyx { get; }
        public static GeneratedSpeechVoice Sage { get; }
        public static GeneratedSpeechVoice Shimmer { get; }
        public readonly bool Equals(GeneratedSpeechVoice other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedSpeechVoice left, GeneratedSpeechVoice right);
        public static implicit operator GeneratedSpeechVoice(string value);
        public static bool operator !=(GeneratedSpeechVoice left, GeneratedSpeechVoice right);
        public override readonly string ToString();
    }
    public static class OpenAIAudioModelFactory {
        public static AudioTranscription AudioTranscription(string language = null, TimeSpan? duration = null, string text = null, IEnumerable<TranscribedWord> words = null, IEnumerable<TranscribedSegment> segments = null);
        public static AudioTranslation AudioTranslation(string language = null, TimeSpan? duration = null, string text = null, IEnumerable<TranscribedSegment> segments = null);
        public static TranscribedSegment TranscribedSegment(int id = 0, int seekOffset = 0, TimeSpan startTime = default, TimeSpan endTime = default, string text = null, ReadOnlyMemory<int> tokenIds = default, float temperature = 0, float averageLogProbability = 0, float compressionRatio = 0, float noSpeechProbability = 0);
        public static TranscribedWord TranscribedWord(string word = null, TimeSpan startTime = default, TimeSpan endTime = default);
    }
    public class SpeechGenerationOptions : IJsonModel<SpeechGenerationOptions>, IPersistableModel<SpeechGenerationOptions> {
        public GeneratedSpeechFormat? ResponseFormat { get; set; }
        public float? SpeedRatio { get; set; }
        public static explicit operator SpeechGenerationOptions(ClientResult result);
        public static implicit operator BinaryContent(SpeechGenerationOptions speechGenerationOptions);
    }
    public readonly partial struct TranscribedSegment : IJsonModel<TranscribedSegment>, IPersistableModel<TranscribedSegment>, IJsonModel<object>, IPersistableModel<object> {
        public float AverageLogProbability { get; }
        public float CompressionRatio { get; }
        public TimeSpan EndTime { get; }
        public int Id { get; }
        public float NoSpeechProbability { get; }
        public int SeekOffset { get; }
        public TimeSpan StartTime { get; }
        public float Temperature { get; }
        public string Text { get; }
        public ReadOnlyMemory<int> TokenIds { get; }
        public static explicit operator TranscribedSegment(ClientResult result);
        public static implicit operator BinaryContent(TranscribedSegment transcribedSegment);
    }
    public readonly partial struct TranscribedWord : IJsonModel<TranscribedWord>, IPersistableModel<TranscribedWord>, IJsonModel<object>, IPersistableModel<object> {
        public TimeSpan EndTime { get; }
        public TimeSpan StartTime { get; }
        public string Word { get; }
        public static explicit operator TranscribedWord(ClientResult result);
        public static implicit operator BinaryContent(TranscribedWord transcribedWord);
    }
}
namespace OpenAI.Batch {
    [Experimental("OPENAI001")]
    public class BatchClient {
        protected BatchClient();
        public BatchClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public BatchClient(ApiKeyCredential credential);
        protected internal BatchClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public BatchClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual CreateBatchOperation CreateBatch(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual Task<CreateBatchOperation> CreateBatchAsync(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual CollectionResult GetBatches(string after, int? limit, RequestOptions options);
        public virtual AsyncCollectionResult GetBatchesAsync(string after, int? limit, RequestOptions options);
    }
    [Experimental("OPENAI001")]
    public class CreateBatchOperation : OperationResult {
        public string BatchId { get; }
        public override ContinuationToken? RehydrationToken { get; protected set; }
        public virtual ClientResult Cancel(RequestOptions? options);
        public virtual Task<ClientResult> CancelAsync(RequestOptions? options);
        public virtual ClientResult GetBatch(RequestOptions? options);
        public virtual Task<ClientResult> GetBatchAsync(RequestOptions? options);
        public static CreateBatchOperation Rehydrate(BatchClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static Task<CreateBatchOperation> RehydrateAsync(BatchClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public override ClientResult UpdateStatus(RequestOptions? options = null);
        public override ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null);
    }
}
namespace OpenAI.Chat {
    public class AssistantChatMessage : ChatMessage, IJsonModel<AssistantChatMessage>, IPersistableModel<AssistantChatMessage> {
        public AssistantChatMessage(ChatCompletion chatCompletion);
        [Obsolete("This constructor is obsolete. Please use the constructor that takes an IEnumerable<ChatToolCall> parameter instead.")]
        public AssistantChatMessage(ChatFunctionCall functionCall);
        public AssistantChatMessage(params ChatMessageContentPart[] contentParts);
        public AssistantChatMessage(ChatOutputAudioReference outputAudioReference);
        public AssistantChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public AssistantChatMessage(IEnumerable<ChatToolCall> toolCalls);
        public AssistantChatMessage(string content);
        [Obsolete("This property is obsolete. Please use ToolCalls instead.")]
        public ChatFunctionCall FunctionCall { get; set; }
        public ChatOutputAudioReference OutputAudioReference { get; set; }
        public string ParticipantName { get; set; }
        public string Refusal { get; set; }
        public IList<ChatToolCall> ToolCalls { get; }
        public new static explicit operator AssistantChatMessage(ClientResult result);
        public static implicit operator BinaryContent(AssistantChatMessage assistantChatMessage);
    }
    public class ChatAudioOptions : IJsonModel<ChatAudioOptions>, IPersistableModel<ChatAudioOptions> {
        public ChatAudioOptions(ChatOutputAudioVoice outputAudioVoice, ChatOutputAudioFormat outputAudioFormat);
        public ChatOutputAudioFormat OutputAudioFormat { get; }
        public ChatOutputAudioVoice OutputAudioVoice { get; }
        public static explicit operator ChatAudioOptions(ClientResult result);
        public static implicit operator BinaryContent(ChatAudioOptions chatAudioOptions);
    }
    public class ChatClient {
        protected ChatClient();
        protected internal ChatClient(ClientPipeline pipeline, string model, OpenAIClientOptions options);
        public ChatClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public ChatClient(string model, ApiKeyCredential credential);
        public ChatClient(string model, string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult<ChatCompletion> CompleteChat(params ChatMessage[] messages);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult CompleteChat(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ChatCompletion> CompleteChat(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<ChatCompletion>> CompleteChatAsync(params ChatMessage[] messages);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> CompleteChatAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ChatCompletion>> CompleteChatAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingChatCompletionUpdate> CompleteChatStreaming(params ChatMessage[] messages);
        public virtual CollectionResult<StreamingChatCompletionUpdate> CompleteChatStreaming(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(params ChatMessage[] messages);
        public virtual AsyncCollectionResult<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default);
    }
    public class ChatCompletion : IJsonModel<ChatCompletion>, IPersistableModel<ChatCompletion> {
        public ChatMessageContent Content { get; }
        public IReadOnlyList<ChatTokenLogProbabilityDetails> ContentTokenLogProbabilities { get; }
        public DateTimeOffset CreatedAt { get; }
        public ChatFinishReason FinishReason { get; }
        [Obsolete("This property is obsolete. Please use ToolCalls instead.")]
        public ChatFunctionCall FunctionCall { get; }
        public string Id { get; }
        public string Model { get; }
        public ChatOutputAudio OutputAudio { get; }
        public string Refusal { get; }
        public IReadOnlyList<ChatTokenLogProbabilityDetails> RefusalTokenLogProbabilities { get; }
        public ChatMessageRole Role { get; }
        public string SystemFingerprint { get; }
        public IReadOnlyList<ChatToolCall> ToolCalls { get; }
        public ChatTokenUsage Usage { get; }
        public static explicit operator ChatCompletion(ClientResult result);
        public static implicit operator BinaryContent(ChatCompletion chatCompletion);
    }
    public class ChatCompletionOptions : IJsonModel<ChatCompletionOptions>, IPersistableModel<ChatCompletionOptions> {
        public bool? AllowParallelToolCalls { get; set; }
        public ChatAudioOptions AudioOptions { get; set; }
        public string EndUserId { get; set; }
        public float? FrequencyPenalty { get; set; }
        [Obsolete("This property is obsolete. Please use ToolChoice instead.")]
        public ChatFunctionChoice FunctionChoice { get; set; }
        [Obsolete("This property is obsolete. Please use Tools instead.")]
        public IList<ChatFunction> Functions { get; }
        public bool? IncludeLogProbabilities { get; set; }
        public IDictionary<int, int> LogitBiases { get; }
        public int? MaxOutputTokenCount { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public ChatOutputPrediction OutputPrediction { get; set; }
        public float? PresencePenalty { get; set; }
        public ChatReasoningEffortLevel? ReasoningEffortLevel { get; set; }
        public ChatResponseFormat ResponseFormat { get; set; }
        public ChatResponseModalities ResponseModalities { get; set; }
        [Experimental("OPENAI001")]
        public long? Seed { get; set; }
        public IList<string> StopSequences { get; }
        public bool? StoredOutputEnabled { get; set; }
        public float? Temperature { get; set; }
        public ChatToolChoice ToolChoice { get; set; }
        public IList<ChatTool> Tools { get; }
        public int? TopLogProbabilityCount { get; set; }
        public float? TopP { get; set; }
        public static explicit operator ChatCompletionOptions(ClientResult result);
        public static implicit operator BinaryContent(ChatCompletionOptions chatCompletionOptions);
    }
    public enum ChatFinishReason {
        Stop = 0,
        Length = 1,
        ContentFilter = 2,
        ToolCalls = 3,
        FunctionCall = 4
    }
    [Obsolete("This class is obsolete. Please use ChatTool instead.")]
    public class ChatFunction : IJsonModel<ChatFunction>, IPersistableModel<ChatFunction> {
        public ChatFunction(string functionName);
        public string FunctionDescription { get; set; }
        public string FunctionName { get; }
        public BinaryData FunctionParameters { get; set; }
        public static explicit operator ChatFunction(ClientResult result);
        public static implicit operator BinaryContent(ChatFunction chatFunction);
    }
    [Obsolete("This class is obsolete. Please use ChatToolCall instead.")]
    public class ChatFunctionCall : IJsonModel<ChatFunctionCall>, IPersistableModel<ChatFunctionCall> {
        public ChatFunctionCall(string functionName, BinaryData functionArguments);
        public BinaryData FunctionArguments { get; }
        public string FunctionName { get; }
        public static explicit operator ChatFunctionCall(ClientResult result);
        public static implicit operator BinaryContent(ChatFunctionCall chatFunctionCall);
    }
    [Obsolete("This class is obsolete. Please use ChatToolChoice instead.")]
    public class ChatFunctionChoice : IJsonModel<ChatFunctionChoice>, IPersistableModel<ChatFunctionChoice> {
        public static ChatFunctionChoice CreateAutoChoice();
        public static ChatFunctionChoice CreateNamedChoice(string functionName);
        public static ChatFunctionChoice CreateNoneChoice();
        public static explicit operator ChatFunctionChoice(ClientResult result);
        public static implicit operator BinaryContent(ChatFunctionChoice chatFunctionChoice);
    }
    public readonly partial struct ChatImageDetailLevel : IEquatable<ChatImageDetailLevel> {
        public ChatImageDetailLevel(string value);
        public static ChatImageDetailLevel Auto { get; }
        public static ChatImageDetailLevel High { get; }
        public static ChatImageDetailLevel Low { get; }
        public readonly bool Equals(ChatImageDetailLevel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ChatImageDetailLevel left, ChatImageDetailLevel right);
        public static implicit operator ChatImageDetailLevel(string value);
        public static bool operator !=(ChatImageDetailLevel left, ChatImageDetailLevel right);
        public override readonly string ToString();
    }
    public readonly partial struct ChatInputAudioFormat : IEquatable<ChatInputAudioFormat> {
        public ChatInputAudioFormat(string value);
        public static ChatInputAudioFormat Mp3 { get; }
        public static ChatInputAudioFormat Wav { get; }
        public readonly bool Equals(ChatInputAudioFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ChatInputAudioFormat left, ChatInputAudioFormat right);
        public static implicit operator ChatInputAudioFormat(string value);
        public static bool operator !=(ChatInputAudioFormat left, ChatInputAudioFormat right);
        public override readonly string ToString();
    }
    public class ChatInputTokenUsageDetails : IJsonModel<ChatInputTokenUsageDetails>, IPersistableModel<ChatInputTokenUsageDetails> {
        public int AudioTokenCount { get; }
        public int CachedTokenCount { get; }
        public static explicit operator ChatInputTokenUsageDetails(ClientResult result);
        public static implicit operator BinaryContent(ChatInputTokenUsageDetails chatInputTokenUsageDetails);
    }
    public class ChatMessage : IJsonModel<ChatMessage>, IPersistableModel<ChatMessage> {
        public ChatMessageContent Content { get; }
        public static AssistantChatMessage CreateAssistantMessage(ChatCompletion chatCompletion);
        public static AssistantChatMessage CreateAssistantMessage(ChatFunctionCall functionCall);
        public static AssistantChatMessage CreateAssistantMessage(params ChatMessageContentPart[] contentParts);
        public static AssistantChatMessage CreateAssistantMessage(ChatOutputAudioReference outputAudioReference);
        public static AssistantChatMessage CreateAssistantMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public static AssistantChatMessage CreateAssistantMessage(IEnumerable<ChatToolCall> toolCalls);
        public static AssistantChatMessage CreateAssistantMessage(string content);
        public static DeveloperChatMessage CreateDeveloperMessage(params ChatMessageContentPart[] contentParts);
        public static DeveloperChatMessage CreateDeveloperMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public static DeveloperChatMessage CreateDeveloperMessage(string content);
        [Obsolete("This method is obsolete. Please use CreateToolMessage instead.")]
        public static FunctionChatMessage CreateFunctionMessage(string functionName, string content);
        public static SystemChatMessage CreateSystemMessage(params ChatMessageContentPart[] contentParts);
        public static SystemChatMessage CreateSystemMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public static SystemChatMessage CreateSystemMessage(string content);
        public static ToolChatMessage CreateToolMessage(string toolCallId, params ChatMessageContentPart[] contentParts);
        public static ToolChatMessage CreateToolMessage(string toolCallId, IEnumerable<ChatMessageContentPart> contentParts);
        public static ToolChatMessage CreateToolMessage(string toolCallId, string content);
        public static UserChatMessage CreateUserMessage(params ChatMessageContentPart[] contentParts);
        public static UserChatMessage CreateUserMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public static UserChatMessage CreateUserMessage(string content);
        public static explicit operator ChatMessage(ClientResult result);
        public static implicit operator BinaryContent(ChatMessage chatMessage);
        public static implicit operator ChatMessage(string content);
    }
    public class ChatMessageContent : ObjectModel.Collection<ChatMessageContentPart> {
        public ChatMessageContent();
        public ChatMessageContent(params ChatMessageContentPart[] contentParts);
        public ChatMessageContent(IEnumerable<ChatMessageContentPart> contentParts);
        public ChatMessageContent(string content);
    }
    public class ChatMessageContentPart : IJsonModel<ChatMessageContentPart>, IPersistableModel<ChatMessageContentPart> {
        public BinaryData ImageBytes { get; }
        public string ImageBytesMediaType { get; }
        public ChatImageDetailLevel? ImageDetailLevel { get; }
        public Uri ImageUri { get; }
        public BinaryData InputAudioBytes { get; }
        public ChatInputAudioFormat? InputAudioFormat { get; }
        public ChatMessageContentPartKind Kind { get; }
        public string Refusal { get; }
        public string Text { get; }
        public static ChatMessageContentPart CreateImagePart(BinaryData imageBytes, string imageBytesMediaType, ChatImageDetailLevel? imageDetailLevel = null);
        public static ChatMessageContentPart CreateImagePart(Uri imageUri, ChatImageDetailLevel? imageDetailLevel = null);
        public static ChatMessageContentPart CreateInputAudioPart(BinaryData inputAudioBytes, ChatInputAudioFormat inputAudioFormat);
        public static ChatMessageContentPart CreateRefusalPart(string refusal);
        public static ChatMessageContentPart CreateTextPart(string text);
        public static explicit operator ChatMessageContentPart(ClientResult result);
        public static implicit operator BinaryContent(ChatMessageContentPart chatMessageContentPart);
        public static implicit operator ChatMessageContentPart(string text);
    }
    public enum ChatMessageContentPartKind {
        Text = 0,
        Refusal = 1,
        Image = 2,
        InputAudio = 3
    }
    public enum ChatMessageRole {
        System = 0,
        User = 1,
        Assistant = 2,
        Tool = 3,
        Function = 4,
        Developer = 5
    }
    public class ChatOutputAudio : IJsonModel<ChatOutputAudio>, IPersistableModel<ChatOutputAudio> {
        public BinaryData AudioBytes { get; }
        public DateTimeOffset ExpiresAt { get; }
        public string Id { get; }
        public string Transcript { get; }
        public static explicit operator ChatOutputAudio(ClientResult result);
        public static implicit operator BinaryContent(ChatOutputAudio chatOutputAudio);
    }
    public readonly partial struct ChatOutputAudioFormat : IEquatable<ChatOutputAudioFormat> {
        public ChatOutputAudioFormat(string value);
        public static ChatOutputAudioFormat Flac { get; }
        public static ChatOutputAudioFormat Mp3 { get; }
        public static ChatOutputAudioFormat Opus { get; }
        public static ChatOutputAudioFormat Pcm16 { get; }
        public static ChatOutputAudioFormat Wav { get; }
        public readonly bool Equals(ChatOutputAudioFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ChatOutputAudioFormat left, ChatOutputAudioFormat right);
        public static implicit operator ChatOutputAudioFormat(string value);
        public static bool operator !=(ChatOutputAudioFormat left, ChatOutputAudioFormat right);
        public override readonly string ToString();
    }
    public class ChatOutputAudioReference : IJsonModel<ChatOutputAudioReference>, IPersistableModel<ChatOutputAudioReference> {
        public ChatOutputAudioReference(string id);
        public string Id { get; }
        public static explicit operator ChatOutputAudioReference(ClientResult result);
        public static implicit operator BinaryContent(ChatOutputAudioReference chatOutputAudioReference);
    }
    public readonly partial struct ChatOutputAudioVoice : IEquatable<ChatOutputAudioVoice> {
        public ChatOutputAudioVoice(string value);
        public static ChatOutputAudioVoice Alloy { get; }
        public static ChatOutputAudioVoice Ash { get; }
        public static ChatOutputAudioVoice Ballad { get; }
        public static ChatOutputAudioVoice Coral { get; }
        public static ChatOutputAudioVoice Echo { get; }
        public static ChatOutputAudioVoice Sage { get; }
        public static ChatOutputAudioVoice Shimmer { get; }
        public static ChatOutputAudioVoice Verse { get; }
        public readonly bool Equals(ChatOutputAudioVoice other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ChatOutputAudioVoice left, ChatOutputAudioVoice right);
        public static implicit operator ChatOutputAudioVoice(string value);
        public static bool operator !=(ChatOutputAudioVoice left, ChatOutputAudioVoice right);
        public override readonly string ToString();
    }
    public class ChatOutputPrediction : IJsonModel<ChatOutputPrediction>, IPersistableModel<ChatOutputPrediction> {
        public static ChatOutputPrediction CreateStaticContentPrediction(IEnumerable<ChatMessageContentPart> staticContentParts);
        public static ChatOutputPrediction CreateStaticContentPrediction(string staticContent);
        public static explicit operator ChatOutputPrediction(ClientResult result);
        public static implicit operator BinaryContent(ChatOutputPrediction chatOutputPrediction);
    }
    public class ChatOutputTokenUsageDetails : IJsonModel<ChatOutputTokenUsageDetails>, IPersistableModel<ChatOutputTokenUsageDetails> {
        public int AcceptedPredictionTokenCount { get; }
        public int AudioTokenCount { get; }
        public int ReasoningTokenCount { get; }
        public int RejectedPredictionTokenCount { get; }
        public static explicit operator ChatOutputTokenUsageDetails(ClientResult result);
        public static implicit operator BinaryContent(ChatOutputTokenUsageDetails chatOutputTokenUsageDetails);
    }
    public readonly partial struct ChatReasoningEffortLevel : IEquatable<ChatReasoningEffortLevel> {
        public ChatReasoningEffortLevel(string value);
        public static ChatReasoningEffortLevel High { get; }
        public static ChatReasoningEffortLevel Low { get; }
        public static ChatReasoningEffortLevel Medium { get; }
        public readonly bool Equals(ChatReasoningEffortLevel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ChatReasoningEffortLevel left, ChatReasoningEffortLevel right);
        public static implicit operator ChatReasoningEffortLevel(string value);
        public static bool operator !=(ChatReasoningEffortLevel left, ChatReasoningEffortLevel right);
        public override readonly string ToString();
    }
    public class ChatResponseFormat : IJsonModel<ChatResponseFormat>, IPersistableModel<ChatResponseFormat> {
        public static ChatResponseFormat CreateJsonObjectFormat();
        public static ChatResponseFormat CreateJsonSchemaFormat(string jsonSchemaFormatName, BinaryData jsonSchema, string jsonSchemaFormatDescription = null, bool? jsonSchemaIsStrict = null);
        public static ChatResponseFormat CreateTextFormat();
        public static explicit operator ChatResponseFormat(ClientResult result);
        public static implicit operator BinaryContent(ChatResponseFormat chatResponseFormat);
    }
    [Flags]
    public enum ChatResponseModalities {
        Default = 0,
        Text = 1,
        Audio = 2
    }
    public class ChatTokenLogProbabilityDetails : IJsonModel<ChatTokenLogProbabilityDetails>, IPersistableModel<ChatTokenLogProbabilityDetails> {
        public float LogProbability { get; }
        public string Token { get; }
        public IReadOnlyList<ChatTokenTopLogProbabilityDetails> TopLogProbabilities { get; }
        public ReadOnlyMemory<byte>? Utf8Bytes { get; }
        public static explicit operator ChatTokenLogProbabilityDetails(ClientResult result);
        public static implicit operator BinaryContent(ChatTokenLogProbabilityDetails chatTokenLogProbabilityDetails);
    }
    public class ChatTokenTopLogProbabilityDetails : IJsonModel<ChatTokenTopLogProbabilityDetails>, IPersistableModel<ChatTokenTopLogProbabilityDetails> {
        public float LogProbability { get; }
        public string Token { get; }
        public ReadOnlyMemory<byte>? Utf8Bytes { get; }
        public static explicit operator ChatTokenTopLogProbabilityDetails(ClientResult result);
        public static implicit operator BinaryContent(ChatTokenTopLogProbabilityDetails chatTokenTopLogProbabilityDetails);
    }
    public class ChatTokenUsage : IJsonModel<ChatTokenUsage>, IPersistableModel<ChatTokenUsage> {
        public int InputTokenCount { get; }
        public ChatInputTokenUsageDetails InputTokenDetails { get; }
        public int OutputTokenCount { get; }
        public ChatOutputTokenUsageDetails OutputTokenDetails { get; }
        public int TotalTokenCount { get; }
        public static explicit operator ChatTokenUsage(ClientResult result);
        public static implicit operator BinaryContent(ChatTokenUsage chatTokenUsage);
    }
    public class ChatTool : IJsonModel<ChatTool>, IPersistableModel<ChatTool> {
        public string FunctionDescription { get; }
        public string FunctionName { get; }
        public BinaryData FunctionParameters { get; }
        public bool? FunctionSchemaIsStrict { get; }
        public ChatToolKind Kind { get; }
        public static ChatTool CreateFunctionTool(string functionName, string functionDescription = null, BinaryData functionParameters = null, bool? functionSchemaIsStrict = null);
        public static explicit operator ChatTool(ClientResult result);
        public static implicit operator BinaryContent(ChatTool chatTool);
    }
    public class ChatToolCall : IJsonModel<ChatToolCall>, IPersistableModel<ChatToolCall> {
        public BinaryData FunctionArguments { get; }
        public string FunctionName { get; }
        public string Id { get; set; }
        public ChatToolCallKind Kind { get; }
        public static ChatToolCall CreateFunctionToolCall(string id, string functionName, BinaryData functionArguments);
        public static explicit operator ChatToolCall(ClientResult result);
        public static implicit operator BinaryContent(ChatToolCall chatToolCall);
    }
    public enum ChatToolCallKind {
        Function = 0
    }
    public class ChatToolChoice : IJsonModel<ChatToolChoice>, IPersistableModel<ChatToolChoice> {
        public static ChatToolChoice CreateAutoChoice();
        public static ChatToolChoice CreateFunctionChoice(string functionName);
        public static ChatToolChoice CreateNoneChoice();
        public static ChatToolChoice CreateRequiredChoice();
        public static explicit operator ChatToolChoice(ClientResult result);
        public static implicit operator BinaryContent(ChatToolChoice chatToolChoice);
    }
    public enum ChatToolKind {
        Function = 0
    }
    public class DeveloperChatMessage : ChatMessage, IJsonModel<DeveloperChatMessage>, IPersistableModel<DeveloperChatMessage> {
        public DeveloperChatMessage(params ChatMessageContentPart[] contentParts);
        public DeveloperChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public DeveloperChatMessage(string content);
        public string ParticipantName { get; set; }
        public new static explicit operator DeveloperChatMessage(ClientResult result);
        public static implicit operator BinaryContent(DeveloperChatMessage developerChatMessage);
    }
    [Obsolete("This class is obsolete. Please use ToolChatMessage instead.")]
    public class FunctionChatMessage : ChatMessage, IJsonModel<FunctionChatMessage>, IPersistableModel<FunctionChatMessage> {
        public FunctionChatMessage(string functionName, string content);
        public string FunctionName { get; }
        public new static explicit operator FunctionChatMessage(ClientResult result);
        public static implicit operator BinaryContent(FunctionChatMessage functionChatMessage);
    }
    public static class OpenAIChatModelFactory {
        public static ChatCompletion ChatCompletion(string id = null, ChatFinishReason finishReason = ChatFinishReason.Stop, ChatMessageContent content = null, string refusal = null, IEnumerable<ChatToolCall> toolCalls = null, ChatMessageRole role = ChatMessageRole.System, ChatFunctionCall functionCall = null, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null, DateTimeOffset createdAt = default, string model = null, string systemFingerprint = null, ChatTokenUsage usage = null, ChatOutputAudio outputAudio = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ChatCompletion ChatCompletion(string id, ChatFinishReason finishReason, ChatMessageContent content, string refusal, IEnumerable<ChatToolCall> toolCalls, ChatMessageRole role, ChatFunctionCall functionCall, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities, DateTimeOffset createdAt, string model, string systemFingerprint, ChatTokenUsage usage);
        public static ChatInputTokenUsageDetails ChatInputTokenUsageDetails(int audioTokenCount = 0, int cachedTokenCount = 0);
        public static ChatOutputAudio ChatOutputAudio(BinaryData audioBytes, string id = null, string transcript = null, DateTimeOffset expiresAt = default);
        public static ChatOutputTokenUsageDetails ChatOutputTokenUsageDetails(int reasoningTokenCount = 0, int audioTokenCount = 0, int acceptedPredictionTokenCount = 0, int rejectedPredictionTokenCount = 0);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ChatOutputTokenUsageDetails ChatOutputTokenUsageDetails(int reasoningTokenCount, int audioTokenCount);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ChatOutputTokenUsageDetails ChatOutputTokenUsageDetails(int reasoningTokenCount);
        public static ChatTokenLogProbabilityDetails ChatTokenLogProbabilityDetails(string token = null, float logProbability = 0, ReadOnlyMemory<byte>? utf8Bytes = null, IEnumerable<ChatTokenTopLogProbabilityDetails> topLogProbabilities = null);
        public static ChatTokenTopLogProbabilityDetails ChatTokenTopLogProbabilityDetails(string token = null, float logProbability = 0, ReadOnlyMemory<byte>? utf8Bytes = null);
        public static ChatTokenUsage ChatTokenUsage(int outputTokenCount = 0, int inputTokenCount = 0, int totalTokenCount = 0, ChatOutputTokenUsageDetails outputTokenDetails = null, ChatInputTokenUsageDetails inputTokenDetails = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ChatTokenUsage ChatTokenUsage(int outputTokenCount, int inputTokenCount, int totalTokenCount, ChatOutputTokenUsageDetails outputTokenDetails);
        public static StreamingChatCompletionUpdate StreamingChatCompletionUpdate(string completionId = null, ChatMessageContent contentUpdate = null, StreamingChatFunctionCallUpdate functionCallUpdate = null, IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates = null, ChatMessageRole? role = null, string refusalUpdate = null, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null, ChatFinishReason? finishReason = null, DateTimeOffset createdAt = default, string model = null, string systemFingerprint = null, ChatTokenUsage usage = null, StreamingChatOutputAudioUpdate outputAudioUpdate = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static StreamingChatCompletionUpdate StreamingChatCompletionUpdate(string completionId, ChatMessageContent contentUpdate, StreamingChatFunctionCallUpdate functionCallUpdate, IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates, ChatMessageRole? role, string refusalUpdate, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities, ChatFinishReason? finishReason, DateTimeOffset createdAt, string model, string systemFingerprint, ChatTokenUsage usage);
        [Obsolete("This class is obsolete. Please use StreamingChatToolCallUpdate instead.")]
        public static StreamingChatFunctionCallUpdate StreamingChatFunctionCallUpdate(string functionName = null, BinaryData functionArgumentsUpdate = null);
        public static StreamingChatOutputAudioUpdate StreamingChatOutputAudioUpdate(string id = null, DateTimeOffset? expiresAt = null, string transcriptUpdate = null, BinaryData audioBytesUpdate = null);
        public static StreamingChatToolCallUpdate StreamingChatToolCallUpdate(int index = 0, string toolCallId = null, ChatToolCallKind kind = ChatToolCallKind.Function, string functionName = null, BinaryData functionArgumentsUpdate = null);
    }
    public class StreamingChatCompletionUpdate : IJsonModel<StreamingChatCompletionUpdate>, IPersistableModel<StreamingChatCompletionUpdate> {
        public string CompletionId { get; }
        public IReadOnlyList<ChatTokenLogProbabilityDetails> ContentTokenLogProbabilities { get; }
        public ChatMessageContent ContentUpdate { get; }
        public DateTimeOffset CreatedAt { get; }
        public ChatFinishReason? FinishReason { get; }
        [Obsolete("This property is obsolete. Please use ToolCallUpdates instead.")]
        public StreamingChatFunctionCallUpdate FunctionCallUpdate { get; }
        public string Model { get; }
        public StreamingChatOutputAudioUpdate OutputAudioUpdate { get; }
        public IReadOnlyList<ChatTokenLogProbabilityDetails> RefusalTokenLogProbabilities { get; }
        public string RefusalUpdate { get; }
        public ChatMessageRole? Role { get; }
        public string SystemFingerprint { get; }
        public IReadOnlyList<StreamingChatToolCallUpdate> ToolCallUpdates { get; }
        public ChatTokenUsage Usage { get; }
        public static explicit operator StreamingChatCompletionUpdate(ClientResult result);
        public static implicit operator BinaryContent(StreamingChatCompletionUpdate streamingChatCompletionUpdate);
    }
    [Obsolete("This class is obsolete. Please use StreamingChatToolCallUpdate instead.")]
    public class StreamingChatFunctionCallUpdate : IJsonModel<StreamingChatFunctionCallUpdate>, IPersistableModel<StreamingChatFunctionCallUpdate> {
        public BinaryData FunctionArgumentsUpdate { get; }
        public string FunctionName { get; }
        public static explicit operator StreamingChatFunctionCallUpdate(ClientResult result);
        public static implicit operator BinaryContent(StreamingChatFunctionCallUpdate streamingChatFunctionCallUpdate);
    }
    public class StreamingChatOutputAudioUpdate : IJsonModel<StreamingChatOutputAudioUpdate>, IPersistableModel<StreamingChatOutputAudioUpdate> {
        public BinaryData AudioBytesUpdate { get; }
        public DateTimeOffset? ExpiresAt { get; }
        public string Id { get; }
        public string TranscriptUpdate { get; }
        public static explicit operator StreamingChatOutputAudioUpdate(ClientResult result);
        public static implicit operator BinaryContent(StreamingChatOutputAudioUpdate streamingChatOutputAudioUpdate);
    }
    public class StreamingChatToolCallUpdate : IJsonModel<StreamingChatToolCallUpdate>, IPersistableModel<StreamingChatToolCallUpdate> {
        public BinaryData FunctionArgumentsUpdate { get; }
        public string FunctionName { get; }
        public int Index { get; }
        public ChatToolCallKind Kind { get; }
        public string ToolCallId { get; }
        public static explicit operator StreamingChatToolCallUpdate(ClientResult result);
        public static implicit operator BinaryContent(StreamingChatToolCallUpdate streamingChatToolCallUpdate);
    }
    public class SystemChatMessage : ChatMessage, IJsonModel<SystemChatMessage>, IPersistableModel<SystemChatMessage> {
        public SystemChatMessage(params ChatMessageContentPart[] contentParts);
        public SystemChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public SystemChatMessage(string content);
        public string ParticipantName { get; set; }
        public new static explicit operator SystemChatMessage(ClientResult result);
        public static implicit operator BinaryContent(SystemChatMessage systemChatMessage);
    }
    public class ToolChatMessage : ChatMessage, IJsonModel<ToolChatMessage>, IPersistableModel<ToolChatMessage> {
        public ToolChatMessage(string toolCallId, params ChatMessageContentPart[] contentParts);
        public ToolChatMessage(string toolCallId, IEnumerable<ChatMessageContentPart> contentParts);
        public ToolChatMessage(string toolCallId, string content);
        public string ToolCallId { get; }
        public new static explicit operator ToolChatMessage(ClientResult result);
        public static implicit operator BinaryContent(ToolChatMessage toolChatMessage);
    }
    public class UserChatMessage : ChatMessage, IJsonModel<UserChatMessage>, IPersistableModel<UserChatMessage> {
        public UserChatMessage(params ChatMessageContentPart[] contentParts);
        public UserChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public UserChatMessage(string content);
        public string ParticipantName { get; set; }
        public new static explicit operator UserChatMessage(ClientResult result);
        public static implicit operator BinaryContent(UserChatMessage userChatMessage);
    }
}
namespace OpenAI.Embeddings {
    public class EmbeddingClient {
        protected EmbeddingClient();
        protected internal EmbeddingClient(ClientPipeline pipeline, string model, OpenAIClientOptions options);
        public EmbeddingClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public EmbeddingClient(string model, ApiKeyCredential credential);
        public EmbeddingClient(string model, string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult<OpenAIEmbedding> GenerateEmbedding(string input, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<OpenAIEmbedding>> GenerateEmbeddingAsync(string input, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GenerateEmbeddings(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<OpenAIEmbeddingCollection> GenerateEmbeddings(IEnumerable<ReadOnlyMemory<int>> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<OpenAIEmbeddingCollection> GenerateEmbeddings(IEnumerable<string> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GenerateEmbeddingsAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<OpenAIEmbeddingCollection>> GenerateEmbeddingsAsync(IEnumerable<ReadOnlyMemory<int>> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<OpenAIEmbeddingCollection>> GenerateEmbeddingsAsync(IEnumerable<string> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
    }
    public class EmbeddingGenerationOptions : IJsonModel<EmbeddingGenerationOptions>, IPersistableModel<EmbeddingGenerationOptions> {
        public int? Dimensions { get; set; }
        public string EndUserId { get; set; }
        public static explicit operator EmbeddingGenerationOptions(ClientResult result);
        public static implicit operator BinaryContent(EmbeddingGenerationOptions embeddingGenerationOptions);
    }
    public class EmbeddingTokenUsage : IJsonModel<EmbeddingTokenUsage>, IPersistableModel<EmbeddingTokenUsage> {
        public int InputTokenCount { get; }
        public int TotalTokenCount { get; }
        public static explicit operator EmbeddingTokenUsage(ClientResult result);
        public static implicit operator BinaryContent(EmbeddingTokenUsage embeddingTokenUsage);
    }
    public class OpenAIEmbedding : IJsonModel<OpenAIEmbedding>, IPersistableModel<OpenAIEmbedding> {
        public int Index { get; }
        public static explicit operator OpenAIEmbedding(ClientResult result);
        public static implicit operator BinaryContent(OpenAIEmbedding openAIEmbedding);
        public ReadOnlyMemory<float> ToFloats();
    }
    public class OpenAIEmbeddingCollection : ObjectModel.ReadOnlyCollection<OpenAIEmbedding>, IJsonModel<OpenAIEmbeddingCollection>, IPersistableModel<OpenAIEmbeddingCollection> {
        public string Model { get; }
        public EmbeddingTokenUsage Usage { get; }
        public static explicit operator OpenAIEmbeddingCollection(ClientResult result);
        public static implicit operator BinaryContent(OpenAIEmbeddingCollection openAIEmbeddingCollection);
    }
    public static class OpenAIEmbeddingsModelFactory {
        public static EmbeddingTokenUsage EmbeddingTokenUsage(int inputTokenCount = 0, int totalTokenCount = 0);
        public static OpenAIEmbedding OpenAIEmbedding(int index = 0, IEnumerable<float> vector = null);
        public static OpenAIEmbeddingCollection OpenAIEmbeddingCollection(IEnumerable<OpenAIEmbedding> items = null, string model = null, EmbeddingTokenUsage usage = null);
    }
}
namespace OpenAI.Files {
    public class FileDeletionResult : IJsonModel<FileDeletionResult>, IPersistableModel<FileDeletionResult> {
        public bool Deleted { get; }
        public string FileId { get; }
        public static explicit operator FileDeletionResult(ClientResult result);
        public static implicit operator BinaryContent(FileDeletionResult fileDeletionResult);
    }
    public enum FilePurpose {
        Assistants = 0,
        AssistantsOutput = 1,
        Batch = 2,
        BatchOutput = 3,
        FineTune = 4,
        FineTuneResults = 5,
        Vision = 6
    }
    [Obsolete("This struct is obsolete. If this is a fine-tuning training file, it may take some time to process after it has been uploaded. While the file is processing, you can still create a fine-tuning job but it will not start until the file processing has completed.")]
    public enum FileStatus {
        Uploaded = 0,
        Processed = 1,
        Error = 2
    }
    public readonly partial struct FileUploadPurpose : IEquatable<FileUploadPurpose> {
        public FileUploadPurpose(string value);
        public static FileUploadPurpose Assistants { get; }
        public static FileUploadPurpose Batch { get; }
        public static FileUploadPurpose FineTune { get; }
        public static FileUploadPurpose Vision { get; }
        public readonly bool Equals(FileUploadPurpose other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(FileUploadPurpose left, FileUploadPurpose right);
        public static implicit operator FileUploadPurpose(string value);
        public static bool operator !=(FileUploadPurpose left, FileUploadPurpose right);
        public override readonly string ToString();
    }
    public class OpenAIFile : IJsonModel<OpenAIFile>, IPersistableModel<OpenAIFile> {
        public DateTimeOffset CreatedAt { get; }
        public string Filename { get; }
        public string Id { get; }
        public FilePurpose Purpose { get; }
        public int? SizeInBytes { get; }
        [Obsolete("This property is obsolete. If this is a fine-tuning training file, it may take some time to process after it has been uploaded. While the file is processing, you can still create a fine-tuning job but it will not start until the file processing has completed.")]
        public FileStatus Status { get; }
        [Obsolete("This property is obsolete. For details on why a fine-tuning training file failed validation, see the `error` field on the fine-tuning job.")]
        public string StatusDetails { get; }
        public static explicit operator OpenAIFile(ClientResult result);
        public static implicit operator BinaryContent(OpenAIFile openAIFile);
    }
    public class OpenAIFileClient {
        protected OpenAIFileClient();
        public OpenAIFileClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public OpenAIFileClient(ApiKeyCredential credential);
        protected internal OpenAIFileClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public OpenAIFileClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        [Experimental("OPENAI001")]
        public virtual ClientResult AddUploadPart(string uploadId, BinaryContent content, string contentType, RequestOptions options = null);
        [Experimental("OPENAI001")]
        public virtual Task<ClientResult> AddUploadPartAsync(string uploadId, BinaryContent content, string contentType, RequestOptions options = null);
        [Experimental("OPENAI001")]
        public virtual ClientResult CancelUpload(string uploadId, RequestOptions options = null);
        [Experimental("OPENAI001")]
        public virtual Task<ClientResult> CancelUploadAsync(string uploadId, RequestOptions options = null);
        [Experimental("OPENAI001")]
        public virtual ClientResult CompleteUpload(string uploadId, BinaryContent content, RequestOptions options = null);
        [Experimental("OPENAI001")]
        public virtual Task<ClientResult> CompleteUploadAsync(string uploadId, BinaryContent content, RequestOptions options = null);
        [Experimental("OPENAI001")]
        public virtual ClientResult CreateUpload(BinaryContent content, RequestOptions options = null);
        [Experimental("OPENAI001")]
        public virtual Task<ClientResult> CreateUploadAsync(BinaryContent content, RequestOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult DeleteFile(string fileId, RequestOptions options);
        public virtual ClientResult<FileDeletionResult> DeleteFile(string fileId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> DeleteFileAsync(string fileId, RequestOptions options);
        public virtual Task<ClientResult<FileDeletionResult>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult DownloadFile(string fileId, RequestOptions options);
        public virtual ClientResult<BinaryData> DownloadFile(string fileId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> DownloadFileAsync(string fileId, RequestOptions options);
        public virtual Task<ClientResult<BinaryData>> DownloadFileAsync(string fileId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetFile(string fileId, RequestOptions options);
        public virtual ClientResult<OpenAIFile> GetFile(string fileId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetFileAsync(string fileId, RequestOptions options);
        public virtual Task<ClientResult<OpenAIFile>> GetFileAsync(string fileId, CancellationToken cancellationToken = default);
        public virtual ClientResult<OpenAIFileCollection> GetFiles(FilePurpose purpose, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetFiles(string purpose, RequestOptions options);
        public virtual ClientResult<OpenAIFileCollection> GetFiles(CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<OpenAIFileCollection>> GetFilesAsync(FilePurpose purpose, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetFilesAsync(string purpose, RequestOptions options);
        public virtual Task<ClientResult<OpenAIFileCollection>> GetFilesAsync(CancellationToken cancellationToken = default);
        public virtual ClientResult<OpenAIFile> UploadFile(BinaryData file, string filename, FileUploadPurpose purpose);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult UploadFile(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult<OpenAIFile> UploadFile(Stream file, string filename, FileUploadPurpose purpose, CancellationToken cancellationToken = default);
        public virtual ClientResult<OpenAIFile> UploadFile(string filePath, FileUploadPurpose purpose);
        public virtual Task<ClientResult<OpenAIFile>> UploadFileAsync(BinaryData file, string filename, FileUploadPurpose purpose);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> UploadFileAsync(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult<OpenAIFile>> UploadFileAsync(Stream file, string filename, FileUploadPurpose purpose, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<OpenAIFile>> UploadFileAsync(string filePath, FileUploadPurpose purpose);
    }
    public class OpenAIFileCollection : ObjectModel.ReadOnlyCollection<OpenAIFile>, IJsonModel<OpenAIFileCollection>, IPersistableModel<OpenAIFileCollection> {
        public static explicit operator OpenAIFileCollection(ClientResult result);
        public static implicit operator BinaryContent(OpenAIFileCollection openAIFileCollection);
    }
    public static class OpenAIFilesModelFactory {
        public static FileDeletionResult FileDeletionResult(string fileId = null, bool deleted = false);
        public static OpenAIFileCollection OpenAIFileCollection(IEnumerable<OpenAIFile> items = null);
        public static OpenAIFile OpenAIFileInfo(string id = null, int? sizeInBytes = null, DateTimeOffset createdAt = default, string filename = null, FilePurpose purpose = FilePurpose.Assistants, FileStatus status = FileStatus.Uploaded, string statusDetails = null);
    }
}
namespace OpenAI.FineTuning {
    [Experimental("OPENAI001")]
    public class FineTuningClient {
        protected FineTuningClient();
        public FineTuningClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public FineTuningClient(ApiKeyCredential credential);
        protected internal FineTuningClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public FineTuningClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual FineTuningJobOperation CreateFineTuningJob(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual Task<FineTuningJobOperation> CreateFineTuningJobAsync(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual ClientResult GetJob(string fineTuningJobId, RequestOptions options);
        public virtual Task<ClientResult> GetJobAsync(string fineTuningJobId, RequestOptions options);
        public virtual CollectionResult GetJobs(string after, int? limit, RequestOptions options);
        public virtual AsyncCollectionResult GetJobsAsync(string after, int? limit, RequestOptions options);
    }
    [Experimental("OPENAI001")]
    public class FineTuningJobOperation : OperationResult {
        public string JobId { get; }
        public override ContinuationToken? RehydrationToken { get; protected set; }
        public virtual ClientResult Cancel(RequestOptions? options);
        public virtual Task<ClientResult> CancelAsync(RequestOptions? options);
        public virtual ClientResult GetJob(RequestOptions? options);
        public virtual Task<ClientResult> GetJobAsync(RequestOptions? options);
        public virtual CollectionResult GetJobCheckpoints(string? after, int? limit, RequestOptions? options);
        public virtual AsyncCollectionResult GetJobCheckpointsAsync(string? after, int? limit, RequestOptions? options);
        public virtual CollectionResult GetJobEvents(string? after, int? limit, RequestOptions options);
        public virtual AsyncCollectionResult GetJobEventsAsync(string? after, int? limit, RequestOptions options);
        public static FineTuningJobOperation Rehydrate(FineTuningClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static FineTuningJobOperation Rehydrate(FineTuningClient client, string fineTuningJobId, CancellationToken cancellationToken = default);
        public static Task<FineTuningJobOperation> RehydrateAsync(FineTuningClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static Task<FineTuningJobOperation> RehydrateAsync(FineTuningClient client, string fineTuningJobId, CancellationToken cancellationToken = default);
        public override ClientResult UpdateStatus(RequestOptions? options = null);
        public override ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null);
    }
}
namespace OpenAI.Images {
    public class GeneratedImage : IJsonModel<GeneratedImage>, IPersistableModel<GeneratedImage> {
        public BinaryData ImageBytes { get; }
        public Uri ImageUri { get; }
        public string RevisedPrompt { get; }
        public static explicit operator GeneratedImage(ClientResult result);
        public static implicit operator BinaryContent(GeneratedImage generatedImage);
    }
    public class GeneratedImageCollection : ObjectModel.ReadOnlyCollection<GeneratedImage>, IJsonModel<GeneratedImageCollection>, IPersistableModel<GeneratedImageCollection> {
        public DateTimeOffset CreatedAt { get; }
        public static explicit operator GeneratedImageCollection(ClientResult result);
        public static implicit operator BinaryContent(GeneratedImageCollection generatedImageCollection);
    }
    public readonly partial struct GeneratedImageFormat : IEquatable<GeneratedImageFormat> {
        public GeneratedImageFormat(string value);
        public static GeneratedImageFormat Bytes { get; }
        public static GeneratedImageFormat Uri { get; }
        public readonly bool Equals(GeneratedImageFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedImageFormat left, GeneratedImageFormat right);
        public static implicit operator GeneratedImageFormat(string value);
        public static bool operator !=(GeneratedImageFormat left, GeneratedImageFormat right);
        public override readonly string ToString();
    }
    public readonly partial struct GeneratedImageQuality : IEquatable<GeneratedImageQuality> {
        public GeneratedImageQuality(string value);
        public static GeneratedImageQuality High { get; }
        public static GeneratedImageQuality Standard { get; }
        public readonly bool Equals(GeneratedImageQuality other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedImageQuality left, GeneratedImageQuality right);
        public static implicit operator GeneratedImageQuality(string value);
        public static bool operator !=(GeneratedImageQuality left, GeneratedImageQuality right);
        public override readonly string ToString();
    }
    public readonly partial struct GeneratedImageSize : IEquatable<GeneratedImageSize> {
        public static readonly GeneratedImageSize W1024xH1024;
        public static readonly GeneratedImageSize W1024xH1792;
        public static readonly GeneratedImageSize W1792xH1024;
        public static readonly GeneratedImageSize W256xH256;
        public static readonly GeneratedImageSize W512xH512;
        public GeneratedImageSize(int width, int height);
        public readonly bool Equals(GeneratedImageSize other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedImageSize left, GeneratedImageSize right);
        public static bool operator !=(GeneratedImageSize left, GeneratedImageSize right);
        public override readonly string ToString();
    }
    public readonly partial struct GeneratedImageStyle : IEquatable<GeneratedImageStyle> {
        public GeneratedImageStyle(string value);
        public static GeneratedImageStyle Natural { get; }
        public static GeneratedImageStyle Vivid { get; }
        public readonly bool Equals(GeneratedImageStyle other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedImageStyle left, GeneratedImageStyle right);
        public static implicit operator GeneratedImageStyle(string value);
        public static bool operator !=(GeneratedImageStyle left, GeneratedImageStyle right);
        public override readonly string ToString();
    }
    public class ImageClient {
        protected ImageClient();
        protected internal ImageClient(ClientPipeline pipeline, string model, OpenAIClientOptions options);
        public ImageClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public ImageClient(string model, ApiKeyCredential credential);
        public ImageClient(string model, string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult<GeneratedImage> GenerateImage(string prompt, ImageGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImage>> GenerateImageAsync(string prompt, ImageGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImage> GenerateImageEdit(Stream image, string imageFilename, string prompt, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImage> GenerateImageEdit(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImage> GenerateImageEdit(string imageFilePath, string prompt, ImageEditOptions options = null);
        public virtual ClientResult<GeneratedImage> GenerateImageEdit(string imageFilePath, string prompt, string maskFilePath, ImageEditOptions options = null);
        public virtual Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(Stream image, string imageFilename, string prompt, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(string imageFilePath, string prompt, ImageEditOptions options = null);
        public virtual Task<ClientResult<GeneratedImage>> GenerateImageEditAsync(string imageFilePath, string prompt, string maskFilePath, ImageEditOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GenerateImageEdits(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(Stream image, string imageFilename, string prompt, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(string imageFilePath, string prompt, int imageCount, ImageEditOptions options = null);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(string imageFilePath, string prompt, string maskFilePath, int imageCount, ImageEditOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GenerateImageEditsAsync(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(Stream image, string imageFilename, string prompt, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(string imageFilePath, string prompt, int imageCount, ImageEditOptions options = null);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(string imageFilePath, string prompt, string maskFilePath, int imageCount, ImageEditOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GenerateImages(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<GeneratedImageCollection> GenerateImages(string prompt, int imageCount, ImageGenerationOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GenerateImagesAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImagesAsync(string prompt, int imageCount, ImageGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImage> GenerateImageVariation(Stream image, string imageFilename, ImageVariationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImage> GenerateImageVariation(string imageFilePath, ImageVariationOptions options = null);
        public virtual Task<ClientResult<GeneratedImage>> GenerateImageVariationAsync(Stream image, string imageFilename, ImageVariationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImage>> GenerateImageVariationAsync(string imageFilePath, ImageVariationOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GenerateImageVariations(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageVariations(Stream image, string imageFilename, int imageCount, ImageVariationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageVariations(string imageFilePath, int imageCount, ImageVariationOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GenerateImageVariationsAsync(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageVariationsAsync(Stream image, string imageFilename, int imageCount, ImageVariationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageVariationsAsync(string imageFilePath, int imageCount, ImageVariationOptions options = null);
    }
    public class ImageEditOptions : IJsonModel<ImageEditOptions>, IPersistableModel<ImageEditOptions> {
        public string EndUserId { get; set; }
        public GeneratedImageFormat? ResponseFormat { get; set; }
        public GeneratedImageSize? Size { get; set; }
        public static explicit operator ImageEditOptions(ClientResult result);
        public static implicit operator BinaryContent(ImageEditOptions imageEditOptions);
    }
    public class ImageGenerationOptions : IJsonModel<ImageGenerationOptions>, IPersistableModel<ImageGenerationOptions> {
        public string EndUserId { get; set; }
        public GeneratedImageQuality? Quality { get; set; }
        public GeneratedImageFormat? ResponseFormat { get; set; }
        public GeneratedImageSize? Size { get; set; }
        public GeneratedImageStyle? Style { get; set; }
        public static explicit operator ImageGenerationOptions(ClientResult result);
        public static implicit operator BinaryContent(ImageGenerationOptions imageGenerationOptions);
    }
    public class ImageVariationOptions : IJsonModel<ImageVariationOptions>, IPersistableModel<ImageVariationOptions> {
        public string EndUserId { get; set; }
        public GeneratedImageFormat? ResponseFormat { get; set; }
        public GeneratedImageSize? Size { get; set; }
        public static explicit operator ImageVariationOptions(ClientResult result);
        public static implicit operator BinaryContent(ImageVariationOptions imageVariationOptions);
    }
    public static class OpenAIImagesModelFactory {
        public static GeneratedImage GeneratedImage(BinaryData imageBytes = null, Uri imageUri = null, string revisedPrompt = null);
        public static GeneratedImageCollection GeneratedImageCollection(DateTimeOffset createdAt = default, IEnumerable<GeneratedImage> items = null);
    }
}
namespace OpenAI.Models {
    public class ModelDeletionResult : IJsonModel<ModelDeletionResult>, IPersistableModel<ModelDeletionResult> {
        public bool Deleted { get; }
        public string ModelId { get; }
        public static explicit operator ModelDeletionResult(ClientResult result);
        public static implicit operator BinaryContent(ModelDeletionResult modelDeletionResult);
    }
    public class OpenAIModel : IJsonModel<OpenAIModel>, IPersistableModel<OpenAIModel> {
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public string OwnedBy { get; }
        public static explicit operator OpenAIModel(ClientResult result);
        public static implicit operator BinaryContent(OpenAIModel openAIModel);
    }
    public class OpenAIModelClient {
        protected OpenAIModelClient();
        public OpenAIModelClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public OpenAIModelClient(ApiKeyCredential credential);
        protected internal OpenAIModelClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public OpenAIModelClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult DeleteModel(string model, RequestOptions options);
        public virtual ClientResult<ModelDeletionResult> DeleteModel(string model, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> DeleteModelAsync(string model, RequestOptions options);
        public virtual Task<ClientResult<ModelDeletionResult>> DeleteModelAsync(string model, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetModel(string model, RequestOptions options);
        public virtual ClientResult<OpenAIModel> GetModel(string model, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetModelAsync(string model, RequestOptions options);
        public virtual Task<ClientResult<OpenAIModel>> GetModelAsync(string model, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetModels(RequestOptions options);
        public virtual ClientResult<OpenAIModelCollection> GetModels(CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetModelsAsync(RequestOptions options);
        public virtual Task<ClientResult<OpenAIModelCollection>> GetModelsAsync(CancellationToken cancellationToken = default);
    }
    public class OpenAIModelCollection : ObjectModel.ReadOnlyCollection<OpenAIModel>, IJsonModel<OpenAIModelCollection>, IPersistableModel<OpenAIModelCollection> {
        public static explicit operator OpenAIModelCollection(ClientResult result);
        public static implicit operator BinaryContent(OpenAIModelCollection openAIModelCollection);
    }
    public static class OpenAIModelsModelFactory {
        public static ModelDeletionResult ModelDeletionResult(string modelId = null, bool deleted = false);
        public static OpenAIModel OpenAIModel(string id = null, DateTimeOffset createdAt = default, string ownedBy = null);
        public static OpenAIModelCollection OpenAIModelCollection(IEnumerable<OpenAIModel> items = null);
    }
}
namespace OpenAI.Moderations {
    public class ModerationCategory {
        public bool Flagged { get; }
        public float Score { get; }
    }
    public class ModerationClient {
        protected ModerationClient();
        protected internal ModerationClient(ClientPipeline pipeline, string model, OpenAIClientOptions options);
        public ModerationClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public ModerationClient(string model, ApiKeyCredential credential);
        public ModerationClient(string model, string apiKey);
        public ClientPipeline Pipeline { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult ClassifyText(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ModerationResultCollection> ClassifyText(IEnumerable<string> inputs, CancellationToken cancellationToken = default);
        public virtual ClientResult<ModerationResult> ClassifyText(string input, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> ClassifyTextAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ModerationResultCollection>> ClassifyTextAsync(IEnumerable<string> inputs, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<ModerationResult>> ClassifyTextAsync(string input, CancellationToken cancellationToken = default);
    }
    public class ModerationResult : IJsonModel<ModerationResult>, IPersistableModel<ModerationResult> {
        public bool Flagged { get; }
        public ModerationCategory Harassment { get; }
        public ModerationCategory HarassmentThreatening { get; }
        public ModerationCategory Hate { get; }
        public ModerationCategory HateThreatening { get; }
        public ModerationCategory Illicit { get; }
        public ModerationCategory IllicitViolent { get; }
        public ModerationCategory SelfHarm { get; }
        public ModerationCategory SelfHarmInstructions { get; }
        public ModerationCategory SelfHarmIntent { get; }
        public ModerationCategory Sexual { get; }
        public ModerationCategory SexualMinors { get; }
        public ModerationCategory Violence { get; }
        public ModerationCategory ViolenceGraphic { get; }
        public static explicit operator ModerationResult(ClientResult result);
        public static implicit operator BinaryContent(ModerationResult moderationResult);
    }
    public class ModerationResultCollection : ObjectModel.ReadOnlyCollection<ModerationResult>, IJsonModel<ModerationResultCollection>, IPersistableModel<ModerationResultCollection> {
        public string Id { get; }
        public string Model { get; }
        public static explicit operator ModerationResultCollection(ClientResult result);
        public static implicit operator BinaryContent(ModerationResultCollection moderationResultCollection);
    }
    public static class OpenAIModerationsModelFactory {
        public static ModerationCategory ModerationCategory(bool flagged = false, float score = 0);
        public static ModerationResult ModerationResult(bool flagged = false, ModerationCategory hate = null, ModerationCategory hateThreatening = null, ModerationCategory harassment = null, ModerationCategory harassmentThreatening = null, ModerationCategory selfHarm = null, ModerationCategory selfHarmIntent = null, ModerationCategory selfHarmInstructions = null, ModerationCategory sexual = null, ModerationCategory sexualMinors = null, ModerationCategory violence = null, ModerationCategory violenceGraphic = null, ModerationCategory illicit = null, ModerationCategory illicitViolent = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ModerationResult ModerationResult(bool flagged, ModerationCategory hate, ModerationCategory hateThreatening, ModerationCategory harassment, ModerationCategory harassmentThreatening, ModerationCategory selfHarm, ModerationCategory selfHarmIntent, ModerationCategory selfHarmInstructions, ModerationCategory sexual, ModerationCategory sexualMinors, ModerationCategory violence, ModerationCategory violenceGraphic);
        public static ModerationResultCollection ModerationResultCollection(string id = null, string model = null, IEnumerable<ModerationResult> items = null);
    }
}
namespace OpenAI.RealtimeConversation {
    [Experimental("OPENAI002")]
    public readonly partial struct ConversationAudioFormat : IEquatable<ConversationAudioFormat> {
        public ConversationAudioFormat(string value);
        public static ConversationAudioFormat G711Alaw { get; }
        public static ConversationAudioFormat G711Ulaw { get; }
        public static ConversationAudioFormat Pcm16 { get; }
        public readonly bool Equals(ConversationAudioFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ConversationAudioFormat left, ConversationAudioFormat right);
        public static implicit operator ConversationAudioFormat(string value);
        public static bool operator !=(ConversationAudioFormat left, ConversationAudioFormat right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    [Flags]
    public enum ConversationContentModalities {
        Default = 0,
        Text = 1,
        Audio = 2
    }
    [Experimental("OPENAI002")]
    public abstract class ConversationContentPart : IJsonModel<ConversationContentPart>, IPersistableModel<ConversationContentPart> {
        public string AudioTranscript { get; }
        public string Text { get; }
        public static ConversationContentPart CreateInputAudioTranscriptPart(string transcript = null);
        public static ConversationContentPart CreateInputTextPart(string text);
        public static ConversationContentPart CreateOutputAudioTranscriptPart(string transcript = null);
        public static ConversationContentPart CreateOutputTextPart(string text);
        public static explicit operator ConversationContentPart(ClientResult result);
        public static implicit operator BinaryContent(ConversationContentPart conversationContentPart);
        public static implicit operator ConversationContentPart(string text);
    }
    [Experimental("OPENAI002")]
    public readonly partial struct ConversationContentPartKind : IEquatable<ConversationContentPartKind> {
        public ConversationContentPartKind(string value);
        public static ConversationContentPartKind InputAudio { get; }
        public static ConversationContentPartKind InputText { get; }
        public static ConversationContentPartKind OutputAudio { get; }
        public static ConversationContentPartKind OutputText { get; }
        public readonly bool Equals(ConversationContentPartKind other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ConversationContentPartKind left, ConversationContentPartKind right);
        public static implicit operator ConversationContentPartKind(string value);
        public static bool operator !=(ConversationContentPartKind left, ConversationContentPartKind right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class ConversationErrorUpdate : ConversationUpdate, IJsonModel<ConversationErrorUpdate>, IPersistableModel<ConversationErrorUpdate> {
        public string ErrorCode { get; }
        public string ErrorEventId { get; }
        public string Message { get; }
        public string ParameterName { get; }
        public new static explicit operator ConversationErrorUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationErrorUpdate conversationErrorUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationFunctionTool : ConversationTool, IJsonModel<ConversationFunctionTool>, IPersistableModel<ConversationFunctionTool> {
        public ConversationFunctionTool();
        [SetsRequiredMembers]
        public ConversationFunctionTool(string name);
        public string Description { get; set; }
        public required string Name { get; set; }
        public BinaryData Parameters { get; set; }
        public new static explicit operator ConversationFunctionTool(ClientResult result);
        public static implicit operator BinaryContent(ConversationFunctionTool conversationFunctionTool);
    }
    [Experimental("OPENAI002")]
    public readonly partial struct ConversationIncompleteReason : IEquatable<ConversationIncompleteReason> {
        public ConversationIncompleteReason(string value);
        public static ConversationIncompleteReason ClientCancelled { get; }
        public static ConversationIncompleteReason ContentFilter { get; }
        public static ConversationIncompleteReason MaxOutputTokens { get; }
        public static ConversationIncompleteReason TurnDetected { get; }
        public readonly bool Equals(ConversationIncompleteReason other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ConversationIncompleteReason left, ConversationIncompleteReason right);
        public static implicit operator ConversationIncompleteReason(string value);
        public static bool operator !=(ConversationIncompleteReason left, ConversationIncompleteReason right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class ConversationInputAudioClearedUpdate : ConversationUpdate, IJsonModel<ConversationInputAudioClearedUpdate>, IPersistableModel<ConversationInputAudioClearedUpdate> {
        public new static explicit operator ConversationInputAudioClearedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationInputAudioClearedUpdate conversationInputAudioClearedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationInputAudioCommittedUpdate : ConversationUpdate, IJsonModel<ConversationInputAudioCommittedUpdate>, IPersistableModel<ConversationInputAudioCommittedUpdate> {
        public string ItemId { get; }
        public string PreviousItemId { get; }
        public new static explicit operator ConversationInputAudioCommittedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationInputAudioCommittedUpdate conversationInputAudioCommittedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationInputSpeechFinishedUpdate : ConversationUpdate, IJsonModel<ConversationInputSpeechFinishedUpdate>, IPersistableModel<ConversationInputSpeechFinishedUpdate> {
        public TimeSpan AudioEndTime { get; }
        public string ItemId { get; }
        public new static explicit operator ConversationInputSpeechFinishedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationInputSpeechFinishedUpdate conversationInputSpeechFinishedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationInputSpeechStartedUpdate : ConversationUpdate, IJsonModel<ConversationInputSpeechStartedUpdate>, IPersistableModel<ConversationInputSpeechStartedUpdate> {
        public TimeSpan AudioStartTime { get; }
        public string ItemId { get; }
        public new static explicit operator ConversationInputSpeechStartedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationInputSpeechStartedUpdate conversationInputSpeechStartedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationInputTokenUsageDetails : IJsonModel<ConversationInputTokenUsageDetails>, IPersistableModel<ConversationInputTokenUsageDetails> {
        public int AudioTokenCount { get; }
        public int CachedTokenCount { get; }
        public int TextTokenCount { get; }
        public static explicit operator ConversationInputTokenUsageDetails(ClientResult result);
        public static implicit operator BinaryContent(ConversationInputTokenUsageDetails conversationInputTokenUsageDetails);
    }
    [Experimental("OPENAI002")]
    public class ConversationInputTranscriptionFailedUpdate : ConversationUpdate, IJsonModel<ConversationInputTranscriptionFailedUpdate>, IPersistableModel<ConversationInputTranscriptionFailedUpdate> {
        public int ContentIndex { get; }
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public string ErrorParameterName { get; }
        public string ItemId { get; }
        public new static explicit operator ConversationInputTranscriptionFailedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationInputTranscriptionFailedUpdate conversationInputTranscriptionFailedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationInputTranscriptionFinishedUpdate : ConversationUpdate, IJsonModel<ConversationInputTranscriptionFinishedUpdate>, IPersistableModel<ConversationInputTranscriptionFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public string Transcript { get; }
        public new static explicit operator ConversationInputTranscriptionFinishedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationInputTranscriptionFinishedUpdate conversationInputTranscriptionFinishedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationInputTranscriptionOptions : IJsonModel<ConversationInputTranscriptionOptions>, IPersistableModel<ConversationInputTranscriptionOptions> {
        public ConversationTranscriptionModel? Model { get; set; }
        public static explicit operator ConversationInputTranscriptionOptions(ClientResult result);
        public static implicit operator BinaryContent(ConversationInputTranscriptionOptions conversationInputTranscriptionOptions);
    }
    [Experimental("OPENAI002")]
    public abstract class ConversationItem : IJsonModel<ConversationItem>, IPersistableModel<ConversationItem> {
        public string FunctionArguments { get; }
        public string FunctionCallId { get; }
        public string FunctionName { get; }
        public string Id { get; set; }
        public IReadOnlyList<ConversationContentPart> MessageContentParts { get; }
        public ConversationMessageRole? MessageRole { get; }
        public static ConversationItem CreateAssistantMessage(IEnumerable<ConversationContentPart> contentItems);
        public static ConversationItem CreateFunctionCall(string name, string callId, string arguments);
        public static ConversationItem CreateFunctionCallOutput(string callId, string output);
        public static ConversationItem CreateSystemMessage(IEnumerable<ConversationContentPart> contentItems);
        public static ConversationItem CreateUserMessage(IEnumerable<ConversationContentPart> contentItems);
        public static explicit operator ConversationItem(ClientResult result);
        public static implicit operator BinaryContent(ConversationItem conversationItem);
    }
    [Experimental("OPENAI002")]
    public class ConversationItemCreatedUpdate : ConversationUpdate, IJsonModel<ConversationItemCreatedUpdate>, IPersistableModel<ConversationItemCreatedUpdate> {
        public string FunctionCallArguments { get; }
        public string FunctionCallId { get; }
        public string FunctionCallOutput { get; }
        public string FunctionName { get; }
        public string ItemId { get; }
        public IReadOnlyList<ConversationContentPart> MessageContentParts { get; }
        public ConversationMessageRole? MessageRole { get; }
        public string PreviousItemId { get; }
        public new static explicit operator ConversationItemCreatedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationItemCreatedUpdate conversationItemCreatedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationItemDeletedUpdate : ConversationUpdate, IJsonModel<ConversationItemDeletedUpdate>, IPersistableModel<ConversationItemDeletedUpdate> {
        public string ItemId { get; }
        public new static explicit operator ConversationItemDeletedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationItemDeletedUpdate conversationItemDeletedUpdate);
    }
    [Experimental("OPENAI002")]
    public readonly partial struct ConversationItemStatus : IEquatable<ConversationItemStatus> {
        public ConversationItemStatus(string value);
        public static ConversationItemStatus Completed { get; }
        public static ConversationItemStatus Incomplete { get; }
        public static ConversationItemStatus InProgress { get; }
        public readonly bool Equals(ConversationItemStatus other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ConversationItemStatus left, ConversationItemStatus right);
        public static implicit operator ConversationItemStatus(string value);
        public static bool operator !=(ConversationItemStatus left, ConversationItemStatus right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class ConversationItemStreamingAudioFinishedUpdate : ConversationUpdate, IJsonModel<ConversationItemStreamingAudioFinishedUpdate>, IPersistableModel<ConversationItemStreamingAudioFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        public new static explicit operator ConversationItemStreamingAudioFinishedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationItemStreamingAudioFinishedUpdate conversationItemStreamingAudioFinishedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationItemStreamingAudioTranscriptionFinishedUpdate : ConversationUpdate, IJsonModel<ConversationItemStreamingAudioTranscriptionFinishedUpdate>, IPersistableModel<ConversationItemStreamingAudioTranscriptionFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        public string Transcript { get; }
        public new static explicit operator ConversationItemStreamingAudioTranscriptionFinishedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationItemStreamingAudioTranscriptionFinishedUpdate conversationItemStreamingAudioTranscriptionFinishedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationItemStreamingFinishedUpdate : ConversationUpdate, IJsonModel<ConversationItemStreamingFinishedUpdate>, IPersistableModel<ConversationItemStreamingFinishedUpdate> {
        public string FunctionCallArguments { get; }
        public string FunctionCallId { get; }
        public string FunctionCallOutput { get; }
        public string FunctionName { get; }
        public string ItemId { get; }
        public IReadOnlyList<ConversationContentPart> MessageContentParts { get; }
        public ConversationMessageRole? MessageRole { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        public new static explicit operator ConversationItemStreamingFinishedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationItemStreamingFinishedUpdate conversationItemStreamingFinishedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationItemStreamingPartDeltaUpdate : ConversationUpdate, IJsonModel<ConversationItemStreamingPartDeltaUpdate>, IPersistableModel<ConversationItemStreamingPartDeltaUpdate> {
        public BinaryData AudioBytes { get; }
        public string AudioTranscript { get; }
        public int ContentPartIndex { get; }
        public string FunctionArguments { get; }
        public string FunctionCallId { get; }
        public string ItemId { get; }
        public int ItemIndex { get; }
        public string ResponseId { get; }
        public string Text { get; }
    }
    public class ConversationItemStreamingPartFinishedUpdate : ConversationUpdate, IJsonModel<ConversationItemStreamingPartFinishedUpdate>, IPersistableModel<ConversationItemStreamingPartFinishedUpdate> {
        public string AudioTranscript { get; }
        public int ContentPartIndex { get; }
        public string FunctionArguments { get; }
        public string FunctionCallId { get; }
        public string ItemId { get; }
        public int ItemIndex { get; }
        public string ResponseId { get; }
        public string Text { get; }
    }
    [Experimental("OPENAI002")]
    public class ConversationItemStreamingStartedUpdate : ConversationUpdate, IJsonModel<ConversationItemStreamingStartedUpdate>, IPersistableModel<ConversationItemStreamingStartedUpdate> {
        public string FunctionCallArguments { get; }
        public string FunctionCallId { get; }
        public string FunctionCallOutput { get; }
        public string FunctionName { get; }
        public string ItemId { get; }
        public int ItemIndex { get; }
        public IReadOnlyList<ConversationContentPart> MessageContentParts { get; }
        public ConversationMessageRole? MessageRole { get; }
        public string ResponseId { get; }
        public new static explicit operator ConversationItemStreamingStartedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationItemStreamingStartedUpdate conversationItemStreamingStartedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationItemStreamingTextFinishedUpdate : ConversationUpdate, IJsonModel<ConversationItemStreamingTextFinishedUpdate>, IPersistableModel<ConversationItemStreamingTextFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        public string Text { get; }
        public new static explicit operator ConversationItemStreamingTextFinishedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationItemStreamingTextFinishedUpdate conversationItemStreamingTextFinishedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationItemTruncatedUpdate : ConversationUpdate, IJsonModel<ConversationItemTruncatedUpdate>, IPersistableModel<ConversationItemTruncatedUpdate> {
        public int AudioEndMs { get; }
        public int ContentIndex { get; }
        public string ItemId { get; }
        public new static explicit operator ConversationItemTruncatedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationItemTruncatedUpdate conversationItemTruncatedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationMaxTokensChoice : IJsonModel<ConversationMaxTokensChoice>, IPersistableModel<ConversationMaxTokensChoice> {
        public ConversationMaxTokensChoice(int numberValue);
        public int? NumericValue { get; }
        public static ConversationMaxTokensChoice CreateDefaultMaxTokensChoice();
        public static ConversationMaxTokensChoice CreateInfiniteMaxTokensChoice();
        public static ConversationMaxTokensChoice CreateNumericMaxTokensChoice(int maxTokens);
        public static implicit operator ConversationMaxTokensChoice(int maxTokens);
    }
    [Experimental("OPENAI002")]
    public readonly partial struct ConversationMessageRole : IEquatable<ConversationMessageRole> {
        public ConversationMessageRole(string value);
        public static ConversationMessageRole Assistant { get; }
        public static ConversationMessageRole System { get; }
        public static ConversationMessageRole User { get; }
        public readonly bool Equals(ConversationMessageRole other);
        [EditorBrowsable(global::EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(global::EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ConversationMessageRole left, ConversationMessageRole right);
        public static implicit operator ConversationMessageRole(string value);
        public static bool operator !=(ConversationMessageRole left, ConversationMessageRole right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class ConversationOutputTokenUsageDetails : IJsonModel<ConversationOutputTokenUsageDetails>, IPersistableModel<ConversationOutputTokenUsageDetails> {
        public int AudioTokenCount { get; }
        public int TextTokenCount { get; }
        public static explicit operator ConversationOutputTokenUsageDetails(ClientResult result);
        public static implicit operator BinaryContent(ConversationOutputTokenUsageDetails conversationOutputTokenUsageDetails);
    }
    [Experimental("OPENAI002")]
    public class ConversationRateLimitDetailsItem : IJsonModel<ConversationRateLimitDetailsItem>, IPersistableModel<ConversationRateLimitDetailsItem> {
        public int MaximumCount { get; }
        public string Name { get; }
        public int RemainingCount { get; }
        public TimeSpan TimeUntilReset { get; }
        public static explicit operator ConversationRateLimitDetailsItem(ClientResult result);
        public static implicit operator BinaryContent(ConversationRateLimitDetailsItem conversationRateLimitDetailsItem);
    }
    [Experimental("OPENAI002")]
    public class ConversationRateLimitsUpdate : ConversationUpdate, IJsonModel<ConversationRateLimitsUpdate>, IPersistableModel<ConversationRateLimitsUpdate> {
        public IReadOnlyList<ConversationRateLimitDetailsItem> AllDetails { get; }
        public ConversationRateLimitDetailsItem RequestDetails { get; }
        public ConversationRateLimitDetailsItem TokenDetails { get; }
        public new static explicit operator ConversationRateLimitsUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationRateLimitsUpdate conversationRateLimitsUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationResponseFinishedUpdate : ConversationUpdate, IJsonModel<ConversationResponseFinishedUpdate>, IPersistableModel<ConversationResponseFinishedUpdate> {
        public IReadOnlyList<ConversationItem> CreatedItems { get; }
        public string ResponseId { get; }
        public ConversationStatus? Status { get; }
        public ConversationStatusDetails StatusDetails { get; }
        public ConversationTokenUsage Usage { get; }
        public new static explicit operator ConversationResponseFinishedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationResponseFinishedUpdate conversationResponseFinishedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationResponseOptions : IJsonModel<ConversationResponseOptions>, IPersistableModel<ConversationResponseOptions> {
        public ConversationContentModalities ContentModalities { get; set; }
        public ResponseConversationSelection? ConversationSelection { get; set; }
        public string Instructions { get; set; }
        public ConversationMaxTokensChoice MaxOutputTokens { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public ConversationAudioFormat? OutputAudioFormat { get; set; }
        public IList<ConversationItem> OverrideItems { get; }
        public float? Temperature { get; set; }
        public ConversationToolChoice ToolChoice { get; set; }
        public IList<ConversationTool> Tools { get; }
        public ConversationVoice? Voice { get; set; }
        public static explicit operator ConversationResponseOptions(ClientResult result);
        public static implicit operator BinaryContent(ConversationResponseOptions conversationResponseOptions);
    }
    [Experimental("OPENAI002")]
    public class ConversationResponseStartedUpdate : ConversationUpdate, IJsonModel<ConversationResponseStartedUpdate>, IPersistableModel<ConversationResponseStartedUpdate> {
        public IReadOnlyList<ConversationItem> CreatedItems { get; }
        public string ResponseId { get; }
        public ConversationStatus Status { get; }
        public ConversationStatusDetails StatusDetails { get; }
        public ConversationTokenUsage Usage { get; }
        public new static explicit operator ConversationResponseStartedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationResponseStartedUpdate conversationResponseStartedUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationSessionConfiguredUpdate : ConversationUpdate, IJsonModel<ConversationSessionConfiguredUpdate>, IPersistableModel<ConversationSessionConfiguredUpdate> {
        public ConversationContentModalities ContentModalities { get; }
        public ConversationAudioFormat InputAudioFormat { get; }
        public ConversationInputTranscriptionOptions InputTranscriptionOptions { get; }
        public string Instructions { get; }
        public ConversationMaxTokensChoice MaxOutputTokens { get; }
        public string Model { get; }
        public ConversationAudioFormat OutputAudioFormat { get; }
        public string SessionId { get; }
        public float Temperature { get; }
        public ConversationToolChoice ToolChoice { get; }
        public IReadOnlyList<ConversationTool> Tools { get; }
        public ConversationTurnDetectionOptions TurnDetectionOptions { get; }
        public ConversationVoice Voice { get; }
        public new static explicit operator ConversationSessionConfiguredUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationSessionConfiguredUpdate conversationSessionConfiguredUpdate);
    }
    [Experimental("OPENAI002")]
    public class ConversationSessionOptions : IJsonModel<ConversationSessionOptions>, IPersistableModel<ConversationSessionOptions> {
        public ConversationContentModalities ContentModalities { get; set; }
        public ConversationAudioFormat? InputAudioFormat { get; set; }
        public ConversationInputTranscriptionOptions InputTranscriptionOptions { get; set; }
        public string Instructions { get; set; }
        public ConversationMaxTokensChoice MaxOutputTokens { get; set; }
        public ConversationAudioFormat? OutputAudioFormat { get; set; }
        public float? Temperature { get; set; }
        public ConversationToolChoice ToolChoice { get; set; }
        public IList<ConversationTool> Tools { get; }
        public ConversationTurnDetectionOptions TurnDetectionOptions { get; set; }
        public ConversationVoice? Voice { get; set; }
        public static explicit operator ConversationSessionOptions(ClientResult result);
        public static implicit operator BinaryContent(ConversationSessionOptions conversationSessionOptions);
    }
    [Experimental("OPENAI002")]
    public class ConversationSessionStartedUpdate : ConversationUpdate, IJsonModel<ConversationSessionStartedUpdate>, IPersistableModel<ConversationSessionStartedUpdate> {
        public ConversationContentModalities ContentModalities { get; }
        public ConversationAudioFormat InputAudioFormat { get; }
        public ConversationInputTranscriptionOptions InputTranscriptionOptions { get; }
        public string Instructions { get; }
        public ConversationMaxTokensChoice MaxOutputTokens { get; }
        public string Model { get; }
        public ConversationAudioFormat OutputAudioFormat { get; }
        public string SessionId { get; }
        public float Temperature { get; }
        public ConversationToolChoice ToolChoice { get; }
        public IReadOnlyList<ConversationTool> Tools { get; }
        public ConversationTurnDetectionOptions TurnDetectionOptions { get; }
        public ConversationVoice Voice { get; }
        public new static explicit operator ConversationSessionStartedUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationSessionStartedUpdate conversationSessionStartedUpdate);
    }
    [Experimental("OPENAI002")]
    public readonly partial struct ConversationStatus : IEquatable<ConversationStatus> {
        public ConversationStatus(string value);
        public static ConversationStatus Cancelled { get; }
        public static ConversationStatus Completed { get; }
        public static ConversationStatus Failed { get; }
        public static ConversationStatus Incomplete { get; }
        public readonly bool Equals(ConversationStatus other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ConversationStatus left, ConversationStatus right);
        public static implicit operator ConversationStatus(string value);
        public static bool operator !=(ConversationStatus left, ConversationStatus right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class ConversationStatusDetails : IJsonModel<ConversationStatusDetails>, IPersistableModel<ConversationStatusDetails> {
        public string ErrorCode { get; }
        public string ErrorKind { get; }
        public ConversationIncompleteReason? IncompleteReason { get; }
        public ConversationStatus StatusKind { get; }
        public static explicit operator ConversationStatusDetails(ClientResult result);
        public static implicit operator BinaryContent(ConversationStatusDetails conversationStatusDetails);
    }
    [Experimental("OPENAI002")]
    public class ConversationTokenUsage : IJsonModel<ConversationTokenUsage>, IPersistableModel<ConversationTokenUsage> {
        public int InputTokenCount { get; }
        public ConversationInputTokenUsageDetails InputTokenDetails { get; }
        public int OutputTokenCount { get; }
        public ConversationOutputTokenUsageDetails OutputTokenDetails { get; }
        public int TotalTokenCount { get; }
        public static explicit operator ConversationTokenUsage(ClientResult result);
        public static implicit operator BinaryContent(ConversationTokenUsage conversationTokenUsage);
    }
    [Experimental("OPENAI002")]
    public abstract class ConversationTool : IJsonModel<ConversationTool>, IPersistableModel<ConversationTool> {
        public ConversationToolKind Kind { get; }
        public static ConversationTool CreateFunctionTool(string name, string description = null, BinaryData parameters = null);
        public static explicit operator ConversationTool(ClientResult result);
        public static implicit operator BinaryContent(ConversationTool conversationTool);
    }
    [Experimental("OPENAI002")]
    public class ConversationToolChoice : IJsonModel<ConversationToolChoice>, IPersistableModel<ConversationToolChoice> {
        public string FunctionName { get; }
        public ConversationToolChoiceKind Kind { get; }
        public static ConversationToolChoice CreateAutoToolChoice();
        public static ConversationToolChoice CreateFunctionToolChoice(string functionName);
        public static ConversationToolChoice CreateNoneToolChoice();
        public static ConversationToolChoice CreateRequiredToolChoice();
    }
    [Experimental("OPENAI002")]
    public enum ConversationToolChoiceKind {
        Unknown = 0,
        Auto = 1,
        None = 2,
        Required = 3,
        Function = 4
    }
    [Experimental("OPENAI002")]
    public readonly partial struct ConversationToolKind : IEquatable<ConversationToolKind> {
        public ConversationToolKind(string value);
        public static ConversationToolKind Function { get; }
        public readonly bool Equals(ConversationToolKind other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ConversationToolKind left, ConversationToolKind right);
        public static implicit operator ConversationToolKind(string value);
        public static bool operator !=(ConversationToolKind left, ConversationToolKind right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public readonly partial struct ConversationTranscriptionModel : IEquatable<ConversationTranscriptionModel> {
        public ConversationTranscriptionModel(string value);
        public static ConversationTranscriptionModel Whisper1 { get; }
        public readonly bool Equals(ConversationTranscriptionModel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ConversationTranscriptionModel left, ConversationTranscriptionModel right);
        public static implicit operator ConversationTranscriptionModel(string value);
        public static bool operator !=(ConversationTranscriptionModel left, ConversationTranscriptionModel right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public enum ConversationTurnDetectionKind {
        ServerVoiceActivityDetection = 0,
        Disabled = 1
    }
    [Experimental("OPENAI002")]
    public abstract class ConversationTurnDetectionOptions : IJsonModel<ConversationTurnDetectionOptions>, IPersistableModel<ConversationTurnDetectionOptions> {
        public ConversationTurnDetectionKind Kind { get; protected internal set; }
        public static ConversationTurnDetectionOptions CreateDisabledTurnDetectionOptions();
        public static ConversationTurnDetectionOptions CreateServerVoiceActivityTurnDetectionOptions(float? detectionThreshold = null, TimeSpan? prefixPaddingDuration = null, TimeSpan? silenceDuration = null, bool? enableAutomaticResponseCreation = null);
        public static explicit operator ConversationTurnDetectionOptions(ClientResult result);
        public static implicit operator BinaryContent(ConversationTurnDetectionOptions conversationTurnDetectionOptions);
    }
    [Experimental("OPENAI002")]
    public abstract class ConversationUpdate : IJsonModel<ConversationUpdate>, IPersistableModel<ConversationUpdate> {
        public string EventId { get; }
        public ConversationUpdateKind Kind { get; protected internal set; }
        public BinaryData GetRawContent();
        public static explicit operator ConversationUpdate(ClientResult result);
        public static implicit operator BinaryContent(ConversationUpdate conversationUpdate);
    }
    [Experimental("OPENAI002")]
    public enum ConversationUpdateKind {
        Unknown = 0,
        SessionStarted = 1,
        SessionConfigured = 2,
        ItemCreated = 3,
        ConversationCreated = 4,
        ItemDeleted = 5,
        ItemTruncated = 6,
        ResponseStarted = 7,
        ResponseFinished = 8,
        RateLimitsUpdated = 9,
        ItemStreamingStarted = 10,
        ItemStreamingFinished = 11,
        ItemContentPartStarted = 12,
        ItemContentPartFinished = 13,
        ItemStreamingPartAudioDelta = 14,
        ItemStreamingPartAudioFinished = 15,
        ItemStreamingPartAudioTranscriptionDelta = 16,
        ItemStreamingPartAudioTranscriptionFinished = 17,
        ItemStreamingPartTextDelta = 18,
        ItemStreamingPartTextFinished = 19,
        ItemStreamingFunctionCallArgumentsDelta = 20,
        ItemStreamingFunctionCallArgumentsFinished = 21,
        InputSpeechStarted = 22,
        InputSpeechStopped = 23,
        InputTranscriptionFinished = 24,
        InputTranscriptionFailed = 25,
        InputAudioCommitted = 26,
        InputAudioCleared = 27,
        Error = 28
    }
    [Experimental("OPENAI002")]
    public readonly partial struct ConversationVoice : IEquatable<ConversationVoice> {
        public ConversationVoice(string value);
        public static ConversationVoice Alloy { get; }
        public static ConversationVoice Ash { get; }
        public static ConversationVoice Ballad { get; }
        public static ConversationVoice Coral { get; }
        public static ConversationVoice Echo { get; }
        public static ConversationVoice Sage { get; }
        public static ConversationVoice Shimmer { get; }
        public static ConversationVoice Verse { get; }
        public readonly bool Equals(ConversationVoice other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ConversationVoice left, ConversationVoice right);
        public static implicit operator ConversationVoice(string value);
        public static bool operator !=(ConversationVoice left, ConversationVoice right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class RealtimeConversationClient {
        protected RealtimeConversationClient();
        protected internal RealtimeConversationClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public RealtimeConversationClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public RealtimeConversationClient(string model, ApiKeyCredential credential);
        public ClientPipeline Pipeline { get; }
        public event EventHandler<BinaryData> OnReceivingCommand { add; remove; }
        public event EventHandler<BinaryData> OnSendingCommand { add; remove; }
        public RealtimeConversationSession StartConversationSession(CancellationToken cancellationToken = default);
        public virtual Task<RealtimeConversationSession> StartConversationSessionAsync(RequestOptions options);
        public virtual Task<RealtimeConversationSession> StartConversationSessionAsync(CancellationToken cancellationToken = default);
    }
    [Experimental("OPENAI002")]
    public class RealtimeConversationSession : IDisposable {
        protected internal RealtimeConversationSession(RealtimeConversationClient parentClient, Uri endpoint, ApiKeyCredential credential);
        public Net.WebSockets.WebSocket WebSocket { get; protected set; }
        public virtual void AddItem(ConversationItem item, string previousItemId, CancellationToken cancellationToken = default);
        public virtual void AddItem(ConversationItem item, CancellationToken cancellationToken = default);
        public virtual Task AddItemAsync(ConversationItem item, string previousItemId, CancellationToken cancellationToken = default);
        public virtual Task AddItemAsync(ConversationItem item, CancellationToken cancellationToken = default);
        public virtual void CancelResponse(CancellationToken cancellationToken = default);
        public virtual Task CancelResponseAsync(CancellationToken cancellationToken = default);
        public virtual void ClearInputAudio(CancellationToken cancellationToken = default);
        public virtual Task ClearInputAudioAsync(CancellationToken cancellationToken = default);
        public virtual void CommitPendingAudio(CancellationToken cancellationToken = default);
        public virtual Task CommitPendingAudioAsync(CancellationToken cancellationToken = default);
        public virtual void ConfigureSession(ConversationSessionOptions sessionOptions, CancellationToken cancellationToken = default);
        public virtual Task ConfigureSessionAsync(ConversationSessionOptions sessionOptions, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal virtual void Connect(RequestOptions options);
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal virtual Task ConnectAsync(RequestOptions options);
        public virtual void DeleteItem(string itemId, CancellationToken cancellationToken = default);
        public virtual Task DeleteItemAsync(string itemId, CancellationToken cancellationToken = default);
        public void Dispose();
        public virtual void InterruptResponse(CancellationToken cancellationToken = default);
        public virtual Task InterruptResponseAsync(CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual IEnumerable<ClientResult> ReceiveUpdates(RequestOptions options);
        public virtual IEnumerable<ConversationUpdate> ReceiveUpdates(CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual IAsyncEnumerable<ClientResult> ReceiveUpdatesAsync(RequestOptions options);
        public virtual IAsyncEnumerable<ConversationUpdate> ReceiveUpdatesAsync(CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void SendCommand(BinaryData data, RequestOptions options);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task SendCommandAsync(BinaryData data, RequestOptions options);
        public virtual void SendInputAudio(BinaryData audio, CancellationToken cancellationToken = default);
        public virtual void SendInputAudio(Stream audio, CancellationToken cancellationToken = default);
        public virtual Task SendInputAudioAsync(BinaryData audio, CancellationToken cancellationToken = default);
        public virtual Task SendInputAudioAsync(Stream audio, CancellationToken cancellationToken = default);
        public virtual void StartResponse(ConversationResponseOptions options, CancellationToken cancellationToken = default);
        public void StartResponse(CancellationToken cancellationToken = default);
        public virtual Task StartResponseAsync(ConversationResponseOptions options, CancellationToken cancellationToken = default);
        public virtual Task StartResponseAsync(CancellationToken cancellationToken = default);
        public virtual void TruncateItem(string itemId, int contentPartIndex, TimeSpan audioDuration, CancellationToken cancellationToken = default);
        public virtual Task TruncateItemAsync(string itemId, int contentPartIndex, TimeSpan audioDuration, CancellationToken cancellationToken = default);
    }
    [Experimental("OPENAI002")]
    public readonly partial struct ResponseConversationSelection : IEquatable<ResponseConversationSelection> {
        public ResponseConversationSelection(string value);
        public static ResponseConversationSelection Auto { get; }
        public static ResponseConversationSelection None { get; }
        public readonly bool Equals(ResponseConversationSelection other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ResponseConversationSelection left, ResponseConversationSelection right);
        public static implicit operator ResponseConversationSelection(string value);
        public static bool operator !=(ResponseConversationSelection left, ResponseConversationSelection right);
        public override readonly string ToString();
    }
}
namespace OpenAI.VectorStores {
    [Experimental("OPENAI001")]
    public class AddFileToVectorStoreOperation : OperationResult {
        public string FileId { get; }
        public override ContinuationToken? RehydrationToken { get; protected set; }
        public VectorStoreFileAssociationStatus? Status { get; }
        public VectorStoreFileAssociation? Value { get; }
        public string VectorStoreId { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetFileAssociation(RequestOptions? options);
        public virtual ClientResult<VectorStoreFileAssociation> GetFileAssociation(CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetFileAssociationAsync(RequestOptions? options);
        public virtual Task<ClientResult<VectorStoreFileAssociation>> GetFileAssociationAsync(CancellationToken cancellationToken = default);
        public static AddFileToVectorStoreOperation Rehydrate(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static Task<AddFileToVectorStoreOperation> RehydrateAsync(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public override ClientResult UpdateStatus(RequestOptions? options = null);
        public override ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null);
    }
    [Experimental("OPENAI001")]
    public class CreateBatchFileJobOperation : OperationResult {
        public string BatchId { get; }
        public override ContinuationToken? RehydrationToken { get; protected set; }
        public VectorStoreBatchFileJobStatus? Status { get; }
        public VectorStoreBatchFileJob? Value { get; }
        public string VectorStoreId { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult Cancel(RequestOptions? options);
        public virtual ClientResult<VectorStoreBatchFileJob> Cancel(CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> CancelAsync(RequestOptions? options);
        public virtual Task<ClientResult<VectorStoreBatchFileJob>> CancelAsync(CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetFileBatch(RequestOptions? options);
        public virtual ClientResult<VectorStoreBatchFileJob> GetFileBatch(CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetFileBatchAsync(RequestOptions? options);
        public virtual Task<ClientResult<VectorStoreBatchFileJob>> GetFileBatchAsync(CancellationToken cancellationToken = default);
        public static CreateBatchFileJobOperation Rehydrate(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static Task<CreateBatchFileJobOperation> RehydrateAsync(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public override ClientResult UpdateStatus(RequestOptions? options = null);
        public override ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null);
    }
    [Experimental("OPENAI001")]
    public class CreateVectorStoreOperation : OperationResult {
        public override ContinuationToken? RehydrationToken { get; protected set; }
        public VectorStoreStatus? Status { get; }
        public VectorStore? Value { get; }
        public string VectorStoreId { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetVectorStore(RequestOptions? options);
        public virtual ClientResult<VectorStore> GetVectorStore(CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetVectorStoreAsync(RequestOptions? options);
        public virtual Task<ClientResult<VectorStore>> GetVectorStoreAsync(CancellationToken cancellationToken = default);
        public static CreateVectorStoreOperation Rehydrate(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static Task<CreateVectorStoreOperation> RehydrateAsync(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public override ClientResult UpdateStatus(RequestOptions? options = null);
        public override ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null);
    }
    [Experimental("OPENAI001")]
    public abstract class FileChunkingStrategy : IJsonModel<FileChunkingStrategy>, IPersistableModel<FileChunkingStrategy> {
        public static FileChunkingStrategy Auto { get; }
        public static FileChunkingStrategy Unknown { get; }
        public static FileChunkingStrategy CreateStaticStrategy(int maxTokensPerChunk, int overlappingTokenCount);
        public static explicit operator FileChunkingStrategy(ClientResult result);
        public static implicit operator BinaryContent(FileChunkingStrategy fileChunkingStrategy);
    }
    [Experimental("OPENAI001")]
    public class FileFromStoreRemovalResult : IJsonModel<FileFromStoreRemovalResult>, IPersistableModel<FileFromStoreRemovalResult> {
        public string FileId { get; }
        public bool Removed { get; }
        public static explicit operator FileFromStoreRemovalResult(ClientResult result);
        public static implicit operator BinaryContent(FileFromStoreRemovalResult fileFromStoreRemovalResult);
    }
    [Experimental("OPENAI001")]
    public class StaticFileChunkingStrategy : FileChunkingStrategy, IJsonModel<StaticFileChunkingStrategy>, IPersistableModel<StaticFileChunkingStrategy> {
        public StaticFileChunkingStrategy(int maxTokensPerChunk, int overlappingTokenCount);
        public int MaxTokensPerChunk { get; }
        public int OverlappingTokenCount { get; }
        public new static explicit operator StaticFileChunkingStrategy(ClientResult result);
        public static implicit operator BinaryContent(StaticFileChunkingStrategy staticFileChunkingStrategy);
    }
    [Experimental("OPENAI001")]
    public class VectorStore : IJsonModel<VectorStore>, IPersistableModel<VectorStore> {
        public DateTimeOffset CreatedAt { get; }
        public VectorStoreExpirationPolicy ExpirationPolicy { get; }
        public DateTimeOffset? ExpiresAt { get; }
        public VectorStoreFileCounts FileCounts { get; }
        public string Id { get; }
        public DateTimeOffset? LastActiveAt { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public string Name { get; }
        public VectorStoreStatus Status { get; }
        public int UsageBytes { get; }
        public static explicit operator VectorStore(ClientResult result);
        public static implicit operator BinaryContent(VectorStore vectorStore);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreBatchFileJob : IJsonModel<VectorStoreBatchFileJob>, IPersistableModel<VectorStoreBatchFileJob> {
        public string BatchId { get; }
        public DateTimeOffset CreatedAt { get; }
        public VectorStoreFileCounts FileCounts { get; }
        public VectorStoreBatchFileJobStatus Status { get; }
        public string VectorStoreId { get; }
        public static explicit operator VectorStoreBatchFileJob(ClientResult result);
        public static implicit operator BinaryContent(VectorStoreBatchFileJob vectorStoreBatchFileJob);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct VectorStoreBatchFileJobStatus : IEquatable<VectorStoreBatchFileJobStatus> {
        public VectorStoreBatchFileJobStatus(string value);
        public static VectorStoreBatchFileJobStatus Cancelled { get; }
        public static VectorStoreBatchFileJobStatus Completed { get; }
        public static VectorStoreBatchFileJobStatus Failed { get; }
        public static VectorStoreBatchFileJobStatus InProgress { get; }
        public readonly bool Equals(VectorStoreBatchFileJobStatus other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(VectorStoreBatchFileJobStatus left, VectorStoreBatchFileJobStatus right);
        public static implicit operator VectorStoreBatchFileJobStatus(string value);
        public static bool operator !=(VectorStoreBatchFileJobStatus left, VectorStoreBatchFileJobStatus right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class VectorStoreClient {
        protected VectorStoreClient();
        public VectorStoreClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public VectorStoreClient(ApiKeyCredential credential);
        protected internal VectorStoreClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public VectorStoreClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AddFileToVectorStoreOperation AddFileToVectorStore(string vectorStoreId, BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual AddFileToVectorStoreOperation AddFileToVectorStore(string vectorStoreId, string fileId, bool waitUntilCompleted, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<AddFileToVectorStoreOperation> AddFileToVectorStoreAsync(string vectorStoreId, BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual Task<AddFileToVectorStoreOperation> AddFileToVectorStoreAsync(string vectorStoreId, string fileId, bool waitUntilCompleted, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult CancelBatchFileJob(string vectorStoreId, string batchId, RequestOptions options);
        public virtual ClientResult<VectorStoreBatchFileJob> CancelBatchFileJob(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> CancelBatchFileJobAsync(string vectorStoreId, string batchId, RequestOptions options);
        public virtual Task<ClientResult<VectorStoreBatchFileJob>> CancelBatchFileJobAsync(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CreateBatchFileJobOperation CreateBatchFileJob(string vectorStoreId, BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual CreateBatchFileJobOperation CreateBatchFileJob(string vectorStoreId, IEnumerable<string> fileIds, bool waitUntilCompleted, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<CreateBatchFileJobOperation> CreateBatchFileJobAsync(string vectorStoreId, BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual Task<CreateBatchFileJobOperation> CreateBatchFileJobAsync(string vectorStoreId, IEnumerable<string> fileIds, bool waitUntilCompleted, CancellationToken cancellationToken = default);
        public virtual CreateVectorStoreOperation CreateVectorStore(bool waitUntilCompleted, VectorStoreCreationOptions vectorStore = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CreateVectorStoreOperation CreateVectorStore(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual ClientResult CreateVectorStore(BinaryContent content, RequestOptions options = null);
        public virtual Task<CreateVectorStoreOperation> CreateVectorStoreAsync(bool waitUntilCompleted, VectorStoreCreationOptions vectorStore = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<CreateVectorStoreOperation> CreateVectorStoreAsync(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual Task<ClientResult> CreateVectorStoreAsync(BinaryContent content, RequestOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult DeleteVectorStore(string vectorStoreId, RequestOptions options);
        public virtual ClientResult<VectorStoreDeletionResult> DeleteVectorStore(string vectorStoreId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> DeleteVectorStoreAsync(string vectorStoreId, RequestOptions options);
        public virtual Task<ClientResult<VectorStoreDeletionResult>> DeleteVectorStoreAsync(string vectorStoreId, CancellationToken cancellationToken = default);
        public virtual ClientResult<VectorStoreBatchFileJob> GetBatchFileJob(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<VectorStoreBatchFileJob>> GetBatchFileJobAsync(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult GetFileAssociation(string vectorStoreId, string fileId, RequestOptions options);
        public virtual ClientResult<VectorStoreFileAssociation> GetFileAssociation(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> GetFileAssociationAsync(string vectorStoreId, string fileId, RequestOptions options);
        public virtual Task<ClientResult<VectorStoreFileAssociation>> GetFileAssociationAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(string vectorStoreId, VectorStoreFileAssociationCollectionOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CollectionResult GetFileAssociations(string vectorStoreId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(string vectorStoreId, string batchJobId, VectorStoreFileAssociationCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(string vectorStoreId, string batchJobId, ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CollectionResult GetFileAssociations(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(string vectorStoreId, VectorStoreFileAssociationCollectionOptions options = null, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AsyncCollectionResult GetFileAssociationsAsync(string vectorStoreId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(string vectorStoreId, string batchJobId, VectorStoreFileAssociationCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(string vectorStoreId, string batchJobId, ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AsyncCollectionResult GetFileAssociationsAsync(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual ClientResult<VectorStore> GetVectorStore(string vectorStoreId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStore> GetVectorStores(VectorStoreCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStore> GetVectorStores(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CollectionResult GetVectorStores(int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<VectorStore> GetVectorStoresAsync(VectorStoreCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<VectorStore> GetVectorStoresAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AsyncCollectionResult GetVectorStoresAsync(int? limit, string order, string after, string before, RequestOptions options);
        public virtual ClientResult<VectorStore> ModifyVectorStore(string vectorStoreId, VectorStoreModificationOptions vectorStore, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult ModifyVectorStore(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<VectorStore>> ModifyVectorStoreAsync(string vectorStoreId, VectorStoreModificationOptions vectorStore, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> ModifyVectorStoreAsync(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ClientResult RemoveFileFromStore(string vectorStoreId, string fileId, RequestOptions options);
        public virtual ClientResult<FileFromStoreRemovalResult> RemoveFileFromStore(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Task<ClientResult> RemoveFileFromStoreAsync(string vectorStoreId, string fileId, RequestOptions options);
        public virtual Task<ClientResult<FileFromStoreRemovalResult>> RemoveFileFromStoreAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public VectorStoreCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    [Experimental("OPENAI001")]
    public readonly partial struct VectorStoreCollectionOrder : IEquatable<VectorStoreCollectionOrder> {
        public VectorStoreCollectionOrder(string value);
        public static VectorStoreCollectionOrder Ascending { get; }
        public static VectorStoreCollectionOrder Descending { get; }
        public readonly bool Equals(VectorStoreCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(VectorStoreCollectionOrder left, VectorStoreCollectionOrder right);
        public static implicit operator VectorStoreCollectionOrder(string value);
        public static bool operator !=(VectorStoreCollectionOrder left, VectorStoreCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class VectorStoreCreationOptions : IJsonModel<VectorStoreCreationOptions>, IPersistableModel<VectorStoreCreationOptions> {
        public FileChunkingStrategy ChunkingStrategy { get; set; }
        public VectorStoreExpirationPolicy ExpirationPolicy { get; set; }
        public IList<string> FileIds { get; }
        public IDictionary<string, string> Metadata { get; }
        public string Name { get; set; }
        public static explicit operator VectorStoreCreationOptions(ClientResult result);
        public static implicit operator BinaryContent(VectorStoreCreationOptions vectorStoreCreationOptions);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreDeletionResult : IJsonModel<VectorStoreDeletionResult>, IPersistableModel<VectorStoreDeletionResult> {
        public bool Deleted { get; }
        public string VectorStoreId { get; }
        public static explicit operator VectorStoreDeletionResult(ClientResult result);
        public static implicit operator BinaryContent(VectorStoreDeletionResult vectorStoreDeletionResult);
    }
    [Experimental("OPENAI001")]
    public enum VectorStoreExpirationAnchor {
        Unknown = 0,
        LastActiveAt = 1
    }
    [Experimental("OPENAI001")]
    public class VectorStoreExpirationPolicy : IJsonModel<VectorStoreExpirationPolicy>, IPersistableModel<VectorStoreExpirationPolicy> {
        public VectorStoreExpirationPolicy();
        [SetsRequiredMembers]
        public VectorStoreExpirationPolicy(VectorStoreExpirationAnchor anchor, int days);
        public required VectorStoreExpirationAnchor Anchor { get; set; }
        public required int Days { get; set; }
        public static explicit operator VectorStoreExpirationPolicy(ClientResult result);
        public static implicit operator BinaryContent(VectorStoreExpirationPolicy vectorStoreExpirationPolicy);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreFileAssociation : IJsonModel<VectorStoreFileAssociation>, IPersistableModel<VectorStoreFileAssociation> {
        public FileChunkingStrategy ChunkingStrategy { get; }
        public DateTimeOffset CreatedAt { get; }
        public string FileId { get; }
        public VectorStoreFileAssociationError LastError { get; }
        public int Size { get; }
        public VectorStoreFileAssociationStatus Status { get; }
        public string VectorStoreId { get; }
        public static explicit operator VectorStoreFileAssociation(ClientResult result);
        public static implicit operator BinaryContent(VectorStoreFileAssociation vectorStoreFileAssociation);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreFileAssociationCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public VectorStoreFileStatusFilter? Filter { get; set; }
        public VectorStoreFileAssociationCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    [Experimental("OPENAI001")]
    public readonly partial struct VectorStoreFileAssociationCollectionOrder : IEquatable<VectorStoreFileAssociationCollectionOrder> {
        public VectorStoreFileAssociationCollectionOrder(string value);
        public static VectorStoreFileAssociationCollectionOrder Ascending { get; }
        public static VectorStoreFileAssociationCollectionOrder Descending { get; }
        public readonly bool Equals(VectorStoreFileAssociationCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(VectorStoreFileAssociationCollectionOrder left, VectorStoreFileAssociationCollectionOrder right);
        public static implicit operator VectorStoreFileAssociationCollectionOrder(string value);
        public static bool operator !=(VectorStoreFileAssociationCollectionOrder left, VectorStoreFileAssociationCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class VectorStoreFileAssociationError : IJsonModel<VectorStoreFileAssociationError>, IPersistableModel<VectorStoreFileAssociationError> {
        public VectorStoreFileAssociationErrorCode Code { get; }
        public string Message { get; }
        public static explicit operator VectorStoreFileAssociationError(ClientResult result);
        public static implicit operator BinaryContent(VectorStoreFileAssociationError vectorStoreFileAssociationError);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct VectorStoreFileAssociationErrorCode : IEquatable<VectorStoreFileAssociationErrorCode> {
        public VectorStoreFileAssociationErrorCode(string value);
        public static VectorStoreFileAssociationErrorCode InvalidFile { get; }
        public static VectorStoreFileAssociationErrorCode ServerError { get; }
        public static VectorStoreFileAssociationErrorCode UnsupportedFile { get; }
        public readonly bool Equals(VectorStoreFileAssociationErrorCode other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(VectorStoreFileAssociationErrorCode left, VectorStoreFileAssociationErrorCode right);
        public static implicit operator VectorStoreFileAssociationErrorCode(string value);
        public static bool operator !=(VectorStoreFileAssociationErrorCode left, VectorStoreFileAssociationErrorCode right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public enum VectorStoreFileAssociationStatus {
        Unknown = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,
        Failed = 4
    }
    [Experimental("OPENAI001")]
    public class VectorStoreFileCounts : IJsonModel<VectorStoreFileCounts>, IPersistableModel<VectorStoreFileCounts> {
        public int Cancelled { get; }
        public int Completed { get; }
        public int Failed { get; }
        public int InProgress { get; }
        public int Total { get; }
        public static explicit operator VectorStoreFileCounts(ClientResult result);
        public static implicit operator BinaryContent(VectorStoreFileCounts vectorStoreFileCounts);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct VectorStoreFileStatusFilter : IEquatable<VectorStoreFileStatusFilter> {
        public VectorStoreFileStatusFilter(string value);
        public static VectorStoreFileStatusFilter Cancelled { get; }
        public static VectorStoreFileStatusFilter Completed { get; }
        public static VectorStoreFileStatusFilter Failed { get; }
        public static VectorStoreFileStatusFilter InProgress { get; }
        public readonly bool Equals(VectorStoreFileStatusFilter other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(VectorStoreFileStatusFilter left, VectorStoreFileStatusFilter right);
        public static implicit operator VectorStoreFileStatusFilter(string value);
        public static bool operator !=(VectorStoreFileStatusFilter left, VectorStoreFileStatusFilter right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class VectorStoreModificationOptions : IJsonModel<VectorStoreModificationOptions>, IPersistableModel<VectorStoreModificationOptions> {
        public VectorStoreExpirationPolicy ExpirationPolicy { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public string Name { get; set; }
        public static explicit operator VectorStoreModificationOptions(ClientResult result);
        public static implicit operator BinaryContent(VectorStoreModificationOptions vectorStoreModificationOptions);
    }
    [Experimental("OPENAI001")]
    public enum VectorStoreStatus {
        Unknown = 0,
        InProgress = 1,
        Completed = 2,
        Expired = 3
    }
}