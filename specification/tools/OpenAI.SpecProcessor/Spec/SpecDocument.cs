using YamlDotNet.RepresentationModel;

namespace OpenAI.SpecProcessor.Spec;

/// <summary>
/// Wraps a parsed OpenAPI 3.1.0 spec as a YamlMappingNode tree,
/// providing convenient accessors and manipulation methods.
/// </summary>
public class SpecDocument
{
    // Supported HTTP methods used when extracting operations from path items.

    private static readonly string[] _httpMethods = ["get", "post", "put", "patch", "delete", "head", "options", "trace"];

    private readonly YamlMappingNode _root;

    /// <summary> Gets the root mapping node of the YAML document. </summary>
    public YamlMappingNode Root => _root;

    /// <summary> Gets the OpenAPI version string from the spec. </summary>
    public string? OpenApiVersion => GetScalar("openapi");

    /// <summary> Gets the spec version from the info section. </summary>
    public string? SpecVersion => GetNestedScalar("info", "version");

    /// <summary> Gets the title from the info section. </summary>
    public string? Title => GetNestedScalar("info", "title");

    /// <summary> Gets the paths mapping node. </summary>
    public YamlMappingNode? Paths => GetMapping("paths");

    /// <summary> Gets the schemas mapping node from components. </summary>
    public YamlMappingNode? Schemas => GetNestedMapping("components", "schemas");

    /// <summary> Gets the tags sequence node. </summary>
    public YamlSequenceNode? Tags => GetSequence("tags");

    /// <summary> Gets the info mapping node. </summary>
    public YamlMappingNode? Info => GetMapping("info");

    /// <summary> Gets the servers sequence node. </summary>
    public YamlSequenceNode? Servers => GetSequence("servers");

    /// <summary> Gets the security sequence node. </summary>
    public YamlSequenceNode? Security => GetSequence("security");

    /// <summary> Gets the components mapping node. </summary>
    public YamlMappingNode? Components => GetMapping("components");

