using YamlDotNet.RepresentationModel;

namespace OpenAI.SpecProcessor.Spec;

/// <summary> Splits a cleaned OpenAPI spec into per-feature self-contained specs. </summary>
public static class SpecSplitter
{
    private static SpecDocument BuildFeatureSpec(
        SpecDocument source,
        FeatureArea area,
        List<(string Path, YamlMappingNode PathItem)> areaPaths)
    {
        var root = new YamlMappingNode();

        // Set the OpenAPI version.

        root.Add("openapi", source.OpenApiVersion ?? "3.1.0");

        // Clone and update the info title.

        if (source.Info != null)
        {
            var info = (YamlMappingNode)SpecDocument.CloneNode(source.Info);
            var titleKey = new YamlScalarNode("title");
            info.Children[titleKey] = new YamlScalarNode($"OpenAI API — {area.Name}");
            root.Add("info", info);
        }

        // Copy the servers section.

        if (source.Servers != null)
        {
            root.Add("servers", (YamlSequenceNode)SpecDocument.CloneNode(source.Servers));
        }

        // Copy the security section.

        if (source.Security != null)
        {
            root.Add("security", (YamlSequenceNode)SpecDocument.CloneNode(source.Security));
        }

        // Include only tags used by this feature's operations.

        var usedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (_, pathItem) in areaPaths)
        {
            foreach (var (_, op) in SpecDocument.GetOperations(pathItem))
            {
                foreach (var tag in SpecDocument.GetOperationTags(op))
                {
                    usedTags.Add(tag);
                }
            }
        }

        if (source.Tags != null && usedTags.Count > 0)
        {
            var tagsSeq = new YamlSequenceNode();
            foreach (var tagNode in source.Tags.Children)
            {
                if (tagNode is not YamlMappingNode tagMapping)
                {
                    continue;
                }

                var nameKey = new YamlScalarNode("name");
                if (!tagMapping.Children.TryGetValue(nameKey, out var nameNode)
                    || nameNode is not YamlScalarNode nameScalar)
                {
                    continue;
                }

                if (usedTags.Contains(nameScalar.Value ?? ""))
                {
                    tagsSeq.Add(SpecDocument.CloneNode(tagMapping));
                }
            }

            if (tagsSeq.Children.Count > 0)
            {
                root.Add("tags", tagsSeq);
            }
        }

        // Clone only this feature's paths.

        var pathsNode = new YamlMappingNode();
        var seedNodes = new List<YamlNode>();

        foreach (var (pathStr, pathItem) in areaPaths)
        {
            var cloned = (YamlMappingNode)SpecDocument.CloneNode(pathItem);
            pathsNode.Add(pathStr, cloned);
            seedNodes.Add(cloned);
        }
        root.Add("paths", pathsNode);

        // Transitively resolve all $ref targets for components and schemas.

        var referencedSchemas = RefResolver.ResolveTransitive(source, seedNodes);
        if (referencedSchemas.Count > 0 && source.Schemas != null)
        {
            var schemasNode = new YamlMappingNode();
            foreach (var schemaName in referencedSchemas.OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
            {
                var key = new YamlScalarNode(schemaName);
                if (source.Schemas.Children.TryGetValue(key, out var schemaValue))
                {
                    schemasNode.Add(schemaName, SpecDocument.CloneNode(schemaValue));
                }
            }

            var componentsNode = new YamlMappingNode();
            componentsNode.Add("schemas", schemasNode);

            // Include security schemes if present.

            if (source.Components != null)
            {
                var secSchemesKey = new YamlScalarNode("securitySchemes");
                if (source.Components.Children.TryGetValue(secSchemesKey, out var secSchemes))
                {
                    componentsNode.Add("securitySchemes", SpecDocument.CloneNode(secSchemes));
                }
            }

            root.Add("components", componentsNode);
        }

        return new SpecDocument(root);
    }

    /// <summary>
    /// Splits the cleaned spec into one SpecDocument per feature area.
    /// Each output spec is self-contained with all transitively referenced schemas.
    /// Returns a dictionary mapping feature area name to its spec.
    /// Also returns a list of unassigned paths.
    /// </summary>
    /// <param name="cleaned"> The cleaned spec document to split. </param>
    /// <returns> A tuple of feature specs and unassigned paths. </returns>
    public static (Dictionary<string, SpecDocument> Features, List<UnassignedPath> UnassignedPaths) Split(SpecDocument cleaned)
    {
        var features = new Dictionary<string, SpecDocument>(StringComparer.OrdinalIgnoreCase);
        var unassigned = new List<UnassignedPath>();

        // Classify all paths by feature area.

        var featurePathMap = new Dictionary<string, List<(string Path, YamlMappingNode PathItem)>>(StringComparer.OrdinalIgnoreCase);
        foreach (var area in FeatureAreaConfig.All)
        {
            featurePathMap[area.Name] = [];
        }

        var paths = cleaned.Paths;
        if (paths == null)
        {
            Console.WriteLine("  Warning: No paths found in cleaned spec");
            return (features, unassigned);
        }

        foreach (var (pathKey, pathValue) in paths.Children)
        {
            if (pathKey is not YamlScalarNode pathScalar || pathScalar.Value == null)
            {
                continue;
            }

            if (pathValue is not YamlMappingNode pathItem)
            {
                continue;
            }

            var pathStr = pathScalar.Value;

            // Determine tags from any operation on this path.

            var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (_, op) in SpecDocument.GetOperations(pathItem))
            {
                foreach (var tag in SpecDocument.GetOperationTags(op))
                {
                    allTags.Add(tag);
                }
            }

            var featureArea = FeatureAreaConfig.FindFeatureArea(pathStr, allTags);

            if (featureArea != null)
            {
                featurePathMap[featureArea.Name].Add((pathStr, pathItem));
            }
            else
            {
                var methods = new List<string>();
                var operationIds = new List<string>();

                foreach (var (method, op) in SpecDocument.GetOperations(pathItem))
                {
                    methods.Add(method.ToUpperInvariant());

                    if (op.Children.TryGetValue(new YamlScalarNode("operationId"), out var idNode)
                        && idNode is YamlScalarNode { Value: { Length: > 0 } operationId })
                    {
                        operationIds.Add(operationId);
                    }
                }

                unassigned.Add(new UnassignedPath
                {
                    Path = pathStr,
                    Methods = methods,
                    OperationIds = operationIds,
                    Tags = [.. allTags.OrderBy(t => t, StringComparer.Ordinal)]
                });
            }
        }

        // Build a self-contained spec for each feature area.

        foreach (var area in FeatureAreaConfig.All)
        {
            var areaPaths = featurePathMap[area.Name];
            if (areaPaths.Count == 0)
            {
                Console.WriteLine($"  Warning: No paths found for feature '{area.Name}'. Skipping.");
                continue;
            }

            var featureSpec = BuildFeatureSpec(cleaned, area, areaPaths);
            features[area.Name] = featureSpec;
        }

        return (features, unassigned);
    }
}
