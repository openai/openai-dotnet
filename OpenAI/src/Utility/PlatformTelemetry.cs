using System;
using System.ClientModel.Primitives;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

#nullable enable

namespace OpenAI;

/// <summary>
/// Structured, individually-parseable SDK platform metadata that is sent alongside the
/// <c>User-Agent</c> header so that consumers do not have to parse the user agent string.
/// </summary>
/// <remarks>
/// <para>
/// Every value is process-invariant, so the set is computed once and cached.
/// </para>
/// <para>
/// Values are derived from the same sources used to build the <c>User-Agent</c> (see
/// <see cref="TelemetryDetails"/>), so the two can never contradict each other. The relationship differs by
/// field: the package version is reproduced verbatim and is therefore always identical to its user agent
/// counterpart, whereas the runtime and operating system are normalized subsets of the free-form text the
/// user agent carries. The process architecture has no user agent counterpart at all.
/// </para>
/// <para>
/// Telemetry must never be able to fail a caller's request. Every value is computed defensively and
/// degrades to <see cref="UnknownValue"/> rather than throwing; a failure in the static initializer would
/// otherwise surface as a <see cref="TypeInitializationException"/> on every request for the lifetime of
/// the process.
/// </para>
/// </remarks>
internal static class PlatformTelemetry
{
    public const string LangHeaderName = "X-Stainless-Lang";
    public const string PackageVersionHeaderName = "X-Stainless-Package-Version";
    public const string RuntimeHeaderName = "X-Stainless-Runtime";
    public const string RuntimeVersionHeaderName = "X-Stainless-Runtime-Version";
    public const string OSHeaderName = "X-Stainless-OS";
    public const string ArchHeaderName = "X-Stainless-Arch";

    public const string DisableTelemetrySwitchName = "OpenAI.DisableTelemetry";
    public const string DisableTelemetryEnvironmentVariableName = "OPENAI_DISABLE_TELEMETRY";

    private const string LangValue = "csharp";
    private const string RuntimeValue = "dotnet";
    private const string UnknownValue = "unknown";

    /// <summary>
    /// Upper bound applied to values that incorporate free-form platform text. <c>OSDescription</c> can
    /// exceed 130 characters on macOS, which is neither useful nor appropriate for a header sent on every
    /// request.
    /// </summary>
    private const int MaxDescriptionLength = 64;

    // OSPlatform.FreeBSD and the mobile/browser platforms are not available on all target frameworks
    // (netstandard2.0 exposes only Windows, Linux, and OSX), so they are created by name instead.
    private static readonly OSPlatform s_freeBsd = OSPlatform.Create("FREEBSD");
    private static readonly OSPlatform s_android = OSPlatform.Create("ANDROID");
    private static readonly OSPlatform s_iOS = OSPlatform.Create("IOS");
    private static readonly OSPlatform s_macCatalyst = OSPlatform.Create("MACCATALYST");
    private static readonly OSPlatform s_browser = OSPlatform.Create("BROWSER");

    private static readonly string s_packageVersion = GetPackageVersion(typeof(PlatformTelemetry).Assembly);
    private static readonly string s_runtimeVersion = GetRuntimeVersion(new TelemetryDetails.RuntimeInformationWrapper());
    private static readonly string s_operatingSystem = GetOperatingSystem(new TelemetryDetails.RuntimeInformationWrapper());
    private static readonly string s_architecture = GetArchitecture(new TelemetryDetails.RuntimeInformationWrapper());

    /// <summary>
    /// Determines whether the caller has opted out of SDK telemetry, which suppresses both these headers and
    /// the <c>User-Agent</c> header that the library would otherwise add.
    /// </summary>
    /// <remarks>
    /// This is intended to be evaluated once when a pipeline is built and cached by the caller, so that the
    /// request path remains a single field test. Configuration is read at that point rather than at type
    /// initialization, matching the behavior of the experimental OpenTelemetry switch.
    /// </remarks>
    public static bool IsTelemetryDisabled()
        => AppContextSwitchHelper.GetConfigValue(
            DisableTelemetrySwitchName,
            DisableTelemetryEnvironmentVariableName);

