using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace OpenAI.Tests.Miscellaneous;

/// <summary>
/// Resilience tests for the SDK platform metadata values against the <em>real</em> environment.
/// </summary>
/// <remarks>
/// Mocked tests cannot catch a target framework misbehaving on a runtime it was not compiled against; for
/// example, the <c>netstandard2.0</c> asset running on .NET Framework, Mono, or Unity. These assert only the
/// invariants that must hold everywhere, since exact values are environment-dependent and cannot be pinned.
/// </remarks>
public class PlatformTelemetrySmokeTests
{
    private static readonly TelemetryDetails.RuntimeInformationWrapper RuntimeInformation = new();

    [Test]
    public void PackageVersionIsUsableOnThisRuntime()
    {
        // Deliberately not length-bounded: the package version must match the user agent verbatim, which the
        // separate drift-guard test asserts.
        AssertUsable(() => PlatformTelemetry.GetPackageVersion(typeof(PlatformTelemetry).Assembly), maxLength: int.MaxValue);
    }

    [Test]
    public void RuntimeVersionIsUsableOnThisRuntime()
        => AssertUsable(() => PlatformTelemetry.GetRuntimeVersion(RuntimeInformation));

    [Test]
    public void OperatingSystemIsUsableOnThisRuntime()
        => AssertUsable(() => PlatformTelemetry.GetOperatingSystem(RuntimeInformation));

    [Test]
    public void ArchitectureIsUsableOnThisRuntime()
        => AssertUsable(() => PlatformTelemetry.GetArchitecture(RuntimeInformation));

    [Test]
    public void OperatingSystemIsRecognizedOnThisRuntime()
    {
        // The test matrix runs on platforms the SDK recognizes explicitly, so falling into the "Other:" bucket
        // signals that detection has regressed rather than that a novel platform appeared.
        Assert.That(PlatformTelemetry.GetOperatingSystem(RuntimeInformation), Does.Not.StartWith("Other:"));
    }

    [Test]
    public void ArchitectureIsRecognizedOnThisRuntime()
    {
        Assert.That(PlatformTelemetry.GetArchitecture(RuntimeInformation), Does.Not.StartWith("other:"));
    }

    [Test]
    public void PackageVersionMatchesTheUserAgentVersion()
    {
        // Guards against the header and the user agent drifting apart; both must read the same attribute and
        // apply the same commit-suffix strip.
        Assembly assembly = typeof(PlatformTelemetry).Assembly;
        string userAgent = TelemetryDetails.GenerateUserAgentString(assembly);
        string version = PlatformTelemetry.GetPackageVersion(assembly);

        Assert.That(userAgent, Does.Contain($"/{version} "));
    }

    private static void AssertUsable(Func<string> valueFactory, int maxLength = 72)
    {
        string value = null;

        Assert.That(() => value = valueFactory(), Throws.Nothing);
        Assert.That(value, Is.Not.Null.And.Not.Empty);
        Assert.That(value.Length, Is.LessThanOrEqualTo(maxLength), "Header values must stay bounded.");

        foreach (char character in value)
        {
            Assert.That(character, Is.InRange(' ', '~'), $"'{value}' contains a character that is not printable ASCII.");
        }

        Assert.That(value, Does.Not.EndWith(" "));
    }
}
