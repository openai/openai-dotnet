// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Assistants
{
    public abstract partial class RunStepUpdateCodeInterpreterOutput
    {
        private protected IDictionary<string, BinaryData> _serializedAdditionalRawData;

        protected RunStepUpdateCodeInterpreterOutput()
        {
        }

        internal RunStepUpdateCodeInterpreterOutput(string type, IDictionary<string, BinaryData> serializedAdditionalRawData)
        {
            Type = type;
            _serializedAdditionalRawData = serializedAdditionalRawData;
        }

        internal string Type { get; init; }
    }
}
