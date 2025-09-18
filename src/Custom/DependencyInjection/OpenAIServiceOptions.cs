namespace OpenAI.Extensions.DependencyInjection;

/// <summary>
/// Configuration options for OpenAI client services when using dependency injection.
/// This extends the base OpenAIClientOptions with DI-specific settings.
/// </summary>
public class OpenAIServiceOptions : OpenAIClientOptions
{
    /// <summary>
    /// The OpenAI API key. If not provided, the OPENAI_API_KEY environment variable will be used.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The default chat model to use when registering ChatClient without specifying a model.
    /// </summary>
    public string DefaultChatModel { get; set; } = "gpt-4o";

    /// <summary>
    /// The default embedding model to use when registering EmbeddingClient without specifying a model.
    /// </summary>
    public string DefaultEmbeddingModel { get; set; } = "text-embedding-3-small";

    /// <summary>
    /// The default audio model to use when registering AudioClient without specifying a model.
    /// </summary>
    public string DefaultAudioModel { get; set; } = "whisper-1";

    /// <summary>
    /// The default image model to use when registering ImageClient without specifying a model.
    /// </summary>
    public string DefaultImageModel { get; set; } = "dall-e-3";

    /// <summary>
    /// The default moderation model to use when registering ModerationClient without specifying a model.
    /// </summary>
    public string DefaultModerationModel { get; set; } = "text-moderation-latest";
}