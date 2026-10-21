using System;

namespace OpenAI.Responses;

internal static partial class ResponseToolChoiceKindExtensions
{
    public static string ToSerialString(this ResponseToolChoiceKind value) => value switch
    {
        ResponseToolChoiceKind.Unknown => "unknown",
        ResponseToolChoiceKind.None => "none",
        ResponseToolChoiceKind.Auto => "auto",
        ResponseToolChoiceKind.Required => "required",
        ResponseToolChoiceKind.FileSearch => "file_search",
        ResponseToolChoiceKind.Function => "function",
        ResponseToolChoiceKind.Computer => "computer_use_preview",
        ResponseToolChoiceKind.WebSearch => "web_search_preview",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown ResponseToolChoiceKind value.")
    };

    public static ResponseToolChoiceKind ToResponseToolChoiceKind(this string value)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "none"))
        {
            return ResponseToolChoiceKind.None;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "auto"))
        {
            return ResponseToolChoiceKind.Auto;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "required"))
        {
            return ResponseToolChoiceKind.Required;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "file_search"))
        {
            return ResponseToolChoiceKind.FileSearch;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "function"))
        {
            return ResponseToolChoiceKind.Function;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "computer_use_preview"))
        {
            return ResponseToolChoiceKind.Computer;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "web_search_preview"))
        {
            return ResponseToolChoiceKind.WebSearch;
        }
        return ResponseToolChoiceKind.Unknown;
    }
}