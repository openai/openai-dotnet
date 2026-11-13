using System.Text;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Report;

/// <summary> Generates a rich markdown diff report using GitHub-flavored markdown. </summary>
public static class DiffReportWriter
{
    /// <summary> Generates a full diff report covering all feature areas. </summary>
    /// <param name="diffs"> The collection of feature diffs to include in the report. </param>
    /// <param name="specVersion"> The new spec version identifier. </param>
    /// <param name="previousVersion"> The previous spec version identifier, or null for initial baseline. </param>
    /// <param name="previousMeta"> Metadata from the previous processing run, or null. </param>
    /// <param name="lineIndexes"> Optional line indexes for each feature spec file. </param>
    /// <param name="featureSpecs"> Optional map of feature name to spec document, for JSON example generation. </param>
    /// <param name="unassignedPaths"> Paths the split could not place. </param>
    /// <param name="metadata"> The metadata recorded alongside this snapshot, used for provenance. </param>
    /// <param name="repairedLineCount"> The number of source lines the sanitizer repaired before parsing. </param>
    /// <returns> A markdown-formatted diff report as a string. </returns>
    public static string Generate(
        IReadOnlyList<FeatureDiff> diffs,
        string specVersion,
        string? previousVersion,
        SpecMetadata? previousMeta = null,
        IReadOnlyDictionary<string, LineIndex>? lineIndexes = null,
        IReadOnlyDictionary<string, SpecDocument>? featureSpecs = null,
        IReadOnlyDictionary<string, LineIndex>? previousLineIndexes = null,
        IReadOnlyList<UnassignedPath>? unassignedPaths = null,
        SpecMetadata? metadata = null,
        int repairedLineCount = 0)
    {
        var sb = new StringBuilder();
        WriteHeader(sb, specVersion, previousVersion, previousMeta);
        WriteProvenanceSection(sb, metadata, previousMeta, repairedLineCount);
        WriteUnassignedSection(sb, unassignedPaths);
        WriteSummaryTable(sb, diffs);

        foreach (var diff in diffs)
        {
            var featureArea = FeatureAreaConfig.All.FirstOrDefault(a => a.Name == diff.FeatureName);
            var fileName = featureArea?.OutputFile;
            LineIndex? lineIndex = null;
            LineIndex? prevLineIndex = null;

            if (fileName != null)
            {
                lineIndexes?.TryGetValue(fileName, out lineIndex);
                previousLineIndexes?.TryGetValue(fileName, out prevLineIndex);
            }

            SpecDocument? featureSpec = null;
            featureSpecs?.TryGetValue(diff.FeatureName, out featureSpec);

            WriteFeatureSection(sb, diff, fileName, lineIndex, featureSpec, prevLineIndex);
        }

        return sb.ToString();
    }

    private static void WriteHeader(StringBuilder sb, string specVersion, string? previousVersion, SpecMetadata? previousMeta)
    {
        var nowLabel = FormatTimestamp(DateTimeOffset.UtcNow);

        sb.AppendLine("# OpenAI REST API — Spec Diff Report");
        sb.AppendLine();

        string prevLabel;

        if (previousVersion != null)
        {
            if (previousMeta != null)
            {
                var prevTimestamp = FormatTimestamp(previousMeta.ProcessedAt);
                prevLabel = $"v{previousVersion} ({prevTimestamp})";
            }
            else
            {
                prevLabel = $"v{previousVersion} (unknown date)";
            }
        }
        else
        {
            prevLabel = "(none — initial baseline)";
        }

        sb.AppendLine("| | |");
        sb.AppendLine("|---|---|");
        sb.AppendLine($"| **New spec** | v{specVersion} ({nowLabel}) |");
        sb.AppendLine($"| **Previous spec** | {prevLabel} |");
        sb.AppendLine();
        sb.AppendLine("> **Legend**: `+` added · `-` removed · `▲` changed · `↔` renamed");
        sb.AppendLine();
        sb.AppendLine("> **Scope**: this is a structural comparison, not a complete semantic one. It");
        sb.AppendLine("> covers paths, operations, schemas, properties, required flags, types, enum values,");
        sb.AppendLine("> and the common schema constraints. Serialization details such as parameter `style`");
        sb.AppendLine("> and `explode`, request-body encodings, response headers, callbacks, and links are");
        sb.AppendLine("> not compared. Treat a clean report as \"nothing structural moved,\" not as proof that");
        sb.AppendLine("> nothing at all did.");
        sb.AppendLine();
        sb.AppendLine("> **Heuristics**: renames, possible duplicates, and anomalies are suggestions produced");
        sb.AppendLine("> by name and shape similarity. They are review prompts, not established facts, and");
        sb.AppendLine("> should be confirmed by a human before anything is based on them.");
        sb.AppendLine();

        WriteScopeChangeNote(sb, previousMeta);

        sb.AppendLine("---");
        sb.AppendLine();
    }