    /// <summary>
    /// Applies the platform metadata headers to <paramref name="request"/>, preserving any value that the
    /// caller has already set.
    /// </summary>
    /// <param name="request">The request to apply the headers to.</param>
    public static void ApplyTo(PipelineRequest request)
    {
        if (request?.Headers is null)
        {
            return;
        }

        SetIfAbsent(request, LangHeaderName, LangValue);
        SetIfAbsent(request, PackageVersionHeaderName, s_packageVersion);
        SetIfAbsent(request, RuntimeHeaderName, RuntimeValue);
        SetIfAbsent(request, RuntimeVersionHeaderName, s_runtimeVersion);
        SetIfAbsent(request, OSHeaderName, s_operatingSystem);
        SetIfAbsent(request, ArchHeaderName, s_architecture);
    }

    private static void SetIfAbsent(PipelineRequest request, string name, string value)
    {
        // Header lookups are case-insensitive, so this honors a caller-supplied value regardless of casing.
        if (!request.Headers.TryGetValue(name, out string? _))
        {
            request.Headers.Set(name, value);
        }
    }

    internal static string GetPackageVersion(Assembly assembly)
    {
        try
        {
            return NormalizePackageVersion(assembly?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion);
        }
        catch (Exception)
        {
            return UnknownValue;
        }
    }

    /// <summary>
    /// Produces the package version exactly as the <c>User-Agent</c> carries it, so that the two can never
    /// disagree.
    /// </summary>
    /// <param name="informationalVersion">The assembly informational version.</param>
    /// <remarks>
    /// Unlike the free-form platform values, this is a value that has to be reproduced verbatim rather than
    /// normalized; rewriting it would produce a header that looks like a plausible version but silently
    /// differs from the user agent. It is therefore validated rather than sanitized, and a value that could
    /// not be transmitted safely is reported as unknown rather than as a mangled near-miss.
    /// </remarks>
    internal static string NormalizePackageVersion(string? informationalVersion)
    {
        if (string.IsNullOrEmpty(informationalVersion))
        {
            return UnknownValue;
        }

        // Matches the user agent's handling; this is the only transformation either applies.
        int hashSeparator = informationalVersion!.LastIndexOf('+');
        string version = hashSeparator == -1
            ? informationalVersion
            : informationalVersion.Substring(0, hashSeparator);

        return IsHeaderSafe(version) ? version : UnknownValue;
    }

    /// <summary>
    /// Determines whether a value can be transmitted as an HTTP header value without alteration.
    /// </summary>
    /// <param name="value">The value to evaluate.</param>
    /// <remarks>
    /// Leading and trailing spaces are rejected because RFC 7230 section 3.2 treats them as optional
    /// whitespace surrounding the field value rather than as part of it, so an HTTP stack is free to strip
    /// them. Interior spaces are legal and are preserved, so they remain acceptable. Tabs and other control
    /// characters are covered by the printable-ASCII check.
    /// </remarks>
    private static bool IsHeaderSafe(string value)
    {
        if (value.Length == 0 || value[0] == ' ' || value[value.Length - 1] == ' ')
        {
            return false;
        }

        foreach (char character in value)
        {
            if (character < ' ' || character > '~')
            {
                return false;
            }
        }

        return true;
    }

    internal static string GetRuntimeVersion(TelemetryDetails.RuntimeInformationWrapper runtimeInformation)
    {
        try
        {
            string description = runtimeInformation.FrameworkDescription;

            if (string.IsNullOrEmpty(description))
            {
                return UnknownValue;
            }

            // FrameworkDescription is authoritative on every target framework and takes forms such as
            // ".NET 8.0.11", ".NET Core 3.1.32", ".NET Framework 4.8.9256.0", and "Mono 6.12.0". The version
            // is the trailing token. Environment.Version is deliberately not used, as it reports 4.0.30319.x
            // on .NET Framework regardless of the version actually installed.
            int lastSpace = description.LastIndexOf(' ');
            string version = lastSpace == -1 ? description : description.Substring(lastSpace + 1);

            if (string.IsNullOrEmpty(version) || !char.IsDigit(version[0]))
            {
                return Sanitize(description, UnknownValue);
            }

            return Sanitize(version, UnknownValue);
        }
        catch (Exception)
        {
            return UnknownValue;
        }
    }

