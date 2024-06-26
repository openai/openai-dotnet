// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Assistants
{
    internal partial class InternalDeleteThreadResponse
    {
        internal IDictionary<string, BinaryData> _serializedAdditionalRawData;

        internal InternalDeleteThreadResponse(string id, bool deleted)
        {
            Argument.AssertNotNull(id, nameof(id));

            Id = id;
            Deleted = deleted;
        }

        internal InternalDeleteThreadResponse(string id, bool deleted, InternalDeleteThreadResponseObject @object, IDictionary<string, BinaryData> serializedAdditionalRawData)
        {
            Id = id;
            Deleted = deleted;
            Object = @object;
            _serializedAdditionalRawData = serializedAdditionalRawData;
        }

        internal InternalDeleteThreadResponse()
        {
        }

        public string Id { get; }
        public bool Deleted { get; }
        public InternalDeleteThreadResponseObject Object { get; } = InternalDeleteThreadResponseObject.ThreadDeleted;
    }
}
