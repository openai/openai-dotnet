using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;

namespace OpenAI.Audio;

[CodeGenType("CreateTranscriptionRequest")]
[CodeGenVisibility(nameof(AudioTranscriptionOptions), CodeGenVisibility.Public)]
[CodeGenSuppress(nameof(AudioTranscriptionOptions), typeof(BinaryData), typeof(InternalCreateTranscriptionRequestModel))]
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

    // CUSTOM: Made internal to allow specification by the operation method.
    [CodeGenMember("Stream")]
    internal bool? Stream { get; set; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="AudioTranscriptionOptions"/>. </summary>
    public AudioTranscriptionOptions()
    {
    }

    /// <summary>
    /// The timestamp granularities to populate for this transcription.
    /// </summary>
    public AudioTimestampGranularities TimestampGranularities { get; set; }

    // CUSTOM: Added Experimental attribute.
    /// <summary>
    /// Flags specifying which additional response information should be provided on transcription results.
    /// </summary>
    [Experimental("OPENAI001")]
    public AudioTranscriptionIncludes Includes { get; set; }

    // CUSTOM: Internal in favor of [Flags] enumeration value.
    [CodeGenMember("Include")]
    internal IList<InternalTranscriptionInclude> InternalInclude { get; }

    internal MultiPartFormDataBinaryContent ToMultipartContent(Stream audio, string audioFilename)
    {
        MultiPartFormDataBinaryContent content = new();

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

        if (Includes.HasFlag(AudioTranscriptionIncludes.Logprobs))
        {
            content.Add("logprobs", "include[]");
        }

        if (Stream == true)
        {
            content.Add(true, "stream");
        }

        return content;
    }

    internal AudioTranscriptionOptions GetClone()
    {
        AudioTranscriptionOptions copiedOptions = (AudioTranscriptionOptions)this.MemberwiseClone();

        if (SerializedAdditionalRawData is not null)
        {
            copiedOptions.SerializedAdditionalRawData = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (KeyValuePair<string, BinaryData> sourcePair in SerializedAdditionalRawData)
            {
                copiedOptions.SerializedAdditionalRawData[sourcePair.Key] = sourcePair.Value;
            }
        }

        return copiedOptions;
    }
}
