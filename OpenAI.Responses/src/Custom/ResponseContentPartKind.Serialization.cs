using System;

namespace OpenAI.Responses;

internal static partial class ResponseContentPartKindExtensions
{
    public static string ToSerialString(this ResponseContentPartKind value) => value switch
    {
        ResponseContentPartKind.InputText => InternalItemContentType.InputText.ToString(),
        ResponseContentPartKind.InputImage => InternalItemContentType.InputImage.ToString(),
        ResponseContentPartKind.InputFile => InternalItemContentType.InputFile.ToString(),
        ResponseContentPartKind.OutputText => InternalItemContentType.OutputText.ToString(),
        ResponseContentPartKind.Refusal => InternalItemContentType.Refusal.ToString(),
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown ResponseContentPartKind value.")
    };

    public static ResponseContentPartKind ToResponseContentPartKind(this string value)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalItemContentType.InputText.ToString()))
        {
            return ResponseContentPartKind.InputText;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalItemContentType.InputImage.ToString()))
        {
            return ResponseContentPartKind.InputImage;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalItemContentType.InputFile.ToString()))
        {
            return ResponseContentPartKind.InputFile;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalItemContentType.OutputText.ToString()))
        {
            return ResponseContentPartKind.OutputText;
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(value, InternalItemContentType.Refusal.ToString()))
        {
            return ResponseContentPartKind.Refusal;
        }
        return ResponseContentPartKind.Unknown;
    }
}
