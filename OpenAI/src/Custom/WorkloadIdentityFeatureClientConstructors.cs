namespace OpenAI.Assistants
{
    public partial class AssistantClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public AssistantClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public AssistantClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Audio
{
    public partial class AudioClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public AudioClient(string model, WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(model, workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public AudioClient(string model, WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(model, OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Batch
{
    public partial class BatchClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public BatchClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public BatchClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Chat
{
    public partial class ChatClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public ChatClient(string model, WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(model, workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public ChatClient(string model, WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(model, OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Containers
{
    public partial class ContainerClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public ContainerClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public ContainerClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Conversations
{
    public partial class ConversationClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public ConversationClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public ConversationClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Embeddings
{
    public partial class EmbeddingClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public EmbeddingClient(string model, WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(model, workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public EmbeddingClient(string model, WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(model, OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Evals
{
    public partial class EvaluationClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public EvaluationClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public EvaluationClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Files
{
    public partial class OpenAIFileClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public OpenAIFileClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public OpenAIFileClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.FineTuning
{
    public partial class FineTuningClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public FineTuningClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public FineTuningClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Graders
{
    public partial class GraderClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public GraderClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public GraderClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Images
{
    public partial class ImageClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public ImageClient(string model, WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(model, workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public ImageClient(string model, WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(model, OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Models
{
    public partial class OpenAIModelClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public OpenAIModelClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public OpenAIModelClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Moderations
{
    public partial class ModerationClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public ModerationClient(string model, WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(model, workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public ModerationClient(string model, WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(model, OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Realtime
{
    public partial class RealtimeClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public RealtimeClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new RealtimeClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public RealtimeClient(WorkloadIdentityFederationOptions workloadIdentityOptions, RealtimeClientOptions options)
            : this(
                new WorkloadIdentityAuthenticationTokenProvider(
                    workloadIdentityOptions ?? throw new System.ArgumentNullException(nameof(workloadIdentityOptions)),
                    options ??= new RealtimeClientOptions()),
                options) { }
    }
}

namespace OpenAI.Responses
{
    public partial class ResponsesClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public ResponsesClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new ResponsesClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public ResponsesClient(WorkloadIdentityFederationOptions workloadIdentityOptions, ResponsesClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new ResponsesClientOptions()), options) { }
    }
}

namespace OpenAI.Skills
{
    public partial class SkillClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public SkillClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public SkillClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.VectorStores
{
    public partial class VectorStoreClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public VectorStoreClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public VectorStoreClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}

namespace OpenAI.Videos
{
    public partial class VideoClient
    {
        /// <summary>Initializes a client using workload identity federation.</summary>
        public VideoClient(WorkloadIdentityFederationOptions workloadIdentityOptions)
            : this(workloadIdentityOptions, new OpenAIClientOptions()) { }

        /// <summary>Initializes a client using workload identity federation.</summary>
        public VideoClient(WorkloadIdentityFederationOptions workloadIdentityOptions, OpenAIClientOptions options)
            : this(OpenAIClient.CreateWorkloadIdentityAuthenticationPolicy(workloadIdentityOptions, options ??= new OpenAIClientOptions()), options) { }
    }
}
