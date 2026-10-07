namespace OpenAI.SpecProcessor.Spec;

/// <summary> How a snapshot's metadata file was interpreted. </summary>
/// <remarks>
/// Both the orchestrator and the processor read this file, and they need to agree on what each
/// outcome means. Collapsing these into "present or not" is what lets a corrupt file be treated as a
/// change, which silently rotates a good baseline out of the way.
/// </remarks>
public enum MetadataState
{
    /// <summary> Read cleanly, and written by a schema version this tool understands. </summary>
    Valid,

    /// <summary> No metadata file exists. This is the legitimate bootstrap case. </summary>
    Missing,

    /// <summary> Read cleanly, but written before the schema carried a version. </summary>
    Legacy,

    /// <summary> Present but not parseable as JSON, or missing fields that must be there. </summary>
    Malformed,

    /// <summary> Written by a newer schema version than this tool knows how to interpret. </summary>
    UnsupportedVersion
}

/// <summary> The outcome of reading a snapshot's metadata, carrying both the state and the reason. </summary>
/// <param name="State"> How the file was interpreted. </param>
/// <param name="Metadata"> The parsed metadata, present only for <see cref="MetadataState.Valid"/> and <see cref="MetadataState.Legacy"/>. </param>
/// <param name="Reason"> A human-readable explanation, present for every state except <see cref="MetadataState.Valid"/>. </param>
public record MetadataReadResult(MetadataState State, SpecMetadata? Metadata, string? Reason)
{
    /// <summary> Whether the metadata can be relied on to describe the baseline. </summary>
    /// <remarks>
    /// Legacy counts as usable. It was written by an older shape of this record, so the fields it
    /// does carry are still true; it just may not carry all of them.
    /// </remarks>
    public bool IsUsable => State is MetadataState.Valid or MetadataState.Legacy;

    /// <summary>
    /// Whether this outcome should stop a run that depends on the metadata being correct, as
    /// opposed to one that only uses it for description.
    /// </summary>
    /// <remarks>
    /// Missing is deliberately not a failure. A snapshot that has never been taken has no metadata,
    /// and that is the state the very first run starts from.
    /// </remarks>
    public bool IsFatalForComparison => State is MetadataState.Malformed or MetadataState.UnsupportedVersion;
}
