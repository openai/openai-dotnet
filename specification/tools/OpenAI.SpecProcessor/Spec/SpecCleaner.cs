using YamlDotNet.RepresentationModel;

namespace OpenAI.SpecProcessor.Spec;

/// <summary> Cleans an OpenAPI spec by removing excluded paths, tags, and documentation metadata. </summary>
public static class SpecCleaner
{
    private static readonly YamlScalarNode DescriptionKey = new("description");
    private static readonly YamlScalarNode SummaryKey = new("summary");

    private static int RemoveExcludedPaths(SpecDocument doc)
    {
        var paths = doc.Paths;
        if (paths == null)
        {
            return 0;
        }

        var toRemove = new List<YamlScalarNode>();

        foreach (var (key, _) in paths.Children)
        {
            if (key is YamlScalarNode scalar && scalar.Value != null)
            {
                if (FeatureAreaConfig.IsExcludedPath(scalar.Value))
                {
                    toRemove.Add(scalar);
                }
            }
        }

        foreach (var key in toRemove)
        {
            paths.Children.Remove(key);
        }

        return toRemove.Count;
    }

    private static int RemoveExcludedAndOrphanedTags(SpecDocument doc)
    {
        var tagsNode = doc.Tags;
        if (tagsNode == null)
        {
            return 0;
        }

        // Collect all tags still referenced by remaining operations.

        var usedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var paths = doc.Paths;
        if (paths != null)
        {
            foreach (var (_, pathValue) in paths.Children)
            {
                if (pathValue is not YamlMappingNode pathItem)
                {
                    continue;
                }

                foreach (var (_, opMapping) in SpecDocument.GetOperations(pathItem))
                {
                    foreach (var tag in SpecDocument.GetOperationTags(opMapping))
                    {
                        usedTags.Add(tag);
                    }
                }
            }
        }

        var toRemove = new List<YamlNode>();

        foreach (var tagEntry in tagsNode.Children)
        {
            if (tagEntry is not YamlMappingNode tagMapping)
            {
                continue;
            }

            var nameKey = new YamlScalarNode("name");
            if (!tagMapping.Children.TryGetValue(nameKey, out var nameNode) || nameNode is not YamlScalarNode nameScalar)
            {
                continue;
            }

            var tagName = nameScalar.Value ?? "";

            // Remove if explicitly excluded or orphaned.

            if (FeatureAreaConfig.ExcludedTags.Contains(tagName, StringComparer.OrdinalIgnoreCase)
                || !usedTags.Contains(tagName))
            {
                toRemove.Add(tagEntry);
            }
        }

        foreach (var node in toRemove)
        {
            tagsNode.Children.Remove(node);
        }

        return toRemove.Count;
    }

    private static int StripMetadata(YamlNode node)
    {
        int count = 0;

        switch (node)
        {
            case YamlMappingNode mapping:
                foreach (var key in FeatureAreaConfig.MetadataKeysToStrip)
                {
                    var yamlKey = new YamlScalarNode(key);
                    if (mapping.Children.Remove(yamlKey))
                    {
                        count++;
                    }
                }

                foreach (var (_, value) in mapping.Children)
                {
                    count += StripMetadata(value);
                }
                break;

            case YamlSequenceNode sequence:
                foreach (var child in sequence.Children)
                {
                    count += StripMetadata(child);
                }
                break;
        }

        return count;
    }

    private static int CollapseDescriptionBlankLines(YamlNode node)
    {
        int count = 0;

        switch (node)
        {
            case YamlMappingNode mapping:
                foreach (var textKey in new[] { DescriptionKey, SummaryKey })
                {
                    if (mapping.Children.TryGetValue(textKey, out var textNode)
                        && textNode is YamlScalarNode textScalar
                        && textScalar.Value != null
                        && textScalar.Value.Contains('\n'))
                    {
                        var normalized = NormalizeDescriptionText(textScalar.Value);

                        if (normalized != textScalar.Value)
                        {
                            textScalar.Value = normalized;
                            count++;
                        }
                    }
                }

                foreach (var (_, value) in mapping.Children)
                {
                    count += CollapseDescriptionBlankLines(value);
                }
                break;

            case YamlSequenceNode sequence:
                foreach (var child in sequence.Children)
                {
                    count += CollapseDescriptionBlankLines(child);
                }
                break;
        }

        return count;
    }

    private static string NormalizeDescriptionText(string text)
    {
        // Mark real paragraph breaks (two or more consecutive newlines) with a placeholder.
        var normalized = System.Text.RegularExpressions.Regex.Replace(text, @"\n{2,}", "\x00PARA\x00");

        // Collapse single newlines to spaces, except before list markers.
        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"\n(?![-*] |\d+[.)]\s)", " ");

        // Restore paragraph breaks as single newlines.
        normalized = normalized.Replace("\x00PARA\x00", "\n");

        return normalized.Trim();
    }

    /// <summary> Creates a cleaned copy of the spec with exclusions and metadata stripped. </summary>
    /// <param name="source"> The source spec document to clean. </param>
    /// <returns> A new spec document with exclusions and metadata removed. </returns>
    public static SpecDocument Clean(SpecDocument source)
    {
        var doc = source.Clone();

        int pathsRemoved = RemoveExcludedPaths(doc);
        int tagsRemoved = RemoveExcludedAndOrphanedTags(doc);
        int metadataRemoved = StripMetadata(doc.Root);
        int descriptionsFixed = CollapseDescriptionBlankLines(doc.Root);

        Console.WriteLine($"  Cleaned: {pathsRemoved} paths removed, {tagsRemoved} tags removed, {metadataRemoved} metadata fields stripped, {descriptionsFixed} descriptions collapsed");

        return doc;
    }
}
