#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace OpenAI;

/// <summary>Configures X.509 workload identity authentication.</summary>
/// <remarks>
/// The application owns the handler, its client certificates, and its connection pool. Automatic
/// redirects and automatic cookies must be disabled before the handler is provided to the credential.
/// </remarks>
[Experimental("OPENAI001")]
public sealed class X509WorkloadIdentityCredentialOptions
{
    /// <summary>
    /// Gets or sets the caller-owned HTTP handler used for both token exchange and API requests.
    /// </summary>
    /// <remarks>
    /// <see cref="HttpClientHandler"/> is supported on every target framework. On .NET 8 and later,
    /// <see cref="SocketsHttpHandler"/> is also supported. Custom and delegating handlers are not
    /// accepted because their redirect behavior cannot be verified.
    /// </remarks>
    public HttpMessageHandler? Handler { get; set; }

    /// <summary>Gets or sets the desired refresh buffer. The default is five minutes.</summary>
    /// <remarks>The effective buffer is capped at half of the access token's lifetime.</remarks>
    public TimeSpan RefreshBuffer { get; set; } = TimeSpan.FromMinutes(5);
}
