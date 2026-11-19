namespace OpenAI.SpecProcessor.Spec;

/// <summary>
/// Repairs the small number of constructs in the upstream OpenAI specification that strict YAML
/// parsers reject.
/// </summary>
/// <remarks>
/// <para>
/// The upstream document at <c>openai/openai-openapi</c> is produced by tooling that is more lenient
/// than the YAML specification. The one construct it emits that YamlDotNet refuses is a block scalar
/// whose leading empty lines are indented further than the block itself, for example:
/// </para>
/// <code>
/// example: |+
///
/// nullable: true
/// </code>
/// <para>
/// where the line between the two is not empty but is made entirely of spaces. The YAML
/// specification forbids a leading empty line from carrying more spaces than the first line with
/// content, and when there is no line with content at all the block indentation falls back to the
/// parent, so the spaces are an error. Parsing fails with "found extra spaces in first line".
/// </para>
/// <para>
/// Blanking those lines is safe. A leading empty line in a block scalar contributes a line break and
/// nothing else no matter how many spaces it carries, so there is no valid reading of the document
/// that this changes. The repair is deliberately narrow, touching only whitespace-only lines that
/// precede the first line with content in a block scalar, so that a real change upstream cannot be
/// silently normalized away.
/// </para>
/// <para>
/// The repair is applied to the parsed document only. The recorded content hash is taken over the
/// bytes as downloaded, so provenance still describes exactly what upstream published.
/// </para>
/// </remarks>
public static class SpecSanitizer
{
    /// <summary>
    /// The revision of the repairs applied here.
    /// </summary>
    /// <remarks>
    /// Increment this when a change would repair a document differently, since the sanitizer runs
    /// before anything is parsed and so can change the whole snapshot without the source moving.
    /// It feeds the processing identity, which is what stops a scheduled run from skipping the month
    /// a repair rule changed.
    /// </remarks>
    public const int Version = 1;

    /// <summary> Repairs block scalars whose leading empty lines would fail strict YAML parsing. </summary>
    /// <param name="yaml"> The YAML document text to repair. </param>
    /// <param name="repairedLines"> The number of lines that were blanked. </param>
    /// <returns> The repaired document text. </returns>
    public static string Repair(string yaml, out int repairedLines)
    {
        var lines = LineEndings.Normalize(yaml).Split('\n');
        var pending = new List<int>();

        var inBlockScalar = false;
        var headerIndent = 0;
        var contentSeen = false;

        repairedLines = 0;

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var isBlank = line.Trim().Length == 0;

            if (inBlockScalar)
            {
                if (isBlank)
                {
                    // Only the run of empty lines before the first line with content can be at fault;
                    // once content has been seen the block indentation is settled.

                    if (!contentSeen && line.Length > 0)
                    {
                        pending.Add(index);
                    }

                    continue;
                }

                if (IndentOf(line) > headerIndent)
                {
                    // The first line with content establishes the block indentation, so any pending
                    // line indented past it is the construct being repaired.

                    contentSeen = true;
                    repairedLines += Blank(lines, pending, IndentOf(line));
                    pending.Clear();

                    continue;
                }

                // The block ended without ever holding content, which is the case that fails to
                // parse. Fall through so this line is considered as a header in its own right.

                repairedLines += Blank(lines, pending, headerIndent);
                pending.Clear();

                inBlockScalar = false;
            }

            if (isBlank)
            {
                continue;
            }

            if (IsBlockScalarHeader(line, out var hasExplicitIndent))
            {
                inBlockScalar = true;
                headerIndent = IndentOf(line);

                // An explicit indentation indicator removes the auto-detection this repair exists to
                // work around, so those blocks are left alone.

                contentSeen = hasExplicitIndent;
                pending.Clear();
            }
        }

        repairedLines += Blank(lines, pending, headerIndent);

        return repairedLines == 0 ? yaml : string.Join('\n', lines);
    }


    /// <summary> Blanks the given lines when they are indented past the block indentation. </summary>
    private static int Blank(string[] lines, List<int> indexes, int blockIndent)
    {
        var count = 0;

        foreach (var index in indexes)
        {
            if (lines[index].Length > blockIndent)
            {
                lines[index] = string.Empty;
                count++;
            }
        }

        return count;
    }

    /// <summary> Counts the leading spaces on a line. </summary>
    private static int IndentOf(string line)
    {
        var indent = 0;

        while ((indent < line.Length) && (line[indent] == ' '))
        {
            indent++;
        }

        return indent;
    }

    /// <summary>
    /// Determines whether a line opens a block scalar, such as <c>description: |</c> or <c>- >-</c>.
    /// </summary>
    /// <remarks>
    /// The indicator has to be the last thing on the line and has to be preceded by whitespace, which
    /// is what keeps a plain value such as <c>default: &lt;|endoftext|&gt;</c> from being mistaken for
    /// a header. Headers carrying a trailing comment are not recognized; missing one costs nothing
    /// beyond leaving the document exactly as it arrived.
    /// </remarks>
    private static bool IsBlockScalarHeader(string line, out bool hasExplicitIndent)
    {
        hasExplicitIndent = false;

        var end = line.Length;

        while ((end > 0) && char.IsWhiteSpace(line[end - 1]))
        {
            end--;
        }

        if (end == 0)
        {
            return false;
        }

        if (char.IsAsciiDigit(line[end - 1]))
        {
            hasExplicitIndent = true;
            end--;
        }

        if ((end > 0) && ((line[end - 1] == '+') || (line[end - 1] == '-')))
        {
            end--;
        }

        if ((end == 0) || ((line[end - 1] != '|') && (line[end - 1] != '>')))
        {
            hasExplicitIndent = false;
            return false;
        }

        // The indicator must start a token; otherwise this is an ordinary value that happens to end
        // in one of these characters.

        if ((end - 1 == 0) || char.IsWhiteSpace(line[end - 2]))
        {
            return true;
        }

        hasExplicitIndent = false;
        return false;
    }
}
