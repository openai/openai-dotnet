using System;
using System.Runtime.InteropServices;
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
        byte[] matchedBase64RawBytes;
        try
        {
            matchedBase64RawBytes = Convert.FromBase64String(matchedBase64Data);
        }
        catch (FormatException)
        {
            bytes = null;
            bytesMediaType = null;
            return false;
        }
        
        bytes = BinaryData.FromBytes(matchedBase64RawBytes);
        bytesMediaType = parsedDataUri.Groups["type"].Value;
        return true;
    }

    public static string CreateDataUri(BinaryData bytes, string bytesMediaType)
    {
        ReadOnlyMemory<byte> memory = bytes.ToMemory();
#if NET8_0_OR_GREATER
        string base64Bytes = Convert.ToBase64String(memory.Span);
#else
        string base64Bytes;
        if (MemoryMarshal.TryGetArray(memory, out ArraySegment<byte> segment)
            && segment.Array is not null)
        {
            base64Bytes = Convert.ToBase64String(
                segment.Array,
                segment.Offset,
                segment.Count);
        }
        else
        {
            base64Bytes = Convert.ToBase64String(memory.ToArray());
        }
#endif
        return $"data:{bytesMediaType};base64,{base64Bytes}";
    }
}
