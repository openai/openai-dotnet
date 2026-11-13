using NUnit.Framework;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Covers the narrow repair that makes the upstream document parseable by a strict YAML reader.
/// </summary>
/// <remarks>
/// Upstream publishes a block scalar whose only leading line is whitespace indented past the block.
/// The YAML specification forbids that, and lenient readers accept it. The repair has to stay
/// narrow: anything it touches beyond that case silently alters the specification we are mirroring.
/// </remarks>
[TestFixture]
public class SpecSanitizerTests
{
    [Test]
    public void AnEmptyBlockScalarLineIndentedPastTheBlockIsRepaired()
    {
        var yaml = "root:\n  example: |+\n            \n  nullable: true\n";

        var repaired = SpecSanitizer.Repair(yaml, out var repairedLines);

        Assert.That(repairedLines, Is.EqualTo(1));
        Assert.That(repaired, Does.Not.Contain("            \n"));
    }

    [Test]
    public void ADocumentThatNeedsNoRepairIsReturnedUnchanged()
    {
        var repaired = SpecSanitizer.Repair(SpecFixture.RichSpec, out var repairedLines);

        Assert.Multiple(() =>
        {
            Assert.That(repairedLines, Is.Zero);
            Assert.That(repaired, Is.EqualTo(SpecFixture.RichSpec));
        });
    }

    /// <remarks>
    /// The upstream document contains <c>default: &lt;|endoftext|&gt;</c> immediately above the
    /// malformed block. A header detector that only looks for a trailing indicator character reads
    /// that as a block scalar and corrupts the line below it.
    /// </remarks>
    [Test]
    public void AValueThatMerelyEndsInAPipeIsNotTreatedAsABlockScalar()
    {
        var yaml = "root:\n  default: <|endoftext|>\n      \n  nullable: true\n";

        var repaired = SpecSanitizer.Repair(yaml, out var repairedLines);

        Assert.Multiple(() =>
        {
            Assert.That(repairedLines, Is.Zero);
            Assert.That(repaired, Is.EqualTo(yaml));
        });
    }

    [Test]
    public void BlockScalarContentIsLeftAlone()
    {
        var yaml = "root:\n  example: |\n    first line\n\n    third line\n";

        var repaired = SpecSanitizer.Repair(yaml, out var repairedLines);

        Assert.Multiple(() =>
        {
            Assert.That(repairedLines, Is.Zero);
            Assert.That(repaired, Is.EqualTo(yaml));
        });
    }
}
