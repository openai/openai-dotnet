using System;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

#nullable enable

namespace OpenAI;

/// <summary>
/// Details about the package to be included in UserAgent telemetry
/// </summary>
internal class TelemetryDetails
{
    private const int MaxApplicationIdLength = 24;
    private readonly string _userAgent;

    /// <summary>
    /// The package type represented by this <see cref="TelemetryDetails"/> instance.
    /// </summary>
    public Assembly Assembly { get; }

    /// <summary>
    /// The value of the applicationId used to initialize this <see cref="TelemetryDetails"/> instance.
    /// </summary>
    public string? ApplicationId { get; }

    /// <summary>
    /// Initialize an instance of <see cref="TelemetryDetails"/> by extracting the name and version information from the <see cref="System.Reflection.Assembly"/> associated with the <paramref name="assembly"/>.
    /// </summary>
    /// <param name="assembly">The <see cref="System.Reflection.Assembly"/> used to generate the package name and version information for the <see cref="TelemetryDetails"/> value.</param>
    /// <param name="applicationId">An optional value to be prepended to the <see cref="TelemetryDetails"/>.
    internal TelemetryDetails(Assembly assembly, string? applicationId = null)
        : this(assembly, applicationId, new RuntimeInformationWrapper())
    { }

    internal TelemetryDetails(Assembly assembly, string? applicationId = null, RuntimeInformationWrapper? runtimeInformation = default)
    {
        Argument.AssertNotNull(assembly, nameof(assembly));
        if (applicationId?.Length > MaxApplicationIdLength)
        {
            throw new ArgumentOutOfRangeException(nameof(applicationId), $"{nameof(applicationId)} must be shorter than {MaxApplicationIdLength + 1} characters");
        }

        Assembly = assembly;
        ApplicationId = applicationId;
        _userAgent = GenerateUserAgentString(assembly, applicationId, runtimeInformation);
    }

    internal static string GenerateUserAgentString(Assembly clientAssembly, string? applicationId = null, RuntimeInformationWrapper? runtimeInformation = default)
    {
        AssemblyInformationalVersionAttribute? versionAttribute
            = clientAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?? throw new InvalidOperationException(
                    $"{nameof(AssemblyInformationalVersionAttribute)} is required on client SDK assembly '{clientAssembly.FullName}'.");

        string version = versionAttribute.InformationalVersion;

        string assemblyName = clientAssembly.GetName().Name!;

        int hashSeparator = version.LastIndexOf('+');
        if (hashSeparator != -1)
        {
            version = version.Substring(0, hashSeparator);
        }
        runtimeInformation ??= new RuntimeInformationWrapper();
        var platformInformation = EscapeProductInformation($"({runtimeInformation.FrameworkDescription}; {runtimeInformation.OSDescription})");

        return applicationId != null
            ? $"{applicationId} {assemblyName}/{version} {platformInformation}"
            : $"{assemblyName}/{version} {platformInformation}";
    }

    /// <summary>
    /// The properly formatted UserAgent string based on this <see cref="TelemetryDetails"/> instance.
    /// </summary>
    public override string ToString() => _userAgent;

    /// <summary>
    /// If the ProductInformation is not in the proper format, this escapes any ')' , '(' or '\' characters per https://www.rfc-editor.org/rfc/rfc7230#section-3.2.6
    /// </summary>
    /// <param name="productInfo">The ProductInfo portion of the UserAgent</param>
    /// <returns></returns>
    private static string EscapeProductInformation(string productInfo)
    {
        // If the string is already valid, we don't need to escape anything
        bool success = false;
        try
        {
            success = ProductInfoHeaderValue.TryParse(productInfo, out var _);
        }
        catch (Exception)
        {
            // Invalid values can throw in Framework due to https://github.com/dotnet/runtime/issues/28558
            // Treat this as a failure to parse.
        }
        if (success)
        {
            return productInfo;
        }

        var sb = new StringBuilder(productInfo.Length + 2);
        sb.Append('(');
        // exclude the first and last characters, which are the enclosing parentheses
        for (int i = 1; i < productInfo.Length - 1; i++)
        {
            char c = productInfo[i];
            if (c == ')' || c == '(')
            {
                sb.Append('\\');
            }
            // If we see a \, we don't need to escape it if it's followed by a '\', '(', or ')', because it is already escaped.
            else if (c == '\\')
            {
                if (i + 1 < (productInfo.Length - 1))
                {
                    char next = productInfo[i + 1];
                    if (next == '\\' || next == '(' || next == ')')
                    {
                        sb.Append(c);
                        sb.Append(next);
                        i++;
                        continue;
                    }
                    else
                    {
                        sb.Append('\\');
                    }
                }
                else
                {
                    sb.Append('\\');
                }
            }
            sb.Append(c);
        }
        sb.Append(')');
        return sb.ToString();
    }

    internal class RuntimeInformationWrapper
    {
        public virtual string FrameworkDescription => RuntimeInformation.FrameworkDescription;
        public virtual string OSDescription => RuntimeInformation.OSDescription;
        public virtual Architecture OSArchitecture => RuntimeInformation.OSArchitecture;
        public virtual Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;
        public virtual bool IsOSPlatform(OSPlatform osPlatform) => RuntimeInformation.IsOSPlatform(osPlatform);
    }
}