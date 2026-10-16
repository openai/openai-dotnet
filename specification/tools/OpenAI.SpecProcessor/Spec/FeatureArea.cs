namespace OpenAI.SpecProcessor.Spec;

/// <summary> Defines a feature area with its tag/path matching rules. </summary>
public record FeatureArea
{
    /// <summary> The display name of the feature area. </summary>
    public required string Name { get; init; }

    /// <summary> The output file name for this feature area's spec. </summary>
    public required string OutputFile { get; init; }

    /// <summary> Tags that identify operations belonging to this feature area. </summary>
    public required string[] Tags { get; init; }

    /// <summary> URL path prefixes that match operations for this feature area. </summary>
    public required string[] PathPrefixes { get; init; }

    /// <summary> Path prefixes to exclude from this feature area even if they match. </summary>
    public string[] ExcludedPathPrefixes { get; init; } = [];

    /// <summary> Exact paths that belong to this feature area regardless of prefix matching. </summary>
    public string[] ExplicitPaths { get; init; } = [];
}
