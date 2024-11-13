using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
internal static partial class ConversationContentModalitiesExtensions
{
    internal static IList<InternalRealtimeRequestSessionModality> ToInternalModalities(this ConversationContentModalities modalities)
    {
        List<InternalRealtimeRequestSessionModality> internalModalities = [];
        if (modalities.HasFlag(ConversationContentModalities.Text))
        {
            internalModalities.Add(InternalRealtimeRequestSessionModality.Text);
        }
        if (modalities.HasFlag(ConversationContentModalities.Audio))
        {
            internalModalities.Add(InternalRealtimeRequestSessionModality.Audio);
        }
        return internalModalities;
    }

    internal static ConversationContentModalities FromInternalModalities(IEnumerable<InternalRealtimeRequestSessionModality> internalModalities)
    {
        ConversationContentModalities result = 0;
        foreach (InternalRealtimeRequestSessionModality internalModality in internalModalities ?? [])
        {
            if (internalModality == InternalRealtimeRequestSessionModality.Text)
            {
                result |= ConversationContentModalities.Text;
            }
            else if (internalModality == InternalRealtimeRequestSessionModality.Audio)
            {
                result |= ConversationContentModalities.Audio;
            }
        }
        return result;
    }
}