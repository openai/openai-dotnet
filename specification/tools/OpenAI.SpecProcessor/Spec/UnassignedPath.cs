using System.Text.Json.Serialization;

namespace OpenAI.SpecProcessor.Spec;

/// <summary> How an unassigned path compares to the previous snapshot. </summary>
/// <remarks>
/// A gap that has been reviewed and deliberately left open should stay visible, but it should not
/// keep announcing itself as a fresh regression every month. That is how a standing callout becomes
/// noise a reviewer learns to scroll past.
///
/// This is written to metadata by name rather than by ordinal. The file is read by a person
/// reviewing a pull request as much as by the next run, and a bare number tells them nothing.
///
/// The numeric values are nonetheless assigned explicitly and must not be changed or reused. An
/// earlier build wrote these as numbers, so those values are still out there in existing snapshots
/// and are still accepted on read. Renumbering or inserting a member in the middle would silently
/// re-point an old file at the wrong status, which is worse than failing to read it: a `Resolved`
/// that comes back as `Unchanged` is a gap that reopens itself in the report with no defect anyone
/// can see. Add new members at the end, with the next unused value.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter<UnassignedStatus>))]
public enum UnassignedStatus
{
    /// <summary> The path was not unassigned in the previous snapshot. </summary>
    New = 0,

    /// <summary> The path was unassigned in the previous snapshot too. </summary>
    Unchanged = 1,

    /// <summary> The path was unassigned in the previous snapshot and is not any more. </summary>
    Resolved = 2
}

/// <summary>
/// A path the split could not place, along with the context needed to decide where it belongs.
/// </summary>
/// <remarks>
/// A bare path forces a reviewer back into the raw specification to answer the first questions they
/// will have: what does it do, and why did nothing claim it. Carrying that context into the report
/// is what makes the callout actionable rather than merely alarming.
/// </remarks>
public record UnassignedPath
{
    /// <summary> The path that matched no feature area. </summary>
    public required string Path { get; init; }

    /// <summary> The HTTP methods defined on the path. </summary>
    public IReadOnlyList<string> Methods { get; init; } = [];

    /// <summary> The operation identifiers defined on the path. </summary>
    public IReadOnlyList<string> OperationIds { get; init; } = [];

    /// <summary> The tags carried by the operations on the path. </summary>
    public IReadOnlyList<string> Tags { get; init; } = [];

    /// <summary> How this path compares to the previous snapshot. </summary>
    public UnassignedStatus Status { get; init; } = UnassignedStatus.New;

    /// <summary> A plain description of why no feature area claimed the path. </summary>
    public string Reason =>
        (Status == UnassignedStatus.Resolved)
            ? "A feature area now claims this path. It no longer needs attention."
            : Tags.Count == 0
                ? "The path carries no tags, and no feature area claims its prefix."
                : $"No feature area claims the prefix, and no area owns the tag(s): {string.Join(", ", Tags)}.";

    /// <summary>
    /// Compares the paths this run could not place against the ones the previous run could not place,
    /// labelling each and carrying forward anything that has since been resolved.
    /// </summary>
    /// <param name="current"> The paths this run could not place. </param>
    /// <param name="previous"> The paths the previous run could not place, if that is known. </param>
    /// <returns> The current paths labelled by status, followed by any that were resolved. </returns>
    public static IReadOnlyList<UnassignedPath> Reconcile(
        IReadOnlyList<UnassignedPath> current,
        IReadOnlyList<UnassignedPath>? previous)
    {
        // Without a previous snapshot there is nothing to compare against. Calling everything new
        // would be a guess, but it is the honest one: this run has no evidence either way, and the
        // bootstrap case is exactly when every gap is worth a fresh look.

        if (previous == null)
        {
            return current;
        }

        var priorPaths = previous.Select(entry => entry.Path).ToHashSet(StringComparer.Ordinal);
        var currentPaths = current.Select(entry => entry.Path).ToHashSet(StringComparer.Ordinal);

        var reconciled = current
            .Select(entry => entry with
            {
                Status = priorPaths.Contains(entry.Path) ? UnassignedStatus.Unchanged : UnassignedStatus.New
            })
            .ToList();

        reconciled.AddRange(previous
            .Where(entry => !currentPaths.Contains(entry.Path))
            .Select(entry => entry with { Status = UnassignedStatus.Resolved }));

        return reconciled;
    }
}
