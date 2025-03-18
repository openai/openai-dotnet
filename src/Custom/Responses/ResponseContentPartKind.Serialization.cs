using System;

namespace OpenAI.Responses;

internal static partial class ResponseContentPartKindExtensions
{
    public static string ToSerialString(this ResponseContentPartKind value) => value switch
    {
        ResponseContentPartKind.InputText => InternalResponsesContentType.InputText.ToString(),
        ResponseContentPartKind.InputImage => InternalResponsesContentType.InputImage.ToString(),
        ResponseContentPartKind.InputFile => InternalResponsesContentType.InputFile.ToString(),
        ResponseContentPartKind.OutputText => InternalResponsesContentType.OutputText.ToString(),
        ResponseContentPartKind.Refusal => InternalResponsesContentType.Refusal.ToString(),
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown ResponseContentPartKind value.")
    };

    public static ResponseContentPartKind ToResponseContentPartKind(this string value)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesContentType.InputText.ToString()))
        {
            return ResponseContentPartKind.InputText;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesContentType.InputImage.ToString()))
        {
            return ResponseContentPartKind.InputImage;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesContentType.InputFile.ToString()))
        {
            return ResponseContentPartKind.InputFile;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesContentType.OutputText.ToString()))
        {
            return ResponseContentPartKind.OutputText;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalResponsesContentType.Refusal.ToString()))
        {
            return ResponseContentPartKind.Refusal;
        }
        return ResponseContentPartKind.Unknown;
    }
}
