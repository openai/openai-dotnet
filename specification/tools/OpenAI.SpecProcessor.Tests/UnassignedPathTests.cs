using NUnit.Framework;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Covers the reporting of paths no feature area claims.
/// </summary>
/// <remarks>
/// These used to be a console warning and nothing else, which meant a brand-new upstream surface
/// was dropped from every feature document with no lasting trace. The first run after this was
/// fixed caught a real one.
/// </remarks>
[TestFixture]
public class UnassignedPathTests
{
    private const string SpecWithAnUnknownPath = """
        openapi: "3.1.0"
        info:
          title: Test
          version: "1.0.0"
        paths:
          /embeddings:
            post:
              operationId: createEmbedding
              tags:
                - Embeddings
              responses:
                "200":
                  description: Success
          /content_provenance_checks:
            post:
              operationId: Createcontentprovenancecheck
              responses:
                "200":
                  description: Success
        components:
          schemas: {}
        """;

    [Test]
    public void APathNoAreaClaimsIsReportedWithContext()
    {
        var (_, unassigned) = SpecSplitter.Split(SpecCleaner.Clean(SpecFixture.Load(SpecWithAnUnknownPath)));

        Assert.That(unassigned, Has.Count.EqualTo(1));

        var entry = unassigned[0];

        Assert.Multiple(() =>
        {
            Assert.That(entry.Path, Is.EqualTo("/content_provenance_checks"));
            Assert.That(entry.Methods, Is.EquivalentTo(new[] { "POST" }));
            Assert.That(entry.OperationIds, Is.EquivalentTo(new[] { "Createcontentprovenancecheck" }));
            Assert.That(entry.Tags, Is.Empty);
            Assert.That(entry.Reason, Does.Contain("no tags"));
        });
    }

    [Test]
    public void AnUnknownTaggedPathExplainsTheTagItCarried()
    {
        var spec = SpecFixture.Load(SpecWithAnUnknownPath.Replace("""
              operationId: Createcontentprovenancecheck
        """, """
              operationId: Createcontentprovenancecheck
              tags:
                - Provenance
        """));

        var (_, unassigned) = SpecSplitter.Split(SpecCleaner.Clean(spec));

        Assert.That(unassigned, Has.Count.EqualTo(1));
        Assert.That(unassigned[0].Tags, Is.EquivalentTo(new[] { "Provenance" }));
        Assert.That(unassigned[0].Reason, Does.Contain("Provenance"));
    }

    [Test]
    public void AKnownPathIsNotReportedAsUnassigned()
    {
        var (features, unassigned) = SpecSplitter.Split(SpecCleaner.Clean(SpecFixture.Load(SpecFixture.RichSpec)));

        Assert.That(unassigned, Is.Empty);
        Assert.That(features.Keys, Does.Contain("Responses"));
    }

    /// <remarks>
    /// A gap that was reviewed and deliberately left open has to stay visible without announcing
    /// itself as a fresh regression every month, or a reviewer learns to scroll past the section
    /// that exists precisely to be read.
    /// </remarks>
    [Test]
    public void AGapCarriedOverFromTheLastRunIsNotReportedAsNew()
    {
        var previous = new[] { Entry("/content_provenance_checks") };
        var current = new[] { Entry("/content_provenance_checks") };

        var reconciled = UnassignedPath.Reconcile(current, previous);

        Assert.That(reconciled, Has.Count.EqualTo(1));
        Assert.That(reconciled[0].Status, Is.EqualTo(UnassignedStatus.Unchanged));
    }

    [Test]
    public void AGapThatAppearedThisRunIsReportedAsNew()
    {
        var reconciled = UnassignedPath.Reconcile([Entry("/widgets")], [Entry("/content_provenance_checks")]);

        var widgets = reconciled.Single(entry => entry.Path == "/widgets");

        Assert.That(widgets.Status, Is.EqualTo(UnassignedStatus.New));
    }

