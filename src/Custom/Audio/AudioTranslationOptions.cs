using System;
using System.Collections.Generic;
using System.IO;

namespace OpenAI.Audio;

[CodeGenType("CreateTranslationRequest")]
[CodeGenVisibility(nameof(AudioTranslationOptions), CodeGenVisibility.Public)]
[CodeGenSuppress(nameof(AudioTranslationOptions), typeof(BinaryData), typeof(InternalCreateTranslationRequestModel))]
public partial class AudioTranslationOptions
{
    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    internal BinaryData File { get; }

    // CUSTOM:
    // - Made internal. The model is specified by the client.
    // - Added setter.
    internal InternalCreateTranslationRequestModel Model { get; set; }

    internal MultiPartFormDataBinaryContent ToMultipartContent(Stream audio, string audioFilename)
    {
        MultiPartFormDataBinaryContent content = new();

        content.Add(audio, "file", audioFilename);
        content.Add(Model.ToString(), "model");

        if (Prompt is not null)
        {
            content.Add(Prompt, "prompt");
        }

        if (ResponseFormat is not null)
        {
            content.Add(ResponseFormat.ToString(), "response_format");
        }

        return content;
    }

    internal AudioTranslationOptions GetClone()
    {
        AudioTranslationOptions copiedOptions = (AudioTranslationOptions)this.MemberwiseClone();

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