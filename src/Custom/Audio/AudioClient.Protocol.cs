using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenAI.Audio;

[CodeGenSuppress("CreateSpeechAsync", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateSpeech", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateTranscriptionAsync", typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CreateTranscription", typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CreateTranslationAsync", typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CreateTranslation", typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
public partial class AudioClient
{
    // CUSTOM:
    // - Renamed.
    // - Edited the cref in the doc comment to point to the correct convenience overload after it was also renamed.
    // - Added the EditorBrowsable attribute to hide protocol methods from IntelliSense when a convenience overload is available.
    /// <summary>
    /// [Protocol Method] Generates text-to-speech audio using the specified voice speaking the provided input text.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> GenerateSpeechAsync(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateSpeechRequest(content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited the cref in the doc comment to point to the correct convenience overload after it was also renamed.
    // - Added the EditorBrowsable attribute to hide protocol methods from IntelliSense when a convenience overload is available.
    /// <summary>
    /// [Protocol Method] Generates text-to-speech audio using the specified voice speaking the provided input text.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult GenerateSpeech(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateSpeechRequest(content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited the cref in the doc comment to point to the correct convenience overload after it was also renamed.
    // - Added the EditorBrowsable attribute to hide protocol methods from IntelliSense when a convenience overload is available.
    // - Added "contentType" parameter.
    /// <summary>
    /// [Protocol Method] Transcribes audio.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="contentType"> The content type of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> or <paramref name="contentType"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="contentType"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> TranscribeAudioAsync(BinaryContent content, string contentType, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));
        Argument.AssertNotNullOrEmpty(contentType, nameof(contentType));

        using PipelineMessage message = CreateCreateTranscriptionRequest(content, contentType, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited the cref in the doc comment to point to the correct convenience overload after it was also renamed.
    // - Added the EditorBrowsable attribute to hide protocol methods from IntelliSense when a convenience overload is available.
    // - Added "contentType" parameter.
    /// <summary>
    /// [Protocol Method] Transcribes audio.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="contentType"> The content type of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> or <paramref name="contentType"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="contentType"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult TranscribeAudio(BinaryContent content, string contentType, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));
        Argument.AssertNotNullOrEmpty(contentType, nameof(contentType));

        using PipelineMessage message = CreateCreateTranscriptionRequest(content, contentType, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited the cref in the doc comment to point to the correct convenience overload after it was also renamed.
    // - Added the EditorBrowsable attribute to hide protocol methods from IntelliSense when a convenience overload is available.
    // - Added "contentType" parameter.
    /// <summary>
    /// [Protocol Method] Translates audio into English.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="contentType"> The content type of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> or <paramref name="contentType"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="contentType"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> TranslateAudioAsync(BinaryContent content, string contentType, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));
        Argument.AssertNotNullOrEmpty(contentType, nameof(contentType));

        using PipelineMessage message = CreateCreateTranslationRequest(content, contentType, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited the cref in the doc comment to point to the correct convenience overload after it was also renamed.
    // - Added the EditorBrowsable attribute to hide protocol methods from IntelliSense when a convenience overload is available.
    // - Added "contentType" parameter.
    /// <summary>
    /// [Protocol Method] Translates audio into English.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="contentType"> The content type of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> or <paramref name="contentType"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="contentType"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult TranslateAudio(BinaryContent content, string contentType, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));
        Argument.AssertNotNullOrEmpty(contentType, nameof(contentType));

        using PipelineMessage message = CreateCreateTranslationRequest(content, contentType, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }
}