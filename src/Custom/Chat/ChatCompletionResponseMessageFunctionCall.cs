using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ChatCompletionResponseMessageFunctionCall
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal ChatCompletionResponseMessageFunctionCall(string name, string arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        internal ChatCompletionResponseMessageFunctionCall(string name, string arguments, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            Name = name;
            Arguments = arguments;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Name { get; }

        public string Arguments { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}