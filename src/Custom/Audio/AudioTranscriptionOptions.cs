using OpenAI.Internal;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenAI.Audio;

[CodeGenModel("CreateTranscriptionRequest")]
[CodeGenSuppress("AudioTranscriptionOptions", typeof(BinaryData), typeof(InternalCreateTranscriptionRequestModel))]
public partial class AudioTranscriptionOptions
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
    internal InternalCreateTranscriptionRequestModel Model { get; set; }

    // CUSTOM: Made internal. The model is specified by the client.
    /// <summary>
    /// The timestamp granularities to populate for this transcription. `response_format` must be set
    /// `verbose_json` to use timestamp granularities. Either or both of these options are supported:
    /// `word`, or `segment`. Note: There is no additional latency for segment timestamps, but
    /// generating word timestamps incurs additional latency.
    /// <para>
    /// To assign an object to the element of this property use <see cref="BinaryData.FromObjectAsJson{T}(T, System.Text.Json.JsonSerializerOptions?)"/>.
    /// </para>
    /// <para>
    /// To assign an already formatted json string to this property use <see cref="BinaryData.FromString(string)"/>.
    /// </para>
    /// <para>
    /// Examples:
    /// <list type="bullet">
    /// <item>
    /// <term>BinaryData.FromObjectAsJson("foo")</term>
    /// <description>Creates a payload of "foo".</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromString("\"foo\"")</term>
    /// <description>Creates a payload of "foo".</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromObjectAsJson(new { key = "value" })</term>
    /// <description>Creates a payload of { "key": "value" }.</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromString("{\"key\": \"value\"}")</term>
    /// <description>Creates a payload of { "key": "value" }.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    internal IList<BinaryData> TimestampGranularities { get; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="AudioTranscriptionOptions"/>. </summary>
    public AudioTranscriptionOptions()
    {
    }

    /// <summary>
    /// The timestamp granularities to populate for this transcription.
    /// </summary>
    public AudioTimestampGranularities Granularities { get; set; }

    internal MultipartFormDataBinaryContent ToMultipartContent(Stream audio, string audioFilename)
    {
        MultipartFormDataBinaryContent content = new();

        content.Add(audio, "file", audioFilename);
        content.Add(Model.ToString(), "model");

        if (Language is not null)
        {
            content.Add(Language, "language");
        }

        if (Prompt is not null)
        {
            content.Add(Prompt, "prompt");
        }

        if (ResponseFormat is not null)
        {
            string value = ResponseFormat switch
            {
                AudioTranscriptionFormat.Simple => "json",
                AudioTranscriptionFormat.Verbose => "verbose_json",
                AudioTranscriptionFormat.Srt => "srt",
                AudioTranscriptionFormat.Vtt => "vtt",
                _ => throw new ArgumentException(nameof(ResponseFormat))
            };

            content.Add(value, "response_format");
        }

        if (Temperature is not null)
        {
            content.Add(Temperature.Value, "temperature");
        }

        if (Granularities.HasFlag(AudioTimestampGranularities.Word))
        {
            content.Add("word", "timestamp_granularities[]");
        }

        if (Granularities.HasFlag(AudioTimestampGranularities.Segment))
        {
            content.Add("segment", "timestamp_granularities[]");
        }

        return content;
    }
}
