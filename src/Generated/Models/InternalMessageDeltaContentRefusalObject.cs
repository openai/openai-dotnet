// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Assistants
{
    internal partial class InternalMessageDeltaContentRefusalObject : InternalMessageDeltaContent
    {
        internal InternalMessageDeltaContentRefusalObject(int index) : base(InternalMessageContentType.Refusal)
        {
            Index = index;
        }

        internal InternalMessageDeltaContentRefusalObject(InternalMessageContentType kind, IDictionary<string, BinaryData> additionalBinaryDataProperties, int index, string refusal) : base(kind, additionalBinaryDataProperties)
        {
            Index = index;
            Refusal = refusal;
        }

        public int Index { get; }

        public string Refusal { get; }
    }
}
