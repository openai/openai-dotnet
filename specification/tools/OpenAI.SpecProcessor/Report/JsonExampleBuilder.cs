using System.Text;
using OpenAI.SpecProcessor.Spec;
using YamlDotNet.RepresentationModel;

namespace OpenAI.SpecProcessor.Report;

/// <summary> Builds structural JSON examples from OpenAPI schema definitions. </summary>
public static class JsonExampleBuilder
{
    private const int MaxDepth = 6;

    /// <summary> Builds a structural JSON example string from a schema. </summary>
    /// <param name="schema"> The schema to render. </param>
    /// <param name="spec"> The spec document for resolving $ref targets. </param>
    /// <returns> A JSON-like string showing the structure with types. </returns>
    public static string Build(SchemaInfo schema, SpecDocument spec)
    {
        var sb = new StringBuilder();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        WriteValue(sb, schema, spec, indent: 0, visited);
        return sb.ToString();
    }

    private static void WriteValue(StringBuilder sb, SchemaInfo schema, SpecDocument spec, int indent, HashSet<string> visited)
    {
        if (indent > MaxDepth)
        {
            sb.Append("{ ... }");
            return;
        }

        // Enum: show first value as example.

        if (schema.EnumValues != null && schema.EnumValues.Count > 0)
        {
            sb.Append($"\"{schema.EnumValues[0]}\"");
            return;
        }

        // Composition: pick the most likely branch.

        if (schema.CompositionType != null && schema.CompositionRefs != null && schema.CompositionRefs.Count > 0)
        {
            var bestRef = PickBestOneOfBranch(schema.CompositionRefs, spec);

            if (bestRef != null)
            {
                var resolved = ResolveSchemaByName(bestRef, spec);

                if (resolved != null && visited.Add(bestRef))
                {
                    WriteValue(sb, resolved, spec, indent, visited);
                    visited.Remove(bestRef);
                    return;
                }
            }

            sb.Append($"/* {schema.CompositionType}: {string.Join(" | ", schema.CompositionRefs)} */");
            return;
        }

        // Object with properties.

        if (schema.Properties != null && schema.Properties.Count > 0)
        {
            sb.AppendLine("{");
            var props = schema.Properties;

            for (int i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                var prefix = new string(' ', (indent + 1) * 2);
                sb.Append($"{prefix}\"{prop.Name}\": ");
                WritePropertyValue(sb, prop, spec, indent + 1, visited);

                if (i < props.Count - 1)
                {
                    sb.Append(',');
                }

                // Add a comment for required properties.

                if (prop.IsRequired)
                {
                    sb.Append("  // required");
                }

                sb.AppendLine();
            }

            sb.Append($"{new string(' ', indent * 2)}}}");
            return;
        }

        // Simple types.

        var type = schema.Type ?? "unknown";
        sb.Append(type switch
        {
            "string" => "\"string\"",
            "integer" => "0",
            "number" => "0.0",
            "boolean" => "false",
            "array" => "[]",
            "null" => "null",
            _ => $"/* {type} */"
        });
    }

    private static void WritePropertyValue(
        StringBuilder sb,
        SchemaProperty prop,
        SpecDocument spec,
        int indent,
        HashSet<string> visited)
    {
        if (indent > MaxDepth)
        {
            sb.Append("\"...\"");
            return;
        }

        // $ref property: resolve and recurse.

        if (prop.RefTarget != null)
        {
            var resolved = ResolveSchemaByName(prop.RefTarget, spec);

            if (resolved != null && visited.Add(prop.RefTarget))
            {
                WriteValue(sb, resolved, spec, indent, visited);
                visited.Remove(prop.RefTarget);
                return;
            }

            sb.Append($"/* {prop.RefTarget} */");
            return;
        }

        // Array property.

        if (prop.Type.StartsWith("array", StringComparison.Ordinal))
        {
            if (prop.ItemType != null)
            {
                var itemSchema = ResolveSchemaByName(prop.ItemType, spec);

                if (itemSchema != null && visited.Add(prop.ItemType))
                {
                    sb.Append('[');
                    WriteValue(sb, itemSchema, spec, indent, visited);
                    visited.Remove(prop.ItemType);
                    sb.Append(']');
                    return;
                }
            }

            sb.Append("[]");
            return;
        }

        // oneOf/anyOf property (embedded in type description).

        if (prop.Type.Contains("oneOf", StringComparison.Ordinal) || prop.Type.Contains("anyOf", StringComparison.Ordinal))
        {
            // Try to extract a ref name from the type description.

            var refs = ExtractRefsFromTypeDescription(prop.Type);

            if (refs.Count > 0)
            {
                var bestRef = PickBestOneOfBranch(refs, spec);

                if (bestRef != null)
                {
                    var resolved = ResolveSchemaByName(bestRef, spec);

                    if (resolved != null && visited.Add(bestRef))
                    {
                        WriteValue(sb, resolved, spec, indent, visited);
                        visited.Remove(bestRef);
                        return;
                    }
                }
            }

            sb.Append($"/* {prop.Type} */");
            return;
        }

        // Simple typed property.

        sb.Append(prop.Type switch
        {
            "string" => "\"string\"",
            "string enum" => "\"string\"",
            "integer" or "integer(int64)" or "integer(int32)" => "0",
            "number" or "number(double)" or "number(float)" => "0.0",
            "boolean" => "false",
            "object" => "{}",
            "null" => "null",
            _ when prop.Type.EndsWith("enum", StringComparison.Ordinal) => "\"string\"",
            _ => $"/* {prop.Type} */"
        });
    }

