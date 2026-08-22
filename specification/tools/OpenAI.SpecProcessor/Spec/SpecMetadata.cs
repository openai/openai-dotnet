using System.Text.Json;

namespace OpenAI.SpecProcessor.Spec;

/// <summary> Metadata written alongside processed spec files to track provenance. </summary>
public record SpecMetadata
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary> The spec version from the info section. </summary>
    public string? Version { get; init; }

    /// <summary>
    /// The revision of this metadata document's own shape.
    /// </summary>
    /// <remarks>
    /// Increment this whenever a field changes meaning or stops being written, so that a reader can
    /// tell an older document from a corrupt one. A file written before this field existed reads
    /// back as zero, which is the legacy case.
    /// </remarks>
    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    /// <summary> The source path or URL the spec was acquired from. </summary>
    public string? Source { get; init; }

    /// <summary> The upstream commit SHA the spec was downloaded from, when known. </summary>
    public string? DownloadedCommitSha { get; init; }

    /// <summary>
    /// The SHA-256 hash of the raw specification that produced this snapshot.
    /// </summary>
    /// <remarks>
    /// Not every source exposes a commit, so the content hash is the provenance that always
    /// applies. It is also what makes the no-op check exact: identical content cannot produce a
    /// different snapshot, so there is nothing to report.
    /// </remarks>
    public string? SourceContentHash { get; init; }

    /// <summary> The UTC timestamp when processing was performed. </summary>
    public DateTimeOffset ProcessedAt { get; init; }

    /// <summary> The number of feature specs produced. </summary>
    public int FeatureCount { get; init; }

    /// <summary>
    /// Paths the split could not assign to any feature area.
    /// </summary>
    /// <remarks>
    /// An unassigned path means upstream published something the feature map does not know about,
    /// which is exactly the case a reviewer needs to see. Recording it here keeps it visible after
    /// the run that produced it has scrolled out of the job log.
    /// </remarks>
    public IReadOnlyList<UnassignedPath> UnassignedPaths { get; init; } = [];

    /// <summary>
    /// Paths deliberately removed before processing, recorded so that an absent path can be told
    /// apart from an unrecognized one.
    /// </summary>
    public IReadOnlyList<string> ExcludedPaths { get; init; } = [];

    /// <summary>
    /// What the structural comparison covers, so a downstream reader does not have to infer it.
    /// </summary>
    public DiffScope DiffScope { get; init; } = DiffScope.Current;

    /// <summary>
    /// The behavior that produced this snapshot, so a change in the tool is as visible as a change
    /// in the source.
    /// </summary>
    /// <remarks>
    /// Without this, the no-op check answers only "did upstream change" and silently assumes the
    /// answer to "would we produce the same thing anyway". A month where upstream is still but the
    /// feature map moved would skip, and the snapshot would go on describing itself with a taxonomy
    /// the tool no longer uses.
    /// </remarks>
    public ProcessingIdentity ProcessingIdentity { get; init; } = ProcessingIdentity.Current;

    /// <summary> The marker file name used when saving metadata. </summary>
    public const string FileName = ".spec-metadata.json";

    /// <summary> The metadata shape this version of the tool writes and understands. </summary>
    public const int CurrentSchemaVersion = 1;

    /// <summary> Saves the metadata to the specified directory. </summary>
    /// <param name="directory"> The directory to write the marker file into. </param>
    public void Save(string directory)
    {
        var path = Path.Combine(directory, FileName);
        var json = JsonSerializer.Serialize(this, _jsonOptions);
        LineEndings.WriteAllText(path, json);
    }

    /// <summary> Loads metadata from the specified directory, or returns null if not found. </summary>
    /// <param name="directory"> The directory to read the marker file from. </param>
    /// <param name="onUnreadable"> Invoked with the reason when a file exists but cannot be used. </param>
    /// <returns> The deserialized metadata, or null when it is absent or unusable. </returns>
    /// <remarks>
    /// The processor reads this only to describe the baseline in the report, so a file it cannot use
    /// must not stop a run. The content hash that actually gates whether a run happens is read by the
    /// orchestrator, which does fail loudly, because that one is a correctness input rather than a
    /// descriptive one. Use <see cref="Read"/> when the distinction between the failure modes
    /// matters.
    /// </remarks>
    public static SpecMetadata? Load(string directory, Action<string>? onUnreadable = null)
    {
        var result = Read(directory);

        if (!result.IsUsable)
        {
            if (result.State != MetadataState.Missing)
            {
                onUnreadable?.Invoke($"{result.Reason} The baseline will be described as unknown.");
            }

            return null;
        }

        return result.Metadata;
    }

    /// <summary> Reads metadata from the specified directory, classifying the outcome. </summary>
    /// <param name="directory"> The directory to read the marker file from. </param>
    /// <returns> The state, the parsed metadata when there is any, and the reason when there is not. </returns>
    public static MetadataReadResult Read(string directory)
    {
        var path = Path.Combine(directory, FileName);

        if (!File.Exists(path))
        {
            return new(MetadataState.Missing, null, $"No snapshot metadata exists at {path}.");
        }

        SpecMetadata? metadata;
        int schemaVersion;

        try
        {
            var json = File.ReadAllText(path);
            metadata = JsonSerializer.Deserialize<SpecMetadata>(json, _jsonOptions);

            // Read the version from the raw document rather than the deserialized record. An absent
            // property leaves the record's default in place, which would make a file written before
            // this field existed indistinguishable from a current one.

            using var document = JsonDocument.Parse(json);

            schemaVersion = document.RootElement.TryGetProperty("schemaVersion", out var declared) && declared.TryGetInt32(out var value)
                ? value
                : 0;
        }
        catch (JsonException ex)
        {
            return new(MetadataState.Malformed, null, $"The metadata at {path} could not be parsed ({ex.Message}).");
        }

        if (metadata == null)
        {
            return new(MetadataState.Malformed, null, $"The metadata at {path} deserialized to nothing.");
        }

        metadata = metadata with { SchemaVersion = schemaVersion };

        // A status the enum does not define deserializes without complaint, so it has to be checked
        // rather than trusted. This is the case where a newer build recorded a status this one has
        // never heard of, and quietly reading it as something is how a file starts meaning the wrong
        // thing.

        var undefined = metadata.UnassignedPaths
            .Where(entry => !Enum.IsDefined(entry.Status))
            .Select(entry => $"{entry.Path} ({(int)entry.Status})")
            .ToList();

        if (undefined.Count > 0)
        {
            return new(MetadataState.Malformed, null,
                $"The metadata at {path} records unassigned path status values this tool does not define: {string.Join(", ", undefined)}.");
        }

        if (schemaVersion > CurrentSchemaVersion)
        {
            return new(MetadataState.UnsupportedVersion, metadata,
                $"The metadata at {path} was written with schema version {schemaVersion}, and this tool understands {CurrentSchemaVersion}. Update the tool rather than reinterpreting it.");
        }

        if (schemaVersion < CurrentSchemaVersion)
        {
            return new(MetadataState.Legacy, metadata,
                $"The metadata at {path} was written with schema version {schemaVersion}, before the current shape. Fields added since then will read as empty.");
        }

        return new(MetadataState.Valid, metadata, null);
    }
}
