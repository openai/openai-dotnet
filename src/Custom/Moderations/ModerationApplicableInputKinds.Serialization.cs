using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Moderations;

internal static partial class ModerationApplicableInputKindsExtensions
{
    internal static IReadOnlyList<string> ToInternalApplicableInputKinds(this ModerationApplicableInputKinds inputKinds)
    {
        List<string> internalInputKinds = [];
        if (inputKinds.HasFlag(ModerationApplicableInputKinds.Text))
        {
            internalInputKinds.Add("text");
        }
        if (inputKinds.HasFlag(ModerationApplicableInputKinds.Image))
        {
            internalInputKinds.Add("image");
        }
        // if (inputKinds.HasFlag(ModerationInputKinds.Audio))
        // {
        //     internalInputKinds.Add("audio");
        // }
        return internalInputKinds;
    }

    internal static ModerationApplicableInputKinds FromInternalApplicableInputKinds(IEnumerable<string> internalInputKinds)
    {
        ModerationApplicableInputKinds result = 0;
        foreach (string internalInputKind in internalInputKinds ?? [])
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(internalInputKind, "text"))
            {
                result |= ModerationApplicableInputKinds.Text;
            }
            else if (StringComparer.OrdinalIgnoreCase.Equals(internalInputKind, "image"))
            {
                result |= ModerationApplicableInputKinds.Image;
            }
            // else if (StringComparer.OrdinalIgnoreCase.Equals(internalInputKind, "audio"))
            // {
            //     result |= ModerationInputKinds.Audio;
            // }
            else
            {
                result |= ModerationApplicableInputKinds.Other;
            }
        }
        return result;
    }

    internal static string ToSerialString(this ModerationApplicableInputKinds value)
        => throw new NotImplementedException();

    internal static ModerationApplicableInputKinds ToModerationApplicableInputKinds(this string value)
        => throw new NotImplementedException();
}