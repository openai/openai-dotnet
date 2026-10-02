using System.Text.Json;
using NUnit.Framework;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Covers how operations are routed to feature areas, and what happens to the ones that are not.
/// </summary>
[TestFixture]
public class FeatureAreaConfigTests
{
    /// <remarks>
    /// Tag matching used to return before exclusions were consulted, so Fine Tuning's declared
    /// exclusion of the grader paths never fired and the Graders area could never receive them.
    /// This is also the only mechanism by which threads, messages, and runs can be carved out of
    /// the single Assistants tag.
    /// </remarks>
    [Test]
    public void AnAreaThatDisclaimsAPathDoesNotGetItFromATag()
    {
        var area = FeatureAreaConfig.FindFeatureArea("/fine_tuning/alpha/graders/run", ["Fine-tuning"]);

        Assert.That(area, Is.Not.Null);
        Assert.That(area!.Name, Is.EqualTo("Graders"));
    }

    [Test]
    public void FineTuningStillOwnsItsOwnPaths()
    {
        var area = FeatureAreaConfig.FindFeatureArea("/fine_tuning/jobs", ["Fine-tuning"]);

        Assert.That(area, Is.Not.Null);
        Assert.That(area!.Name, Is.EqualTo("Fine Tuning"));
    }

    [TestCase("/threads", "Threads")]
    [TestCase("/threads/{thread_id}", "Threads")]
    [TestCase("/threads/{thread_id}/messages", "Messages")]
    [TestCase("/threads/runs", "Runs")]
    [TestCase("/threads/{thread_id}/runs", "Runs")]
    [TestCase("/threads/{thread_id}/runs/{run_id}/steps", "Runs")]
    [TestCase("/assistants", "Assistants")]
    public void TheAssistantsTagIsSplitByPath(string path, string expectedArea)
    {
        var area = FeatureAreaConfig.FindFeatureArea(path, ["Assistants"]);

        Assert.That(area, Is.Not.Null, $"'{path}' matched no area.");
        Assert.That(area!.Name, Is.EqualTo(expectedArea));
    }

    [TestCase("/files", "Files")]
    [TestCase("/uploads", "Uploads")]
    public void FilesAndUploadsAreSeparateAreas(string path, string expectedArea)
    {
        var area = FeatureAreaConfig.FindFeatureArea(path, null);

        Assert.That(area, Is.Not.Null);
        Assert.That(area!.Name, Is.EqualTo(expectedArea));
    }

    [TestCase("/completions")]
    [TestCase("/chatkit/sessions")]
    [TestCase("/realtime/sessions")]
    [TestCase("/realtime/transcription_sessions")]
    public void DeliberatelyExcludedPathsAreExcluded(string path)
    {
        Assert.That(FeatureAreaConfig.IsExcludedPath(path), Is.True);
    }

    [TestCase("/chat/completions")]
    [TestCase("/realtime/calls")]
    public void PathsThatMerelyResembleExclusionsAreKept(string path)
    {
        Assert.That(FeatureAreaConfig.IsExcludedPath(path), Is.False);
    }