    [Test]
    public void AGapThatHasSinceBeenClaimedIsCarriedForwardAsResolved()
    {
        var reconciled = UnassignedPath.Reconcile([], [Entry("/content_provenance_checks")]);

        Assert.That(reconciled, Has.Count.EqualTo(1));

        Assert.Multiple(() =>
        {
            Assert.That(reconciled[0].Status, Is.EqualTo(UnassignedStatus.Resolved));
            Assert.That(reconciled[0].Reason, Does.Contain("no longer"));
        });
    }

    /// <remarks>
    /// With nothing to compare against, calling everything new is a guess, but it is the honest one.
    /// The bootstrap run is exactly when every gap deserves a fresh look.
    /// </remarks>
    [Test]
    public void WithoutAPreviousSnapshotEveryGapIsNew()
    {
        var reconciled = UnassignedPath.Reconcile([Entry("/widgets")], null);

        Assert.That(reconciled[0].Status, Is.EqualTo(UnassignedStatus.New));
    }

    [Test]
    public void ResolvedGapsAreNotCarriedIntoTheNextBaseline()
    {
        var reconciled = UnassignedPath.Reconcile([Entry("/widgets")], [Entry("/content_provenance_checks")]);
        var persisted = reconciled.Where(entry => entry.Status != UnassignedStatus.Resolved).ToList();

        Assert.That(persisted.Select(entry => entry.Path), Is.EquivalentTo(new[] { "/widgets" }),
            "A path that is no longer unassigned must not be recorded as unassigned for the next run to find.");
    }

    /// <remarks>
    /// This is the whole life cycle, run month over month. A gap appears, persists, is claimed, and
    /// then the routing regresses. The interesting part is the third step: "resolved" is reported in
    /// the month it happens and then dropped from the baseline, so it is said once rather than
    /// repeated forever. The fourth step is the consequence of that choice, and it is the right one:
    /// a path that falls out of the feature map a second time is genuinely a new regression, not a
    /// continuation of the old one, and should read that way.
    /// </remarks>
    [Test]
    public void AResolvedGapIsReportedOnceAndThenForgotten()
    {
        // Month one: nothing to compare against, so the gap is new.

        var firstMonth = UnassignedPath.Reconcile([Entry("/widgets")], null);
        var firstBaseline = Persist(firstMonth);

        // Month two: still unclaimed, so it stays visible without reading as a regression.

        var secondMonth = UnassignedPath.Reconcile([Entry("/widgets")], firstBaseline);
        var secondBaseline = Persist(secondMonth);

        // Month three: an area claims it, so it is called out as resolved and leaves the baseline.

        var thirdMonth = UnassignedPath.Reconcile([], secondBaseline);
        var thirdBaseline = Persist(thirdMonth);

        // Month four: nothing is unassigned, and the resolution is not repeated.

        var fourthMonth = UnassignedPath.Reconcile([], thirdBaseline);

        Assert.Multiple(() =>
        {
            Assert.That(firstMonth[0].Status, Is.EqualTo(UnassignedStatus.New));
            Assert.That(secondMonth[0].Status, Is.EqualTo(UnassignedStatus.Unchanged));

            Assert.That(thirdMonth.Count(entry => entry.Status == UnassignedStatus.Resolved), Is.EqualTo(1),
                "The resolution is worth saying, once, in the month it happened.");

            Assert.That(thirdBaseline, Is.Empty,
                "A resolved path leaves the baseline, so the next run has nothing to repeat.");

            Assert.That(fourthMonth, Is.Empty,
                "Having been reported once, the resolution is not announced again.");
        });

        // Month five: the routing regresses and the path falls out of the map again.

        var fifthMonth = UnassignedPath.Reconcile([Entry("/widgets")], Persist(fourthMonth));

        Assert.That(fifthMonth[0].Status, Is.EqualTo(UnassignedStatus.New),
            "A gap that reopens is a fresh regression rather than a continuation of the old one.");
    }

    /// <summary> Applies the same filter the processor applies when writing the next baseline. </summary>
    private static IReadOnlyList<UnassignedPath> Persist(IReadOnlyList<UnassignedPath> reconciled) =>
        [.. reconciled.Where(entry => entry.Status != UnassignedStatus.Resolved)];

    private static UnassignedPath Entry(string path) =>
        new() { Path = path, Methods = ["POST"], OperationIds = ["someOperation"] };
}
