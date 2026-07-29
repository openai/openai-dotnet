using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace OpenAI.Tests.Miscellaneous;

/// <summary>
/// Unit tests for the normalization contract behind the SDK platform metadata headers.
/// </summary>
/// <remarks>
/// These exercise the helpers through a mocked <see cref="TelemetryDetails.RuntimeInformationWrapper"/> so that
/// every branch is reachable without depending on the machine the tests happen to run on. The complementary
/// smoke tests in <c>PlatformTelemetrySmokeTests</c> cover the real environment.
/// </remarks>
public class PlatformTelemetryTests
{
    private const string UnknownValue = "unknown";

    #region Operating system

    [TestCase("WINDOWS", "Windows")]
    [TestCase("OSX", "MacOS")]
    [TestCase("LINUX", "Linux")]
    [TestCase("FREEBSD", "FreeBSD")]
    [TestCase("ANDROID", "Android")]
    [TestCase("IOS", "iOS")]
    [TestCase("MACCATALYST", "MacCatalyst")]
    [TestCase("BROWSER", "Browser")]
    public void OperatingSystemIsNormalizedForKnownPlatforms(string platformName, string expected)
    {
        MockRuntimeInformation runtimeInformation = new();
        runtimeInformation.Platforms.Add(OSPlatform.Create(platformName));

        Assert.That(PlatformTelemetry.GetOperatingSystem(runtimeInformation), Is.EqualTo(expected));
    }

    // Modern .NET reports these platforms as mutually exclusive, but Mono and legacy Xamarin report Android as
    // Linux and the Apple mobile platforms as OSX. The more specific platform must win on every runtime.
    [TestCase("ANDROID", "LINUX", "Android")]
    [TestCase("IOS", "OSX", "iOS")]
    [TestCase("MACCATALYST", "OSX", "MacCatalyst")]
    [TestCase("BROWSER", "LINUX", "Browser")]
    public void SpecificOperatingSystemsAreEvaluatedBeforeTheirBasePlatform(string specific, string general, string expected)
    {
        MockRuntimeInformation runtimeInformation = new();
        runtimeInformation.Platforms.Add(OSPlatform.Create(specific));
        runtimeInformation.Platforms.Add(OSPlatform.Create(general));

        Assert.That(PlatformTelemetry.GetOperatingSystem(runtimeInformation), Is.EqualTo(expected));
    }

    [Test]
    public void UnrecognizedOperatingSystemFallsBackToItsDescription()
    {
        MockRuntimeInformation runtimeInformation = new() { OSDescriptionValue = "Contoso OS 1.2" };

        Assert.That(PlatformTelemetry.GetOperatingSystem(runtimeInformation), Is.EqualTo("Other:Contoso OS 1.2"));
    }

    [Test]
    public void UnrecognizedOperatingSystemWithNoDescriptionIsUnknown()
    {
        MockRuntimeInformation runtimeInformation = new() { OSDescriptionValue = string.Empty };

        Assert.That(PlatformTelemetry.GetOperatingSystem(runtimeInformation), Is.EqualTo($"Other:{UnknownValue}"));
    }

    [Test]
    public void OperatingSystemDescriptionIsSanitized()
    {
        // Azure.Core carries an equivalent guard because operating systems with non-ASCII characters in their
        // release names have been reported in the field.
        MockRuntimeInformation runtimeInformation = new() { OSDescriptionValue = "Contoso\r\nOS \u00e9\u4e2d" };

        Assert.That(PlatformTelemetry.GetOperatingSystem(runtimeInformation), Is.EqualTo("Other:Contoso__OS __"));
    }

    [Test]
    public void OperatingSystemNeverThrows()
    {
        MockRuntimeInformation runtimeInformation = new() { ShouldThrow = true };

        string value = null;
        Assert.That(() => value = PlatformTelemetry.GetOperatingSystem(runtimeInformation), Throws.Nothing);
        Assert.That(value, Is.EqualTo($"Other:{UnknownValue}"));
    }

    #endregion

    #region Architecture

    [TestCase(Architecture.X64, "x64")]
    [TestCase(Architecture.X86, "x86")]
    [TestCase(Architecture.Arm64, "arm64")]
    [TestCase(Architecture.Arm, "arm")]
    public void ArchitectureIsNormalizedForKnownValues(Architecture architecture, string expected)
    {
        MockRuntimeInformation runtimeInformation = new() { ProcessArchitectureValue = architecture };

        Assert.That(PlatformTelemetry.GetArchitecture(runtimeInformation), Is.EqualTo(expected));
    }

    [Test]
    public void UnrecognizedArchitectureFallsBackToItsName()
    {
        // Wasm, RiscV64, and friends are absent from the netstandard2.0 reference assembly but resolve against
        // the running runtime's metadata, so the enum name is still produced rather than a bare number.
        MockRuntimeInformation runtimeInformation = new() { ProcessArchitectureValue = (Architecture)9999 };

        Assert.That(PlatformTelemetry.GetArchitecture(runtimeInformation), Is.EqualTo("other:9999"));
    }

