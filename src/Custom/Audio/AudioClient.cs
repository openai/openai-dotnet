using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Audio;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed methods that only take the options parameter.
/// <summary> The service client for OpenAI audio operations. </summary>
[CodeGenType("Audio")]
[CodeGenSuppress("AudioClient", typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress("GenerateSpeechAsync", typeof(SpeechGenerationOptions), typeof(CancellationToken))]
[CodeGenSuppress("GenerateSpeech", typeof(SpeechGenerationOptions), typeof(CancellationToken))]
public partial class AudioClient
{
    private readonly string _model;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="AudioClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="apiKey"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public AudioClient(string model, string apiKey) : this(model, new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="AudioClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public AudioClient(string model, ApiKeyCredential credential) : this(model, credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="AudioClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public AudioClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="AudioClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> or <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    protected internal AudioClient(ClientPipeline pipeline, string model, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    #region GenerateSpeech

    /// <summary> Generates a life-like, spoken audio recording of the input text. </summary>
    /// <remarks>
    ///     The default format of the generated audio is <see cref="GeneratedSpeechFormat.Mp3"/> unless otherwise specified
    ///     via <see cref="SpeechGenerationOptions.ResponseFormat"/>.
    /// </remarks>
    /// <param name="text"> The text to generate audio for. </param>
    /// <param name="voice"> The voice to use in the generated audio. </param>
    /// <param name="options"> The options to configure the audio generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="text"/> is null. </exception>
    /// <returns> The generated audio in the specified output format. </returns>
    public virtual async Task<ClientResult<BinaryData>> GenerateSpeechAsync(string text, GeneratedSpeechVoice voice, SpeechGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(text, nameof(text));

        options ??= new();
        CreateSpeechGenerationOptions(text, voice, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = await GenerateSpeechAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(result.GetRawResponse().Content, result.GetRawResponse());
    }

    /// <summary> Generates a life-like, spoken audio recording of the input text. </summary>
    /// <remarks>
    ///     The default format of the generated audio is <see cref="GeneratedSpeechFormat.Mp3"/> unless otherwise specified
    ///     via <see cref="SpeechGenerationOptions.ResponseFormat"/>.
    /// </remarks>
    /// <param name="text"> The text to generate audio for. </param>
    /// <param name="voice"> The voice to use in the generated audio. </param>
    /// <param name="options"> The options to configure the audio generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="text"/> is null. </exception>
    /// <returns> The generated audio in the specified output format. </returns>
    public virtual ClientResult<BinaryData> GenerateSpeech(string text, GeneratedSpeechVoice voice, SpeechGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(text, nameof(text));

        options ??= new();
        CreateSpeechGenerationOptions(text, voice, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = GenerateSpeech(content, cancellationToken.ToRequestOptions()); ;
        return ClientResult.FromValue(result.GetRawResponse().Content, result.GetRawResponse());
    }

    #endregion

    #region TranscribeAudio

    /// <summary> Transcribes the input audio. </summary>
    /// <param name="audio"> The audio stream to transcribe. </param>
    /// <param name="audioFilename">
    ///     The filename associated with the audio stream. The filename's extension (for example: .mp3) will be used to
    ///     validate the format of the input audio. The request may fail if the filename's extension and the actual
    ///     format of the input audio do not match.
    /// </param>
    /// <param name="options"> The options to configure the audio transcription. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="audio"/> or <paramref name="audioFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="audioFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<AudioTranscription>> TranscribeAudioAsync(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        Argument.AssertNotNullOrEmpty(audioFilename, nameof(audioFilename));

        using MultiPartFormDataBinaryContent content
            = CreatePerCallTranscriptionOptions(options)
                .ToMultipartContent(audio, audioFilename);

        ClientResult result = await TranscribeAudioAsync(content, content.ContentType, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(AudioTranscription.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Transcribes the input audio. </summary>
    /// <param name="audio"> The audio stream to transcribe. </param>
    /// <param name="audioFilename">
    ///     The filename associated with the audio stream. The filename's extension (for example: .mp3) will be used to
    ///     validate the format of the input audio. The request may fail if the filename's extension and the actual
    ///     format of the input audio do not match.
    /// </param>
    /// <param name="options"> The options to configure the audio transcription. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="audio"/> or <paramref name="audioFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="audioFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<AudioTranscription> TranscribeAudio(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        Argument.AssertNotNullOrEmpty(audioFilename, nameof(audioFilename));

        using MultiPartFormDataBinaryContent content
            = CreatePerCallTranscriptionOptions(options)
                .ToMultipartContent(audio, audioFilename);

        ClientResult result = TranscribeAudio(content, content.ContentType, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(AudioTranscription.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Transcribes the input audio. </summary>
    /// <param name="audioFilePath">
    ///     The path of the audio file to transcribe. The provided file path's extension (for example: .mp3) will be
    ///     used to validate the format of the input audio. The request may fail if the file path's extension and the
    ///     actual format of the input audio do not match.
    /// </param>
    /// <param name="options"> The options to configure the audio transcription. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="audioFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="audioFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<AudioTranscription>> TranscribeAudioAsync(string audioFilePath, AudioTranscriptionOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(audioFilePath, nameof(audioFilePath));

        using FileStream audioStream = File.OpenRead(audioFilePath);
        return await TranscribeAudioAsync(audioStream, audioFilePath, options).ConfigureAwait(false);
    }

    /// <summary> Transcribes the input audio. </summary>
    /// <param name="audioFilePath">
    ///     The path of the audio file to transcribe. The provided file path's extension (for example: .mp3) will be
    ///     used to validate the format of the input audio. The request may fail if the file path's extension and the
    ///     actual format of the input audio do not match.
    /// </param>
    /// <param name="options"> The options to configure the audio transcription. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="audioFilePath"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="audioFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<AudioTranscription> TranscribeAudio(string audioFilePath, AudioTranscriptionOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(audioFilePath, nameof(audioFilePath));

        using FileStream audioStream = File.OpenRead(audioFilePath);
        return TranscribeAudio(audioStream, audioFilePath, options);
    }

    // CUSTOM: Added Experimental attribute.
    [Experimental("OPENAI001")]
    public virtual AsyncCollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreamingAsync(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        Argument.AssertNotNullOrEmpty(audioFilename, nameof(audioFilename));

        EnsureModelSupportsStreaming();

        MultiPartFormDataBinaryContent content
            = CreatePerCallTranscriptionOptions(options, stream: true)
                .ToMultipartContent(audio, audioFilename);

        return new AsyncSseUpdateCollection<StreamingAudioTranscriptionUpdate>(
            async () => await TranscribeAudioAsync(content, content.ContentType, cancellationToken.ToRequestOptions(streaming: true)).ConfigureAwait(false),
            StreamingAudioTranscriptionUpdate.DeserializeStreamingAudioTranscriptionUpdate,
            cancellationToken);
    }

    // CUSTOM: Added Experimental attribute.
    [Experimental("OPENAI001")]
    public virtual AsyncCollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreamingAsync(string audioFilePath, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(audioFilePath, nameof(audioFilePath));

        EnsureModelSupportsStreaming();

        FileStream inputStream = File.OpenRead(audioFilePath);

        MultiPartFormDataBinaryContent content
            = CreatePerCallTranscriptionOptions(options, stream: true)
                .ToMultipartContent(inputStream, audioFilePath);

        AsyncSseUpdateCollection<StreamingAudioTranscriptionUpdate> result = new(
            async () => await TranscribeAudioAsync(content, content.ContentType, cancellationToken.ToRequestOptions(streaming: true)).ConfigureAwait(false),
            StreamingAudioTranscriptionUpdate.DeserializeStreamingAudioTranscriptionUpdate,
            cancellationToken);
        result.AdditionalDisposalActions.Add(() => inputStream?.Dispose());
        return result;
    }

    // CUSTOM: Added Experimental attribute.
    [Experimental("OPENAI001")]
    public virtual CollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreaming(Stream audio, string audioFilename, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        Argument.AssertNotNullOrEmpty(audioFilename, nameof(audioFilename));

        EnsureModelSupportsStreaming();

        MultiPartFormDataBinaryContent content
            = CreatePerCallTranscriptionOptions(options, stream: true)
                .ToMultipartContent(audio, audioFilename);

        return new SseUpdateCollection<StreamingAudioTranscriptionUpdate>(
            () => TranscribeAudio(content, content.ContentType, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingAudioTranscriptionUpdate.DeserializeStreamingAudioTranscriptionUpdate,
            cancellationToken);
    }

    // CUSTOM: Added Experimental attribute.
    [Experimental("OPENAI001")]
    public virtual CollectionResult<StreamingAudioTranscriptionUpdate> TranscribeAudioStreaming(string audioFilePath, AudioTranscriptionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(audioFilePath, nameof(audioFilePath));

        EnsureModelSupportsStreaming();

        FileStream inputStream = File.OpenRead(audioFilePath);

        MultiPartFormDataBinaryContent content
            = CreatePerCallTranscriptionOptions(options, stream: true)
                .ToMultipartContent(inputStream, audioFilePath);

        SseUpdateCollection<StreamingAudioTranscriptionUpdate> result = new(
            () => TranscribeAudio(content, content.ContentType, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingAudioTranscriptionUpdate.DeserializeStreamingAudioTranscriptionUpdate,
            cancellationToken);
        result.AdditionalDisposalActions.Add(() => inputStream?.Dispose());
        return result;
    }

    private void EnsureModelSupportsStreaming()
    {
        if (string.Equals(_model, "whisper-1", StringComparison.OrdinalIgnoreCase))
        {
            string isEnabled = Environment.GetEnvironmentVariable("OPENAI_ENABLE_WHISPER_1_STREAMING");
            if (!string.Equals(isEnabled, "true", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException(
                    "The selected model 'whisper-1' does not support streaming transcription. " +
                    "Please use a compatible model or set the environment variable 'OPENAI_ENABLE_WHISPER_1_STREAMING=true' to bypass this check.");
            }
        }
    }

    #endregion

    #region TranslateAudio

    /// <summary> Translates the input audio into English. </summary>
    /// <param name="audio"> The audio stream to translate. </param>
    /// <param name="audioFilename">
    ///     The filename associated with the audio stream. The filename's extension (for example: .mp3) will be used to
    ///     validate the format of the input audio. The request may fail if the filename's extension and the actual
    ///     format of the input audio do not match.
    /// </param>
    /// <param name="options"> The options to configure the audio translation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="audio"/> or <paramref name="audioFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="audioFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<AudioTranslation>> TranslateAudioAsync(Stream audio, string audioFilename, AudioTranslationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        Argument.AssertNotNullOrEmpty(audioFilename, nameof(audioFilename));

        options ??= new();
        CreateAudioTranslationOptions(audio, audioFilename, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(audio, audioFilename);
        ClientResult result = await TranslateAudioAsync(content, content.ContentType, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(AudioTranslation.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Translates the input audio into English. </summary>
    /// <param name="audio"> The audio stream to translate. </param>
    /// <param name="audioFilename">
    ///     The filename associated with the audio stream. The filename's extension (for example: .mp3) will be used to
    ///     validate the format of the input audio. The request may fail if the filename's extension and the actual
    ///     format of the input audio do not match.
    /// </param>
    /// <param name="options"> The options to configure the audio translation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="audio"/> or <paramref name="audioFilename"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="audioFilename"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<AudioTranslation> TranslateAudio(Stream audio, string audioFilename, AudioTranslationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        Argument.AssertNotNullOrEmpty(audioFilename, nameof(audioFilename));

        options ??= new();
        CreateAudioTranslationOptions(audio, audioFilename, ref options);

        using MultiPartFormDataBinaryContent content = options.ToMultipartContent(audio, audioFilename);
        ClientResult result = TranslateAudio(content, content.ContentType, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(AudioTranslation.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Translates the input audio into English. </summary>
    /// <param name="audioFilePath">
    ///     The path of the audio file to translate. The provided file path's extension (for example: .mp3) will be
    ///     used to validate the format of the input audio. The request may fail if the file path's extension and the
    ///     actual format of the input audio do not match.
    /// </param>
    /// <param name="options"> The options to configure the audio translation. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="audioFilePath"/> was null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="audioFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<AudioTranslation> TranslateAudio(string audioFilePath, AudioTranslationOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(audioFilePath, nameof(audioFilePath));

        using FileStream audioStream = File.OpenRead(audioFilePath);
        return TranslateAudio(audioStream, audioFilePath, options);
    }

    /// <summary> Translates the input audio into English. </summary>
    /// <param name="audioFilePath">
    ///     The path of the audio file to translate. The provided file path's extension (for example: .mp3) will be
    ///     used to validate the format of the input audio. The request may fail if the file path's extension and the
    ///     actual format of the input audio do not match.
    /// </param>
    /// <param name="options"> The options to configure the audio translation. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="audioFilePath"/> was null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="audioFilePath"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<AudioTranslation>> TranslateAudioAsync(string audioFilePath, AudioTranslationOptions options = null)
    {
        Argument.AssertNotNull(audioFilePath, nameof(audioFilePath));

        using FileStream audioStream = File.OpenRead(audioFilePath);
        return await TranslateAudioAsync(audioStream, audioFilePath, options);
    }

    #endregion

    private void CreateSpeechGenerationOptions(string text, GeneratedSpeechVoice voice, ref SpeechGenerationOptions options)
    {
        options.Input = text;
        options.Voice = voice;
        options.Model = _model;
    }

    internal virtual AudioTranscriptionOptions CreatePerCallTranscriptionOptions(AudioTranscriptionOptions userOptions, bool stream = false)
    {
        AudioTranscriptionOptions copiedOptions = userOptions is null ? new() : userOptions.GetClone();

        copiedOptions.Model = _model;

        if (stream)
        {
            copiedOptions.Stream = true;
        }

        return copiedOptions;
    }

    private void CreateAudioTranslationOptions(Stream audio, string audioFilename, ref AudioTranslationOptions options)
    {
        options.Model = _model;
    }
}
