namespace OpenAI {
    public class OpenAIClient {
        protected OpenAIClient();
        public OpenAIClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public OpenAIClient(ApiKeyCredential credential);
        [Experimental("OPENAI001")]
        public OpenAIClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        [Experimental("OPENAI001")]
        public OpenAIClient(AuthenticationPolicy authenticationPolicy);
        protected internal OpenAIClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public OpenAIClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        [Experimental("OPENAI001")]
        public virtual AssistantClient GetAssistantClient();
        public virtual AudioClient GetAudioClient(string model);
        [Experimental("OPENAI001")]
        public virtual BatchClient GetBatchClient();
        public virtual ChatClient GetChatClient(string model);
        [Experimental("OPENAI001")]
        public virtual ContainerClient GetContainerClient();
        [Experimental("OPENAI001")]
        public virtual ConversationClient GetConversationClient();
        public virtual EmbeddingClient GetEmbeddingClient(string model);
        [Experimental("OPENAI001")]
        public virtual EvaluationClient GetEvaluationClient();
        [Experimental("OPENAI001")]
        public virtual FineTuningClient GetFineTuningClient();
        [Experimental("OPENAI001")]
        public virtual GraderClient GetGraderClient();
        public virtual ImageClient GetImageClient(string model);
        public virtual ModerationClient GetModerationClient(string model);
        public virtual OpenAIFileClient GetOpenAIFileClient();
        public virtual OpenAIModelClient GetOpenAIModelClient();
        [Experimental("OPENAI002")]
        public virtual RealtimeClient GetRealtimeClient();
        [Experimental("OPENAI001")]
        public virtual ResponsesClient GetResponsesClient();
        [Experimental("OPENAI001")]
        public virtual VectorStoreClient GetVectorStoreClient();
        [Experimental("OPENAI001")]
        public virtual VideoClient GetVideoClient();
    }
    public class OpenAIClientOptions : ClientPipelineOptions {
        public Uri Endpoint { get; set; }
        public string OrganizationId { get; set; }
        public string ProjectId { get; set; }
        public string UserAgentApplicationId { get; set; }
    }
    [Experimental("OPENAI001")]
    public class OpenAIContext : ModelReaderWriterContext {
        public static OpenAIContext Default { get; }
        protected override bool TryGetTypeBuilderCore(Type type, out ModelReaderWriterTypeBuilder builder);
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
        protected virtual Assistant JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator Assistant(ClientResult result);
        protected virtual Assistant PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class AssistantClient {
        protected AssistantClient();
        public AssistantClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public AssistantClient(ApiKeyCredential credential);
        public AssistantClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        public AssistantClient(AuthenticationPolicy authenticationPolicy);
        protected internal AssistantClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public AssistantClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult CancelRun(string threadId, string runId, RequestOptions options);
        public virtual ClientResult<ThreadRun> CancelRun(string threadId, string runId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CancelRunAsync(string threadId, string runId, RequestOptions options);
        public virtual Task<ClientResult<ThreadRun>> CancelRunAsync(string threadId, string runId, CancellationToken cancellationToken = default);
        public virtual ClientResult CreateAssistant(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<Assistant> CreateAssistant(string model, AssistantCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CreateAssistantAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<Assistant>> CreateAssistantAsync(string model, AssistantCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<ThreadMessage> CreateMessage(string threadId, MessageRole role, IEnumerable<MessageContent> content, MessageCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult CreateMessage(string threadId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ThreadMessage>> CreateMessageAsync(string threadId, MessageRole role, IEnumerable<MessageContent> content, MessageCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CreateMessageAsync(string threadId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult CreateRun(string threadId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ThreadRun> CreateRun(string threadId, string assistantId, RunCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CreateRunAsync(string threadId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ThreadRun>> CreateRunAsync(string threadId, string assistantId, RunCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingUpdate> CreateRunStreaming(string threadId, string assistantId, RunCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingUpdate> CreateRunStreamingAsync(string threadId, string assistantId, RunCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<AssistantThread> CreateThread(ThreadCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult CreateThread(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult CreateThreadAndRun(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ThreadRun> CreateThreadAndRun(string assistantId, ThreadCreationOptions threadOptions = null, RunCreationOptions runOptions = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CreateThreadAndRunAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ThreadRun>> CreateThreadAndRunAsync(string assistantId, ThreadCreationOptions threadOptions = null, RunCreationOptions runOptions = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingUpdate> CreateThreadAndRunStreaming(string assistantId, ThreadCreationOptions threadOptions = null, RunCreationOptions runOptions = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingUpdate> CreateThreadAndRunStreamingAsync(string assistantId, ThreadCreationOptions threadOptions = null, RunCreationOptions runOptions = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<AssistantThread>> CreateThreadAsync(ThreadCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CreateThreadAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult DeleteAssistant(string assistantId, RequestOptions options);
        public virtual ClientResult<AssistantDeletionResult> DeleteAssistant(string assistantId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteAssistantAsync(string assistantId, RequestOptions options);
        public virtual Task<ClientResult<AssistantDeletionResult>> DeleteAssistantAsync(string assistantId, CancellationToken cancellationToken = default);
        public virtual ClientResult DeleteMessage(string threadId, string messageId, RequestOptions options);
        public virtual ClientResult<MessageDeletionResult> DeleteMessage(string threadId, string messageId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteMessageAsync(string threadId, string messageId, RequestOptions options);
        public virtual Task<ClientResult<MessageDeletionResult>> DeleteMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default);
        public virtual ClientResult DeleteThread(string threadId, RequestOptions options);
        public virtual ClientResult<ThreadDeletionResult> DeleteThread(string threadId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteThreadAsync(string threadId, RequestOptions options);
        public virtual Task<ClientResult<ThreadDeletionResult>> DeleteThreadAsync(string threadId, CancellationToken cancellationToken = default);
        public virtual ClientResult GetAssistant(string assistantId, RequestOptions options);
        public virtual ClientResult<Assistant> GetAssistant(string assistantId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetAssistantAsync(string assistantId, RequestOptions options);
        public virtual Task<ClientResult<Assistant>> GetAssistantAsync(string assistantId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<Assistant> GetAssistants(AssistantCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetAssistants(int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<Assistant> GetAssistantsAsync(AssistantCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetAssistantsAsync(int? limit, string order, string after, string before, RequestOptions options);
        public virtual ClientResult GetMessage(string threadId, string messageId, RequestOptions options);
        public virtual ClientResult<ThreadMessage> GetMessage(string threadId, string messageId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetMessageAsync(string threadId, string messageId, RequestOptions options);
        public virtual Task<ClientResult<ThreadMessage>> GetMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ThreadMessage> GetMessages(string threadId, MessageCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetMessages(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<ThreadMessage> GetMessagesAsync(string threadId, MessageCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetMessagesAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual ClientResult GetRun(string threadId, string runId, RequestOptions options);
        public virtual ClientResult<ThreadRun> GetRun(string threadId, string runId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetRunAsync(string threadId, string runId, RequestOptions options);
        public virtual Task<ClientResult<ThreadRun>> GetRunAsync(string threadId, string runId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ThreadRun> GetRuns(string threadId, RunCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetRuns(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<ThreadRun> GetRunsAsync(string threadId, RunCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetRunsAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual ClientResult GetRunStep(string threadId, string runId, string stepId, RequestOptions options);
        public virtual ClientResult<RunStep> GetRunStep(string threadId, string runId, string stepId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetRunStepAsync(string threadId, string runId, string stepId, RequestOptions options);
        public virtual Task<ClientResult<RunStep>> GetRunStepAsync(string threadId, string runId, string stepId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<RunStep> GetRunSteps(string threadId, string runId, RunStepCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetRunSteps(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<RunStep> GetRunStepsAsync(string threadId, string runId, RunStepCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetRunStepsAsync(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual ClientResult GetThread(string threadId, RequestOptions options);
        public virtual ClientResult<AssistantThread> GetThread(string threadId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetThreadAsync(string threadId, RequestOptions options);
        public virtual Task<ClientResult<AssistantThread>> GetThreadAsync(string threadId, CancellationToken cancellationToken = default);
        public virtual ClientResult<Assistant> ModifyAssistant(string assistantId, AssistantModificationOptions options, CancellationToken cancellationToken = default);
        public virtual ClientResult ModifyAssistant(string assistantId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<Assistant>> ModifyAssistantAsync(string assistantId, AssistantModificationOptions options, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> ModifyAssistantAsync(string assistantId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ThreadMessage> ModifyMessage(string threadId, string messageId, MessageModificationOptions options, CancellationToken cancellationToken = default);
        public virtual ClientResult ModifyMessage(string threadId, string messageId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ThreadMessage>> ModifyMessageAsync(string threadId, string messageId, MessageModificationOptions options, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> ModifyMessageAsync(string threadId, string messageId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult ModifyRun(string threadId, string runId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> ModifyRunAsync(string threadId, string runId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<AssistantThread> ModifyThread(string threadId, ThreadModificationOptions options, CancellationToken cancellationToken = default);
        public virtual ClientResult ModifyThread(string threadId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<AssistantThread>> ModifyThreadAsync(string threadId, ThreadModificationOptions options, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> ModifyThreadAsync(string threadId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult SubmitToolOutputsToRun(string threadId, string runId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ThreadRun> SubmitToolOutputsToRun(string threadId, string runId, IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> SubmitToolOutputsToRunAsync(string threadId, string runId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ThreadRun>> SubmitToolOutputsToRunAsync(string threadId, string runId, IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingUpdate> SubmitToolOutputsToRunStreaming(string threadId, string runId, IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingUpdate> SubmitToolOutputsToRunStreamingAsync(string threadId, string runId, IEnumerable<ToolOutput> toolOutputs, CancellationToken cancellationToken = default);
    }
    [Experimental("OPENAI001")]
    public class AssistantCollectionOptions : IJsonModel<AssistantCollectionOptions>, IPersistableModel<AssistantCollectionOptions> {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public AssistantCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        protected virtual AssistantCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual AssistantCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator AssistantCollectionOrder?(string value);
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
        protected virtual AssistantCreationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual AssistantCreationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class AssistantDeletionResult : IJsonModel<AssistantDeletionResult>, IPersistableModel<AssistantDeletionResult> {
        public string AssistantId { get; }
        public bool Deleted { get; }
        protected virtual AssistantDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator AssistantDeletionResult(ClientResult result);
        protected virtual AssistantDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual AssistantModificationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual AssistantModificationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class AssistantResponseFormat : IEquatable<AssistantResponseFormat>, IEquatable<string>, IJsonModel<AssistantResponseFormat>, IPersistableModel<AssistantResponseFormat> {
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
        protected virtual AssistantResponseFormat JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator ==(AssistantResponseFormat first, AssistantResponseFormat second);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static implicit operator AssistantResponseFormat(string plainTextFormat);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator !=(AssistantResponseFormat first, AssistantResponseFormat second);
        protected virtual AssistantResponseFormat PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual AssistantThread JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator AssistantThread(ClientResult result);
        protected virtual AssistantThread PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterToolDefinition : ToolDefinition, IJsonModel<CodeInterpreterToolDefinition>, IPersistableModel<CodeInterpreterToolDefinition> {
        public CodeInterpreterToolDefinition();
        protected override ToolDefinition JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ToolDefinition PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterToolResources : IJsonModel<CodeInterpreterToolResources>, IPersistableModel<CodeInterpreterToolResources> {
        public IList<string> FileIds { get; }
        protected virtual CodeInterpreterToolResources JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CodeInterpreterToolResources PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator FileSearchRanker?(string value);
        public static bool operator !=(FileSearchRanker left, FileSearchRanker right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class FileSearchRankingOptions : IJsonModel<FileSearchRankingOptions>, IPersistableModel<FileSearchRankingOptions> {
        public FileSearchRankingOptions(float scoreThreshold);
        public FileSearchRanker? Ranker { get; set; }
        public float ScoreThreshold { get; set; }
        protected virtual FileSearchRankingOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileSearchRankingOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FileSearchToolDefinition : ToolDefinition, IJsonModel<FileSearchToolDefinition>, IPersistableModel<FileSearchToolDefinition> {
        public FileSearchToolDefinition();
        public int? MaxResults { get; set; }
        public FileSearchRankingOptions RankingOptions { get; set; }
        protected override ToolDefinition JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ToolDefinition PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FileSearchToolResources : IJsonModel<FileSearchToolResources>, IPersistableModel<FileSearchToolResources> {
        public IList<VectorStoreCreationHelper> NewVectorStores { get; }
        public IList<string> VectorStoreIds { get; }
        protected virtual FileSearchToolResources JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileSearchToolResources PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FunctionToolDefinition : ToolDefinition, IJsonModel<FunctionToolDefinition>, IPersistableModel<FunctionToolDefinition> {
        public FunctionToolDefinition();
        public FunctionToolDefinition(string name);
        public string Description { get; set; }
        public string FunctionName { get; set; }
        public BinaryData Parameters { get; set; }
        public bool? StrictParameterSchemaEnabled { get; set; }
        protected override ToolDefinition JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ToolDefinition PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class MessageCollectionOptions : IJsonModel<MessageCollectionOptions>, IPersistableModel<MessageCollectionOptions> {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public MessageCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        protected virtual MessageCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual MessageCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator MessageCollectionOrder?(string value);
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
        protected virtual MessageContent JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static implicit operator MessageContent(string value);
        protected virtual MessageContent PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual MessageCreationAttachment JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual MessageCreationAttachment PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class MessageCreationOptions : IJsonModel<MessageCreationOptions>, IPersistableModel<MessageCreationOptions> {
        public IList<MessageCreationAttachment> Attachments { get; set; }
        public IDictionary<string, string> Metadata { get; }
        protected virtual MessageCreationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual MessageCreationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class MessageDeletionResult : IJsonModel<MessageDeletionResult>, IPersistableModel<MessageDeletionResult> {
        public bool Deleted { get; }
        public string MessageId { get; }
        protected virtual MessageDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator MessageDeletionResult(ClientResult result);
        protected virtual MessageDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class MessageFailureDetails : IJsonModel<MessageFailureDetails>, IPersistableModel<MessageFailureDetails> {
        public MessageFailureReason Reason { get; }
        protected virtual MessageFailureDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual MessageFailureDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator MessageFailureReason?(string value);
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
        protected virtual MessageModificationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual MessageModificationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator MessageStatus?(string value);
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
    public class RunCollectionOptions : IJsonModel<RunCollectionOptions>, IPersistableModel<RunCollectionOptions> {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public RunCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        protected virtual RunCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator RunCollectionOrder?(string value);
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
        protected virtual RunCreationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunCreationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class RunError : IJsonModel<RunError>, IPersistableModel<RunError> {
        public RunErrorCode Code { get; }
        public string Message { get; }
        protected virtual RunError JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunError PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator RunErrorCode?(string value);
        public static bool operator !=(RunErrorCode left, RunErrorCode right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunIncompleteDetails : IJsonModel<RunIncompleteDetails>, IPersistableModel<RunIncompleteDetails> {
        public RunIncompleteReason? Reason { get; }
        protected virtual RunIncompleteDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunIncompleteDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator RunIncompleteReason?(string value);
        public static bool operator !=(RunIncompleteReason left, RunIncompleteReason right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunModificationOptions : IJsonModel<RunModificationOptions>, IPersistableModel<RunModificationOptions> {
        public IDictionary<string, string> Metadata { get; }
        protected virtual RunModificationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunModificationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator RunStatus?(string value);
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
        protected virtual RunStep JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator RunStep(ClientResult result);
        protected virtual RunStep PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public abstract class RunStepCodeInterpreterOutput : IJsonModel<RunStepCodeInterpreterOutput>, IPersistableModel<RunStepCodeInterpreterOutput> {
        public string ImageFileId { get; }
        public string Logs { get; }
        protected virtual RunStepCodeInterpreterOutput JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepCodeInterpreterOutput PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class RunStepCollectionOptions : IJsonModel<RunStepCollectionOptions>, IPersistableModel<RunStepCollectionOptions> {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public RunStepCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        protected virtual RunStepCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator RunStepCollectionOrder?(string value);
        public static bool operator !=(RunStepCollectionOrder left, RunStepCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public abstract class RunStepDetails : IJsonModel<RunStepDetails>, IPersistableModel<RunStepDetails> {
        public string CreatedMessageId { get; }
        public IReadOnlyList<RunStepToolCall> ToolCalls { get; }
        protected virtual RunStepDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual RunStepError JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepError PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator RunStepErrorCode?(string value);
        public static bool operator !=(RunStepErrorCode left, RunStepErrorCode right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunStepFileSearchResult : IJsonModel<RunStepFileSearchResult>, IPersistableModel<RunStepFileSearchResult> {
        public IReadOnlyList<RunStepFileSearchResultContent> Content { get; }
        public string FileId { get; }
        public string FileName { get; }
        public float Score { get; }
        protected virtual RunStepFileSearchResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepFileSearchResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class RunStepFileSearchResultContent : IJsonModel<RunStepFileSearchResultContent>, IPersistableModel<RunStepFileSearchResultContent> {
        public RunStepFileSearchResultContentKind Kind { get; }
        public string Text { get; }
        protected virtual RunStepFileSearchResultContent JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepFileSearchResultContent PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator RunStepStatus?(string value);
        public static bool operator !=(RunStepStatus left, RunStepStatus right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunStepTokenUsage : IJsonModel<RunStepTokenUsage>, IPersistableModel<RunStepTokenUsage> {
        public int InputTokenCount { get; }
        public int OutputTokenCount { get; }
        public int TotalTokenCount { get; }
        protected virtual RunStepTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class RunStepToolCall : IJsonModel<RunStepToolCall>, IPersistableModel<RunStepToolCall> {
        public string CodeInterpreterInput { get; }
        public IReadOnlyList<RunStepCodeInterpreterOutput> CodeInterpreterOutputs { get; }
        public FileSearchRankingOptions FileSearchRankingOptions { get; }
        public IReadOnlyList<RunStepFileSearchResult> FileSearchResults { get; }
        public string FunctionArguments { get; }
        public string FunctionName { get; }
        public string FunctionOutput { get; }
        public string Id { get; }
        public RunStepToolCallKind Kind { get; }
        protected virtual RunStepToolCall JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepToolCall PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
    public class RunStepUpdateCodeInterpreterOutput : IJsonModel<RunStepUpdateCodeInterpreterOutput>, IPersistableModel<RunStepUpdateCodeInterpreterOutput> {
        public string ImageFileId { get; }
        public string Logs { get; }
        public int OutputIndex { get; }
        protected virtual RunStepUpdateCodeInterpreterOutput JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepUpdateCodeInterpreterOutput PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class RunTokenUsage : IJsonModel<RunTokenUsage>, IPersistableModel<RunTokenUsage> {
        public int InputTokenCount { get; }
        public int OutputTokenCount { get; }
        public int TotalTokenCount { get; }
        protected virtual RunTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class RunTruncationStrategy : IJsonModel<RunTruncationStrategy>, IPersistableModel<RunTruncationStrategy> {
        public static RunTruncationStrategy Auto { get; }
        public int? LastMessages { get; }
        public static RunTruncationStrategy CreateLastMessagesStrategy(int lastMessageCount);
        protected virtual RunTruncationStrategy JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunTruncationStrategy PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual ThreadCreationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ThreadCreationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ThreadDeletionResult : IJsonModel<ThreadDeletionResult>, IPersistableModel<ThreadDeletionResult> {
        public bool Deleted { get; }
        public string ThreadId { get; }
        protected virtual ThreadDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ThreadDeletionResult(ClientResult result);
        protected virtual ThreadDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual ThreadMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ThreadMessage(ClientResult result);
        protected virtual ThreadMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ThreadModificationOptions : IJsonModel<ThreadModificationOptions>, IPersistableModel<ThreadModificationOptions> {
        public IDictionary<string, string> Metadata { get; }
        public ToolResources ToolResources { get; set; }
        protected virtual ThreadModificationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ThreadModificationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual ThreadRun JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ThreadRun(ClientResult result);
        protected virtual ThreadRun PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual ToolConstraint JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ToolConstraint PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ToolDefinition : IJsonModel<ToolDefinition>, IPersistableModel<ToolDefinition> {
        public static CodeInterpreterToolDefinition CreateCodeInterpreter();
        public static FileSearchToolDefinition CreateFileSearch(int? maxResults = null);
        public static FunctionToolDefinition CreateFunction(string name, string description = null, BinaryData parameters = null, bool? strictParameterSchemaEnabled = null);
        protected virtual ToolDefinition JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ToolDefinition PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ToolOutput : IJsonModel<ToolOutput>, IPersistableModel<ToolOutput> {
        public ToolOutput();
        public ToolOutput(string toolCallId, string output);
        public string Output { get; set; }
        public string ToolCallId { get; set; }
        protected virtual ToolOutput JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ToolOutput PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ToolResources : IJsonModel<ToolResources>, IPersistableModel<ToolResources> {
        public CodeInterpreterToolResources CodeInterpreter { get; set; }
        public FileSearchToolResources FileSearch { get; set; }
        protected virtual ToolResources JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ToolResources PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreCreationHelper : IJsonModel<VectorStoreCreationHelper>, IPersistableModel<VectorStoreCreationHelper> {
        public VectorStoreCreationHelper();
        public VectorStoreCreationHelper(IEnumerable<OpenAIFile> files);
        public VectorStoreCreationHelper(IEnumerable<string> fileIds);
        public FileChunkingStrategy ChunkingStrategy { get; set; }
        public IList<string> FileIds { get; }
        public IDictionary<string, string> Metadata { get; }
        protected virtual VectorStoreCreationHelper JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreCreationHelper PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
}
namespace OpenAI.Audio {
    public class AudioClient {
        protected AudioClient();
        protected internal AudioClient(ClientPipeline pipeline, string model, OpenAIClientOptions options);
        public AudioClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public AudioClient(string model, ApiKeyCredential credential);
        [Experimental("OPENAI001")]
        public AudioClient(string model, AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        [Experimental("OPENAI001")]
        public AudioClient(string model, AuthenticationPolicy authenticationPolicy);
        public AudioClient(string model, string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        [Experimental("OPENAI001")]
        public string Model { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult GenerateSpeech(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<BinaryData> GenerateSpeech(string text, GeneratedSpeechVoice voice, SpeechGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GenerateSpeechAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<BinaryData>> GenerateSpeechAsync(string text, GeneratedSpeechVoice voice, SpeechGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult TranscribeAudio(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult<AudioTranscription> TranscribeAudio(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<AudioTranscription> TranscribeAudio(string audioFilePath, AudioTranscriptionOptions options = null);
        public virtual Task<ClientResult> TranscribeAudioAsync(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult<AudioTranscription>> TranscribeAudioAsync(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<AudioTranscription>> TranscribeAudioAsync(string audioFilePath, AudioTranscriptionOptions options = null);
        [Experimental("OPENAI001")]
        public virtual CollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreaming(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual CollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreaming(string audioFilePath, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual AsyncCollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreamingAsync(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual AsyncCollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreamingAsync(string audioFilePath, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
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
    [Experimental("OPENAI001")]
    public class AudioTokenLogProbabilityDetails : IJsonModel<AudioTokenLogProbabilityDetails>, IPersistableModel<AudioTokenLogProbabilityDetails> {
        public float LogProbability { get; }
        public string Token { get; }
        public ReadOnlyMemory<byte> Utf8Bytes { get; }
        protected virtual AudioTokenLogProbabilityDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual AudioTokenLogProbabilityDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class AudioTranscription : IJsonModel<AudioTranscription>, IPersistableModel<AudioTranscription> {
        public TimeSpan? Duration { get; }
        public string Language { get; }
        public IReadOnlyList<TranscribedSegment> Segments { get; }
        public string Text { get; }
        [Experimental("OPENAI001")]
        public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
        public IReadOnlyList<TranscribedWord> Words { get; }
        [Experimental("OPENAI001")]
        protected virtual AudioTranscription JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator AudioTranscription(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual AudioTranscription PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator AudioTranscriptionFormat?(string value);
        public static bool operator !=(AudioTranscriptionFormat left, AudioTranscriptionFormat right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    [Flags]
    public enum AudioTranscriptionIncludes {
        Default = 0,
        Logprobs = 1
    }
    public class AudioTranscriptionOptions : IJsonModel<AudioTranscriptionOptions>, IPersistableModel<AudioTranscriptionOptions> {
        [Experimental("OPENAI001")]
        public AudioTranscriptionIncludes Includes { get; set; }
        public string Language { get; set; }
        public string Prompt { get; set; }
        public AudioTranscriptionFormat? ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        public AudioTimestampGranularities TimestampGranularities { get; set; }
        [Experimental("OPENAI001")]
        protected virtual AudioTranscriptionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual AudioTranscriptionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class AudioTranslation : IJsonModel<AudioTranslation>, IPersistableModel<AudioTranslation> {
        public TimeSpan? Duration { get; }
        public string Language { get; }
        public IReadOnlyList<TranscribedSegment> Segments { get; }
        public string Text { get; }
        [Experimental("OPENAI001")]
        protected virtual AudioTranslation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator AudioTranslation(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual AudioTranslation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator AudioTranslationFormat?(string value);
        public static bool operator !=(AudioTranslationFormat left, AudioTranslationFormat right);
        public override readonly string ToString();
    }
    public class AudioTranslationOptions : IJsonModel<AudioTranslationOptions>, IPersistableModel<AudioTranslationOptions> {
        public string Prompt { get; set; }
        public AudioTranslationFormat? ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        [Experimental("OPENAI001")]
        protected virtual AudioTranslationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual AudioTranslationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator GeneratedSpeechFormat?(string value);
        public static bool operator !=(GeneratedSpeechFormat left, GeneratedSpeechFormat right);
        public override readonly string ToString();
    }
    public readonly partial struct GeneratedSpeechVoice : IEquatable<GeneratedSpeechVoice> {
        public GeneratedSpeechVoice(string value);
        public static GeneratedSpeechVoice Alloy { get; }
        [Experimental("OPENAI001")]
        public static GeneratedSpeechVoice Ash { get; }
        [Experimental("OPENAI001")]
        public static GeneratedSpeechVoice Ballad { get; }
        [Experimental("OPENAI001")]
        public static GeneratedSpeechVoice Coral { get; }
        public static GeneratedSpeechVoice Echo { get; }
        public static GeneratedSpeechVoice Fable { get; }
        public static GeneratedSpeechVoice Nova { get; }
        public static GeneratedSpeechVoice Onyx { get; }
        [Experimental("OPENAI001")]
        public static GeneratedSpeechVoice Sage { get; }
        public static GeneratedSpeechVoice Shimmer { get; }
        [Experimental("OPENAI001")]
        public static GeneratedSpeechVoice Verse { get; }
        public readonly bool Equals(GeneratedSpeechVoice other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedSpeechVoice left, GeneratedSpeechVoice right);
        public static implicit operator GeneratedSpeechVoice(string value);
        public static implicit operator GeneratedSpeechVoice?(string value);
        public static bool operator !=(GeneratedSpeechVoice left, GeneratedSpeechVoice right);
        public override readonly string ToString();
    }
    public static class OpenAIAudioModelFactory {
        [Experimental("OPENAI001")]
        public static AudioTranscription AudioTranscription(string language = null, TimeSpan? duration = null, string text = null, IEnumerable<TranscribedWord> words = null, IEnumerable<TranscribedSegment> segments = null, IEnumerable<AudioTokenLogProbabilityDetails> transcriptionTokenLogProbabilities = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static AudioTranscription AudioTranscription(string language, TimeSpan? duration, string text, IEnumerable<TranscribedWord> words, IEnumerable<TranscribedSegment> segments);
        public static AudioTranslation AudioTranslation(string language = null, TimeSpan? duration = null, string text = null, IEnumerable<TranscribedSegment> segments = null);
        public static TranscribedSegment TranscribedSegment(int id = 0, int seekOffset = 0, TimeSpan startTime = default, TimeSpan endTime = default, string text = null, ReadOnlyMemory<int> tokenIds = default, float temperature = 0, float averageLogProbability = 0, float compressionRatio = 0, float noSpeechProbability = 0);
        public static TranscribedWord TranscribedWord(string word = null, TimeSpan startTime = default, TimeSpan endTime = default);
    }
    public class SpeechGenerationOptions : IJsonModel<SpeechGenerationOptions>, IPersistableModel<SpeechGenerationOptions> {
        [Experimental("OPENAI001")]
        public string Instructions { get; set; }
        public GeneratedSpeechFormat? ResponseFormat { get; set; }
        public float? SpeedRatio { get; set; }
        [Experimental("OPENAI001")]
        protected virtual SpeechGenerationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual SpeechGenerationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingAudioTranscriptionTextDeltaUpdate : StreamingAudioTranscriptionUpdate, IJsonModel<StreamingAudioTranscriptionTextDeltaUpdate>, IPersistableModel<StreamingAudioTranscriptionTextDeltaUpdate> {
        public string Delta { get; }
        public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
        protected override StreamingAudioTranscriptionUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingAudioTranscriptionUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingAudioTranscriptionTextDoneUpdate : StreamingAudioTranscriptionUpdate, IJsonModel<StreamingAudioTranscriptionTextDoneUpdate>, IPersistableModel<StreamingAudioTranscriptionTextDoneUpdate> {
        public string Text { get; }
        public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
        protected override StreamingAudioTranscriptionUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingAudioTranscriptionUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingAudioTranscriptionUpdate : IJsonModel<StreamingAudioTranscriptionUpdate>, IPersistableModel<StreamingAudioTranscriptionUpdate> {
        protected virtual StreamingAudioTranscriptionUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual StreamingAudioTranscriptionUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct StreamingAudioTranscriptionUpdateKind : IEquatable<StreamingAudioTranscriptionUpdateKind> {
        public StreamingAudioTranscriptionUpdateKind(string value);
        public static StreamingAudioTranscriptionUpdateKind TranscriptTextDelta { get; }
        public static StreamingAudioTranscriptionUpdateKind TranscriptTextDone { get; }
        public readonly bool Equals(StreamingAudioTranscriptionUpdateKind other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(StreamingAudioTranscriptionUpdateKind left, StreamingAudioTranscriptionUpdateKind right);
        public static implicit operator StreamingAudioTranscriptionUpdateKind(string value);
        public static implicit operator StreamingAudioTranscriptionUpdateKind?(string value);
        public static bool operator !=(StreamingAudioTranscriptionUpdateKind left, StreamingAudioTranscriptionUpdateKind right);
        public override readonly string ToString();
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
    }
    public readonly partial struct TranscribedWord : IJsonModel<TranscribedWord>, IPersistableModel<TranscribedWord>, IJsonModel<object>, IPersistableModel<object> {
        public TimeSpan EndTime { get; }
        public TimeSpan StartTime { get; }
        public string Word { get; }
    }
}
namespace OpenAI.Batch {
    [Experimental("OPENAI001")]
    public class BatchClient {
        protected BatchClient();
        public BatchClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public BatchClient(ApiKeyCredential credential);
        public BatchClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        public BatchClient(AuthenticationPolicy authenticationPolicy);
        protected internal BatchClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public BatchClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual CreateBatchOperation CreateBatch(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual Task<CreateBatchOperation> CreateBatchAsync(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual ClientResult GetBatch(string batchId, RequestOptions options);
        public virtual Task<ClientResult> GetBatchAsync(string batchId, RequestOptions options);
        public virtual CollectionResult<BatchJob> GetBatches(BatchCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetBatches(string after, int? limit, RequestOptions options);
        public virtual AsyncCollectionResult<BatchJob> GetBatchesAsync(BatchCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetBatchesAsync(string after, int? limit, RequestOptions options);
    }
    [Experimental("OPENAI001")]
    public class BatchCollectionOptions : IJsonModel<BatchCollectionOptions>, IPersistableModel<BatchCollectionOptions> {
        public string AfterId { get; set; }
        public int? PageSizeLimit { get; set; }
        protected virtual BatchCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual BatchCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class BatchJob : IJsonModel<BatchJob>, IPersistableModel<BatchJob> {
        public DateTimeOffset? CancelledAt { get; }
        public DateTimeOffset? CancellingAt { get; }
        public DateTimeOffset? CompletedAt { get; }
        public string CompletionWindow { get; }
        public DateTimeOffset CreatedAt { get; }
        public string Endpoint { get; }
        public string ErrorFileId { get; }
        public DateTimeOffset? ExpiredAt { get; }
        public DateTimeOffset? ExpiresAt { get; }
        public DateTimeOffset? FailedAt { get; }
        public DateTimeOffset? FinalizingAt { get; }
        public string Id { get; }
        public DateTimeOffset? InProgressAt { get; }
        public string InputFileId { get; }
        public IDictionary<string, string> Metadata { get; }
        public string Object { get; }
        public string OutputFileId { get; }
        protected virtual BatchJob JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator BatchJob(ClientResult result);
        protected virtual BatchJob PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static CreateBatchOperation Rehydrate(BatchClient client, string batchId, CancellationToken cancellationToken = default);
        public static Task<CreateBatchOperation> RehydrateAsync(BatchClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static Task<CreateBatchOperation> RehydrateAsync(BatchClient client, string batchId, CancellationToken cancellationToken = default);
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
        [Experimental("OPENAI001")]
        public AssistantChatMessage(ChatOutputAudioReference outputAudioReference);
        public AssistantChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public AssistantChatMessage(IEnumerable<ChatToolCall> toolCalls);
        public AssistantChatMessage(string content);
        [Obsolete("This property is obsolete. Please use ToolCalls instead.")]
        public ChatFunctionCall FunctionCall { get; set; }
        [Experimental("OPENAI001")]
        public ChatOutputAudioReference OutputAudioReference { get; set; }
        public string ParticipantName { get; set; }
        public string Refusal { get; set; }
        public IList<ChatToolCall> ToolCalls { get; }
        [Experimental("OPENAI001")]
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ChatAudioOptions : IJsonModel<ChatAudioOptions>, IPersistableModel<ChatAudioOptions> {
        public ChatAudioOptions(ChatOutputAudioVoice outputAudioVoice, ChatOutputAudioFormat outputAudioFormat);
        public ChatOutputAudioFormat OutputAudioFormat { get; }
        public ChatOutputAudioVoice OutputAudioVoice { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ChatAudioOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatAudioOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatClient {
        protected ChatClient();
        protected internal ChatClient(ClientPipeline pipeline, string model, OpenAIClientOptions options);
        public ChatClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public ChatClient(string model, ApiKeyCredential credential);
        [Experimental("OPENAI001")]
        public ChatClient(string model, AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        [Experimental("OPENAI001")]
        public ChatClient(string model, AuthenticationPolicy authenticationPolicy);
        public ChatClient(string model, string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        [Experimental("OPENAI001")]
        public string Model { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult<ChatCompletion> CompleteChat(params ChatMessage[] messages);
        public virtual ClientResult CompleteChat(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ChatCompletion> CompleteChat(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<ChatCompletion>> CompleteChatAsync(params ChatMessage[] messages);
        public virtual Task<ClientResult> CompleteChatAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ChatCompletion>> CompleteChatAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingChatCompletionUpdate> CompleteChatStreaming(params ChatMessage[] messages);
        public virtual CollectionResult<StreamingChatCompletionUpdate> CompleteChatStreaming(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(params ChatMessage[] messages);
        public virtual AsyncCollectionResult<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual ClientResult DeleteChatCompletion(string completionId, RequestOptions options);
        [Experimental("OPENAI001")]
        public virtual ClientResult<ChatCompletionDeletionResult> DeleteChatCompletion(string completionId, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual Task<ClientResult> DeleteChatCompletionAsync(string completionId, RequestOptions options);
        [Experimental("OPENAI001")]
        public virtual Task<ClientResult<ChatCompletionDeletionResult>> DeleteChatCompletionAsync(string completionId, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual ClientResult GetChatCompletion(string completionId, RequestOptions options);
        [Experimental("OPENAI001")]
        public virtual ClientResult<ChatCompletion> GetChatCompletion(string completionId, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual Task<ClientResult> GetChatCompletionAsync(string completionId, RequestOptions options);
        [Experimental("OPENAI001")]
        public virtual Task<ClientResult<ChatCompletion>> GetChatCompletionAsync(string completionId, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual CollectionResult<ChatCompletionMessageListDatum> GetChatCompletionMessages(string completionId, ChatCompletionMessageCollectionOptions options = null, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual CollectionResult GetChatCompletionMessages(string completionId, string after, int? limit, string order, RequestOptions options);
        [Experimental("OPENAI001")]
        public virtual AsyncCollectionResult<ChatCompletionMessageListDatum> GetChatCompletionMessagesAsync(string completionId, ChatCompletionMessageCollectionOptions options = null, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual AsyncCollectionResult GetChatCompletionMessagesAsync(string completionId, string after, int? limit, string order, RequestOptions options);
        [Experimental("OPENAI001")]
        public virtual CollectionResult<ChatCompletion> GetChatCompletions(ChatCompletionCollectionOptions options = null, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual CollectionResult GetChatCompletions(string after, int? limit, string order, IDictionary<string, string> metadata, string model, RequestOptions options);
        [Experimental("OPENAI001")]
        public virtual AsyncCollectionResult<ChatCompletion> GetChatCompletionsAsync(ChatCompletionCollectionOptions options = null, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual AsyncCollectionResult GetChatCompletionsAsync(string after, int? limit, string order, IDictionary<string, string> metadata, string model, RequestOptions options);
        [Experimental("OPENAI001")]
        public virtual ClientResult UpdateChatCompletion(string completionId, BinaryContent content, RequestOptions options = null);
        [Experimental("OPENAI001")]
        public virtual ClientResult<ChatCompletion> UpdateChatCompletion(string completionId, IDictionary<string, string> metadata, CancellationToken cancellationToken = default);
        [Experimental("OPENAI001")]
        public virtual Task<ClientResult> UpdateChatCompletionAsync(string completionId, BinaryContent content, RequestOptions options = null);
        [Experimental("OPENAI001")]
        public virtual Task<ClientResult<ChatCompletion>> UpdateChatCompletionAsync(string completionId, IDictionary<string, string> metadata, CancellationToken cancellationToken = default);
    }
    public class ChatCompletion : IJsonModel<ChatCompletion>, IPersistableModel<ChatCompletion> {
        [Experimental("OPENAI001")]
        public IReadOnlyList<ChatMessageAnnotation> Annotations { get; }
        public ChatMessageContent Content { get; }
        public IReadOnlyList<ChatTokenLogProbabilityDetails> ContentTokenLogProbabilities { get; }
        public DateTimeOffset CreatedAt { get; }
        public ChatFinishReason FinishReason { get; }
        [Obsolete("This property is obsolete. Please use ToolCalls instead.")]
        public ChatFunctionCall FunctionCall { get; }
        public string Id { get; }
        public string Model { get; }
        [Experimental("OPENAI001")]
        public ChatOutputAudio OutputAudio { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string Refusal { get; }
        public IReadOnlyList<ChatTokenLogProbabilityDetails> RefusalTokenLogProbabilities { get; }
        public ChatMessageRole Role { get; }
        [Experimental("OPENAI001")]
        public ChatServiceTier? ServiceTier { get; }
        public string SystemFingerprint { get; }
        public IReadOnlyList<ChatToolCall> ToolCalls { get; }
        public ChatTokenUsage Usage { get; }
        [Experimental("OPENAI001")]
        protected virtual ChatCompletion JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator ChatCompletion(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual ChatCompletion PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ChatCompletionCollectionOptions : IJsonModel<ChatCompletionCollectionOptions>, IPersistableModel<ChatCompletionCollectionOptions> {
        public string AfterId { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public string Model { get; set; }
        public ChatCompletionCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ChatCompletionCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatCompletionCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ChatCompletionCollectionOrder : IEquatable<ChatCompletionCollectionOrder> {
        public ChatCompletionCollectionOrder(string value);
        public static ChatCompletionCollectionOrder Ascending { get; }
        public static ChatCompletionCollectionOrder Descending { get; }
        public readonly bool Equals(ChatCompletionCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ChatCompletionCollectionOrder left, ChatCompletionCollectionOrder right);
        public static implicit operator ChatCompletionCollectionOrder(string value);
        public static implicit operator ChatCompletionCollectionOrder?(string value);
        public static bool operator !=(ChatCompletionCollectionOrder left, ChatCompletionCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ChatCompletionDeletionResult : IJsonModel<ChatCompletionDeletionResult>, IPersistableModel<ChatCompletionDeletionResult> {
        public string ChatCompletionId { get; }
        public bool Deleted { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ChatCompletionDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ChatCompletionDeletionResult(ClientResult result);
        protected virtual ChatCompletionDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ChatCompletionMessageCollectionOptions : IJsonModel<ChatCompletionMessageCollectionOptions>, IPersistableModel<ChatCompletionMessageCollectionOptions> {
        public string AfterId { get; set; }
        public ChatCompletionMessageCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ChatCompletionMessageCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatCompletionMessageCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ChatCompletionMessageCollectionOrder : IEquatable<ChatCompletionMessageCollectionOrder> {
        public ChatCompletionMessageCollectionOrder(string value);
        public static ChatCompletionMessageCollectionOrder Ascending { get; }
        public static ChatCompletionMessageCollectionOrder Descending { get; }
        public readonly bool Equals(ChatCompletionMessageCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ChatCompletionMessageCollectionOrder left, ChatCompletionMessageCollectionOrder right);
        public static implicit operator ChatCompletionMessageCollectionOrder(string value);
        public static implicit operator ChatCompletionMessageCollectionOrder?(string value);
        public static bool operator !=(ChatCompletionMessageCollectionOrder left, ChatCompletionMessageCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ChatCompletionMessageListDatum : IJsonModel<ChatCompletionMessageListDatum>, IPersistableModel<ChatCompletionMessageListDatum> {
        public IReadOnlyList<ChatMessageAnnotation> Annotations { get; }
        public string Content { get; }
        public IList<ChatMessageContentPart> ContentParts { get; }
        public string Id { get; }
        public ChatOutputAudio OutputAudio { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string Refusal { get; }
        public IReadOnlyList<ChatToolCall> ToolCalls { get; }
        protected virtual ChatCompletionMessageListDatum JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatCompletionMessageListDatum PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatCompletionOptions : IJsonModel<ChatCompletionOptions>, IPersistableModel<ChatCompletionOptions> {
        public bool? AllowParallelToolCalls { get; set; }
        [Experimental("OPENAI001")]
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
        [Experimental("OPENAI001")]
        public ChatOutputPrediction OutputPrediction { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public float? PresencePenalty { get; set; }
        [Experimental("OPENAI001")]
        public ChatReasoningEffortLevel? ReasoningEffortLevel { get; set; }
        public ChatResponseFormat ResponseFormat { get; set; }
        [Experimental("OPENAI001")]
        public ChatResponseModalities ResponseModalities { get; set; }
        [Experimental("OPENAI001")]
        public string SafetyIdentifier { get; set; }
        [Experimental("OPENAI001")]
        public long? Seed { get; set; }
        [Experimental("OPENAI001")]
        public ChatServiceTier? ServiceTier { get; set; }
        public IList<string> StopSequences { get; }
        public bool? StoredOutputEnabled { get; set; }
        public float? Temperature { get; set; }
        public ChatToolChoice ToolChoice { get; set; }
        public IList<ChatTool> Tools { get; }
        public int? TopLogProbabilityCount { get; set; }
        public float? TopP { get; set; }
        [Experimental("OPENAI001")]
        public ChatWebSearchOptions WebSearchOptions { get; set; }
        [Experimental("OPENAI001")]
        protected virtual ChatCompletionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatCompletionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        [Experimental("OPENAI001")]
        protected virtual ChatFunction JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatFunction PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Obsolete("This class is obsolete. Please use ChatToolCall instead.")]
    public class ChatFunctionCall : IJsonModel<ChatFunctionCall>, IPersistableModel<ChatFunctionCall> {
        public ChatFunctionCall(string functionName, BinaryData functionArguments);
        public BinaryData FunctionArguments { get; }
        public string FunctionName { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        [Experimental("OPENAI001")]
        protected virtual ChatFunctionCall JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatFunctionCall PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Obsolete("This class is obsolete. Please use ChatToolChoice instead.")]
    public class ChatFunctionChoice : IJsonModel<ChatFunctionChoice>, IPersistableModel<ChatFunctionChoice> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static ChatFunctionChoice CreateAutoChoice();
        public static ChatFunctionChoice CreateNamedChoice(string functionName);
        public static ChatFunctionChoice CreateNoneChoice();
        [Experimental("OPENAI001")]
        protected virtual ChatFunctionChoice JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatFunctionChoice PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator ChatImageDetailLevel?(string value);
        public static bool operator !=(ChatImageDetailLevel left, ChatImageDetailLevel right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
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
        public static implicit operator ChatInputAudioFormat?(string value);
        public static bool operator !=(ChatInputAudioFormat left, ChatInputAudioFormat right);
        public override readonly string ToString();
    }
    public class ChatInputTokenUsageDetails : IJsonModel<ChatInputTokenUsageDetails>, IPersistableModel<ChatInputTokenUsageDetails> {
        public int AudioTokenCount { get; }
        public int CachedTokenCount { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        [Experimental("OPENAI001")]
        protected virtual ChatInputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatInputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatMessage : IJsonModel<ChatMessage>, IPersistableModel<ChatMessage> {
        public ChatMessageContent Content { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static AssistantChatMessage CreateAssistantMessage(ChatCompletion chatCompletion);
        public static AssistantChatMessage CreateAssistantMessage(ChatFunctionCall functionCall);
        public static AssistantChatMessage CreateAssistantMessage(params ChatMessageContentPart[] contentParts);
        [Experimental("OPENAI001")]
        public static AssistantChatMessage CreateAssistantMessage(ChatOutputAudioReference outputAudioReference);
        public static AssistantChatMessage CreateAssistantMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public static AssistantChatMessage CreateAssistantMessage(IEnumerable<ChatToolCall> toolCalls);
        public static AssistantChatMessage CreateAssistantMessage(string content);
        [Experimental("OPENAI001")]
        public static DeveloperChatMessage CreateDeveloperMessage(params ChatMessageContentPart[] contentParts);
        [Experimental("OPENAI001")]
        public static DeveloperChatMessage CreateDeveloperMessage(IEnumerable<ChatMessageContentPart> contentParts);
        [Experimental("OPENAI001")]
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
        [Experimental("OPENAI001")]
        protected virtual ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static implicit operator ChatMessage(string content);
        [Experimental("OPENAI001")]
        protected virtual ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ChatMessageAnnotation : IJsonModel<ChatMessageAnnotation>, IPersistableModel<ChatMessageAnnotation> {
        public int EndIndex { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public int StartIndex { get; }
        public string WebResourceTitle { get; }
        public Uri WebResourceUri { get; }
        protected virtual ChatMessageAnnotation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatMessageAnnotation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatMessageContent : ObjectModel.Collection<ChatMessageContentPart> {
        public ChatMessageContent();
        public ChatMessageContent(params ChatMessageContentPart[] contentParts);
        public ChatMessageContent(IEnumerable<ChatMessageContentPart> contentParts);
        public ChatMessageContent(string content);
    }
    public class ChatMessageContentPart : IJsonModel<ChatMessageContentPart>, IPersistableModel<ChatMessageContentPart> {
        [Experimental("OPENAI001")]
        public BinaryData FileBytes { get; }
        [Experimental("OPENAI001")]
        public string FileBytesMediaType { get; }
        [Experimental("OPENAI001")]
        public string FileId { get; }
        [Experimental("OPENAI001")]
        public string Filename { get; }
        public BinaryData ImageBytes { get; }
        public string ImageBytesMediaType { get; }
        public ChatImageDetailLevel? ImageDetailLevel { get; }
        public Uri ImageUri { get; }
        [Experimental("OPENAI001")]
        public BinaryData InputAudioBytes { get; }
        [Experimental("OPENAI001")]
        public ChatInputAudioFormat? InputAudioFormat { get; }
        public ChatMessageContentPartKind Kind { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string Refusal { get; }
        public string Text { get; }
        [Experimental("OPENAI001")]
        public static ChatMessageContentPart CreateFilePart(BinaryData fileBytes, string fileBytesMediaType, string filename);
        [Experimental("OPENAI001")]
        public static ChatMessageContentPart CreateFilePart(string fileId);
        public static ChatMessageContentPart CreateImagePart(BinaryData imageBytes, string imageBytesMediaType, ChatImageDetailLevel? imageDetailLevel = null);
        public static ChatMessageContentPart CreateImagePart(Uri imageUri, ChatImageDetailLevel? imageDetailLevel = null);
        [Experimental("OPENAI001")]
        public static ChatMessageContentPart CreateInputAudioPart(BinaryData inputAudioBytes, ChatInputAudioFormat inputAudioFormat);
        public static ChatMessageContentPart CreateRefusalPart(string refusal);
        public static ChatMessageContentPart CreateTextPart(string text);
        [Experimental("OPENAI001")]
        protected virtual ChatMessageContentPart JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static implicit operator ChatMessageContentPart(string text);
        [Experimental("OPENAI001")]
        protected virtual ChatMessageContentPart PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum ChatMessageContentPartKind {
        Text = 0,
        Refusal = 1,
        Image = 2,
        InputAudio = 3,
        File = 4
    }
    public enum ChatMessageRole {
        System = 0,
        User = 1,
        Assistant = 2,
        Tool = 3,
        Function = 4,
        Developer = 5
    }
    [Experimental("OPENAI001")]
    public class ChatOutputAudio : IJsonModel<ChatOutputAudio>, IPersistableModel<ChatOutputAudio> {
        public BinaryData AudioBytes { get; }
        public DateTimeOffset ExpiresAt { get; }
        public string Id { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string Transcript { get; }
        protected virtual ChatOutputAudio JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatOutputAudio PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ChatOutputAudioFormat : IEquatable<ChatOutputAudioFormat> {
        public ChatOutputAudioFormat(string value);
        public static ChatOutputAudioFormat Aac { get; }
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
        public static implicit operator ChatOutputAudioFormat?(string value);
        public static bool operator !=(ChatOutputAudioFormat left, ChatOutputAudioFormat right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ChatOutputAudioReference : IJsonModel<ChatOutputAudioReference>, IPersistableModel<ChatOutputAudioReference> {
        public ChatOutputAudioReference(string id);
        public string Id { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ChatOutputAudioReference JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatOutputAudioReference PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ChatOutputAudioVoice : IEquatable<ChatOutputAudioVoice> {
        public ChatOutputAudioVoice(string value);
        public static ChatOutputAudioVoice Alloy { get; }
        public static ChatOutputAudioVoice Ash { get; }
        public static ChatOutputAudioVoice Ballad { get; }
        public static ChatOutputAudioVoice Coral { get; }
        public static ChatOutputAudioVoice Echo { get; }
        public static ChatOutputAudioVoice Fable { get; }
        public static ChatOutputAudioVoice Nova { get; }
        public static ChatOutputAudioVoice Onyx { get; }
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
        public static implicit operator ChatOutputAudioVoice?(string value);
        public static bool operator !=(ChatOutputAudioVoice left, ChatOutputAudioVoice right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ChatOutputPrediction : IJsonModel<ChatOutputPrediction>, IPersistableModel<ChatOutputPrediction> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static ChatOutputPrediction CreateStaticContentPrediction(IEnumerable<ChatMessageContentPart> staticContentParts);
        public static ChatOutputPrediction CreateStaticContentPrediction(string staticContent);
        protected virtual ChatOutputPrediction JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatOutputPrediction PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatOutputTokenUsageDetails : IJsonModel<ChatOutputTokenUsageDetails>, IPersistableModel<ChatOutputTokenUsageDetails> {
        [Experimental("OPENAI001")]
        public int AcceptedPredictionTokenCount { get; }
        public int AudioTokenCount { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public int ReasoningTokenCount { get; }
        [Experimental("OPENAI001")]
        public int RejectedPredictionTokenCount { get; }
        [Experimental("OPENAI001")]
        protected virtual ChatOutputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatOutputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ChatReasoningEffortLevel : IEquatable<ChatReasoningEffortLevel> {
        public ChatReasoningEffortLevel(string value);
        public static ChatReasoningEffortLevel High { get; }
        public static ChatReasoningEffortLevel Low { get; }
        public static ChatReasoningEffortLevel Medium { get; }
        public static ChatReasoningEffortLevel Minimal { get; }
        public readonly bool Equals(ChatReasoningEffortLevel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ChatReasoningEffortLevel left, ChatReasoningEffortLevel right);
        public static implicit operator ChatReasoningEffortLevel(string value);
        public static implicit operator ChatReasoningEffortLevel?(string value);
        public static bool operator !=(ChatReasoningEffortLevel left, ChatReasoningEffortLevel right);
        public override readonly string ToString();
    }
    public class ChatResponseFormat : IJsonModel<ChatResponseFormat>, IPersistableModel<ChatResponseFormat> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static ChatResponseFormat CreateJsonObjectFormat();
        public static ChatResponseFormat CreateJsonSchemaFormat(string jsonSchemaFormatName, BinaryData jsonSchema, string jsonSchemaFormatDescription = null, bool? jsonSchemaIsStrict = null);
        public static ChatResponseFormat CreateTextFormat();
        [Experimental("OPENAI001")]
        protected virtual ChatResponseFormat JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatResponseFormat PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    [Flags]
    public enum ChatResponseModalities {
        Default = 0,
        Text = 1,
        Audio = 2
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ChatServiceTier : IEquatable<ChatServiceTier> {
        public ChatServiceTier(string value);
        public static ChatServiceTier Auto { get; }
        public static ChatServiceTier Default { get; }
        public static ChatServiceTier Flex { get; }
        public static ChatServiceTier Scale { get; }
        public readonly bool Equals(ChatServiceTier other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ChatServiceTier left, ChatServiceTier right);
        public static implicit operator ChatServiceTier(string value);
        public static implicit operator ChatServiceTier?(string value);
        public static bool operator !=(ChatServiceTier left, ChatServiceTier right);
        public override readonly string ToString();
    }
    public class ChatTokenLogProbabilityDetails : IJsonModel<ChatTokenLogProbabilityDetails>, IPersistableModel<ChatTokenLogProbabilityDetails> {
        public float LogProbability { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string Token { get; }
        public IReadOnlyList<ChatTokenTopLogProbabilityDetails> TopLogProbabilities { get; }
        public ReadOnlyMemory<byte>? Utf8Bytes { get; }
        [Experimental("OPENAI001")]
        protected virtual ChatTokenLogProbabilityDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatTokenLogProbabilityDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatTokenTopLogProbabilityDetails : IJsonModel<ChatTokenTopLogProbabilityDetails>, IPersistableModel<ChatTokenTopLogProbabilityDetails> {
        public float LogProbability { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string Token { get; }
        public ReadOnlyMemory<byte>? Utf8Bytes { get; }
        [Experimental("OPENAI001")]
        protected virtual ChatTokenTopLogProbabilityDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatTokenTopLogProbabilityDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatTokenUsage : IJsonModel<ChatTokenUsage>, IPersistableModel<ChatTokenUsage> {
        public int InputTokenCount { get; }
        public ChatInputTokenUsageDetails InputTokenDetails { get; }
        public int OutputTokenCount { get; }
        public ChatOutputTokenUsageDetails OutputTokenDetails { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public int TotalTokenCount { get; }
        [Experimental("OPENAI001")]
        protected virtual ChatTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatTool : IJsonModel<ChatTool>, IPersistableModel<ChatTool> {
        public string FunctionDescription { get; }
        public string FunctionName { get; }
        public BinaryData FunctionParameters { get; }
        public bool? FunctionSchemaIsStrict { get; }
        public ChatToolKind Kind { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static ChatTool CreateFunctionTool(string functionName, string functionDescription = null, BinaryData functionParameters = null, bool? functionSchemaIsStrict = null);
        [Experimental("OPENAI001")]
        protected virtual ChatTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatToolCall : IJsonModel<ChatToolCall>, IPersistableModel<ChatToolCall> {
        public BinaryData FunctionArguments { get; }
        public string FunctionName { get; }
        public string Id { get; set; }
        public ChatToolCallKind Kind { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static ChatToolCall CreateFunctionToolCall(string id, string functionName, BinaryData functionArguments);
        [Experimental("OPENAI001")]
        protected virtual ChatToolCall JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatToolCall PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum ChatToolCallKind {
        Function = 0
    }
    public class ChatToolChoice : IJsonModel<ChatToolChoice>, IPersistableModel<ChatToolChoice> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static ChatToolChoice CreateAutoChoice();
        public static ChatToolChoice CreateFunctionChoice(string functionName);
        public static ChatToolChoice CreateNoneChoice();
        public static ChatToolChoice CreateRequiredChoice();
        [Experimental("OPENAI001")]
        protected virtual ChatToolChoice JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ChatToolChoice PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum ChatToolKind {
        Function = 0
    }
    [Experimental("OPENAI001")]
    public class ChatWebSearchOptions : IJsonModel<ChatWebSearchOptions>, IPersistableModel<ChatWebSearchOptions> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ChatWebSearchOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatWebSearchOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class DeveloperChatMessage : ChatMessage, IJsonModel<DeveloperChatMessage>, IPersistableModel<DeveloperChatMessage> {
        public DeveloperChatMessage(params ChatMessageContentPart[] contentParts);
        public DeveloperChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public DeveloperChatMessage(string content);
        public string ParticipantName { get; set; }
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Obsolete("This class is obsolete. Please use ToolChatMessage instead.")]
    public class FunctionChatMessage : ChatMessage, IJsonModel<FunctionChatMessage>, IPersistableModel<FunctionChatMessage> {
        public FunctionChatMessage(string functionName, string content);
        public string FunctionName { get; }
        [Experimental("OPENAI001")]
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public static class OpenAIChatModelFactory {
        [Experimental("OPENAI001")]
        public static ChatCompletion ChatCompletion(string id = null, ChatFinishReason finishReason = ChatFinishReason.Stop, ChatMessageContent content = null, string refusal = null, IEnumerable<ChatToolCall> toolCalls = null, ChatMessageRole role = ChatMessageRole.System, ChatFunctionCall functionCall = null, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null, DateTimeOffset createdAt = default, string model = null, ChatServiceTier? serviceTier = null, string systemFingerprint = null, ChatTokenUsage usage = null, ChatOutputAudio outputAudio = null, IEnumerable<ChatMessageAnnotation> messageAnnotations = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ChatCompletion ChatCompletion(string id, ChatFinishReason finishReason, ChatMessageContent content, string refusal, IEnumerable<ChatToolCall> toolCalls, ChatMessageRole role, ChatFunctionCall functionCall, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities, DateTimeOffset createdAt, string model, string systemFingerprint, ChatTokenUsage usage);
        [Experimental("OPENAI001")]
        public static ChatCompletionMessageListDatum ChatCompletionMessageListDatum(string id, string content, string refusal, ChatMessageRole role, IList<ChatMessageContentPart> contentParts = null, IList<ChatToolCall> toolCalls = null, IList<ChatMessageAnnotation> annotations = null, string functionName = null, string functionArguments = null, ChatOutputAudio outputAudio = null);
        public static ChatInputTokenUsageDetails ChatInputTokenUsageDetails(int audioTokenCount = 0, int cachedTokenCount = 0);
        [Experimental("OPENAI001")]
        public static ChatMessageAnnotation ChatMessageAnnotation(int startIndex = 0, int endIndex = 0, Uri webResourceUri = null, string webResourceTitle = null);
        [Experimental("OPENAI001")]
        public static ChatOutputAudio ChatOutputAudio(BinaryData audioBytes, string id = null, string transcript = null, DateTimeOffset expiresAt = default);
        [Experimental("OPENAI001")]
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
        [Experimental("OPENAI001")]
        public static StreamingChatCompletionUpdate StreamingChatCompletionUpdate(string completionId = null, ChatMessageContent contentUpdate = null, StreamingChatFunctionCallUpdate functionCallUpdate = null, IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates = null, ChatMessageRole? role = null, string refusalUpdate = null, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null, ChatFinishReason? finishReason = null, DateTimeOffset createdAt = default, string model = null, ChatServiceTier? serviceTier = null, string systemFingerprint = null, ChatTokenUsage usage = null, StreamingChatOutputAudioUpdate outputAudioUpdate = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static StreamingChatCompletionUpdate StreamingChatCompletionUpdate(string completionId, ChatMessageContent contentUpdate, StreamingChatFunctionCallUpdate functionCallUpdate, IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates, ChatMessageRole? role, string refusalUpdate, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities, ChatFinishReason? finishReason, DateTimeOffset createdAt, string model, string systemFingerprint, ChatTokenUsage usage);
        [Obsolete("This class is obsolete. Please use StreamingChatToolCallUpdate instead.")]
        public static StreamingChatFunctionCallUpdate StreamingChatFunctionCallUpdate(string functionName = null, BinaryData functionArgumentsUpdate = null);
        [Experimental("OPENAI001")]
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
        [Experimental("OPENAI001")]
        public StreamingChatOutputAudioUpdate OutputAudioUpdate { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public IReadOnlyList<ChatTokenLogProbabilityDetails> RefusalTokenLogProbabilities { get; }
        public string RefusalUpdate { get; }
        public ChatMessageRole? Role { get; }
        [Experimental("OPENAI001")]
        public ChatServiceTier? ServiceTier { get; }
        public string SystemFingerprint { get; }
        public IReadOnlyList<StreamingChatToolCallUpdate> ToolCallUpdates { get; }
        public ChatTokenUsage Usage { get; }
        [Experimental("OPENAI001")]
        protected virtual StreamingChatCompletionUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual StreamingChatCompletionUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Obsolete("This class is obsolete. Please use StreamingChatToolCallUpdate instead.")]
    public class StreamingChatFunctionCallUpdate : IJsonModel<StreamingChatFunctionCallUpdate>, IPersistableModel<StreamingChatFunctionCallUpdate> {
        public BinaryData FunctionArgumentsUpdate { get; }
        public string FunctionName { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        [Experimental("OPENAI001")]
        protected virtual StreamingChatFunctionCallUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual StreamingChatFunctionCallUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingChatOutputAudioUpdate : IJsonModel<StreamingChatOutputAudioUpdate>, IPersistableModel<StreamingChatOutputAudioUpdate> {
        public BinaryData AudioBytesUpdate { get; }
        public DateTimeOffset? ExpiresAt { get; }
        public string Id { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string TranscriptUpdate { get; }
        protected virtual StreamingChatOutputAudioUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual StreamingChatOutputAudioUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingChatToolCallUpdate : IJsonModel<StreamingChatToolCallUpdate>, IPersistableModel<StreamingChatToolCallUpdate> {
        public BinaryData FunctionArgumentsUpdate { get; }
        public string FunctionName { get; }
        public int Index { get; }
        public ChatToolCallKind Kind { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string ToolCallId { get; }
        [Experimental("OPENAI001")]
        protected virtual StreamingChatToolCallUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual StreamingChatToolCallUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class SystemChatMessage : ChatMessage, IJsonModel<SystemChatMessage>, IPersistableModel<SystemChatMessage> {
        public SystemChatMessage(params ChatMessageContentPart[] contentParts);
        public SystemChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public SystemChatMessage(string content);
        public string ParticipantName { get; set; }
        [Experimental("OPENAI001")]
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ToolChatMessage : ChatMessage, IJsonModel<ToolChatMessage>, IPersistableModel<ToolChatMessage> {
        public ToolChatMessage(string toolCallId, params ChatMessageContentPart[] contentParts);
        public ToolChatMessage(string toolCallId, IEnumerable<ChatMessageContentPart> contentParts);
        public ToolChatMessage(string toolCallId, string content);
        public string ToolCallId { get; }
        [Experimental("OPENAI001")]
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class UserChatMessage : ChatMessage, IJsonModel<UserChatMessage>, IPersistableModel<UserChatMessage> {
        public UserChatMessage(params ChatMessageContentPart[] contentParts);
        public UserChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public UserChatMessage(string content);
        public string ParticipantName { get; set; }
        [Experimental("OPENAI001")]
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
}
namespace OpenAI.Containers {
    [Experimental("OPENAI001")]
    public class ContainerClient {
        protected ContainerClient();
        public ContainerClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public ContainerClient(ApiKeyCredential credential);
        public ContainerClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        public ContainerClient(AuthenticationPolicy authenticationPolicy);
        protected internal ContainerClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public ContainerClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult<ContainerResource> CreateContainer(CreateContainerBody body, CancellationToken cancellationToken = default);
        public virtual ClientResult CreateContainer(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ContainerResource>> CreateContainerAsync(CreateContainerBody body, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CreateContainerAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult CreateContainerFile(string containerId, BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult> CreateContainerFileAsync(string containerId, BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult DeleteContainer(string containerId, RequestOptions options);
        public virtual ClientResult<DeleteContainerResponse> DeleteContainer(string containerId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteContainerAsync(string containerId, RequestOptions options);
        public virtual Task<ClientResult<DeleteContainerResponse>> DeleteContainerAsync(string containerId, CancellationToken cancellationToken = default);
        public virtual ClientResult DeleteContainerFile(string containerId, string fileId, RequestOptions options);
        public virtual ClientResult<DeleteContainerFileResponse> DeleteContainerFile(string containerId, string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteContainerFileAsync(string containerId, string fileId, RequestOptions options);
        public virtual Task<ClientResult<DeleteContainerFileResponse>> DeleteContainerFileAsync(string containerId, string fileId, CancellationToken cancellationToken = default);
        public virtual ClientResult DownloadContainerFile(string containerId, string fileId, RequestOptions options);
        public virtual ClientResult<BinaryData> DownloadContainerFile(string containerId, string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DownloadContainerFileAsync(string containerId, string fileId, RequestOptions options);
        public virtual Task<ClientResult<BinaryData>> DownloadContainerFileAsync(string containerId, string fileId, CancellationToken cancellationToken = default);
        public virtual ClientResult GetContainer(string containerId, RequestOptions options);
        public virtual ClientResult<ContainerResource> GetContainer(string containerId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetContainerAsync(string containerId, RequestOptions options);
        public virtual Task<ClientResult<ContainerResource>> GetContainerAsync(string containerId, CancellationToken cancellationToken = default);
        public virtual ClientResult GetContainerFile(string containerId, string fileId, RequestOptions options);
        public virtual ClientResult<ContainerFileResource> GetContainerFile(string containerId, string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetContainerFileAsync(string containerId, string fileId, RequestOptions options);
        public virtual Task<ClientResult<ContainerFileResource>> GetContainerFileAsync(string containerId, string fileId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ContainerFileResource> GetContainerFiles(string containerId, ContainerFileCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetContainerFiles(string containerId, int? limit, string order, string after, RequestOptions options);
        public virtual AsyncCollectionResult<ContainerFileResource> GetContainerFilesAsync(string containerId, ContainerFileCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetContainerFilesAsync(string containerId, int? limit, string order, string after, RequestOptions options);
        public virtual CollectionResult<ContainerResource> GetContainers(ContainerCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetContainers(int? limit, string order, string after, RequestOptions options);
        public virtual AsyncCollectionResult<ContainerResource> GetContainersAsync(ContainerCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetContainersAsync(int? limit, string order, string after, RequestOptions options);
    }
    [Experimental("OPENAI001")]
    public class ContainerCollectionOptions : IJsonModel<ContainerCollectionOptions>, IPersistableModel<ContainerCollectionOptions> {
        public string AfterId { get; set; }
        public ContainerCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        protected virtual ContainerCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ContainerCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ContainerCollectionOrder : IEquatable<ContainerCollectionOrder> {
        public ContainerCollectionOrder(string value);
        public static ContainerCollectionOrder Ascending { get; }
        public static ContainerCollectionOrder Descending { get; }
        public readonly bool Equals(ContainerCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ContainerCollectionOrder left, ContainerCollectionOrder right);
        public static implicit operator ContainerCollectionOrder(string value);
        public static implicit operator ContainerCollectionOrder?(string value);
        public static bool operator !=(ContainerCollectionOrder left, ContainerCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ContainerFileCollectionOptions : IJsonModel<ContainerFileCollectionOptions>, IPersistableModel<ContainerFileCollectionOptions> {
        public string AfterId { get; set; }
        public ContainerCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        protected virtual ContainerFileCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ContainerFileCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ContainerFileResource : IJsonModel<ContainerFileResource>, IPersistableModel<ContainerFileResource> {
        public int Bytes { get; }
        public string ContainerId { get; }
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public string Object { get; }
        public string Path { get; }
        public string Source { get; }
        protected virtual ContainerFileResource JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ContainerFileResource(ClientResult result);
        protected virtual ContainerFileResource PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ContainerResource : IJsonModel<ContainerResource>, IPersistableModel<ContainerResource> {
        public DateTimeOffset CreatedAt { get; }
        public ContainerResourceExpiresAfter ExpiresAfter { get; }
        public string Id { get; }
        public string Name { get; }
        public string Object { get; }
        public string Status { get; }
        protected virtual ContainerResource JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ContainerResource(ClientResult result);
        protected virtual ContainerResource PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ContainerResourceExpiresAfter : IJsonModel<ContainerResourceExpiresAfter>, IPersistableModel<ContainerResourceExpiresAfter> {
        public string Anchor { get; }
        public int? Minutes { get; }
        protected virtual ContainerResourceExpiresAfter JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ContainerResourceExpiresAfter PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CreateContainerBody : IJsonModel<CreateContainerBody>, IPersistableModel<CreateContainerBody> {
        public CreateContainerBody(string name);
        public CreateContainerBodyExpiresAfter ExpiresAfter { get; set; }
        public IList<string> FileIds { get; }
        public string Name { get; }
        protected virtual CreateContainerBody JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static implicit operator BinaryContent(CreateContainerBody createContainerBody);
        protected virtual CreateContainerBody PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CreateContainerBodyExpiresAfter : IJsonModel<CreateContainerBodyExpiresAfter>, IPersistableModel<CreateContainerBodyExpiresAfter> {
        public CreateContainerBodyExpiresAfter(int minutes);
        public string Anchor { get; }
        public int Minutes { get; }
        protected virtual CreateContainerBodyExpiresAfter JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CreateContainerBodyExpiresAfter PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CreateContainerFileBody : IJsonModel<CreateContainerFileBody>, IPersistableModel<CreateContainerFileBody> {
        public BinaryData File { get; set; }
        public string FileId { get; set; }
        protected virtual CreateContainerFileBody JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CreateContainerFileBody PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class DeleteContainerFileResponse : IJsonModel<DeleteContainerFileResponse>, IPersistableModel<DeleteContainerFileResponse> {
        public bool Deleted { get; }
        public string Id { get; }
        public string Object { get; }
        protected virtual DeleteContainerFileResponse JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator DeleteContainerFileResponse(ClientResult result);
        protected virtual DeleteContainerFileResponse PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class DeleteContainerResponse : IJsonModel<DeleteContainerResponse>, IPersistableModel<DeleteContainerResponse> {
        public bool Deleted { get; }
        public string Id { get; }
        public string Object { get; }
        protected virtual DeleteContainerResponse JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator DeleteContainerResponse(ClientResult result);
        protected virtual DeleteContainerResponse PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
}
namespace OpenAI.Conversations {
    [Experimental("OPENAI001")]
    public class ConversationClient {
        protected ConversationClient();
        public ConversationClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public ConversationClient(ApiKeyCredential credential);
        public ConversationClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        public ConversationClient(AuthenticationPolicy authenticationPolicy);
        protected internal ConversationClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public ConversationClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult CreateConversation(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> CreateConversationAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult CreateConversationItems(string conversationId, BinaryContent content, IEnumerable<IncludedConversationItemProperty> include = null, RequestOptions options = null);
        public virtual Task<ClientResult> CreateConversationItemsAsync(string conversationId, BinaryContent content, IEnumerable<IncludedConversationItemProperty> include = null, RequestOptions options = null);
        public virtual ClientResult DeleteConversation(string conversationId, RequestOptions options = null);
        public virtual Task<ClientResult> DeleteConversationAsync(string conversationId, RequestOptions options = null);
        public virtual ClientResult DeleteConversationItem(string conversationId, string itemId, RequestOptions options = null);
        public virtual Task<ClientResult> DeleteConversationItemAsync(string conversationId, string itemId, RequestOptions options = null);
        public virtual ClientResult GetConversation(string conversationId, RequestOptions options = null);
        public virtual Task<ClientResult> GetConversationAsync(string conversationId, RequestOptions options = null);
        public virtual ClientResult GetConversationItem(string conversationId, string itemId, IEnumerable<IncludedConversationItemProperty> include = null, RequestOptions options = null);
        public virtual Task<ClientResult> GetConversationItemAsync(string conversationId, string itemId, IEnumerable<IncludedConversationItemProperty> include = null, RequestOptions options = null);
        public virtual CollectionResult GetConversationItems(string conversationId, long? limit = null, string order = null, string after = null, IEnumerable<IncludedConversationItemProperty> include = null, RequestOptions options = null);
        public virtual AsyncCollectionResult GetConversationItemsAsync(string conversationId, long? limit = null, string order = null, string after = null, IEnumerable<IncludedConversationItemProperty> include = null, RequestOptions options = null);
        public virtual ClientResult UpdateConversation(string conversationId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> UpdateConversationAsync(string conversationId, BinaryContent content, RequestOptions options = null);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct IncludedConversationItemProperty : IEquatable<IncludedConversationItemProperty> {
        public IncludedConversationItemProperty(string value);
        public static IncludedConversationItemProperty CodeInterpreterCallOutputs { get; }
        public static IncludedConversationItemProperty ComputerCallOutputImageUri { get; }
        public static IncludedConversationItemProperty FileSearchCallResults { get; }
        public static IncludedConversationItemProperty MessageInputImageUri { get; }
        public static IncludedConversationItemProperty MessageOutputTextLogprobs { get; }
        public static IncludedConversationItemProperty ReasoningEncryptedContent { get; }
        public static IncludedConversationItemProperty WebSearchCallActionSources { get; }
        public static IncludedConversationItemProperty WebSearchCallResults { get; }
        public readonly bool Equals(IncludedConversationItemProperty other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(IncludedConversationItemProperty left, IncludedConversationItemProperty right);
        public static implicit operator IncludedConversationItemProperty(string value);
        public static implicit operator IncludedConversationItemProperty?(string value);
        public static bool operator !=(IncludedConversationItemProperty left, IncludedConversationItemProperty right);
        public override readonly string ToString();
    }
}
namespace OpenAI.Embeddings {
    public class EmbeddingClient {
        protected EmbeddingClient();
        protected internal EmbeddingClient(ClientPipeline pipeline, string model, OpenAIClientOptions options);
        public EmbeddingClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public EmbeddingClient(string model, ApiKeyCredential credential);
        [Experimental("OPENAI001")]
        public EmbeddingClient(string model, AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        [Experimental("OPENAI001")]
        public EmbeddingClient(string model, AuthenticationPolicy authenticationPolicy);
        public EmbeddingClient(string model, string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        [Experimental("OPENAI001")]
        public string Model { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult<OpenAIEmbedding> GenerateEmbedding(string input, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<OpenAIEmbedding>> GenerateEmbeddingAsync(string input, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult GenerateEmbeddings(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<OpenAIEmbeddingCollection> GenerateEmbeddings(IEnumerable<ReadOnlyMemory<int>> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<OpenAIEmbeddingCollection> GenerateEmbeddings(IEnumerable<string> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GenerateEmbeddingsAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<OpenAIEmbeddingCollection>> GenerateEmbeddingsAsync(IEnumerable<ReadOnlyMemory<int>> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<OpenAIEmbeddingCollection>> GenerateEmbeddingsAsync(IEnumerable<string> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default);
    }
    public class EmbeddingGenerationOptions : IJsonModel<EmbeddingGenerationOptions>, IPersistableModel<EmbeddingGenerationOptions> {
        public int? Dimensions { get; set; }
        public string EndUserId { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        [Experimental("OPENAI001")]
        protected virtual EmbeddingGenerationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual EmbeddingGenerationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class EmbeddingTokenUsage : IJsonModel<EmbeddingTokenUsage>, IPersistableModel<EmbeddingTokenUsage> {
        public int InputTokenCount { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public int TotalTokenCount { get; }
        [Experimental("OPENAI001")]
        protected virtual EmbeddingTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual EmbeddingTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class OpenAIEmbedding : IJsonModel<OpenAIEmbedding>, IPersistableModel<OpenAIEmbedding> {
        public int Index { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        [Experimental("OPENAI001")]
        protected virtual OpenAIEmbedding JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual OpenAIEmbedding PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
        public ReadOnlyMemory<float> ToFloats();
    }
    public class OpenAIEmbeddingCollection : ObjectModel.ReadOnlyCollection<OpenAIEmbedding>, IJsonModel<OpenAIEmbeddingCollection>, IPersistableModel<OpenAIEmbeddingCollection> {
        public string Model { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public EmbeddingTokenUsage Usage { get; }
        [Experimental("OPENAI001")]
        protected virtual OpenAIEmbeddingCollection JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator OpenAIEmbeddingCollection(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual OpenAIEmbeddingCollection PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public static class OpenAIEmbeddingsModelFactory {
        public static EmbeddingTokenUsage EmbeddingTokenUsage(int inputTokenCount = 0, int totalTokenCount = 0);
        public static OpenAIEmbedding OpenAIEmbedding(int index = 0, IEnumerable<float> vector = null);
        public static OpenAIEmbeddingCollection OpenAIEmbeddingCollection(IEnumerable<OpenAIEmbedding> items = null, string model = null, EmbeddingTokenUsage usage = null);
    }
}
namespace OpenAI.Evals {
    [Experimental("OPENAI001")]
    public class EvaluationClient {
        protected EvaluationClient();
        public EvaluationClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public EvaluationClient(ApiKeyCredential credential);
        public EvaluationClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        public EvaluationClient(AuthenticationPolicy authenticationPolicy);
        protected internal EvaluationClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public EvaluationClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult CancelEvaluationRun(string evaluationId, string evaluationRunId, RequestOptions options);
        public virtual Task<ClientResult> CancelEvaluationRunAsync(string evaluationId, string evaluationRunId, RequestOptions options);
        public virtual ClientResult CreateEvaluation(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> CreateEvaluationAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult CreateEvaluationRun(string evaluationId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> CreateEvaluationRunAsync(string evaluationId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult DeleteEvaluation(string evaluationId, RequestOptions options);
        public virtual Task<ClientResult> DeleteEvaluationAsync(string evaluationId, RequestOptions options);
        public virtual ClientResult DeleteEvaluationRun(string evaluationId, string evaluationRunId, RequestOptions options);
        public virtual Task<ClientResult> DeleteEvaluationRunAsync(string evaluationId, string evaluationRunId, RequestOptions options);
        public virtual ClientResult GetEvaluation(string evaluationId, RequestOptions options);
        public virtual Task<ClientResult> GetEvaluationAsync(string evaluationId, RequestOptions options);
        public virtual ClientResult GetEvaluationRun(string evaluationId, string evaluationRunId, RequestOptions options);
        public virtual Task<ClientResult> GetEvaluationRunAsync(string evaluationId, string evaluationRunId, RequestOptions options);
        public virtual ClientResult GetEvaluationRunOutputItem(string evaluationId, string evaluationRunId, string outputItemId, RequestOptions options);
        public virtual Task<ClientResult> GetEvaluationRunOutputItemAsync(string evaluationId, string evaluationRunId, string outputItemId, RequestOptions options);
        public virtual ClientResult GetEvaluationRunOutputItems(string evaluationId, string evaluationRunId, int? limit, string order, string after, string outputItemStatus, RequestOptions options);
        public virtual Task<ClientResult> GetEvaluationRunOutputItemsAsync(string evaluationId, string evaluationRunId, int? limit, string order, string after, string outputItemStatus, RequestOptions options);
        public virtual ClientResult GetEvaluationRuns(string evaluationId, int? limit, string order, string after, string evaluationRunStatus, RequestOptions options);
        public virtual Task<ClientResult> GetEvaluationRunsAsync(string evaluationId, int? limit, string order, string after, string evaluationRunStatus, RequestOptions options);
        public virtual ClientResult GetEvaluations(int? limit, string orderBy, string order, string after, RequestOptions options);
        public virtual Task<ClientResult> GetEvaluationsAsync(int? limit, string orderBy, string order, string after, RequestOptions options);
        public virtual ClientResult UpdateEvaluation(string evaluationId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> UpdateEvaluationAsync(string evaluationId, BinaryContent content, RequestOptions options = null);
    }
}
namespace OpenAI.Files {
    public class FileDeletionResult : IJsonModel<FileDeletionResult>, IPersistableModel<FileDeletionResult> {
        public bool Deleted { get; }
        public string FileId { get; }
        [Experimental("OPENAI001")]
        protected virtual FileDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator FileDeletionResult(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual FileDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum FilePurpose {
        Assistants = 0,
        AssistantsOutput = 1,
        Batch = 2,
        BatchOutput = 3,
        FineTune = 4,
        FineTuneResults = 5,
        Vision = 6,
        UserData = 7,
        Evaluations = 8
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
        [Experimental("OPENAI001")]
        public static FileUploadPurpose Evaluations { get; }
        public static FileUploadPurpose FineTune { get; }
        [Experimental("OPENAI001")]
        public static FileUploadPurpose UserData { get; }
        public static FileUploadPurpose Vision { get; }
        public readonly bool Equals(FileUploadPurpose other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(FileUploadPurpose left, FileUploadPurpose right);
        public static implicit operator FileUploadPurpose(string value);
        public static implicit operator FileUploadPurpose?(string value);
        public static bool operator !=(FileUploadPurpose left, FileUploadPurpose right);
        public override readonly string ToString();
    }
    public class OpenAIFile : IJsonModel<OpenAIFile>, IPersistableModel<OpenAIFile> {
        public DateTimeOffset CreatedAt { get; }
        [Experimental("OPENAI001")]
        public DateTimeOffset? ExpiresAt { get; }
        public string Filename { get; }
        public string Id { get; }
        public FilePurpose Purpose { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int? SizeInBytes { get; }
        [Experimental("OPENAI001")]
        public long? SizeInBytesLong { get; }
        [Obsolete("This property is obsolete. If this is a fine-tuning training file, it may take some time to process after it has been uploaded. While the file is processing, you can still create a fine-tuning job but it will not start until the file processing has completed.")]
        public FileStatus Status { get; }
        [Obsolete("This property is obsolete. For details on why a fine-tuning training file failed validation, see the `error` field on the fine-tuning job.")]
        public string StatusDetails { get; }
        [Experimental("OPENAI001")]
        protected virtual OpenAIFile JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator OpenAIFile(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual OpenAIFile PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class OpenAIFileClient {
        protected OpenAIFileClient();
        public OpenAIFileClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public OpenAIFileClient(ApiKeyCredential credential);
        [Experimental("OPENAI001")]
        public OpenAIFileClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        [Experimental("OPENAI001")]
        public OpenAIFileClient(AuthenticationPolicy authenticationPolicy);
        protected internal OpenAIFileClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public OpenAIFileClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
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
        public virtual ClientResult DeleteFile(string fileId, RequestOptions options);
        public virtual ClientResult<FileDeletionResult> DeleteFile(string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteFileAsync(string fileId, RequestOptions options);
        public virtual Task<ClientResult<FileDeletionResult>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);
        public virtual ClientResult DownloadFile(string fileId, RequestOptions options);
        public virtual ClientResult<BinaryData> DownloadFile(string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DownloadFileAsync(string fileId, RequestOptions options);
        public virtual Task<ClientResult<BinaryData>> DownloadFileAsync(string fileId, CancellationToken cancellationToken = default);
        public virtual ClientResult GetFile(string fileId, RequestOptions options);
        public virtual ClientResult<OpenAIFile> GetFile(string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetFileAsync(string fileId, RequestOptions options);
        public virtual Task<ClientResult<OpenAIFile>> GetFileAsync(string fileId, CancellationToken cancellationToken = default);
        public virtual ClientResult<OpenAIFileCollection> GetFiles(FilePurpose purpose, CancellationToken cancellationToken = default);
        public virtual ClientResult GetFiles(string purpose, RequestOptions options);
        public virtual ClientResult<OpenAIFileCollection> GetFiles(CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<OpenAIFileCollection>> GetFilesAsync(FilePurpose purpose, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetFilesAsync(string purpose, RequestOptions options);
        public virtual Task<ClientResult<OpenAIFileCollection>> GetFilesAsync(CancellationToken cancellationToken = default);
        public virtual ClientResult<OpenAIFile> UploadFile(BinaryData file, string filename, FileUploadPurpose purpose);
        public virtual ClientResult UploadFile(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult<OpenAIFile> UploadFile(Stream file, string filename, FileUploadPurpose purpose, CancellationToken cancellationToken = default);
        public virtual ClientResult<OpenAIFile> UploadFile(string filePath, FileUploadPurpose purpose);
        public virtual Task<ClientResult<OpenAIFile>> UploadFileAsync(BinaryData file, string filename, FileUploadPurpose purpose);
        public virtual Task<ClientResult> UploadFileAsync(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult<OpenAIFile>> UploadFileAsync(Stream file, string filename, FileUploadPurpose purpose, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<OpenAIFile>> UploadFileAsync(string filePath, FileUploadPurpose purpose);
    }
    public class OpenAIFileCollection : ObjectModel.ReadOnlyCollection<OpenAIFile>, IJsonModel<OpenAIFileCollection>, IPersistableModel<OpenAIFileCollection> {
        [Experimental("OPENAI001")]
        protected virtual OpenAIFileCollection JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator OpenAIFileCollection(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual OpenAIFileCollection PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public static class OpenAIFilesModelFactory {
        public static FileDeletionResult FileDeletionResult(string fileId = null, bool deleted = false);
        public static OpenAIFileCollection OpenAIFileCollection(IEnumerable<OpenAIFile> items = null);
        [Experimental("OPENAI001")]
        public static OpenAIFile OpenAIFileInfo(string id = null, int? sizeInBytes = null, DateTimeOffset createdAt = default, string filename = null, FilePurpose purpose = FilePurpose.Assistants, FileStatus status = FileStatus.Uploaded, string statusDetails = null, DateTimeOffset? expiresAt = null, long? sizeInBytesLong = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static OpenAIFile OpenAIFileInfo(string id, int? sizeInBytes, DateTimeOffset createdAt, string filename, FilePurpose purpose, FileStatus status, string statusDetails);
    }
}
namespace OpenAI.FineTuning {
    [Experimental("OPENAI001")]
    public class FineTuningCheckpoint : IJsonModel<FineTuningCheckpoint>, IPersistableModel<FineTuningCheckpoint> {
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public string JobId { get; }
        public FineTuningCheckpointMetrics Metrics { get; }
        public string ModelId { get; }
        public int StepNumber { get; }
        protected virtual FineTuningCheckpoint JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuningCheckpoint PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
        public override string ToString();
    }
    [Experimental("OPENAI001")]
    public class FineTuningCheckpointMetrics : IJsonModel<FineTuningCheckpointMetrics>, IPersistableModel<FineTuningCheckpointMetrics> {
        public float? FullValidLoss { get; }
        public float? FullValidMeanTokenAccuracy { get; }
        public int StepNumber { get; }
        public float? TrainLoss { get; }
        public float? TrainMeanTokenAccuracy { get; }
        public float? ValidLoss { get; }
        public float? ValidMeanTokenAccuracy { get; }
        protected virtual FineTuningCheckpointMetrics JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuningCheckpointMetrics PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FineTuningClient {
        protected FineTuningClient();
        public FineTuningClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public FineTuningClient(ApiKeyCredential credential);
        public FineTuningClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        public FineTuningClient(AuthenticationPolicy authenticationPolicy);
        protected internal FineTuningClient(ClientPipeline pipeline, OpenAIClientOptions options);
        protected internal FineTuningClient(ClientPipeline pipeline, Uri endpoint);
        public FineTuningClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult CreateFineTuningCheckpointPermission(string fineTunedModelCheckpoint, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> CreateFineTuningCheckpointPermissionAsync(string fineTunedModelCheckpoint, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult DeleteFineTuningCheckpointPermission(string fineTunedModelCheckpoint, string permissionId, RequestOptions options);
        public virtual Task<ClientResult> DeleteFineTuningCheckpointPermissionAsync(string fineTunedModelCheckpoint, string permissionId, RequestOptions options);
        public virtual FineTuningJob FineTune(BinaryContent content, bool waitUntilCompleted, RequestOptions options);
        public virtual FineTuningJob FineTune(string baseModel, string trainingFileId, bool waitUntilCompleted, FineTuningOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<FineTuningJob> FineTuneAsync(BinaryContent content, bool waitUntilCompleted, RequestOptions options);
        public virtual Task<FineTuningJob> FineTuneAsync(string baseModel, string trainingFileId, bool waitUntilCompleted, FineTuningOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult GetFineTuningCheckpointPermissions(string fineTunedModelCheckpoint, string after, int? limit, string order, string projectId, RequestOptions options);
        public virtual Task<ClientResult> GetFineTuningCheckpointPermissionsAsync(string fineTunedModelCheckpoint, string after, int? limit, string order, string projectId, RequestOptions options);
        public virtual FineTuningJob GetJob(string jobId, CancellationToken cancellationToken = default);
        public virtual Task<FineTuningJob> GetJobAsync(string jobId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<FineTuningJob> GetJobs(FineTuningJobCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<FineTuningJob> GetJobsAsync(FineTuningJobCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult PauseFineTuningJob(string fineTuningJobId, RequestOptions options);
        public virtual Task<ClientResult> PauseFineTuningJobAsync(string fineTuningJobId, RequestOptions options);
        public virtual ClientResult ResumeFineTuningJob(string fineTuningJobId, RequestOptions options);
        public virtual Task<ClientResult> ResumeFineTuningJobAsync(string fineTuningJobId, RequestOptions options);
    }
    [Experimental("OPENAI001")]
    public class FineTuningError : IJsonModel<FineTuningError>, IPersistableModel<FineTuningError> {
        public string Code { get; }
        public string InvalidParameter { get; }
        public string Message { get; }
        protected virtual FineTuningError JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuningError PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FineTuningEvent : IJsonModel<FineTuningEvent>, IPersistableModel<FineTuningEvent> {
        public string Level;
        public DateTimeOffset CreatedAt { get; }
        public BinaryData Data { get; }
        public string Id { get; }
        public FineTuningJobEventKind? Kind { get; }
        public string Message { get; }
        protected virtual FineTuningEvent JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuningEvent PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct FineTuningHyperparameters : IJsonModel<FineTuningHyperparameters>, IPersistableModel<FineTuningHyperparameters>, IJsonModel<object>, IPersistableModel<object> {
        public int BatchSize { get; }
        public int EpochCount { get; }
        public float LearningRateMultiplier { get; }
    }
    [Experimental("OPENAI001")]
    public class FineTuningIntegration : IJsonModel<FineTuningIntegration>, IPersistableModel<FineTuningIntegration> {
        protected virtual FineTuningIntegration JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuningIntegration PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FineTuningJob : OperationResult {
        public string? Value;
        public string BaseModel { get; }
        public int BillableTrainedTokenCount { get; }
        public DateTimeOffset? EstimatedFinishAt { get; }
        [Obsolete("This property is deprecated. Use the MethodHyperparameters property instead.")]
        public FineTuningHyperparameters Hyperparameters { get; }
        public IReadOnlyList<FineTuningIntegration> Integrations { get; }
        public string JobId { get; }
        public IDictionary<string, string> Metadata { get; }
        public MethodHyperparameters? MethodHyperparameters { get; }
        public override ContinuationToken? RehydrationToken { get; protected set; }
        public IReadOnlyList<string> ResultFileIds { get; }
        public int? Seed { get; }
        public FineTuningStatus Status { get; }
        public string TrainingFileId { get; }
        public FineTuningTrainingMethod? TrainingMethod { get; }
        public string? UserProvidedSuffix { get; }
        public string ValidationFileId { get; }
        public virtual ClientResult Cancel(RequestOptions options);
        public virtual ClientResult CancelAndUpdate(CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CancelAndUpdateAsync(CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CancelAsync(RequestOptions options);
        public virtual CollectionResult<FineTuningCheckpoint> GetCheckpoints(GetCheckpointsOptions? options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetCheckpoints(string? after, int? limit, RequestOptions? options);
        public virtual AsyncCollectionResult<FineTuningCheckpoint> GetCheckpointsAsync(GetCheckpointsOptions? options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetCheckpointsAsync(string? after, int? limit, RequestOptions? options);
        public virtual CollectionResult<FineTuningEvent> GetEvents(GetEventsOptions options, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetEvents(string? after, int? limit, RequestOptions options);
        public virtual AsyncCollectionResult<FineTuningEvent> GetEventsAsync(GetEventsOptions options, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetEventsAsync(string? after, int? limit, RequestOptions options);
        public static FineTuningJob Rehydrate(FineTuningClient client, ContinuationToken rehydrationToken, RequestOptions options);
        public static FineTuningJob Rehydrate(FineTuningClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static FineTuningJob Rehydrate(FineTuningClient client, string JobId, RequestOptions options);
        public static FineTuningJob Rehydrate(FineTuningClient client, string JobId, CancellationToken cancellationToken = default);
        public static Task<FineTuningJob> RehydrateAsync(FineTuningClient client, ContinuationToken rehydrationToken, RequestOptions options);
        public static Task<FineTuningJob> RehydrateAsync(FineTuningClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static Task<FineTuningJob> RehydrateAsync(FineTuningClient client, string JobId, RequestOptions options);
        public static Task<FineTuningJob> RehydrateAsync(FineTuningClient client, string JobId, CancellationToken cancellationToken = default);
        public override ClientResult UpdateStatus(RequestOptions? options);
        public ClientResult UpdateStatus(CancellationToken cancellationToken = default);
        public override ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options);
        public ValueTask<ClientResult> UpdateStatusAsync(CancellationToken cancellationToken = default);
        public override void WaitForCompletion(CancellationToken cancellationToken = default);
        public override ValueTask WaitForCompletionAsync(CancellationToken cancellationToken = default);
    }
    [Experimental("OPENAI001")]
    public class FineTuningJobCollectionOptions {
        public string AfterJobId { get; set; }
        public int? PageSize { get; set; }
    }
    [Experimental("OPENAI001")]
    public readonly partial struct FineTuningJobEventKind : IEquatable<FineTuningJobEventKind> {
        public FineTuningJobEventKind(string value);
        public static FineTuningJobEventKind Message { get; }
        public static FineTuningJobEventKind Metrics { get; }
        public readonly bool Equals(FineTuningJobEventKind other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(FineTuningJobEventKind left, FineTuningJobEventKind right);
        public static implicit operator FineTuningJobEventKind(string value);
        public static implicit operator FineTuningJobEventKind?(string value);
        public static bool operator !=(FineTuningJobEventKind left, FineTuningJobEventKind right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class FineTuningOptions : IJsonModel<FineTuningOptions>, IPersistableModel<FineTuningOptions> {
        public IList<FineTuningIntegration> Integrations { get; }
        public IDictionary<string, string> Metadata { get; }
        public int? Seed { get; set; }
        public string Suffix { get; set; }
        public FineTuningTrainingMethod TrainingMethod { get; set; }
        public string ValidationFile { get; set; }
        protected virtual FineTuningOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuningOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct FineTuningStatus : IEquatable<string>, IEquatable<FineTuningStatus> {
        public FineTuningStatus(string value);
        public static FineTuningStatus Cancelled { get; }
        public static FineTuningStatus Failed { get; }
        public bool InProgress { get; }
        public static FineTuningStatus Queued { get; }
        public static FineTuningStatus Running { get; }
        public static FineTuningStatus Succeeded { get; }
        public static FineTuningStatus ValidatingFiles { get; }
        public readonly bool Equals(FineTuningStatus other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        public readonly bool Equals(string other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(FineTuningStatus left, FineTuningStatus right);
        public static implicit operator FineTuningStatus(string value);
        public static implicit operator FineTuningStatus?(string value);
        public static bool operator !=(FineTuningStatus left, FineTuningStatus right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class FineTuningTrainingMethod : IJsonModel<FineTuningTrainingMethod>, IPersistableModel<FineTuningTrainingMethod> {
        public static FineTuningTrainingMethod CreateDirectPreferenceOptimization(HyperparameterBatchSize batchSize = null, HyperparameterEpochCount epochCount = null, HyperparameterLearningRate learningRate = null, HyperparameterBetaFactor betaFactor = null);
        public static FineTuningTrainingMethod CreateSupervised(HyperparameterBatchSize batchSize = null, HyperparameterEpochCount epochCount = null, HyperparameterLearningRate learningRate = null);
        protected virtual FineTuningTrainingMethod JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuningTrainingMethod PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class GetCheckpointsOptions {
        public string AfterCheckpointId { get; set; }
        public int? PageSize { get; set; }
    }
    [Experimental("OPENAI001")]
    public class GetEventsOptions {
        public string AfterEventId { get; set; }
        public int? PageSize { get; set; }
    }
    [Experimental("OPENAI001")]
    public class HyperparameterBatchSize : IEquatable<int>, IEquatable<string>, IJsonModel<HyperparameterBatchSize>, IPersistableModel<HyperparameterBatchSize> {
        public HyperparameterBatchSize(int batchSize);
        public static HyperparameterBatchSize CreateAuto();
        public static HyperparameterBatchSize CreateSize(int batchSize);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(int other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(string other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode();
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator ==(HyperparameterBatchSize first, HyperparameterBatchSize second);
        public static implicit operator HyperparameterBatchSize(int batchSize);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator !=(HyperparameterBatchSize first, HyperparameterBatchSize second);
    }
    [Experimental("OPENAI001")]
    public class HyperparameterBetaFactor : IEquatable<int>, IEquatable<string>, IJsonModel<HyperparameterBetaFactor>, IPersistableModel<HyperparameterBetaFactor> {
        public HyperparameterBetaFactor(int beta);
        public static HyperparameterBetaFactor CreateAuto();
        public static HyperparameterBetaFactor CreateBeta(int beta);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(int other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(string other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode();
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator ==(HyperparameterBetaFactor first, HyperparameterBetaFactor second);
        public static implicit operator HyperparameterBetaFactor(int beta);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator !=(HyperparameterBetaFactor first, HyperparameterBetaFactor second);
    }
    [Experimental("OPENAI001")]
    public class HyperparameterEpochCount : IEquatable<int>, IEquatable<string>, IJsonModel<HyperparameterEpochCount>, IPersistableModel<HyperparameterEpochCount> {
        public HyperparameterEpochCount(int epochCount);
        public static HyperparameterEpochCount CreateAuto();
        public static HyperparameterEpochCount CreateEpochCount(int epochCount);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(int other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(string other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode();
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator ==(HyperparameterEpochCount first, HyperparameterEpochCount second);
        public static implicit operator HyperparameterEpochCount(int epochCount);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator !=(HyperparameterEpochCount first, HyperparameterEpochCount second);
    }
    [Experimental("OPENAI001")]
    public class HyperparameterLearningRate : IEquatable<double>, IEquatable<int>, IEquatable<string>, IJsonModel<HyperparameterLearningRate>, IPersistableModel<HyperparameterLearningRate> {
        public HyperparameterLearningRate(double learningRateMultiplier);
        public static HyperparameterLearningRate CreateAuto();
        public static HyperparameterLearningRate CreateMultiplier(double learningRateMultiplier);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(double other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(int other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(string other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode();
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator ==(HyperparameterLearningRate first, HyperparameterLearningRate second);
        public static implicit operator HyperparameterLearningRate(double learningRateMultiplier);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator !=(HyperparameterLearningRate first, HyperparameterLearningRate second);
    }
    [Experimental("OPENAI001")]
    public class HyperparametersForDPO : MethodHyperparameters, IJsonModel<HyperparametersForDPO>, IPersistableModel<HyperparametersForDPO> {
        public int BatchSize { get; }
        public float Beta { get; }
        public int EpochCount { get; }
        public float LearningRateMultiplier { get; }
        protected virtual HyperparametersForDPO JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual HyperparametersForDPO PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class HyperparametersForSupervised : MethodHyperparameters, IJsonModel<HyperparametersForSupervised>, IPersistableModel<HyperparametersForSupervised> {
        public int BatchSize { get; }
        public int EpochCount { get; }
        public float LearningRateMultiplier { get; }
        protected virtual HyperparametersForSupervised JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual HyperparametersForSupervised PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class MethodHyperparameters {
    }
    [Experimental("OPENAI001")]
    public class WeightsAndBiasesIntegration : FineTuningIntegration, IJsonModel<WeightsAndBiasesIntegration>, IPersistableModel<WeightsAndBiasesIntegration> {
        public WeightsAndBiasesIntegration(string projectName);
        public string DisplayName { get; set; }
        public string EntityName { get; set; }
        public string ProjectName { get; set; }
        public IList<string> Tags { get; }
        protected override FineTuningIntegration JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override FineTuningIntegration PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
}
namespace OpenAI.Graders {
    [Experimental("OPENAI001")]
    public class FineTuneReinforcementHyperparameters : IJsonModel<FineTuneReinforcementHyperparameters>, IPersistableModel<FineTuneReinforcementHyperparameters> {
        public BinaryData BatchSize { get; set; }
        public BinaryData ComputeMultiplier { get; set; }
        public BinaryData EvalInterval { get; set; }
        public BinaryData EvalSamples { get; set; }
        public BinaryData LearningRateMultiplier { get; set; }
        public BinaryData NEpochs { get; set; }
        protected virtual FineTuneReinforcementHyperparameters JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuneReinforcementHyperparameters PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    [PersistableModelProxy(typeof(UnknownGrader))]
    public class Grader : IJsonModel<Grader>, IPersistableModel<Grader> {
        protected virtual Grader JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual Grader PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class GraderClient {
        protected GraderClient();
        public GraderClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public GraderClient(ApiKeyCredential credential);
        public GraderClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        public GraderClient(AuthenticationPolicy authenticationPolicy);
        protected internal GraderClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public GraderClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult RunGrader(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> RunGraderAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult ValidateGrader(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> ValidateGraderAsync(BinaryContent content, RequestOptions options = null);
    }
    [Experimental("OPENAI001")]
    public class GraderLabelModel : Grader, IJsonModel<GraderLabelModel>, IPersistableModel<GraderLabelModel> {
        public IList<string> Labels { get; }
        public string Model { get; set; }
        public string Name { get; set; }
        public IList<string> PassingLabels { get; }
        protected override Grader JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override Grader PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class GraderMulti : Grader, IJsonModel<GraderMulti>, IPersistableModel<GraderMulti> {
        public GraderMulti(string name, BinaryData graders, string calculateOutput);
        public string CalculateOutput { get; set; }
        public BinaryData Graders { get; set; }
        public string Name { get; set; }
        protected override Grader JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override Grader PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class GraderPython : Grader, IJsonModel<GraderPython>, IPersistableModel<GraderPython> {
        public GraderPython(string name, string source);
        public string ImageTag { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        protected override Grader JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override Grader PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class GraderScoreModel : Grader, IJsonModel<GraderScoreModel>, IPersistableModel<GraderScoreModel> {
        public string Model { get; set; }
        public string Name { get; set; }
        public IList<float> Range { get; }
        public BinaryData SamplingParams { get; set; }
        protected override Grader JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override Grader PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class GraderStringCheck : Grader, IJsonModel<GraderStringCheck>, IPersistableModel<GraderStringCheck> {
        public GraderStringCheck(string name, string input, string reference, GraderStringCheckOperation operation);
        public string Input { get; set; }
        public string Name { get; set; }
        public GraderStringCheckOperation Operation { get; set; }
        public string Reference { get; set; }
        protected override Grader JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override Grader PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct GraderStringCheckOperation : IEquatable<GraderStringCheckOperation> {
        public GraderStringCheckOperation(string value);
        public static GraderStringCheckOperation Eq { get; }
        public static GraderStringCheckOperation Ilike { get; }
        public static GraderStringCheckOperation Like { get; }
        public static GraderStringCheckOperation Ne { get; }
        public readonly bool Equals(GraderStringCheckOperation other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GraderStringCheckOperation left, GraderStringCheckOperation right);
        public static implicit operator GraderStringCheckOperation(string value);
        public static implicit operator GraderStringCheckOperation?(string value);
        public static bool operator !=(GraderStringCheckOperation left, GraderStringCheckOperation right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class GraderTextSimilarity : Grader, IJsonModel<GraderTextSimilarity>, IPersistableModel<GraderTextSimilarity> {
        public GraderTextSimilarity(string name, string input, string reference, GraderTextSimilarityEvaluationMetric evaluationMetric);
        public GraderTextSimilarityEvaluationMetric EvaluationMetric { get; set; }
        public string Input { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        protected override Grader JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override Grader PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct GraderTextSimilarityEvaluationMetric : IEquatable<GraderTextSimilarityEvaluationMetric> {
        public GraderTextSimilarityEvaluationMetric(string value);
        public static GraderTextSimilarityEvaluationMetric Bleu { get; }
        public static GraderTextSimilarityEvaluationMetric FuzzyMatch { get; }
        public static GraderTextSimilarityEvaluationMetric Gleu { get; }
        public static GraderTextSimilarityEvaluationMetric Meteor { get; }
        public static GraderTextSimilarityEvaluationMetric Rouge1 { get; }
        public static GraderTextSimilarityEvaluationMetric Rouge2 { get; }
        public static GraderTextSimilarityEvaluationMetric Rouge3 { get; }
        public static GraderTextSimilarityEvaluationMetric Rouge4 { get; }
        public static GraderTextSimilarityEvaluationMetric Rouge5 { get; }
        public static GraderTextSimilarityEvaluationMetric RougeL { get; }
        public readonly bool Equals(GraderTextSimilarityEvaluationMetric other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GraderTextSimilarityEvaluationMetric left, GraderTextSimilarityEvaluationMetric right);
        public static implicit operator GraderTextSimilarityEvaluationMetric(string value);
        public static implicit operator GraderTextSimilarityEvaluationMetric?(string value);
        public static bool operator !=(GraderTextSimilarityEvaluationMetric left, GraderTextSimilarityEvaluationMetric right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public readonly partial struct GraderType : IEquatable<GraderType> {
        public GraderType(string value);
        public static GraderType LabelModel { get; }
        public static GraderType Multi { get; }
        public static GraderType Python { get; }
        public static GraderType ScoreModel { get; }
        public static GraderType StringCheck { get; }
        public static GraderType TextSimilarity { get; }
        public readonly bool Equals(GraderType other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GraderType left, GraderType right);
        public static implicit operator GraderType(string value);
        public static implicit operator GraderType?(string value);
        public static bool operator !=(GraderType left, GraderType right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class RunGraderRequest : IJsonModel<RunGraderRequest>, IPersistableModel<RunGraderRequest> {
        public BinaryData Grader { get; }
        public BinaryData Item { get; }
        public string ModelSample { get; }
        protected virtual RunGraderRequest JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunGraderRequest PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class RunGraderResponse : IJsonModel<RunGraderResponse>, IPersistableModel<RunGraderResponse> {
        public RunGraderResponseMetadata Metadata { get; }
        public BinaryData ModelGraderTokenUsagePerModel { get; }
        public float Reward { get; }
        public BinaryData SubRewards { get; }
        protected virtual RunGraderResponse JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator RunGraderResponse(ClientResult result);
        protected virtual RunGraderResponse PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class RunGraderResponseMetadata : IJsonModel<RunGraderResponseMetadata>, IPersistableModel<RunGraderResponseMetadata> {
        public RunGraderResponseMetadataErrors Errors { get; }
        public float ExecutionTime { get; }
        public string Kind { get; }
        public string Name { get; }
        public string SampledModelName { get; }
        public BinaryData Scores { get; }
        public int? TokenUsage { get; }
        protected virtual RunGraderResponseMetadata JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunGraderResponseMetadata PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class RunGraderResponseMetadataErrors : IJsonModel<RunGraderResponseMetadataErrors>, IPersistableModel<RunGraderResponseMetadataErrors> {
        public bool FormulaParseError { get; }
        public bool InvalidVariableError { get; }
        public bool ModelGraderParseError { get; }
        public bool ModelGraderRefusalError { get; }
        public bool ModelGraderServerError { get; }
        public string ModelGraderServerErrorDetails { get; }
        public bool OtherError { get; }
        public bool PythonGraderRuntimeError { get; }
        public string PythonGraderRuntimeErrorDetails { get; }
        public bool PythonGraderServerError { get; }
        public string PythonGraderServerErrorType { get; }
        public bool SampleParseError { get; }
        public bool TruncatedObservationError { get; }
        public bool UnresponsiveRewardError { get; }
        protected virtual RunGraderResponseMetadataErrors JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunGraderResponseMetadataErrors PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class UnknownGrader : Grader, IJsonModel<Grader>, IPersistableModel<Grader> {
        protected override Grader JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override Grader PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ValidateGraderRequest : IJsonModel<ValidateGraderRequest>, IPersistableModel<ValidateGraderRequest> {
        public BinaryData Grader { get; }
        protected virtual ValidateGraderRequest JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ValidateGraderRequest PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ValidateGraderResponse : IJsonModel<ValidateGraderResponse>, IPersistableModel<ValidateGraderResponse> {
        public BinaryData Grader { get; }
        protected virtual ValidateGraderResponse JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ValidateGraderResponse(ClientResult result);
        protected virtual ValidateGraderResponse PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
}
namespace OpenAI.Images {
    public class GeneratedImage : IJsonModel<GeneratedImage>, IPersistableModel<GeneratedImage> {
        public BinaryData ImageBytes { get; }
        public Uri ImageUri { get; }
        public string RevisedPrompt { get; }
        [Experimental("OPENAI001")]
        protected virtual GeneratedImage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual GeneratedImage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct GeneratedImageBackground : IEquatable<GeneratedImageBackground> {
        public GeneratedImageBackground(string value);
        public static GeneratedImageBackground Auto { get; }
        public static GeneratedImageBackground Opaque { get; }
        public static GeneratedImageBackground Transparent { get; }
        public readonly bool Equals(GeneratedImageBackground other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedImageBackground left, GeneratedImageBackground right);
        public static implicit operator GeneratedImageBackground(string value);
        public static implicit operator GeneratedImageBackground?(string value);
        public static bool operator !=(GeneratedImageBackground left, GeneratedImageBackground right);
        public override readonly string ToString();
    }
    public class GeneratedImageCollection : ObjectModel.ReadOnlyCollection<GeneratedImage>, IJsonModel<GeneratedImageCollection>, IPersistableModel<GeneratedImageCollection> {
        public DateTimeOffset CreatedAt { get; }
        [Experimental("OPENAI001")]
        public ImageTokenUsage Usage { get; }
        [Experimental("OPENAI001")]
        protected virtual GeneratedImageCollection JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator GeneratedImageCollection(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual GeneratedImageCollection PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct GeneratedImageFileFormat : IEquatable<GeneratedImageFileFormat> {
        public GeneratedImageFileFormat(string value);
        public static GeneratedImageFileFormat Jpeg { get; }
        public static GeneratedImageFileFormat Png { get; }
        public static GeneratedImageFileFormat Webp { get; }
        public readonly bool Equals(GeneratedImageFileFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedImageFileFormat left, GeneratedImageFileFormat right);
        public static implicit operator GeneratedImageFileFormat(string value);
        public static implicit operator GeneratedImageFileFormat?(string value);
        public static bool operator !=(GeneratedImageFileFormat left, GeneratedImageFileFormat right);
        public override readonly string ToString();
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
        public static implicit operator GeneratedImageFormat?(string value);
        public static bool operator !=(GeneratedImageFormat left, GeneratedImageFormat right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public readonly partial struct GeneratedImageModerationLevel : IEquatable<GeneratedImageModerationLevel> {
        public GeneratedImageModerationLevel(string value);
        public static GeneratedImageModerationLevel Auto { get; }
        public static GeneratedImageModerationLevel Low { get; }
        public readonly bool Equals(GeneratedImageModerationLevel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedImageModerationLevel left, GeneratedImageModerationLevel right);
        public static implicit operator GeneratedImageModerationLevel(string value);
        public static implicit operator GeneratedImageModerationLevel?(string value);
        public static bool operator !=(GeneratedImageModerationLevel left, GeneratedImageModerationLevel right);
        public override readonly string ToString();
    }
    public readonly partial struct GeneratedImageQuality : IEquatable<GeneratedImageQuality> {
        public GeneratedImageQuality(string value);
        [Experimental("OPENAI001")]
        public static GeneratedImageQuality Auto { get; }
        public static GeneratedImageQuality High { get; }
        [Experimental("OPENAI001")]
        public static GeneratedImageQuality Low { get; }
        [Experimental("OPENAI001")]
        public static GeneratedImageQuality Medium { get; }
        public static GeneratedImageQuality Standard { get; }
        public readonly bool Equals(GeneratedImageQuality other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GeneratedImageQuality left, GeneratedImageQuality right);
        public static implicit operator GeneratedImageQuality(string value);
        public static implicit operator GeneratedImageQuality?(string value);
        public static bool operator !=(GeneratedImageQuality left, GeneratedImageQuality right);
        public override readonly string ToString();
    }
    public readonly partial struct GeneratedImageSize : IEquatable<GeneratedImageSize> {
        public static readonly GeneratedImageSize W1024xH1024;
        [Experimental("OPENAI001")]
        public static readonly GeneratedImageSize W1024xH1536;
        public static readonly GeneratedImageSize W1024xH1792;
        [Experimental("OPENAI001")]
        public static readonly GeneratedImageSize W1536xH1024;
        public static readonly GeneratedImageSize W1792xH1024;
        public static readonly GeneratedImageSize W256xH256;
        public static readonly GeneratedImageSize W512xH512;
        public GeneratedImageSize(int width, int height);
        [Experimental("OPENAI001")]
        public static GeneratedImageSize Auto { get; }
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
        public static implicit operator GeneratedImageStyle?(string value);
        public static bool operator !=(GeneratedImageStyle left, GeneratedImageStyle right);
        public override readonly string ToString();
    }
    public class ImageClient {
        protected ImageClient();
        protected internal ImageClient(ClientPipeline pipeline, string model, OpenAIClientOptions options);
        public ImageClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public ImageClient(string model, ApiKeyCredential credential);
        [Experimental("OPENAI001")]
        public ImageClient(string model, AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        [Experimental("OPENAI001")]
        public ImageClient(string model, AuthenticationPolicy authenticationPolicy);
        public ImageClient(string model, string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        [Experimental("OPENAI001")]
        public string Model { get; }
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
        public virtual ClientResult GenerateImageEdits(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(Stream image, string imageFilename, string prompt, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(string imageFilePath, string prompt, int imageCount, ImageEditOptions options = null);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageEdits(string imageFilePath, string prompt, string maskFilePath, int imageCount, ImageEditOptions options = null);
        public virtual Task<ClientResult> GenerateImageEditsAsync(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(Stream image, string imageFilename, string prompt, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(Stream image, string imageFilename, string prompt, Stream mask, string maskFilename, int imageCount, ImageEditOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(string imageFilePath, string prompt, int imageCount, ImageEditOptions options = null);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageEditsAsync(string imageFilePath, string prompt, string maskFilePath, int imageCount, ImageEditOptions options = null);
        public virtual ClientResult GenerateImages(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<GeneratedImageCollection> GenerateImages(string prompt, int imageCount, ImageGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GenerateImagesAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImagesAsync(string prompt, int imageCount, ImageGenerationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImage> GenerateImageVariation(Stream image, string imageFilename, ImageVariationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImage> GenerateImageVariation(string imageFilePath, ImageVariationOptions options = null);
        public virtual Task<ClientResult<GeneratedImage>> GenerateImageVariationAsync(Stream image, string imageFilename, ImageVariationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImage>> GenerateImageVariationAsync(string imageFilePath, ImageVariationOptions options = null);
        public virtual ClientResult GenerateImageVariations(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageVariations(Stream image, string imageFilename, int imageCount, ImageVariationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<GeneratedImageCollection> GenerateImageVariations(string imageFilePath, int imageCount, ImageVariationOptions options = null);
        public virtual Task<ClientResult> GenerateImageVariationsAsync(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageVariationsAsync(Stream image, string imageFilename, int imageCount, ImageVariationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<GeneratedImageCollection>> GenerateImageVariationsAsync(string imageFilePath, int imageCount, ImageVariationOptions options = null);
    }
    public class ImageEditOptions : IJsonModel<ImageEditOptions>, IPersistableModel<ImageEditOptions> {
        [Experimental("OPENAI001")]
        public GeneratedImageBackground? Background { get; set; }
        public string EndUserId { get; set; }
        [Experimental("OPENAI001")]
        public GeneratedImageQuality? Quality { get; set; }
        public GeneratedImageFormat? ResponseFormat { get; set; }
        public GeneratedImageSize? Size { get; set; }
        [Experimental("OPENAI001")]
        protected virtual ImageEditOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ImageEditOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ImageGenerationOptions : IJsonModel<ImageGenerationOptions>, IPersistableModel<ImageGenerationOptions> {
        [Experimental("OPENAI001")]
        public GeneratedImageBackground? Background { get; set; }
        public string EndUserId { get; set; }
        [Experimental("OPENAI001")]
        public GeneratedImageModerationLevel? ModerationLevel { get; set; }
        [Experimental("OPENAI001")]
        public int? OutputCompressionFactor { get; set; }
        [Experimental("OPENAI001")]
        public GeneratedImageFileFormat? OutputFileFormat { get; set; }
        public GeneratedImageQuality? Quality { get; set; }
        public GeneratedImageFormat? ResponseFormat { get; set; }
        public GeneratedImageSize? Size { get; set; }
        public GeneratedImageStyle? Style { get; set; }
        [Experimental("OPENAI001")]
        protected virtual ImageGenerationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ImageGenerationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ImageInputTokenUsageDetails : IJsonModel<ImageInputTokenUsageDetails>, IPersistableModel<ImageInputTokenUsageDetails> {
        public int ImageTokenCount { get; }
        public int TextTokenCount { get; }
        protected virtual ImageInputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ImageInputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ImageTokenUsage : IJsonModel<ImageTokenUsage>, IPersistableModel<ImageTokenUsage> {
        public int InputTokenCount { get; }
        public ImageInputTokenUsageDetails InputTokenDetails { get; }
        public int OutputTokenCount { get; }
        public int TotalTokenCount { get; }
        protected virtual ImageTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ImageTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ImageVariationOptions : IJsonModel<ImageVariationOptions>, IPersistableModel<ImageVariationOptions> {
        public string EndUserId { get; set; }
        public GeneratedImageFormat? ResponseFormat { get; set; }
        public GeneratedImageSize? Size { get; set; }
        [Experimental("OPENAI001")]
        protected virtual ImageVariationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ImageVariationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public static class OpenAIImagesModelFactory {
        public static GeneratedImage GeneratedImage(BinaryData imageBytes = null, Uri imageUri = null, string revisedPrompt = null);
        [Experimental("OPENAI001")]
        public static GeneratedImageCollection GeneratedImageCollection(DateTimeOffset createdAt = default, IEnumerable<GeneratedImage> items = null, ImageTokenUsage usage = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static GeneratedImageCollection GeneratedImageCollection(DateTimeOffset createdAt, IEnumerable<GeneratedImage> items);
    }
}
namespace OpenAI.Models {
    public class ModelDeletionResult : IJsonModel<ModelDeletionResult>, IPersistableModel<ModelDeletionResult> {
        public bool Deleted { get; }
        public string ModelId { get; }
        [Experimental("OPENAI001")]
        protected virtual ModelDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator ModelDeletionResult(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual ModelDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class OpenAIModel : IJsonModel<OpenAIModel>, IPersistableModel<OpenAIModel> {
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public string OwnedBy { get; }
        [Experimental("OPENAI001")]
        protected virtual OpenAIModel JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator OpenAIModel(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual OpenAIModel PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class OpenAIModelClient {
        protected OpenAIModelClient();
        public OpenAIModelClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public OpenAIModelClient(ApiKeyCredential credential);
        [Experimental("OPENAI001")]
        public OpenAIModelClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        [Experimental("OPENAI001")]
        public OpenAIModelClient(AuthenticationPolicy authenticationPolicy);
        protected internal OpenAIModelClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public OpenAIModelClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult DeleteModel(string model, RequestOptions options);
        public virtual ClientResult<ModelDeletionResult> DeleteModel(string model, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteModelAsync(string model, RequestOptions options);
        public virtual Task<ClientResult<ModelDeletionResult>> DeleteModelAsync(string model, CancellationToken cancellationToken = default);
        public virtual ClientResult GetModel(string model, RequestOptions options);
        public virtual ClientResult<OpenAIModel> GetModel(string model, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetModelAsync(string model, RequestOptions options);
        public virtual Task<ClientResult<OpenAIModel>> GetModelAsync(string model, CancellationToken cancellationToken = default);
        public virtual ClientResult GetModels(RequestOptions options);
        public virtual ClientResult<OpenAIModelCollection> GetModels(CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetModelsAsync(RequestOptions options);
        public virtual Task<ClientResult<OpenAIModelCollection>> GetModelsAsync(CancellationToken cancellationToken = default);
    }
    public class OpenAIModelCollection : ObjectModel.ReadOnlyCollection<OpenAIModel>, IJsonModel<OpenAIModelCollection>, IPersistableModel<OpenAIModelCollection> {
        [Experimental("OPENAI001")]
        protected virtual OpenAIModelCollection JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator OpenAIModelCollection(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual OpenAIModelCollection PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        [Experimental("OPENAI001")]
        public ModerationClient(string model, AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        [Experimental("OPENAI001")]
        public ModerationClient(string model, AuthenticationPolicy authenticationPolicy);
        public ModerationClient(string model, string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        [Experimental("OPENAI001")]
        public string Model { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult ClassifyText(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ModerationResultCollection> ClassifyText(IEnumerable<string> inputs, CancellationToken cancellationToken = default);
        public virtual ClientResult<ModerationResult> ClassifyText(string input, CancellationToken cancellationToken = default);
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
        [Experimental("OPENAI001")]
        protected virtual ModerationResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual ModerationResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ModerationResultCollection : ObjectModel.ReadOnlyCollection<ModerationResult>, IJsonModel<ModerationResultCollection>, IPersistableModel<ModerationResultCollection> {
        public string Id { get; }
        public string Model { get; }
        [Experimental("OPENAI001")]
        protected virtual ModerationResultCollection JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        public static explicit operator ModerationResultCollection(ClientResult result);
        [Experimental("OPENAI001")]
        protected virtual ModerationResultCollection PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public static class OpenAIModerationsModelFactory {
        public static ModerationCategory ModerationCategory(bool flagged = false, float score = 0);
        [Experimental("OPENAI001")]
        public static ModerationResult ModerationResult(bool flagged = false, ModerationCategory hate = null, ModerationCategory hateThreatening = null, ModerationCategory harassment = null, ModerationCategory harassmentThreatening = null, ModerationCategory selfHarm = null, ModerationCategory selfHarmIntent = null, ModerationCategory selfHarmInstructions = null, ModerationCategory sexual = null, ModerationCategory sexualMinors = null, ModerationCategory violence = null, ModerationCategory violenceGraphic = null, ModerationCategory illicit = null, ModerationCategory illicitViolent = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ModerationResult ModerationResult(bool flagged, ModerationCategory hate, ModerationCategory hateThreatening, ModerationCategory harassment, ModerationCategory harassmentThreatening, ModerationCategory selfHarm, ModerationCategory selfHarmIntent, ModerationCategory selfHarmInstructions, ModerationCategory sexual, ModerationCategory sexualMinors, ModerationCategory violence, ModerationCategory violenceGraphic);
        public static ModerationResultCollection ModerationResultCollection(string id = null, string model = null, IEnumerable<ModerationResult> items = null);
    }
}
namespace OpenAI.Realtime {
    [Experimental("OPENAI002")]
    public class ConversationContentPart : IJsonModel<ConversationContentPart>, IPersistableModel<ConversationContentPart> {
        public string AudioTranscript { get; }
        public string Text { get; }
        public static ConversationContentPart CreateInputAudioTranscriptPart(string transcript = null);
        public static ConversationContentPart CreateInputTextPart(string text);
        public static ConversationContentPart CreateOutputAudioTranscriptPart(string transcript = null);
        public static ConversationContentPart CreateOutputTextPart(string text);
        protected virtual ConversationContentPart JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static implicit operator ConversationContentPart(string text);
        protected virtual ConversationContentPart PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator ConversationContentPartKind?(string value);
        public static bool operator !=(ConversationContentPartKind left, ConversationContentPartKind right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class ConversationFunctionTool : ConversationTool, IJsonModel<ConversationFunctionTool>, IPersistableModel<ConversationFunctionTool> {
        public ConversationFunctionTool(string name);
        public string Description { get; set; }
        public string Name { get; set; }
        public BinaryData Parameters { get; set; }
        protected override ConversationTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ConversationTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator ConversationIncompleteReason?(string value);
        public static bool operator !=(ConversationIncompleteReason left, ConversationIncompleteReason right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class ConversationInputTokenUsageDetails : IJsonModel<ConversationInputTokenUsageDetails>, IPersistableModel<ConversationInputTokenUsageDetails> {
        public int AudioTokenCount { get; }
        public int CachedTokenCount { get; }
        public int TextTokenCount { get; }
        protected virtual ConversationInputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationInputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator ConversationItemStatus?(string value);
        public static bool operator !=(ConversationItemStatus left, ConversationItemStatus right);
        public override readonly string ToString();
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
        public static implicit operator ConversationMessageRole?(string value);
        public static bool operator !=(ConversationMessageRole left, ConversationMessageRole right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class ConversationOutputTokenUsageDetails : IJsonModel<ConversationOutputTokenUsageDetails>, IPersistableModel<ConversationOutputTokenUsageDetails> {
        public int AudioTokenCount { get; }
        public int TextTokenCount { get; }
        protected virtual ConversationOutputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationOutputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ConversationRateLimitDetailsItem : IJsonModel<ConversationRateLimitDetailsItem>, IPersistableModel<ConversationRateLimitDetailsItem> {
        public int MaximumCount { get; }
        public string Name { get; }
        public int RemainingCount { get; }
        public TimeSpan TimeUntilReset { get; }
        protected virtual ConversationRateLimitDetailsItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationRateLimitDetailsItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ConversationResponseOptions : IJsonModel<ConversationResponseOptions>, IPersistableModel<ConversationResponseOptions> {
        public ConversationVoice? Voice;
        public RealtimeContentModalities ContentModalities { get; set; }
        public ResponseConversationSelection? ConversationSelection { get; set; }
        public string Instructions { get; set; }
        public ConversationMaxTokensChoice MaxOutputTokens { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public RealtimeAudioFormat? OutputAudioFormat { get; set; }
        public IList<RealtimeItem> OverrideItems { get; }
        public float? Temperature { get; set; }
        public ConversationToolChoice ToolChoice { get; set; }
        public IList<ConversationTool> Tools { get; }
        protected virtual ConversationResponseOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationResponseOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ConversationSessionConfiguredUpdate : RealtimeUpdate, IJsonModel<ConversationSessionConfiguredUpdate>, IPersistableModel<ConversationSessionConfiguredUpdate> {
        public RealtimeContentModalities ContentModalities { get; }
        public RealtimeAudioFormat InputAudioFormat { get; }
        public InputTranscriptionOptions InputTranscriptionOptions { get; }
        public string Instructions { get; }
        public ConversationMaxTokensChoice MaxOutputTokens { get; }
        public string Model { get; }
        public RealtimeAudioFormat OutputAudioFormat { get; }
        public string SessionId { get; }
        public float Temperature { get; }
        public ConversationToolChoice ToolChoice { get; }
        public IReadOnlyList<ConversationTool> Tools { get; }
        public TurnDetectionOptions TurnDetectionOptions { get; }
        public ConversationVoice Voice { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ConversationSessionOptions : IJsonModel<ConversationSessionOptions>, IPersistableModel<ConversationSessionOptions> {
        public RealtimeContentModalities ContentModalities { get; set; }
        public RealtimeAudioFormat? InputAudioFormat { get; set; }
        public InputNoiseReductionOptions InputNoiseReductionOptions { get; set; }
        public InputTranscriptionOptions InputTranscriptionOptions { get; set; }
        public string Instructions { get; set; }
        public ConversationMaxTokensChoice MaxOutputTokens { get; set; }
        public RealtimeAudioFormat? OutputAudioFormat { get; set; }
        public float? Temperature { get; set; }
        public ConversationToolChoice ToolChoice { get; set; }
        public IList<ConversationTool> Tools { get; }
        public TurnDetectionOptions TurnDetectionOptions { get; set; }
        public ConversationVoice? Voice { get; set; }
        protected virtual ConversationSessionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationSessionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ConversationSessionStartedUpdate : RealtimeUpdate, IJsonModel<ConversationSessionStartedUpdate>, IPersistableModel<ConversationSessionStartedUpdate> {
        public RealtimeContentModalities ContentModalities { get; }
        public RealtimeAudioFormat InputAudioFormat { get; }
        public InputTranscriptionOptions InputTranscriptionOptions { get; }
        public string Instructions { get; }
        public ConversationMaxTokensChoice MaxOutputTokens { get; }
        public string Model { get; }
        public RealtimeAudioFormat OutputAudioFormat { get; }
        public string SessionId { get; }
        public float Temperature { get; }
        public ConversationToolChoice ToolChoice { get; }
        public IReadOnlyList<ConversationTool> Tools { get; }
        public TurnDetectionOptions TurnDetectionOptions { get; }
        public ConversationVoice Voice { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator ConversationStatus?(string value);
        public static bool operator !=(ConversationStatus left, ConversationStatus right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class ConversationStatusDetails : IJsonModel<ConversationStatusDetails>, IPersistableModel<ConversationStatusDetails> {
        public string ErrorCode { get; }
        public string ErrorKind { get; }
        public ConversationIncompleteReason? IncompleteReason { get; }
        public ConversationStatus StatusKind { get; }
        protected virtual ConversationStatusDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationStatusDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ConversationTokenUsage : IJsonModel<ConversationTokenUsage>, IPersistableModel<ConversationTokenUsage> {
        public int InputTokenCount { get; }
        public ConversationInputTokenUsageDetails InputTokenDetails { get; }
        public int OutputTokenCount { get; }
        public ConversationOutputTokenUsageDetails OutputTokenDetails { get; }
        public int TotalTokenCount { get; }
        protected virtual ConversationTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ConversationTool : IJsonModel<ConversationTool>, IPersistableModel<ConversationTool> {
        public ConversationToolKind Kind { get; }
        public static ConversationTool CreateFunctionTool(string name, string description = null, BinaryData parameters = null);
        protected virtual ConversationTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator ConversationToolKind?(string value);
        public static bool operator !=(ConversationToolKind left, ConversationToolKind right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public readonly partial struct ConversationVoice : IEquatable<ConversationVoice> {
        public ConversationVoice(string value);
        public static ConversationVoice Alloy { get; }
        public static ConversationVoice Ash { get; }
        public static ConversationVoice Ballad { get; }
        public static ConversationVoice Coral { get; }
        public static ConversationVoice Echo { get; }
        public static ConversationVoice Fable { get; }
        public static ConversationVoice Nova { get; }
        public static ConversationVoice Onyx { get; }
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
        public static implicit operator ConversationVoice?(string value);
        public static bool operator !=(ConversationVoice left, ConversationVoice right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class InputAudioClearedUpdate : RealtimeUpdate, IJsonModel<InputAudioClearedUpdate>, IPersistableModel<InputAudioClearedUpdate> {
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class InputAudioCommittedUpdate : RealtimeUpdate, IJsonModel<InputAudioCommittedUpdate>, IPersistableModel<InputAudioCommittedUpdate> {
        public string ItemId { get; }
        public string PreviousItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class InputAudioSpeechFinishedUpdate : RealtimeUpdate, IJsonModel<InputAudioSpeechFinishedUpdate>, IPersistableModel<InputAudioSpeechFinishedUpdate> {
        public TimeSpan AudioEndTime { get; }
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class InputAudioSpeechStartedUpdate : RealtimeUpdate, IJsonModel<InputAudioSpeechStartedUpdate>, IPersistableModel<InputAudioSpeechStartedUpdate> {
        public TimeSpan AudioStartTime { get; }
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class InputAudioTranscriptionDeltaUpdate : RealtimeUpdate, IJsonModel<InputAudioTranscriptionDeltaUpdate>, IPersistableModel<InputAudioTranscriptionDeltaUpdate> {
        public int? ContentIndex { get; }
        public string Delta { get; }
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class InputAudioTranscriptionFailedUpdate : RealtimeUpdate, IJsonModel<InputAudioTranscriptionFailedUpdate>, IPersistableModel<InputAudioTranscriptionFailedUpdate> {
        public int ContentIndex { get; }
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public string ErrorParameterName { get; }
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class InputAudioTranscriptionFinishedUpdate : RealtimeUpdate, IJsonModel<InputAudioTranscriptionFinishedUpdate>, IPersistableModel<InputAudioTranscriptionFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public string Transcript { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public enum InputNoiseReductionKind {
        Unknown = 0,
        NearField = 1,
        FarField = 2,
        Disabled = 3
    }
    [Experimental("OPENAI002")]
    public class InputNoiseReductionOptions : IJsonModel<InputNoiseReductionOptions>, IPersistableModel<InputNoiseReductionOptions> {
        public InputNoiseReductionKind Kind { get; set; }
        public static InputNoiseReductionOptions CreateDisabledOptions();
        public static InputNoiseReductionOptions CreateFarFieldOptions();
        public static InputNoiseReductionOptions CreateNearFieldOptions();
        protected virtual InputNoiseReductionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual InputNoiseReductionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public readonly partial struct InputTranscriptionModel : IEquatable<InputTranscriptionModel> {
        public InputTranscriptionModel(string value);
        public static InputTranscriptionModel Whisper1 { get; }
        public readonly bool Equals(InputTranscriptionModel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(InputTranscriptionModel left, InputTranscriptionModel right);
        public static implicit operator InputTranscriptionModel(string value);
        public static implicit operator InputTranscriptionModel?(string value);
        public static bool operator !=(InputTranscriptionModel left, InputTranscriptionModel right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class InputTranscriptionOptions : IJsonModel<InputTranscriptionOptions>, IPersistableModel<InputTranscriptionOptions> {
        public string Language { get; set; }
        public InputTranscriptionModel? Model { get; set; }
        public string Prompt { get; set; }
        protected virtual InputTranscriptionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual InputTranscriptionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ItemCreatedUpdate : RealtimeUpdate, IJsonModel<ItemCreatedUpdate>, IPersistableModel<ItemCreatedUpdate> {
        public string FunctionCallArguments { get; }
        public string FunctionCallId { get; }
        public string FunctionCallOutput { get; }
        public string FunctionName { get; }
        public string ItemId { get; }
        public IReadOnlyList<ConversationContentPart> MessageContentParts { get; }
        public ConversationMessageRole? MessageRole { get; }
        public string PreviousItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ItemDeletedUpdate : RealtimeUpdate, IJsonModel<ItemDeletedUpdate>, IPersistableModel<ItemDeletedUpdate> {
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ItemRetrievedUpdate : RealtimeUpdate, IJsonModel<ItemRetrievedUpdate>, IPersistableModel<ItemRetrievedUpdate> {
        public RealtimeItem Item { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ItemTruncatedUpdate : RealtimeUpdate, IJsonModel<ItemTruncatedUpdate>, IPersistableModel<ItemTruncatedUpdate> {
        public int AudioEndMs { get; }
        public int ContentIndex { get; }
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class OutputAudioFinishedUpdate : RealtimeUpdate, IJsonModel<OutputAudioFinishedUpdate>, IPersistableModel<OutputAudioFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class OutputAudioTranscriptionFinishedUpdate : RealtimeUpdate, IJsonModel<OutputAudioTranscriptionFinishedUpdate>, IPersistableModel<OutputAudioTranscriptionFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        public string Transcript { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class OutputDeltaUpdate : RealtimeUpdate, IJsonModel<OutputDeltaUpdate>, IPersistableModel<OutputDeltaUpdate> {
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
    [Experimental("OPENAI002")]
    public class OutputPartFinishedUpdate : RealtimeUpdate, IJsonModel<OutputPartFinishedUpdate>, IPersistableModel<OutputPartFinishedUpdate> {
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
    public class OutputStreamingFinishedUpdate : RealtimeUpdate, IJsonModel<OutputStreamingFinishedUpdate>, IPersistableModel<OutputStreamingFinishedUpdate> {
        public string FunctionCallArguments { get; }
        public string FunctionCallId { get; }
        public string FunctionCallOutput { get; }
        public string FunctionName { get; }
        public string ItemId { get; }
        public IReadOnlyList<ConversationContentPart> MessageContentParts { get; }
        public ConversationMessageRole? MessageRole { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class OutputStreamingStartedUpdate : RealtimeUpdate, IJsonModel<OutputStreamingStartedUpdate>, IPersistableModel<OutputStreamingStartedUpdate> {
        public string FunctionCallArguments { get; }
        public string FunctionCallId { get; }
        public string FunctionCallOutput { get; }
        public string FunctionName { get; }
        public string ItemId { get; }
        public int ItemIndex { get; }
        public IReadOnlyList<ConversationContentPart> MessageContentParts { get; }
        public ConversationMessageRole? MessageRole { get; }
        public string ResponseId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class OutputTextFinishedUpdate : RealtimeUpdate, IJsonModel<OutputTextFinishedUpdate>, IPersistableModel<OutputTextFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        public string Text { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class RateLimitsUpdate : RealtimeUpdate, IJsonModel<RateLimitsUpdate>, IPersistableModel<RateLimitsUpdate> {
        public IReadOnlyList<ConversationRateLimitDetailsItem> AllDetails { get; }
        public ConversationRateLimitDetailsItem RequestDetails { get; }
        public ConversationRateLimitDetailsItem TokenDetails { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public readonly partial struct RealtimeAudioFormat : IEquatable<RealtimeAudioFormat> {
        public RealtimeAudioFormat(string value);
        public static RealtimeAudioFormat G711Alaw { get; }
        public static RealtimeAudioFormat G711Ulaw { get; }
        public static RealtimeAudioFormat Pcm16 { get; }
        public readonly bool Equals(RealtimeAudioFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(RealtimeAudioFormat left, RealtimeAudioFormat right);
        public static implicit operator RealtimeAudioFormat(string value);
        public static implicit operator RealtimeAudioFormat?(string value);
        public static bool operator !=(RealtimeAudioFormat left, RealtimeAudioFormat right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class RealtimeClient {
        protected RealtimeClient();
        public RealtimeClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public RealtimeClient(ApiKeyCredential credential);
        [Experimental("OPENAI001")]
        public RealtimeClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        [Experimental("OPENAI001")]
        public RealtimeClient(AuthenticationPolicy authenticationPolicy);
        protected internal RealtimeClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public RealtimeClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public event EventHandler<BinaryData> OnReceivingCommand { add; remove; }
        public event EventHandler<BinaryData> OnSendingCommand { add; remove; }
        public virtual ClientResult CreateEphemeralToken(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> CreateEphemeralTokenAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult CreateEphemeralTranscriptionToken(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> CreateEphemeralTranscriptionTokenAsync(BinaryContent content, RequestOptions options = null);
        public RealtimeSession StartConversationSession(string model, RealtimeSessionOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<RealtimeSession> StartConversationSessionAsync(string model, RealtimeSessionOptions options = null, CancellationToken cancellationToken = default);
        public RealtimeSession StartSession(string model, string intent, RealtimeSessionOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<RealtimeSession> StartSessionAsync(string model, string intent, RealtimeSessionOptions options = null, CancellationToken cancellationToken = default);
        public RealtimeSession StartTranscriptionSession(RealtimeSessionOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<RealtimeSession> StartTranscriptionSessionAsync(RealtimeSessionOptions options = null, CancellationToken cancellationToken = default);
    }
    [Experimental("OPENAI002")]
    [Flags]
    public enum RealtimeContentModalities {
        Default = 0,
        Text = 1,
        Audio = 2
    }
    [Experimental("OPENAI002")]
    public class RealtimeErrorUpdate : RealtimeUpdate, IJsonModel<RealtimeErrorUpdate>, IPersistableModel<RealtimeErrorUpdate> {
        public string ErrorCode { get; }
        public string ErrorEventId { get; }
        public string Message { get; }
        public string ParameterName { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class RealtimeItem : IJsonModel<RealtimeItem>, IPersistableModel<RealtimeItem> {
        public string FunctionArguments { get; }
        public string FunctionCallId { get; }
        public string FunctionName { get; }
        public string Id { get; set; }
        public IReadOnlyList<ConversationContentPart> MessageContentParts { get; }
        public ConversationMessageRole? MessageRole { get; }
        public static RealtimeItem CreateAssistantMessage(IEnumerable<ConversationContentPart> contentItems);
        public static RealtimeItem CreateFunctionCall(string name, string callId, string arguments);
        public static RealtimeItem CreateFunctionCallOutput(string callId, string output);
        public static RealtimeItem CreateSystemMessage(IEnumerable<ConversationContentPart> contentItems);
        public static RealtimeItem CreateUserMessage(IEnumerable<ConversationContentPart> contentItems);
        protected virtual RealtimeItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RealtimeItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class RealtimeSession : IDisposable {
        protected internal RealtimeSession(ApiKeyCredential credential, RealtimeClient parentClient, Uri endpoint, string model, string intent);
        public Net.WebSockets.WebSocket WebSocket { get; protected set; }
        public virtual void AddItem(RealtimeItem item, string previousItemId, CancellationToken cancellationToken = default);
        public virtual void AddItem(RealtimeItem item, CancellationToken cancellationToken = default);
        public virtual Task AddItemAsync(RealtimeItem item, string previousItemId, CancellationToken cancellationToken = default);
        public virtual Task AddItemAsync(RealtimeItem item, CancellationToken cancellationToken = default);
        public virtual void CancelResponse(CancellationToken cancellationToken = default);
        public virtual Task CancelResponseAsync(CancellationToken cancellationToken = default);
        public virtual void ClearInputAudio(CancellationToken cancellationToken = default);
        public virtual Task ClearInputAudioAsync(CancellationToken cancellationToken = default);
        public virtual void CommitPendingAudio(CancellationToken cancellationToken = default);
        public virtual Task CommitPendingAudioAsync(CancellationToken cancellationToken = default);
        public virtual Task ConfigureConversationSessionAsync(ConversationSessionOptions sessionOptions, CancellationToken cancellationToken = default);
        public virtual void ConfigureSession(ConversationSessionOptions sessionOptions, CancellationToken cancellationToken = default);
        public virtual void ConfigureTranscriptionSession(TranscriptionSessionOptions sessionOptions, CancellationToken cancellationToken = default);
        public virtual Task ConfigureTranscriptionSessionAsync(TranscriptionSessionOptions sessionOptions, CancellationToken cancellationToken = default);
        protected internal virtual void Connect(string queryString = null, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default);
        protected internal virtual Task ConnectAsync(string queryString = null, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default);
        public virtual void DeleteItem(string itemId, CancellationToken cancellationToken = default);
        public virtual Task DeleteItemAsync(string itemId, CancellationToken cancellationToken = default);
        public void Dispose();
        public virtual void InterruptResponse(CancellationToken cancellationToken = default);
        public virtual Task InterruptResponseAsync(CancellationToken cancellationToken = default);
        public virtual IEnumerable<ClientResult> ReceiveUpdates(RequestOptions options);
        public virtual IEnumerable<RealtimeUpdate> ReceiveUpdates(CancellationToken cancellationToken = default);
        public virtual IAsyncEnumerable<ClientResult> ReceiveUpdatesAsync(RequestOptions options);
        public virtual IAsyncEnumerable<RealtimeUpdate> ReceiveUpdatesAsync(CancellationToken cancellationToken = default);
        public virtual void RequestItemRetrieval(string itemId, CancellationToken cancellationToken = default);
        public virtual Task RequestItemRetrievalAsync(string itemId, CancellationToken cancellationToken = default);
        public virtual void SendCommand(BinaryData data, RequestOptions options);
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
    public class RealtimeSessionOptions {
        public IDictionary<string, string> Headers { get; }
        public string QueryString { get; set; }
    }
    [Experimental("OPENAI002")]
    public class RealtimeUpdate : IJsonModel<RealtimeUpdate>, IPersistableModel<RealtimeUpdate> {
        public string EventId { get; }
        public RealtimeUpdateKind Kind { get; }
        public BinaryData GetRawContent();
        protected virtual RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator RealtimeUpdate(ClientResult result);
        protected virtual RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public enum RealtimeUpdateKind {
        Unknown = 0,
        SessionStarted = 1,
        SessionConfigured = 2,
        ItemCreated = 3,
        ConversationCreated = 4,
        ItemRetrieved = 5,
        ItemDeleted = 6,
        ItemTruncated = 7,
        ResponseStarted = 8,
        ResponseFinished = 9,
        RateLimitsUpdated = 10,
        ItemStreamingStarted = 11,
        ItemStreamingFinished = 12,
        ItemContentPartStarted = 13,
        ItemContentPartFinished = 14,
        ItemStreamingPartAudioDelta = 15,
        ItemStreamingPartAudioFinished = 16,
        ItemStreamingPartAudioTranscriptionDelta = 17,
        ItemStreamingPartAudioTranscriptionFinished = 18,
        ItemStreamingPartTextDelta = 19,
        ItemStreamingPartTextFinished = 20,
        ItemStreamingFunctionCallArgumentsDelta = 21,
        ItemStreamingFunctionCallArgumentsFinished = 22,
        InputSpeechStarted = 23,
        InputSpeechStopped = 24,
        InputTranscriptionFinished = 25,
        InputTranscriptionDelta = 26,
        InputTranscriptionFailed = 27,
        InputAudioCommitted = 28,
        InputAudioCleared = 29,
        OutputAudioBufferCleared = 30,
        OutputAudioBufferStarted = 31,
        OutputAudioBufferStopped = 32,
        TranscriptionSessionStarted = 33,
        TranscriptionSessionConfigured = 34,
        Error = 35
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
        public static implicit operator ResponseConversationSelection?(string value);
        public static bool operator !=(ResponseConversationSelection left, ResponseConversationSelection right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class ResponseFinishedUpdate : RealtimeUpdate, IJsonModel<ResponseFinishedUpdate>, IPersistableModel<ResponseFinishedUpdate> {
        public IReadOnlyList<RealtimeItem> CreatedItems { get; }
        public string ResponseId { get; }
        public ConversationStatus? Status { get; }
        public ConversationStatusDetails StatusDetails { get; }
        public ConversationTokenUsage Usage { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class ResponseStartedUpdate : RealtimeUpdate, IJsonModel<ResponseStartedUpdate>, IPersistableModel<ResponseStartedUpdate> {
        public IReadOnlyList<RealtimeItem> CreatedItems { get; }
        public string ResponseId { get; }
        public ConversationStatus Status { get; }
        public ConversationStatusDetails StatusDetails { get; }
        public ConversationTokenUsage Usage { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public readonly partial struct SemanticEagernessLevel : IEquatable<SemanticEagernessLevel> {
        public SemanticEagernessLevel(string value);
        public static SemanticEagernessLevel Auto { get; }
        public static SemanticEagernessLevel High { get; }
        public static SemanticEagernessLevel Low { get; }
        public static SemanticEagernessLevel Medium { get; }
        public readonly bool Equals(SemanticEagernessLevel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(SemanticEagernessLevel left, SemanticEagernessLevel right);
        public static implicit operator SemanticEagernessLevel(string value);
        public static implicit operator SemanticEagernessLevel?(string value);
        public static bool operator !=(SemanticEagernessLevel left, SemanticEagernessLevel right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI002")]
    public class TranscriptionSessionConfiguredUpdate : RealtimeUpdate, IJsonModel<TranscriptionSessionConfiguredUpdate>, IPersistableModel<TranscriptionSessionConfiguredUpdate> {
        public RealtimeContentModalities ContentModalities { get; }
        public RealtimeAudioFormat InputAudioFormat { get; }
        public InputTranscriptionOptions InputAudioTranscription { get; }
        public TurnDetectionOptions TurnDetection { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public class TranscriptionSessionOptions : IJsonModel<TranscriptionSessionOptions>, IPersistableModel<TranscriptionSessionOptions> {
        public RealtimeContentModalities ContentModalities { get; set; }
        public IList<string> Include { get; }
        public RealtimeAudioFormat? InputAudioFormat { get; set; }
        public InputNoiseReductionOptions InputNoiseReductionOptions { get; set; }
        public InputTranscriptionOptions InputTranscriptionOptions { get; set; }
        public TurnDetectionOptions TurnDetectionOptions { get; set; }
        protected virtual TranscriptionSessionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual TranscriptionSessionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI002")]
    public enum TurnDetectionKind {
        Unknown = 0,
        ServerVoiceActivityDetection = 1,
        SemanticVoiceActivityDetection = 2,
        Disabled = 3
    }
    [Experimental("OPENAI002")]
    public class TurnDetectionOptions : IJsonModel<TurnDetectionOptions>, IPersistableModel<TurnDetectionOptions> {
        public TurnDetectionKind Kind { get; }
        public static TurnDetectionOptions CreateDisabledTurnDetectionOptions();
        public static TurnDetectionOptions CreateSemanticVoiceActivityTurnDetectionOptions(SemanticEagernessLevel? eagernessLevel = null, bool? enableAutomaticResponseCreation = null, bool? enableResponseInterruption = null);
        public static TurnDetectionOptions CreateServerVoiceActivityTurnDetectionOptions(float? detectionThreshold = null, TimeSpan? prefixPaddingDuration = null, TimeSpan? silenceDuration = null, bool? enableAutomaticResponseCreation = null, bool? enableResponseInterruption = null);
        protected virtual TurnDetectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual TurnDetectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
}
namespace OpenAI.Responses {
    [Experimental("OPENAI001")]
    public class AutomaticCodeInterpreterToolContainerConfiguration : CodeInterpreterToolContainerConfiguration, IJsonModel<AutomaticCodeInterpreterToolContainerConfiguration>, IPersistableModel<AutomaticCodeInterpreterToolContainerConfiguration> {
        public AutomaticCodeInterpreterToolContainerConfiguration();
        public IList<string> FileIds { get; }
        protected override CodeInterpreterToolContainerConfiguration JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override CodeInterpreterToolContainerConfiguration PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterCallImageOutput : CodeInterpreterCallOutput, IJsonModel<CodeInterpreterCallImageOutput>, IPersistableModel<CodeInterpreterCallImageOutput> {
        public CodeInterpreterCallImageOutput(Uri imageUri);
        public Uri ImageUri { get; set; }
        protected override CodeInterpreterCallOutput JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override CodeInterpreterCallOutput PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterCallLogsOutput : CodeInterpreterCallOutput, IJsonModel<CodeInterpreterCallLogsOutput>, IPersistableModel<CodeInterpreterCallLogsOutput> {
        public CodeInterpreterCallLogsOutput(string logs);
        public string Logs { get; set; }
        protected override CodeInterpreterCallOutput JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override CodeInterpreterCallOutput PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterCallOutput : IJsonModel<CodeInterpreterCallOutput>, IPersistableModel<CodeInterpreterCallOutput> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual CodeInterpreterCallOutput JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CodeInterpreterCallOutput PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterCallResponseItem : ResponseItem, IJsonModel<CodeInterpreterCallResponseItem>, IPersistableModel<CodeInterpreterCallResponseItem> {
        public CodeInterpreterCallResponseItem(string code);
        public string Code { get; set; }
        public string ContainerId { get; set; }
        public IList<CodeInterpreterCallOutput> Outputs { get; }
        public CodeInterpreterCallStatus? Status { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum CodeInterpreterCallStatus {
        InProgress = 0,
        Interpreting = 1,
        Completed = 2,
        Incomplete = 3,
        Failed = 4
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterTool : ResponseTool, IJsonModel<CodeInterpreterTool>, IPersistableModel<CodeInterpreterTool> {
        public CodeInterpreterTool(CodeInterpreterToolContainer container);
        public CodeInterpreterToolContainer Container { get; }
        protected override ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterToolContainer : IJsonModel<CodeInterpreterToolContainer>, IPersistableModel<CodeInterpreterToolContainer> {
        public CodeInterpreterToolContainer(CodeInterpreterToolContainerConfiguration containerConfiguration);
        public CodeInterpreterToolContainer(string containerId);
        public CodeInterpreterToolContainerConfiguration ContainerConfiguration { get; }
        public string ContainerId { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual CodeInterpreterToolContainer JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CodeInterpreterToolContainer PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CodeInterpreterToolContainerConfiguration : IJsonModel<CodeInterpreterToolContainerConfiguration>, IPersistableModel<CodeInterpreterToolContainerConfiguration> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static AutomaticCodeInterpreterToolContainerConfiguration CreateAutomaticContainerConfiguration(IEnumerable<string> fileIds = null);
        protected virtual CodeInterpreterToolContainerConfiguration JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CodeInterpreterToolContainerConfiguration PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAICUA001")]
    public class ComputerCallAction : IJsonModel<ComputerCallAction>, IPersistableModel<ComputerCallAction> {
        public Drawing.Point? ClickCoordinates { get; }
        public ComputerCallActionMouseButton? ClickMouseButton { get; }
        public Drawing.Point? DoubleClickCoordinates { get; }
        public IList<Drawing.Point> DragPath { get; }
        public IList<string> KeyPressKeyCodes { get; }
        public ComputerCallActionKind Kind { get; }
        public Drawing.Point? MoveCoordinates { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public Drawing.Point? ScrollCoordinates { get; }
        public int? ScrollHorizontalOffset { get; }
        public int? ScrollVerticalOffset { get; }
        public string TypeText { get; }
        public static ComputerCallAction CreateClickAction(Drawing.Point clickCoordinates, ComputerCallActionMouseButton clickMouseButton);
        public static ComputerCallAction CreateDoubleClickAction(Drawing.Point doubleClickCoordinates, ComputerCallActionMouseButton doubleClickMouseButton);
        public static ComputerCallAction CreateDragAction(IList<Drawing.Point> dragPath);
        public static ComputerCallAction CreateKeyPressAction(IList<string> keyCodes);
        public static ComputerCallAction CreateMoveAction(Drawing.Point moveCoordinates);
        public static ComputerCallAction CreateScreenshotAction();
        public static ComputerCallAction CreateScrollAction(Drawing.Point scrollCoordinates, int horizontalOffset, int verticalOffset);
        public static ComputerCallAction CreateTypeAction(string typeText);
        public static ComputerCallAction CreateWaitAction();
        protected virtual ComputerCallAction JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ComputerCallAction PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAICUA001")]
    public enum ComputerCallActionKind {
        Click = 0,
        DoubleClick = 1,
        Drag = 2,
        KeyPress = 3,
        Move = 4,
        Screenshot = 5,
        Scroll = 6,
        Type = 7,
        Wait = 8
    }
    [Experimental("OPENAICUA001")]
    public enum ComputerCallActionMouseButton {
        Left = 0,
        Right = 1,
        Wheel = 2,
        Back = 3,
        Forward = 4
    }
    [Experimental("OPENAICUA001")]
    public class ComputerCallOutput : IJsonModel<ComputerCallOutput>, IPersistableModel<ComputerCallOutput> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static ComputerCallOutput CreateScreenshotOutput(BinaryData screenshotImageBytes, string screenshotImageBytesMediaType);
        public static ComputerCallOutput CreateScreenshotOutput(string screenshotImageFileId);
        public static ComputerCallOutput CreateScreenshotOutput(Uri screenshotImageUri);
        protected virtual ComputerCallOutput JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ComputerCallOutput PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAICUA001")]
    public class ComputerCallOutputResponseItem : ResponseItem, IJsonModel<ComputerCallOutputResponseItem>, IPersistableModel<ComputerCallOutputResponseItem> {
        public ComputerCallOutputResponseItem(string callId, ComputerCallOutput output);
        public IList<ComputerCallSafetyCheck> AcknowledgedSafetyChecks { get; }
        public string CallId { get; set; }
        public ComputerCallOutput Output { get; set; }
        public ComputerCallOutputStatus? Status { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAICUA001")]
    public enum ComputerCallOutputStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    [Experimental("OPENAICUA001")]
    public class ComputerCallResponseItem : ResponseItem, IJsonModel<ComputerCallResponseItem>, IPersistableModel<ComputerCallResponseItem> {
        public ComputerCallResponseItem(string callId, ComputerCallAction action, IEnumerable<ComputerCallSafetyCheck> pendingSafetyChecks);
        public ComputerCallAction Action { get; set; }
        public string CallId { get; set; }
        public IList<ComputerCallSafetyCheck> PendingSafetyChecks { get; }
        public ComputerCallStatus? Status { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAICUA001")]
    public class ComputerCallSafetyCheck : IJsonModel<ComputerCallSafetyCheck>, IPersistableModel<ComputerCallSafetyCheck> {
        public ComputerCallSafetyCheck(string id, string code, string message);
        public string Code { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ComputerCallSafetyCheck JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ComputerCallSafetyCheck PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAICUA001")]
    public enum ComputerCallStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    [Experimental("OPENAI001")]
    public class ComputerTool : ResponseTool, IJsonModel<ComputerTool>, IPersistableModel<ComputerTool> {
        public ComputerTool(ComputerToolEnvironment environment, int displayWidth, int displayHeight);
        public int DisplayHeight { get; set; }
        public int DisplayWidth { get; set; }
        public ComputerToolEnvironment Environment { get; set; }
        protected override ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAICUA001")]
    public readonly partial struct ComputerToolEnvironment : IEquatable<ComputerToolEnvironment> {
        public ComputerToolEnvironment(string value);
        public static ComputerToolEnvironment Browser { get; }
        public static ComputerToolEnvironment Linux { get; }
        public static ComputerToolEnvironment Mac { get; }
        public static ComputerToolEnvironment Ubuntu { get; }
        public static ComputerToolEnvironment Windows { get; }
        public readonly bool Equals(ComputerToolEnvironment other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ComputerToolEnvironment left, ComputerToolEnvironment right);
        public static implicit operator ComputerToolEnvironment(string value);
        public static implicit operator ComputerToolEnvironment?(string value);
        public static bool operator !=(ComputerToolEnvironment left, ComputerToolEnvironment right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ContainerFileCitationMessageAnnotation : ResponseMessageAnnotation, IJsonModel<ContainerFileCitationMessageAnnotation>, IPersistableModel<ContainerFileCitationMessageAnnotation> {
        public ContainerFileCitationMessageAnnotation(string containerId, string fileId, int startIndex, int endIndex, string filename);
        public string ContainerId { get; set; }
        public int EndIndex { get; set; }
        public string FileId { get; set; }
        public string Filename { get; set; }
        public int StartIndex { get; set; }
        protected override ResponseMessageAnnotation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseMessageAnnotation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CreateResponseOptions : IJsonModel<CreateResponseOptions>, IPersistableModel<CreateResponseOptions> {
        public CreateResponseOptions();
        public CreateResponseOptions(string model, IEnumerable<ResponseItem> inputItems);
        public bool? BackgroundModeEnabled { get; set; }
        public ResponseConversationOptions ConversationOptions { get; set; }
        public string EndUserId { get; set; }
        public IList<IncludedResponseProperty> IncludedProperties { get; }
        public IList<ResponseItem> InputItems { get; }
        public string Instructions { get; set; }
        public int? MaxOutputTokenCount { get; set; }
        public int? MaxToolCallCount { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public string Model { get; set; }
        public bool? ParallelToolCallsEnabled { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string PreviousResponseId { get; set; }
        public ResponseReasoningOptions ReasoningOptions { get; set; }
        public string SafetyIdentifier { get; set; }
        public ResponseServiceTier? ServiceTier { get; set; }
        public bool? StoredOutputEnabled { get; set; }
        public bool? StreamingEnabled { get; set; }
        public float? Temperature { get; set; }
        public ResponseTextOptions TextOptions { get; set; }
        public ResponseToolChoice ToolChoice { get; set; }
        public IList<ResponseTool> Tools { get; }
        public int? TopLogProbabilityCount { get; set; }
        public float? TopP { get; set; }
        public ResponseTruncationMode? TruncationMode { get; set; }
        protected virtual CreateResponseOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static implicit operator BinaryContent(CreateResponseOptions createResponseOptions);
        protected virtual CreateResponseOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class CustomMcpToolCallApprovalPolicy : IJsonModel<CustomMcpToolCallApprovalPolicy>, IPersistableModel<CustomMcpToolCallApprovalPolicy> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public McpToolFilter ToolsAlwaysRequiringApproval { get; set; }
        public McpToolFilter ToolsNeverRequiringApproval { get; set; }
        protected virtual CustomMcpToolCallApprovalPolicy JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CustomMcpToolCallApprovalPolicy PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FileCitationMessageAnnotation : ResponseMessageAnnotation, IJsonModel<FileCitationMessageAnnotation>, IPersistableModel<FileCitationMessageAnnotation> {
        public FileCitationMessageAnnotation(string fileId, int index, string filename);
        public string FileId { get; set; }
        public string Filename { get; set; }
        public int Index { get; set; }
        protected override ResponseMessageAnnotation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseMessageAnnotation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FilePathMessageAnnotation : ResponseMessageAnnotation, IJsonModel<FilePathMessageAnnotation>, IPersistableModel<FilePathMessageAnnotation> {
        public FilePathMessageAnnotation(string fileId, int index);
        public string FileId { get; set; }
        public int Index { get; set; }
        protected override ResponseMessageAnnotation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseMessageAnnotation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FileSearchCallResponseItem : ResponseItem, IJsonModel<FileSearchCallResponseItem>, IPersistableModel<FileSearchCallResponseItem> {
        public FileSearchCallResponseItem(IEnumerable<string> queries);
        public IList<string> Queries { get; }
        public IList<FileSearchCallResult> Results { get; set; }
        public FileSearchCallStatus? Status { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FileSearchCallResult : IJsonModel<FileSearchCallResult>, IPersistableModel<FileSearchCallResult> {
        public IDictionary<string, BinaryData> Attributes { get; }
        public string FileId { get; set; }
        public string Filename { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public float? Score { get; set; }
        public string Text { get; set; }
        protected virtual FileSearchCallResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileSearchCallResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum FileSearchCallStatus {
        InProgress = 0,
        Searching = 1,
        Completed = 2,
        Incomplete = 3,
        Failed = 4
    }
    [Experimental("OPENAI001")]
    public class FileSearchTool : ResponseTool, IJsonModel<FileSearchTool>, IPersistableModel<FileSearchTool> {
        public FileSearchTool(IEnumerable<string> vectorStoreIds);
        public BinaryData Filters { get; set; }
        public int? MaxResultCount { get; set; }
        public FileSearchToolRankingOptions RankingOptions { get; set; }
        public IList<string> VectorStoreIds { get; }
        protected override ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct FileSearchToolRanker : IEquatable<FileSearchToolRanker> {
        public FileSearchToolRanker(string value);
        public static FileSearchToolRanker Auto { get; }
        public static FileSearchToolRanker Default20241115 { get; }
        public readonly bool Equals(FileSearchToolRanker other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(FileSearchToolRanker left, FileSearchToolRanker right);
        public static implicit operator FileSearchToolRanker(string value);
        public static implicit operator FileSearchToolRanker?(string value);
        public static bool operator !=(FileSearchToolRanker left, FileSearchToolRanker right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class FileSearchToolRankingOptions : IJsonModel<FileSearchToolRankingOptions>, IPersistableModel<FileSearchToolRankingOptions> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public FileSearchToolRanker? Ranker { get; set; }
        public float? ScoreThreshold { get; set; }
        protected virtual FileSearchToolRankingOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileSearchToolRankingOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FunctionCallOutputResponseItem : ResponseItem, IJsonModel<FunctionCallOutputResponseItem>, IPersistableModel<FunctionCallOutputResponseItem> {
        public FunctionCallOutputResponseItem(string callId, string functionOutput);
        public string CallId { get; set; }
        public string FunctionOutput { get; set; }
        public FunctionCallOutputStatus? Status { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum FunctionCallOutputStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    [Experimental("OPENAI001")]
    public class FunctionCallResponseItem : ResponseItem, IJsonModel<FunctionCallResponseItem>, IPersistableModel<FunctionCallResponseItem> {
        public FunctionCallResponseItem(string callId, string functionName, BinaryData functionArguments);
        public string CallId { get; set; }
        public BinaryData FunctionArguments { get; set; }
        public string FunctionName { get; set; }
        public FunctionCallStatus? Status { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum FunctionCallStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    [Experimental("OPENAI001")]
    public class FunctionTool : ResponseTool, IJsonModel<FunctionTool>, IPersistableModel<FunctionTool> {
        public FunctionTool(string functionName, BinaryData functionParameters, bool? strictModeEnabled);
        public string FunctionDescription { get; set; }
        public string FunctionName { get; set; }
        public BinaryData FunctionParameters { get; set; }
        public bool? StrictModeEnabled { get; set; }
        protected override ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class GetResponseOptions : IJsonModel<GetResponseOptions>, IPersistableModel<GetResponseOptions> {
        public GetResponseOptions(string responseId);
        public IList<IncludedResponseProperty> IncludedProperties { get; set; }
        public bool? IncludeObfuscation { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string ResponseId { get; }
        public int? StartingAfter { get; set; }
        public bool? StreamingEnabled { get; set; }
        protected virtual GetResponseOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual GetResponseOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct GlobalMcpToolCallApprovalPolicy : IEquatable<GlobalMcpToolCallApprovalPolicy> {
        public GlobalMcpToolCallApprovalPolicy(string value);
        public static GlobalMcpToolCallApprovalPolicy AlwaysRequireApproval { get; }
        public static GlobalMcpToolCallApprovalPolicy NeverRequireApproval { get; }
        public readonly bool Equals(GlobalMcpToolCallApprovalPolicy other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(GlobalMcpToolCallApprovalPolicy left, GlobalMcpToolCallApprovalPolicy right);
        public static implicit operator GlobalMcpToolCallApprovalPolicy(string value);
        public static implicit operator GlobalMcpToolCallApprovalPolicy?(string value);
        public static bool operator !=(GlobalMcpToolCallApprovalPolicy left, GlobalMcpToolCallApprovalPolicy right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ImageGenerationCallResponseItem : ResponseItem, IJsonModel<ImageGenerationCallResponseItem>, IPersistableModel<ImageGenerationCallResponseItem> {
        public ImageGenerationCallResponseItem(BinaryData imageResultBytes);
        public BinaryData ImageResultBytes { get; set; }
        public ImageGenerationCallStatus? Status { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum ImageGenerationCallStatus {
        InProgress = 0,
        Completed = 1,
        Generating = 2,
        Failed = 3
    }
    [Experimental("OPENAI001")]
    public class ImageGenerationTool : ResponseTool, IJsonModel<ImageGenerationTool>, IPersistableModel<ImageGenerationTool> {
        public ImageGenerationTool();
        public ImageGenerationToolBackground? Background { get; set; }
        public ImageGenerationToolInputFidelity? InputFidelity { get; set; }
        public ImageGenerationToolInputImageMask InputImageMask { get; set; }
        public string Model { get; set; }
        public ImageGenerationToolModerationLevel? ModerationLevel { get; set; }
        public int? OutputCompressionFactor { get; set; }
        public ImageGenerationToolOutputFileFormat? OutputFileFormat { get; set; }
        public int? PartialImageCount { get; set; }
        public ImageGenerationToolQuality? Quality { get; set; }
        public ImageGenerationToolSize? Size { get; set; }
        protected override ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ImageGenerationToolBackground : IEquatable<ImageGenerationToolBackground> {
        public ImageGenerationToolBackground(string value);
        public static ImageGenerationToolBackground Auto { get; }
        public static ImageGenerationToolBackground Opaque { get; }
        public static ImageGenerationToolBackground Transparent { get; }
        public readonly bool Equals(ImageGenerationToolBackground other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ImageGenerationToolBackground left, ImageGenerationToolBackground right);
        public static implicit operator ImageGenerationToolBackground(string value);
        public static implicit operator ImageGenerationToolBackground?(string value);
        public static bool operator !=(ImageGenerationToolBackground left, ImageGenerationToolBackground right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ImageGenerationToolInputFidelity : IEquatable<ImageGenerationToolInputFidelity> {
        public ImageGenerationToolInputFidelity(string value);
        public static ImageGenerationToolInputFidelity High { get; }
        public static ImageGenerationToolInputFidelity Low { get; }
        public readonly bool Equals(ImageGenerationToolInputFidelity other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ImageGenerationToolInputFidelity left, ImageGenerationToolInputFidelity right);
        public static implicit operator ImageGenerationToolInputFidelity(string value);
        public static implicit operator ImageGenerationToolInputFidelity?(string value);
        public static bool operator !=(ImageGenerationToolInputFidelity left, ImageGenerationToolInputFidelity right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ImageGenerationToolInputImageMask : IJsonModel<ImageGenerationToolInputImageMask>, IPersistableModel<ImageGenerationToolInputImageMask> {
        public ImageGenerationToolInputImageMask(BinaryData imageBytes, string imageBytesMediaType);
        public ImageGenerationToolInputImageMask(string fileId);
        public ImageGenerationToolInputImageMask(Uri imageUri);
        public string FileId { get; }
        public string ImageUrl { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ImageGenerationToolInputImageMask JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ImageGenerationToolInputImageMask PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ImageGenerationToolModerationLevel : IEquatable<ImageGenerationToolModerationLevel> {
        public ImageGenerationToolModerationLevel(string value);
        public static ImageGenerationToolModerationLevel Auto { get; }
        public static ImageGenerationToolModerationLevel Low { get; }
        public readonly bool Equals(ImageGenerationToolModerationLevel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ImageGenerationToolModerationLevel left, ImageGenerationToolModerationLevel right);
        public static implicit operator ImageGenerationToolModerationLevel(string value);
        public static implicit operator ImageGenerationToolModerationLevel?(string value);
        public static bool operator !=(ImageGenerationToolModerationLevel left, ImageGenerationToolModerationLevel right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ImageGenerationToolOutputFileFormat : IEquatable<ImageGenerationToolOutputFileFormat> {
        public ImageGenerationToolOutputFileFormat(string value);
        public static ImageGenerationToolOutputFileFormat Jpeg { get; }
        public static ImageGenerationToolOutputFileFormat Png { get; }
        public static ImageGenerationToolOutputFileFormat Webp { get; }
        public readonly bool Equals(ImageGenerationToolOutputFileFormat other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ImageGenerationToolOutputFileFormat left, ImageGenerationToolOutputFileFormat right);
        public static implicit operator ImageGenerationToolOutputFileFormat(string value);
        public static implicit operator ImageGenerationToolOutputFileFormat?(string value);
        public static bool operator !=(ImageGenerationToolOutputFileFormat left, ImageGenerationToolOutputFileFormat right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ImageGenerationToolQuality : IEquatable<ImageGenerationToolQuality> {
        public ImageGenerationToolQuality(string value);
        public static ImageGenerationToolQuality Auto { get; }
        public static ImageGenerationToolQuality High { get; }
        public static ImageGenerationToolQuality Low { get; }
        public static ImageGenerationToolQuality Medium { get; }
        public readonly bool Equals(ImageGenerationToolQuality other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ImageGenerationToolQuality left, ImageGenerationToolQuality right);
        public static implicit operator ImageGenerationToolQuality(string value);
        public static implicit operator ImageGenerationToolQuality?(string value);
        public static bool operator !=(ImageGenerationToolQuality left, ImageGenerationToolQuality right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ImageGenerationToolSize : IEquatable<ImageGenerationToolSize> {
        public static readonly ImageGenerationToolSize W1024xH1024;
        public static readonly ImageGenerationToolSize W1024xH1536;
        public static readonly ImageGenerationToolSize W1536xH1024;
        public ImageGenerationToolSize(int width, int height);
        public static ImageGenerationToolSize Auto { get; }
        public readonly bool Equals(ImageGenerationToolSize other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ImageGenerationToolSize left, ImageGenerationToolSize right);
        public static bool operator !=(ImageGenerationToolSize left, ImageGenerationToolSize right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public readonly partial struct IncludedResponseProperty : IEquatable<IncludedResponseProperty> {
        public IncludedResponseProperty(string value);
        public static IncludedResponseProperty CodeInterpreterCallOutputs { get; }
        public static IncludedResponseProperty ComputerCallOutputImageUri { get; }
        public static IncludedResponseProperty FileSearchCallResults { get; }
        public static IncludedResponseProperty MessageInputImageUri { get; }
        public static IncludedResponseProperty ReasoningEncryptedContent { get; }
        public readonly bool Equals(IncludedResponseProperty other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(IncludedResponseProperty left, IncludedResponseProperty right);
        public static implicit operator IncludedResponseProperty(string value);
        public static implicit operator IncludedResponseProperty?(string value);
        public static bool operator !=(IncludedResponseProperty left, IncludedResponseProperty right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class McpTool : ResponseTool, IJsonModel<McpTool>, IPersistableModel<McpTool> {
        public McpTool(string serverLabel, McpToolConnectorId connectorId);
        public McpTool(string serverLabel, Uri serverUri);
        public McpToolFilter AllowedTools { get; set; }
        public string AuthorizationToken { get; set; }
        public McpToolConnectorId? ConnectorId { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string ServerDescription { get; set; }
        public string ServerLabel { get; set; }
        public Uri ServerUri { get; set; }
        public McpToolCallApprovalPolicy ToolCallApprovalPolicy { get; set; }
        protected override ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class McpToolCallApprovalPolicy : IJsonModel<McpToolCallApprovalPolicy>, IPersistableModel<McpToolCallApprovalPolicy> {
        public McpToolCallApprovalPolicy(CustomMcpToolCallApprovalPolicy customPolicy);
        public McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy globalPolicy);
        public CustomMcpToolCallApprovalPolicy CustomPolicy { get; }
        public GlobalMcpToolCallApprovalPolicy? GlobalPolicy { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual McpToolCallApprovalPolicy JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual McpToolCallApprovalPolicy PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class McpToolCallApprovalRequestItem : ResponseItem, IJsonModel<McpToolCallApprovalRequestItem>, IPersistableModel<McpToolCallApprovalRequestItem> {
        public McpToolCallApprovalRequestItem(string id, string serverLabel, string toolName, BinaryData toolArguments);
        public string ServerLabel { get; set; }
        public BinaryData ToolArguments { get; set; }
        public string ToolName { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class McpToolCallApprovalResponseItem : ResponseItem, IJsonModel<McpToolCallApprovalResponseItem>, IPersistableModel<McpToolCallApprovalResponseItem> {
        public McpToolCallApprovalResponseItem(string approvalRequestId, bool approved);
        public string ApprovalRequestId { get; set; }
        public bool Approved { get; set; }
        public string Reason { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class McpToolCallItem : ResponseItem, IJsonModel<McpToolCallItem>, IPersistableModel<McpToolCallItem> {
        public McpToolCallItem(string serverLabel, string toolName, BinaryData toolArguments);
        public BinaryData Error { get; set; }
        public string ServerLabel { get; set; }
        public BinaryData ToolArguments { get; set; }
        public string ToolName { get; set; }
        public string ToolOutput { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct McpToolConnectorId : IEquatable<McpToolConnectorId> {
        public McpToolConnectorId(string value);
        public static McpToolConnectorId Dropbox { get; }
        public static McpToolConnectorId Gmail { get; }
        public static McpToolConnectorId GoogleCalendar { get; }
        public static McpToolConnectorId GoogleDrive { get; }
        public static McpToolConnectorId MicrosoftTeams { get; }
        public static McpToolConnectorId OutlookCalendar { get; }
        public static McpToolConnectorId OutlookEmail { get; }
        public static McpToolConnectorId SharePoint { get; }
        public readonly bool Equals(McpToolConnectorId other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(McpToolConnectorId left, McpToolConnectorId right);
        public static implicit operator McpToolConnectorId(string value);
        public static implicit operator McpToolConnectorId?(string value);
        public static bool operator !=(McpToolConnectorId left, McpToolConnectorId right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class McpToolDefinition : IJsonModel<McpToolDefinition>, IPersistableModel<McpToolDefinition> {
        public McpToolDefinition(string name, BinaryData inputSchema);
        public BinaryData Annotations { get; set; }
        public string Description { get; set; }
        public BinaryData InputSchema { get; set; }
        public string Name { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual McpToolDefinition JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual McpToolDefinition PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class McpToolDefinitionListItem : ResponseItem, IJsonModel<McpToolDefinitionListItem>, IPersistableModel<McpToolDefinitionListItem> {
        public McpToolDefinitionListItem(string serverLabel, IEnumerable<McpToolDefinition> toolDefinitions);
        public BinaryData Error { get; set; }
        public string ServerLabel { get; set; }
        public IList<McpToolDefinition> ToolDefinitions { get; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class McpToolFilter : IJsonModel<McpToolFilter>, IPersistableModel<McpToolFilter> {
        public bool? IsReadOnly { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public IList<string> ToolNames { get; }
        protected virtual McpToolFilter JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual McpToolFilter PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class MessageResponseItem : ResponseItem, IJsonModel<MessageResponseItem>, IPersistableModel<MessageResponseItem> {
        public IList<ResponseContentPart> Content { get; }
        public MessageRole Role { get; }
        public MessageStatus? Status { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum MessageRole {
        Unknown = 0,
        Assistant = 1,
        Developer = 2,
        System = 3,
        User = 4
    }
    [Experimental("OPENAI001")]
    public enum MessageStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    [Experimental("OPENAI001")]
    public class ReasoningResponseItem : ResponseItem, IJsonModel<ReasoningResponseItem>, IPersistableModel<ReasoningResponseItem> {
        public ReasoningResponseItem(IEnumerable<ReasoningSummaryPart> summaryParts);
        public ReasoningResponseItem(string summaryText);
        public string EncryptedContent { get; set; }
        public ReasoningStatus? Status { get; set; }
        public IList<ReasoningSummaryPart> SummaryParts { get; }
        public string GetSummaryText();
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum ReasoningStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    [Experimental("OPENAI001")]
    public class ReasoningSummaryPart : IJsonModel<ReasoningSummaryPart>, IPersistableModel<ReasoningSummaryPart> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static ReasoningSummaryTextPart CreateTextPart(string text);
        protected virtual ReasoningSummaryPart JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ReasoningSummaryPart PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ReasoningSummaryTextPart : ReasoningSummaryPart, IJsonModel<ReasoningSummaryTextPart>, IPersistableModel<ReasoningSummaryTextPart> {
        public ReasoningSummaryTextPart(string text);
        public string Text { get; set; }
        protected override ReasoningSummaryPart JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ReasoningSummaryPart PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ReferenceResponseItem : ResponseItem, IJsonModel<ReferenceResponseItem>, IPersistableModel<ReferenceResponseItem> {
        public ReferenceResponseItem(string id);
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ResponseContentPart : IJsonModel<ResponseContentPart>, IPersistableModel<ResponseContentPart> {
        public BinaryData InputFileBytes { get; }
        public string InputFileBytesMediaType { get; }
        public string InputFileId { get; }
        public string InputFilename { get; }
        public ResponseImageDetailLevel? InputImageDetailLevel { get; }
        public string InputImageFileId { get; }
        public ResponseContentPartKind Kind { get; }
        public IReadOnlyList<ResponseMessageAnnotation> OutputTextAnnotations { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string Refusal { get; }
        public string Text { get; }
        public static ResponseContentPart CreateInputFilePart(BinaryData fileBytes, string fileBytesMediaType, string filename);
        public static ResponseContentPart CreateInputFilePart(string fileId);
        public static ResponseContentPart CreateInputImagePart(BinaryData imageBytes, string imageBytesMediaType, ResponseImageDetailLevel? imageDetailLevel = null);
        public static ResponseContentPart CreateInputImagePart(string imageFileId, ResponseImageDetailLevel? imageDetailLevel = null);
        public static ResponseContentPart CreateInputImagePart(Uri imageUri, ResponseImageDetailLevel? imageDetailLevel = null);
        public static ResponseContentPart CreateInputTextPart(string text);
        public static ResponseContentPart CreateOutputTextPart(string text, IEnumerable<ResponseMessageAnnotation> annotations);
        public static ResponseContentPart CreateRefusalPart(string refusal);
        protected virtual ResponseContentPart JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseContentPart PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum ResponseContentPartKind {
        Unknown = 0,
        InputText = 1,
        InputImage = 2,
        InputFile = 3,
        OutputText = 4,
        Refusal = 5
    }
    [Experimental("OPENAI001")]
    public class ResponseConversationOptions : IJsonModel<ResponseConversationOptions>, IPersistableModel<ResponseConversationOptions> {
        public ResponseConversationOptions(string conversationId);
        public string ConversationId { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ResponseConversationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseConversationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ResponseDeletionResult : IJsonModel<ResponseDeletionResult>, IPersistableModel<ResponseDeletionResult> {
        public bool Deleted { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Object { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string ResponseId { get; set; }
        protected virtual ResponseDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ResponseDeletionResult(ClientResult result);
        protected virtual ResponseDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ResponseError : IJsonModel<ResponseError>, IPersistableModel<ResponseError> {
        public ResponseErrorCode Code { get; }
        public string Message { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ResponseError JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseError PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ResponseErrorCode : IEquatable<ResponseErrorCode> {
        public ResponseErrorCode(string value);
        public static ResponseErrorCode EmptyImageFile { get; }
        public static ResponseErrorCode FailedToDownloadImage { get; }
        public static ResponseErrorCode ImageContentPolicyViolation { get; }
        public static ResponseErrorCode ImageFileNotFound { get; }
        public static ResponseErrorCode ImageFileTooLarge { get; }
        public static ResponseErrorCode ImageParseError { get; }
        public static ResponseErrorCode ImageTooLarge { get; }
        public static ResponseErrorCode ImageTooSmall { get; }
        public static ResponseErrorCode InvalidBase64Image { get; }
        public static ResponseErrorCode InvalidImage { get; }
        public static ResponseErrorCode InvalidImageFormat { get; }
        public static ResponseErrorCode InvalidImageMode { get; }
        public static ResponseErrorCode InvalidImageUrl { get; }
        public static ResponseErrorCode InvalidPrompt { get; }
        public static ResponseErrorCode RateLimitExceeded { get; }
        public static ResponseErrorCode ServerError { get; }
        public static ResponseErrorCode UnsupportedImageMediaType { get; }
        public static ResponseErrorCode VectorStoreTimeout { get; }
        public readonly bool Equals(ResponseErrorCode other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ResponseErrorCode left, ResponseErrorCode right);
        public static implicit operator ResponseErrorCode(string value);
        public static implicit operator ResponseErrorCode?(string value);
        public static bool operator !=(ResponseErrorCode left, ResponseErrorCode right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ResponseImageDetailLevel : IEquatable<ResponseImageDetailLevel> {
        public ResponseImageDetailLevel(string value);
        public static ResponseImageDetailLevel Auto { get; }
        public static ResponseImageDetailLevel High { get; }
        public static ResponseImageDetailLevel Low { get; }
        public readonly bool Equals(ResponseImageDetailLevel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ResponseImageDetailLevel left, ResponseImageDetailLevel right);
        public static implicit operator ResponseImageDetailLevel(string value);
        public static implicit operator ResponseImageDetailLevel?(string value);
        public static bool operator !=(ResponseImageDetailLevel left, ResponseImageDetailLevel right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ResponseIncompleteStatusDetails : IJsonModel<ResponseIncompleteStatusDetails>, IPersistableModel<ResponseIncompleteStatusDetails> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public ResponseIncompleteStatusReason? Reason { get; }
        protected virtual ResponseIncompleteStatusDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseIncompleteStatusDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ResponseIncompleteStatusReason : IEquatable<ResponseIncompleteStatusReason> {
        public ResponseIncompleteStatusReason(string value);
        public static ResponseIncompleteStatusReason ContentFilter { get; }
        public static ResponseIncompleteStatusReason MaxOutputTokens { get; }
        public readonly bool Equals(ResponseIncompleteStatusReason other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ResponseIncompleteStatusReason left, ResponseIncompleteStatusReason right);
        public static implicit operator ResponseIncompleteStatusReason(string value);
        public static implicit operator ResponseIncompleteStatusReason?(string value);
        public static bool operator !=(ResponseIncompleteStatusReason left, ResponseIncompleteStatusReason right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ResponseInputTokenUsageDetails : IJsonModel<ResponseInputTokenUsageDetails>, IPersistableModel<ResponseInputTokenUsageDetails> {
        public int CachedTokenCount { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ResponseInputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseInputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ResponseItem : IJsonModel<ResponseItem>, IPersistableModel<ResponseItem> {
        public string Id { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static MessageResponseItem CreateAssistantMessageItem(IEnumerable<ResponseContentPart> contentParts);
        public static MessageResponseItem CreateAssistantMessageItem(string outputTextContent, IEnumerable<ResponseMessageAnnotation> annotations = null);
        [Experimental("OPENAICUA001")]
        public static ComputerCallResponseItem CreateComputerCallItem(string callId, ComputerCallAction action, IEnumerable<ComputerCallSafetyCheck> pendingSafetyChecks);
        [Experimental("OPENAICUA001")]
        public static ComputerCallOutputResponseItem CreateComputerCallOutputItem(string callId, ComputerCallOutput output);
        public static MessageResponseItem CreateDeveloperMessageItem(IEnumerable<ResponseContentPart> contentParts);
        public static MessageResponseItem CreateDeveloperMessageItem(string inputTextContent);
        public static FileSearchCallResponseItem CreateFileSearchCallItem(IEnumerable<string> queries);
        public static FunctionCallResponseItem CreateFunctionCallItem(string callId, string functionName, BinaryData functionArguments);
        public static FunctionCallOutputResponseItem CreateFunctionCallOutputItem(string callId, string functionOutput);
        public static McpToolCallApprovalRequestItem CreateMcpApprovalRequestItem(string id, string serverLabel, string name, BinaryData arguments);
        public static McpToolCallApprovalResponseItem CreateMcpApprovalResponseItem(string approvalRequestId, bool approved);
        public static McpToolCallItem CreateMcpToolCallItem(string serverLabel, string name, BinaryData arguments);
        public static McpToolDefinitionListItem CreateMcpToolDefinitionListItem(string serverLabel, IEnumerable<McpToolDefinition> toolDefinitions);
        public static ReasoningResponseItem CreateReasoningItem(IEnumerable<ReasoningSummaryPart> summaryParts);
        public static ReasoningResponseItem CreateReasoningItem(string summaryText);
        public static ReferenceResponseItem CreateReferenceItem(string id);
        public static MessageResponseItem CreateSystemMessageItem(IEnumerable<ResponseContentPart> contentParts);
        public static MessageResponseItem CreateSystemMessageItem(string inputTextContent);
        public static MessageResponseItem CreateUserMessageItem(IEnumerable<ResponseContentPart> contentParts);
        public static MessageResponseItem CreateUserMessageItem(string inputTextContent);
        public static WebSearchCallResponseItem CreateWebSearchCallItem();
        protected virtual ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ResponseItem(ClientResult result);
        protected virtual ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ResponseItemCollectionOptions : IJsonModel<ResponseItemCollectionOptions>, IPersistableModel<ResponseItemCollectionOptions> {
        public ResponseItemCollectionOptions(string responseId);
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public ResponseItemCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        public string ResponseId { get; }
        protected virtual ResponseItemCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseItemCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ResponseItemCollectionOrder : IEquatable<ResponseItemCollectionOrder> {
        public ResponseItemCollectionOrder(string value);
        public static ResponseItemCollectionOrder Ascending { get; }
        public static ResponseItemCollectionOrder Descending { get; }
        public readonly bool Equals(ResponseItemCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ResponseItemCollectionOrder left, ResponseItemCollectionOrder right);
        public static implicit operator ResponseItemCollectionOrder(string value);
        public static implicit operator ResponseItemCollectionOrder?(string value);
        public static bool operator !=(ResponseItemCollectionOrder left, ResponseItemCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ResponseItemCollectionPage : IJsonModel<ResponseItemCollectionPage>, IPersistableModel<ResponseItemCollectionPage> {
        public IList<ResponseItem> Data { get; }
        public string FirstId { get; set; }
        public bool HasMore { get; set; }
        public string LastId { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Object { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ResponseItemCollectionPage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ResponseItemCollectionPage(ClientResult result);
        protected virtual ResponseItemCollectionPage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ResponseMessageAnnotation : IJsonModel<ResponseMessageAnnotation>, IPersistableModel<ResponseMessageAnnotation> {
        public ResponseMessageAnnotationKind Kind { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual ResponseMessageAnnotation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseMessageAnnotation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum ResponseMessageAnnotationKind {
        FileCitation = 0,
        UriCitation = 1,
        FilePath = 2,
        ContainerFileCitation = 3
    }
    [Experimental("OPENAI001")]
    public class ResponseOutputTokenUsageDetails : IJsonModel<ResponseOutputTokenUsageDetails>, IPersistableModel<ResponseOutputTokenUsageDetails> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public int ReasoningTokenCount { get; }
        protected virtual ResponseOutputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseOutputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ResponseReasoningEffortLevel : IEquatable<ResponseReasoningEffortLevel> {
        public ResponseReasoningEffortLevel(string value);
        public static ResponseReasoningEffortLevel High { get; }
        public static ResponseReasoningEffortLevel Low { get; }
        public static ResponseReasoningEffortLevel Medium { get; }
        public static ResponseReasoningEffortLevel Minimal { get; }
        public readonly bool Equals(ResponseReasoningEffortLevel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ResponseReasoningEffortLevel left, ResponseReasoningEffortLevel right);
        public static implicit operator ResponseReasoningEffortLevel(string value);
        public static implicit operator ResponseReasoningEffortLevel?(string value);
        public static bool operator !=(ResponseReasoningEffortLevel left, ResponseReasoningEffortLevel right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ResponseReasoningOptions : IJsonModel<ResponseReasoningOptions>, IPersistableModel<ResponseReasoningOptions> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public ResponseReasoningEffortLevel? ReasoningEffortLevel { get; set; }
        public ResponseReasoningSummaryVerbosity? ReasoningSummaryVerbosity { get; set; }
        protected virtual ResponseReasoningOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseReasoningOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ResponseReasoningSummaryVerbosity : IEquatable<ResponseReasoningSummaryVerbosity> {
        public ResponseReasoningSummaryVerbosity(string value);
        public static ResponseReasoningSummaryVerbosity Auto { get; }
        public static ResponseReasoningSummaryVerbosity Concise { get; }
        public static ResponseReasoningSummaryVerbosity Detailed { get; }
        public readonly bool Equals(ResponseReasoningSummaryVerbosity other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ResponseReasoningSummaryVerbosity left, ResponseReasoningSummaryVerbosity right);
        public static implicit operator ResponseReasoningSummaryVerbosity(string value);
        public static implicit operator ResponseReasoningSummaryVerbosity?(string value);
        public static bool operator !=(ResponseReasoningSummaryVerbosity left, ResponseReasoningSummaryVerbosity right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class ResponseResult : IJsonModel<ResponseResult>, IPersistableModel<ResponseResult> {
        public bool? BackgroundModeEnabled { get; set; }
        public ResponseConversationOptions ConversationOptions { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string EndUserId { get; set; }
        public ResponseError Error { get; set; }
        public string Id { get; set; }
        public ResponseIncompleteStatusDetails IncompleteStatusDetails { get; set; }
        public string Instructions { get; set; }
        public int? MaxOutputTokenCount { get; set; }
        public int? MaxToolCallCount { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public string Model { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Object { get; set; }
        public IList<ResponseItem> OutputItems { get; }
        public bool ParallelToolCallsEnabled { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public string PreviousResponseId { get; set; }
        public ResponseReasoningOptions ReasoningOptions { get; set; }
        public string SafetyIdentifier { get; set; }
        public ResponseServiceTier? ServiceTier { get; set; }
        public ResponseStatus? Status { get; set; }
        public float? Temperature { get; set; }
        public ResponseTextOptions TextOptions { get; set; }
        public ResponseToolChoice ToolChoice { get; set; }
        public IList<ResponseTool> Tools { get; }
        public int? TopLogProbabilityCount { get; set; }
        public float? TopP { get; set; }
        public ResponseTruncationMode? TruncationMode { get; set; }
        public ResponseTokenUsage Usage { get; set; }
        public string GetOutputText();
        protected virtual ResponseResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator ResponseResult(ClientResult result);
        protected virtual ResponseResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ResponsesClient {
        protected ResponsesClient();
        public ResponsesClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public ResponsesClient(ApiKeyCredential credential);
        public ResponsesClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        public ResponsesClient(AuthenticationPolicy authenticationPolicy);
        protected internal ResponsesClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public ResponsesClient(string apiKey);
        [Experimental("OPENAI001")]
        public virtual Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult CancelResponse(string responseId, RequestOptions options);
        public virtual ClientResult<ResponseResult> CancelResponse(string responseId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CancelResponseAsync(string responseId, RequestOptions options);
        public virtual Task<ClientResult<ResponseResult>> CancelResponseAsync(string responseId, CancellationToken cancellationToken = default);
        public virtual ClientResult<ResponseResult> CreateResponse(CreateResponseOptions options, CancellationToken cancellationToken = default);
        public virtual ClientResult CreateResponse(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<ResponseResult> CreateResponse(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<ResponseResult>> CreateResponseAsync(CreateResponseOptions options, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CreateResponseAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<ResponseResult>> CreateResponseAsync(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(CreateResponseOptions options, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(CreateResponseOptions options, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default);
        public virtual ClientResult DeleteResponse(string responseId, RequestOptions options);
        public virtual ClientResult<ResponseDeletionResult> DeleteResponse(string responseId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteResponseAsync(string responseId, RequestOptions options);
        public virtual Task<ClientResult<ResponseDeletionResult>> DeleteResponseAsync(string responseId, CancellationToken cancellationToken = default);
        public virtual ClientResult<ResponseResult> GetResponse(GetResponseOptions options, CancellationToken cancellationToken = default);
        public virtual ClientResult GetResponse(string responseId, IEnumerable<IncludedResponseProperty> include, bool? stream, int? startingAfter, bool? includeObfuscation, RequestOptions options);
        public virtual ClientResult<ResponseResult> GetResponse(string responseId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<ResponseResult>> GetResponseAsync(GetResponseOptions options, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetResponseAsync(string responseId, IEnumerable<IncludedResponseProperty> include, bool? stream, int? startingAfter, bool? includeObfuscation, RequestOptions options);
        public virtual Task<ClientResult<ResponseResult>> GetResponseAsync(string responseId, CancellationToken cancellationToken = default);
        public virtual ClientResult<ResponseItemCollectionPage> GetResponseInputItemCollectionPage(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default);
        public virtual ClientResult GetResponseInputItemCollectionPage(string responseId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual Task<ClientResult<ResponseItemCollectionPage>> GetResponseInputItemCollectionPageAsync(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetResponseInputItemCollectionPageAsync(string responseId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual CollectionResult<ResponseItem> GetResponseInputItems(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ResponseItem> GetResponseInputItems(string responseId, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<ResponseItem> GetResponseInputItemsAsync(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<ResponseItem> GetResponseInputItemsAsync(string responseId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingResponseUpdate> GetResponseStreaming(GetResponseOptions options, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingResponseUpdate> GetResponseStreaming(string responseId, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(GetResponseOptions options, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(string responseId, CancellationToken cancellationToken = default);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ResponseServiceTier : IEquatable<ResponseServiceTier> {
        public ResponseServiceTier(string value);
        public static ResponseServiceTier Auto { get; }
        public static ResponseServiceTier Default { get; }
        public static ResponseServiceTier Flex { get; }
        public static ResponseServiceTier Scale { get; }
        public readonly bool Equals(ResponseServiceTier other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ResponseServiceTier left, ResponseServiceTier right);
        public static implicit operator ResponseServiceTier(string value);
        public static implicit operator ResponseServiceTier?(string value);
        public static bool operator !=(ResponseServiceTier left, ResponseServiceTier right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public enum ResponseStatus {
        InProgress = 0,
        Completed = 1,
        Cancelled = 2,
        Queued = 3,
        Incomplete = 4,
        Failed = 5
    }
    [Experimental("OPENAI001")]
    public class ResponseTextFormat : IJsonModel<ResponseTextFormat>, IPersistableModel<ResponseTextFormat> {
        public ResponseTextFormatKind Kind { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static ResponseTextFormat CreateJsonObjectFormat();
        public static ResponseTextFormat CreateJsonSchemaFormat(string jsonSchemaFormatName, BinaryData jsonSchema, string jsonSchemaFormatDescription = null, bool? jsonSchemaIsStrict = null);
        public static ResponseTextFormat CreateTextFormat();
        protected virtual ResponseTextFormat JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseTextFormat PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum ResponseTextFormatKind {
        Unknown = 0,
        Text = 1,
        JsonObject = 2,
        JsonSchema = 3
    }
    [Experimental("OPENAI001")]
    public class ResponseTextOptions : IJsonModel<ResponseTextOptions>, IPersistableModel<ResponseTextOptions> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public ResponseTextFormat TextFormat { get; set; }
        protected virtual ResponseTextOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseTextOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ResponseTokenUsage : IJsonModel<ResponseTokenUsage>, IPersistableModel<ResponseTokenUsage> {
        public int InputTokenCount { get; }
        public ResponseInputTokenUsageDetails InputTokenDetails { get; }
        public int OutputTokenCount { get; }
        public ResponseOutputTokenUsageDetails OutputTokenDetails { get; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public int TotalTokenCount { get; }
        protected virtual ResponseTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ResponseTool : IJsonModel<ResponseTool>, IPersistableModel<ResponseTool> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static CodeInterpreterTool CreateCodeInterpreterTool(CodeInterpreterToolContainer container);
        [Experimental("OPENAICUA001")]
        public static ComputerTool CreateComputerTool(ComputerToolEnvironment environment, int displayWidth, int displayHeight);
        public static FileSearchTool CreateFileSearchTool(IEnumerable<string> vectorStoreIds, int? maxResultCount = null, FileSearchToolRankingOptions rankingOptions = null, BinaryData filters = null);
        public static FunctionTool CreateFunctionTool(string functionName, BinaryData functionParameters, bool? strictModeEnabled, string functionDescription = null);
        public static ImageGenerationTool CreateImageGenerationTool(string model, ImageGenerationToolQuality? quality = null, ImageGenerationToolSize? size = null, ImageGenerationToolOutputFileFormat? outputFileFormat = null, int? outputCompressionFactor = null, ImageGenerationToolModerationLevel? moderationLevel = null, ImageGenerationToolBackground? background = null, ImageGenerationToolInputFidelity? inputFidelity = null, ImageGenerationToolInputImageMask inputImageMask = null, int? partialImageCount = null);
        public static McpTool CreateMcpTool(string serverLabel, McpToolConnectorId connectorId, string authorizationToken = null, string serverDescription = null, IDictionary<string, string> headers = null, McpToolFilter allowedTools = null, McpToolCallApprovalPolicy toolCallApprovalPolicy = null);
        public static McpTool CreateMcpTool(string serverLabel, Uri serverUri, string authorizationToken = null, string serverDescription = null, IDictionary<string, string> headers = null, McpToolFilter allowedTools = null, McpToolCallApprovalPolicy toolCallApprovalPolicy = null);
        public static WebSearchPreviewTool CreateWebSearchPreviewTool(WebSearchToolLocation userLocation = null, WebSearchToolContextSize? searchContextSize = null);
        public static WebSearchTool CreateWebSearchTool(WebSearchToolLocation userLocation = null, WebSearchToolContextSize? searchContextSize = null, WebSearchToolFilters filters = null);
        protected virtual ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class ResponseToolChoice : IJsonModel<ResponseToolChoice>, IPersistableModel<ResponseToolChoice> {
        public string FunctionName { get; }
        public ResponseToolChoiceKind Kind { get; }
        public static ResponseToolChoice CreateAutoChoice();
        [Experimental("OPENAICUA001")]
        public static ResponseToolChoice CreateComputerChoice();
        public static ResponseToolChoice CreateFileSearchChoice();
        public static ResponseToolChoice CreateFunctionChoice(string functionName);
        public static ResponseToolChoice CreateNoneChoice();
        public static ResponseToolChoice CreateRequiredChoice();
        public static ResponseToolChoice CreateWebSearchChoice();
    }
    [Experimental("OPENAI001")]
    public enum ResponseToolChoiceKind {
        Unknown = 0,
        Auto = 1,
        None = 2,
        Required = 3,
        Function = 4,
        FileSearch = 5,
        WebSearch = 6,
        Computer = 7
    }
    [Experimental("OPENAI001")]
    public readonly partial struct ResponseTruncationMode : IEquatable<ResponseTruncationMode> {
        public ResponseTruncationMode(string value);
        public static ResponseTruncationMode Auto { get; }
        public static ResponseTruncationMode Disabled { get; }
        public readonly bool Equals(ResponseTruncationMode other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ResponseTruncationMode left, ResponseTruncationMode right);
        public static implicit operator ResponseTruncationMode(string value);
        public static implicit operator ResponseTruncationMode?(string value);
        public static bool operator !=(ResponseTruncationMode left, ResponseTruncationMode right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseCodeInterpreterCallCodeDeltaUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseCodeInterpreterCallCodeDeltaUpdate>, IPersistableModel<StreamingResponseCodeInterpreterCallCodeDeltaUpdate> {
        public StreamingResponseCodeInterpreterCallCodeDeltaUpdate();
        public string Delta { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseCodeInterpreterCallCodeDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseCodeInterpreterCallCodeDoneUpdate>, IPersistableModel<StreamingResponseCodeInterpreterCallCodeDoneUpdate> {
        public StreamingResponseCodeInterpreterCallCodeDoneUpdate();
        public string Code { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseCodeInterpreterCallCompletedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseCodeInterpreterCallCompletedUpdate>, IPersistableModel<StreamingResponseCodeInterpreterCallCompletedUpdate> {
        public StreamingResponseCodeInterpreterCallCompletedUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseCodeInterpreterCallInProgressUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseCodeInterpreterCallInProgressUpdate>, IPersistableModel<StreamingResponseCodeInterpreterCallInProgressUpdate> {
        public StreamingResponseCodeInterpreterCallInProgressUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseCodeInterpreterCallInterpretingUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseCodeInterpreterCallInterpretingUpdate>, IPersistableModel<StreamingResponseCodeInterpreterCallInterpretingUpdate> {
        public StreamingResponseCodeInterpreterCallInterpretingUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseCompletedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseCompletedUpdate>, IPersistableModel<StreamingResponseCompletedUpdate> {
        public StreamingResponseCompletedUpdate();
        public ResponseResult Response { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseContentPartAddedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseContentPartAddedUpdate>, IPersistableModel<StreamingResponseContentPartAddedUpdate> {
        public StreamingResponseContentPartAddedUpdate();
        public int ContentIndex { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public ResponseContentPart Part { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseContentPartDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseContentPartDoneUpdate>, IPersistableModel<StreamingResponseContentPartDoneUpdate> {
        public StreamingResponseContentPartDoneUpdate();
        public int ContentIndex { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public ResponseContentPart Part { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseCreatedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseCreatedUpdate>, IPersistableModel<StreamingResponseCreatedUpdate> {
        public StreamingResponseCreatedUpdate();
        public ResponseResult Response { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseErrorUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseErrorUpdate>, IPersistableModel<StreamingResponseErrorUpdate> {
        public StreamingResponseErrorUpdate();
        public string Code { get; set; }
        public string Message { get; set; }
        public string Param { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseFailedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFailedUpdate>, IPersistableModel<StreamingResponseFailedUpdate> {
        public StreamingResponseFailedUpdate();
        public ResponseResult Response { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseFileSearchCallCompletedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFileSearchCallCompletedUpdate>, IPersistableModel<StreamingResponseFileSearchCallCompletedUpdate> {
        public StreamingResponseFileSearchCallCompletedUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseFileSearchCallInProgressUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFileSearchCallInProgressUpdate>, IPersistableModel<StreamingResponseFileSearchCallInProgressUpdate> {
        public StreamingResponseFileSearchCallInProgressUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseFileSearchCallSearchingUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFileSearchCallSearchingUpdate>, IPersistableModel<StreamingResponseFileSearchCallSearchingUpdate> {
        public StreamingResponseFileSearchCallSearchingUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseFunctionCallArgumentsDeltaUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFunctionCallArgumentsDeltaUpdate>, IPersistableModel<StreamingResponseFunctionCallArgumentsDeltaUpdate> {
        public StreamingResponseFunctionCallArgumentsDeltaUpdate();
        public BinaryData Delta { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseFunctionCallArgumentsDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFunctionCallArgumentsDoneUpdate>, IPersistableModel<StreamingResponseFunctionCallArgumentsDoneUpdate> {
        public StreamingResponseFunctionCallArgumentsDoneUpdate();
        public BinaryData FunctionArguments { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseImageGenerationCallCompletedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseImageGenerationCallCompletedUpdate>, IPersistableModel<StreamingResponseImageGenerationCallCompletedUpdate> {
        public StreamingResponseImageGenerationCallCompletedUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseImageGenerationCallGeneratingUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseImageGenerationCallGeneratingUpdate>, IPersistableModel<StreamingResponseImageGenerationCallGeneratingUpdate> {
        public StreamingResponseImageGenerationCallGeneratingUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseImageGenerationCallInProgressUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseImageGenerationCallInProgressUpdate>, IPersistableModel<StreamingResponseImageGenerationCallInProgressUpdate> {
        public StreamingResponseImageGenerationCallInProgressUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseImageGenerationCallPartialImageUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseImageGenerationCallPartialImageUpdate>, IPersistableModel<StreamingResponseImageGenerationCallPartialImageUpdate> {
        public StreamingResponseImageGenerationCallPartialImageUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public BinaryData PartialImageBytes { get; set; }
        public int PartialImageIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseIncompleteUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseIncompleteUpdate>, IPersistableModel<StreamingResponseIncompleteUpdate> {
        public StreamingResponseIncompleteUpdate();
        public ResponseResult Response { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseInProgressUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseInProgressUpdate>, IPersistableModel<StreamingResponseInProgressUpdate> {
        public StreamingResponseInProgressUpdate();
        public ResponseResult Response { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseMcpCallArgumentsDeltaUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseMcpCallArgumentsDeltaUpdate>, IPersistableModel<StreamingResponseMcpCallArgumentsDeltaUpdate> {
        public StreamingResponseMcpCallArgumentsDeltaUpdate();
        public BinaryData Delta { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseMcpCallArgumentsDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseMcpCallArgumentsDoneUpdate>, IPersistableModel<StreamingResponseMcpCallArgumentsDoneUpdate> {
        public StreamingResponseMcpCallArgumentsDoneUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public BinaryData ToolArguments { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseMcpCallCompletedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseMcpCallCompletedUpdate>, IPersistableModel<StreamingResponseMcpCallCompletedUpdate> {
        public StreamingResponseMcpCallCompletedUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseMcpCallFailedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseMcpCallFailedUpdate>, IPersistableModel<StreamingResponseMcpCallFailedUpdate> {
        public StreamingResponseMcpCallFailedUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseMcpCallInProgressUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseMcpCallInProgressUpdate>, IPersistableModel<StreamingResponseMcpCallInProgressUpdate> {
        public StreamingResponseMcpCallInProgressUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseMcpListToolsCompletedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseMcpListToolsCompletedUpdate>, IPersistableModel<StreamingResponseMcpListToolsCompletedUpdate> {
        public StreamingResponseMcpListToolsCompletedUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseMcpListToolsFailedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseMcpListToolsFailedUpdate>, IPersistableModel<StreamingResponseMcpListToolsFailedUpdate> {
        public StreamingResponseMcpListToolsFailedUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseMcpListToolsInProgressUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseMcpListToolsInProgressUpdate>, IPersistableModel<StreamingResponseMcpListToolsInProgressUpdate> {
        public StreamingResponseMcpListToolsInProgressUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseOutputItemAddedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseOutputItemAddedUpdate>, IPersistableModel<StreamingResponseOutputItemAddedUpdate> {
        public StreamingResponseOutputItemAddedUpdate();
        public ResponseItem Item { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseOutputItemDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseOutputItemDoneUpdate>, IPersistableModel<StreamingResponseOutputItemDoneUpdate> {
        public StreamingResponseOutputItemDoneUpdate();
        public ResponseItem Item { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseOutputTextDeltaUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseOutputTextDeltaUpdate>, IPersistableModel<StreamingResponseOutputTextDeltaUpdate> {
        public StreamingResponseOutputTextDeltaUpdate();
        public int ContentIndex { get; set; }
        public string Delta { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseOutputTextDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseOutputTextDoneUpdate>, IPersistableModel<StreamingResponseOutputTextDoneUpdate> {
        public StreamingResponseOutputTextDoneUpdate();
        public int ContentIndex { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public string Text { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseQueuedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseQueuedUpdate>, IPersistableModel<StreamingResponseQueuedUpdate> {
        public StreamingResponseQueuedUpdate();
        public ResponseResult Response { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseReasoningSummaryPartAddedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseReasoningSummaryPartAddedUpdate>, IPersistableModel<StreamingResponseReasoningSummaryPartAddedUpdate> {
        public StreamingResponseReasoningSummaryPartAddedUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public ReasoningSummaryPart Part { get; set; }
        public int SummaryIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseReasoningSummaryPartDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseReasoningSummaryPartDoneUpdate>, IPersistableModel<StreamingResponseReasoningSummaryPartDoneUpdate> {
        public StreamingResponseReasoningSummaryPartDoneUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public ReasoningSummaryPart Part { get; set; }
        public int SummaryIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseReasoningSummaryTextDeltaUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseReasoningSummaryTextDeltaUpdate>, IPersistableModel<StreamingResponseReasoningSummaryTextDeltaUpdate> {
        public StreamingResponseReasoningSummaryTextDeltaUpdate();
        public string Delta { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public int SummaryIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseReasoningSummaryTextDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseReasoningSummaryTextDoneUpdate>, IPersistableModel<StreamingResponseReasoningSummaryTextDoneUpdate> {
        public StreamingResponseReasoningSummaryTextDoneUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public int SummaryIndex { get; set; }
        public string Text { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseReasoningTextDeltaUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseReasoningTextDeltaUpdate>, IPersistableModel<StreamingResponseReasoningTextDeltaUpdate> {
        public StreamingResponseReasoningTextDeltaUpdate();
        public int ContentIndex { get; set; }
        public string Delta { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseReasoningTextDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseReasoningTextDoneUpdate>, IPersistableModel<StreamingResponseReasoningTextDoneUpdate> {
        public StreamingResponseReasoningTextDoneUpdate();
        public int ContentIndex { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public string Text { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseRefusalDeltaUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseRefusalDeltaUpdate>, IPersistableModel<StreamingResponseRefusalDeltaUpdate> {
        public StreamingResponseRefusalDeltaUpdate();
        public int ContentIndex { get; set; }
        public string Delta { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseRefusalDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseRefusalDoneUpdate>, IPersistableModel<StreamingResponseRefusalDoneUpdate> {
        public StreamingResponseRefusalDoneUpdate();
        public int ContentIndex { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        public string Refusal { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseTextAnnotationAddedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseTextAnnotationAddedUpdate>, IPersistableModel<StreamingResponseTextAnnotationAddedUpdate> {
        public StreamingResponseTextAnnotationAddedUpdate();
        public BinaryData Annotation { get; set; }
        public int AnnotationIndex { get; set; }
        public int ContentIndex { get; set; }
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseUpdate : IJsonModel<StreamingResponseUpdate>, IPersistableModel<StreamingResponseUpdate> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public int SequenceNumber { get; set; }
        protected virtual StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseWebSearchCallCompletedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseWebSearchCallCompletedUpdate>, IPersistableModel<StreamingResponseWebSearchCallCompletedUpdate> {
        public StreamingResponseWebSearchCallCompletedUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseWebSearchCallInProgressUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseWebSearchCallInProgressUpdate>, IPersistableModel<StreamingResponseWebSearchCallInProgressUpdate> {
        public StreamingResponseWebSearchCallInProgressUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StreamingResponseWebSearchCallSearchingUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseWebSearchCallSearchingUpdate>, IPersistableModel<StreamingResponseWebSearchCallSearchingUpdate> {
        public StreamingResponseWebSearchCallSearchingUpdate();
        public string ItemId { get; set; }
        public int OutputIndex { get; set; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class UriCitationMessageAnnotation : ResponseMessageAnnotation, IJsonModel<UriCitationMessageAnnotation>, IPersistableModel<UriCitationMessageAnnotation> {
        public UriCitationMessageAnnotation(Uri uri, int startIndex, int endIndex, string title);
        public int EndIndex { get; set; }
        public int StartIndex { get; set; }
        public string Title { get; set; }
        public Uri Uri { get; set; }
        protected override ResponseMessageAnnotation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseMessageAnnotation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class WebSearchCallResponseItem : ResponseItem, IJsonModel<WebSearchCallResponseItem>, IPersistableModel<WebSearchCallResponseItem> {
        public WebSearchCallResponseItem();
        public WebSearchCallStatus? Status { get; set; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum WebSearchCallStatus {
        InProgress = 0,
        Searching = 1,
        Completed = 2,
        Failed = 3
    }
    [Experimental("OPENAI001")]
    public class WebSearchPreviewTool : ResponseTool, IJsonModel<WebSearchPreviewTool>, IPersistableModel<WebSearchPreviewTool> {
        public WebSearchPreviewTool();
        public WebSearchToolContextSize? SearchContextSize { get; set; }
        public WebSearchToolLocation UserLocation { get; set; }
        protected override ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class WebSearchTool : ResponseTool, IJsonModel<WebSearchTool>, IPersistableModel<WebSearchTool> {
        public WebSearchTool();
        public WebSearchToolFilters Filters { get; set; }
        public WebSearchToolContextSize? SearchContextSize { get; set; }
        public WebSearchToolLocation UserLocation { get; set; }
        protected override ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class WebSearchToolApproximateLocation : WebSearchToolLocation, IJsonModel<WebSearchToolApproximateLocation>, IPersistableModel<WebSearchToolApproximateLocation> {
        public WebSearchToolApproximateLocation();
        public string City { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Timezone { get; set; }
        protected override WebSearchToolLocation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override WebSearchToolLocation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct WebSearchToolContextSize : IEquatable<WebSearchToolContextSize> {
        public WebSearchToolContextSize(string value);
        public static WebSearchToolContextSize High { get; }
        public static WebSearchToolContextSize Low { get; }
        public static WebSearchToolContextSize Medium { get; }
        public readonly bool Equals(WebSearchToolContextSize other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(WebSearchToolContextSize left, WebSearchToolContextSize right);
        public static implicit operator WebSearchToolContextSize(string value);
        public static implicit operator WebSearchToolContextSize?(string value);
        public static bool operator !=(WebSearchToolContextSize left, WebSearchToolContextSize right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class WebSearchToolFilters : IJsonModel<WebSearchToolFilters>, IPersistableModel<WebSearchToolFilters> {
        public IList<string> AllowedDomains { get; set; }
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        protected virtual WebSearchToolFilters JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual WebSearchToolFilters PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class WebSearchToolLocation : IJsonModel<WebSearchToolLocation>, IPersistableModel<WebSearchToolLocation> {
        [Serialization.JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch { get; }
        public static WebSearchToolApproximateLocation CreateApproximateLocation(string country = null, string region = null, string city = null, string timezone = null);
        protected virtual WebSearchToolLocation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual WebSearchToolLocation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
}
namespace OpenAI.VectorStores {
    [Experimental("OPENAI001")]
    public abstract class FileChunkingStrategy : IJsonModel<FileChunkingStrategy>, IPersistableModel<FileChunkingStrategy> {
        public static FileChunkingStrategy Auto { get; }
        public static FileChunkingStrategy Unknown { get; }
        public static FileChunkingStrategy CreateStaticStrategy(int maxTokensPerChunk, int overlappingTokenCount);
        protected virtual FileChunkingStrategy JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileChunkingStrategy PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class FileFromStoreRemovalResult : IJsonModel<FileFromStoreRemovalResult>, IPersistableModel<FileFromStoreRemovalResult> {
        public string FileId { get; }
        public bool Removed { get; }
        protected virtual FileFromStoreRemovalResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator FileFromStoreRemovalResult(ClientResult result);
        protected virtual FileFromStoreRemovalResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class StaticFileChunkingStrategy : FileChunkingStrategy, IJsonModel<StaticFileChunkingStrategy>, IPersistableModel<StaticFileChunkingStrategy> {
        public StaticFileChunkingStrategy(int maxTokensPerChunk, int overlappingTokenCount);
        public int MaxTokensPerChunk { get; }
        public int OverlappingTokenCount { get; }
        protected override FileChunkingStrategy JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override FileChunkingStrategy PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual VectorStore JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator VectorStore(ClientResult result);
        protected virtual VectorStore PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreClient {
        protected VectorStoreClient();
        public VectorStoreClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public VectorStoreClient(ApiKeyCredential credential);
        public VectorStoreClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        public VectorStoreClient(AuthenticationPolicy authenticationPolicy);
        protected internal VectorStoreClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public VectorStoreClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult AddFileBatchToVectorStore(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<VectorStoreFileBatch> AddFileBatchToVectorStore(string vectorStoreId, IEnumerable<string> fileIds, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> AddFileBatchToVectorStoreAsync(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<VectorStoreFileBatch>> AddFileBatchToVectorStoreAsync(string vectorStoreId, IEnumerable<string> fileIds, CancellationToken cancellationToken = default);
        public virtual ClientResult AddFileToVectorStore(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<VectorStoreFile> AddFileToVectorStore(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> AddFileToVectorStoreAsync(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<VectorStoreFile>> AddFileToVectorStoreAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual ClientResult CancelVectorStoreFileBatch(string vectorStoreId, string batchId, RequestOptions options);
        public virtual ClientResult<VectorStoreFileBatch> CancelVectorStoreFileBatch(string vectorStoreId, string batchId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CancelVectorStoreFileBatchAsync(string vectorStoreId, string batchId, RequestOptions options);
        public virtual Task<ClientResult<VectorStoreFileBatch>> CancelVectorStoreFileBatchAsync(string vectorStoreId, string batchId, CancellationToken cancellationToken = default);
        public virtual ClientResult<VectorStore> CreateVectorStore(VectorStoreCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult CreateVectorStore(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<VectorStore>> CreateVectorStoreAsync(VectorStoreCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CreateVectorStoreAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult DeleteVectorStore(string vectorStoreId, RequestOptions options);
        public virtual ClientResult<VectorStoreDeletionResult> DeleteVectorStore(string vectorStoreId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteVectorStoreAsync(string vectorStoreId, RequestOptions options);
        public virtual Task<ClientResult<VectorStoreDeletionResult>> DeleteVectorStoreAsync(string vectorStoreId, CancellationToken cancellationToken = default);
        public virtual ClientResult GetVectorStore(string vectorStoreId, RequestOptions options);
        public virtual ClientResult<VectorStore> GetVectorStore(string vectorStoreId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetVectorStoreAsync(string vectorStoreId, RequestOptions options);
        public virtual Task<ClientResult<VectorStore>> GetVectorStoreAsync(string vectorStoreId, CancellationToken cancellationToken = default);
        public virtual ClientResult GetVectorStoreFile(string vectorStoreId, string fileId, RequestOptions options);
        public virtual ClientResult<VectorStoreFile> GetVectorStoreFile(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetVectorStoreFileAsync(string vectorStoreId, string fileId, RequestOptions options);
        public virtual Task<ClientResult<VectorStoreFile>> GetVectorStoreFileAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual ClientResult GetVectorStoreFileBatch(string vectorStoreId, string batchId, RequestOptions options);
        public virtual ClientResult<VectorStoreFileBatch> GetVectorStoreFileBatch(string vectorStoreId, string batchId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetVectorStoreFileBatchAsync(string vectorStoreId, string batchId, RequestOptions options);
        public virtual Task<ClientResult<VectorStoreFileBatch>> GetVectorStoreFileBatchAsync(string vectorStoreId, string batchId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStoreFile> GetVectorStoreFiles(string vectorStoreId, VectorStoreFileCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetVectorStoreFiles(string vectorStoreId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual AsyncCollectionResult<VectorStoreFile> GetVectorStoreFilesAsync(string vectorStoreId, VectorStoreFileCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetVectorStoreFilesAsync(string vectorStoreId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual CollectionResult<VectorStoreFile> GetVectorStoreFilesInBatch(string vectorStoreId, string batchId, VectorStoreFileCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetVectorStoreFilesInBatch(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual AsyncCollectionResult<VectorStoreFile> GetVectorStoreFilesInBatchAsync(string vectorStoreId, string batchId, VectorStoreFileCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetVectorStoreFilesInBatchAsync(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual CollectionResult<VectorStore> GetVectorStores(VectorStoreCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetVectorStores(int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<VectorStore> GetVectorStoresAsync(VectorStoreCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetVectorStoresAsync(int? limit, string order, string after, string before, RequestOptions options);
        public virtual ClientResult<VectorStore> ModifyVectorStore(string vectorStoreId, VectorStoreModificationOptions options, CancellationToken cancellationToken = default);
        public virtual ClientResult ModifyVectorStore(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<VectorStore>> ModifyVectorStoreAsync(string vectorStoreId, VectorStoreModificationOptions options, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> ModifyVectorStoreAsync(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult RemoveFileFromVectorStore(string vectorStoreId, string fileId, RequestOptions options);
        public virtual ClientResult<FileFromStoreRemovalResult> RemoveFileFromVectorStore(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> RemoveFileFromVectorStoreAsync(string vectorStoreId, string fileId, RequestOptions options);
        public virtual Task<ClientResult<FileFromStoreRemovalResult>> RemoveFileFromVectorStoreAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual ClientResult RetrieveVectorStoreFileContent(string vectorStoreId, string fileId, RequestOptions options);
        public virtual Task<ClientResult> RetrieveVectorStoreFileContentAsync(string vectorStoreId, string fileId, RequestOptions options);
        public virtual ClientResult SearchVectorStore(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> SearchVectorStoreAsync(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult UpdateVectorStoreFileAttributes(string vectorStoreId, string fileId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<VectorStoreFile> UpdateVectorStoreFileAttributes(string vectorStoreId, string fileId, IDictionary<string, BinaryData> attributes, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> UpdateVectorStoreFileAttributesAsync(string vectorStoreId, string fileId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<VectorStoreFile>> UpdateVectorStoreFileAttributesAsync(string vectorStoreId, string fileId, IDictionary<string, BinaryData> attributes, CancellationToken cancellationToken = default);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreCollectionOptions : IJsonModel<VectorStoreCollectionOptions>, IPersistableModel<VectorStoreCollectionOptions> {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public VectorStoreCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        protected virtual VectorStoreCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public static implicit operator VectorStoreCollectionOrder?(string value);
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
        protected virtual VectorStoreCreationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreCreationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreDeletionResult : IJsonModel<VectorStoreDeletionResult>, IPersistableModel<VectorStoreDeletionResult> {
        public bool Deleted { get; }
        public string VectorStoreId { get; }
        protected virtual VectorStoreDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator VectorStoreDeletionResult(ClientResult result);
        protected virtual VectorStoreDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum VectorStoreExpirationAnchor {
        Unknown = 0,
        LastActiveAt = 1
    }
    [Experimental("OPENAI001")]
    public class VectorStoreExpirationPolicy : IJsonModel<VectorStoreExpirationPolicy>, IPersistableModel<VectorStoreExpirationPolicy> {
        public VectorStoreExpirationPolicy(VectorStoreExpirationAnchor anchor, int days);
        public VectorStoreExpirationAnchor Anchor { get; set; }
        public int Days { get; set; }
        protected virtual VectorStoreExpirationPolicy JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreExpirationPolicy PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreFile : IJsonModel<VectorStoreFile>, IPersistableModel<VectorStoreFile> {
        public IDictionary<string, BinaryData> Attributes { get; }
        public FileChunkingStrategy ChunkingStrategy { get; }
        public DateTimeOffset CreatedAt { get; }
        public string FileId { get; }
        public VectorStoreFileError LastError { get; }
        public int Size { get; }
        public VectorStoreFileStatus Status { get; }
        public string VectorStoreId { get; }
        protected virtual VectorStoreFile JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator VectorStoreFile(ClientResult result);
        protected virtual VectorStoreFile PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreFileBatch : IJsonModel<VectorStoreFileBatch>, IPersistableModel<VectorStoreFileBatch> {
        public string BatchId { get; }
        public DateTimeOffset CreatedAt { get; }
        public VectorStoreFileCounts FileCounts { get; }
        public VectorStoreFileBatchStatus Status { get; }
        public string VectorStoreId { get; }
        protected virtual VectorStoreFileBatch JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static explicit operator VectorStoreFileBatch(ClientResult result);
        protected virtual VectorStoreFileBatch PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct VectorStoreFileBatchStatus : IEquatable<VectorStoreFileBatchStatus> {
        public VectorStoreFileBatchStatus(string value);
        public static VectorStoreFileBatchStatus Cancelled { get; }
        public static VectorStoreFileBatchStatus Completed { get; }
        public static VectorStoreFileBatchStatus Failed { get; }
        public static VectorStoreFileBatchStatus InProgress { get; }
        public readonly bool Equals(VectorStoreFileBatchStatus other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(VectorStoreFileBatchStatus left, VectorStoreFileBatchStatus right);
        public static implicit operator VectorStoreFileBatchStatus(string value);
        public static implicit operator VectorStoreFileBatchStatus?(string value);
        public static bool operator !=(VectorStoreFileBatchStatus left, VectorStoreFileBatchStatus right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class VectorStoreFileCollectionOptions : IJsonModel<VectorStoreFileCollectionOptions>, IPersistableModel<VectorStoreFileCollectionOptions> {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public VectorStoreFileStatusFilter? Filter { get; set; }
        public VectorStoreFileCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
        protected virtual VectorStoreFileCollectionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreFileCollectionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct VectorStoreFileCollectionOrder : IEquatable<VectorStoreFileCollectionOrder> {
        public VectorStoreFileCollectionOrder(string value);
        public static VectorStoreFileCollectionOrder Ascending { get; }
        public static VectorStoreFileCollectionOrder Descending { get; }
        public readonly bool Equals(VectorStoreFileCollectionOrder other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(VectorStoreFileCollectionOrder left, VectorStoreFileCollectionOrder right);
        public static implicit operator VectorStoreFileCollectionOrder(string value);
        public static implicit operator VectorStoreFileCollectionOrder?(string value);
        public static bool operator !=(VectorStoreFileCollectionOrder left, VectorStoreFileCollectionOrder right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class VectorStoreFileCounts : IJsonModel<VectorStoreFileCounts>, IPersistableModel<VectorStoreFileCounts> {
        public int Cancelled { get; }
        public int Completed { get; }
        public int Failed { get; }
        public int InProgress { get; }
        public int Total { get; }
        protected virtual VectorStoreFileCounts JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreFileCounts PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public class VectorStoreFileError : IJsonModel<VectorStoreFileError>, IPersistableModel<VectorStoreFileError> {
        public VectorStoreFileErrorCode Code { get; }
        public string Message { get; }
        protected virtual VectorStoreFileError JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreFileError PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public readonly partial struct VectorStoreFileErrorCode : IEquatable<VectorStoreFileErrorCode> {
        public VectorStoreFileErrorCode(string value);
        public static VectorStoreFileErrorCode InvalidFile { get; }
        public static VectorStoreFileErrorCode ServerError { get; }
        public static VectorStoreFileErrorCode UnsupportedFile { get; }
        public readonly bool Equals(VectorStoreFileErrorCode other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(VectorStoreFileErrorCode left, VectorStoreFileErrorCode right);
        public static implicit operator VectorStoreFileErrorCode(string value);
        public static implicit operator VectorStoreFileErrorCode?(string value);
        public static bool operator !=(VectorStoreFileErrorCode left, VectorStoreFileErrorCode right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public enum VectorStoreFileStatus {
        Unknown = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,
        Failed = 4
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
        public static implicit operator VectorStoreFileStatusFilter?(string value);
        public static bool operator !=(VectorStoreFileStatusFilter left, VectorStoreFileStatusFilter right);
        public override readonly string ToString();
    }
    [Experimental("OPENAI001")]
    public class VectorStoreModificationOptions : IJsonModel<VectorStoreModificationOptions>, IPersistableModel<VectorStoreModificationOptions> {
        public VectorStoreExpirationPolicy ExpirationPolicy { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public string Name { get; set; }
        protected virtual VectorStoreModificationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreModificationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Experimental("OPENAI001")]
    public enum VectorStoreStatus {
        Unknown = 0,
        InProgress = 1,
        Completed = 2,
        Expired = 3
    }
}
namespace OpenAI.Videos {
    [Experimental("OPENAI001")]
    public class VideoClient {
        protected VideoClient();
        public VideoClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public VideoClient(ApiKeyCredential credential);
        public VideoClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options);
        public VideoClient(AuthenticationPolicy authenticationPolicy);
        protected internal VideoClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public VideoClient(string apiKey);
        [Experimental("OPENAI001")]
        public Uri Endpoint { get; }
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult CreateVideo(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult> CreateVideoAsync(BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult CreateVideoRemix(string videoId, BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult> CreateVideoRemixAsync(string videoId, BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult DeleteVideo(string videoId, RequestOptions options = null);
        public virtual Task<ClientResult> DeleteVideoAsync(string videoId, RequestOptions options = null);
        public virtual ClientResult DownloadVideo(string videoId, string variant = null, RequestOptions options = null);
        public virtual Task<ClientResult> DownloadVideoAsync(string videoId, string variant = null, RequestOptions options = null);
        public virtual ClientResult GetVideo(string videoId, RequestOptions options = null);
        public virtual Task<ClientResult> GetVideoAsync(string videoId, RequestOptions options = null);
        public virtual CollectionResult GetVideos(long? limit = null, string order = null, string after = null, RequestOptions options = null);
        public virtual AsyncCollectionResult GetVideosAsync(long? limit = null, string order = null, string after = null, RequestOptions options = null);
    }
}