    internal static string GetOperatingSystem(TelemetryDetails.RuntimeInformationWrapper runtimeInformation)
    {
        try
        {
            // Android and the Apple mobile platforms are evaluated before Linux and MacOS. Modern .NET
            // reports them as distinct platforms, but Mono and legacy Xamarin report Android as Linux, so
            // ordering keeps the result stable across runtimes.
            if (runtimeInformation.IsOSPlatform(s_android))
            {
                return "Android";
            }

            if (runtimeInformation.IsOSPlatform(s_iOS))
            {
                return "iOS";
            }

            if (runtimeInformation.IsOSPlatform(s_macCatalyst))
            {
                return "MacCatalyst";
            }

            if (runtimeInformation.IsOSPlatform(s_browser))
            {
                return "Browser";
            }

            if (runtimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }

            if (runtimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "MacOS";
            }

            if (runtimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }

            if (runtimeInformation.IsOSPlatform(s_freeBsd))
            {
                return "FreeBSD";
            }

            return $"Other:{Sanitize(runtimeInformation.OSDescription, UnknownValue)}";
        }
        catch (Exception)
        {
            return $"Other:{UnknownValue}";
        }
    }

    internal static string GetArchitecture(TelemetryDetails.RuntimeInformationWrapper runtimeInformation)
    {
        try
        {
            switch (runtimeInformation.ProcessArchitecture)
            {
                case Architecture.X64:
                    return "x64";
                case Architecture.X86:
                    return "x86";
                case Architecture.Arm64:
                    return "arm64";
                case Architecture.Arm:
                    return "arm";
                default:
                    // Values such as Wasm and RiscV64 are absent from the netstandard2.0 reference assembly
                    // but resolve against the running runtime's metadata, so ToString() yields the real name.
                    return $"other:{Sanitize(runtimeInformation.ProcessArchitecture.ToString(), UnknownValue)}";
            }
        }
        catch (Exception)
        {
            return $"other:{UnknownValue}";
        }
    }

    /// <summary>
    /// Reduces a value to something safe to transmit as an HTTP header: printable ASCII only, no line
    /// breaks, no leading or trailing spaces, and bounded in length.
    /// </summary>
    /// <param name="value">The value to sanitize.</param>
    /// <param name="fallback">The value to return when nothing usable remains.</param>
    internal static string Sanitize(string? value, string fallback)
    {
        if (string.IsNullOrEmpty(value))
        {
            return fallback;
        }

        StringBuilder builder = new(Math.Min(value!.Length, MaxDescriptionLength));

        foreach (char character in value)
        {
            if (builder.Length == MaxDescriptionLength)
            {
                break;
            }

            // Printable ASCII only. Anything else, including CR/LF and non-ASCII release names, becomes '_'
            // rather than being dropped, so that distinct platforms remain distinguishable.
            builder.Append(character >= ' ' && character <= '~' ? character : '_');
        }

        // Leading and trailing spaces are not part of an HTTP header field value; they are optional
        // whitespace that intermediaries are free to strip. Removing them here means the value that was
        // computed is the value that arrives, rather than one silently normalized in transit.
        while (builder.Length > 0 && builder[builder.Length - 1] == ' ')
        {
            builder.Length--;
        }

        int leadingSpaces = 0;

        while (leadingSpaces < builder.Length && builder[leadingSpaces] == ' ')
        {
            leadingSpaces++;
        }

        if (leadingSpaces > 0)
        {
            builder.Remove(0, leadingSpaces);
        }

        return builder.Length == 0 ? fallback : builder.ToString();
    }
}