    [Test]
    public void ArchitectureNeverThrows()
    {
        MockRuntimeInformation runtimeInformation = new() { ShouldThrow = true };

        string value = null;
        Assert.That(() => value = PlatformTelemetry.GetArchitecture(runtimeInformation), Throws.Nothing);
        Assert.That(value, Is.EqualTo($"other:{UnknownValue}"));
    }

    #endregion

    #region Runtime version

    [TestCase(".NET 8.0.11", "8.0.11")]
    [TestCase(".NET 10.0.0-preview.1.25080.5", "10.0.0-preview.1.25080.5")]
    [TestCase(".NET Core 3.1.32", "3.1.32")]
    [TestCase(".NET Framework 4.8.9256.0", "4.8.9256.0")]
    [TestCase("Mono 6.12.0.199", "6.12.0.199")]
    public void RuntimeVersionIsExtractedFromTheFrameworkDescription(string description, string expected)
    {
        MockRuntimeInformation runtimeInformation = new() { FrameworkDescriptionValue = description };

        Assert.That(PlatformTelemetry.GetRuntimeVersion(runtimeInformation), Is.EqualTo(expected));
    }

    [Test]
    public void RuntimeVersionFallsBackToTheWholeDescriptionWhenNoVersionIsPresent()
    {
        MockRuntimeInformation runtimeInformation = new() { FrameworkDescriptionValue = "Contoso Runtime" };

        Assert.That(PlatformTelemetry.GetRuntimeVersion(runtimeInformation), Is.EqualTo("Contoso Runtime"));
    }

    [TestCase("")]
    [TestCase(null)]
    public void RuntimeVersionIsUnknownWhenTheDescriptionIsEmpty(string description)
    {
        MockRuntimeInformation runtimeInformation = new() { FrameworkDescriptionValue = description };

        Assert.That(PlatformTelemetry.GetRuntimeVersion(runtimeInformation), Is.EqualTo(UnknownValue));
    }

    [Test]
    public void RuntimeVersionNeverThrows()
    {
        MockRuntimeInformation runtimeInformation = new() { ShouldThrow = true };

        string value = null;
        Assert.That(() => value = PlatformTelemetry.GetRuntimeVersion(runtimeInformation), Throws.Nothing);
        Assert.That(value, Is.EqualTo(UnknownValue));
    }

    #endregion

    #region Package version

    [Test]
    public void PackageVersionStripsTheCommitSuffix()
    {
        Assert.That(PlatformTelemetry.NormalizePackageVersion("2.12.0+abc123def"), Is.EqualTo("2.12.0"));
        Assert.That(PlatformTelemetry.NormalizePackageVersion("2.13.0-alpha.1+abc123def"), Is.EqualTo("2.13.0-alpha.1"));
    }

    [TestCase("2.12.0")]
    [TestCase("0.0.1-dev.1")]
    [TestCase("2.13.0-alpha.20260408.1")]
    public void PackageVersionIsReproducedVerbatim(string version)
    {
        // The user agent applies no transformation beyond the commit-suffix strip, so neither can this. Any
        // rewriting here would produce a header that looks like a plausible version but silently disagrees.
        Assert.That(PlatformTelemetry.NormalizePackageVersion(version), Is.EqualTo(version));
    }

    [Test]
    public void PackageVersionIsNotTruncated()
    {
        // Free-form platform text is length-bounded, but the package version is not: capping it would break
        // the guarantee that this header matches the user agent.
        string version = "2.12.0-" + new string('a', 200);

        Assert.That(PlatformTelemetry.NormalizePackageVersion(version), Is.EqualTo(version));
    }

    [TestCase("2.12.0\r\n")]
    [TestCase("2.12.0-caf\u00e9")]
    [TestCase("2.12.0\u0000")]
    [TestCase("2.12.0\t")]
    [TestCase("2.12.0 ")]
    [TestCase(" 2.12.0")]
    [TestCase("+abc123")]
    public void PackageVersionIsUnknownWhenItCouldNotBeSentVerbatim(string version)
    {
        // Reporting unknown is preferable to emitting a rewritten value that would be indistinguishable from a
        // real version while disagreeing with the user agent. Surrounding spaces count as unsendable because
        // RFC 7230 lets an HTTP stack strip them from a field value.
        Assert.That(PlatformTelemetry.NormalizePackageVersion(version), Is.EqualTo(UnknownValue));
    }

    [Test]
    public void PackageVersionAllowsInteriorSpaces()
    {
        // Interior whitespace is legal in a header value and is transmitted unaltered, so it does not force
        // the fallback.
        Assert.That(PlatformTelemetry.NormalizePackageVersion("2.12.0 beta"), Is.EqualTo("2.12.0 beta"));
    }

