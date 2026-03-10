using System;

namespace OpenAI.Assistants;

internal static partial class MessageImageDetailExtensions
{
    public static string ToSerialString(this MessageImageDetail value) => value switch
    {
        MessageImageDetail.Auto => "auto",
        MessageImageDetail.Low => "low",
        MessageImageDetail.High => "high",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown MessageImageDetail value: {value}")
    };

    public static MessageImageDetail ToMessageImageDetail(this string value)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "auto")) return MessageImageDetail.Auto;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "low")) return MessageImageDetail.Low;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "high")) return MessageImageDetail.High;
        throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown MessageImageDetail value: {value}");
    }
}
