# API-key + HTTP mTLS example

This example configures mTLS on .NET's native `SocketsHttpHandler` and passes the resulting `HttpClient` to the OpenAI SDK through `HttpClientPipelineTransport`. Keeping certificate handling in the HTTP transport preserves .NET's native certificate, proxy, and rotation capabilities without adding an SDK-specific mTLS option.

Before running it, follow the [OpenAI Mutual TLS Beta Program](https://help.openai.com/en/articles/10876024-openai-mutual-tls-beta-program) instructions to understand request authentication and enroll, upload, and activate a client certificate for your organization or project. To manage uploaded certificates programmatically, see the [Certificates API reference](https://developers.openai.com/api/reference/resources/admin/subresources/organization/subresources/certificates/).

## Certificate bundle

Set `OPENAI_CLIENT_PFX_PATH` to a PFX/PKCS#12 bundle containing:

1. Exactly one certificate with a private key: the leaf client certificate.
2. Every required intermediate certificate.

Do not include the root CA certificate; TLS peers establish trust from their own root stores. Do not rely on `X509Certificate2.CreateFromPemFile` for a certificate-chain PEM: it loads only the first certificate. A PFX bundle keeps the leaf, private key, and intermediates together for `SslStreamCertificateContext`, which ensures the TLS handshake can present the complete chain.

## Endpoint

Set `OPENAI_BASE_URL` to the base URL that matches your data residency:

| Scope | Base URL |
| --- | --- |
| Global (default) | `https://mtls.api.openai.com/v1` |
| EU Data Residency | `https://mtls-eu.api.openai.com/v1` |

`OPENAI_BASE_URL` must be an absolute HTTPS URI.

## Run the example

From PowerShell at the repository root, set the environment variables for the
current process and run the explicit example test:

```powershell
$env:OPENAI_API_KEY = "sk-..."
$env:OPENAI_CLIENT_PFX_PATH = "C:\path\to\client-chain.pfx"
$env:OPENAI_CLIENT_PFX_PASSWORD = "optional-password"

dotnet test .\examples\OpenAI.Examples.csproj `
  --framework net10.0 `
  -- NUnit.Where="test == 'OpenAI.Examples.MutualTlsExamples.Example01_MutualTlsAsync'"
```

The example calls the supported Chat Completions endpoint and prints its text
response. For EU Data Residency, set:

```powershell
$env:OPENAI_BASE_URL = "https://mtls-eu.api.openai.com/v1"
```

The handler disables automatic redirects so the certificate-bearing transport does not follow a redirect to another host. Keep the `HttpClient` alive for the lifetime of the SDK client. When rotating the certificate, create a new handler, transport, and SDK client; an existing pooled TLS connection will not renegotiate with replacement credentials.

## End-to-end testing

The automated mTLS tests create a private root, intermediate, server, and client
certificate for each test. They start a loopback TLS server that requires a
trusted client certificate, send a request through the real
`SocketsHttpHandler`, `HttpClientPipelineTransport`, and OpenAI Chat client, and
verify the TLS chain, API key, HTTP request, and deserialized SDK response. They
also verify that missing or untrusted client credentials fail closed and that
the certificate-bearing handler does not follow redirects.

Run these deterministic tests from PowerShell:

```powershell
dotnet test .\tests\OpenAI.Tests.csproj `
  --filter "TestCategory=MutualTls"
```

The explicit example test above is the live end-to-end check against OpenAI. It
requires an API key and a client certificate whose CA has been uploaded and
activated for the same organization or project, so it is not suitable for
credential-free pull request CI.

This example covers API-key + HTTP mTLS only. It does not configure Realtime WebSocket connections or X.509 workload identity federation.