    [TestCase(null)]
    [TestCase("")]
    public void PackageVersionIsUnknownWhenTheVersionIsEmpty(string version)
    {
        Assert.That(PlatformTelemetry.NormalizePackageVersion(version), Is.EqualTo(UnknownValue));
    }

    [Test]
    public void PackageVersionMatchesTheAssemblyAttribute()
    {
        Assembly assembly = typeof(PlatformTelemetryTests).Assembly;
        string informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        Assume.That(informationalVersion, Is.Not.Null.And.Not.Empty);

        Assert.That(
            PlatformTelemetry.GetPackageVersion(assembly),
            Is.EqualTo(PlatformTelemetry.NormalizePackageVersion(informationalVersion)));
    }

    [Test]
    public void PackageVersionIsUnknownWhenTheAssemblyIsUnavailable()
    {
        string value = null;
        Assert.That(() => value = PlatformTelemetry.GetPackageVersion(null), Throws.Nothing);
        Assert.That(value, Is.EqualTo(UnknownValue));
    }

    #endregion

    #region Sanitization

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void SanitizeUsesTheFallbackWhenNothingUsableRemains(string value)
    {
        // A whitespace-only value is not meaningful, and trailing spaces are invalid in a header value.
        Assert.That(PlatformTelemetry.Sanitize(value, UnknownValue), Is.EqualTo(UnknownValue));
    }

    [Test]
    public void SanitizeReplacesRatherThanDropsUnusableCharacters()
    {
        // Replacing keeps distinct platforms distinguishable, which dropping would not.
        Assert.That(PlatformTelemetry.Sanitize("\r\n", UnknownValue), Is.EqualTo("__"));
    }

    [Test]
    public void SanitizeReplacesNonPrintableAndNonAsciiCharacters()
    {
        Assert.That(PlatformTelemetry.Sanitize("a\rb\nc\td\u00e9e\u0000f", UnknownValue), Is.EqualTo("a_b_c_d_e_f"));
    }

    [Test]
    public void SanitizePreservesPrintableAscii()
    {
        const string value = "Darwin 21.1.0 (x86_64) ~!@#$%^&*()_+";
        Assert.That(PlatformTelemetry.Sanitize(value, UnknownValue), Is.EqualTo(value));
    }

    [Test]
    public void SanitizeBoundsTheValueLength()
    {
        // OSDescription exceeds 130 characters on macOS, which is not appropriate for a header sent on every
        // request.
        string value = new('a', 500);
        string sanitized = PlatformTelemetry.Sanitize(value, UnknownValue);

        Assert.That(sanitized.Length, Is.EqualTo(64));
        Assert.That(sanitized, Is.EqualTo(new string('a', 64)));
    }

    [Test]
    public void SanitizeTrimsTrailingSpacesIntroducedByTruncation()
    {
        string sanitized = PlatformTelemetry.Sanitize(new string('a', 60) + "    tail", UnknownValue);

        Assert.That(sanitized, Is.EqualTo(new string('a', 60)));
    }

    [Test]
    public void SanitizeTrimsLeadingSpaces()
    {
        // A leading space is optional whitespace that an HTTP stack may strip, so the value that was computed
        // would not be the value that arrives.
        Assert.That(PlatformTelemetry.Sanitize("   Darwin 21.1.0", UnknownValue), Is.EqualTo("Darwin 21.1.0"));
    }

    [Test]
    public void SanitizePreservesInteriorSpaces()
    {
        // Only the edges are whitespace-sensitive; interior spaces are legal and carry meaning.
        Assert.That(PlatformTelemetry.Sanitize("  Darwin 21.1.0 (x86_64)  ", UnknownValue), Is.EqualTo("Darwin 21.1.0 (x86_64)"));
    }

    #endregion

    /// <summary>
    /// A controllable stand-in for the runtime, allowing every normalization branch to be reached regardless of
    /// the machine the tests run on.
    /// </summary>
    private class MockRuntimeInformation : TelemetryDetails.RuntimeInformationWrapper
    {
        public HashSet<OSPlatform> Platforms { get; } = new();

        public string FrameworkDescriptionValue { get; set; } = ".NET 8.0.0";

        public string OSDescriptionValue { get; set; } = "Mock OS";

        public Architecture ProcessArchitectureValue { get; set; } = Architecture.X64;

        public bool ShouldThrow { get; set; }

        public override string FrameworkDescription => ShouldThrow ? throw new InvalidOperationException() : FrameworkDescriptionValue;

        public override string OSDescription => ShouldThrow ? throw new InvalidOperationException() : OSDescriptionValue;

        public override Architecture ProcessArchitecture => ShouldThrow ? throw new InvalidOperationException() : ProcessArchitectureValue;

        public override Architecture OSArchitecture => ShouldThrow ? throw new InvalidOperationException() : ProcessArchitectureValue;

        public override bool IsOSPlatform(OSPlatform osPlatform)
            => ShouldThrow ? throw new InvalidOperationException() : Platforms.Contains(osPlatform);
    }
}
