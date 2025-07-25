namespace OpenAI {
    public class OpenAIClient {
        protected OpenAIClient();
        public OpenAIClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public OpenAIClient(ApiKeyCredential credential);
        protected internal OpenAIClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public OpenAIClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual AssistantClient GetAssistantClient();
        public virtual AudioClient GetAudioClient(string model);
        public virtual BatchClient GetBatchClient();
        public virtual ChatClient GetChatClient(string model);
        public virtual ContainerClient GetContainerClient();
        public virtual EmbeddingClient GetEmbeddingClient(string model);
        public virtual EvaluationClient GetEvaluationClient();
        public virtual FineTuningClient GetFineTuningClient();
        public virtual GraderClient GetGraderClient();
        public virtual ImageClient GetImageClient(string model);
        public virtual ModerationClient GetModerationClient(string model);
        public virtual OpenAIFileClient GetOpenAIFileClient();
        public virtual OpenAIModelClient GetOpenAIModelClient();
        public virtual OpenAIResponseClient GetOpenAIResponseClient(string model);
        public virtual RealtimeClient GetRealtimeClient();
        public virtual VectorStoreClient GetVectorStoreClient();
    }
    public class OpenAIClientOptions : ClientPipelineOptions {
        public Uri Endpoint { get; set; }
        public string OrganizationId { get; set; }
        public string ProjectId { get; set; }
        public string UserAgentApplicationId { get; set; }
    }
    public class OpenAIContext : ModelReaderWriterContext {
        public static OpenAIContext Default { get; }
        protected override bool TryGetTypeBuilderCore(Type type, out ModelReaderWriterTypeBuilder builder);
    }
}
namespace OpenAI.Assistants {
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
        protected virtual Assistant PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class AssistantClient {
        protected AssistantClient();
        public AssistantClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public AssistantClient(ApiKeyCredential credential);
        protected internal AssistantClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public AssistantClient(string apiKey);
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
        public virtual CollectionResult<Assistant> GetAssistants(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetAssistants(int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<Assistant> GetAssistantsAsync(AssistantCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<Assistant> GetAssistantsAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetAssistantsAsync(int? limit, string order, string after, string before, RequestOptions options);
        public virtual ClientResult GetMessage(string threadId, string messageId, RequestOptions options);
        public virtual ClientResult<ThreadMessage> GetMessage(string threadId, string messageId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetMessageAsync(string threadId, string messageId, RequestOptions options);
        public virtual Task<ClientResult<ThreadMessage>> GetMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ThreadMessage> GetMessages(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ThreadMessage> GetMessages(string threadId, MessageCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetMessages(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<ThreadMessage> GetMessagesAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<ThreadMessage> GetMessagesAsync(string threadId, MessageCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetMessagesAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual ClientResult GetRun(string threadId, string runId, RequestOptions options);
        public virtual ClientResult<ThreadRun> GetRun(string threadId, string runId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetRunAsync(string threadId, string runId, RequestOptions options);
        public virtual Task<ClientResult<ThreadRun>> GetRunAsync(string threadId, string runId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ThreadRun> GetRuns(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ThreadRun> GetRuns(string threadId, RunCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetRuns(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<ThreadRun> GetRunsAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<ThreadRun> GetRunsAsync(string threadId, RunCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetRunsAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual ClientResult GetRunStep(string threadId, string runId, string stepId, RequestOptions options);
        public virtual ClientResult<RunStep> GetRunStep(string threadId, string runId, string stepId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetRunStepAsync(string threadId, string runId, string stepId, RequestOptions options);
        public virtual Task<ClientResult<RunStep>> GetRunStepAsync(string threadId, string runId, string stepId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<RunStep> GetRunSteps(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult<RunStep> GetRunSteps(string threadId, string runId, RunStepCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetRunSteps(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<RunStep> GetRunStepsAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
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
    public class AssistantCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public AssistantCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
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
    public class AssistantDeletionResult : IJsonModel<AssistantDeletionResult>, IPersistableModel<AssistantDeletionResult> {
        public string AssistantId { get; }
        public bool Deleted { get; }
        protected virtual AssistantDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual AssistantDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class AssistantThread : IJsonModel<AssistantThread>, IPersistableModel<AssistantThread> {
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public ToolResources ToolResources { get; }
        protected virtual AssistantThread JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual AssistantThread PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class CodeInterpreterToolDefinition : ToolDefinition, IJsonModel<CodeInterpreterToolDefinition>, IPersistableModel<CodeInterpreterToolDefinition> {
        public CodeInterpreterToolDefinition();
        protected override ToolDefinition JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ToolDefinition PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class CodeInterpreterToolResources : IJsonModel<CodeInterpreterToolResources>, IPersistableModel<CodeInterpreterToolResources> {
        public IList<string> FileIds { get; }
        protected virtual CodeInterpreterToolResources JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CodeInterpreterToolResources PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class FileSearchRankingOptions : IJsonModel<FileSearchRankingOptions>, IPersistableModel<FileSearchRankingOptions> {
        public FileSearchRankingOptions(float scoreThreshold);
        public FileSearchRanker? Ranker { get; set; }
        public float ScoreThreshold { get; set; }
        protected virtual FileSearchRankingOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileSearchRankingOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class FileSearchToolDefinition : ToolDefinition, IJsonModel<FileSearchToolDefinition>, IPersistableModel<FileSearchToolDefinition> {
        public FileSearchToolDefinition();
        public int? MaxResults { get; set; }
        public FileSearchRankingOptions RankingOptions { get; set; }
        protected override ToolDefinition JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ToolDefinition PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class FileSearchToolResources : IJsonModel<FileSearchToolResources>, IPersistableModel<FileSearchToolResources> {
        public IList<VectorStoreCreationHelper> NewVectorStores { get; }
        public IList<string> VectorStoreIds { get; }
        protected virtual FileSearchToolResources JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileSearchToolResources PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class MessageCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public MessageCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
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
    public class MessageCreationAttachment : IJsonModel<MessageCreationAttachment>, IPersistableModel<MessageCreationAttachment> {
        public MessageCreationAttachment(string fileId, IEnumerable<ToolDefinition> tools);
        public string FileId { get; }
        public IReadOnlyList<ToolDefinition> Tools { get; }
        protected virtual MessageCreationAttachment JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual MessageCreationAttachment PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class MessageCreationOptions : IJsonModel<MessageCreationOptions>, IPersistableModel<MessageCreationOptions> {
        public IList<MessageCreationAttachment> Attachments { get; set; }
        public IDictionary<string, string> Metadata { get; }
        protected virtual MessageCreationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual MessageCreationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class MessageDeletionResult : IJsonModel<MessageDeletionResult>, IPersistableModel<MessageDeletionResult> {
        public bool Deleted { get; }
        public string MessageId { get; }
        protected virtual MessageDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual MessageDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class MessageFailureDetails : IJsonModel<MessageFailureDetails>, IPersistableModel<MessageFailureDetails> {
        public MessageFailureReason Reason { get; }
        protected virtual MessageFailureDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual MessageFailureDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public enum MessageImageDetail {
        Auto = 0,
        Low = 1,
        High = 2
    }
    public class MessageModificationOptions : IJsonModel<MessageModificationOptions>, IPersistableModel<MessageModificationOptions> {
        public IDictionary<string, string> Metadata { get; }
        protected virtual MessageModificationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual MessageModificationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum MessageRole {
        User = 0,
        Assistant = 1
    }
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
    public class MessageStatusUpdate : StreamingUpdate<ThreadMessage> {
    }
    public abstract class RequiredAction {
        public string FunctionArguments { get; }
        public string FunctionName { get; }
        public string ToolCallId { get; }
    }
    public class RequiredActionUpdate : RunUpdate {
        public string FunctionArguments { get; }
        public string FunctionName { get; }
        public string ToolCallId { get; }
        public ThreadRun GetThreadRun();
    }
    public class RunCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public RunCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
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
    public class RunError : IJsonModel<RunError>, IPersistableModel<RunError> {
        public RunErrorCode Code { get; }
        public string Message { get; }
        protected virtual RunError JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunError PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class RunIncompleteDetails : IJsonModel<RunIncompleteDetails>, IPersistableModel<RunIncompleteDetails> {
        public RunIncompleteReason? Reason { get; }
        protected virtual RunIncompleteDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunIncompleteDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class RunModificationOptions : IJsonModel<RunModificationOptions>, IPersistableModel<RunModificationOptions> {
        public IDictionary<string, string> Metadata { get; }
        protected virtual RunModificationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunModificationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        protected virtual RunStep PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public abstract class RunStepCodeInterpreterOutput : IJsonModel<RunStepCodeInterpreterOutput>, IPersistableModel<RunStepCodeInterpreterOutput> {
        public string ImageFileId { get; }
        public string Logs { get; }
        protected virtual RunStepCodeInterpreterOutput JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepCodeInterpreterOutput PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class RunStepCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public RunStepCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
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
    public abstract class RunStepDetails : IJsonModel<RunStepDetails>, IPersistableModel<RunStepDetails> {
        public string CreatedMessageId { get; }
        public IReadOnlyList<RunStepToolCall> ToolCalls { get; }
        protected virtual RunStepDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class RunStepError : IJsonModel<RunStepError>, IPersistableModel<RunStepError> {
        public RunStepErrorCode Code { get; }
        public string Message { get; }
        protected virtual RunStepError JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepError PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class RunStepFileSearchResultContent : IJsonModel<RunStepFileSearchResultContent>, IPersistableModel<RunStepFileSearchResultContent> {
        public RunStepFileSearchResultContentKind Kind { get; }
        public string Text { get; }
        protected virtual RunStepFileSearchResultContent JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepFileSearchResultContent PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum RunStepFileSearchResultContentKind {
        Text = 0
    }
    public enum RunStepKind {
        CreatedMessage = 0,
        ToolCall = 1
    }
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
    public class RunStepTokenUsage : IJsonModel<RunStepTokenUsage>, IPersistableModel<RunStepTokenUsage> {
        public int InputTokenCount { get; }
        public int OutputTokenCount { get; }
        public int TotalTokenCount { get; }
        protected virtual RunStepTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public enum RunStepToolCallKind {
        CodeInterpreter = 0,
        FileSearch = 1,
        Function = 2
    }
    public class RunStepUpdate : StreamingUpdate<RunStep> {
    }
    public class RunStepUpdateCodeInterpreterOutput : IJsonModel<RunStepUpdateCodeInterpreterOutput>, IPersistableModel<RunStepUpdateCodeInterpreterOutput> {
        public string ImageFileId { get; }
        public string Logs { get; }
        public int OutputIndex { get; }
        protected virtual RunStepUpdateCodeInterpreterOutput JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunStepUpdateCodeInterpreterOutput PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class RunTokenUsage : IJsonModel<RunTokenUsage>, IPersistableModel<RunTokenUsage> {
        public int InputTokenCount { get; }
        public int OutputTokenCount { get; }
        public int TotalTokenCount { get; }
        protected virtual RunTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class RunTruncationStrategy : IJsonModel<RunTruncationStrategy>, IPersistableModel<RunTruncationStrategy> {
        public static RunTruncationStrategy Auto { get; }
        public int? LastMessages { get; }
        public static RunTruncationStrategy CreateLastMessagesStrategy(int lastMessageCount);
        protected virtual RunTruncationStrategy JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunTruncationStrategy PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class RunUpdate : StreamingUpdate<ThreadRun> {
    }
    public abstract class StreamingUpdate {
        public StreamingUpdateReason UpdateKind { get; }
    }
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
    public class StreamingUpdate<T> : StreamingUpdate where T : class {
        public T Value { get; }
        public static implicit operator T(StreamingUpdate<T> update);
    }
    public class TextAnnotation {
        public int EndIndex { get; }
        public string InputFileId { get; }
        public string OutputFileId { get; }
        public int StartIndex { get; }
        public string TextToReplace { get; }
    }
    public class TextAnnotationUpdate {
        public int ContentIndex { get; }
        public int? EndIndex { get; }
        public string InputFileId { get; }
        public string OutputFileId { get; }
        public int? StartIndex { get; }
        public string TextToReplace { get; }
    }
    public class ThreadCreationOptions : IJsonModel<ThreadCreationOptions>, IPersistableModel<ThreadCreationOptions> {
        public IList<ThreadInitializationMessage> InitialMessages { get; }
        public IDictionary<string, string> Metadata { get; }
        public ToolResources ToolResources { get; set; }
        protected virtual ThreadCreationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ThreadCreationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ThreadDeletionResult : IJsonModel<ThreadDeletionResult>, IPersistableModel<ThreadDeletionResult> {
        public bool Deleted { get; }
        public string ThreadId { get; }
        protected virtual ThreadDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ThreadDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ThreadInitializationMessage : MessageCreationOptions {
        public ThreadInitializationMessage(MessageRole role, IEnumerable<MessageContent> content);
        public static implicit operator ThreadInitializationMessage(string initializationMessage);
    }
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
        protected virtual ThreadMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ThreadModificationOptions : IJsonModel<ThreadModificationOptions>, IPersistableModel<ThreadModificationOptions> {
        public IDictionary<string, string> Metadata { get; }
        public ToolResources ToolResources { get; set; }
        protected virtual ThreadModificationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ThreadModificationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        protected virtual ThreadRun PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ThreadUpdate : StreamingUpdate<AssistantThread> {
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public ToolResources ToolResources { get; }
    }
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
    public class ToolDefinition : IJsonModel<ToolDefinition>, IPersistableModel<ToolDefinition> {
        public static CodeInterpreterToolDefinition CreateCodeInterpreter();
        public static FileSearchToolDefinition CreateFileSearch(int? maxResults = null);
        public static FunctionToolDefinition CreateFunction(string name, string description = null, BinaryData parameters = null, bool? strictParameterSchemaEnabled = null);
        protected virtual ToolDefinition JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ToolDefinition PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class ToolResources : IJsonModel<ToolResources>, IPersistableModel<ToolResources> {
        public CodeInterpreterToolResources CodeInterpreter { get; set; }
        public FileSearchToolResources FileSearch { get; set; }
        protected virtual ToolResources JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ToolResources PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        public AudioClient(string model, string apiKey);
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
        public virtual CollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreaming(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreaming(string audioFilePath, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreamingAsync(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default);
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
        public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
        public IReadOnlyList<TranscribedWord> Words { get; }
        protected virtual AudioTranscription JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual AudioTranscription PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
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
        public static bool operator !=(AudioTranscriptionFormat left, AudioTranscriptionFormat right);
        public override readonly string ToString();
    }
    [Flags]
    public enum AudioTranscriptionIncludes {
        Default = 0,
        Logprobs = 1
    }
    public class AudioTranscriptionOptions : IJsonModel<AudioTranscriptionOptions>, IPersistableModel<AudioTranscriptionOptions> {
        public AudioTranscriptionIncludes Includes { get; set; }
        public string Language { get; set; }
        public string Prompt { get; set; }
        public AudioTranscriptionFormat? ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        public AudioTimestampGranularities TimestampGranularities { get; set; }
        protected virtual AudioTranscriptionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual AudioTranscriptionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class AudioTranslation : IJsonModel<AudioTranslation>, IPersistableModel<AudioTranslation> {
        public TimeSpan? Duration { get; }
        public string Language { get; }
        public IReadOnlyList<TranscribedSegment> Segments { get; }
        public string Text { get; }
        protected virtual AudioTranslation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual AudioTranslation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
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
        public static bool operator !=(AudioTranslationFormat left, AudioTranslationFormat right);
        public override readonly string ToString();
    }
    public class AudioTranslationOptions : IJsonModel<AudioTranslationOptions>, IPersistableModel<AudioTranslationOptions> {
        public string Prompt { get; set; }
        public AudioTranslationFormat? ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        protected virtual AudioTranslationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual AudioTranslationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
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
        public static bool operator !=(GeneratedSpeechFormat left, GeneratedSpeechFormat right);
        public override readonly string ToString();
    }
    public readonly partial struct GeneratedSpeechVoice : IEquatable<GeneratedSpeechVoice> {
        public GeneratedSpeechVoice(string value);
        public static GeneratedSpeechVoice Alloy { get; }
        public static GeneratedSpeechVoice Ash { get; }
        public static GeneratedSpeechVoice Ballad { get; }
        public static GeneratedSpeechVoice Coral { get; }
        public static GeneratedSpeechVoice Echo { get; }
        public static GeneratedSpeechVoice Fable { get; }
        public static GeneratedSpeechVoice Nova { get; }
        public static GeneratedSpeechVoice Onyx { get; }
        public static GeneratedSpeechVoice Sage { get; }
        public static GeneratedSpeechVoice Shimmer { get; }
        public static GeneratedSpeechVoice Verse { get; }
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
        public static AudioTranscription AudioTranscription(string language = null, TimeSpan? duration = null, string text = null, IEnumerable<TranscribedWord> words = null, IEnumerable<TranscribedSegment> segments = null, IEnumerable<AudioTokenLogProbabilityDetails> transcriptionTokenLogProbabilities = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static AudioTranscription AudioTranscription(string language, TimeSpan? duration, string text, IEnumerable<TranscribedWord> words, IEnumerable<TranscribedSegment> segments);
        public static AudioTranslation AudioTranslation(string language = null, TimeSpan? duration = null, string text = null, IEnumerable<TranscribedSegment> segments = null);
        public static TranscribedSegment TranscribedSegment(int id = 0, int seekOffset = 0, TimeSpan startTime = default, TimeSpan endTime = default, string text = null, ReadOnlyMemory<int> tokenIds = default, float temperature = 0, float averageLogProbability = 0, float compressionRatio = 0, float noSpeechProbability = 0);
        public static TranscribedWord TranscribedWord(string word = null, TimeSpan startTime = default, TimeSpan endTime = default);
    }
    public class SpeechGenerationOptions : IJsonModel<SpeechGenerationOptions>, IPersistableModel<SpeechGenerationOptions> {
        public string Instructions { get; set; }
        public GeneratedSpeechFormat? ResponseFormat { get; set; }
        public float? SpeedRatio { get; set; }
        protected virtual SpeechGenerationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual SpeechGenerationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingAudioTranscriptionTextDeltaUpdate : StreamingAudioTranscriptionUpdate, IJsonModel<StreamingAudioTranscriptionTextDeltaUpdate>, IPersistableModel<StreamingAudioTranscriptionTextDeltaUpdate> {
        public string Delta { get; }
        public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
        protected override StreamingAudioTranscriptionUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingAudioTranscriptionUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingAudioTranscriptionTextDoneUpdate : StreamingAudioTranscriptionUpdate, IJsonModel<StreamingAudioTranscriptionTextDoneUpdate>, IPersistableModel<StreamingAudioTranscriptionTextDoneUpdate> {
        public string Text { get; }
        public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
        protected override StreamingAudioTranscriptionUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingAudioTranscriptionUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingAudioTranscriptionUpdate : IJsonModel<StreamingAudioTranscriptionUpdate>, IPersistableModel<StreamingAudioTranscriptionUpdate> {
        protected virtual StreamingAudioTranscriptionUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual StreamingAudioTranscriptionUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class BatchClient {
        protected BatchClient();
        public BatchClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public BatchClient(ApiKeyCredential credential);
        protected internal BatchClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public BatchClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual CreateBatchOperation CreateBatch(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual Task<CreateBatchOperation> CreateBatchAsync(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual ClientResult GetBatch(string batchId, RequestOptions options);
        public virtual Task<ClientResult> GetBatchAsync(string batchId, RequestOptions options);
        public virtual CollectionResult GetBatches(string after, int? limit, RequestOptions options);
        public virtual AsyncCollectionResult GetBatchesAsync(string after, int? limit, RequestOptions options);
    }
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
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatAudioOptions : IJsonModel<ChatAudioOptions>, IPersistableModel<ChatAudioOptions> {
        public ChatAudioOptions(ChatOutputAudioVoice outputAudioVoice, ChatOutputAudioFormat outputAudioFormat);
        public ChatOutputAudioFormat OutputAudioFormat { get; }
        public ChatOutputAudioVoice OutputAudioVoice { get; }
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
        public ChatClient(string model, string apiKey);
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
        public virtual ClientResult DeleteChatCompletion(string completionId, RequestOptions options);
        public virtual ClientResult<ChatCompletionDeletionResult> DeleteChatCompletion(string completionId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteChatCompletionAsync(string completionId, RequestOptions options);
        public virtual Task<ClientResult<ChatCompletionDeletionResult>> DeleteChatCompletionAsync(string completionId, CancellationToken cancellationToken = default);
        public virtual ClientResult GetChatCompletion(string completionId, RequestOptions options);
        public virtual ClientResult<ChatCompletion> GetChatCompletion(string completionId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetChatCompletionAsync(string completionId, RequestOptions options);
        public virtual Task<ClientResult<ChatCompletion>> GetChatCompletionAsync(string completionId, CancellationToken cancellationToken = default);
    }
    public class ChatCompletion : IJsonModel<ChatCompletion>, IPersistableModel<ChatCompletion> {
        public IReadOnlyList<ChatMessageAnnotation> Annotations { get; }
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
        protected virtual ChatCompletion JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatCompletion PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatCompletionDeletionResult : IJsonModel<ChatCompletionDeletionResult>, IPersistableModel<ChatCompletionDeletionResult> {
        public string ChatCompletionId { get; }
        public bool Deleted { get; }
        protected virtual ChatCompletionDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatCompletionDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        public long? Seed { get; set; }
        public IList<string> StopSequences { get; }
        public bool? StoredOutputEnabled { get; set; }
        public float? Temperature { get; set; }
        public ChatToolChoice ToolChoice { get; set; }
        public IList<ChatTool> Tools { get; }
        public int? TopLogProbabilityCount { get; set; }
        public float? TopP { get; set; }
        public ChatWebSearchOptions WebSearchOptions { get; set; }
        protected virtual ChatCompletionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatCompletionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
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
        protected virtual ChatFunction JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatFunction PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Obsolete("This class is obsolete. Please use ChatToolCall instead.")]
    public class ChatFunctionCall : IJsonModel<ChatFunctionCall>, IPersistableModel<ChatFunctionCall> {
        public ChatFunctionCall(string functionName, BinaryData functionArguments);
        public BinaryData FunctionArguments { get; }
        public string FunctionName { get; }
        protected virtual ChatFunctionCall JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatFunctionCall PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Obsolete("This class is obsolete. Please use ChatToolChoice instead.")]
    public class ChatFunctionChoice : IJsonModel<ChatFunctionChoice>, IPersistableModel<ChatFunctionChoice> {
        public static ChatFunctionChoice CreateAutoChoice();
        public static ChatFunctionChoice CreateNamedChoice(string functionName);
        public static ChatFunctionChoice CreateNoneChoice();
        protected virtual ChatFunctionChoice JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatFunctionChoice PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
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
        protected virtual ChatInputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatInputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static implicit operator ChatMessage(string content);
        protected virtual ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatMessageAnnotation : IJsonModel<ChatMessageAnnotation>, IPersistableModel<ChatMessageAnnotation> {
        public int EndIndex { get; }
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
        public BinaryData FileBytes { get; }
        public string FileBytesMediaType { get; }
        public string FileId { get; }
        public string Filename { get; }
        public BinaryData ImageBytes { get; }
        public string ImageBytesMediaType { get; }
        public ChatImageDetailLevel? ImageDetailLevel { get; }
        public Uri ImageUri { get; }
        public BinaryData InputAudioBytes { get; }
        public ChatInputAudioFormat? InputAudioFormat { get; }
        public ChatMessageContentPartKind Kind { get; }
        public string Refusal { get; }
        public string Text { get; }
        public static ChatMessageContentPart CreateFilePart(BinaryData fileBytes, string fileBytesMediaType, string filename);
        public static ChatMessageContentPart CreateFilePart(string fileId);
        public static ChatMessageContentPart CreateImagePart(BinaryData imageBytes, string imageBytesMediaType, ChatImageDetailLevel? imageDetailLevel = null);
        public static ChatMessageContentPart CreateImagePart(Uri imageUri, ChatImageDetailLevel? imageDetailLevel = null);
        public static ChatMessageContentPart CreateInputAudioPart(BinaryData inputAudioBytes, ChatInputAudioFormat inputAudioFormat);
        public static ChatMessageContentPart CreateRefusalPart(string refusal);
        public static ChatMessageContentPart CreateTextPart(string text);
        protected virtual ChatMessageContentPart JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        public static implicit operator ChatMessageContentPart(string text);
        protected virtual ChatMessageContentPart PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
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
    public class ChatOutputAudio : IJsonModel<ChatOutputAudio>, IPersistableModel<ChatOutputAudio> {
        public BinaryData AudioBytes { get; }
        public DateTimeOffset ExpiresAt { get; }
        public string Id { get; }
        public string Transcript { get; }
        protected virtual ChatOutputAudio JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatOutputAudio PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        public static bool operator !=(ChatOutputAudioFormat left, ChatOutputAudioFormat right);
        public override readonly string ToString();
    }
    public class ChatOutputAudioReference : IJsonModel<ChatOutputAudioReference>, IPersistableModel<ChatOutputAudioReference> {
        public ChatOutputAudioReference(string id);
        public string Id { get; }
        protected virtual ChatOutputAudioReference JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatOutputAudioReference PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        public static bool operator !=(ChatOutputAudioVoice left, ChatOutputAudioVoice right);
        public override readonly string ToString();
    }
    public class ChatOutputPrediction : IJsonModel<ChatOutputPrediction>, IPersistableModel<ChatOutputPrediction> {
        public static ChatOutputPrediction CreateStaticContentPrediction(IEnumerable<ChatMessageContentPart> staticContentParts);
        public static ChatOutputPrediction CreateStaticContentPrediction(string staticContent);
        protected virtual ChatOutputPrediction JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatOutputPrediction PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatOutputTokenUsageDetails : IJsonModel<ChatOutputTokenUsageDetails>, IPersistableModel<ChatOutputTokenUsageDetails> {
        public int AcceptedPredictionTokenCount { get; }
        public int AudioTokenCount { get; }
        public int ReasoningTokenCount { get; }
        public int RejectedPredictionTokenCount { get; }
        protected virtual ChatOutputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatOutputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual ChatResponseFormat JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatResponseFormat PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual ChatTokenLogProbabilityDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatTokenLogProbabilityDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatTokenTopLogProbabilityDetails : IJsonModel<ChatTokenTopLogProbabilityDetails>, IPersistableModel<ChatTokenTopLogProbabilityDetails> {
        public float LogProbability { get; }
        public string Token { get; }
        public ReadOnlyMemory<byte>? Utf8Bytes { get; }
        protected virtual ChatTokenTopLogProbabilityDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatTokenTopLogProbabilityDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatTokenUsage : IJsonModel<ChatTokenUsage>, IPersistableModel<ChatTokenUsage> {
        public int InputTokenCount { get; }
        public ChatInputTokenUsageDetails InputTokenDetails { get; }
        public int OutputTokenCount { get; }
        public ChatOutputTokenUsageDetails OutputTokenDetails { get; }
        public int TotalTokenCount { get; }
        protected virtual ChatTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatTool : IJsonModel<ChatTool>, IPersistableModel<ChatTool> {
        public string FunctionDescription { get; }
        public string FunctionName { get; }
        public BinaryData FunctionParameters { get; }
        public bool? FunctionSchemaIsStrict { get; }
        public ChatToolKind Kind { get; }
        public static ChatTool CreateFunctionTool(string functionName, string functionDescription = null, BinaryData functionParameters = null, bool? functionSchemaIsStrict = null);
        protected virtual ChatTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ChatToolCall : IJsonModel<ChatToolCall>, IPersistableModel<ChatToolCall> {
        public BinaryData FunctionArguments { get; }
        public string FunctionName { get; }
        public string Id { get; set; }
        public ChatToolCallKind Kind { get; }
        public static ChatToolCall CreateFunctionToolCall(string id, string functionName, BinaryData functionArguments);
        protected virtual ChatToolCall JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatToolCall PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum ChatToolCallKind {
        Function = 0
    }
    public class ChatToolChoice : IJsonModel<ChatToolChoice>, IPersistableModel<ChatToolChoice> {
        public static ChatToolChoice CreateAutoChoice();
        public static ChatToolChoice CreateFunctionChoice(string functionName);
        public static ChatToolChoice CreateNoneChoice();
        public static ChatToolChoice CreateRequiredChoice();
        protected virtual ChatToolChoice JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatToolChoice PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum ChatToolKind {
        Function = 0
    }
    public class ChatWebSearchOptions : IJsonModel<ChatWebSearchOptions>, IPersistableModel<ChatWebSearchOptions> {
        protected virtual ChatWebSearchOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ChatWebSearchOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public static class OpenAIChatModelFactory {
        public static ChatCompletion ChatCompletion(string id = null, ChatFinishReason finishReason = ChatFinishReason.Stop, ChatMessageContent content = null, string refusal = null, IEnumerable<ChatToolCall> toolCalls = null, ChatMessageRole role = ChatMessageRole.System, ChatFunctionCall functionCall = null, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null, DateTimeOffset createdAt = default, string model = null, string systemFingerprint = null, ChatTokenUsage usage = null, ChatOutputAudio outputAudio = null, IEnumerable<ChatMessageAnnotation> messageAnnotations = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ChatCompletion ChatCompletion(string id, ChatFinishReason finishReason, ChatMessageContent content, string refusal, IEnumerable<ChatToolCall> toolCalls, ChatMessageRole role, ChatFunctionCall functionCall, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities, DateTimeOffset createdAt, string model, string systemFingerprint, ChatTokenUsage usage);
        public static ChatInputTokenUsageDetails ChatInputTokenUsageDetails(int audioTokenCount = 0, int cachedTokenCount = 0);
        public static ChatMessageAnnotation ChatMessageAnnotation(int startIndex = 0, int endIndex = 0, Uri webResourceUri = null, string webResourceTitle = null);
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
        protected virtual StreamingChatCompletionUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual StreamingChatCompletionUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    [Obsolete("This class is obsolete. Please use StreamingChatToolCallUpdate instead.")]
    public class StreamingChatFunctionCallUpdate : IJsonModel<StreamingChatFunctionCallUpdate>, IPersistableModel<StreamingChatFunctionCallUpdate> {
        public BinaryData FunctionArgumentsUpdate { get; }
        public string FunctionName { get; }
        protected virtual StreamingChatFunctionCallUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual StreamingChatFunctionCallUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingChatOutputAudioUpdate : IJsonModel<StreamingChatOutputAudioUpdate>, IPersistableModel<StreamingChatOutputAudioUpdate> {
        public BinaryData AudioBytesUpdate { get; }
        public DateTimeOffset? ExpiresAt { get; }
        public string Id { get; }
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
        public string ToolCallId { get; }
        protected virtual StreamingChatToolCallUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual StreamingChatToolCallUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class SystemChatMessage : ChatMessage, IJsonModel<SystemChatMessage>, IPersistableModel<SystemChatMessage> {
        public SystemChatMessage(params ChatMessageContentPart[] contentParts);
        public SystemChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public SystemChatMessage(string content);
        public string ParticipantName { get; set; }
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ToolChatMessage : ChatMessage, IJsonModel<ToolChatMessage>, IPersistableModel<ToolChatMessage> {
        public ToolChatMessage(string toolCallId, params ChatMessageContentPart[] contentParts);
        public ToolChatMessage(string toolCallId, IEnumerable<ChatMessageContentPart> contentParts);
        public ToolChatMessage(string toolCallId, string content);
        public string ToolCallId { get; }
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class UserChatMessage : ChatMessage, IJsonModel<UserChatMessage>, IPersistableModel<UserChatMessage> {
        public UserChatMessage(params ChatMessageContentPart[] contentParts);
        public UserChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public UserChatMessage(string content);
        public string ParticipantName { get; set; }
        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
}
namespace OpenAI.Containers {
    public class ContainerClient {
        protected ContainerClient();
        public ContainerClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public ContainerClient(ApiKeyCredential credential);
        protected internal ContainerClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public ContainerClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult CreateContainer(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> CreateContainerAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult CreateContainerFile(string containerId, BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult> CreateContainerFileAsync(string containerId, BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult DeleteContainer(string containerId, RequestOptions options = null);
        public virtual Task<ClientResult> DeleteContainerAsync(string containerId, RequestOptions options = null);
        public virtual ClientResult DeleteContainerFile(string containerId, string fileId, RequestOptions options = null);
        public virtual Task<ClientResult> DeleteContainerFileAsync(string containerId, string fileId, RequestOptions options = null);
        public virtual ClientResult GetContainer(string containerId, RequestOptions options = null);
        public virtual Task<ClientResult> GetContainerAsync(string containerId, RequestOptions options = null);
        public virtual ClientResult GetContainerFile(string containerId, string fileId, RequestOptions options = null);
        public virtual Task<ClientResult> GetContainerFileAsync(string containerId, string fileId, RequestOptions options = null);
        public virtual ClientResult GetContainerFileContent(string containerId, string fileId, RequestOptions options = null);
        public virtual Task<ClientResult> GetContainerFileContentAsync(string containerId, string fileId, RequestOptions options = null);
        public virtual ClientResult GetContainerFiles(string containerId, int? limit = null, string order = null, string after = null, RequestOptions options = null);
        public virtual Task<ClientResult> GetContainerFilesAsync(string containerId, int? limit = null, string order = null, string after = null, RequestOptions options = null);
        public virtual ClientResult GetContainers(int? limit = null, string order = null, string after = null, RequestOptions options = null);
        public virtual Task<ClientResult> GetContainersAsync(int? limit = null, string order = null, string after = null, RequestOptions options = null);
    }
    public class ContainerFileListResource : IJsonModel<ContainerFileListResource>, IPersistableModel<ContainerFileListResource> {
        public IList<ContainerFileResource> Data { get; }
        public string FirstId { get; }
        public bool HasMore { get; }
        public string LastId { get; }
        public string Object { get; }
        protected virtual ContainerFileListResource JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ContainerFileListResource PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        protected virtual ContainerFileResource PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ContainerListResource : IJsonModel<ContainerListResource>, IPersistableModel<ContainerListResource> {
        public IList<ContainerResource> Data { get; }
        public string FirstId { get; }
        public bool HasMore { get; }
        public string LastId { get; }
        public string Object { get; }
        protected virtual ContainerListResource JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ContainerListResource PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ContainerResource : IJsonModel<ContainerResource>, IPersistableModel<ContainerResource> {
        public DateTimeOffset CreatedAt { get; }
        public ContainerResourceExpiresAfter ExpiresAfter { get; }
        public string Id { get; }
        public string Name { get; }
        public string Object { get; }
        public string Status { get; }
        protected virtual ContainerResource JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ContainerResource PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ContainerResourceExpiresAfter : IJsonModel<ContainerResourceExpiresAfter>, IPersistableModel<ContainerResourceExpiresAfter> {
        public string Anchor { get; }
        public int? Minutes { get; }
        protected virtual ContainerResourceExpiresAfter JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ContainerResourceExpiresAfter PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class CreateContainerBody : IJsonModel<CreateContainerBody>, IPersistableModel<CreateContainerBody> {
        public CreateContainerBodyExpiresAfter ExpiresAfter { get; }
        public IList<string> FileIds { get; }
        public string Name { get; }
        protected virtual CreateContainerBody JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CreateContainerBody PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class CreateContainerBodyExpiresAfter : IJsonModel<CreateContainerBodyExpiresAfter>, IPersistableModel<CreateContainerBodyExpiresAfter> {
        public string Anchor { get; }
        public int Minutes { get; }
        protected virtual CreateContainerBodyExpiresAfter JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CreateContainerBodyExpiresAfter PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class CreateContainerFileBody : IJsonModel<CreateContainerFileBody>, IPersistableModel<CreateContainerFileBody> {
        public BinaryData File { get; }
        public string FileId { get; }
        protected virtual CreateContainerFileBody JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual CreateContainerFileBody PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class DeleteContainerFileResponse : IJsonModel<DeleteContainerFileResponse>, IPersistableModel<DeleteContainerFileResponse> {
        public bool Deleted { get; }
        public string Id { get; }
        public string Object { get; }
        protected virtual DeleteContainerFileResponse JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual DeleteContainerFileResponse PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class DeleteContainerResponse : IJsonModel<DeleteContainerResponse>, IPersistableModel<DeleteContainerResponse> {
        public bool Deleted { get; }
        public string Id { get; }
        public string Object { get; }
        protected virtual DeleteContainerResponse JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual DeleteContainerResponse PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
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
        protected virtual EmbeddingGenerationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual EmbeddingGenerationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class EmbeddingTokenUsage : IJsonModel<EmbeddingTokenUsage>, IPersistableModel<EmbeddingTokenUsage> {
        public int InputTokenCount { get; }
        public int TotalTokenCount { get; }
        protected virtual EmbeddingTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual EmbeddingTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class OpenAIEmbedding : IJsonModel<OpenAIEmbedding>, IPersistableModel<OpenAIEmbedding> {
        public int Index { get; }
        protected virtual OpenAIEmbedding JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual OpenAIEmbedding PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
        public ReadOnlyMemory<float> ToFloats();
    }
    public class OpenAIEmbeddingCollection : ObjectModel.ReadOnlyCollection<OpenAIEmbedding>, IJsonModel<OpenAIEmbeddingCollection>, IPersistableModel<OpenAIEmbeddingCollection> {
        public string Model { get; }
        public EmbeddingTokenUsage Usage { get; }
        protected virtual OpenAIEmbeddingCollection JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual OpenAIEmbeddingCollection PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public static class OpenAIEmbeddingsModelFactory {
        public static EmbeddingTokenUsage EmbeddingTokenUsage(int inputTokenCount = 0, int totalTokenCount = 0);
        public static OpenAIEmbedding OpenAIEmbedding(int index = 0, IEnumerable<float> vector = null);
        public static OpenAIEmbeddingCollection OpenAIEmbeddingCollection(IEnumerable<OpenAIEmbedding> items = null, string model = null, EmbeddingTokenUsage usage = null);
    }
}
namespace OpenAI.Evals {
    public class EvaluationClient {
        protected EvaluationClient();
        public EvaluationClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public EvaluationClient(ApiKeyCredential credential);
        protected internal EvaluationClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public EvaluationClient(string apiKey);
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
        protected virtual FileDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
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
        public static FileUploadPurpose Evaluations { get; }
        public static FileUploadPurpose FineTune { get; }
        public static FileUploadPurpose UserData { get; }
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
        public DateTimeOffset? ExpiresAt { get; }
        public string Filename { get; }
        public string Id { get; }
        public FilePurpose Purpose { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int? SizeInBytes { get; }
        public long? SizeInBytesLong { get; }
        [Obsolete("This property is obsolete. If this is a fine-tuning training file, it may take some time to process after it has been uploaded. While the file is processing, you can still create a fine-tuning job but it will not start until the file processing has completed.")]
        public FileStatus Status { get; }
        [Obsolete("This property is obsolete. For details on why a fine-tuning training file failed validation, see the `error` field on the fine-tuning job.")]
        public string StatusDetails { get; }
        protected virtual OpenAIFile JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual OpenAIFile PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class OpenAIFileClient {
        protected OpenAIFileClient();
        public OpenAIFileClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public OpenAIFileClient(ApiKeyCredential credential);
        protected internal OpenAIFileClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public OpenAIFileClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult AddUploadPart(string uploadId, BinaryContent content, string contentType, RequestOptions options = null);
        public virtual Task<ClientResult> AddUploadPartAsync(string uploadId, BinaryContent content, string contentType, RequestOptions options = null);
        public virtual ClientResult CancelUpload(string uploadId, RequestOptions options = null);
        public virtual Task<ClientResult> CancelUploadAsync(string uploadId, RequestOptions options = null);
        public virtual ClientResult CompleteUpload(string uploadId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> CompleteUploadAsync(string uploadId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult CreateUpload(BinaryContent content, RequestOptions options = null);
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
        protected virtual OpenAIFileCollection JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual OpenAIFileCollection PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public static class OpenAIFilesModelFactory {
        public static FileDeletionResult FileDeletionResult(string fileId = null, bool deleted = false);
        public static OpenAIFileCollection OpenAIFileCollection(IEnumerable<OpenAIFile> items = null);
        public static OpenAIFile OpenAIFileInfo(string id = null, int? sizeInBytes = null, DateTimeOffset createdAt = default, string filename = null, FilePurpose purpose = FilePurpose.Assistants, FileStatus status = FileStatus.Uploaded, string statusDetails = null, DateTimeOffset? expiresAt = null, long? sizeInBytesLong = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static OpenAIFile OpenAIFileInfo(string id, int? sizeInBytes, DateTimeOffset createdAt, string filename, FilePurpose purpose, FileStatus status, string statusDetails);
    }
}
namespace OpenAI.FineTuning {
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
    public class FineTuningClient {
        protected FineTuningClient();
        public FineTuningClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public FineTuningClient(ApiKeyCredential credential);
        protected internal FineTuningClient(ClientPipeline pipeline, OpenAIClientOptions options);
        protected internal FineTuningClient(ClientPipeline pipeline, Uri endpoint);
        public FineTuningClient(string apiKey);
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
    public class FineTuningError : IJsonModel<FineTuningError>, IPersistableModel<FineTuningError> {
        public string Code { get; }
        public string InvalidParameter { get; }
        public string Message { get; }
        protected virtual FineTuningError JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuningError PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public readonly partial struct FineTuningHyperparameters : IJsonModel<FineTuningHyperparameters>, IPersistableModel<FineTuningHyperparameters>, IJsonModel<object>, IPersistableModel<object> {
        public int BatchSize { get; }
        public int EpochCount { get; }
        public float LearningRateMultiplier { get; }
    }
    public class FineTuningIntegration : IJsonModel<FineTuningIntegration>, IPersistableModel<FineTuningIntegration> {
        protected virtual FineTuningIntegration JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuningIntegration PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class FineTuningJobCollectionOptions {
        public string AfterJobId { get; set; }
        public int? PageSize { get; set; }
    }
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
        public static bool operator !=(FineTuningJobEventKind left, FineTuningJobEventKind right);
        public override readonly string ToString();
    }
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
        public static bool operator !=(FineTuningStatus left, FineTuningStatus right);
        public override readonly string ToString();
    }
    public class FineTuningTrainingMethod : IJsonModel<FineTuningTrainingMethod>, IPersistableModel<FineTuningTrainingMethod> {
        public static FineTuningTrainingMethod CreateDirectPreferenceOptimization(HyperparameterBatchSize batchSize = null, HyperparameterEpochCount epochCount = null, HyperparameterLearningRate learningRate = null, HyperparameterBetaFactor betaFactor = null);
        public static FineTuningTrainingMethod CreateSupervised(HyperparameterBatchSize batchSize = null, HyperparameterEpochCount epochCount = null, HyperparameterLearningRate learningRate = null);
        protected virtual FineTuningTrainingMethod JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FineTuningTrainingMethod PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class GetCheckpointsOptions {
        public string AfterCheckpointId { get; set; }
        public int? PageSize { get; set; }
    }
    public class GetEventsOptions {
        public string AfterEventId { get; set; }
        public int? PageSize { get; set; }
    }
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
    public class HyperparametersForSupervised : MethodHyperparameters, IJsonModel<HyperparametersForSupervised>, IPersistableModel<HyperparametersForSupervised> {
        public int BatchSize { get; }
        public int EpochCount { get; }
        public float LearningRateMultiplier { get; }
        protected virtual HyperparametersForSupervised JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual HyperparametersForSupervised PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class MethodHyperparameters {
    }
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
    [PersistableModelProxy(typeof(UnknownGrader))]
    public class Grader : IJsonModel<Grader>, IPersistableModel<Grader> {
        protected virtual Grader JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual Grader PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class GraderClient {
        protected GraderClient();
        public GraderClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public GraderClient(ApiKeyCredential credential);
        protected internal GraderClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public GraderClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult RunGrader(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> RunGraderAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult ValidateGrader(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> ValidateGraderAsync(BinaryContent content, RequestOptions options = null);
    }
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
        public static bool operator !=(GraderStringCheckOperation left, GraderStringCheckOperation right);
        public override readonly string ToString();
    }
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
        public static bool operator !=(GraderTextSimilarityEvaluationMetric left, GraderTextSimilarityEvaluationMetric right);
        public override readonly string ToString();
    }
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
        public static bool operator !=(GraderType left, GraderType right);
        public override readonly string ToString();
    }
    public class RunGraderRequest : IJsonModel<RunGraderRequest>, IPersistableModel<RunGraderRequest> {
        public BinaryData Grader { get; }
        public BinaryData Item { get; }
        public string ModelSample { get; }
        protected virtual RunGraderRequest JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunGraderRequest PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class RunGraderResponse : IJsonModel<RunGraderResponse>, IPersistableModel<RunGraderResponse> {
        public RunGraderResponseMetadata Metadata { get; }
        public BinaryData ModelGraderTokenUsagePerModel { get; }
        public float Reward { get; }
        public BinaryData SubRewards { get; }
        protected virtual RunGraderResponse JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual RunGraderResponse PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class UnknownGrader : Grader, IJsonModel<Grader>, IPersistableModel<Grader> {
        protected override Grader JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override Grader PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ValidateGraderRequest : IJsonModel<ValidateGraderRequest>, IPersistableModel<ValidateGraderRequest> {
        public BinaryData Grader { get; }
        protected virtual ValidateGraderRequest JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ValidateGraderRequest PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ValidateGraderResponse : IJsonModel<ValidateGraderResponse>, IPersistableModel<ValidateGraderResponse> {
        public BinaryData Grader { get; }
        protected virtual ValidateGraderResponse JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ValidateGraderResponse PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
}
namespace OpenAI.Images {
    public class GeneratedImage : IJsonModel<GeneratedImage>, IPersistableModel<GeneratedImage> {
        public BinaryData ImageBytes { get; }
        public Uri ImageUri { get; }
        public string RevisedPrompt { get; }
        protected virtual GeneratedImage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual GeneratedImage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        public static bool operator !=(GeneratedImageBackground left, GeneratedImageBackground right);
        public override readonly string ToString();
    }
    public class GeneratedImageCollection : ObjectModel.ReadOnlyCollection<GeneratedImage>, IJsonModel<GeneratedImageCollection>, IPersistableModel<GeneratedImageCollection> {
        public DateTimeOffset CreatedAt { get; }
        public ImageTokenUsage Usage { get; }
        protected virtual GeneratedImageCollection JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual GeneratedImageCollection PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        public static bool operator !=(GeneratedImageFormat left, GeneratedImageFormat right);
        public override readonly string ToString();
    }
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
        public static bool operator !=(GeneratedImageModerationLevel left, GeneratedImageModerationLevel right);
        public override readonly string ToString();
    }
    public readonly partial struct GeneratedImageQuality : IEquatable<GeneratedImageQuality> {
        public GeneratedImageQuality(string value);
        public static GeneratedImageQuality Auto { get; }
        public static GeneratedImageQuality High { get; }
        public static GeneratedImageQuality Low { get; }
        public static GeneratedImageQuality Medium { get; }
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
        public static readonly GeneratedImageSize W1024xH1536;
        public static readonly GeneratedImageSize W1024xH1792;
        public static readonly GeneratedImageSize W1536xH1024;
        public static readonly GeneratedImageSize W1792xH1024;
        public static readonly GeneratedImageSize W256xH256;
        public static readonly GeneratedImageSize W512xH512;
        public GeneratedImageSize(int width, int height);
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
        public string EndUserId { get; set; }
        public GeneratedImageFormat? ResponseFormat { get; set; }
        public GeneratedImageSize? Size { get; set; }
        protected virtual ImageEditOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ImageEditOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ImageGenerationOptions : IJsonModel<ImageGenerationOptions>, IPersistableModel<ImageGenerationOptions> {
        public GeneratedImageBackground? Background { get; set; }
        public string EndUserId { get; set; }
        public GeneratedImageModerationLevel? ModerationLevel { get; set; }
        public int? OutputCompressionFactor { get; set; }
        public GeneratedImageFileFormat? OutputFileFormat { get; set; }
        public GeneratedImageQuality? Quality { get; set; }
        public GeneratedImageFormat? ResponseFormat { get; set; }
        public GeneratedImageSize? Size { get; set; }
        public GeneratedImageStyle? Style { get; set; }
        protected virtual ImageGenerationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ImageGenerationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ImageInputTokenUsageDetails : IJsonModel<ImageInputTokenUsageDetails>, IPersistableModel<ImageInputTokenUsageDetails> {
        public int ImageTokenCount { get; }
        public int TextTokenCount { get; }
        protected virtual ImageInputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ImageInputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        protected virtual ImageVariationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ImageVariationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public static class OpenAIImagesModelFactory {
        public static GeneratedImage GeneratedImage(BinaryData imageBytes = null, Uri imageUri = null, string revisedPrompt = null);
        public static GeneratedImageCollection GeneratedImageCollection(DateTimeOffset createdAt = default, IEnumerable<GeneratedImage> items = null, ImageTokenUsage usage = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static GeneratedImageCollection GeneratedImageCollection(DateTimeOffset createdAt, IEnumerable<GeneratedImage> items);
    }
}
namespace OpenAI.Models {
    public class ModelDeletionResult : IJsonModel<ModelDeletionResult>, IPersistableModel<ModelDeletionResult> {
        public bool Deleted { get; }
        public string ModelId { get; }
        protected virtual ModelDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ModelDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class OpenAIModel : IJsonModel<OpenAIModel>, IPersistableModel<OpenAIModel> {
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public string OwnedBy { get; }
        protected virtual OpenAIModel JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual OpenAIModel PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class OpenAIModelClient {
        protected OpenAIModelClient();
        public OpenAIModelClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public OpenAIModelClient(ApiKeyCredential credential);
        protected internal OpenAIModelClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public OpenAIModelClient(string apiKey);
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
        protected virtual OpenAIModelCollection JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual OpenAIModelCollection PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
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
        public ModerationClient(string model, string apiKey);
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
        protected virtual ModerationResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ModerationResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ModerationResultCollection : ObjectModel.ReadOnlyCollection<ModerationResult>, IJsonModel<ModerationResultCollection>, IPersistableModel<ModerationResultCollection> {
        public string Id { get; }
        public string Model { get; }
        protected virtual ModerationResultCollection JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ModerationResultCollection PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public static class OpenAIModerationsModelFactory {
        public static ModerationCategory ModerationCategory(bool flagged = false, float score = 0);
        public static ModerationResult ModerationResult(bool flagged = false, ModerationCategory hate = null, ModerationCategory hateThreatening = null, ModerationCategory harassment = null, ModerationCategory harassmentThreatening = null, ModerationCategory selfHarm = null, ModerationCategory selfHarmIntent = null, ModerationCategory selfHarmInstructions = null, ModerationCategory sexual = null, ModerationCategory sexualMinors = null, ModerationCategory violence = null, ModerationCategory violenceGraphic = null, ModerationCategory illicit = null, ModerationCategory illicitViolent = null);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ModerationResult ModerationResult(bool flagged, ModerationCategory hate, ModerationCategory hateThreatening, ModerationCategory harassment, ModerationCategory harassmentThreatening, ModerationCategory selfHarm, ModerationCategory selfHarmIntent, ModerationCategory selfHarmInstructions, ModerationCategory sexual, ModerationCategory sexualMinors, ModerationCategory violence, ModerationCategory violenceGraphic);
        public static ModerationResultCollection ModerationResultCollection(string id = null, string model = null, IEnumerable<ModerationResult> items = null);
    }
}
namespace OpenAI.Realtime {
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
    public class ConversationInputTokenUsageDetails : IJsonModel<ConversationInputTokenUsageDetails>, IPersistableModel<ConversationInputTokenUsageDetails> {
        public int AudioTokenCount { get; }
        public int CachedTokenCount { get; }
        public int TextTokenCount { get; }
        protected virtual ConversationInputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationInputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class ConversationMaxTokensChoice : IJsonModel<ConversationMaxTokensChoice>, IPersistableModel<ConversationMaxTokensChoice> {
        public ConversationMaxTokensChoice(int numberValue);
        public int? NumericValue { get; }
        public static ConversationMaxTokensChoice CreateDefaultMaxTokensChoice();
        public static ConversationMaxTokensChoice CreateInfiniteMaxTokensChoice();
        public static ConversationMaxTokensChoice CreateNumericMaxTokensChoice(int maxTokens);
        public static implicit operator ConversationMaxTokensChoice(int maxTokens);
    }
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
    public class ConversationOutputTokenUsageDetails : IJsonModel<ConversationOutputTokenUsageDetails>, IPersistableModel<ConversationOutputTokenUsageDetails> {
        public int AudioTokenCount { get; }
        public int TextTokenCount { get; }
        protected virtual ConversationOutputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationOutputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class ConversationTool : IJsonModel<ConversationTool>, IPersistableModel<ConversationTool> {
        public ConversationToolKind Kind { get; }
        public static ConversationTool CreateFunctionTool(string name, string description = null, BinaryData parameters = null);
        protected virtual ConversationTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ConversationTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ConversationToolChoice : IJsonModel<ConversationToolChoice>, IPersistableModel<ConversationToolChoice> {
        public string FunctionName { get; }
        public ConversationToolChoiceKind Kind { get; }
        public static ConversationToolChoice CreateAutoToolChoice();
        public static ConversationToolChoice CreateFunctionToolChoice(string functionName);
        public static ConversationToolChoice CreateNoneToolChoice();
        public static ConversationToolChoice CreateRequiredToolChoice();
    }
    public enum ConversationToolChoiceKind {
        Unknown = 0,
        Auto = 1,
        None = 2,
        Required = 3,
        Function = 4
    }
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
        public static bool operator !=(ConversationVoice left, ConversationVoice right);
        public override readonly string ToString();
    }
    public class InputAudioClearedUpdate : RealtimeUpdate, IJsonModel<InputAudioClearedUpdate>, IPersistableModel<InputAudioClearedUpdate> {
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class InputAudioCommittedUpdate : RealtimeUpdate, IJsonModel<InputAudioCommittedUpdate>, IPersistableModel<InputAudioCommittedUpdate> {
        public string ItemId { get; }
        public string PreviousItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class InputAudioSpeechFinishedUpdate : RealtimeUpdate, IJsonModel<InputAudioSpeechFinishedUpdate>, IPersistableModel<InputAudioSpeechFinishedUpdate> {
        public TimeSpan AudioEndTime { get; }
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class InputAudioSpeechStartedUpdate : RealtimeUpdate, IJsonModel<InputAudioSpeechStartedUpdate>, IPersistableModel<InputAudioSpeechStartedUpdate> {
        public TimeSpan AudioStartTime { get; }
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class InputAudioTranscriptionDeltaUpdate : RealtimeUpdate, IJsonModel<InputAudioTranscriptionDeltaUpdate>, IPersistableModel<InputAudioTranscriptionDeltaUpdate> {
        public int? ContentIndex { get; }
        public string Delta { get; }
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class InputAudioTranscriptionFinishedUpdate : RealtimeUpdate, IJsonModel<InputAudioTranscriptionFinishedUpdate>, IPersistableModel<InputAudioTranscriptionFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public string Transcript { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum InputNoiseReductionKind {
        Unknown = 0,
        NearField = 1,
        FarField = 2,
        Disabled = 3
    }
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
        public static bool operator !=(InputTranscriptionModel left, InputTranscriptionModel right);
        public override readonly string ToString();
    }
    public class InputTranscriptionOptions : IJsonModel<InputTranscriptionOptions>, IPersistableModel<InputTranscriptionOptions> {
        public string Language { get; set; }
        public InputTranscriptionModel? Model { get; set; }
        public string Prompt { get; set; }
        protected virtual InputTranscriptionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual InputTranscriptionOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class ItemDeletedUpdate : RealtimeUpdate, IJsonModel<ItemDeletedUpdate>, IPersistableModel<ItemDeletedUpdate> {
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ItemRetrievedUpdate : RealtimeUpdate, IJsonModel<ItemRetrievedUpdate>, IPersistableModel<ItemRetrievedUpdate> {
        public RealtimeItem Item { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ItemTruncatedUpdate : RealtimeUpdate, IJsonModel<ItemTruncatedUpdate>, IPersistableModel<ItemTruncatedUpdate> {
        public int AudioEndMs { get; }
        public int ContentIndex { get; }
        public string ItemId { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class RateLimitsUpdate : RealtimeUpdate, IJsonModel<RateLimitsUpdate>, IPersistableModel<RateLimitsUpdate> {
        public IReadOnlyList<ConversationRateLimitDetailsItem> AllDetails { get; }
        public ConversationRateLimitDetailsItem RequestDetails { get; }
        public ConversationRateLimitDetailsItem TokenDetails { get; }
        protected override RealtimeUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override RealtimeUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        public static bool operator !=(RealtimeAudioFormat left, RealtimeAudioFormat right);
        public override readonly string ToString();
    }
    public class RealtimeClient {
        protected RealtimeClient();
        public RealtimeClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public RealtimeClient(ApiKeyCredential credential);
        protected internal RealtimeClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public ClientPipeline Pipeline { get; }
        public event EventHandler<BinaryData> OnReceivingCommand { add; remove; }
        public event EventHandler<BinaryData> OnSendingCommand { add; remove; }
        public virtual ClientResult CreateEphemeralToken(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> CreateEphemeralTokenAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult CreateEphemeralTranscriptionToken(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> CreateEphemeralTranscriptionTokenAsync(BinaryContent content, RequestOptions options = null);
        public RealtimeSession StartConversationSession(string model, CancellationToken cancellationToken = default);
        public virtual Task<RealtimeSession> StartConversationSessionAsync(string model, RequestOptions options);
        public virtual Task<RealtimeSession> StartConversationSessionAsync(string model, CancellationToken cancellationToken = default);
        public virtual Task<RealtimeSession> StartSessionAsync(string model, string intent, RequestOptions options);
        public RealtimeSession StartTranscriptionSession(CancellationToken cancellationToken = default);
        public virtual Task<RealtimeSession> StartTranscriptionSessionAsync(RequestOptions options);
        public virtual Task<RealtimeSession> StartTranscriptionSessionAsync(CancellationToken cancellationToken = default);
    }
    [Flags]
    public enum RealtimeContentModalities {
        Default = 0,
        Text = 1,
        Audio = 2
    }
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
    public class RealtimeSession : IDisposable {
        protected internal RealtimeSession(RealtimeClient parentClient, Uri endpoint, ApiKeyCredential credential);
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
        protected internal virtual void Connect(RequestOptions options);
        protected internal virtual Task ConnectAsync(RequestOptions options);
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
        public static bool operator !=(SemanticEagernessLevel left, SemanticEagernessLevel right);
        public override readonly string ToString();
    }
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
    public enum TurnDetectionKind {
        Unknown = 0,
        ServerVoiceActivityDetection = 1,
        SemanticVoiceActivityDetection = 2,
        Disabled = 3
    }
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
    public class ComputerCallAction : IJsonModel<ComputerCallAction>, IPersistableModel<ComputerCallAction> {
        public Drawing.Point? ClickCoordinates { get; }
        public ComputerCallActionMouseButton? ClickMouseButton { get; }
        public Drawing.Point? DoubleClickCoordinates { get; }
        public IList<Drawing.Point> DragPath { get; }
        public IList<string> KeyPressKeyCodes { get; }
        public ComputerCallActionKind Kind { get; }
        public Drawing.Point? MoveCoordinates { get; }
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
    public enum ComputerCallActionMouseButton {
        Left = 0,
        Right = 1,
        Wheel = 2,
        Back = 3,
        Forward = 4
    }
    public class ComputerCallOutputResponseItem : ResponseItem, IJsonModel<ComputerCallOutputResponseItem>, IPersistableModel<ComputerCallOutputResponseItem> {
        public IList<ComputerCallSafetyCheck> AcknowledgedSafetyChecks { get; }
        public string CallId { get; }
        public ComputerOutput Output { get; }
        public ComputerCallOutputStatus? Status { get; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum ComputerCallOutputStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    public class ComputerCallResponseItem : ResponseItem, IJsonModel<ComputerCallResponseItem>, IPersistableModel<ComputerCallResponseItem> {
        public ComputerCallResponseItem(string callId, ComputerCallAction action, IEnumerable<ComputerCallSafetyCheck> pendingSafetyChecks);
        public ComputerCallAction Action { get; }
        public string CallId { get; }
        public IList<ComputerCallSafetyCheck> PendingSafetyChecks { get; }
        public ComputerCallStatus? Status { get; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ComputerCallSafetyCheck : IJsonModel<ComputerCallSafetyCheck>, IPersistableModel<ComputerCallSafetyCheck> {
        public ComputerCallSafetyCheck(string id, string code, string message);
        public string Code { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
        protected virtual ComputerCallSafetyCheck JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ComputerCallSafetyCheck PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum ComputerCallStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    public class ComputerOutput : IJsonModel<ComputerOutput>, IPersistableModel<ComputerOutput> {
        public static ComputerOutput CreateScreenshotOutput(BinaryData screenshotImageBytes, string screenshotImageBytesMediaType);
        public static ComputerOutput CreateScreenshotOutput(string screenshotImageFileId);
        public static ComputerOutput CreateScreenshotOutput(Uri screenshotImageUri);
        protected virtual ComputerOutput JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ComputerOutput PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        public static bool operator !=(ComputerToolEnvironment left, ComputerToolEnvironment right);
        public override readonly string ToString();
    }
    public class FileSearchCallResponseItem : ResponseItem, IJsonModel<FileSearchCallResponseItem>, IPersistableModel<FileSearchCallResponseItem> {
        public IList<string> Queries { get; }
        public IList<FileSearchCallResult> Results { get; }
        public FileSearchCallStatus? Status { get; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class FileSearchCallResult : IJsonModel<FileSearchCallResult>, IPersistableModel<FileSearchCallResult> {
        public IReadOnlyDictionary<string, BinaryData> Attributes { get; }
        public string FileId { get; set; }
        public string Filename { get; set; }
        public float? Score { get; set; }
        public string Text { get; set; }
        protected virtual FileSearchCallResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileSearchCallResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum FileSearchCallStatus {
        InProgress = 0,
        Searching = 1,
        Completed = 2,
        Incomplete = 3,
        Failed = 4
    }
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
        public static bool operator !=(FileSearchToolRanker left, FileSearchToolRanker right);
        public override readonly string ToString();
    }
    public class FileSearchToolRankingOptions : IJsonModel<FileSearchToolRankingOptions>, IPersistableModel<FileSearchToolRankingOptions> {
        public FileSearchToolRanker? Ranker { get; set; }
        public float? ScoreThreshold { get; set; }
        protected virtual FileSearchToolRankingOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileSearchToolRankingOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class FunctionCallOutputResponseItem : ResponseItem, IJsonModel<FunctionCallOutputResponseItem>, IPersistableModel<FunctionCallOutputResponseItem> {
        public string CallId { get; }
        public string FunctionOutput { get; set; }
        public FunctionCallOutputStatus? Status { get; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum FunctionCallOutputStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    public class FunctionCallResponseItem : ResponseItem, IJsonModel<FunctionCallResponseItem>, IPersistableModel<FunctionCallResponseItem> {
        public string CallId { get; }
        public BinaryData FunctionArguments { get; set; }
        public string FunctionName { get; set; }
        public FunctionCallStatus? Status { get; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum FunctionCallStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    public class MessageResponseItem : ResponseItem, IJsonModel<MessageResponseItem>, IPersistableModel<MessageResponseItem> {
        public IList<ResponseContentPart> Content { get; }
        public MessageRole Role { get; }
        public MessageStatus? Status { get; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum MessageRole {
        Unknown = 0,
        Assistant = 1,
        Developer = 2,
        System = 3,
        User = 4
    }
    public enum MessageStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    public class OpenAIResponse : IJsonModel<OpenAIResponse>, IPersistableModel<OpenAIResponse> {
        public bool? Background { get; }
        public DateTimeOffset CreatedAt { get; }
        public string EndUserId { get; }
        public ResponseError Error { get; }
        public string Id { get; }
        public ResponseIncompleteStatusDetails IncompleteStatusDetails { get; }
        public string Instructions { get; }
        public int? MaxOutputTokenCount { get; }
        public IDictionary<string, string> Metadata { get; }
        public string Model { get; }
        public IList<ResponseItem> OutputItems { get; }
        public bool ParallelToolCallsEnabled { get; }
        public string PreviousResponseId { get; }
        public ResponseReasoningOptions ReasoningOptions { get; }
        public ResponseStatus? Status { get; }
        public float? Temperature { get; }
        public ResponseTextOptions TextOptions { get; }
        public ResponseToolChoice ToolChoice { get; }
        public IList<ResponseTool> Tools { get; }
        public float? TopP { get; }
        public ResponseTruncationMode? TruncationMode { get; }
        public ResponseTokenUsage Usage { get; }
        public string GetOutputText();
        protected virtual OpenAIResponse JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual OpenAIResponse PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class OpenAIResponseClient {
        protected OpenAIResponseClient();
        protected internal OpenAIResponseClient(ClientPipeline pipeline, string model, OpenAIClientOptions options);
        public OpenAIResponseClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public OpenAIResponseClient(string model, ApiKeyCredential credential);
        public OpenAIResponseClient(string model, string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual ClientResult CancelResponse(string responseId, RequestOptions options);
        public virtual ClientResult<OpenAIResponse> CancelResponse(string responseId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CancelResponseAsync(string responseId, RequestOptions options);
        public virtual Task<ClientResult<OpenAIResponse>> CancelResponseAsync(string responseId, CancellationToken cancellationToken = default);
        public virtual ClientResult CreateResponse(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<OpenAIResponse> CreateResponse(IEnumerable<ResponseItem> inputItems, ResponseCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult<OpenAIResponse> CreateResponse(string userInputText, ResponseCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CreateResponseAsync(BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<OpenAIResponse>> CreateResponseAsync(IEnumerable<ResponseItem> inputItems, ResponseCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<OpenAIResponse>> CreateResponseAsync(string userInputText, ResponseCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(IEnumerable<ResponseItem> inputItems, ResponseCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(string userInputText, ResponseCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(IEnumerable<ResponseItem> inputItems, ResponseCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(string userInputText, ResponseCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual ClientResult DeleteResponse(string responseId, RequestOptions options);
        public virtual ClientResult<ResponseDeletionResult> DeleteResponse(string responseId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteResponseAsync(string responseId, RequestOptions options);
        public virtual Task<ClientResult<ResponseDeletionResult>> DeleteResponseAsync(string responseId, CancellationToken cancellationToken = default);
        public virtual ClientResult GetResponse(string responseId, bool? stream, int? startingAfter, RequestOptions options);
        public virtual ClientResult<OpenAIResponse> GetResponse(string responseId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetResponseAsync(string responseId, bool? stream, int? startingAfter, RequestOptions options);
        public virtual Task<ClientResult<OpenAIResponse>> GetResponseAsync(string responseId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<ResponseItem> GetResponseInputItems(string responseId, ResponseItemCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetResponseInputItems(string responseId, int? limit, string order, string after, string before, RequestOptions options = null);
        public virtual AsyncCollectionResult<ResponseItem> GetResponseInputItemsAsync(string responseId, ResponseItemCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetResponseInputItemsAsync(string responseId, int? limit, string order, string after, string before, RequestOptions options = null);
        public virtual CollectionResult<StreamingResponseUpdate> GetResponseStreaming(string responseId, int? startingAfter = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(string responseId, int? startingAfter = null, CancellationToken cancellationToken = default);
    }
    public class ReasoningResponseItem : ResponseItem, IJsonModel<ReasoningResponseItem>, IPersistableModel<ReasoningResponseItem> {
        public ReasoningResponseItem(IEnumerable<ReasoningSummaryPart> summaryParts);
        public ReasoningResponseItem(string summaryText);
        public string EncryptedContent { get; }
        public ReasoningStatus? Status { get; }
        public IReadOnlyList<ReasoningSummaryPart> SummaryParts { get; }
        public string GetSummaryText();
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum ReasoningStatus {
        InProgress = 0,
        Completed = 1,
        Incomplete = 2
    }
    public class ReasoningSummaryPart : IJsonModel<ReasoningSummaryPart>, IPersistableModel<ReasoningSummaryPart> {
        public static ReasoningSummaryPart CreateTextPart(string text);
        protected virtual ReasoningSummaryPart JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ReasoningSummaryPart PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ReasoningSummaryTextPart : ReasoningSummaryPart, IJsonModel<ReasoningSummaryTextPart>, IPersistableModel<ReasoningSummaryTextPart> {
        public ReasoningSummaryTextPart(string text);
        public string Text { get; set; }
        protected override ReasoningSummaryPart JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ReasoningSummaryPart PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ReferenceResponseItem : ResponseItem, IJsonModel<ReferenceResponseItem>, IPersistableModel<ReferenceResponseItem> {
        public ReferenceResponseItem(string id);
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ResponseContentPart : IJsonModel<ResponseContentPart>, IPersistableModel<ResponseContentPart> {
        public BinaryData InputFileBytes { get; }
        public string InputFileBytesMediaType { get; }
        public string InputFileId { get; }
        public string InputFilename { get; }
        public ResponseImageDetailLevel? InputImageDetailLevel { get; }
        public string InputImageFileId { get; }
        public ResponseContentPartKind Kind { get; }
        public IReadOnlyList<ResponseMessageAnnotation> OutputTextAnnotations { get; }
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
    public enum ResponseContentPartKind {
        Unknown = 0,
        InputText = 1,
        InputImage = 2,
        InputFile = 3,
        OutputText = 4,
        Refusal = 5
    }
    public class ResponseCreationOptions : IJsonModel<ResponseCreationOptions>, IPersistableModel<ResponseCreationOptions> {
        public bool? Background { get; set; }
        public string EndUserId { get; set; }
        public string Instructions { get; set; }
        public int? MaxOutputTokenCount { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public bool? ParallelToolCallsEnabled { get; set; }
        public string PreviousResponseId { get; set; }
        public ResponseReasoningOptions ReasoningOptions { get; set; }
        public bool? StoredOutputEnabled { get; set; }
        public float? Temperature { get; set; }
        public ResponseTextOptions TextOptions { get; set; }
        public ResponseToolChoice ToolChoice { get; set; }
        public IList<ResponseTool> Tools { get; }
        public float? TopP { get; set; }
        public ResponseTruncationMode? TruncationMode { get; set; }
        protected virtual ResponseCreationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseCreationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ResponseDeletionResult : IJsonModel<ResponseDeletionResult>, IPersistableModel<ResponseDeletionResult> {
        public bool Deleted { get; }
        public string Id { get; }
        protected virtual ResponseDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ResponseError : IJsonModel<ResponseError>, IPersistableModel<ResponseError> {
        public ResponseErrorCode Code { get; }
        public string Message { get; }
        protected virtual ResponseError JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseError PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        public static bool operator !=(ResponseErrorCode left, ResponseErrorCode right);
        public override readonly string ToString();
    }
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
        public static bool operator !=(ResponseImageDetailLevel left, ResponseImageDetailLevel right);
        public override readonly string ToString();
    }
    public class ResponseIncompleteStatusDetails : IJsonModel<ResponseIncompleteStatusDetails>, IPersistableModel<ResponseIncompleteStatusDetails> {
        public ResponseIncompleteStatusReason? Reason { get; }
        protected virtual ResponseIncompleteStatusDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseIncompleteStatusDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        public static bool operator !=(ResponseIncompleteStatusReason left, ResponseIncompleteStatusReason right);
        public override readonly string ToString();
    }
    public class ResponseInputTokenUsageDetails : IJsonModel<ResponseInputTokenUsageDetails>, IPersistableModel<ResponseInputTokenUsageDetails> {
        public int CachedTokenCount { get; }
        protected virtual ResponseInputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseInputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ResponseItem : IJsonModel<ResponseItem>, IPersistableModel<ResponseItem> {
        public string Id { get; }
        public static MessageResponseItem CreateAssistantMessageItem(IEnumerable<ResponseContentPart> contentParts);
        public static MessageResponseItem CreateAssistantMessageItem(string outputTextContent, IEnumerable<ResponseMessageAnnotation> annotations = null);
        public static ComputerCallResponseItem CreateComputerCallItem(string callId, ComputerCallAction action, IEnumerable<ComputerCallSafetyCheck> pendingSafetyChecks);
        public static ComputerCallOutputResponseItem CreateComputerCallOutputItem(string callId, IList<ComputerCallSafetyCheck> acknowledgedSafetyChecks, BinaryData screenshotImageBytes, string screenshotImageBytesMediaType);
        public static ComputerCallOutputResponseItem CreateComputerCallOutputItem(string callId, IList<ComputerCallSafetyCheck> acknowledgedSafetyChecks, string screenshotImageFileId);
        public static ComputerCallOutputResponseItem CreateComputerCallOutputItem(string callId, IList<ComputerCallSafetyCheck> acknowledgedSafetyChecks, Uri screenshotImageUri);
        public static MessageResponseItem CreateDeveloperMessageItem(IEnumerable<ResponseContentPart> contentParts);
        public static MessageResponseItem CreateDeveloperMessageItem(string inputTextContent);
        public static FileSearchCallResponseItem CreateFileSearchCallItem(IEnumerable<string> queries, IEnumerable<FileSearchCallResult> results);
        public static FunctionCallResponseItem CreateFunctionCallItem(string callId, string functionName, BinaryData functionArguments);
        public static FunctionCallOutputResponseItem CreateFunctionCallOutputItem(string callId, string functionOutput);
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
        protected virtual ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ResponseItemCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public ResponseItemCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
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
        public static bool operator !=(ResponseItemCollectionOrder left, ResponseItemCollectionOrder right);
        public override readonly string ToString();
    }
    public class ResponseMessageAnnotation : IJsonModel<ResponseMessageAnnotation>, IPersistableModel<ResponseMessageAnnotation> {
        public string FileCitationFileId { get; }
        public int? FileCitationIndex { get; }
        public string FilePathFileId { get; }
        public int? FilePathIndex { get; }
        public ResponseMessageAnnotationKind Kind { get; }
        public int? UriCitationEndIndex { get; }
        public int? UriCitationStartIndex { get; }
        public string UriCitationTitle { get; }
        public Uri UriCitationUri { get; }
        protected virtual ResponseMessageAnnotation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseMessageAnnotation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum ResponseMessageAnnotationKind {
        FileCitation = 0,
        UriCitation = 1,
        FilePath = 2,
        ContainerFileCitation = 3
    }
    public class ResponseOutputTokenUsageDetails : IJsonModel<ResponseOutputTokenUsageDetails>, IPersistableModel<ResponseOutputTokenUsageDetails> {
        public int ReasoningTokenCount { get; }
        protected virtual ResponseOutputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseOutputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public readonly partial struct ResponseReasoningEffortLevel : IEquatable<ResponseReasoningEffortLevel> {
        public ResponseReasoningEffortLevel(string value);
        public static ResponseReasoningEffortLevel High { get; }
        public static ResponseReasoningEffortLevel Low { get; }
        public static ResponseReasoningEffortLevel Medium { get; }
        public readonly bool Equals(ResponseReasoningEffortLevel other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(ResponseReasoningEffortLevel left, ResponseReasoningEffortLevel right);
        public static implicit operator ResponseReasoningEffortLevel(string value);
        public static bool operator !=(ResponseReasoningEffortLevel left, ResponseReasoningEffortLevel right);
        public override readonly string ToString();
    }
    public class ResponseReasoningOptions : IJsonModel<ResponseReasoningOptions>, IPersistableModel<ResponseReasoningOptions> {
        public ResponseReasoningEffortLevel? ReasoningEffortLevel { get; set; }
        public ResponseReasoningSummaryVerbosity? ReasoningSummaryVerbosity { get; set; }
        protected virtual ResponseReasoningOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseReasoningOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        public static bool operator !=(ResponseReasoningSummaryVerbosity left, ResponseReasoningSummaryVerbosity right);
        public override readonly string ToString();
    }
    public enum ResponseStatus {
        InProgress = 0,
        Completed = 1,
        Cancelled = 2,
        Queued = 3,
        Incomplete = 4,
        Failed = 5
    }
    public class ResponseTextFormat : IJsonModel<ResponseTextFormat>, IPersistableModel<ResponseTextFormat> {
        public ResponseTextFormatKind Kind { get; set; }
        public static ResponseTextFormat CreateJsonObjectFormat();
        public static ResponseTextFormat CreateJsonSchemaFormat(string jsonSchemaFormatName, BinaryData jsonSchema, string jsonSchemaFormatDescription = null, bool? jsonSchemaIsStrict = null);
        public static ResponseTextFormat CreateTextFormat();
        protected virtual ResponseTextFormat JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseTextFormat PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum ResponseTextFormatKind {
        Unknown = 0,
        Text = 1,
        JsonObject = 2,
        JsonSchema = 3
    }
    public class ResponseTextOptions : IJsonModel<ResponseTextOptions>, IPersistableModel<ResponseTextOptions> {
        public ResponseTextFormat TextFormat { get; set; }
        protected virtual ResponseTextOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseTextOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ResponseTokenUsage : IJsonModel<ResponseTokenUsage>, IPersistableModel<ResponseTokenUsage> {
        public int InputTokenCount { get; }
        public ResponseInputTokenUsageDetails InputTokenDetails { get; }
        public int OutputTokenCount { get; }
        public ResponseOutputTokenUsageDetails OutputTokenDetails { get; }
        public int TotalTokenCount { get; }
        protected virtual ResponseTokenUsage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseTokenUsage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ResponseTool : IJsonModel<ResponseTool>, IPersistableModel<ResponseTool> {
        public static ResponseTool CreateComputerTool(ComputerToolEnvironment environment, int displayWidth, int displayHeight);
        public static ResponseTool CreateFileSearchTool(IEnumerable<string> vectorStoreIds, int? maxResultCount = null, FileSearchToolRankingOptions rankingOptions = null, BinaryData filters = null);
        public static ResponseTool CreateFunctionTool(string functionName, string functionDescription, BinaryData functionParameters, bool functionSchemaIsStrict = false);
        public static ResponseTool CreateWebSearchTool(WebSearchUserLocation userLocation = null, WebSearchContextSize? searchContextSize = null);
        protected virtual ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class ResponseToolChoice : IJsonModel<ResponseToolChoice>, IPersistableModel<ResponseToolChoice> {
        public string FunctionName { get; }
        public ResponseToolChoiceKind Kind { get; }
        public static ResponseToolChoice CreateAutoChoice();
        public static ResponseToolChoice CreateComputerChoice();
        public static ResponseToolChoice CreateFileSearchChoice();
        public static ResponseToolChoice CreateFunctionChoice(string functionName);
        public static ResponseToolChoice CreateNoneChoice();
        public static ResponseToolChoice CreateRequiredChoice();
        public static ResponseToolChoice CreateWebSearchChoice();
    }
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
        public static bool operator !=(ResponseTruncationMode left, ResponseTruncationMode right);
        public override readonly string ToString();
    }
    public class StreamingResponseCompletedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseCompletedUpdate>, IPersistableModel<StreamingResponseCompletedUpdate> {
        public OpenAIResponse Response { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseContentPartAddedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseContentPartAddedUpdate>, IPersistableModel<StreamingResponseContentPartAddedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public ResponseContentPart Part { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseContentPartDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseContentPartDoneUpdate>, IPersistableModel<StreamingResponseContentPartDoneUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public ResponseContentPart Part { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseCreatedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseCreatedUpdate>, IPersistableModel<StreamingResponseCreatedUpdate> {
        public OpenAIResponse Response { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseErrorUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseErrorUpdate>, IPersistableModel<StreamingResponseErrorUpdate> {
        public string Code { get; }
        public string Message { get; }
        public string Param { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseFailedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFailedUpdate>, IPersistableModel<StreamingResponseFailedUpdate> {
        public OpenAIResponse Response { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseFileSearchCallCompletedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFileSearchCallCompletedUpdate>, IPersistableModel<StreamingResponseFileSearchCallCompletedUpdate> {
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseFileSearchCallInProgressUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFileSearchCallInProgressUpdate>, IPersistableModel<StreamingResponseFileSearchCallInProgressUpdate> {
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseFileSearchCallSearchingUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFileSearchCallSearchingUpdate>, IPersistableModel<StreamingResponseFileSearchCallSearchingUpdate> {
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseFunctionCallArgumentsDeltaUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFunctionCallArgumentsDeltaUpdate>, IPersistableModel<StreamingResponseFunctionCallArgumentsDeltaUpdate> {
        public string Delta { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseFunctionCallArgumentsDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseFunctionCallArgumentsDoneUpdate>, IPersistableModel<StreamingResponseFunctionCallArgumentsDoneUpdate> {
        public string Arguments { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseIncompleteUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseIncompleteUpdate>, IPersistableModel<StreamingResponseIncompleteUpdate> {
        public OpenAIResponse Response { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseInProgressUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseInProgressUpdate>, IPersistableModel<StreamingResponseInProgressUpdate> {
        public OpenAIResponse Response { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseOutputItemAddedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseOutputItemAddedUpdate>, IPersistableModel<StreamingResponseOutputItemAddedUpdate> {
        public ResponseItem Item { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseOutputItemDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseOutputItemDoneUpdate>, IPersistableModel<StreamingResponseOutputItemDoneUpdate> {
        public ResponseItem Item { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseOutputTextDeltaUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseOutputTextDeltaUpdate>, IPersistableModel<StreamingResponseOutputTextDeltaUpdate> {
        public int ContentIndex { get; }
        public string Delta { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseOutputTextDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseOutputTextDoneUpdate>, IPersistableModel<StreamingResponseOutputTextDoneUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string Text { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseQueuedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseQueuedUpdate>, IPersistableModel<StreamingResponseQueuedUpdate> {
        public OpenAIResponse Response { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseRefusalDeltaUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseRefusalDeltaUpdate>, IPersistableModel<StreamingResponseRefusalDeltaUpdate> {
        public int ContentIndex { get; }
        public string Delta { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseRefusalDoneUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseRefusalDoneUpdate>, IPersistableModel<StreamingResponseRefusalDoneUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string Refusal { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseTextAnnotationAddedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseTextAnnotationAddedUpdate>, IPersistableModel<StreamingResponseTextAnnotationAddedUpdate> {
        public BinaryData Annotation { get; }
        public int AnnotationIndex { get; }
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseUpdate : IJsonModel<StreamingResponseUpdate>, IPersistableModel<StreamingResponseUpdate> {
        public int SequenceNumber { get; }
        protected virtual StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseWebSearchCallCompletedUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseWebSearchCallCompletedUpdate>, IPersistableModel<StreamingResponseWebSearchCallCompletedUpdate> {
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseWebSearchCallInProgressUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseWebSearchCallInProgressUpdate>, IPersistableModel<StreamingResponseWebSearchCallInProgressUpdate> {
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StreamingResponseWebSearchCallSearchingUpdate : StreamingResponseUpdate, IJsonModel<StreamingResponseWebSearchCallSearchingUpdate>, IPersistableModel<StreamingResponseWebSearchCallSearchingUpdate> {
        public string ItemId { get; }
        public int OutputIndex { get; }
        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class WebSearchCallResponseItem : ResponseItem, IJsonModel<WebSearchCallResponseItem>, IPersistableModel<WebSearchCallResponseItem> {
        public WebSearchCallStatus? Status { get; }
        protected override ResponseItem JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override ResponseItem PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum WebSearchCallStatus {
        InProgress = 0,
        Searching = 1,
        Completed = 2,
        Failed = 3
    }
    public readonly partial struct WebSearchContextSize : IEquatable<WebSearchContextSize> {
        public WebSearchContextSize(string value);
        public static WebSearchContextSize High { get; }
        public static WebSearchContextSize Low { get; }
        public static WebSearchContextSize Medium { get; }
        public readonly bool Equals(WebSearchContextSize other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(WebSearchContextSize left, WebSearchContextSize right);
        public static implicit operator WebSearchContextSize(string value);
        public static bool operator !=(WebSearchContextSize left, WebSearchContextSize right);
        public override readonly string ToString();
    }
    public class WebSearchUserLocation : IJsonModel<WebSearchUserLocation>, IPersistableModel<WebSearchUserLocation> {
        public static WebSearchUserLocation CreateApproximateLocation(string country = null, string region = null, string city = null, string timezone = null);
        protected virtual WebSearchUserLocation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual WebSearchUserLocation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
}
namespace OpenAI.VectorStores {
    public class AddFileToVectorStoreOperation : OperationResult {
        public string FileId { get; }
        public override ContinuationToken? RehydrationToken { get; protected set; }
        public VectorStoreFileAssociationStatus? Status { get; }
        public VectorStoreFileAssociation? Value { get; }
        public string VectorStoreId { get; }
        public virtual ClientResult GetFileAssociation(RequestOptions? options);
        public virtual ClientResult<VectorStoreFileAssociation> GetFileAssociation(CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetFileAssociationAsync(RequestOptions? options);
        public virtual Task<ClientResult<VectorStoreFileAssociation>> GetFileAssociationAsync(CancellationToken cancellationToken = default);
        public static AddFileToVectorStoreOperation Rehydrate(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static Task<AddFileToVectorStoreOperation> RehydrateAsync(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public override ClientResult UpdateStatus(RequestOptions? options = null);
        public override ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null);
    }
    public class CreateBatchFileJobOperation : OperationResult {
        public string BatchId { get; }
        public override ContinuationToken? RehydrationToken { get; protected set; }
        public VectorStoreBatchFileJobStatus? Status { get; }
        public VectorStoreBatchFileJob? Value { get; }
        public string VectorStoreId { get; }
        public virtual ClientResult Cancel(RequestOptions? options);
        public virtual ClientResult<VectorStoreBatchFileJob> Cancel(CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CancelAsync(RequestOptions? options);
        public virtual Task<ClientResult<VectorStoreBatchFileJob>> CancelAsync(CancellationToken cancellationToken = default);
        public virtual ClientResult GetFileBatch(RequestOptions? options);
        public virtual ClientResult<VectorStoreBatchFileJob> GetFileBatch(CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetFileBatchAsync(RequestOptions? options);
        public virtual Task<ClientResult<VectorStoreBatchFileJob>> GetFileBatchAsync(CancellationToken cancellationToken = default);
        public static CreateBatchFileJobOperation Rehydrate(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static Task<CreateBatchFileJobOperation> RehydrateAsync(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public override ClientResult UpdateStatus(RequestOptions? options = null);
        public override ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null);
    }
    public class CreateVectorStoreOperation : OperationResult {
        public override ContinuationToken? RehydrationToken { get; protected set; }
        public VectorStoreStatus? Status { get; }
        public VectorStore? Value { get; }
        public string VectorStoreId { get; }
        public virtual ClientResult GetVectorStore(RequestOptions? options);
        public virtual ClientResult<VectorStore> GetVectorStore(CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetVectorStoreAsync(RequestOptions? options);
        public virtual Task<ClientResult<VectorStore>> GetVectorStoreAsync(CancellationToken cancellationToken = default);
        public static CreateVectorStoreOperation Rehydrate(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public static Task<CreateVectorStoreOperation> RehydrateAsync(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default);
        public override ClientResult UpdateStatus(RequestOptions? options = null);
        public override ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null);
    }
    public abstract class FileChunkingStrategy : IJsonModel<FileChunkingStrategy>, IPersistableModel<FileChunkingStrategy> {
        public static FileChunkingStrategy Auto { get; }
        public static FileChunkingStrategy Unknown { get; }
        public static FileChunkingStrategy CreateStaticStrategy(int maxTokensPerChunk, int overlappingTokenCount);
        protected virtual FileChunkingStrategy JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileChunkingStrategy PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class FileFromStoreRemovalResult : IJsonModel<FileFromStoreRemovalResult>, IPersistableModel<FileFromStoreRemovalResult> {
        public string FileId { get; }
        public bool Removed { get; }
        protected virtual FileFromStoreRemovalResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual FileFromStoreRemovalResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class StaticFileChunkingStrategy : FileChunkingStrategy, IJsonModel<StaticFileChunkingStrategy>, IPersistableModel<StaticFileChunkingStrategy> {
        public StaticFileChunkingStrategy(int maxTokensPerChunk, int overlappingTokenCount);
        public int MaxTokensPerChunk { get; }
        public int OverlappingTokenCount { get; }
        protected override FileChunkingStrategy JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected override FileChunkingStrategy PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
        protected virtual VectorStore PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class VectorStoreBatchFileJob : IJsonModel<VectorStoreBatchFileJob>, IPersistableModel<VectorStoreBatchFileJob> {
        public string BatchId { get; }
        public DateTimeOffset CreatedAt { get; }
        public VectorStoreFileCounts FileCounts { get; }
        public VectorStoreBatchFileJobStatus Status { get; }
        public string VectorStoreId { get; }
        protected virtual VectorStoreBatchFileJob JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreBatchFileJob PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public class VectorStoreClient {
        protected VectorStoreClient();
        public VectorStoreClient(ApiKeyCredential credential, OpenAIClientOptions options);
        public VectorStoreClient(ApiKeyCredential credential);
        protected internal VectorStoreClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public VectorStoreClient(string apiKey);
        public ClientPipeline Pipeline { get; }
        public virtual AddFileToVectorStoreOperation AddFileToVectorStore(string vectorStoreId, BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual AddFileToVectorStoreOperation AddFileToVectorStore(string vectorStoreId, string fileId, bool waitUntilCompleted, CancellationToken cancellationToken = default);
        public virtual Task<AddFileToVectorStoreOperation> AddFileToVectorStoreAsync(string vectorStoreId, BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual Task<AddFileToVectorStoreOperation> AddFileToVectorStoreAsync(string vectorStoreId, string fileId, bool waitUntilCompleted, CancellationToken cancellationToken = default);
        public virtual ClientResult CancelBatchFileJob(string vectorStoreId, string batchId, RequestOptions options);
        public virtual ClientResult<VectorStoreBatchFileJob> CancelBatchFileJob(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> CancelBatchFileJobAsync(string vectorStoreId, string batchId, RequestOptions options);
        public virtual Task<ClientResult<VectorStoreBatchFileJob>> CancelBatchFileJobAsync(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default);
        public virtual CreateBatchFileJobOperation CreateBatchFileJob(string vectorStoreId, BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual CreateBatchFileJobOperation CreateBatchFileJob(string vectorStoreId, IEnumerable<string> fileIds, bool waitUntilCompleted, CancellationToken cancellationToken = default);
        public virtual Task<CreateBatchFileJobOperation> CreateBatchFileJobAsync(string vectorStoreId, BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual Task<CreateBatchFileJobOperation> CreateBatchFileJobAsync(string vectorStoreId, IEnumerable<string> fileIds, bool waitUntilCompleted, CancellationToken cancellationToken = default);
        public virtual CreateVectorStoreOperation CreateVectorStore(bool waitUntilCompleted, VectorStoreCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual CreateVectorStoreOperation CreateVectorStore(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual ClientResult CreateVectorStore(BinaryContent content, RequestOptions options = null);
        public virtual Task<CreateVectorStoreOperation> CreateVectorStoreAsync(bool waitUntilCompleted, VectorStoreCreationOptions options = null, CancellationToken cancellationToken = default);
        public virtual Task<CreateVectorStoreOperation> CreateVectorStoreAsync(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null);
        public virtual Task<ClientResult> CreateVectorStoreAsync(BinaryContent content, RequestOptions options = null);
        public virtual ClientResult DeleteVectorStore(string vectorStoreId, RequestOptions options);
        public virtual ClientResult<VectorStoreDeletionResult> DeleteVectorStore(string vectorStoreId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> DeleteVectorStoreAsync(string vectorStoreId, RequestOptions options);
        public virtual Task<ClientResult<VectorStoreDeletionResult>> DeleteVectorStoreAsync(string vectorStoreId, CancellationToken cancellationToken = default);
        public virtual ClientResult<VectorStoreBatchFileJob> GetBatchFileJob(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult<VectorStoreBatchFileJob>> GetBatchFileJobAsync(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default);
        public virtual ClientResult GetFileAssociation(string vectorStoreId, string fileId, RequestOptions options);
        public virtual ClientResult<VectorStoreFileAssociation> GetFileAssociation(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> GetFileAssociationAsync(string vectorStoreId, string fileId, RequestOptions options);
        public virtual Task<ClientResult<VectorStoreFileAssociation>> GetFileAssociationAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(string vectorStoreId, VectorStoreFileAssociationCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetFileAssociations(string vectorStoreId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(string vectorStoreId, string batchJobId, VectorStoreFileAssociationCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(string vectorStoreId, string batchJobId, ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetFileAssociations(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(string vectorStoreId, VectorStoreFileAssociationCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetFileAssociationsAsync(string vectorStoreId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(string vectorStoreId, string batchJobId, VectorStoreFileAssociationCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(string vectorStoreId, string batchJobId, ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetFileAssociationsAsync(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options);
        public virtual ClientResult<VectorStore> GetVectorStore(string vectorStoreId, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStore> GetVectorStores(VectorStoreCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual CollectionResult<VectorStore> GetVectorStores(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual CollectionResult GetVectorStores(int? limit, string order, string after, string before, RequestOptions options);
        public virtual AsyncCollectionResult<VectorStore> GetVectorStoresAsync(VectorStoreCollectionOptions options = null, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult<VectorStore> GetVectorStoresAsync(ContinuationToken firstPageToken, CancellationToken cancellationToken = default);
        public virtual AsyncCollectionResult GetVectorStoresAsync(int? limit, string order, string after, string before, RequestOptions options);
        public virtual ClientResult<VectorStore> ModifyVectorStore(string vectorStoreId, VectorStoreModificationOptions options, CancellationToken cancellationToken = default);
        public virtual ClientResult ModifyVectorStore(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<VectorStore>> ModifyVectorStoreAsync(string vectorStoreId, VectorStoreModificationOptions options, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> ModifyVectorStoreAsync(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult RemoveFileFromStore(string vectorStoreId, string fileId, RequestOptions options);
        public virtual ClientResult<FileFromStoreRemovalResult> RemoveFileFromStore(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> RemoveFileFromStoreAsync(string vectorStoreId, string fileId, RequestOptions options);
        public virtual Task<ClientResult<FileFromStoreRemovalResult>> RemoveFileFromStoreAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default);
        public virtual ClientResult RetrieveVectorStoreFileContent(string vectorStoreId, string fileId, RequestOptions options);
        public virtual Task<ClientResult> RetrieveVectorStoreFileContentAsync(string vectorStoreId, string fileId, RequestOptions options);
        public virtual ClientResult SearchVectorStore(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult> SearchVectorStoreAsync(string vectorStoreId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult UpdateVectorStoreFileAttributes(string vectorStoreId, string fileId, BinaryContent content, RequestOptions options = null);
        public virtual ClientResult<VectorStoreFileAssociation> UpdateVectorStoreFileAttributes(string vectorStoreId, string fileId, IDictionary<string, BinaryData> attributes, CancellationToken cancellationToken = default);
        public virtual Task<ClientResult> UpdateVectorStoreFileAttributesAsync(string vectorStoreId, string fileId, BinaryContent content, RequestOptions options = null);
        public virtual Task<ClientResult<VectorStoreFileAssociation>> UpdateVectorStoreFileAttributesAsync(string vectorStoreId, string fileId, IDictionary<string, BinaryData> attributes, CancellationToken cancellationToken = default);
    }
    public class VectorStoreCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public VectorStoreCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
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
    public class VectorStoreDeletionResult : IJsonModel<VectorStoreDeletionResult>, IPersistableModel<VectorStoreDeletionResult> {
        public bool Deleted { get; }
        public string VectorStoreId { get; }
        protected virtual VectorStoreDeletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreDeletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum VectorStoreExpirationAnchor {
        Unknown = 0,
        LastActiveAt = 1
    }
    public class VectorStoreExpirationPolicy : IJsonModel<VectorStoreExpirationPolicy>, IPersistableModel<VectorStoreExpirationPolicy> {
        public VectorStoreExpirationPolicy(VectorStoreExpirationAnchor anchor, int days);
        public VectorStoreExpirationAnchor Anchor { get; set; }
        public int Days { get; set; }
        protected virtual VectorStoreExpirationPolicy JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreExpirationPolicy PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class VectorStoreFileAssociation : IJsonModel<VectorStoreFileAssociation>, IPersistableModel<VectorStoreFileAssociation> {
        public IDictionary<string, BinaryData> Attributes { get; }
        public FileChunkingStrategy ChunkingStrategy { get; }
        public DateTimeOffset CreatedAt { get; }
        public string FileId { get; }
        public VectorStoreFileAssociationError LastError { get; }
        public int Size { get; }
        public VectorStoreFileAssociationStatus Status { get; }
        public string VectorStoreId { get; }
        protected virtual VectorStoreFileAssociation JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreFileAssociation PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public class VectorStoreFileAssociationCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public VectorStoreFileStatusFilter? Filter { get; set; }
        public VectorStoreFileAssociationCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
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
    public class VectorStoreFileAssociationError : IJsonModel<VectorStoreFileAssociationError>, IPersistableModel<VectorStoreFileAssociationError> {
        public VectorStoreFileAssociationErrorCode Code { get; }
        public string Message { get; }
        protected virtual VectorStoreFileAssociationError JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreFileAssociationError PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
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
    public enum VectorStoreFileAssociationStatus {
        Unknown = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,
        Failed = 4
    }
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
    public class VectorStoreModificationOptions : IJsonModel<VectorStoreModificationOptions>, IPersistableModel<VectorStoreModificationOptions> {
        public VectorStoreExpirationPolicy ExpirationPolicy { get; set; }
        public IDictionary<string, string> Metadata { get; }
        public string Name { get; set; }
        protected virtual VectorStoreModificationOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        protected virtual VectorStoreModificationOptions PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options);
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options);
    }
    public enum VectorStoreStatus {
        Unknown = 0,
        InProgress = 1,
        Completed = 2,
        Expired = 3
    }
}