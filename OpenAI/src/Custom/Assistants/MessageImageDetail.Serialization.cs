using System;

namespace OpenAI.Assistants;

internal static partial class MessageImageDetailExtensions
{
    public static string ToSerialString(this MessageImageDetail value) => value.ToString();

    public static MessageImageDetail ToMessageImageDetail(this string value) => new MessageImageDetail(value);
}
