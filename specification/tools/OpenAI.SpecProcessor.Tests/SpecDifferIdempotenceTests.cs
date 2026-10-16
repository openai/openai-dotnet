using NUnit.Framework;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Guards the property the whole snapshot approach depends on: comparing a specification against
/// itself must find nothing.
/// </summary>
/// <remarks>
/// A false positive here is not a cosmetic problem. It produces a pull request every month
/// reporting changes nobody made, which is exactly the noise that trains reviewers to stop reading
/// these reports. A discriminator regression once produced 56 such changes.
/// </remarks>
[TestFixture]
public class SpecDifferIdempotenceTests
{
    [Test]
    public void ComparingASpecificationToItselfFindsNoChanges()
    {
        var older = SpecFixture.Load(SpecFixture.RichSpec);
        var newer = SpecFixture.Load(SpecFixture.RichSpec);

        var diff = SpecDiffer.Diff("Responses", older, newer, includeDescriptions: true);

        Assert.That(diff.Changes, Is.Empty, "An unchanged specification reported changes.");
        Assert.Multiple(() =>
        {
            Assert.That(diff.PathsAdded, Is.Zero);
            Assert.That(diff.PathsRemoved, Is.Zero);
            Assert.That(diff.OperationsAdded, Is.Zero);
            Assert.That(diff.OperationsRemoved, Is.Zero);
            Assert.That(diff.OperationsChanged, Is.Zero);
            Assert.That(diff.SchemasAdded, Is.Zero);
            Assert.That(diff.SchemasRemoved, Is.Zero);
            Assert.That(diff.SchemasChanged, Is.Zero);
            Assert.That(diff.SchemasRenamed, Is.Zero);
        });
    }

    [Test]
    public void SplittingTheSameSpecificationTwiceProducesTheSameDocuments()
    {
        var first = SpecSplitter.Split(SpecCleaner.Clean(SpecFixture.Load(SpecFixture.RichSpec)));
        var second = SpecSplitter.Split(SpecCleaner.Clean(SpecFixture.Load(SpecFixture.RichSpec)));

        Assert.That(second.Features.Keys, Is.EquivalentTo(first.Features.Keys));

        foreach (var (name, spec) in first.Features)
        {
            var diff = SpecDiffer.Diff(name, spec, second.Features[name], includeDescriptions: true);
            Assert.That(diff.Changes, Is.Empty, $"Splitting twice produced a different '{name}' document.");
        }
    }

    [Test]
    public void AChangedDiscriminatorIsStillReported()
    {
        var older = SpecFixture.Load(SpecFixture.RichSpec);
        var newer = SpecFixture.Load(SpecFixture.RichSpec.Replace("propertyName: type", "propertyName: kind"));

        var diff = SpecDiffer.Diff("Responses", older, newer, includeDescriptions: true);

        Assert.That(diff.Changes, Is.Not.Empty, "Suppressing discriminator noise also suppressed a real change.");
    }

    /// <remarks>
    /// This is the byte-level half of the guarantee. Splitting twice can produce documents that
    /// compare as equal while serializing differently, through key ordering or line endings, and a
    /// forced rerun would then show a diff on every feature file with nothing behind it.
    /// </remarks>
    [Test]
    public void SerializingTheSameFeatureDocumentTwiceProducesIdenticalBytes()
    {
        var first = SpecSplitter.Split(SpecCleaner.Clean(SpecFixture.Load(SpecFixture.RichSpec)));
        var second = SpecSplitter.Split(SpecCleaner.Clean(SpecFixture.Load(SpecFixture.RichSpec)));

        var directory = Path.Combine(Path.GetTempPath(), $"spec-determinism-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        try
        {
            foreach (var (name, spec) in first.Features)
            {
                var firstPath = Path.Combine(directory, $"{name}-1.yml");
                var secondPath = Path.Combine(directory, $"{name}-2.yml");

                spec.Save(firstPath);
                second.Features[name].Save(secondPath);

                Assert.That(File.ReadAllBytes(secondPath), Is.EqualTo(File.ReadAllBytes(firstPath)),
                    $"Serializing '{name}' twice produced different bytes.");
            }
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    /// <remarks>
    /// A snapshot file must not carry a carriage return regardless of the host that produced it.
    /// The comparison is byte-for-byte, so a run on Windows and a run on the CI runner have to agree
    /// or every file reads as changed.
    /// </remarks>
    [Test]
    public void SavedFeatureDocumentsUseLineFeedEndings()
    {
        var (features, _) = SpecSplitter.Split(SpecCleaner.Clean(SpecFixture.Load(SpecFixture.RichSpec)));
        var path = Path.Combine(Path.GetTempPath(), $"spec-endings-{Guid.NewGuid():N}.yml");

        try
        {
            features.Values.First().Save(path);

            Assert.That(File.ReadAllBytes(path), Has.No.Member((byte)'\r'),
                "A saved feature document carried a carriage return.");
        }
        finally
        {
            File.Delete(path);
        }
    }
}