    /// <summary>
    /// Notes a change in what the comparison covers, when the previous snapshot was produced under a
    /// different scope.
    /// </summary>
    /// <param name="sb"> The builder to append to. </param>
    /// <param name="previousMeta"> The metadata of the snapshot being compared against. </param>
    /// <remarks>
    /// Widening the comparison makes findings appear that were always true and simply were not being
    /// looked for. Narrowing it makes findings disappear without upstream having changed. Either way
    /// the count moves for a reason that has nothing to do with the API, so it has to be called out
    /// separately or it will be read as an API change.
    /// </remarks>
    private static void WriteScopeChangeNote(StringBuilder sb, SpecMetadata? previousMeta)
    {
        var previousScope = previousMeta?.DiffScope.Version;
        var currentScope = DiffScope.Current.Version;

        if (previousScope == null || previousScope == currentScope)
        {
            return;
        }

        var direction = (currentScope > previousScope) ? "widened" : "narrowed";

        sb.AppendLine($"> **Scope change**: the comparison scope {direction} from version {previousScope} to");
        sb.AppendLine($"> {currentScope} since the previous snapshot. Some of the differences below may reflect");
        sb.AppendLine("> what is now being compared rather than anything upstream changed. Read this report");
        sb.AppendLine("> against the scope statement above rather than against last month's counts.");
        sb.AppendLine();
    }

    /// <summary>
    /// Writes what this snapshot was made from, so the report and the metadata tell the same story.
    /// </summary>
    /// <param name="sb"> The builder to append to. </param>
    /// <param name="metadata"> The metadata recorded alongside this snapshot. </param>
    /// <param name="previousMeta"> The metadata of the snapshot being compared against. </param>
    /// <param name="repairedLineCount"> The number of source lines the sanitizer repaired. </param>
    /// <remarks>
    /// A reviewer reading the report should not have to open a JSON file to find out which upstream
    /// commit the changes came from, and the two should never be able to disagree. Both are written
    /// from the same record in the same run, so this section is the metadata rendered for a human
    /// rather than a second account of it.
    ///
    /// The sanitizer is named here for the same reason. It edits the source before anything is
    /// parsed, so a reader comparing the report against the raw upstream file needs to know that a
    /// repair happened and how much of the document it touched.
    /// </remarks>
    private static void WriteProvenanceSection(StringBuilder sb, SpecMetadata? metadata, SpecMetadata? previousMeta, int repairedLineCount)
    {
        if (metadata == null)
        {
            return;
        }

        sb.AppendLine("## Where this came from");
        sb.AppendLine();
        sb.AppendLine("| | |");
        sb.AppendLine("|---|---|");
        sb.AppendLine($"| **Source** | `{metadata.Source}` |");
        sb.AppendLine($"| **Upstream commit** | {(string.IsNullOrEmpty(metadata.DownloadedCommitSha) ? "(not recorded)" : $"`{metadata.DownloadedCommitSha}`")} |");
        sb.AppendLine($"| **Source content hash** | {(string.IsNullOrEmpty(metadata.SourceContentHash) ? "(not recorded)" : $"`{metadata.SourceContentHash}`")} |");
        sb.AppendLine($"| **Previous content hash** | {(string.IsNullOrEmpty(previousMeta?.SourceContentHash) ? "(not recorded)" : $"`{previousMeta!.SourceContentHash}`")} |");
        sb.AppendLine($"| **Feature specifications** | {metadata.FeatureCount} |");
        sb.AppendLine($"| **Source repairs** | {(repairedLineCount == 0 ? "none" : $"{repairedLineCount} line(s) repaired before parsing")} |");
        sb.AppendLine($"| **Metadata schema** | v{metadata.SchemaVersion} |");
        sb.AppendLine($"| **Comparison scope** | v{DiffScope.Current.Version} |");
        sb.AppendLine();
        sb.AppendLine("Every figure here is written from the same record as `.spec-metadata.json`, so the");
        sb.AppendLine("report and the metadata cannot disagree about what was processed.");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
    }