    /// <summary> Returns path keys (e.g., "/responses/{response_id}"). </summary>
    public IEnumerable<string> PathKeys
        => Paths?.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!) ?? [];

    /// <summary> Returns schema names from components/schemas. </summary>
    public IEnumerable<string> SchemaNames
        => Schemas?.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!) ?? [];

    /// <summary> Returns tag names from the top-level tags array. </summary>
    public IEnumerable<string> TagNames
        => Tags?.Children.OfType<YamlMappingNode>()
            .Select(t => ((YamlScalarNode)t.Children[new YamlScalarNode("name")]).Value!)
           ?? [];

    /// <summary> Initializes a new instance of the <see cref="SpecDocument"/> class. </summary>
    /// <param name="root"> The root YAML mapping node for the spec. </param>
    public SpecDocument(YamlMappingNode root)
    {
        _root = root ?? throw new ArgumentNullException(nameof(root));
    }

    private static YamlMappingNode CloneMapping(YamlMappingNode source)
    {
        var clone = new YamlMappingNode { Style = source.Style, Tag = source.Tag };
        foreach (var (key, value) in source.Children)
        {
            clone.Add(CloneNode(key), CloneNode(value));
        }
        return clone;
    }

    private static YamlSequenceNode CloneSequence(YamlSequenceNode source)
    {
        var clone = new YamlSequenceNode { Style = source.Style, Tag = source.Tag };
        foreach (var child in source.Children)
        {
            clone.Add(CloneNode(child));
        }
        return clone;
    }

    private string? GetScalar(string key)
    {
        var k = new YamlScalarNode(key);
        return _root.Children.TryGetValue(k, out var v) && v is YamlScalarNode s ? s.Value : null;
    }

    private string? GetNestedScalar(string key1, string key2)
    {
        var mapping = GetMapping(key1);
        if (mapping == null)
        {
            return null;
        }

        var k = new YamlScalarNode(key2);
        return mapping.Children.TryGetValue(k, out var v) && v is YamlScalarNode s ? s.Value : null;
    }

    private YamlMappingNode? GetMapping(string key)
    {
        var k = new YamlScalarNode(key);
        return _root.Children.TryGetValue(k, out var v) && v is YamlMappingNode m ? m : null;
    }

    private YamlMappingNode? GetNestedMapping(string key1, string key2)
    {
        var outer = GetMapping(key1);
        if (outer == null)
        {
            return null;
        }

        var k = new YamlScalarNode(key2);
        return outer.Children.TryGetValue(k, out var v) && v is YamlMappingNode m ? m : null;
    }

    private YamlSequenceNode? GetSequence(string key)
    {
        var k = new YamlScalarNode(key);
        return _root.Children.TryGetValue(k, out var v) && v is YamlSequenceNode s ? s : null;
    }

    /// <summary> Loads a spec document from the specified file path. </summary>
    /// <param name="path"> The file path to load the YAML spec from. </param>
    /// <param name="onRepair"> Invoked when the document needed repair before it could be parsed. </param>
    /// <returns> A new <see cref="SpecDocument"/> instance. </returns>
    public static SpecDocument Load(string path, Action<int>? onRepair = null)
    {
        var text = SpecSanitizer.Repair(File.ReadAllText(path), out var repairedLines);

        if (repairedLines > 0)
        {
            onRepair?.Invoke(repairedLines);
        }

        using var reader = new StringReader(text);
        var yaml = new YamlStream();
        yaml.Load(reader);

        if (yaml.Documents.Count == 0)
        {
            throw new InvalidOperationException($"No YAML documents found in: {path}");
        }

        return new SpecDocument((YamlMappingNode)yaml.Documents[0].RootNode);
    }

    /// <summary> Deep clones a YAML node and all of its children. </summary>
    /// <param name="node"> The YAML node to clone. </param>
    /// <returns> A deep copy of the node. </returns>
    public static YamlNode CloneNode(YamlNode node)
    {
        return node switch
        {
            YamlMappingNode mapping => CloneMapping(mapping),
            YamlSequenceNode sequence => CloneSequence(sequence),
            YamlScalarNode scalar => new YamlScalarNode(scalar.Value) { Style = scalar.Style, Tag = scalar.Tag },
            _ => throw new InvalidOperationException($"Unknown YAML node type: {node.GetType()}")
        };
    }

    /// <summary> Returns all operations under a given path entry, yielding (method, operationNode). </summary>
    /// <param name="pathItem"> The path item mapping node to extract operations from. </param>
    /// <returns> An enumerable of method and operation node pairs. </returns>
    public static IEnumerable<(string Method, YamlMappingNode Operation)> GetOperations(YamlMappingNode pathItem)
    {
        foreach (var method in _httpMethods)
        {
            var key = new YamlScalarNode(method);
            if (pathItem.Children.TryGetValue(key, out var opNode) && opNode is YamlMappingNode opMapping)
            {
                yield return (method, opMapping);
            }
        }
    }

    /// <summary> Extracts tags from an operation node, returning an empty list if none. </summary>
    /// <param name="operation"> The operation mapping node to extract tags from. </param>
    /// <returns> A read-only list of tag strings. </returns>
    public static IReadOnlyList<string> GetOperationTags(YamlMappingNode operation)
    {
        var tagsKey = new YamlScalarNode("tags");
        if (!operation.Children.TryGetValue(tagsKey, out var tagsNode) || tagsNode is not YamlSequenceNode tagsSeq)
        {
            return [];
        }

        return tagsSeq.Children
            .OfType<YamlScalarNode>()
            .Select(s => s.Value!)
            .ToList();
    }

    /// <summary> Saves the spec document to the specified file path. </summary>
    /// <param name="path"> The file path to save the YAML spec to. </param>
    public void Save(string path)
    {
        var doc = new YamlDocument(_root);
        var stream = new YamlStream(doc);

        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        // Serialize in memory so the output can be normalized to LF before it lands on disk.
        // These files are the month-over-month comparison baseline, so they must be identical
        // no matter which platform produced them.

        using var buffer = new StringWriter();
        stream.Save(buffer, false);

        LineEndings.WriteAllText(path, buffer.ToString());
    }

    /// <summary> Creates a deep clone of this spec document. </summary>
    /// <returns> A new <see cref="SpecDocument"/> instance with cloned data. </returns>
    public SpecDocument Clone()
    {
        return new SpecDocument((YamlMappingNode)CloneNode(_root));
    }
}
