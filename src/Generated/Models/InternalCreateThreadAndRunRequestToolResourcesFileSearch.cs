// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Assistants
{
    internal partial class InternalCreateThreadAndRunRequestToolResourcesFileSearch
    {
        internal IDictionary<string, BinaryData> _serializedAdditionalRawData;

        public InternalCreateThreadAndRunRequestToolResourcesFileSearch()
        {
            VectorStoreIds = new ChangeTrackingList<string>();
        }

        internal InternalCreateThreadAndRunRequestToolResourcesFileSearch(IList<string> vectorStoreIds, IDictionary<string, BinaryData> serializedAdditionalRawData)
        {
            VectorStoreIds = vectorStoreIds;
            _serializedAdditionalRawData = serializedAdditionalRawData;
        }

        public IList<string> VectorStoreIds { get; }
    }
}
