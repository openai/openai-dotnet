using System;

namespace OpenAI.Responses;

internal static partial class ResponseTextFormatKindExtensions
{
    public static string ToSerialString(this ResponseTextFormatKind value) => value switch
    {
        ResponseTextFormatKind.Text => InternalResponsesTextFormatType.Text.ToString(),
        ResponseTextFormatKind.JsonObject => InternalResponsesTextFormatType.JsonObject.ToString(),
        ResponseTextFormatKind.JsonSchema => InternalResponsesTextFormatType.JsonSchema.ToString(),
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown ResponseTextFormatKind value."),
    };

    public static ResponseTextFormatKind ToResponseTextFormatKind(this string value)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesTextFormatType.Text.ToString()))
        {
            return ResponseTextFormatKind.Text;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesTextFormatType.JsonObject.ToString()))
        {
            return ResponseTextFormatKind.JsonObject;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesTextFormatType.JsonSchema.ToString()))
        {
            return ResponseTextFormatKind.JsonSchema;
        }
        return ResponseTextFormatKind.Unknown;
    }
}
