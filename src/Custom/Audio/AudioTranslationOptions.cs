using OpenAI.Embeddings;
using OpenAI.Images;
using OpenAI.Internal;
using System;
using System.IO;

namespace OpenAI.Audio;

[CodeGenModel("CreateTranslationRequest")]
[CodeGenSuppress("AudioTranslationOptions", typeof(BinaryData), typeof(InternalCreateTranslationRequestModel))]
public partial class AudioTranslationOptions
{
    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    /// <summary>
    /// The audio file object (not file name) to transcribe, in one of these formats: flac, mp3, mp4,
    /// mpeg, mpga, m4a, ogg, pcm, wav, or webm.
    /// <para>
    /// To assign a byte[] to this property use <see cref="BinaryData.FromBytes(byte[])"/>.
    /// The byte[] will be serialized to a Base64 encoded string.
    /// </para>
    /// <para>
    /// Examples:
    /// <list type="bullet">
    /// <item>
    /// <term>BinaryData.FromBytes(new byte[] { 1, 2, 3 })</term>
    /// <description>Creates a payload of "AQID".</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    internal BinaryData File { get; }

    // CUSTOM:
    // - Made internal. The model is specified by the client.
    // - Added setter.
    /// <summary>
    /// ID of the model to use. Only `whisper-1` (which is powered by our open source Whisper V2 model)
    /// is currently available.
    /// </summary>
    internal InternalCreateTranslationRequestModel Model { get; set; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="AudioTranslationOptions"/>. </summary>
    public AudioTranslationOptions()
    {
    }

    internal MultipartFormDataBinaryContent ToMultipartContent(Stream audio, string audioFilename)
    {
        MultipartFormDataBinaryContent content = new();

        content.Add(audio, "file", audioFilename);
        content.Add(Model.ToString(), "model");

        if (Prompt is not null)
        {
            content.Add(Prompt, "prompt");
        }

        if (ResponseFormat is not null)
        {
            string value = ResponseFormat switch
            {
                AudioTranslationFormat.Simple => "json",
                AudioTranslationFormat.Verbose => "verbose_json",
                AudioTranslationFormat.Srt => "srt",
                AudioTranslationFormat.Vtt => "vtt",
                _ => throw new ArgumentException(nameof(ResponseFormat))
            };

            content.Add(value, "response_format");
        }

        return content;
    }
}