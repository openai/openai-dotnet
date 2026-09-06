using System;
using System.Text.RegularExpressions;

#if !NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

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
        const string dataPrefix = "data:";
        const string base64Prefix = ";base64,";
        int base64Length = checked(((memory.Length + 2) / 3) * 4);
        int prefixLength = checked(dataPrefix.Length + bytesMediaType.Length + base64Prefix.Length);

        return string.Create(
            checked(prefixLength + base64Length),
            (memory, bytesMediaType, base64Length),
            static (destination, state) =>
            {
                int offset = 0;
                dataPrefix.AsSpan().CopyTo(destination);
                offset += dataPrefix.Length;
                state.bytesMediaType.AsSpan().CopyTo(destination[offset..]);
                offset += state.bytesMediaType.Length;
                base64Prefix.AsSpan().CopyTo(destination[offset..]);
                offset += base64Prefix.Length;

                if (!Convert.TryToBase64Chars(
                    state.memory.Span,
                    destination[offset..],
                    out int charsWritten)
                    || charsWritten != state.base64Length)
                {
                    throw new InvalidOperationException("Base64 encoding did not produce the expected output.");
                }
            });
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
        return $"data:{bytesMediaType};base64,{base64Bytes}";
#endif
    }
}
