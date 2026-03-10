using System;

namespace OpenAI.Responses;

internal static partial class MessageRoleExtensions
{
    public static string ToSerialString(this MessageRole value) => value switch
    {
        MessageRole.Assistant => InternalResponsesMessageRole.Assistant.ToString(),
        MessageRole.Developer => InternalResponsesMessageRole.Developer.ToString(),
        MessageRole.System => InternalResponsesMessageRole.System.ToString(),
        MessageRole.User => InternalResponsesMessageRole.User.ToString(),
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown MessageRole value."),
    };

    public static MessageRole ToMessageRole(this string value)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesMessageRole.Assistant.ToString()))
        {
            return MessageRole.Assistant;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesMessageRole.Developer.ToString()))
        {
            return MessageRole.Developer;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesMessageRole.System.ToString()))
        {
            return MessageRole.System;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesMessageRole.User.ToString()))
        {
            return MessageRole.User;
        }
        return MessageRole.Unknown;
    }
}
