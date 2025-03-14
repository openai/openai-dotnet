// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Audio
{
    public partial class AudioTranscriptionOptions
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal AudioTranscriptionOptions(string language, string prompt, AudioTranscriptionFormat? responseFormat, float? temperature, BinaryData @file, InternalCreateTranscriptionRequestModel model, IList<BinaryData> internalTimestampGranularities, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            Language = language;
            Prompt = prompt;
            ResponseFormat = responseFormat;
            Temperature = temperature;
            File = @file;
            Model = model;
            InternalTimestampGranularities = internalTimestampGranularities;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Language { get; set; }

        public string Prompt { get; set; }

        public AudioTranscriptionFormat? ResponseFormat { get; set; }

        public float? Temperature { get; set; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
