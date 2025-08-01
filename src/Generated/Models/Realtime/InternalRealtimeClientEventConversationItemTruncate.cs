// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using OpenAI;

namespace OpenAI.Realtime
{
    internal partial class InternalRealtimeClientEventConversationItemTruncate : InternalRealtimeClientEvent
    {
        public InternalRealtimeClientEventConversationItemTruncate(string itemId, int contentIndex, int audioEndMs) : base(InternalRealtimeClientEventType.ConversationItemTruncate)
        {
            Argument.AssertNotNull(itemId, nameof(itemId));

            ItemId = itemId;
            ContentIndex = contentIndex;
            AudioEndMs = audioEndMs;
        }

        internal InternalRealtimeClientEventConversationItemTruncate(InternalRealtimeClientEventType kind, string eventId, IDictionary<string, BinaryData> additionalBinaryDataProperties, string itemId, int contentIndex, int audioEndMs) : base(kind, eventId, additionalBinaryDataProperties)
        {
            ItemId = itemId;
            ContentIndex = contentIndex;
            AudioEndMs = audioEndMs;
        }

        public string ItemId { get; }

        public int ContentIndex { get; }

        public int AudioEndMs { get; }
    }
}
