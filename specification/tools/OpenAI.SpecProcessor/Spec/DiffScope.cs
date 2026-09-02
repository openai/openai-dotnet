namespace OpenAI.SpecProcessor.Spec;

/// <summary>
/// Describes what the structural comparison covers and which of its findings are heuristic.
/// </summary>
/// <remarks>
/// The report carries the same statement in prose, but prose is easy for a downstream agent to skim
/// past. Recording it in metadata means a consumer can tell how much weight the report deserves
/// without parsing it, and can notice when the scope itself changes between snapshots.
/// </remarks>
public record DiffScope
{
    /// <summary> The scope in effect for snapshots produced by this version of the tool. </summary>
    public static DiffScope Current { get; } = new();

    /// <summary>
    /// The revision of the comparison scope. Increment this whenever what is compared changes, so
    /// that a difference in report coverage is visible rather than inferred.
    /// </summary>
    public int Version { get; init; } = 1;

    /// <summary> The structure the comparison covers. </summary>
    public IReadOnlyList<string> Compared { get; init; } =
    [
        "paths",
        "operations",
        "operation summaries and descriptions",
        "schemas",
        "schema properties",
        "required flags",
        "types and formats",
        "enum values",
        "compositions and discriminators",
        "common schema constraints"
    ];

    /// <summary> The structure the comparison does not cover. </summary>
    public IReadOnlyList<string> NotCompared { get; init; } =
    [
        "parameter style and explode",
        "request body encodings",
        "response headers",
        "callbacks",
        "links",
        "operation-level servers and security",
        "examples"
    ];

    /// <summary> The findings produced by similarity heuristics rather than exact comparison. </summary>
    public IReadOnlyList<string> Heuristic { get; init; } =
    [
        "renames",
        "possible duplicates",
        "duplicate operations",
        "anomalies"
    ];
}
