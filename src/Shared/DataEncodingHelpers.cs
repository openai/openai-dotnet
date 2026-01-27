using System;
using System.Text.RegularExpressions;

#nullable enable

namespace OpenAI;

internal static partial class DataEncodingHelpers
{
#if NET8_0_OR_GREATER
    [GeneratedRegex(@"^data:(?<type>.+?);base64,(?<data>.+)$")]
    private static partial Regex ParseDataUriRegex();
#else
    private static Regex ParseDataUriRegex() => s_parseDataUriRegex;
    private static readonly Regex s_parseDataUriRegex = new(@"^data:(?<type>.+?);base64,(?<data>.+)$", RegexOptions.Compiled);
#endif

    public static bool TryParseDataUri(string dataUri, out BinaryData? bytes, out string? bytesMediaType)
    {
        Match parsedDataUri = ParseDataUriRegex().Match(dataUri);

        if (!parsedDataUri.Success)
        {
            bytes = null;
            bytesMediaType = null;
            return false;
        }

        string matchedBase64Data = parsedDataUri.Groups["data"].Value;
        byte[] matchedBase64RawBytes = Convert.FromBase64String(matchedBase64Data);
        
        bytes = BinaryData.FromBytes(matchedBase64RawBytes);
        bytesMediaType = parsedDataUri.Groups["type"].Value;
        return true;
    }

    public static string CreateDataUri(BinaryData bytes, string bytesMediaType)
    {
        string base64Bytes = Convert.ToBase64String(bytes.ToArray());
        return $"data:{bytesMediaType};base64,{base64Bytes}";
    }
}