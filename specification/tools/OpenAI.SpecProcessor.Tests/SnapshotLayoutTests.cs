using NUnit.Framework;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Checks the committed snapshots against the feature map they are supposed to reflect.
/// </summary>
/// <remarks>
/// The expected file set is derived from <see cref="FeatureAreaConfig"/> rather than written down as
/// a count. A fixed number turns an intentional feature-area addition into a brittle failure whose
/// obvious fix is to edit the number, which is exactly the moment nobody looks at what actually
/// landed in the folder. Derived from the map, adding an area is a one-line change in one place and
/// the check stays strict about the thing that matters: that every area the tool knows about has a
/// file, and that no file is there which no area claims.
/// </remarks>
[TestFixture]
public class SnapshotLayoutTests
{
    private static readonly string _specRoot = LocateSpecRoot();

    private static IEnumerable<string> SnapshotDirectories()
    {
        yield return "current";
        yield return "previous";
    }

    [TestCaseSource(nameof(SnapshotDirectories))]
    public void EveryFeatureAreaHasAFile(string snapshot)
    {
        var directory = Path.Combine(_specRoot, snapshot);

        Assert.That(Directory.Exists(directory), Is.True, $"The {snapshot} snapshot is missing entirely.");

        var expected = FeatureAreaConfig.All.Select(area => area.OutputFile).ToList();
        var present = Directory.GetFiles(directory, "*.yml").Select(Path.GetFileName).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(present, Is.EquivalentTo(expected),
                "The snapshot's feature files and the feature map have to name the same set. A file with no "
                + "area is an orphan the tool will never rewrite; an area with no file is a gap in coverage.");

            Assert.That(expected, Is.Unique, "Two feature areas writing to one file would silently lose one of them.");
        });
    }

    /// <remarks>
    /// Named individually rather than counted, so that a missing report reads as "the report is
    /// missing" rather than as an off-by-one in a total.
    /// </remarks>
    [TestCaseSource(nameof(SnapshotDirectories))]
    public void TheSupportingFilesArePresent(string snapshot)
    {
        var directory = Path.Combine(_specRoot, snapshot);

        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(Path.Combine(directory, SpecMetadata.FileName)), Is.True,
                "Without metadata the snapshot cannot answer for its own provenance and the no-op check has nothing to read.");

            Assert.That(File.Exists(Path.Combine(directory, "diff-report.md")), Is.True,
                "The report is what a reviewer reads; a snapshot without one is only half a deliverable.");
        });
    }

    [TestCaseSource(nameof(SnapshotDirectories))]
    public void NothingUnexpectedIsInTheSnapshot(string snapshot)
    {
        var directory = Path.Combine(_specRoot, snapshot);

        var expected = FeatureAreaConfig.All
            .Select(area => area.OutputFile)
            .Concat([SpecMetadata.FileName, "diff-report.md"])
            .ToHashSet(StringComparer.Ordinal);

        var unexpected = Directory
            .GetFiles(directory)
            .Select(Path.GetFileName)
            .Where(name => !expected.Contains(name!))
            .ToList();

        Assert.That(unexpected, Is.Empty,
            "A hand-added file in the snapshot is one the tool will never update, so it will drift and then mislead.");
    }

    /// <remarks>
    /// The metadata's own count has to agree with what is on disk. These are written by the same run
    /// and so should never disagree, which is precisely why a disagreement is worth catching: it
    /// means something wrote to the folder that was not the tool.
    /// </remarks>
    [TestCaseSource(nameof(SnapshotDirectories))]
    public void TheRecordedFeatureCountMatchesWhatIsOnDisk(string snapshot)
    {
        var directory = Path.Combine(_specRoot, snapshot);
        var result = SpecMetadata.Read(directory);

        Assert.That(result.IsUsable, Is.True, result.Reason);
        Assert.That(result.Metadata!.FeatureCount, Is.EqualTo(Directory.GetFiles(directory, "*.yml").Length));
    }

    private static string LocateSpecRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "specification", "openai");

            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate specification/openai from the test directory.");
    }
}
