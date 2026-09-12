using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OpenAI.SpecProcessor.Spec;

/// <summary>
/// A fingerprint of everything about this tool that can change a snapshot without the source
/// changing.
/// </summary>
/// <remarks>
/// The decision to skip a run was originally made on the source content hash alone, which answers
/// "did upstream change" and quietly assumes the answer to "would we produce the same thing anyway".
/// That assumption stops holding the moment the feature map, the exclusions, the sanitizer, or the
/// comparison scope is edited. A month where upstream is still but the taxonomy moved would
/// short-circuit, and the committed snapshot would go on describing itself with a taxonomy the tool
/// no longer uses. Nobody would see it, because the run that would have shown it never happened.
///
/// So the no-op decision compares two things: the bytes that came in, and the behavior that would
/// process them. Either one moving is a reason to run.
///
/// Most of this is derived rather than declared, because a value a person has to remember to bump is
/// a value that will eventually be wrong. Adding a feature area or editing an exclusion changes the
/// identity on its own. The two hand-maintained numbers cover the cases that cannot be derived: the
/// logic inside the splitter and differ, and the logic inside the sanitizer.
/// </remarks>
public record ProcessingIdentity
{
    /// <summary>
    /// The revision of the split and diff logic itself.
    /// </summary>
    /// <remarks>
    /// Increment this when a change to the processor would produce different output from identical
    /// input, in a way that is not already captured by the configuration hash. Reference resolution,
    /// ordering, serialization, and comparison rules all qualify.
    /// </remarks>
    public const int CurrentBehaviorVersion = 1;

    /// <summary> The revision of the split and diff logic. </summary>
    public int BehaviorVersion { get; init; } = CurrentBehaviorVersion;

    /// <summary> The revision of the source repairs applied before parsing. </summary>
    public int SanitizerVersion { get; init; } = SpecSanitizer.Version;

    /// <summary> The revision of what the structural comparison covers. </summary>
    public int DiffScopeVersion { get; init; } = DiffScope.Current.Version;

    /// <summary> A hash of the feature map and the exclusion rules. </summary>
    public string ConfigurationHash { get; init; } = ComputeConfigurationHash();

    /// <summary> The identity in effect for snapshots produced by this build. </summary>
    public static ProcessingIdentity Current { get; } = new();

    /// <summary>
    /// A single value combining every component, for comparison.
    /// </summary>
    /// <remarks>
    /// The components are recorded individually as well, so that a run which decides to regenerate
    /// can say which part of the behavior moved rather than only that something did.
    /// </remarks>
    public string Fingerprint =>
        Hash($"behavior={BehaviorVersion};sanitizer={SanitizerVersion};diffScope={DiffScopeVersion};config={ConfigurationHash}");

    /// <summary> Describes how this identity differs from another, for a human. </summary>
    /// <param name="other"> The identity to compare against, typically the recorded one. </param>
    /// <returns> One line per component that differs, empty when they match. </returns>
    public IReadOnlyList<string> DescribeDifferences(ProcessingIdentity? other)
    {
        if (other == null)
        {
            return ["the previous snapshot recorded no processing identity"];
        }

        var differences = new List<string>();

        if (BehaviorVersion != other.BehaviorVersion)
        {
            differences.Add($"the split and diff logic moved from version {other.BehaviorVersion} to {BehaviorVersion}");
        }

        if (SanitizerVersion != other.SanitizerVersion)
        {
            differences.Add($"the source repairs moved from version {other.SanitizerVersion} to {SanitizerVersion}");
        }

        if (DiffScopeVersion != other.DiffScopeVersion)
        {
            differences.Add($"the comparison scope moved from version {other.DiffScopeVersion} to {DiffScopeVersion}");
        }

        if (!string.Equals(ConfigurationHash, other.ConfigurationHash, StringComparison.Ordinal))
        {
            differences.Add("the feature map or the exclusion rules changed");
        }

        return differences;
    }

    /// <summary>
    /// Hashes the configuration that decides what lands where.
    /// </summary>
    /// <remarks>
    /// Everything that influences the split is included, and it is written out in a stable order so
    /// that reordering a declaration does not read as a behavior change. What is deliberately left
    /// out is anything cosmetic: a reworded comment or a renamed local should not invalidate a
    /// snapshot.
    /// </remarks>
    private static string ComputeConfigurationHash()
    {
        var builder = new StringBuilder();

        builder.Append("excludedPathPrefixes=").AppendJoin(',', FeatureAreaConfig.ExcludedPathPrefixes.Order(StringComparer.Ordinal)).Append(';');
        builder.Append("excludedExactPaths=").AppendJoin(',', FeatureAreaConfig.ExcludedExactPaths.Order(StringComparer.Ordinal)).Append(';');
        builder.Append("excludedTags=").AppendJoin(',', FeatureAreaConfig.ExcludedTags.Order(StringComparer.Ordinal)).Append(';');
        builder.Append("strippedKeys=").AppendJoin(',', FeatureAreaConfig.MetadataKeysToStrip.Order(StringComparer.Ordinal)).Append(';');

        foreach (var area in FeatureAreaConfig.All.OrderBy(area => area.Name, StringComparer.Ordinal))
        {
            builder.Append("area=").Append(area.Name)
                .Append("|file=").Append(area.OutputFile)
                .Append("|tags=").AppendJoin(',', area.Tags.Order(StringComparer.Ordinal))
                .Append("|prefixes=").AppendJoin(',', area.PathPrefixes.Order(StringComparer.Ordinal))
                .Append("|explicit=").AppendJoin(',', area.ExplicitPaths.Order(StringComparer.Ordinal))
                .Append("|excluded=").AppendJoin(',', area.ExcludedPathPrefixes.Order(StringComparer.Ordinal))
                .Append(';');
        }

        return Hash(builder.ToString());
    }

    private static string Hash(string value) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    /// <summary> Serializes the current identity for another process to compare against. </summary>
    /// <returns> A JSON document describing the identity. </returns>
    public string ToJson() =>
        JsonSerializer.Serialize(this, ProcessingIdentityContext.Options);
}

internal static class ProcessingIdentityContext
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
