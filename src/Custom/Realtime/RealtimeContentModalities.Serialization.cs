using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
internal static partial class RealtimeContentModalitiesExtensions
{
    internal static IList<InternalRealtimeRequestSessionModality> ToInternalModalities(this RealtimeContentModalities modalities)
    {
        ChangeTrackingList<InternalRealtimeRequestSessionModality> internalModalities = new();
        if (modalities.HasFlag(RealtimeContentModalities.Text))
        {
            internalModalities.Add(InternalRealtimeRequestSessionModality.Text);
        }
        if (modalities.HasFlag(RealtimeContentModalities.Audio))
        {
            internalModalities.Add(InternalRealtimeRequestSessionModality.Audio);
        }
        return internalModalities;
    }

    internal static RealtimeContentModalities FromInternalModalities(IEnumerable<InternalRealtimeRequestSessionModality> internalModalities)
    {
        RealtimeContentModalities result = 0;
        foreach (InternalRealtimeRequestSessionModality internalModality in internalModalities ?? [])
        {
            if (internalModality == InternalRealtimeRequestSessionModality.Text)
            {
                result |= RealtimeContentModalities.Text;
            }
            else if (internalModality == InternalRealtimeRequestSessionModality.Audio)
            {
                result |= RealtimeContentModalities.Audio;
            }
        }
        return result;
    }
}