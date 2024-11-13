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
        public virtual EmbeddingClient GetEmbeddingClient(string model);
        public virtual FineTuningClient GetFineTuningClient();
        public virtual ImageClient GetImageClient(string model);
        public virtual ModerationClient GetModerationClient(string model);
        public virtual OpenAIFileClient GetOpenAIFileClient();
        public virtual OpenAIModelClient GetOpenAIModelClient();
        public virtual RealtimeConversation.RealtimeConversationClient GetRealtimeConversationClient(string model);
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
        Assistant IJsonModel<Assistant>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<Assistant>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        Assistant IPersistableModel<Assistant>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<Assistant>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<Assistant>.Write(ModelReaderWriterOptions options);
    }
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
    public class AssistantCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public AssistantCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    public readonly partial struct AssistantCollectionOrder : IEquatable<AssistantCollectionOrder> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        public IDictionary<string, string> Metadata { get; set; }
        public string Name { get; set; }
        public float? NucleusSamplingFactor { get; set; }
        public AssistantResponseFormat ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        public ToolResources ToolResources { get; set; }
        public IList<ToolDefinition> Tools { get; }
        AssistantCreationOptions IJsonModel<AssistantCreationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<AssistantCreationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        AssistantCreationOptions IPersistableModel<AssistantCreationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<AssistantCreationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<AssistantCreationOptions>.Write(ModelReaderWriterOptions options);
    }
    public class AssistantDeletionResult : IJsonModel<AssistantDeletionResult>, IPersistableModel<AssistantDeletionResult> {
        public string AssistantId { get; }
        public bool Deleted { get; }
        AssistantDeletionResult IJsonModel<AssistantDeletionResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<AssistantDeletionResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        AssistantDeletionResult IPersistableModel<AssistantDeletionResult>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<AssistantDeletionResult>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<AssistantDeletionResult>.Write(ModelReaderWriterOptions options);
    }
    public class AssistantModificationOptions : IJsonModel<AssistantModificationOptions>, IPersistableModel<AssistantModificationOptions> {
        public IList<ToolDefinition> DefaultTools { get; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public float? NucleusSamplingFactor { get; set; }
        public AssistantResponseFormat ResponseFormat { get; set; }
        public float? Temperature { get; set; }
        public ToolResources ToolResources { get; set; }
        AssistantModificationOptions IJsonModel<AssistantModificationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<AssistantModificationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        AssistantModificationOptions IPersistableModel<AssistantModificationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<AssistantModificationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<AssistantModificationOptions>.Write(ModelReaderWriterOptions options);
    }
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static implicit operator AssistantResponseFormat(string plainTextFormat);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator !=(AssistantResponseFormat first, AssistantResponseFormat second);
        AssistantResponseFormat IJsonModel<AssistantResponseFormat>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<AssistantResponseFormat>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        AssistantResponseFormat IPersistableModel<AssistantResponseFormat>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<AssistantResponseFormat>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<AssistantResponseFormat>.Write(ModelReaderWriterOptions options);
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
        AssistantThread IJsonModel<AssistantThread>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<AssistantThread>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        AssistantThread IPersistableModel<AssistantThread>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<AssistantThread>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<AssistantThread>.Write(ModelReaderWriterOptions options);
    }
    public class CodeInterpreterToolDefinition : ToolDefinition, IJsonModel<CodeInterpreterToolDefinition>, IPersistableModel<CodeInterpreterToolDefinition> {
        CodeInterpreterToolDefinition IJsonModel<CodeInterpreterToolDefinition>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<CodeInterpreterToolDefinition>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        CodeInterpreterToolDefinition IPersistableModel<CodeInterpreterToolDefinition>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<CodeInterpreterToolDefinition>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<CodeInterpreterToolDefinition>.Write(ModelReaderWriterOptions options);
    }
    public class CodeInterpreterToolResources : IJsonModel<CodeInterpreterToolResources>, IPersistableModel<CodeInterpreterToolResources> {
        public IList<string> FileIds { get; }
        CodeInterpreterToolResources IJsonModel<CodeInterpreterToolResources>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<CodeInterpreterToolResources>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        CodeInterpreterToolResources IPersistableModel<CodeInterpreterToolResources>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<CodeInterpreterToolResources>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<CodeInterpreterToolResources>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct FileSearchRanker : IEquatable<FileSearchRanker> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        public FileSearchRankingOptions();
        public FileSearchRankingOptions(float scoreThreshold);
        public FileSearchRanker? Ranker { get; set; }
        public required float ScoreThreshold { get; set; }
        FileSearchRankingOptions IJsonModel<FileSearchRankingOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<FileSearchRankingOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        FileSearchRankingOptions IPersistableModel<FileSearchRankingOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<FileSearchRankingOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<FileSearchRankingOptions>.Write(ModelReaderWriterOptions options);
    }
    public class FileSearchToolDefinition : ToolDefinition, IJsonModel<FileSearchToolDefinition>, IPersistableModel<FileSearchToolDefinition> {
        public int? MaxResults { get; set; }
        public FileSearchRankingOptions RankingOptions { get; set; }
        FileSearchToolDefinition IJsonModel<FileSearchToolDefinition>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<FileSearchToolDefinition>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        FileSearchToolDefinition IPersistableModel<FileSearchToolDefinition>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<FileSearchToolDefinition>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<FileSearchToolDefinition>.Write(ModelReaderWriterOptions options);
    }
    public class FileSearchToolResources : IJsonModel<FileSearchToolResources>, IPersistableModel<FileSearchToolResources> {
        public IList<VectorStoreCreationHelper> NewVectorStores { get; }
        public IList<string> VectorStoreIds { get; }
        FileSearchToolResources IJsonModel<FileSearchToolResources>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<FileSearchToolResources>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        FileSearchToolResources IPersistableModel<FileSearchToolResources>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<FileSearchToolResources>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<FileSearchToolResources>.Write(ModelReaderWriterOptions options);
    }
    public class FunctionToolDefinition : ToolDefinition, IJsonModel<FunctionToolDefinition>, IPersistableModel<FunctionToolDefinition> {
        public FunctionToolDefinition();
        public FunctionToolDefinition(string name);
        public string Description { get; set; }
        public required string FunctionName { get; set; }
        public BinaryData Parameters { get; set; }
        public bool? StrictParameterSchemaEnabled { get; set; }
        FunctionToolDefinition IJsonModel<FunctionToolDefinition>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<FunctionToolDefinition>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        FunctionToolDefinition IPersistableModel<FunctionToolDefinition>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<FunctionToolDefinition>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<FunctionToolDefinition>.Write(ModelReaderWriterOptions options);
    }
    public class MessageCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public MessageCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    public readonly partial struct MessageCollectionOrder : IEquatable<MessageCollectionOrder> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        public static implicit operator MessageContent(string value);
        MessageContent IJsonModel<MessageContent>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<MessageContent>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        MessageContent IPersistableModel<MessageContent>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<MessageContent>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<MessageContent>.Write(ModelReaderWriterOptions options);
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
        MessageCreationAttachment IJsonModel<MessageCreationAttachment>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<MessageCreationAttachment>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        MessageCreationAttachment IPersistableModel<MessageCreationAttachment>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<MessageCreationAttachment>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<MessageCreationAttachment>.Write(ModelReaderWriterOptions options);
    }
    public class MessageCreationOptions : IJsonModel<MessageCreationOptions>, IPersistableModel<MessageCreationOptions> {
        public IList<MessageCreationAttachment> Attachments { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
        MessageCreationOptions IJsonModel<MessageCreationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<MessageCreationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        MessageCreationOptions IPersistableModel<MessageCreationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<MessageCreationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<MessageCreationOptions>.Write(ModelReaderWriterOptions options);
    }
    public class MessageDeletionResult : IJsonModel<MessageDeletionResult>, IPersistableModel<MessageDeletionResult> {
        public bool Deleted { get; }
        public string MessageId { get; }
        MessageDeletionResult IJsonModel<MessageDeletionResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<MessageDeletionResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        MessageDeletionResult IPersistableModel<MessageDeletionResult>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<MessageDeletionResult>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<MessageDeletionResult>.Write(ModelReaderWriterOptions options);
    }
    public class MessageFailureDetails : IJsonModel<MessageFailureDetails>, IPersistableModel<MessageFailureDetails> {
        public MessageFailureReason Reason { get; }
        MessageFailureDetails IJsonModel<MessageFailureDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<MessageFailureDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        MessageFailureDetails IPersistableModel<MessageFailureDetails>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<MessageFailureDetails>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<MessageFailureDetails>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct MessageFailureReason : IEquatable<MessageFailureReason> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        public IDictionary<string, string> Metadata { get; set; }
        MessageModificationOptions IJsonModel<MessageModificationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<MessageModificationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        MessageModificationOptions IPersistableModel<MessageModificationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<MessageModificationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<MessageModificationOptions>.Write(ModelReaderWriterOptions options);
    }
    public enum MessageRole {
        User = 0,
        Assistant = 1
    }
    public readonly partial struct MessageStatus : IEquatable<MessageStatus> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        RunCreationOptions IJsonModel<RunCreationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunCreationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunCreationOptions IPersistableModel<RunCreationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunCreationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunCreationOptions>.Write(ModelReaderWriterOptions options);
    }
    public class RunError : IJsonModel<RunError>, IPersistableModel<RunError> {
        public RunErrorCode Code { get; }
        public string Message { get; }
        RunError IJsonModel<RunError>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunError>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunError IPersistableModel<RunError>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunError>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunError>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct RunErrorCode : IEquatable<RunErrorCode> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        RunIncompleteDetails IJsonModel<RunIncompleteDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunIncompleteDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunIncompleteDetails IPersistableModel<RunIncompleteDetails>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunIncompleteDetails>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunIncompleteDetails>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct RunIncompleteReason : IEquatable<RunIncompleteReason> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        public IDictionary<string, string> Metadata { get; set; }
        RunModificationOptions IJsonModel<RunModificationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunModificationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunModificationOptions IPersistableModel<RunModificationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunModificationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunModificationOptions>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct RunStatus : IEquatable<RunStatus> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        public RunStepError LastError { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public string RunId { get; }
        public RunStepStatus Status { get; }
        public string ThreadId { get; }
        public RunStepType Type { get; }
        public RunStepTokenUsage Usage { get; }
        RunStep IJsonModel<RunStep>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunStep>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunStep IPersistableModel<RunStep>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunStep>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunStep>.Write(ModelReaderWriterOptions options);
    }
    public abstract class RunStepCodeInterpreterOutput : IJsonModel<RunStepCodeInterpreterOutput>, IPersistableModel<RunStepCodeInterpreterOutput> {
        public string ImageFileId { get; }
        public string Logs { get; }
        RunStepCodeInterpreterOutput IJsonModel<RunStepCodeInterpreterOutput>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunStepCodeInterpreterOutput>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunStepCodeInterpreterOutput IPersistableModel<RunStepCodeInterpreterOutput>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunStepCodeInterpreterOutput>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunStepCodeInterpreterOutput>.Write(ModelReaderWriterOptions options);
    }
    public class RunStepCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public RunStepCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    public readonly partial struct RunStepCollectionOrder : IEquatable<RunStepCollectionOrder> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        RunStepDetails IJsonModel<RunStepDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunStepDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunStepDetails IPersistableModel<RunStepDetails>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunStepDetails>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunStepDetails>.Write(ModelReaderWriterOptions options);
    }
    public class RunStepDetailsUpdate : StreamingUpdate {
        public string CodeInterpreterInput { get; }
        public IReadOnlyList<RunStepUpdateCodeInterpreterOutput> CodeInterpreterOutputs { get; }
        public string CreatedMessageId { get; }
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
        RunStepError IJsonModel<RunStepError>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunStepError>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunStepError IPersistableModel<RunStepError>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunStepError>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunStepError>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct RunStepErrorCode : IEquatable<RunStepErrorCode> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        public string FileId { get; }
        public string FileName { get; }
        public float Score { get; }
        RunStepFileSearchResult IJsonModel<RunStepFileSearchResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunStepFileSearchResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunStepFileSearchResult IPersistableModel<RunStepFileSearchResult>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunStepFileSearchResult>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunStepFileSearchResult>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct RunStepStatus : IEquatable<RunStepStatus> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        RunStepTokenUsage IJsonModel<RunStepTokenUsage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunStepTokenUsage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunStepTokenUsage IPersistableModel<RunStepTokenUsage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunStepTokenUsage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunStepTokenUsage>.Write(ModelReaderWriterOptions options);
    }
    public abstract class RunStepToolCall : IJsonModel<RunStepToolCall>, IPersistableModel<RunStepToolCall> {
        public string CodeInterpreterInput { get; }
        public IReadOnlyList<RunStepCodeInterpreterOutput> CodeInterpreterOutputs { get; }
        public FileSearchRanker? FileSearchRanker { get; }
        public IReadOnlyList<RunStepFileSearchResult> FileSearchResults { get; }
        public float? FileSearchScoreThreshold { get; }
        public string FunctionArguments { get; }
        public string FunctionName { get; }
        public string FunctionOutput { get; }
        public string ToolCallId { get; }
        public RunStepToolCallKind ToolKind { get; }
        RunStepToolCall IJsonModel<RunStepToolCall>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunStepToolCall>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunStepToolCall IPersistableModel<RunStepToolCall>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunStepToolCall>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunStepToolCall>.Write(ModelReaderWriterOptions options);
    }
    public enum RunStepToolCallKind {
        Unknown = 0,
        CodeInterpreter = 1,
        FileSearch = 2,
        Function = 3
    }
    public readonly partial struct RunStepType : IEquatable<RunStepType> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
        public RunStepType(string value);
        public static RunStepType MessageCreation { get; }
        public static RunStepType ToolCalls { get; }
        public readonly bool Equals(RunStepType other);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly bool Equals(object obj);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override readonly int GetHashCode();
        public static bool operator ==(RunStepType left, RunStepType right);
        public static implicit operator RunStepType(string value);
        public static bool operator !=(RunStepType left, RunStepType right);
        public override readonly string ToString();
    }
    public class RunStepUpdate : StreamingUpdate<RunStep> {
    }
    public abstract class RunStepUpdateCodeInterpreterOutput : IJsonModel<RunStepUpdateCodeInterpreterOutput>, IPersistableModel<RunStepUpdateCodeInterpreterOutput> {
        public string ImageFileId { get; }
        public string Logs { get; }
        public int OutputIndex { get; }
        RunStepUpdateCodeInterpreterOutput IJsonModel<RunStepUpdateCodeInterpreterOutput>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunStepUpdateCodeInterpreterOutput>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunStepUpdateCodeInterpreterOutput IPersistableModel<RunStepUpdateCodeInterpreterOutput>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunStepUpdateCodeInterpreterOutput>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunStepUpdateCodeInterpreterOutput>.Write(ModelReaderWriterOptions options);
    }
    public class RunTokenUsage : IJsonModel<RunTokenUsage>, IPersistableModel<RunTokenUsage> {
        public int InputTokenCount { get; }
        public int OutputTokenCount { get; }
        public int TotalTokenCount { get; }
        RunTokenUsage IJsonModel<RunTokenUsage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunTokenUsage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunTokenUsage IPersistableModel<RunTokenUsage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunTokenUsage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunTokenUsage>.Write(ModelReaderWriterOptions options);
    }
    public class RunTruncationStrategy : IJsonModel<RunTruncationStrategy>, IPersistableModel<RunTruncationStrategy> {
        public static RunTruncationStrategy Auto { get; }
        public int? LastMessages { get; }
        public static RunTruncationStrategy CreateLastMessagesStrategy(int lastMessageCount);
        RunTruncationStrategy IJsonModel<RunTruncationStrategy>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<RunTruncationStrategy>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        RunTruncationStrategy IPersistableModel<RunTruncationStrategy>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<RunTruncationStrategy>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<RunTruncationStrategy>.Write(ModelReaderWriterOptions options);
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
        public IDictionary<string, string> Metadata { get; set; }
        public ToolResources ToolResources { get; set; }
        ThreadCreationOptions IJsonModel<ThreadCreationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ThreadCreationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ThreadCreationOptions IPersistableModel<ThreadCreationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ThreadCreationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ThreadCreationOptions>.Write(ModelReaderWriterOptions options);
    }
    public class ThreadDeletionResult : IJsonModel<ThreadDeletionResult>, IPersistableModel<ThreadDeletionResult> {
        public bool Deleted { get; }
        public string ThreadId { get; }
        ThreadDeletionResult IJsonModel<ThreadDeletionResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ThreadDeletionResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ThreadDeletionResult IPersistableModel<ThreadDeletionResult>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ThreadDeletionResult>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ThreadDeletionResult>.Write(ModelReaderWriterOptions options);
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
        ThreadMessage IJsonModel<ThreadMessage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ThreadMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ThreadMessage IPersistableModel<ThreadMessage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ThreadMessage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ThreadMessage>.Write(ModelReaderWriterOptions options);
    }
    public class ThreadModificationOptions : IJsonModel<ThreadModificationOptions>, IPersistableModel<ThreadModificationOptions> {
        public IDictionary<string, string> Metadata { get; set; }
        public ToolResources ToolResources { get; set; }
        ThreadModificationOptions IJsonModel<ThreadModificationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ThreadModificationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ThreadModificationOptions IPersistableModel<ThreadModificationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ThreadModificationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ThreadModificationOptions>.Write(ModelReaderWriterOptions options);
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
        ThreadRun IJsonModel<ThreadRun>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ThreadRun>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ThreadRun IPersistableModel<ThreadRun>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ThreadRun>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ThreadRun>.Write(ModelReaderWriterOptions options);
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
        ToolConstraint IJsonModel<ToolConstraint>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ToolConstraint>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ToolConstraint IPersistableModel<ToolConstraint>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ToolConstraint>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ToolConstraint>.Write(ModelReaderWriterOptions options);
    }
    public abstract class ToolDefinition : IJsonModel<ToolDefinition>, IPersistableModel<ToolDefinition> {
        protected ToolDefinition();
        protected ToolDefinition(string type);
        public static CodeInterpreterToolDefinition CreateCodeInterpreter();
        public static FileSearchToolDefinition CreateFileSearch(int? maxResults = null);
        public static FunctionToolDefinition CreateFunction(string name, string description = null, BinaryData parameters = null, bool? strictParameterSchemaEnabled = null);
        ToolDefinition IJsonModel<ToolDefinition>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ToolDefinition>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ToolDefinition IPersistableModel<ToolDefinition>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ToolDefinition>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ToolDefinition>.Write(ModelReaderWriterOptions options);
    }
    public class ToolOutput : IJsonModel<ToolOutput>, IPersistableModel<ToolOutput> {
        public ToolOutput();
        public ToolOutput(string toolCallId, string output);
        public string Output { get; set; }
        public string ToolCallId { get; set; }
        ToolOutput IJsonModel<ToolOutput>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ToolOutput>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ToolOutput IPersistableModel<ToolOutput>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ToolOutput>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ToolOutput>.Write(ModelReaderWriterOptions options);
    }
    public class ToolResources : IJsonModel<ToolResources>, IPersistableModel<ToolResources> {
        public CodeInterpreterToolResources CodeInterpreter { get; set; }
        public FileSearchToolResources FileSearch { get; set; }
        ToolResources IJsonModel<ToolResources>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ToolResources>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ToolResources IPersistableModel<ToolResources>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ToolResources>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ToolResources>.Write(ModelReaderWriterOptions options);
    }
    public class VectorStoreCreationHelper : IJsonModel<VectorStoreCreationHelper>, IPersistableModel<VectorStoreCreationHelper> {
        public VectorStoreCreationHelper();
        public VectorStoreCreationHelper(IEnumerable<OpenAIFile> files);
        public VectorStoreCreationHelper(IEnumerable<string> fileIds);
        public FileChunkingStrategy ChunkingStrategy { get; set; }
        public IList<string> FileIds { get; }
        public IDictionary<string, string> Metadata { get; }
        VectorStoreCreationHelper IJsonModel<VectorStoreCreationHelper>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<VectorStoreCreationHelper>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        VectorStoreCreationHelper IPersistableModel<VectorStoreCreationHelper>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<VectorStoreCreationHelper>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<VectorStoreCreationHelper>.Write(ModelReaderWriterOptions options);
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
        AudioTranscription IJsonModel<AudioTranscription>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<AudioTranscription>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        AudioTranscription IPersistableModel<AudioTranscription>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<AudioTranscription>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<AudioTranscription>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct AudioTranscriptionFormat : IEquatable<AudioTranscriptionFormat> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        AudioTranscriptionOptions IJsonModel<AudioTranscriptionOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<AudioTranscriptionOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        AudioTranscriptionOptions IPersistableModel<AudioTranscriptionOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<AudioTranscriptionOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<AudioTranscriptionOptions>.Write(ModelReaderWriterOptions options);
    }
    public class AudioTranslation : IJsonModel<AudioTranslation>, IPersistableModel<AudioTranslation> {
        public TimeSpan? Duration { get; }
        public string Language { get; }
        public IReadOnlyList<TranscribedSegment> Segments { get; }
        public string Text { get; }
        AudioTranslation IJsonModel<AudioTranslation>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<AudioTranslation>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        AudioTranslation IPersistableModel<AudioTranslation>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<AudioTranslation>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<AudioTranslation>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct AudioTranslationFormat : IEquatable<AudioTranslationFormat> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        AudioTranslationOptions IJsonModel<AudioTranslationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<AudioTranslationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        AudioTranslationOptions IPersistableModel<AudioTranslationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<AudioTranslationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<AudioTranslationOptions>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct GeneratedSpeechFormat : IEquatable<GeneratedSpeechFormat> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
        public GeneratedSpeechVoice(string value);
        public static GeneratedSpeechVoice Alloy { get; }
        public static GeneratedSpeechVoice Echo { get; }
        public static GeneratedSpeechVoice Fable { get; }
        public static GeneratedSpeechVoice Nova { get; }
        public static GeneratedSpeechVoice Onyx { get; }
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
        SpeechGenerationOptions IJsonModel<SpeechGenerationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<SpeechGenerationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        SpeechGenerationOptions IPersistableModel<SpeechGenerationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<SpeechGenerationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<SpeechGenerationOptions>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct TranscribedSegment : IJsonModel<TranscribedSegment>, IPersistableModel<TranscribedSegment>, IJsonModel<object>, IPersistableModel<object> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        readonly TranscribedSegment IJsonModel<TranscribedSegment>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        readonly void IJsonModel<TranscribedSegment>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        readonly object IJsonModel<object>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        readonly void IJsonModel<object>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        readonly TranscribedSegment IPersistableModel<TranscribedSegment>.Create(BinaryData data, ModelReaderWriterOptions options);
        readonly string IPersistableModel<TranscribedSegment>.GetFormatFromOptions(ModelReaderWriterOptions options);
        readonly BinaryData IPersistableModel<TranscribedSegment>.Write(ModelReaderWriterOptions options);
        readonly object IPersistableModel<object>.Create(BinaryData data, ModelReaderWriterOptions options);
        readonly string IPersistableModel<object>.GetFormatFromOptions(ModelReaderWriterOptions options);
        readonly BinaryData IPersistableModel<object>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct TranscribedWord : IJsonModel<TranscribedWord>, IPersistableModel<TranscribedWord>, IJsonModel<object>, IPersistableModel<object> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
        public TimeSpan EndTime { get; }
        public TimeSpan StartTime { get; }
        public string Word { get; }
        readonly TranscribedWord IJsonModel<TranscribedWord>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        readonly void IJsonModel<TranscribedWord>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        readonly object IJsonModel<object>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        readonly void IJsonModel<object>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        readonly TranscribedWord IPersistableModel<TranscribedWord>.Create(BinaryData data, ModelReaderWriterOptions options);
        readonly string IPersistableModel<TranscribedWord>.GetFormatFromOptions(ModelReaderWriterOptions options);
        readonly BinaryData IPersistableModel<TranscribedWord>.Write(ModelReaderWriterOptions options);
        readonly object IPersistableModel<object>.Create(BinaryData data, ModelReaderWriterOptions options);
        readonly string IPersistableModel<object>.GetFormatFromOptions(ModelReaderWriterOptions options);
        readonly BinaryData IPersistableModel<object>.Write(ModelReaderWriterOptions options);
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
        public AssistantChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public AssistantChatMessage(IEnumerable<ChatToolCall> toolCalls);
        public AssistantChatMessage(string content);
        [Obsolete("This property is obsolete. Please use ToolCalls instead.")]
        public ChatFunctionCall FunctionCall { get; set; }
        public string ParticipantName { get; set; }
        public string Refusal { get; set; }
        public IList<ChatToolCall> ToolCalls { get; }
        AssistantChatMessage IJsonModel<AssistantChatMessage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<AssistantChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        AssistantChatMessage IPersistableModel<AssistantChatMessage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<AssistantChatMessage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<AssistantChatMessage>.Write(ModelReaderWriterOptions options);
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
        public string Refusal { get; }
        public IReadOnlyList<ChatTokenLogProbabilityDetails> RefusalTokenLogProbabilities { get; }
        public ChatMessageRole Role { get; }
        public string SystemFingerprint { get; }
        public IReadOnlyList<ChatToolCall> ToolCalls { get; }
        public ChatTokenUsage Usage { get; }
        ChatCompletion IJsonModel<ChatCompletion>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatCompletion>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatCompletion IPersistableModel<ChatCompletion>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatCompletion>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatCompletion>.Write(ModelReaderWriterOptions options);
    }
    public class ChatCompletionOptions : IJsonModel<ChatCompletionOptions>, IPersistableModel<ChatCompletionOptions> {
        public bool? AllowParallelToolCalls { get; set; }
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
        public float? PresencePenalty { get; set; }
        public ChatResponseFormat ResponseFormat { get; set; }
        public long? Seed { get; set; }
        public IList<string> StopSequences { get; }
        public bool? StoredOutputEnabled { get; set; }
        public float? Temperature { get; set; }
        public ChatToolChoice ToolChoice { get; set; }
        public IList<ChatTool> Tools { get; }
        public int? TopLogProbabilityCount { get; set; }
        public float? TopP { get; set; }
        ChatCompletionOptions IJsonModel<ChatCompletionOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatCompletionOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatCompletionOptions IPersistableModel<ChatCompletionOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatCompletionOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatCompletionOptions>.Write(ModelReaderWriterOptions options);
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
        ChatFunction IJsonModel<ChatFunction>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatFunction>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatFunction IPersistableModel<ChatFunction>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatFunction>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatFunction>.Write(ModelReaderWriterOptions options);
    }
    [Obsolete("This class is obsolete. Please use ChatToolCall instead.")]
    public class ChatFunctionCall : IJsonModel<ChatFunctionCall>, IPersistableModel<ChatFunctionCall> {
        public ChatFunctionCall(string functionName, BinaryData functionArguments);
        public BinaryData FunctionArguments { get; }
        public string FunctionName { get; }
        ChatFunctionCall IJsonModel<ChatFunctionCall>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatFunctionCall>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatFunctionCall IPersistableModel<ChatFunctionCall>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatFunctionCall>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatFunctionCall>.Write(ModelReaderWriterOptions options);
    }
    [Obsolete("This class is obsolete. Please use ChatToolChoice instead.")]
    public class ChatFunctionChoice : IJsonModel<ChatFunctionChoice>, IPersistableModel<ChatFunctionChoice> {
        public static ChatFunctionChoice CreateAutoChoice();
        public static ChatFunctionChoice CreateNamedChoice(string functionName);
        public static ChatFunctionChoice CreateNoneChoice();
        ChatFunctionChoice IJsonModel<ChatFunctionChoice>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatFunctionChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatFunctionChoice IPersistableModel<ChatFunctionChoice>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatFunctionChoice>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatFunctionChoice>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct ChatImageDetailLevel : IEquatable<ChatImageDetailLevel> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
    public class ChatInputTokenUsageDetails : IJsonModel<ChatInputTokenUsageDetails>, IPersistableModel<ChatInputTokenUsageDetails> {
        public int? AudioTokenCount { get; }
        public int? CachedTokenCount { get; }
        ChatInputTokenUsageDetails IJsonModel<ChatInputTokenUsageDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatInputTokenUsageDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatInputTokenUsageDetails IPersistableModel<ChatInputTokenUsageDetails>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatInputTokenUsageDetails>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatInputTokenUsageDetails>.Write(ModelReaderWriterOptions options);
    }
    public class ChatMessage : IJsonModel<ChatMessage>, IPersistableModel<ChatMessage> {
        public ChatMessageContent Content { get; }
        public static AssistantChatMessage CreateAssistantMessage(ChatCompletion chatCompletion);
        public static AssistantChatMessage CreateAssistantMessage(ChatFunctionCall functionCall);
        public static AssistantChatMessage CreateAssistantMessage(params ChatMessageContentPart[] contentParts);
        public static AssistantChatMessage CreateAssistantMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public static AssistantChatMessage CreateAssistantMessage(IEnumerable<ChatToolCall> toolCalls);
        public static AssistantChatMessage CreateAssistantMessage(string content);
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
        public static implicit operator ChatMessage(string content);
        ChatMessage IJsonModel<ChatMessage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatMessage IPersistableModel<ChatMessage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatMessage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatMessage>.Write(ModelReaderWriterOptions options);
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
        public ChatMessageContentPartKind Kind { get; }
        public string Refusal { get; }
        public string Text { get; }
        public static ChatMessageContentPart CreateImagePart(BinaryData imageBytes, string imageBytesMediaType, ChatImageDetailLevel? imageDetailLevel = null);
        public static ChatMessageContentPart CreateImagePart(Uri imageUri, ChatImageDetailLevel? imageDetailLevel = null);
        public static ChatMessageContentPart CreateRefusalPart(string refusal);
        public static ChatMessageContentPart CreateTextPart(string text);
        public static implicit operator ChatMessageContentPart(string text);
        ChatMessageContentPart IJsonModel<ChatMessageContentPart>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatMessageContentPart>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatMessageContentPart IPersistableModel<ChatMessageContentPart>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatMessageContentPart>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatMessageContentPart>.Write(ModelReaderWriterOptions options);
    }
    public enum ChatMessageContentPartKind {
        Text = 0,
        Refusal = 1,
        Image = 2
    }
    public enum ChatMessageRole {
        System = 0,
        User = 1,
        Assistant = 2,
        Tool = 3,
        Function = 4
    }
    public class ChatOutputTokenUsageDetails : IJsonModel<ChatOutputTokenUsageDetails>, IPersistableModel<ChatOutputTokenUsageDetails> {
        public int? AudioTokenCount { get; }
        public int ReasoningTokenCount { get; }
        ChatOutputTokenUsageDetails IJsonModel<ChatOutputTokenUsageDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatOutputTokenUsageDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatOutputTokenUsageDetails IPersistableModel<ChatOutputTokenUsageDetails>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatOutputTokenUsageDetails>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatOutputTokenUsageDetails>.Write(ModelReaderWriterOptions options);
    }
    public class ChatResponseFormat : IJsonModel<ChatResponseFormat>, IPersistableModel<ChatResponseFormat> {
        public static ChatResponseFormat CreateJsonObjectFormat();
        public static ChatResponseFormat CreateJsonSchemaFormat(string jsonSchemaFormatName, BinaryData jsonSchema, string jsonSchemaFormatDescription = null, bool? jsonSchemaIsStrict = null);
        public static ChatResponseFormat CreateTextFormat();
        ChatResponseFormat IJsonModel<ChatResponseFormat>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatResponseFormat>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatResponseFormat IPersistableModel<ChatResponseFormat>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatResponseFormat>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatResponseFormat>.Write(ModelReaderWriterOptions options);
    }
    public class ChatTokenLogProbabilityDetails : IJsonModel<ChatTokenLogProbabilityDetails>, IPersistableModel<ChatTokenLogProbabilityDetails> {
        public float LogProbability { get; }
        public string Token { get; }
        public IReadOnlyList<ChatTokenTopLogProbabilityDetails> TopLogProbabilities { get; }
        public ReadOnlyMemory<byte>? Utf8Bytes { get; }
        ChatTokenLogProbabilityDetails IJsonModel<ChatTokenLogProbabilityDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatTokenLogProbabilityDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatTokenLogProbabilityDetails IPersistableModel<ChatTokenLogProbabilityDetails>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatTokenLogProbabilityDetails>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatTokenLogProbabilityDetails>.Write(ModelReaderWriterOptions options);
    }
    public class ChatTokenTopLogProbabilityDetails : IJsonModel<ChatTokenTopLogProbabilityDetails>, IPersistableModel<ChatTokenTopLogProbabilityDetails> {
        public float LogProbability { get; }
        public string Token { get; }
        public ReadOnlyMemory<byte>? Utf8Bytes { get; }
        ChatTokenTopLogProbabilityDetails IJsonModel<ChatTokenTopLogProbabilityDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatTokenTopLogProbabilityDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatTokenTopLogProbabilityDetails IPersistableModel<ChatTokenTopLogProbabilityDetails>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatTokenTopLogProbabilityDetails>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatTokenTopLogProbabilityDetails>.Write(ModelReaderWriterOptions options);
    }
    public class ChatTokenUsage : IJsonModel<ChatTokenUsage>, IPersistableModel<ChatTokenUsage> {
        public int InputTokenCount { get; }
        public ChatInputTokenUsageDetails InputTokenDetails { get; }
        public int OutputTokenCount { get; }
        public ChatOutputTokenUsageDetails OutputTokenDetails { get; }
        public int TotalTokenCount { get; }
        ChatTokenUsage IJsonModel<ChatTokenUsage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatTokenUsage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatTokenUsage IPersistableModel<ChatTokenUsage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatTokenUsage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatTokenUsage>.Write(ModelReaderWriterOptions options);
    }
    public class ChatTool : IJsonModel<ChatTool>, IPersistableModel<ChatTool> {
        public string FunctionDescription { get; }
        public string FunctionName { get; }
        public BinaryData FunctionParameters { get; }
        public bool? FunctionSchemaIsStrict { get; }
        public ChatToolKind Kind { get; }
        public static ChatTool CreateFunctionTool(string functionName, string functionDescription = null, BinaryData functionParameters = null, bool? functionSchemaIsStrict = null);
        ChatTool IJsonModel<ChatTool>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatTool>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatTool IPersistableModel<ChatTool>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatTool>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatTool>.Write(ModelReaderWriterOptions options);
    }
    public class ChatToolCall : IJsonModel<ChatToolCall>, IPersistableModel<ChatToolCall> {
        public BinaryData FunctionArguments { get; }
        public string FunctionName { get; }
        public string Id { get; set; }
        public ChatToolCallKind Kind { get; }
        public static ChatToolCall CreateFunctionToolCall(string id, string functionName, BinaryData functionArguments);
        ChatToolCall IJsonModel<ChatToolCall>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatToolCall>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatToolCall IPersistableModel<ChatToolCall>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatToolCall>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatToolCall>.Write(ModelReaderWriterOptions options);
    }
    public enum ChatToolCallKind {
        Function = 0
    }
    public class ChatToolChoice : IJsonModel<ChatToolChoice>, IPersistableModel<ChatToolChoice> {
        public static ChatToolChoice CreateAutoChoice();
        public static ChatToolChoice CreateFunctionChoice(string functionName);
        public static ChatToolChoice CreateNoneChoice();
        public static ChatToolChoice CreateRequiredChoice();
        ChatToolChoice IJsonModel<ChatToolChoice>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ChatToolChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ChatToolChoice IPersistableModel<ChatToolChoice>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ChatToolChoice>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ChatToolChoice>.Write(ModelReaderWriterOptions options);
    }
    public enum ChatToolKind {
        Function = 0
    }
    [Obsolete("This class is obsolete. Please use ToolChatMessage instead.")]
    public class FunctionChatMessage : ChatMessage, IJsonModel<FunctionChatMessage>, IPersistableModel<FunctionChatMessage> {
        public FunctionChatMessage(string functionName, string content);
        public string FunctionName { get; }
        FunctionChatMessage IJsonModel<FunctionChatMessage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<FunctionChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        FunctionChatMessage IPersistableModel<FunctionChatMessage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<FunctionChatMessage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<FunctionChatMessage>.Write(ModelReaderWriterOptions options);
    }
    public static class OpenAIChatModelFactory {
        public static ChatCompletion ChatCompletion(string id = null, ChatFinishReason finishReason = ChatFinishReason.Stop, ChatMessageContent content = null, string refusal = null, IEnumerable<ChatToolCall> toolCalls = null, ChatMessageRole role = ChatMessageRole.System, ChatFunctionCall functionCall = null, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null, DateTimeOffset createdAt = default, string model = null, string systemFingerprint = null, ChatTokenUsage usage = null);
        public static ChatInputTokenUsageDetails ChatInputTokenUsageDetails(int? audioTokenCount = null, int? cachedTokenCount = null);
        public static ChatOutputTokenUsageDetails ChatOutputTokenUsageDetails(int reasoningTokenCount = 0, int? audioTokenCount = null);
        public static ChatTokenLogProbabilityDetails ChatTokenLogProbabilityDetails(string token = null, float logProbability = 0, ReadOnlyMemory<byte>? utf8Bytes = null, IEnumerable<ChatTokenTopLogProbabilityDetails> topLogProbabilities = null);
        public static ChatTokenTopLogProbabilityDetails ChatTokenTopLogProbabilityDetails(string token = null, float logProbability = 0, ReadOnlyMemory<byte>? utf8Bytes = null);
        public static ChatTokenUsage ChatTokenUsage(int outputTokenCount = 0, int inputTokenCount = 0, int totalTokenCount = 0, ChatOutputTokenUsageDetails outputTokenDetails = null, ChatInputTokenUsageDetails inputTokenDetails = null);
        public static StreamingChatCompletionUpdate StreamingChatCompletionUpdate(string completionId = null, ChatMessageContent contentUpdate = null, StreamingChatFunctionCallUpdate functionCallUpdate = null, IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates = null, ChatMessageRole? role = null, string refusalUpdate = null, IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null, IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null, ChatFinishReason? finishReason = null, DateTimeOffset createdAt = default, string model = null, string systemFingerprint = null, ChatTokenUsage usage = null);
        [Obsolete("This class is obsolete. Please use StreamingChatToolCallUpdate instead.")]
        public static StreamingChatFunctionCallUpdate StreamingChatFunctionCallUpdate(string functionName = null, BinaryData functionArgumentsUpdate = null);
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
        public IReadOnlyList<ChatTokenLogProbabilityDetails> RefusalTokenLogProbabilities { get; }
        public string RefusalUpdate { get; }
        public ChatMessageRole? Role { get; }
        public string SystemFingerprint { get; }
        public IReadOnlyList<StreamingChatToolCallUpdate> ToolCallUpdates { get; }
        public ChatTokenUsage Usage { get; }
        StreamingChatCompletionUpdate IJsonModel<StreamingChatCompletionUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<StreamingChatCompletionUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        StreamingChatCompletionUpdate IPersistableModel<StreamingChatCompletionUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<StreamingChatCompletionUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<StreamingChatCompletionUpdate>.Write(ModelReaderWriterOptions options);
    }
    [Obsolete("This class is obsolete. Please use StreamingChatToolCallUpdate instead.")]
    public class StreamingChatFunctionCallUpdate : IJsonModel<StreamingChatFunctionCallUpdate>, IPersistableModel<StreamingChatFunctionCallUpdate> {
        public BinaryData FunctionArgumentsUpdate { get; }
        public string FunctionName { get; }
        StreamingChatFunctionCallUpdate IJsonModel<StreamingChatFunctionCallUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<StreamingChatFunctionCallUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        StreamingChatFunctionCallUpdate IPersistableModel<StreamingChatFunctionCallUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<StreamingChatFunctionCallUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<StreamingChatFunctionCallUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class StreamingChatToolCallUpdate : IJsonModel<StreamingChatToolCallUpdate>, IPersistableModel<StreamingChatToolCallUpdate> {
        public BinaryData FunctionArgumentsUpdate { get; }
        public string FunctionName { get; }
        public int Index { get; }
        public ChatToolCallKind Kind { get; }
        public string ToolCallId { get; }
        StreamingChatToolCallUpdate IJsonModel<StreamingChatToolCallUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<StreamingChatToolCallUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        StreamingChatToolCallUpdate IPersistableModel<StreamingChatToolCallUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<StreamingChatToolCallUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<StreamingChatToolCallUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class SystemChatMessage : ChatMessage, IJsonModel<SystemChatMessage>, IPersistableModel<SystemChatMessage> {
        public SystemChatMessage(params ChatMessageContentPart[] contentParts);
        public SystemChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public SystemChatMessage(string content);
        public string ParticipantName { get; set; }
        SystemChatMessage IJsonModel<SystemChatMessage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<SystemChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        SystemChatMessage IPersistableModel<SystemChatMessage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<SystemChatMessage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<SystemChatMessage>.Write(ModelReaderWriterOptions options);
    }
    public class ToolChatMessage : ChatMessage, IJsonModel<ToolChatMessage>, IPersistableModel<ToolChatMessage> {
        public ToolChatMessage(string toolCallId, params ChatMessageContentPart[] contentParts);
        public ToolChatMessage(string toolCallId, IEnumerable<ChatMessageContentPart> contentParts);
        public ToolChatMessage(string toolCallId, string content);
        public string ToolCallId { get; }
        ToolChatMessage IJsonModel<ToolChatMessage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ToolChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ToolChatMessage IPersistableModel<ToolChatMessage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ToolChatMessage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ToolChatMessage>.Write(ModelReaderWriterOptions options);
    }
    public class UserChatMessage : ChatMessage, IJsonModel<UserChatMessage>, IPersistableModel<UserChatMessage> {
        public UserChatMessage(params ChatMessageContentPart[] contentParts);
        public UserChatMessage(IEnumerable<ChatMessageContentPart> contentParts);
        public UserChatMessage(string content);
        public string ParticipantName { get; set; }
        UserChatMessage IJsonModel<UserChatMessage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<UserChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        UserChatMessage IPersistableModel<UserChatMessage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<UserChatMessage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<UserChatMessage>.Write(ModelReaderWriterOptions options);
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
        EmbeddingGenerationOptions IJsonModel<EmbeddingGenerationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<EmbeddingGenerationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        EmbeddingGenerationOptions IPersistableModel<EmbeddingGenerationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<EmbeddingGenerationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<EmbeddingGenerationOptions>.Write(ModelReaderWriterOptions options);
    }
    public class EmbeddingTokenUsage : IJsonModel<EmbeddingTokenUsage>, IPersistableModel<EmbeddingTokenUsage> {
        public int InputTokenCount { get; }
        public int TotalTokenCount { get; }
        EmbeddingTokenUsage IJsonModel<EmbeddingTokenUsage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<EmbeddingTokenUsage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        EmbeddingTokenUsage IPersistableModel<EmbeddingTokenUsage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<EmbeddingTokenUsage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<EmbeddingTokenUsage>.Write(ModelReaderWriterOptions options);
    }
    public class OpenAIEmbedding : IJsonModel<OpenAIEmbedding>, IPersistableModel<OpenAIEmbedding> {
        public int Index { get; }
        OpenAIEmbedding IJsonModel<OpenAIEmbedding>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<OpenAIEmbedding>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        OpenAIEmbedding IPersistableModel<OpenAIEmbedding>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<OpenAIEmbedding>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<OpenAIEmbedding>.Write(ModelReaderWriterOptions options);
        public ReadOnlyMemory<float> ToFloats();
    }
    public class OpenAIEmbeddingCollection : ObjectModel.ReadOnlyCollection<OpenAIEmbedding>, IJsonModel<OpenAIEmbeddingCollection>, IPersistableModel<OpenAIEmbeddingCollection> {
        public string Model { get; }
        public EmbeddingTokenUsage Usage { get; }
        OpenAIEmbeddingCollection IJsonModel<OpenAIEmbeddingCollection>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<OpenAIEmbeddingCollection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        OpenAIEmbeddingCollection IPersistableModel<OpenAIEmbeddingCollection>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<OpenAIEmbeddingCollection>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<OpenAIEmbeddingCollection>.Write(ModelReaderWriterOptions options);
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
        FileDeletionResult IJsonModel<FileDeletionResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<FileDeletionResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        FileDeletionResult IPersistableModel<FileDeletionResult>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<FileDeletionResult>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<FileDeletionResult>.Write(ModelReaderWriterOptions options);
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
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        OpenAIFile IJsonModel<OpenAIFile>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<OpenAIFile>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        OpenAIFile IPersistableModel<OpenAIFile>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<OpenAIFile>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<OpenAIFile>.Write(ModelReaderWriterOptions options);
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
        OpenAIFileCollection IJsonModel<OpenAIFileCollection>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<OpenAIFileCollection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        OpenAIFileCollection IPersistableModel<OpenAIFileCollection>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<OpenAIFileCollection>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<OpenAIFileCollection>.Write(ModelReaderWriterOptions options);
    }
    public static class OpenAIFilesModelFactory {
        public static FileDeletionResult FileDeletionResult(string fileId = null, bool deleted = false);
        public static OpenAIFileCollection OpenAIFileCollection(IEnumerable<OpenAIFile> items = null);
        public static OpenAIFile OpenAIFileInfo(string id = null, int? sizeInBytes = null, DateTimeOffset createdAt = default, string filename = null, FilePurpose purpose = FilePurpose.Assistants, FileStatus status = FileStatus.Uploaded, string statusDetails = null);
    }
}
namespace OpenAI.FineTuning {
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
        GeneratedImage IJsonModel<GeneratedImage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<GeneratedImage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        GeneratedImage IPersistableModel<GeneratedImage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<GeneratedImage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<GeneratedImage>.Write(ModelReaderWriterOptions options);
    }
    public class GeneratedImageCollection : ObjectModel.ReadOnlyCollection<GeneratedImage>, IJsonModel<GeneratedImageCollection>, IPersistableModel<GeneratedImageCollection> {
        public DateTimeOffset CreatedAt { get; }
        GeneratedImageCollection IJsonModel<GeneratedImageCollection>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<GeneratedImageCollection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        GeneratedImageCollection IPersistableModel<GeneratedImageCollection>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<GeneratedImageCollection>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<GeneratedImageCollection>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct GeneratedImageFormat : IEquatable<GeneratedImageFormat> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        ImageEditOptions IJsonModel<ImageEditOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ImageEditOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ImageEditOptions IPersistableModel<ImageEditOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ImageEditOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ImageEditOptions>.Write(ModelReaderWriterOptions options);
    }
    public class ImageGenerationOptions : IJsonModel<ImageGenerationOptions>, IPersistableModel<ImageGenerationOptions> {
        public string EndUserId { get; set; }
        public GeneratedImageQuality? Quality { get; set; }
        public GeneratedImageFormat? ResponseFormat { get; set; }
        public GeneratedImageSize? Size { get; set; }
        public GeneratedImageStyle? Style { get; set; }
        ImageGenerationOptions IJsonModel<ImageGenerationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ImageGenerationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ImageGenerationOptions IPersistableModel<ImageGenerationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ImageGenerationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ImageGenerationOptions>.Write(ModelReaderWriterOptions options);
    }
    public class ImageVariationOptions : IJsonModel<ImageVariationOptions>, IPersistableModel<ImageVariationOptions> {
        public string EndUserId { get; set; }
        public GeneratedImageFormat? ResponseFormat { get; set; }
        public GeneratedImageSize? Size { get; set; }
        ImageVariationOptions IJsonModel<ImageVariationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ImageVariationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ImageVariationOptions IPersistableModel<ImageVariationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ImageVariationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ImageVariationOptions>.Write(ModelReaderWriterOptions options);
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
        ModelDeletionResult IJsonModel<ModelDeletionResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ModelDeletionResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ModelDeletionResult IPersistableModel<ModelDeletionResult>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ModelDeletionResult>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ModelDeletionResult>.Write(ModelReaderWriterOptions options);
    }
    public class OpenAIModel : IJsonModel<OpenAIModel>, IPersistableModel<OpenAIModel> {
        public DateTimeOffset CreatedAt { get; }
        public string Id { get; }
        public string OwnedBy { get; }
        OpenAIModel IJsonModel<OpenAIModel>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<OpenAIModel>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        OpenAIModel IPersistableModel<OpenAIModel>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<OpenAIModel>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<OpenAIModel>.Write(ModelReaderWriterOptions options);
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
        OpenAIModelCollection IJsonModel<OpenAIModelCollection>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<OpenAIModelCollection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        OpenAIModelCollection IPersistableModel<OpenAIModelCollection>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<OpenAIModelCollection>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<OpenAIModelCollection>.Write(ModelReaderWriterOptions options);
    }
    public static class OpenAIModelsModelFactory {
        public static ModelDeletionResult ModelDeletionResult(string modelId = null, bool deleted = false);
        public static OpenAIModel OpenAIModel(string id = null, DateTimeOffset createdAt = default, string ownedBy = null);
        public static OpenAIModelCollection OpenAIModelCollection(IEnumerable<OpenAIModel> items = null);
    }
}
namespace OpenAI.Moderations {
    [Flags]
    public enum ModerationApplicableInputKinds {
        None = 0,
        Other = 1,
        Text = 2,
        Image = 4
    }
    public class ModerationCategory {
        public ModerationApplicableInputKinds ApplicableInputKinds { get; }
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
        ModerationResult IJsonModel<ModerationResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ModerationResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ModerationResult IPersistableModel<ModerationResult>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ModerationResult>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ModerationResult>.Write(ModelReaderWriterOptions options);
    }
    public class ModerationResultCollection : ObjectModel.ReadOnlyCollection<ModerationResult>, IJsonModel<ModerationResultCollection>, IPersistableModel<ModerationResultCollection> {
        public string Id { get; }
        public string Model { get; }
        ModerationResultCollection IJsonModel<ModerationResultCollection>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ModerationResultCollection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ModerationResultCollection IPersistableModel<ModerationResultCollection>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ModerationResultCollection>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ModerationResultCollection>.Write(ModelReaderWriterOptions options);
    }
    public static class OpenAIModerationsModelFactory {
        public static ModerationCategory ModerationCategory(bool flagged = false, float score = 0, ModerationApplicableInputKinds applicableInputKinds = ModerationApplicableInputKinds.None);
        public static ModerationResult ModerationResult(bool flagged = false, ModerationCategory hate = null, ModerationCategory hateThreatening = null, ModerationCategory harassment = null, ModerationCategory harassmentThreatening = null, ModerationCategory selfHarm = null, ModerationCategory selfHarmIntent = null, ModerationCategory selfHarmInstructions = null, ModerationCategory sexual = null, ModerationCategory sexualMinors = null, ModerationCategory violence = null, ModerationCategory violenceGraphic = null, ModerationCategory illicit = null, ModerationCategory illicitViolent = null);
        public static ModerationResultCollection ModerationResultCollection(string id = null, string model = null, IEnumerable<ModerationResult> items = null);
    }
}
namespace OpenAI.RealtimeConversation {
    public readonly partial struct ConversationAudioFormat : IEquatable<ConversationAudioFormat> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
    [Flags]
    public enum ConversationContentModalities {
        Text = 1,
        Audio = 2
    }
    public abstract class ConversationContentPart : IJsonModel<ConversationContentPart>, IPersistableModel<ConversationContentPart> {
        public string AudioTranscript { get; }
        public string Text { get; }
        public static ConversationContentPart FromInputAudioTranscript(string transcript = null);
        public static ConversationContentPart FromInputText(string text);
        public static ConversationContentPart FromOutputAudioTranscript(string transcript = null);
        public static ConversationContentPart FromOutputText(string text);
        public static implicit operator ConversationContentPart(string text);
        ConversationContentPart IJsonModel<ConversationContentPart>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationContentPart>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationContentPart IPersistableModel<ConversationContentPart>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationContentPart>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationContentPart>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct ConversationContentPartKind : IEquatable<ConversationContentPartKind> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
    public class ConversationErrorUpdate : ConversationUpdate, IJsonModel<ConversationErrorUpdate>, IPersistableModel<ConversationErrorUpdate> {
        public string ErrorCode { get; }
        public string ErrorEventId { get; }
        public string Message { get; }
        public string ParameterName { get; }
        ConversationErrorUpdate IJsonModel<ConversationErrorUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationErrorUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationErrorUpdate IPersistableModel<ConversationErrorUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationErrorUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationErrorUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationFunctionTool : ConversationTool, IJsonModel<ConversationFunctionTool>, IPersistableModel<ConversationFunctionTool> {
        public ConversationFunctionTool();
        public ConversationFunctionTool(string name);
        public string Description { get; set; }
        public required string Name { get; set; }
        public BinaryData Parameters { get; set; }
        ConversationFunctionTool IJsonModel<ConversationFunctionTool>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationFunctionTool>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationFunctionTool IPersistableModel<ConversationFunctionTool>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationFunctionTool>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationFunctionTool>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationInputAudioClearedUpdate : ConversationUpdate, IJsonModel<ConversationInputAudioClearedUpdate>, IPersistableModel<ConversationInputAudioClearedUpdate> {
        ConversationInputAudioClearedUpdate IJsonModel<ConversationInputAudioClearedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationInputAudioClearedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationInputAudioClearedUpdate IPersistableModel<ConversationInputAudioClearedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationInputAudioClearedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationInputAudioClearedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationInputAudioCommittedUpdate : ConversationUpdate, IJsonModel<ConversationInputAudioCommittedUpdate>, IPersistableModel<ConversationInputAudioCommittedUpdate> {
        public string ItemId { get; }
        public string PreviousItemId { get; }
        ConversationInputAudioCommittedUpdate IJsonModel<ConversationInputAudioCommittedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationInputAudioCommittedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationInputAudioCommittedUpdate IPersistableModel<ConversationInputAudioCommittedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationInputAudioCommittedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationInputAudioCommittedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationInputSpeechFinishedUpdate : ConversationUpdate, IJsonModel<ConversationInputSpeechFinishedUpdate>, IPersistableModel<ConversationInputSpeechFinishedUpdate> {
        public TimeSpan AudioEndTime { get; }
        public string ItemId { get; }
        ConversationInputSpeechFinishedUpdate IJsonModel<ConversationInputSpeechFinishedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationInputSpeechFinishedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationInputSpeechFinishedUpdate IPersistableModel<ConversationInputSpeechFinishedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationInputSpeechFinishedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationInputSpeechFinishedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationInputSpeechStartedUpdate : ConversationUpdate, IJsonModel<ConversationInputSpeechStartedUpdate>, IPersistableModel<ConversationInputSpeechStartedUpdate> {
        public TimeSpan AudioStartTime { get; }
        public string ItemId { get; }
        ConversationInputSpeechStartedUpdate IJsonModel<ConversationInputSpeechStartedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationInputSpeechStartedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationInputSpeechStartedUpdate IPersistableModel<ConversationInputSpeechStartedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationInputSpeechStartedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationInputSpeechStartedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationInputTokenUsageDetails : IJsonModel<ConversationInputTokenUsageDetails>, IPersistableModel<ConversationInputTokenUsageDetails> {
        public int AudioTokens { get; }
        public int CachedTokens { get; }
        public int TextTokens { get; }
        ConversationInputTokenUsageDetails IJsonModel<ConversationInputTokenUsageDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationInputTokenUsageDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationInputTokenUsageDetails IPersistableModel<ConversationInputTokenUsageDetails>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationInputTokenUsageDetails>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationInputTokenUsageDetails>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationInputTranscriptionFailedUpdate : ConversationUpdate, IJsonModel<ConversationInputTranscriptionFailedUpdate>, IPersistableModel<ConversationInputTranscriptionFailedUpdate> {
        public int ContentIndex { get; }
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public string ErrorParameterName { get; }
        public string ItemId { get; }
        ConversationInputTranscriptionFailedUpdate IJsonModel<ConversationInputTranscriptionFailedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationInputTranscriptionFailedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationInputTranscriptionFailedUpdate IPersistableModel<ConversationInputTranscriptionFailedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationInputTranscriptionFailedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationInputTranscriptionFailedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationInputTranscriptionFinishedUpdate : ConversationUpdate, IJsonModel<ConversationInputTranscriptionFinishedUpdate>, IPersistableModel<ConversationInputTranscriptionFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public string Transcript { get; }
        ConversationInputTranscriptionFinishedUpdate IJsonModel<ConversationInputTranscriptionFinishedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationInputTranscriptionFinishedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationInputTranscriptionFinishedUpdate IPersistableModel<ConversationInputTranscriptionFinishedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationInputTranscriptionFinishedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationInputTranscriptionFinishedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationInputTranscriptionOptions : IJsonModel<ConversationInputTranscriptionOptions>, IPersistableModel<ConversationInputTranscriptionOptions> {
        public ConversationTranscriptionModel? Model { get; set; }
        ConversationInputTranscriptionOptions IJsonModel<ConversationInputTranscriptionOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationInputTranscriptionOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationInputTranscriptionOptions IPersistableModel<ConversationInputTranscriptionOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationInputTranscriptionOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationInputTranscriptionOptions>.Write(ModelReaderWriterOptions options);
    }
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
        public static ConversationItem CreateSystemMessage(string toolCallId, IEnumerable<ConversationContentPart> contentItems);
        public static ConversationItem CreateUserMessage(IEnumerable<ConversationContentPart> contentItems);
        ConversationItem IJsonModel<ConversationItem>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItem>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItem IPersistableModel<ConversationItem>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItem>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItem>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationItemCreatedUpdate : ConversationUpdate, IJsonModel<ConversationItemCreatedUpdate>, IPersistableModel<ConversationItemCreatedUpdate> {
        public string FunctionCallArguments { get; }
        public string FunctionCallId { get; }
        public string FunctionCallOutput { get; }
        public string FunctionName { get; }
        public string ItemId { get; }
        public IReadOnlyList<ConversationContentPart> MessageContentParts { get; }
        public ConversationMessageRole? MessageRole { get; }
        public string PreviousItemId { get; }
        ConversationItemCreatedUpdate IJsonModel<ConversationItemCreatedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItemCreatedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItemCreatedUpdate IPersistableModel<ConversationItemCreatedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItemCreatedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItemCreatedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationItemDeletedUpdate : ConversationUpdate, IJsonModel<ConversationItemDeletedUpdate>, IPersistableModel<ConversationItemDeletedUpdate> {
        public string ItemId { get; }
        ConversationItemDeletedUpdate IJsonModel<ConversationItemDeletedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItemDeletedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItemDeletedUpdate IPersistableModel<ConversationItemDeletedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItemDeletedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItemDeletedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct ConversationItemStatus : IEquatable<ConversationItemStatus> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
    public class ConversationItemStreamingAudioFinishedUpdate : ConversationUpdate, IJsonModel<ConversationItemStreamingAudioFinishedUpdate>, IPersistableModel<ConversationItemStreamingAudioFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        ConversationItemStreamingAudioFinishedUpdate IJsonModel<ConversationItemStreamingAudioFinishedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItemStreamingAudioFinishedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItemStreamingAudioFinishedUpdate IPersistableModel<ConversationItemStreamingAudioFinishedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItemStreamingAudioFinishedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItemStreamingAudioFinishedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationItemStreamingAudioTranscriptionFinishedUpdate : ConversationUpdate, IJsonModel<ConversationItemStreamingAudioTranscriptionFinishedUpdate>, IPersistableModel<ConversationItemStreamingAudioTranscriptionFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        public string Transcript { get; }
        ConversationItemStreamingAudioTranscriptionFinishedUpdate IJsonModel<ConversationItemStreamingAudioTranscriptionFinishedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItemStreamingAudioTranscriptionFinishedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItemStreamingAudioTranscriptionFinishedUpdate IPersistableModel<ConversationItemStreamingAudioTranscriptionFinishedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItemStreamingAudioTranscriptionFinishedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItemStreamingAudioTranscriptionFinishedUpdate>.Write(ModelReaderWriterOptions options);
    }
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
        ConversationItemStreamingFinishedUpdate IJsonModel<ConversationItemStreamingFinishedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItemStreamingFinishedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItemStreamingFinishedUpdate IPersistableModel<ConversationItemStreamingFinishedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItemStreamingFinishedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItemStreamingFinishedUpdate>.Write(ModelReaderWriterOptions options);
    }
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
        ConversationItemStreamingPartDeltaUpdate IJsonModel<ConversationItemStreamingPartDeltaUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItemStreamingPartDeltaUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItemStreamingPartDeltaUpdate IPersistableModel<ConversationItemStreamingPartDeltaUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItemStreamingPartDeltaUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItemStreamingPartDeltaUpdate>.Write(ModelReaderWriterOptions options);
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
        ConversationItemStreamingPartFinishedUpdate IJsonModel<ConversationItemStreamingPartFinishedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItemStreamingPartFinishedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItemStreamingPartFinishedUpdate IPersistableModel<ConversationItemStreamingPartFinishedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItemStreamingPartFinishedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItemStreamingPartFinishedUpdate>.Write(ModelReaderWriterOptions options);
    }
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
        ConversationItemStreamingStartedUpdate IJsonModel<ConversationItemStreamingStartedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItemStreamingStartedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItemStreamingStartedUpdate IPersistableModel<ConversationItemStreamingStartedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItemStreamingStartedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItemStreamingStartedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationItemStreamingTextFinishedUpdate : ConversationUpdate, IJsonModel<ConversationItemStreamingTextFinishedUpdate>, IPersistableModel<ConversationItemStreamingTextFinishedUpdate> {
        public int ContentIndex { get; }
        public string ItemId { get; }
        public int OutputIndex { get; }
        public string ResponseId { get; }
        public string Text { get; }
        ConversationItemStreamingTextFinishedUpdate IJsonModel<ConversationItemStreamingTextFinishedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItemStreamingTextFinishedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItemStreamingTextFinishedUpdate IPersistableModel<ConversationItemStreamingTextFinishedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItemStreamingTextFinishedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItemStreamingTextFinishedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationItemTruncatedUpdate : ConversationUpdate, IJsonModel<ConversationItemTruncatedUpdate>, IPersistableModel<ConversationItemTruncatedUpdate> {
        public int AudioEndMs { get; }
        public int ContentIndex { get; }
        public string ItemId { get; }
        ConversationItemTruncatedUpdate IJsonModel<ConversationItemTruncatedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationItemTruncatedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationItemTruncatedUpdate IPersistableModel<ConversationItemTruncatedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationItemTruncatedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationItemTruncatedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationMaxTokensChoice : IJsonModel<ConversationMaxTokensChoice>, IPersistableModel<ConversationMaxTokensChoice> {
        public ConversationMaxTokensChoice(int numberValue);
        public int? NumericValue { get; }
        public static ConversationMaxTokensChoice CreateDefaultMaxTokensChoice();
        public static ConversationMaxTokensChoice CreateInfiniteMaxTokensChoice();
        public static ConversationMaxTokensChoice CreateNumericMaxTokensChoice(int maxTokens);
        public static implicit operator ConversationMaxTokensChoice(int maxTokens);
        ConversationMaxTokensChoice IJsonModel<ConversationMaxTokensChoice>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationMaxTokensChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationMaxTokensChoice IPersistableModel<ConversationMaxTokensChoice>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationMaxTokensChoice>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationMaxTokensChoice>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct ConversationMessageRole : IEquatable<ConversationMessageRole> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        public int AudioTokens { get; }
        public int TextTokens { get; }
        ConversationOutputTokenUsageDetails IJsonModel<ConversationOutputTokenUsageDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationOutputTokenUsageDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationOutputTokenUsageDetails IPersistableModel<ConversationOutputTokenUsageDetails>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationOutputTokenUsageDetails>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationOutputTokenUsageDetails>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationRateLimitDetailsItem : IJsonModel<ConversationRateLimitDetailsItem>, IPersistableModel<ConversationRateLimitDetailsItem> {
        public int MaximumCount { get; }
        public string Name { get; }
        public int RemainingCount { get; }
        public TimeSpan TimeUntilReset { get; }
        ConversationRateLimitDetailsItem IJsonModel<ConversationRateLimitDetailsItem>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationRateLimitDetailsItem>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationRateLimitDetailsItem IPersistableModel<ConversationRateLimitDetailsItem>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationRateLimitDetailsItem>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationRateLimitDetailsItem>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationRateLimitsUpdate : ConversationUpdate, IJsonModel<ConversationRateLimitsUpdate>, IPersistableModel<ConversationRateLimitsUpdate> {
        public IReadOnlyList<ConversationRateLimitDetailsItem> AllDetails { get; }
        public ConversationRateLimitDetailsItem RequestDetails { get; }
        public ConversationRateLimitDetailsItem TokenDetails { get; }
        ConversationRateLimitsUpdate IJsonModel<ConversationRateLimitsUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationRateLimitsUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationRateLimitsUpdate IPersistableModel<ConversationRateLimitsUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationRateLimitsUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationRateLimitsUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationResponseFinishedUpdate : ConversationUpdate, IJsonModel<ConversationResponseFinishedUpdate>, IPersistableModel<ConversationResponseFinishedUpdate> {
        public IReadOnlyList<ConversationItem> CreatedItems { get; }
        public string ResponseId { get; }
        public ConversationStatus? Status { get; }
        public ConversationStatusDetails StatusDetails { get; }
        public ConversationTokenUsage Usage { get; }
        ConversationResponseFinishedUpdate IJsonModel<ConversationResponseFinishedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationResponseFinishedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationResponseFinishedUpdate IPersistableModel<ConversationResponseFinishedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationResponseFinishedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationResponseFinishedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationResponseStartedUpdate : ConversationUpdate, IJsonModel<ConversationResponseStartedUpdate>, IPersistableModel<ConversationResponseStartedUpdate> {
        public IReadOnlyList<ConversationItem> CreatedItems { get; }
        public string ResponseId { get; }
        public ConversationStatus Status { get; }
        public ConversationStatusDetails StatusDetails { get; }
        public ConversationTokenUsage Usage { get; }
        ConversationResponseStartedUpdate IJsonModel<ConversationResponseStartedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationResponseStartedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationResponseStartedUpdate IPersistableModel<ConversationResponseStartedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationResponseStartedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationResponseStartedUpdate>.Write(ModelReaderWriterOptions options);
    }
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
        ConversationSessionConfiguredUpdate IJsonModel<ConversationSessionConfiguredUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationSessionConfiguredUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationSessionConfiguredUpdate IPersistableModel<ConversationSessionConfiguredUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationSessionConfiguredUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationSessionConfiguredUpdate>.Write(ModelReaderWriterOptions options);
    }
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
        ConversationSessionOptions IJsonModel<ConversationSessionOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationSessionOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationSessionOptions IPersistableModel<ConversationSessionOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationSessionOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationSessionOptions>.Write(ModelReaderWriterOptions options);
    }
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
        ConversationSessionStartedUpdate IJsonModel<ConversationSessionStartedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationSessionStartedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationSessionStartedUpdate IPersistableModel<ConversationSessionStartedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationSessionStartedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationSessionStartedUpdate>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct ConversationStatus : IEquatable<ConversationStatus> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
        public ConversationStatus(string value);
        public static ConversationStatus Cancelled { get; }
        public static ConversationStatus Completed { get; }
        public static ConversationStatus Failed { get; }
        public static ConversationStatus Incomplete { get; }
        public static ConversationStatus InProgress { get; }
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
    public abstract class ConversationStatusDetails : IJsonModel<ConversationStatusDetails>, IPersistableModel<ConversationStatusDetails> {
        public ConversationStatus StatusKind { get; }
        ConversationStatusDetails IJsonModel<ConversationStatusDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationStatusDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationStatusDetails IPersistableModel<ConversationStatusDetails>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationStatusDetails>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationStatusDetails>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationTokenUsage : IJsonModel<ConversationTokenUsage>, IPersistableModel<ConversationTokenUsage> {
        public ConversationInputTokenUsageDetails InputTokenDetails { get; }
        public int InputTokens { get; }
        public ConversationOutputTokenUsageDetails OutputTokenDetails { get; }
        public int OutputTokens { get; }
        public int TotalTokens { get; }
        ConversationTokenUsage IJsonModel<ConversationTokenUsage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationTokenUsage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationTokenUsage IPersistableModel<ConversationTokenUsage>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationTokenUsage>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationTokenUsage>.Write(ModelReaderWriterOptions options);
    }
    public abstract class ConversationTool : IJsonModel<ConversationTool>, IPersistableModel<ConversationTool> {
        public ConversationToolKind Kind { get; }
        public static ConversationTool CreateFunctionTool(string name, string description = null, BinaryData parameters = null);
        ConversationTool IJsonModel<ConversationTool>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationTool>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationTool IPersistableModel<ConversationTool>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationTool>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationTool>.Write(ModelReaderWriterOptions options);
    }
    public class ConversationToolChoice : IJsonModel<ConversationToolChoice>, IPersistableModel<ConversationToolChoice> {
        public string FunctionName { get; }
        public ConversationToolChoiceKind Kind { get; }
        public static ConversationToolChoice CreateAutoToolChoice();
        public static ConversationToolChoice CreateFunctionToolChoice(string functionName);
        public static ConversationToolChoice CreateNoneToolChoice();
        public static ConversationToolChoice CreateRequiredToolChoice();
        ConversationToolChoice IJsonModel<ConversationToolChoice>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationToolChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationToolChoice IPersistableModel<ConversationToolChoice>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationToolChoice>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationToolChoice>.Write(ModelReaderWriterOptions options);
    }
    public enum ConversationToolChoiceKind {
        Unknown = 0,
        Auto = 1,
        None = 2,
        Required = 3,
        Function = 4
    }
    public readonly partial struct ConversationToolKind : IEquatable<ConversationToolKind> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
    public readonly partial struct ConversationTranscriptionModel : IEquatable<ConversationTranscriptionModel> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
    public enum ConversationTurnDetectionKind {
        ServerVoiceActivityDetection = 0,
        Disabled = 1
    }
    public abstract class ConversationTurnDetectionOptions : IJsonModel<ConversationTurnDetectionOptions>, IPersistableModel<ConversationTurnDetectionOptions> {
        public ConversationTurnDetectionKind Kind { get; protected internal set; }
        public static ConversationTurnDetectionOptions CreateDisabledTurnDetectionOptions();
        public static ConversationTurnDetectionOptions CreateServerVoiceActivityTurnDetectionOptions(float? detectionThreshold = null, TimeSpan? prefixPaddingDuration = null, TimeSpan? silenceDuration = null);
        ConversationTurnDetectionOptions IJsonModel<ConversationTurnDetectionOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationTurnDetectionOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationTurnDetectionOptions IPersistableModel<ConversationTurnDetectionOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationTurnDetectionOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationTurnDetectionOptions>.Write(ModelReaderWriterOptions options);
    }
    public abstract class ConversationUpdate : IJsonModel<ConversationUpdate>, IPersistableModel<ConversationUpdate> {
        protected ConversationUpdate(string eventId);
        public string EventId { get; }
        public ConversationUpdateKind Kind { get; protected internal set; }
        public BinaryData GetRawContent();
        ConversationUpdate IJsonModel<ConversationUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<ConversationUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        ConversationUpdate IPersistableModel<ConversationUpdate>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<ConversationUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<ConversationUpdate>.Write(ModelReaderWriterOptions options);
    }
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
    public readonly partial struct ConversationVoice : IEquatable<ConversationVoice> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
        public ConversationVoice(string value);
        public static ConversationVoice Alloy { get; }
        public static ConversationVoice Echo { get; }
        public static ConversationVoice Shimmer { get; }
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
    public class RealtimeConversationClient {
        protected RealtimeConversationClient();
        protected internal RealtimeConversationClient(ClientPipeline pipeline, OpenAIClientOptions options);
        public RealtimeConversationClient(string model, ApiKeyCredential credential, OpenAIClientOptions options);
        public RealtimeConversationClient(string model, ApiKeyCredential credential);
        public virtual ClientPipeline Pipeline { get; }
        public event EventHandler<BinaryData> OnReceivingCommand { add; remove; }
        public event EventHandler<BinaryData> OnSendingCommand { add; remove; }
        public RealtimeConversationSession StartConversationSession(CancellationToken cancellationToken = default);
        public virtual Task<RealtimeConversationSession> StartConversationSessionAsync(RequestOptions options);
        public virtual Task<RealtimeConversationSession> StartConversationSessionAsync(CancellationToken cancellationToken = default);
    }
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
        public virtual void StartResponse(ConversationSessionOptions sessionOptionOverrides, CancellationToken cancellationToken = default);
        public virtual void StartResponse(CancellationToken cancellationToken = default);
        public virtual Task StartResponseAsync(ConversationSessionOptions sessionOptionOverrides, CancellationToken cancellationToken = default);
        public virtual Task StartResponseAsync(CancellationToken cancellationToken = default);
        public virtual void TruncateItem(string itemId, int contentPartIndex, TimeSpan audioDuration, CancellationToken cancellationToken = default);
        public virtual Task TruncateItemAsync(string itemId, int contentPartIndex, TimeSpan audioDuration, CancellationToken cancellationToken = default);
    }
}
namespace OpenAI.VectorStores {
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
    public abstract class FileChunkingStrategy : IJsonModel<FileChunkingStrategy>, IPersistableModel<FileChunkingStrategy> {
        public static FileChunkingStrategy Auto { get; }
        public static FileChunkingStrategy Unknown { get; }
        public static FileChunkingStrategy CreateStaticStrategy(int maxTokensPerChunk, int overlappingTokenCount);
        FileChunkingStrategy IJsonModel<FileChunkingStrategy>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<FileChunkingStrategy>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        FileChunkingStrategy IPersistableModel<FileChunkingStrategy>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<FileChunkingStrategy>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<FileChunkingStrategy>.Write(ModelReaderWriterOptions options);
    }
    public class FileFromStoreRemovalResult : IJsonModel<FileFromStoreRemovalResult>, IPersistableModel<FileFromStoreRemovalResult> {
        public string FileId { get; }
        public bool Removed { get; }
        FileFromStoreRemovalResult IJsonModel<FileFromStoreRemovalResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<FileFromStoreRemovalResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        FileFromStoreRemovalResult IPersistableModel<FileFromStoreRemovalResult>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<FileFromStoreRemovalResult>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<FileFromStoreRemovalResult>.Write(ModelReaderWriterOptions options);
    }
    public class StaticFileChunkingStrategy : FileChunkingStrategy, IJsonModel<StaticFileChunkingStrategy>, IPersistableModel<StaticFileChunkingStrategy> {
        public StaticFileChunkingStrategy(int maxTokensPerChunk, int overlappingTokenCount);
        public int MaxTokensPerChunk { get; }
        public int OverlappingTokenCount { get; }
        StaticFileChunkingStrategy IJsonModel<StaticFileChunkingStrategy>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<StaticFileChunkingStrategy>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        StaticFileChunkingStrategy IPersistableModel<StaticFileChunkingStrategy>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<StaticFileChunkingStrategy>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<StaticFileChunkingStrategy>.Write(ModelReaderWriterOptions options);
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
        VectorStore IJsonModel<VectorStore>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<VectorStore>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        VectorStore IPersistableModel<VectorStore>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<VectorStore>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<VectorStore>.Write(ModelReaderWriterOptions options);
    }
    public class VectorStoreBatchFileJob : IJsonModel<VectorStoreBatchFileJob>, IPersistableModel<VectorStoreBatchFileJob> {
        public string BatchId { get; }
        public DateTimeOffset CreatedAt { get; }
        public VectorStoreFileCounts FileCounts { get; }
        public VectorStoreBatchFileJobStatus Status { get; }
        public string VectorStoreId { get; }
        VectorStoreBatchFileJob IJsonModel<VectorStoreBatchFileJob>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<VectorStoreBatchFileJob>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        VectorStoreBatchFileJob IPersistableModel<VectorStoreBatchFileJob>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<VectorStoreBatchFileJob>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<VectorStoreBatchFileJob>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct VectorStoreBatchFileJobStatus : IEquatable<VectorStoreBatchFileJobStatus> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
    public class VectorStoreCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public VectorStoreCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    public readonly partial struct VectorStoreCollectionOrder : IEquatable<VectorStoreCollectionOrder> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        public IDictionary<string, string> Metadata { get; set; }
        public string Name { get; set; }
        VectorStoreCreationOptions IJsonModel<VectorStoreCreationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<VectorStoreCreationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        VectorStoreCreationOptions IPersistableModel<VectorStoreCreationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<VectorStoreCreationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<VectorStoreCreationOptions>.Write(ModelReaderWriterOptions options);
    }
    public class VectorStoreDeletionResult : IJsonModel<VectorStoreDeletionResult>, IPersistableModel<VectorStoreDeletionResult> {
        public bool Deleted { get; }
        public string VectorStoreId { get; }
        VectorStoreDeletionResult IJsonModel<VectorStoreDeletionResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<VectorStoreDeletionResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        VectorStoreDeletionResult IPersistableModel<VectorStoreDeletionResult>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<VectorStoreDeletionResult>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<VectorStoreDeletionResult>.Write(ModelReaderWriterOptions options);
    }
    public enum VectorStoreExpirationAnchor {
        Unknown = 0,
        LastActiveAt = 1
    }
    public class VectorStoreExpirationPolicy : IJsonModel<VectorStoreExpirationPolicy>, IPersistableModel<VectorStoreExpirationPolicy> {
        public VectorStoreExpirationPolicy();
        public VectorStoreExpirationPolicy(VectorStoreExpirationAnchor anchor, int days);
        public required VectorStoreExpirationAnchor Anchor { get; set; }
        public required int Days { get; set; }
        VectorStoreExpirationPolicy IJsonModel<VectorStoreExpirationPolicy>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<VectorStoreExpirationPolicy>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        VectorStoreExpirationPolicy IPersistableModel<VectorStoreExpirationPolicy>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<VectorStoreExpirationPolicy>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<VectorStoreExpirationPolicy>.Write(ModelReaderWriterOptions options);
    }
    public class VectorStoreFileAssociation : IJsonModel<VectorStoreFileAssociation>, IPersistableModel<VectorStoreFileAssociation> {
        public FileChunkingStrategy ChunkingStrategy { get; }
        public DateTimeOffset CreatedAt { get; }
        public string FileId { get; }
        public VectorStoreFileAssociationError LastError { get; }
        public int Size { get; }
        public VectorStoreFileAssociationStatus Status { get; }
        public string VectorStoreId { get; }
        VectorStoreFileAssociation IJsonModel<VectorStoreFileAssociation>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<VectorStoreFileAssociation>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        VectorStoreFileAssociation IPersistableModel<VectorStoreFileAssociation>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<VectorStoreFileAssociation>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<VectorStoreFileAssociation>.Write(ModelReaderWriterOptions options);
    }
    public class VectorStoreFileAssociationCollectionOptions {
        public string AfterId { get; set; }
        public string BeforeId { get; set; }
        public VectorStoreFileStatusFilter? Filter { get; set; }
        public VectorStoreFileAssociationCollectionOrder? Order { get; set; }
        public int? PageSizeLimit { get; set; }
    }
    public readonly partial struct VectorStoreFileAssociationCollectionOrder : IEquatable<VectorStoreFileAssociationCollectionOrder> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        VectorStoreFileAssociationError IJsonModel<VectorStoreFileAssociationError>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<VectorStoreFileAssociationError>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        VectorStoreFileAssociationError IPersistableModel<VectorStoreFileAssociationError>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<VectorStoreFileAssociationError>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<VectorStoreFileAssociationError>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct VectorStoreFileAssociationErrorCode : IEquatable<VectorStoreFileAssociationErrorCode> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        VectorStoreFileCounts IJsonModel<VectorStoreFileCounts>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<VectorStoreFileCounts>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        VectorStoreFileCounts IPersistableModel<VectorStoreFileCounts>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<VectorStoreFileCounts>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<VectorStoreFileCounts>.Write(ModelReaderWriterOptions options);
    }
    public readonly partial struct VectorStoreFileStatusFilter : IEquatable<VectorStoreFileStatusFilter> {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;
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
        public IDictionary<string, string> Metadata { get; set; }
        public string Name { get; set; }
        VectorStoreModificationOptions IJsonModel<VectorStoreModificationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options);
        void IJsonModel<VectorStoreModificationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options);
        VectorStoreModificationOptions IPersistableModel<VectorStoreModificationOptions>.Create(BinaryData data, ModelReaderWriterOptions options);
        string IPersistableModel<VectorStoreModificationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options);
        BinaryData IPersistableModel<VectorStoreModificationOptions>.Write(ModelReaderWriterOptions options);
    }
    public enum VectorStoreStatus {
        Unknown = 0,
        InProgress = 1,
        Completed = 2,
        Expired = 3
    }
}