    /// <summary>
    /// Writes the unassigned-path callout, when there is one.
    /// </summary>
    /// <param name="sb"> The builder to append to. </param>
    /// <param name="unassignedPaths"> Paths the split could not place. </param>
    /// <remarks>
    /// This sits above the summary deliberately. An unassigned path means upstream published
    /// something the feature map does not know about, so it is not carried by any feature file and
    /// would otherwise be invisible in a report organized by feature.
    /// </remarks>
    private static void WriteUnassignedSection(StringBuilder sb, IReadOnlyList<UnassignedPath>? unassignedPaths)
    {
        if (unassignedPaths is not { Count: > 0 })
        {
            return;
        }

        var open = unassignedPaths.Where(entry => entry.Status != UnassignedStatus.Resolved).ToList();
        var resolved = unassignedPaths.Where(entry => entry.Status == UnassignedStatus.Resolved).ToList();
        var added = open.Count(entry => entry.Status == UnassignedStatus.New);

        var heading = (added > 0)
            ? $"## UNASSIGNED paths ({open.Count}, {added} new)"
            : $"## UNASSIGNED paths ({open.Count})";

        sb.AppendLine(heading);
        sb.AppendLine();
        sb.AppendLine("These paths matched no feature area, so they appear in no feature specification");
        sb.AppendLine("below. This usually means upstream added a path or a tag the split does not know");
        sb.AppendLine("about, and the feature map needs a reviewed update.");
        sb.AppendLine();
        sb.AppendLine("`new` means the previous snapshot had no such gap and this one does, which is the case");
        sb.AppendLine("that needs a decision. `unchanged` is a gap that was already known, listed so it stays");
        sb.AppendLine("visible without reading as a fresh regression.");
        sb.AppendLine();
        sb.AppendLine("| Status | Path | Methods | Operations | Tags | Why |");
        sb.AppendLine("|---|---|---|---|---|---|");

        foreach (var entry in open.OrderBy(entry => entry.Status).ThenBy(entry => entry.Path, StringComparer.Ordinal))
        {
            sb.AppendLine($"| {DescribeStatus(entry.Status)} | {FormatUnassignedRow(entry)} |");
        }

        if (resolved.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"**Resolved since the previous snapshot ({resolved.Count})**");
            sb.AppendLine();
            sb.AppendLine("| Path | Methods | Operations | Tags | Why |");
            sb.AppendLine("|---|---|---|---|---|");

            foreach (var entry in resolved.OrderBy(entry => entry.Path, StringComparer.Ordinal))
            {
                sb.AppendLine($"| {FormatUnassignedRow(entry)} |");
            }
        }

        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
    }

    /// <summary> Renders the shared columns of an unassigned-path row. </summary>
    /// <param name="entry"> The path to render. </param>
    /// <returns> The row body, without the leading or trailing pipe. </returns>
    private static string FormatUnassignedRow(UnassignedPath entry)
    {
        var methods = (entry.Methods.Count > 0) ? string.Join(", ", entry.Methods) : "none";
        var operations = (entry.OperationIds.Count > 0) ? string.Join(", ", entry.OperationIds.Select(id => $"`{id}`")) : "none";
        var tags = (entry.Tags.Count > 0) ? string.Join(", ", entry.Tags) : "untagged";

        return $"`{entry.Path}` | {methods} | {operations} | {tags} | {entry.Reason}";
    }

    /// <summary> Renders an unassigned status as the label used in the report. </summary>
    /// <param name="status"> The status to render. </param>
    /// <returns> The label. </returns>
    private static string DescribeStatus(UnassignedStatus status) =>
        status switch
        {
            UnassignedStatus.New => "**new**",
            UnassignedStatus.Unchanged => "unchanged",
            _ => "resolved"
        };

    /// <summary>
    /// Formats a timestamp as ISO-8601 UTC. The report is produced by a workflow that may run on
    /// any host, so a single absolute representation keeps it unambiguous.
    /// </summary>
    /// <param name="timestamp"> The timestamp to format. </param>
    /// <returns> The ISO-8601 UTC representation. </returns>
    private static string FormatTimestamp(DateTimeOffset timestamp)
    {
        return timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    /// <summary> Converts a heading string to the GFM-compatible anchor slug. </summary>
    private static string ToAnchor(string heading)
    {
        return heading.ToLowerInvariant().Replace(' ', '-');
    }

    private static void WriteSummaryTable(StringBuilder sb, IReadOnlyList<FeatureDiff> diffs)
    {
        sb.AppendLine("## Summary");
        sb.AppendLine();
        sb.AppendLine("| Feature | Paths +/- | Ops +/-/▲ | Schemas +/-/▲/↔ | Total Changes |");
        sb.AppendLine("|---------|:---------:|:---------:|:----------------:|:-------------:|");

        int totalChanges = 0;

        foreach (var diff in diffs)
        {
            int changes = diff.Changes.Count;
            totalChanges += changes;
            var schemaRename = diff.SchemasRenamed > 0 ? $" / ↔{diff.SchemasRenamed}" : "";
            var featureLink = $"[{diff.FeatureName}](#{ToAnchor(diff.FeatureName)})";
            sb.AppendLine($"| {featureLink} | +{diff.PathsAdded} / -{diff.PathsRemoved} | +{diff.OperationsAdded} / -{diff.OperationsRemoved} / ▲{diff.OperationsChanged} | +{diff.SchemasAdded} / -{diff.SchemasRemoved} / ▲{diff.SchemasChanged}{schemaRename} | {changes} |");
        }

        sb.AppendLine();
        sb.AppendLine($"**Total changes across all features**: {totalChanges}");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
    }

    private static void WriteFeatureSection(StringBuilder sb, FeatureDiff diff, string? fileName, LineIndex? lineIndex, SpecDocument? featureSpec, LineIndex? prevLineIndex)
    {
        sb.AppendLine($"## {diff.FeatureName}");
        sb.AppendLine();

        if (diff.Changes.Count == 0 && (diff.EquivalentSchemas == null || diff.EquivalentSchemas.Count == 0))
        {
            sb.AppendLine("_No changes detected._");
            sb.AppendLine();
            return;
        }

        // Open the feature-level collapsible section.

        var fileLabel = fileName != null ? $" · `{fileName}`" : "";
        var changeWord = diff.Changes.Count == 1 ? "change" : "changes";
        sb.AppendLine($"<details><summary>{diff.Changes.Count} {changeWord}{fileLabel}</summary>");
        sb.AppendLine();

        // Collect duplicate schema names to exclude from main change sections.

        var duplicateSchemaNames = new HashSet<string>(StringComparer.Ordinal);

        if (diff.Duplicates != null)
        {
            foreach (var dup in diff.Duplicates)
            {
                duplicateSchemaNames.Add(dup.SchemaA);
                duplicateSchemaNames.Add(dup.SchemaB);
            }
        }

        // Filter changes, excluding schemas that will appear in the duplicates section.

        var mainChanges = diff.Changes
            .Where(c => !(c.Category == "schema" && c.Type == ChangeType.Added && duplicateSchemaNames.Contains(c.Path)))
            .ToList();

        var added = mainChanges.Where(c => c.Type == ChangeType.Added).ToList();
        var removed = mainChanges.Where(c => c.Type == ChangeType.Removed).ToList();
        var changed = mainChanges.Where(c => c.Type == ChangeType.Changed && c.Category != "schema-rename").ToList();
        var renames = mainChanges.Where(c => c.Category == "schema-rename").ToList();

        if (renames.Count > 0)
        {
            sb.AppendLine($"<details><summary>Renames Detected ({renames.Count})</summary>");
            sb.AppendLine();

            sb.AppendLine("```diff");

            foreach (var rename in renames)
            {
                // rename.Path is the new name; look up its line number.

                var line = lineIndex?.GetLine($"schema:{rename.Path}");
                var lineRef = line.HasValue ? $"  (line {line.Value})" : "";
                sb.AppendLine($"~ {rename.Detail}{lineRef}");
            }

            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        if (added.Count > 0)
        {
            sb.AppendLine($"<details><summary>Added ({added.Count})</summary>");
            sb.AppendLine();
            WriteAddedChanges(sb, added, lineIndex, featureSpec);
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        if (removed.Count > 0)
        {
            sb.AppendLine($"<details><summary>Removed ({removed.Count})</summary>");
            sb.AppendLine();
            WriteRemovedChanges(sb, removed, prevLineIndex, fileName);
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        if (changed.Count > 0)
        {
            sb.AppendLine($"<details><summary>Changed ({changed.Count})</summary>");
            sb.AppendLine();
            WriteStructuralChanges(sb, changed, lineIndex, featureSpec);
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        // Write equivalent schemas section (same-file structural duplicates).

        if (diff.EquivalentSchemas != null && diff.EquivalentSchemas.Count > 0)
        {
            sb.AppendLine($"<details><summary>Structurally Equivalent Schemas ({diff.EquivalentSchemas.Count})</summary>");
            sb.AppendLine();
            WriteEquivalentSchemas(sb, diff.EquivalentSchemas);
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        // Write possible duplicates section (among changes only).

        if (diff.Duplicates != null && diff.Duplicates.Count > 0)
        {
            sb.AppendLine($"<details><summary>Possible Duplicates ({diff.Duplicates.Count})</summary>");
            sb.AppendLine();
            WritePossibleDuplicates(sb, diff.Duplicates);
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        // Write duplicate operations section.

        if (diff.DuplicateOperations != null && diff.DuplicateOperations.Count > 0)
        {
            sb.AppendLine($"<details><summary>Duplicate Operations ({diff.DuplicateOperations.Count})</summary>");
            sb.AppendLine();
            WriteDuplicateOperations(sb, diff.DuplicateOperations, lineIndex, featureSpec);
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        // Write anomalies section.

        if (diff.Anomalies != null && diff.Anomalies.Count > 0)
        {
            sb.AppendLine($"<details><summary>⚠ Spec Anomalies ({diff.Anomalies.Count})</summary>");
            sb.AppendLine();
            WriteAnomalies(sb, diff.Anomalies);
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        // Close the feature-level collapsible section.

        sb.AppendLine("</details>");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
    }

    private static void WriteAddedChanges(StringBuilder sb, List<SpecChange> changes, LineIndex? lineIndex, SpecDocument? featureSpec)
    {
        // Group by category.

        var paths = changes.Where(c => c.Category == "path").ToList();
        var operations = changes.Where(c => c.Category == "operation").ToList();
        var schemas = changes.Where(c => c.Category == "schema").ToList();

        if (paths.Count > 0)
        {
            sb.AppendLine("#### Paths");
            sb.AppendLine();
            sb.AppendLine("```diff");

            foreach (var change in paths)
            {
                var line = lineIndex?.GetLine($"path:{change.Path}");
                var lineRef = line.HasValue ? $"  (line {line.Value})" : "";
                sb.AppendLine($"+ {change.Path}{lineRef}");
            }

            sb.AppendLine("```");
            sb.AppendLine();
        }

        if (operations.Count > 0)
        {
            sb.AppendLine("#### Operations");
            sb.AppendLine();
            sb.AppendLine("```diff");

            foreach (var change in operations)
            {
                var line = lineIndex?.GetLine($"op:{change.Path}");
                var lineRef = line.HasValue ? $"  (line {line.Value})" : "";
                sb.AppendLine($"+ {change.Path}: {change.NewValue}{lineRef}");
            }

            sb.AppendLine("```");
            sb.AppendLine();

            // Write collapsible payload details for each added operation.

            if (featureSpec != null)
            {
                foreach (var change in operations)
                {
                    WriteOperationDetails(sb, change, featureSpec);
                }
            }
        }

        if (schemas.Count > 0)
        {
            sb.AppendLine("#### New Schemas");
            sb.AppendLine();

            foreach (var change in schemas)
            {
                var line = lineIndex?.GetLine($"schema:{change.Path}");
                var lineRef = line.HasValue ? $" (line {line.Value})" : "";

                sb.AppendLine("```diff");
                sb.AppendLine($"+ {change.Path}  ({change.Detail ?? "unknown"}){lineRef}");
                sb.AppendLine("```");
                sb.AppendLine();

                if (change.Schema != null)
                {
                    WriteSchemaSkeletonBlock(sb, change.Schema);
                }
            }
        }
    }

    private static void WriteRemovedChanges(StringBuilder sb, List<SpecChange> changes, LineIndex? prevLineIndex, string? fileName)
    {
        var paths = changes.Where(c => c.Category == "path").ToList();
        var operations = changes.Where(c => c.Category == "operation").ToList();
        var schemas = changes.Where(c => c.Category == "schema").ToList();

        if (prevLineIndex != null && fileName != null)
        {
            sb.AppendLine($"> Previously in `{fileName}`");
            sb.AppendLine();
        }

        if (paths.Count > 0)
        {
            sb.AppendLine("#### Paths");
            sb.AppendLine();
            sb.AppendLine("```diff");

            foreach (var change in paths)
            {
                var line = prevLineIndex?.GetLine($"path:{change.Path}");
                var lineRef = line.HasValue ? $"  (was line {line.Value})" : "";
                sb.AppendLine($"- {change.Path}{lineRef}");
            }

            sb.AppendLine("```");
            sb.AppendLine();
        }

        if (operations.Count > 0)
        {
            sb.AppendLine("#### Operations");
            sb.AppendLine();
            sb.AppendLine("```diff");

            foreach (var change in operations)
            {
                var line = prevLineIndex?.GetLine($"op:{change.Path}");
                var lineRef = line.HasValue ? $"  (was line {line.Value})" : "";
                sb.AppendLine($"- {change.Path}: {change.OldValue}{lineRef}");
            }

            sb.AppendLine("```");
            sb.AppendLine();
        }

        if (schemas.Count > 0)
        {
            sb.AppendLine("#### Removed Schemas");
            sb.AppendLine();

            foreach (var change in schemas)
            {
                var line = prevLineIndex?.GetLine($"schema:{change.Path}");
                var lineRef = line.HasValue ? $" (was line {line.Value})" : "";

                sb.AppendLine("```diff");
                sb.AppendLine($"- {change.Path}  ({change.Detail ?? "unknown"}){lineRef}");
                sb.AppendLine("```");
                sb.AppendLine();

                if (change.Schema != null)
                {
                    WriteSchemaSkeletonBlock(sb, change.Schema);
                }
            }
        }
    }

    private static void WriteStructuralChanges(StringBuilder sb, List<SpecChange> changes, LineIndex? lineIndex, SpecDocument? featureSpec)
    {
        var operations = changes.Where(c => c.Category == "operation").ToList();
        var schemas = changes.Where(c => c.Category == "schema").ToList();

        if (operations.Count > 0)
        {
            sb.AppendLine("#### Operations");
            sb.AppendLine();

            foreach (var change in operations)
            {
                var opLine = lineIndex?.GetLine($"op:{change.Path}");
                var opLineRef = opLine.HasValue ? $" (line {opLine.Value})" : "";
                sb.AppendLine($"##### `{change.Path}` ({change.NewValue}){opLineRef}");
                sb.AppendLine();

                if (change.PropertyChanges != null && change.PropertyChanges.Count > 0)
                {
                    WritePropertyChanges(sb, change.PropertyChanges);
                }

                // Write collapsible payload details for changed operations.

                if (featureSpec != null)
                {
                    WriteOperationDetails(sb, change, featureSpec);
                }
            }
        }

        if (schemas.Count > 0)
        {
            sb.AppendLine("#### Schemas");
            sb.AppendLine();

            foreach (var change in schemas)
            {
                var line = lineIndex?.GetLine($"schema:{change.Path}");
                var lineRef = line.HasValue ? $" (line {line.Value})" : "";
                var typeTag = change.Detail != null ? $" `{change.Detail}`" : "";
                sb.AppendLine($"##### `{change.Path}`{typeTag}{lineRef}");
                sb.AppendLine();

                if (change.PropertyChanges != null && change.PropertyChanges.Count > 0)
                {
                    // Filter out composition variant add/remove noise.

                    var meaningful = change.PropertyChanges
                        .Where(p => !IsCompositionVariantNoise(p))
                        .ToList();

                    if (meaningful.Count > 0)
                    {
                        WritePropertyChanges(sb, meaningful, indent: true);
                    }
                }

                // Show the new-state schema skeleton for visual context.

                if (change.Schema != null)
                {
                    WriteSchemaSkeletonBlock(sb, change.Schema, indent: true);
                }
            }
        }
    }

    private static void WritePropertyChanges(StringBuilder sb, IReadOnlyList<PropertyChange> changes, bool indent = false)
    {
        var pre = indent ? "> " : "";
        sb.AppendLine($"{pre}```diff");

        foreach (var change in changes)
        {
            switch (change.Type)
            {
                case ChangeType.Added:
                    sb.Append($"{pre}+ {change.Name}: {change.NewType}");

                    if (change.Detail != null)
                    {
                        sb.Append($"  ({change.Detail})");
                    }

                    sb.AppendLine();
                    break;

                case ChangeType.Removed:
                    sb.Append($"{pre}- {change.Name}: {change.OldType}");

                    if (change.Detail != null)
                    {
                        sb.Append($"  ({change.Detail})");
                    }

                    sb.AppendLine();
                    break;

                case ChangeType.Changed:
                    sb.Append($"{pre}! {change.Name}");

                    if (change.Detail != null)
                    {
                        sb.Append($"  [{change.Detail}]");
                    }

                    sb.AppendLine();
                    break;
            }
        }

        sb.AppendLine($"{pre}```");
        sb.AppendLine();
    }

    /// <summary>
    /// Returns true if a property change is composition variant add/remove noise.
    /// These are individual oneOf/anyOf variant refs that were renamed; the structural
    /// skeleton already shows the new state clearly.
    /// </summary>
    private static bool IsCompositionVariantNoise(PropertyChange change)
    {
        if (change.Type != ChangeType.Added && change.Type != ChangeType.Removed)
        {
            return false;
        }

        // Composition variant entries have names like "oneOf:DragParam" or "anyOf:SomeRef".

        return change.Name.StartsWith("oneOf:", StringComparison.Ordinal)
            || change.Name.StartsWith("anyOf:", StringComparison.Ordinal)
            || change.Name.StartsWith("allOf:", StringComparison.Ordinal);
    }

    private static SchemaInfo? ExtractSchemaInfoByName(SpecDocument spec, string schemaName)
    {
        return SpecDiffer.GetSchemaInfo(spec, schemaName);
    }

    private static void WriteSchemaSkeletonBlock(StringBuilder sb, SchemaInfo schema, bool indent = false)
    {
        var pre = indent ? "> " : "";

        // Enum schema.

        if (schema.EnumValues != null && schema.EnumValues.Count > 0)
        {
            sb.AppendLine($"{pre}```yaml");
            sb.AppendLine($"{pre}# schema: {schema.Type ?? "string"} enum");

            foreach (var val in schema.EnumValues)
            {
                sb.AppendLine($"{pre}  | {val}");
            }

            sb.AppendLine($"{pre}```");
            sb.AppendLine();
            return;
        }

        // Composition schema (anyOf/oneOf/allOf).

        if (schema.CompositionType != null && schema.CompositionRefs != null)
        {
            sb.AppendLine($"{pre}```yaml");
            sb.AppendLine($"{pre}# schema: {schema.CompositionType}");

            foreach (var r in schema.CompositionRefs)
            {
                sb.AppendLine($"{pre}  | {r}");
            }

            sb.AppendLine($"{pre}```");
            sb.AppendLine();
            return;
        }

        // Object schema with properties.

        if (schema.Properties != null && schema.Properties.Count > 0)
        {
            sb.AppendLine($"{pre}```yaml");
            sb.AppendLine($"{pre}# schema: {schema.Type ?? "object"}");

            foreach (var prop in schema.Properties)
            {
                var req = prop.IsRequired ? " (required)" : "";
                sb.AppendLine($"{pre}  {prop.Name}: {prop.Type}{req}");
            }

            sb.AppendLine($"{pre}```");
            sb.AppendLine();
            return;
        }

        // Fallback — no skeleton, just a note in the diff block.
    }

    private static void WriteOperationDetails(StringBuilder sb, SpecChange change, SpecDocument featureSpec)
    {
        var hasRequest = change.RequestSchema != null;
        var hasResponse = change.ResponseSchema != null;

        if (!hasRequest && !hasResponse)
        {
            return;
        }

        var opLabel = change.NewValue ?? change.Path;

        // Build schema ref summary for the details header.

        var schemaRefs = new List<string>();

        if (change.RequestSchemaRef != null)
        {
            schemaRefs.Add($"req: {change.RequestSchemaRef}");
        }

        if (change.ResponseSchemaRef != null)
        {
            schemaRefs.Add($"resp: {change.ResponseSchemaRef}");
        }

        var schemaRefLabel = schemaRefs.Count > 0 ? $" — {string.Join(", ", schemaRefs)}" : "";
        sb.AppendLine($"<details><summary>📋 Payload: <code>{opLabel}</code>{schemaRefLabel}</summary>");
        sb.AppendLine();

        if (hasRequest)
        {
            var reqLabel = change.RequestSchemaRef != null ? $"**Request Body** (`{change.RequestSchemaRef}`)" : "**Request Body**";
            sb.AppendLine($"> {reqLabel}");
            sb.AppendLine(">");

            // Indent JSON content inside blockquote for visual nesting.

            var requestJson = JsonExampleBuilder.Build(change.RequestSchema!, featureSpec);

            sb.AppendLine("> ```json");

            foreach (var jsonLine in requestJson.Split('\n'))
            {
                sb.AppendLine($"> {jsonLine.TrimEnd('\r')}");
            }

            sb.AppendLine("> ```");
            sb.AppendLine();
        }

        if (hasResponse)
        {
            var respLabel = change.ResponseSchemaRef != null ? $"**Response** (`{change.ResponseSchemaRef}`)" : "**Response**";
            sb.AppendLine($"> {respLabel}");
            sb.AppendLine(">");

            var responseJson = JsonExampleBuilder.Build(change.ResponseSchema!, featureSpec);

            sb.AppendLine("> ```json");

            foreach (var jsonLine in responseJson.Split('\n'))
            {
                sb.AppendLine($"> {jsonLine.TrimEnd('\r')}");
            }

            sb.AppendLine("> ```");
            sb.AppendLine();
        }

        sb.AppendLine("</details>");
        sb.AppendLine();
    }

    private static void WriteEquivalentSchemas(StringBuilder sb, IReadOnlyList<SchemaEquivalenceGroup> groups)
    {
        sb.AppendLine("> The following schemas in this spec file are structurally identical and may represent the same type duplicated locally.");
        sb.AppendLine();

        foreach (var group in groups)
        {
            sb.AppendLine($"- **{string.Join("**, **", group.SchemaNames)}**");
            sb.AppendLine($"  Signature: `{group.PropertySignature}`");
            sb.AppendLine();
        }
    }

    private static void WritePossibleDuplicates(StringBuilder sb, IReadOnlyList<SchemaDuplicate> duplicates)
    {
        sb.AppendLine("> The following added schemas have high structural similarity and may be duplicates. They are not reported in the change sections above.");
        sb.AppendLine();

        // Collapse pairwise duplicates into groups using union-find.

        var groups = CollapseDuplicatePairs(duplicates);

        foreach (var (schemas, sharedProps, similarity) in groups)
        {
            var pct = (similarity * 100).ToString("F0");
            sb.AppendLine($"- **{string.Join("**, **", schemas)}** ({pct}% similar)");
            sb.AppendLine($"  Shared: `{string.Join("`, `", sharedProps)}`");
            sb.AppendLine();
        }
    }

    private static List<(List<string> Schemas, IReadOnlyList<string> SharedProperties, double Similarity)> CollapseDuplicatePairs(
        IReadOnlyList<SchemaDuplicate> duplicates)
    {
        // Simple union-find to group connected schemas.

        var parent = new Dictionary<string, string>(StringComparer.Ordinal);

        string Find(string x)
        {
            if (!parent.ContainsKey(x))
            {
                parent[x] = x;
            }

            while (parent[x] != x)
            {
                parent[x] = parent[parent[x]];
                x = parent[x];
            }

            return x;
        }

        void Union(string a, string b)
        {
            var ra = Find(a);
            var rb = Find(b);

            if (ra != rb)
            {
                parent[ra] = rb;
            }
        }

        foreach (var dup in duplicates)
        {
            Union(dup.SchemaA, dup.SchemaB);
        }

        // Group schemas by root.

        var groups = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var groupShared = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        var groupMinSimilarity = new Dictionary<string, double>(StringComparer.Ordinal);

        foreach (var dup in duplicates)
        {
            var root = Find(dup.SchemaA);

            if (!groups.ContainsKey(root))
            {
                groups[root] = [];
                groupShared[root] = new HashSet<string>(dup.SharedProperties, StringComparer.Ordinal);
                groupMinSimilarity[root] = dup.Similarity;
            }
            else
            {
                groupShared[root].IntersectWith(dup.SharedProperties);

                if (dup.Similarity < groupMinSimilarity[root])
                {
                    groupMinSimilarity[root] = dup.Similarity;
                }
            }

            if (!groups[root].Contains(dup.SchemaA, StringComparer.Ordinal))
            {
                groups[root].Add(dup.SchemaA);
            }

            if (!groups[root].Contains(dup.SchemaB, StringComparer.Ordinal))
            {
                groups[root].Add(dup.SchemaB);
            }
        }

        return groups.Select(kv => (
            kv.Value.OrderBy(n => n, StringComparer.Ordinal).ToList(),
            (IReadOnlyList<string>)groupShared[kv.Key].OrderBy(p => p, StringComparer.Ordinal).ToList(),
            groupMinSimilarity[kv.Key]
        )).OrderBy(g => g.Item1[0], StringComparer.Ordinal).ToList();
    }

    private static void WriteDuplicateOperations(StringBuilder sb, IReadOnlyList<OperationDuplicate> duplicates, LineIndex? lineIndex, SpecDocument? featureSpec)
    {
        sb.AppendLine("> The following operations share identical request and/or response schema definitions.");
        sb.AppendLine();

        // Track schemas we've already shown to avoid repeating skeletons.

        var shownSchemas = new HashSet<string>(StringComparer.Ordinal);

        foreach (var dup in duplicates)
        {
            var opA = dup.OperationIdA ?? dup.OperationA;
            var opB = dup.OperationIdB ?? dup.OperationB;

            var lineA = lineIndex?.GetLine($"opid:{opA}") ?? lineIndex?.GetLine($"op:{dup.OperationA}");
            var lineB = lineIndex?.GetLine($"opid:{opB}") ?? lineIndex?.GetLine($"op:{dup.OperationB}");

            var lineRefA = lineA.HasValue ? $" (line {lineA.Value})" : "";
            var lineRefB = lineB.HasValue ? $" (line {lineB.Value})" : "";

            var shared = new List<string>();

            if (dup.SharedRequestSchema != null)
            {
                shared.Add($"request: `{dup.SharedRequestSchema}`");
            }

            if (dup.SharedResponseSchema != null)
            {
                shared.Add($"response: `{dup.SharedResponseSchema}`");
            }

            sb.AppendLine($"- **{opA}**{lineRefA} ↔ **{opB}**{lineRefB}");
            sb.AppendLine($"  Shared: {string.Join(", ", shared)}");
            sb.AppendLine();

            // Show collapsed schema skeleton for shared schemas not yet displayed.

            if (featureSpec != null)
            {
                foreach (var schemaRef in new[] { dup.SharedRequestSchema, dup.SharedResponseSchema })
                {
                    if (schemaRef != null && shownSchemas.Add(schemaRef))
                    {
                        var schemaInfo = ExtractSchemaInfoByName(featureSpec, schemaRef);

                        if (schemaInfo != null)
                        {
                            sb.AppendLine();
                            sb.AppendLine($"<details><summary>Schema: <code>{schemaRef}</code></summary>");
                            sb.AppendLine();
                            WriteSchemaSkeletonBlock(sb, schemaInfo);
                            sb.AppendLine("</details>");
                            sb.AppendLine();
                        }
                    }
                }
            }
        }
    }

    private static void WriteAnomalies(StringBuilder sb, IReadOnlyList<SpecAnomaly> anomalies)
    {
        sb.AppendLine("> The following inconsistencies were detected in the spec and may indicate errors or intentional divergence.");
        sb.AppendLine();

        foreach (var anomaly in anomalies)
        {
            var icon = anomaly.Severity == "error" ? "🔴" : "🟡";
            sb.AppendLine($"- {icon} **{anomaly.Category}** at `{anomaly.Path}`");
            sb.AppendLine($"  {anomaly.Detail}");
            sb.AppendLine();
        }
    }
}
