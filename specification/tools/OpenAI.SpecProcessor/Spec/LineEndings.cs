namespace OpenAI.SpecProcessor.Spec;

/// <summary>
/// Writes text files with LF line endings.
/// </summary>
/// <remarks>
/// The repository normalizes text to LF via <c>.gitattributes</c>. Emitting CRLF would either be
/// rewritten on commit or show up as a whole-file diff, either of which defeats the byte-stable
/// output that the month-over-month comparison depends on. Normalizing here keeps the output
/// identical regardless of which platform produced it.
/// </remarks>
public static class LineEndings
{
    /// <summary> Normalizes line endings to LF and writes the content to the given path. </summary>
    /// <param name="path"> The file to write. </param>
    /// <param name="content"> The content to normalize and write. </param>
    public static void WriteAllText(string path, string content)
    {
        File.WriteAllText(path, Normalize(content));
    }

    /// <summary> Normalizes line endings to LF and writes the content to the given path. </summary>
    /// <param name="path"> The file to write. </param>
    /// <param name="content"> The content to normalize and write. </param>
    /// <returns> A task that completes when the file has been written. </returns>
    public static Task WriteAllTextAsync(string path, string content)
    {
        return File.WriteAllTextAsync(path, Normalize(content));
    }

    /// <summary> Converts any CRLF or bare CR line endings in the content to LF. </summary>
    /// <param name="content"> The content to normalize. </param>
    /// <returns> The content with LF line endings. </returns>
    public static string Normalize(string content)
    {
        return content
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);
    }
}
