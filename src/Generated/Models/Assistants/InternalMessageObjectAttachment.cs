// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using OpenAI;

namespace OpenAI.Assistants
{
    internal partial class InternalMessageObjectAttachment
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal InternalMessageObjectAttachment() : this(null, null, null)
        {
        }

        internal InternalMessageObjectAttachment(string fileId, IList<BinaryData> tools, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            FileId = fileId;
            Tools = tools ?? new ChangeTrackingList<BinaryData>();
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string FileId { get; }

        public IList<BinaryData> Tools { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
