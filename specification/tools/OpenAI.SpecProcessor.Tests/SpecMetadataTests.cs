using System.Text.Json;
using NUnit.Framework;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Covers how a snapshot's metadata file is interpreted, since the orchestrator and the processor
/// both read it and a disagreement between them is what lets a corrupt file rotate a good baseline
/// out of the way.
/// </summary>
[TestFixture]
public class SpecMetadataTests
{
    private string _directory = string.Empty;

    [SetUp]
    public void CreateDirectory()
    {
        _directory = Path.Combine(Path.GetTempPath(), $"spec-metadata-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_directory);
    }

    [TearDown]
    public void RemoveDirectory()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    [Test]
    public void AnAbsentFileReadsAsMissing()
    {
        var result = SpecMetadata.Read(_directory);

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(MetadataState.Missing));
            Assert.That(result.IsUsable, Is.False);
            Assert.That(result.IsFatalForComparison, Is.False, "A first run has no metadata, so missing cannot be fatal.");
        });
    }

    [Test]
    public void AFileWrittenByThisToolReadsAsValid()
    {
        new SpecMetadata { Version = "2.3.0", SourceContentHash = "abc" }.Save(_directory);

        var result = SpecMetadata.Read(_directory);

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(MetadataState.Valid));
            Assert.That(result.IsUsable, Is.True);
            Assert.That(result.Metadata!.SourceContentHash, Is.EqualTo("abc"));
        });
    }

    [Test]
    public void AFileWithoutASchemaVersionReadsAsLegacy()
    {
        Write("""{ "version": "2.3.0", "sourceContentHash": "abc" }""");

        var result = SpecMetadata.Read(_directory);

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(MetadataState.Legacy));
            Assert.That(result.IsUsable, Is.True, "The fields a legacy file does carry are still true.");
            Assert.That(result.IsFatalForComparison, Is.False);
            Assert.That(result.Metadata!.SourceContentHash, Is.EqualTo("abc"));
        });
    }

    [Test]
    public void UnparseableContentReadsAsMalformed()
    {
        Write("{ this is not json");

        var result = SpecMetadata.Read(_directory);

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(MetadataState.Malformed));
            Assert.That(result.IsUsable, Is.False);
            Assert.That(result.IsFatalForComparison, Is.True, "Corruption must not be read as a change.");
        });
    }

    [Test]
    public void ANewerSchemaVersionReadsAsUnsupported()
    {
        Write($$"""{ "schemaVersion": {{SpecMetadata.CurrentSchemaVersion + 1}}, "sourceContentHash": "abc" }""");

        var result = SpecMetadata.Read(_directory);

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(MetadataState.UnsupportedVersion));
            Assert.That(result.IsUsable, Is.False);
            Assert.That(result.IsFatalForComparison, Is.True, "Guessing at a shape written by a newer tool is worse than stopping.");
        });
    }

    /// <remarks>
    /// The processor uses this file only to describe the baseline, so a file it cannot read must
    /// warn rather than stop the run. That is the opposite of what the orchestrator does with it,
    /// and the difference is deliberate.
    /// </remarks>
    [Test]
    public void LoadDegradesWithAWarningRatherThanThrowing()
    {
        Write("{ this is not json");

        string? warning = null;
        var metadata = SpecMetadata.Load(_directory, reason => warning = reason);

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Is.Null);
            Assert.That(warning, Is.Not.Null.And.Contains("could not be parsed"));
        });
    }

    [Test]
    public void LoadIsQuietWhenThereIsSimplyNoFile()
    {
        string? warning = null;
        var metadata = SpecMetadata.Load(_directory, reason => warning = reason);

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Is.Null);
            Assert.That(warning, Is.Null, "A first run should not warn about the absence it is defined by.");
        });
    }

    [Test]
    public void TheRecordedSchemaVersionRoundTrips()
    {
        new SpecMetadata { Version = "2.3.0" }.Save(_directory);

        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(_directory, SpecMetadata.FileName)));

        Assert.That(document.RootElement.GetProperty("schemaVersion").GetInt32(), Is.EqualTo(SpecMetadata.CurrentSchemaVersion));
    }

    /// <remarks>
    /// The same options have to write and read this file. A status written as a name and read back
    /// as a number, or the reverse, would break the very comparison the status exists to support.
    /// </remarks>
    [Test]
    public void AnUnassignedStatusRoundTripsThroughTheFileAsAName()
    {
        new SpecMetadata
        {
            UnassignedPaths = [new() { Path = "/content_provenance_checks", Status = UnassignedStatus.Unchanged }]
        }.Save(_directory);

        var raw = File.ReadAllText(Path.Combine(_directory, SpecMetadata.FileName));
        var read = SpecMetadata.Read(_directory);

        Assert.Multiple(() =>
        {
            Assert.That(raw, Does.Contain("\"Unchanged\""), "The status should be written by name, not by ordinal.");
            Assert.That(read.State, Is.EqualTo(MetadataState.Valid));
            Assert.That(read.Metadata!.UnassignedPaths[0].Status, Is.EqualTo(UnassignedStatus.Unchanged));
        });
    }

    [TestCase(UnassignedStatus.New)]
    [TestCase(UnassignedStatus.Unchanged)]
    [TestCase(UnassignedStatus.Resolved)]
    public void EveryUnassignedStatusRoundTrips(UnassignedStatus status)
    {
        new SpecMetadata { UnassignedPaths = [new() { Path = "/widgets", Status = status }] }.Save(_directory);

        Assert.That(SpecMetadata.Read(_directory).Metadata!.UnassignedPaths[0].Status, Is.EqualTo(status));
    }

    /// <remarks>
    /// The very first snapshot written after the status field was added recorded it as a number,
    /// before the representation was corrected. A baseline that only the writer that produced it can
    /// read is not a baseline, so the numeric form stays readable.
    /// </remarks>
    [Test]
    public void AStatusRecordedAsANumberIsStillReadable()
    {
        Write($$"""
            {
              "schemaVersion": {{SpecMetadata.CurrentSchemaVersion}},
              "sourceContentHash": "abc",
              "unassignedPaths": [ { "path": "/content_provenance_checks", "status": 1 } ]
            }
            """);

        var result = SpecMetadata.Read(_directory);

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(MetadataState.Valid));
            Assert.That(result.Metadata!.UnassignedPaths[0].Status, Is.EqualTo(UnassignedStatus.Unchanged));
        });
    }

    /// <remarks>
    /// This pins the persisted numeric mapping itself rather than the declaration order that happens
    /// to produce it. Those are the same today and would silently stop being the same if a member
    /// were ever inserted or reordered, at which point every old snapshot would keep deserializing
    /// successfully and mean something different. A `Resolved` reading back as `Unchanged` is a gap
    /// that reopens itself in the report with no defect anyone can point at.
    ///
    /// If this test fails, the fix is to restore the numeric values, not to update the expectations.
    /// </remarks>
    [TestCase(0, UnassignedStatus.New)]
    [TestCase(1, UnassignedStatus.Unchanged)]
    [TestCase(2, UnassignedStatus.Resolved)]
    public void ThePersistedNumericMappingIsFixed(int persisted, UnassignedStatus expected)
    {
        Write($$"""
            {
              "schemaVersion": {{SpecMetadata.CurrentSchemaVersion}},
              "sourceContentHash": "abc",
              "unassignedPaths": [ { "path": "/widgets", "status": {{persisted}} } ]
            }
            """);

        Assert.Multiple(() =>
        {
            Assert.That(SpecMetadata.Read(_directory).Metadata!.UnassignedPaths[0].Status, Is.EqualTo(expected));
            Assert.That((int)expected, Is.EqualTo(persisted),
                "The enum's own numeric value has to match what was persisted, or old files change meaning.");
        });
    }

    /// <remarks>
    /// A numeric value that no member claims must not be quietly accepted as something. This is the
    /// case where a newer build wrote a status this one does not know about.
    /// </remarks>
    [Test]
    public void AnUnrecognizedNumericStatusIsRejected()
    {
        Write($$"""
            {
              "schemaVersion": {{SpecMetadata.CurrentSchemaVersion}},
              "sourceContentHash": "abc",
              "unassignedPaths": [ { "path": "/widgets", "status": 99 } ]
            }
            """);

        Assert.That(SpecMetadata.Read(_directory).State, Is.EqualTo(MetadataState.Malformed),
            "An unknown status is a file this build cannot interpret, which is not the same as a file it can.");
    }

    /// <remarks>
    /// Legacy numeric values are read once and then rewritten by name, so the numeric form drains out
    /// of the snapshots rather than living on indefinitely as a second representation to support.
    /// </remarks>
    [Test]
    public void ALegacyNumericStatusIsRewrittenByName()
    {
        Write($$"""
            {
              "schemaVersion": {{SpecMetadata.CurrentSchemaVersion}},
              "sourceContentHash": "abc",
              "unassignedPaths": [ { "path": "/widgets", "status": 2 } ]
            }
            """);

        SpecMetadata.Read(_directory).Metadata!.Save(_directory);

        var raw = File.ReadAllText(Path.Combine(_directory, SpecMetadata.FileName));

        Assert.Multiple(() =>
        {
            Assert.That(raw, Does.Contain("\"Resolved\""));
            Assert.That(raw, Does.Not.Contain("\"status\": 2"));
        });
    }

    /// <remarks>
    /// Metadata written before the status field existed carries no status at all. Those entries were
    /// recorded when every gap was, by definition, the first one seen, so the default is the honest
    /// reading rather than a failure.
    /// </remarks>
    [Test]
    public void AnUnassignedPathWithNoRecordedStatusReadsAsNew()
    {
        Write($$"""
            {
              "schemaVersion": {{SpecMetadata.CurrentSchemaVersion}},
              "sourceContentHash": "abc",
              "unassignedPaths": [ { "path": "/content_provenance_checks" } ]
            }
            """);

        Assert.That(SpecMetadata.Read(_directory).Metadata!.UnassignedPaths[0].Status, Is.EqualTo(UnassignedStatus.New));
    }

    /// <remarks>
    /// A legacy document predates the status field entirely, and the reconciliation has to treat the
    /// paths it lists as known gaps rather than discarding them. Losing them would report every
    /// standing gap as new the month after a migration.
    /// </remarks>
    [Test]
    public void ALegacyDocumentStillYieldsItsUnassignedPaths()
    {
        Write("""
            {
              "sourceContentHash": "abc",
              "unassignedPaths": [ { "path": "/content_provenance_checks" } ]
            }
            """);

        var result = SpecMetadata.Read(_directory);

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(MetadataState.Legacy));
            Assert.That(result.IsUsable, Is.True);
            Assert.That(result.Metadata!.UnassignedPaths.Select(entry => entry.Path),
                Is.EquivalentTo(new[] { "/content_provenance_checks" }));
        });
    }

    private void Write(string json) =>
        File.WriteAllText(Path.Combine(_directory, SpecMetadata.FileName), json);
}
