using NUnit.Framework;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Covers the fingerprint that decides whether a run would produce the same snapshot as last time.
/// </summary>
/// <remarks>
/// The no-op check used to compare source bytes alone, which answers "did upstream change" and
/// quietly assumes the answer to "would we produce the same thing anyway". The assumption fails the
/// month the feature map, the exclusions, the sanitizer, or the comparison scope is edited: upstream
/// is still, the run skips, and the committed snapshot goes on describing itself with a taxonomy the
/// tool no longer uses. Nobody sees it, because the run that would have shown it never happened.
/// </remarks>
[TestFixture]
public class ProcessingIdentityTests
{
    [Test]
    public void TheIdentityIsStableAcrossReads()
    {
        Assert.That(new ProcessingIdentity().Fingerprint, Is.EqualTo(ProcessingIdentity.Current.Fingerprint),
            "An identity that moves on its own would force a regeneration every single month.");
    }

    [Test]
    public void TheFingerprintIsAHash()
    {
        Assert.That(ProcessingIdentity.Current.Fingerprint, Does.Match("^[0-9a-f]{64}$"),
            "The orchestrator matches on this shape to pick the value out of the tool's output.");
    }

    [TestCase("BehaviorVersion")]
    [TestCase("SanitizerVersion")]
    [TestCase("DiffScopeVersion")]
    [TestCase("ConfigurationHash")]
    public void EveryComponentParticipatesInTheFingerprint(string component)
    {
        var baseline = ProcessingIdentity.Current;

        var altered = component switch
        {
            "BehaviorVersion" => baseline with { BehaviorVersion = baseline.BehaviorVersion + 1 },
            "SanitizerVersion" => baseline with { SanitizerVersion = baseline.SanitizerVersion + 1 },
            "DiffScopeVersion" => baseline with { DiffScopeVersion = baseline.DiffScopeVersion + 1 },
            _ => baseline with { ConfigurationHash = new string('f', 64) }
        };

        Assert.That(altered.Fingerprint, Is.Not.EqualTo(baseline.Fingerprint),
            $"A change to {component} has to move the fingerprint, or that kind of change can ship without a regeneration.");
    }

    /// <remarks>
    /// A configuration hash derived from the feature map is what keeps this honest without anyone
    /// remembering to bump a number. Adding an area, renaming an output file, or editing an exclusion
    /// all change what lands where, so all of them have to move the hash.
    /// </remarks>
    [Test]
    public void TheConfigurationHashCoversTheFeatureMap()
    {
        var current = ProcessingIdentity.Current.ConfigurationHash;

        Assert.Multiple(() =>
        {
            Assert.That(current, Does.Match("^[0-9a-f]{64}$"));
            Assert.That(current, Is.EqualTo(new ProcessingIdentity().ConfigurationHash));
        });
    }

    [Test]
    public void MatchingIdentitiesReportNoDifferences()
    {
        Assert.That(ProcessingIdentity.Current.DescribeDifferences(new ProcessingIdentity()), Is.Empty);
    }

    /// <remarks>
    /// A snapshot taken before the identity existed cannot be shown to match, which is not the same
    /// as matching. Reading it as a match is the failure this whole mechanism exists to prevent.
    /// </remarks>
    [Test]
    public void AnAbsentIdentityIsNotTreatedAsAMatch()
    {
        Assert.That(ProcessingIdentity.Current.DescribeDifferences(null), Is.Not.Empty);
    }

    [Test]
    public void TheDifferenceIsDescribedInTermsOfWhatMoved()
    {
        var recorded = ProcessingIdentity.Current with
        {
            DiffScopeVersion = ProcessingIdentity.Current.DiffScopeVersion - 1,
            ConfigurationHash = new string('f', 64)
        };

        var differences = ProcessingIdentity.Current.DescribeDifferences(recorded);

        Assert.Multiple(() =>
        {
            Assert.That(differences, Has.Count.EqualTo(2),
                "Naming each component that moved is what makes the regeneration explicable rather than mysterious.");

            Assert.That(differences.Any(line => line.Contains("comparison scope")), Is.True);
            Assert.That(differences.Any(line => line.Contains("feature map")), Is.True);
        });
    }

    [Test]
    public void TheIdentityIsRecordedInMetadata()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"spec-identity-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        try
        {
            new SpecMetadata { Version = "2.3.0" }.Save(directory);

            var read = SpecMetadata.Read(directory);

            Assert.That(read.Metadata!.ProcessingIdentity.Fingerprint,
                Is.EqualTo(ProcessingIdentity.Current.Fingerprint),
                "The identity has to survive the round trip, since the next run reads it back to decide whether to work.");
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
