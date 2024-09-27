using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace OpenAI.Audio;

[CodeGenModel("CreateTranscriptionRequest")]
[CodeGenSuppress("AudioTranscriptionOptions", typeof(BinaryData), typeof(InternalCreateTranscriptionRequestModel))]
public partial class AudioTranscriptionOptions
{
    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    internal BinaryData File { get; }

    // CUSTOM:
    // - Made internal. The model is specified by the client.
    // - Added setter.
    internal InternalCreateTranscriptionRequestModel Model { get; set; }

    [CodeGenMember("TimestampGranularities")]
    internal IList<BinaryData> InternalTimestampGranularities { get; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="AudioTranscriptionOptions"/>. </summary>
    public AudioTranscriptionOptions()
    {
    }

    /// <summary>
    /// The timestamp granularities to populate for this transcription.
    /// </summary>
    public AudioTimestampGranularities TimestampGranularities { get; set; }

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
            content.Add(ResponseFormat.ToString(), "response_format");
        }

        if (Temperature is not null)
        {
            content.Add(Temperature.Value, "temperature");
        }

        if (TimestampGranularities.HasFlag(AudioTimestampGranularities.Word))
        {
            content.Add("word", "timestamp_granularities[]");
        }

        if (TimestampGranularities.HasFlag(AudioTimestampGranularities.Segment))
        {
            content.Add("segment", "timestamp_granularities[]");
        }

        return content;
    }
}