    // Picks the most likely oneOf/anyOf branch: prefer object schemas over primitives,
    // prefer schemas with more properties, avoid "null" branches.
    private static string? PickBestOneOfBranch(IReadOnlyList<string> refs, SpecDocument spec)
    {
        string? bestRef = null;
        int bestScore = -1;

        foreach (var refName in refs)
        {
            if (refName == "null" || refName == "string" || refName == "integer" || refName == "boolean")
            {
                continue;
            }

            var schema = ResolveSchemaByName(refName, spec);

            if (schema == null)
            {
                if (bestRef == null)
                {
                    bestRef = refName;
                }

                continue;
            }

            int score = 0;

            if (schema.Properties != null)
            {
                score = schema.Properties.Count + 10;
            }
            else if (schema.CompositionRefs != null)
            {
                score = schema.CompositionRefs.Count + 5;
            }
            else if (schema.EnumValues != null)
            {
                score = 1;
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestRef = refName;
            }
        }

        return bestRef ?? refs.FirstOrDefault(r => r != "null");
    }

    private static SchemaInfo? ResolveSchemaByName(string name, SpecDocument spec)
    {
        // Primitive types don't resolve.

        if (name is "string" or "integer" or "number" or "boolean" or "null" or "object" or "array" or "inline")
        {
            return new SchemaInfo { Name = name, Type = name };
        }

        var schemas = spec.Schemas;

        if (schemas == null)
        {
            return null;
        }

        if (!schemas.Children.TryGetValue(new YamlScalarNode(name), out var schemaNode)
            || schemaNode is not YamlMappingNode schemaMap)
        {
            return null;
        }

        return ExtractSchemaInfoFromNode(name, schemaMap);
    }

    private static SchemaInfo ExtractSchemaInfoFromNode(string name, YamlMappingNode schema)
    {
        var type = GetScalar(schema, "type");
        var enumValues = GetEnumValues(schema);
        var required = GetRequiredSet(schema);

        List<SchemaProperty>? properties = null;
        var propsNode = GetMapping(schema, "properties");

        if (propsNode != null)
        {
            properties = [];

            foreach (var (key, value) in propsNode.Children)
            {
                if (key is not YamlScalarNode scalarKey || scalarKey.Value == null)
                {
                    continue;
                }

                var propName = scalarKey.Value;
                var propType = DescribePropertyType(value);
                string? itemType = null;
                string? refTarget = null;

                if (value is YamlMappingNode propMap)
                {
                    itemType = GetArrayItemType(propMap);
                    refTarget = GetRefTarget(propMap);
                }

                properties.Add(new SchemaProperty
                {
                    Name = propName,
                    Type = propType,
                    IsRequired = required.Contains(propName),
                    ItemType = itemType,
                    RefTarget = refTarget
                });
            }
        }

        string? compositionType = null;
        List<string>? compositionRefs = null;

        foreach (var compKey in new[] { "anyOf", "oneOf", "allOf" })
        {
            var refs = GetCompositionRefs(schema, compKey);

            if (refs.Count > 0)
            {
                compositionType = compKey;
                compositionRefs = refs;
                break;
            }
        }

        return new SchemaInfo
        {
            Name = name,
            Type = type,
            EnumValues = enumValues,
            Properties = properties,
            Required = required.Count > 0 ? required.ToList() : null,
            CompositionRefs = compositionRefs,
            CompositionType = compositionType
        };
    }

