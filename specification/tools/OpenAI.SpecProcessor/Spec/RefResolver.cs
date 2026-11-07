using YamlDotNet.RepresentationModel;

namespace OpenAI.SpecProcessor.Spec;

/// <summary>
/// Resolves $ref chains transitively to collect all schemas
/// needed for a self-contained feature spec.
/// </summary>
public static class RefResolver
{
    private const string RefKey = "$ref";
    private const string SchemaRefPrefix = "#/components/schemas/";

    private static void CollectRefTargets(YamlNode node, Queue<string> queue, HashSet<string> resolved)
    {
        switch (node)
        {
            case YamlMappingNode mapping:
                var refKey = new YamlScalarNode(RefKey);
                if (mapping.Children.TryGetValue(refKey, out var refValue) && refValue is YamlScalarNode refScalar)
                {
                    var refPath = refScalar.Value;
                    if (refPath?.StartsWith(SchemaRefPrefix) == true)
                    {
                        var schemaName = refPath[SchemaRefPrefix.Length..];
                        if (resolved.Add(schemaName))
                        {
                            queue.Enqueue(schemaName);
                        }
                    }
                }

                foreach (var (_, value) in mapping.Children)
                {
                    CollectRefTargets(value, queue, resolved);
                }
                break;

            case YamlSequenceNode sequence:
                foreach (var child in sequence.Children)
                {
                    CollectRefTargets(child, queue, resolved);
                }
                break;
        }
    }

    private static void CollectAllRefsRecursive(YamlNode node, HashSet<string> refs)
    {
        switch (node)
        {
            case YamlMappingNode mapping:
                var refKey = new YamlScalarNode(RefKey);
                if (mapping.Children.TryGetValue(refKey, out var refValue) && refValue is YamlScalarNode refScalar)
                {
                    if (refScalar.Value != null)
                    {
                        refs.Add(refScalar.Value);
                    }
                }

                foreach (var (_, value) in mapping.Children)
                {
                    CollectAllRefsRecursive(value, refs);
                }
                break;

            case YamlSequenceNode sequence:
                foreach (var child in sequence.Children)
                {
                    CollectAllRefsRecursive(child, refs);
                }
                break;
        }
    }

    /// <summary>
    /// Given a set of seed YAML nodes (e.g., operation definitions),
    /// walks all $ref targets and returns the full transitive closure
    /// of schema names referenced.
    /// </summary>
    /// <param name="spec"> The spec document containing schema definitions. </param>
    /// <param name="seedNodes"> The initial YAML nodes to scan for $ref targets. </param>
    /// <returns> The set of all transitively referenced schema names. </returns>
    public static HashSet<string> ResolveTransitive(
        SpecDocument spec,
        IEnumerable<YamlNode> seedNodes)
    {
        var schemas = spec.Schemas;
        if (schemas == null)
        {
            return [];
        }

        var resolved = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>();

        // Collect direct $ref targets from seed nodes.

        foreach (var node in seedNodes)
        {
            CollectRefTargets(node, queue, resolved);
        }

        // Walk transitive references.

        while (queue.Count > 0)
        {
            var schemaName = queue.Dequeue();
            var key = new YamlScalarNode(schemaName);

            if (!schemas.Children.TryGetValue(key, out var schemaNode))
            {
                continue;
            }

            CollectRefTargets(schemaNode, queue, resolved);
        }

        return resolved;
    }

    /// <summary>
    /// Collects all $ref strings (not just schema refs) from a node tree.
    /// Useful for validation — checking for dangling references.
    /// </summary>
    /// <param name="node"> The root YAML node to scan. </param>
    /// <returns> The set of all $ref string values found. </returns>
    public static HashSet<string> CollectAllRefs(YamlNode node)
    {
        var refs = new HashSet<string>(StringComparer.Ordinal);
        CollectAllRefsRecursive(node, refs);
        return refs;
    }
}
