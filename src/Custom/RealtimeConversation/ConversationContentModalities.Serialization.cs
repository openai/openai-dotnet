using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
internal static partial class ConversationContentModalitiesExtensions
{
    internal static void ToInternalModalities(this ConversationContentModalities modalities, IList<InternalRealtimeRequestSessionUpdateCommandSessionModality> internalModalities)
    {
        internalModalities.Clear();
        if (modalities.HasFlag(ConversationContentModalities.Text))
        {
            internalModalities.Add(InternalRealtimeRequestSessionUpdateCommandSessionModality.Text);
        }
        if (modalities.HasFlag(ConversationContentModalities.Audio))
        {
            internalModalities.Add(InternalRealtimeRequestSessionUpdateCommandSessionModality.Audio);
        }
    }

    internal static ConversationContentModalities FromInternalModalities(IEnumerable<InternalRealtimeRequestSessionUpdateCommandSessionModality> internalModalities)
    {
        ConversationContentModalities result = 0;
        foreach (InternalRealtimeRequestSessionUpdateCommandSessionModality internalModality in internalModalities ?? [])
        {
            if (internalModality == InternalRealtimeRequestSessionUpdateCommandSessionModality.Text)
            {
                result |= ConversationContentModalities.Text;
            }
            else if (internalModality == InternalRealtimeRequestSessionUpdateCommandSessionModality.Audio)
            {
                result |= ConversationContentModalities.Audio;
            }
        }
        return result;
    }
}