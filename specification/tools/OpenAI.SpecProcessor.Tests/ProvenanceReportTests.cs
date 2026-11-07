using NUnit.Framework;
using OpenAI.SpecProcessor.Report;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Covers the promise that the report and the metadata describe the same snapshot.
/// </summary>
/// <remarks>
/// The two are read by different audiences. A reviewer reads the report; the next month's run reads
/// the metadata. If they can disagree about which upstream commit was processed, one of them is
/// lying and there is no way to tell which. They are written from a single record in a single run,
/// and these tests hold that arrangement in place.
/// </remarks>
[TestFixture]
public class ProvenanceReportTests
{
    [Test]
    public void TheReportStatesTheSameProvenanceAsTheMetadata()
    {
        var metadata = Metadata();
        var report = DiffReportWriter.Generate([], "2.3.0", "2.2.0", metadata: metadata);

        Assert.Multiple(() =>
        {
            Assert.That(report, Does.Contain(metadata.Source!));
            Assert.That(report, Does.Contain(metadata.DownloadedCommitSha!));
            Assert.That(report, Does.Contain(metadata.SourceContentHash!));
            Assert.That(report, Does.Contain($"v{metadata.SchemaVersion}"));
            Assert.That(report, Does.Contain($"v{DiffScope.Current.Version}"));
            Assert.That(report, Does.Contain(metadata.FeatureCount.ToString()));
        });
    }

    /// <remarks>
    /// The hash of the source that produced the previous snapshot is what the no-op decision is
    /// made against, so a reviewer looking at a report that exists at all should be able to see both
    /// sides of that comparison without opening two files.
    /// </remarks>
    [Test]
    public void TheReportShowsBothSidesOfTheContentComparison()
    {
        var previous = Metadata() with { SourceContentHash = new string('b', 64) };
        var report = DiffReportWriter.Generate([], "2.3.0", "2.2.0", previous, metadata: Metadata());

        Assert.Multiple(() =>
        {
            Assert.That(report, Does.Contain(new string('a', 64)));
            Assert.That(report, Does.Contain(new string('b', 64)));
        });
    }

    /// <remarks>
    /// The sanitizer edits the source before it is parsed. A reader comparing the report against the
    /// raw upstream file needs to know that happened, or a discrepancy looks like a processing bug.
    /// </remarks>
    [TestCase(0, "none")]
    [TestCase(3, "3 line(s) repaired")]
    public void TheReportSaysWhetherTheSourceWasRepaired(int repaired, string expected)
    {
        var report = DiffReportWriter.Generate([], "2.3.0", null, metadata: Metadata(), repairedLineCount: repaired);

        Assert.That(report, Does.Contain(expected));
    }

    /// <remarks>
    /// Provenance a run does not have must read as missing rather than as blank. A source with no
    /// resolvable commit is a legitimate case, a manual local file for instance, and it should be
    /// obvious in the report that this is what happened.
    /// </remarks>
    [Test]
    public void ProvenanceThatWasNotRecordedSaysSo()
    {
        var metadata = Metadata() with { DownloadedCommitSha = null };
        var report = DiffReportWriter.Generate([], "2.3.0", null, metadata: metadata);

        Assert.That(report, Does.Contain("(not recorded)"));
    }

    [Test]
    public void WithoutMetadataTheReportMakesNoProvenanceClaim()
    {
        var report = DiffReportWriter.Generate([], "2.3.0", null);

        Assert.That(report, Does.Not.Contain("Where this came from"),
            "Silence is correct here. A provenance section with nothing in it would imply the run had no source.");
    }

    private static SpecMetadata Metadata() =>
        new()
        {
            Version = "2.3.0",
            Source = "https://raw.githubusercontent.com/openai/openai-openapi/abc123/openapi.yaml",
            DownloadedCommitSha = "abc123def456abc123def456abc123def456abcd",
            SourceContentHash = new string('a', 64),
            ProcessedAt = DateTimeOffset.UnixEpoch,
            FeatureCount = 24
        };
}
