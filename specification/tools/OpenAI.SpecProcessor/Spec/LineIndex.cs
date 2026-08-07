using System.Text.RegularExpressions;

namespace OpenAI.SpecProcessor.Spec;

/// <summary> Builds a lookup from schema/path names to line numbers in a saved YAML file. </summary>
public class LineIndex
{
    private readonly Dictionary<string, int> _entries = new(StringComparer.Ordinal);

    /// <summary> The file name (not full path) this index was built from. </summary>
    public string FileName { get; }

    private LineIndex(string fileName)
    {
        FileName = fileName;
    }

    /// <summary> Gets the line number for the given key, or null if not found. </summary>
    /// <param name="key"> The key to look up (e.g., schema name or path). </param>
    /// <returns> The 1-based line number, or null. </returns>
    public int? GetLine(string key)
    {
        return _entries.TryGetValue(key, out var line) ? line : null;
    }

    /// <summary> Builds a line index by scanning a saved YAML feature spec file. </summary>
    /// <param name="filePath"> The full path to the YAML file. </param>
    /// <returns> A populated <see cref="LineIndex"/> instance. </returns>
    public static LineIndex Build(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var index = new LineIndex(fileName);
        var lines = File.ReadAllLines(filePath);

        // Track indentation context to identify top-level keys under paths: and schemas:.

        var topLevelEntryPattern = new Regex(@"^  (\S.+?):", RegexOptions.Compiled);
        var schemaEntryPattern = new Regex(@"^    (\w[\w-]*):", RegexOptions.Compiled);
        var methodPattern = new Regex(@"^    (get|post|put|patch|delete|head|options|trace):", RegexOptions.Compiled);
        var operationIdPattern = new Regex(@"^\s{6}operationId:\s*(.+)", RegexOptions.Compiled);

        string? currentSection = null;
        string? currentPath = null;
        string? currentMethod = null;
        int currentMethodLine = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var lineNumber = i + 1;

            // Detect top-level sections.

            if (line == "paths:")
            {
                currentSection = "paths";
                continue;
            }

            if (line == "components:")
            {
                currentSection = "components";
                currentPath = null;
                continue;
            }

            if (line == "  schemas:")
            {
                currentSection = "schemas";
                continue;
            }

            // Reset section if we hit another top-level key.

            if (line.Length > 0 && !char.IsWhiteSpace(line[0]) && line != "paths:" && line != "components:")
            {
                currentSection = null;
                currentPath = null;
                continue;
            }

            // Path entries are at 2-space indent under paths:.

            if (currentSection == "paths")
            {
                var pathMatch = topLevelEntryPattern.Match(line);

                if (pathMatch.Success)
                {
                    var pathKey = pathMatch.Groups[1].Value;

                    // YAML wraps path keys in quotes; strip them.

                    if (pathKey.StartsWith("'") || pathKey.StartsWith("\""))
                    {
                        pathKey = pathKey[1..^1];
                    }

                    index._entries[$"path:{pathKey}"] = lineNumber;
                    currentPath = pathKey;
                    currentMethod = null;
                    continue;
                }

                // HTTP method entries are at 4-space indent under a path.

                if (currentPath != null)
                {
                    var methodMatch = methodPattern.Match(line);

                    if (methodMatch.Success)
                    {
                        currentMethod = methodMatch.Groups[1].Value;
                        currentMethodLine = lineNumber;
                        index._entries[$"op:{currentPath}.{currentMethod}"] = lineNumber;
                        continue;
                    }

                    // operationId at 6-space indent under a method.

                    if (currentMethod != null)
                    {
                        var opIdMatch = operationIdPattern.Match(line);

                        if (opIdMatch.Success)
                        {
                            var opId = opIdMatch.Groups[1].Value.Trim();
                            index._entries[$"opid:{opId}"] = currentMethodLine;
                        }
                    }
                }
            }

            // Schema entries are at 4-space indent under schemas:.

            if (currentSection == "schemas")
            {
                var schemaMatch = schemaEntryPattern.Match(line);

                if (schemaMatch.Success)
                {
                    index._entries[$"schema:{schemaMatch.Groups[1].Value}"] = lineNumber;
                }
            }
        }

        return index;
    }
}