    [Test]
    public void AreaNamesAndOutputFilesAreUnique()
    {
        Assert.Multiple(() =>
        {
            Assert.That(FeatureAreaConfig.All.Select(a => a.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                Is.EqualTo(FeatureAreaConfig.All.Length));

            Assert.That(FeatureAreaConfig.All.Select(a => a.OutputFile).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                Is.EqualTo(FeatureAreaConfig.All.Length));
        });
    }

    /// <remarks>
    /// The repository's own breakdown is the folder set under <c>specification/base/typespec</c>.
    /// Keeping the two aligned is what lets the diff report speak the same language as the code, so
    /// a folder appearing or disappearing upstream of this tool should be a reviewed decision rather
    /// than a silent divergence.
    ///
    /// Deliberate divergence is allowed for, but it has to be declared. The exceptions live in
    /// <c>specification/tools/taxonomy-exceptions.json</c> with a reason attached, so reorganizing on
    /// purpose means editing a reviewed file rather than editing an assertion.
    /// </remarks>
    [Test]
    public void TheAreaMapMatchesTheRepositoryTaxonomy()
    {
        var typeSpecRoot = FindTypeSpecRoot();

        if (typeSpecRoot == null)
        {
            Assert.Ignore("The repository TypeSpec folder was not found from the test location.");
            return;
        }

        var (foldersWithoutArea, areasWithoutFolder) = LoadTaxonomyExceptions(typeSpecRoot);

        var folders = Directory.GetDirectories(typeSpecRoot)
            .Select(Path.GetFileName)
            .OfType<string>()
            .Where(name => !foldersWithoutArea.Contains(name))
            .ToArray();

        var areas = FeatureAreaConfig.All
            .Select(area => Path.GetFileNameWithoutExtension(area.OutputFile))
            .Where(name => !areasWithoutFolder.Contains(name))
            .ToArray();

        var unmirrored = folders.Except(areas, StringComparer.OrdinalIgnoreCase).ToArray();
        var unbacked = areas.Except(folders, StringComparer.OrdinalIgnoreCase).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(unmirrored, Is.Empty,
                "TypeSpec folders with no matching feature area. Either add the area, or declare the divergence in " +
                "specification/tools/taxonomy-exceptions.json under 'typeSpecFoldersWithoutFeatureArea' with a reason.");

            Assert.That(unbacked, Is.Empty,
                "Feature areas with no matching TypeSpec folder. Either rename the area to match, or declare the " +
                "divergence in specification/tools/taxonomy-exceptions.json under 'featureAreasWithoutTypeSpecFolder'.");
        });
    }

    /// <remarks>
    /// A declared exception has to name something that exists. Otherwise the list quietly rots: a
    /// folder gets renamed, its exception stops matching anything, and the check silently starts
    /// enforcing something nobody decided.
    /// </remarks>
    [Test]
    public void TheDeclaredTaxonomyExceptionsAreStillReal()
    {
        var typeSpecRoot = FindTypeSpecRoot();

        if (typeSpecRoot == null)
        {
            Assert.Ignore("The repository TypeSpec folder was not found from the test location.");
            return;
        }

        var (foldersWithoutArea, areasWithoutFolder) = LoadTaxonomyExceptions(typeSpecRoot);

        var folders = Directory.GetDirectories(typeSpecRoot)
            .Select(Path.GetFileName)
            .OfType<string>()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var areas = FeatureAreaConfig.All
            .Select(area => Path.GetFileNameWithoutExtension(area.OutputFile))
            .OfType<string>()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Multiple(() =>
        {
            Assert.That(foldersWithoutArea.Where(name => !folders.Contains(name)), Is.Empty,
                "A declared TypeSpec folder exception names a folder that no longer exists. Remove the stale entry.");

            Assert.That(areasWithoutFolder.Where(name => !areas.Contains(name)), Is.Empty,
                "A declared feature area exception names an area that no longer exists. Remove the stale entry.");
        });
    }

    /// <summary> Reads the declared taxonomy divergences. </summary>
    /// <param name="typeSpecRoot"> The resolved TypeSpec folder, used to locate the repository. </param>
    /// <returns> The folders with no feature area, and the feature areas with no folder. </returns>
    private static (HashSet<string> FoldersWithoutArea, HashSet<string> AreasWithoutFolder) LoadTaxonomyExceptions(string typeSpecRoot)
    {
        var path = Path.Combine(
            Directory.GetParent(typeSpecRoot)!.Parent!.FullName,
            "tools",
            "taxonomy-exceptions.json");

        Assert.That(File.Exists(path), Is.True, $"The taxonomy exception list is missing from {path}.");

        using var document = JsonDocument.Parse(File.ReadAllText(path));

        var folders = ReadNames(document.RootElement, "typeSpecFoldersWithoutFeatureArea", "folder");
        var areas = ReadNames(document.RootElement, "featureAreasWithoutTypeSpecFolder", "area");

        return (folders, areas);
    }

    /// <summary> Reads the declared names from one exception list, requiring a reason for each. </summary>
    /// <param name="root"> The document root. </param>
    /// <param name="property"> The list to read. </param>
    /// <param name="nameField"> The field holding the name within each entry. </param>
    /// <returns> The declared names. </returns>
    private static HashSet<string> ReadNames(JsonElement root, string property, string nameField)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!root.TryGetProperty(property, out var list))
        {
            return names;
        }

        foreach (var entry in list.EnumerateArray())
        {
            var name = entry.GetProperty(nameField).GetString();

            Assert.That(name, Is.Not.Null.And.Not.Empty, $"An entry in '{property}' has no {nameField}.");
            Assert.That(entry.TryGetProperty("reason", out var reason) && !string.IsNullOrWhiteSpace(reason.GetString()), Is.True,
                $"The '{name}' entry in '{property}' has no reason. The list exists to record why a divergence is intended.");

            names.Add(name!);
        }

        return names;
    }

    private static string? FindTypeSpecRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "specification", "base", "typespec");

            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