    // Extracts ref names from a type description like "oneOf(Foo | Bar | null)".
    private static List<string> ExtractRefsFromTypeDescription(string typeDesc)
    {
        var refs = new List<string>();
        var parenStart = typeDesc.IndexOf('(');
        var parenEnd = typeDesc.LastIndexOf(')');

        if (parenStart < 0 || parenEnd <= parenStart)
        {
            return refs;
        }

        var inner = typeDesc[(parenStart + 1)..parenEnd];

        foreach (var part in inner.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            refs.Add(part.Trim());
        }

        return refs;
    }

    // ── Helpers mirroring SpecDiffer's private methods ────────────────────

    private static string? GetScalar(YamlMappingNode node, string key)
    {
        return node.Children.TryGetValue(new YamlScalarNode(key), out var v) && v is YamlScalarNode s ? s.Value : null;
    }

    private static YamlMappingNode? GetMapping(YamlMappingNode node, string key)
    {
        return node.Children.TryGetValue(new YamlScalarNode(key), out var v) && v is YamlMappingNode m ? m : null;
    }

    private static HashSet<string> GetRequiredSet(YamlMappingNode schema)
    {
        if (!schema.Children.TryGetValue(new YamlScalarNode("required"), out var reqNode) || reqNode is not YamlSequenceNode reqSeq)
        {
            return [];
        }

        return reqSeq.Children.OfType<YamlScalarNode>().Select(s => s.Value!).ToHashSet(StringComparer.Ordinal);
    }

    private static List<string>? GetEnumValues(YamlMappingNode schema)
    {
        if (!schema.Children.TryGetValue(new YamlScalarNode("enum"), out var enumNode) || enumNode is not YamlSequenceNode enumSeq)
        {
            return null;
        }

        return enumSeq.Children.OfType<YamlScalarNode>().Select(s => s.Value ?? "null").ToList();
    }

    private static string DescribePropertyType(YamlNode node)
    {
        if (node is not YamlMappingNode map)
        {
            return node is YamlScalarNode s ? s.Value ?? "unknown" : "unknown";
        }

        var refVal = GetScalar(map, "$ref");

        if (refVal != null)
        {
            var lastSlash = refVal.LastIndexOf('/');
            return lastSlash >= 0 ? refVal[(lastSlash + 1)..] : refVal;
        }

        var type = GetScalar(map, "type");

        if (GetEnumValues(map) != null)
        {
            return $"{type ?? "string"} enum";
        }

        if (type == "array")
        {
            var itemType = GetArrayItemType(map);
            return itemType != null ? $"array<{itemType}>" : "array";
        }

        foreach (var compKey in new[] { "anyOf", "oneOf", "allOf" })
        {
            var refs = GetCompositionRefs(map, compKey);

            if (refs.Count > 0)
            {
                return $"{compKey}({string.Join(" | ", refs)})";
            }
        }

        var format = GetScalar(map, "format");

        if (format != null && type != null)
        {
            return $"{type}({format})";
        }

        return type ?? "unknown";
    }

    private static string? GetArrayItemType(YamlMappingNode schema)
    {
        var items = GetMapping(schema, "items");

        if (items == null)
        {
            return null;
        }

        var refVal = GetScalar(items, "$ref");

        if (refVal != null)
        {
            var lastSlash = refVal.LastIndexOf('/');
            return lastSlash >= 0 ? refVal[(lastSlash + 1)..] : refVal;
        }

        return GetScalar(items, "type");
    }

    private static string? GetRefTarget(YamlMappingNode schema)
    {
        var refVal = GetScalar(schema, "$ref");

        if (refVal == null)
        {
            return null;
        }

        var lastSlash = refVal.LastIndexOf('/');
        return lastSlash >= 0 ? refVal[(lastSlash + 1)..] : refVal;
    }

    private static List<string> GetCompositionRefs(YamlMappingNode schema, string compKey)
    {
        if (!schema.Children.TryGetValue(new YamlScalarNode(compKey), out var compNode) || compNode is not YamlSequenceNode compSeq)
        {
            return [];
        }

        var refs = new List<string>();

        foreach (var item in compSeq.Children)
        {
            if (item is YamlMappingNode itemMap)
            {
                var refVal = GetScalar(itemMap, "$ref");

                if (refVal != null)
                {
                    var lastSlash = refVal.LastIndexOf('/');
                    refs.Add(lastSlash >= 0 ? refVal[(lastSlash + 1)..] : refVal);
                }
                else
                {
                    var itemType = GetScalar(itemMap, "type");
                    refs.Add(itemType ?? "inline");
                }
            }
        }

        return refs;
    }
}
