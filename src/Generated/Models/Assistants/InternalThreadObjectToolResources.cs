// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Assistants
{
    internal partial class InternalThreadObjectToolResources
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal InternalThreadObjectToolResources()
        {
        }

        internal InternalThreadObjectToolResources(InternalThreadObjectToolResourcesCodeInterpreter codeInterpreter, InternalThreadObjectToolResourcesFileSearch fileSearch, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            CodeInterpreter = codeInterpreter;
            FileSearch = fileSearch;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        internal InternalThreadObjectToolResourcesCodeInterpreter CodeInterpreter { get; }

        internal InternalThreadObjectToolResourcesFileSearch FileSearch { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
